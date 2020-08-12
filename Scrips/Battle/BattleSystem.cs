using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState { Start, ActionSelection, MoveSelection, PerformMove, Busy, PartyScreen, BattleOver }

public class BattleSystem : MonoBehaviour
{

    [SerializeField] PlayerBattleUnit playerUnit = default;
    [SerializeField] PlayerBattleUnit enemyUnit = default;
    [SerializeField] BattleDialogueBox dialogBox = default;
    [SerializeField] PartyScreen partyScreen = default;

    public event Action<bool> onBattleOver;

    BattleState state;
    int currentAction;
    int currentMove;
    int currentMember;

    ServantParty servantParty;
    Servant wildServant;

    public void StartBattle(ServantParty servantParty, Servant wildServant)
    {
        this.servantParty = servantParty;
        this.wildServant = wildServant;

        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        playerUnit.placementSetUp(servantParty.GetHealthyServant());

        enemyUnit.placementSetUp(wildServant);

        partyScreen.Init();

        dialogBox.SetMoveNames(playerUnit.servantInfo.Moves);

        yield return dialogBox.TypeDialog($"A wild {enemyUnit.servantInfo.servantBase.Name} appeared.");

        ActionSelection();
    }

    void BattleOver(bool won)
    {
        state = BattleState.BattleOver;
        onBattleOver(won);
    }


    void ActionSelection()
    {
        state = BattleState.ActionSelection;
        dialogBox.SetDialog("Choose an action");
        dialogBox.EnableActionSelector(true);
    }

    void OpenPartyScreen()
    {
        state = BattleState.PartyScreen;
        partyScreen.SetPartyData(servantParty.Servants);
        partyScreen.gameObject.SetActive(true);
    }

    void MoveSelection()
    {
        state = BattleState.MoveSelection;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }

    IEnumerator PlayerMove()
    {
        state = BattleState.PerformMove;

        var move = playerUnit.servantInfo.Moves[currentMove];
        yield return RunMove(playerUnit, enemyUnit, move);
        
        // If the battle was not changed by RunMove, then go to next step
        if (state == BattleState.PerformMove)
            StartCoroutine(EnemyMove());
    }

    IEnumerator EnemyMove()
    {
        state = BattleState.PerformMove;

        var move = enemyUnit.servantInfo.GetRandomMove();
        yield return RunMove(enemyUnit, playerUnit, move);

        // If the battle was not changed by RunMove, then go to next step
        if (state == BattleState.PerformMove)
            ActionSelection();
    }

    IEnumerator RunMove(PlayerBattleUnit sourceUnit, PlayerBattleUnit targetUnit, MoveBehaviour move)
    {
        move.UsageMove--;
        yield return dialogBox.TypeDialog($"{sourceUnit.servantInfo.servantBase.Name} used {move.Base.Name}");

        sourceUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);

        targetUnit.PlayHitAnimation();

        var damageDetails = targetUnit.servantInfo.TakeDamage(move, sourceUnit.servantInfo);
        yield return targetUnit.Hud.UpdateHP();
        yield return ShowDamageDetails(damageDetails);

        if (damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog($"{targetUnit.servantInfo.servantBase.Name} Fainted");
            targetUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);

            CheckForBattleOver(targetUnit);
        }
    }

    void CheckForBattleOver(PlayerBattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayerUnit)
        {
            var nextServant = servantParty.GetHealthyServant();
            if (nextServant != null)
            {
                OpenPartyScreen();
            }
            else
            {
                BattleOver(false);
            }
        }
        else
            BattleOver(true);
    }


    IEnumerator ShowDamageDetails(Servant.DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
            yield return dialogBox.TypeDialog("It's a critical hit!");

        if (damageDetails.Type > 1f)
            yield return dialogBox.TypeDialog("It's super effective!");

        if (damageDetails.Type == 0f)
            yield return dialogBox.TypeDialog("It's not effetive at all...!");

        else if (damageDetails.Type < 1f)
            yield return dialogBox.TypeDialog("It's not very effective...");
    }

    public void HandleUpdate() 
    { 
        if ( state == BattleState.ActionSelection)
        {
            HandleActionSelection();
        }
        else if ( state == BattleState.MoveSelection)
        {
            HandleMoveSelection();
        }
        else if (state == BattleState.PartyScreen)
        {
            HandlePartySelection();
        }
    }

    void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentAction;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentAction;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentAction += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentAction -= 2;

        currentAction = Mathf.Clamp(currentAction, 0, 3);

            dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if ( currentAction == 0 )
            {
                // Fight
                MoveSelection();
            }

            else if ( currentAction == 1)
            {
                // Inventory
            }

            else if (currentAction == 2)
            {
                // ServantParty
                OpenPartyScreen();
            }

            else if (currentAction == 3)
            {
                // Run
            }
        }

    }

    void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentMove;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentMove;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentMove += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentMove -= 2;

        currentMove = Mathf.Clamp(currentMove, 0, playerUnit.servantInfo.Moves.Count - 1);

        dialogBox.UpdateMoveSelection(currentMove, playerUnit.servantInfo.Moves[currentMove]);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(PlayerMove());
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            ActionSelection();
        }
    }

    void HandlePartySelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentMember;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentMember;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentMember += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentMember -= 2;

        currentMember = Mathf.Clamp(currentMember, 0, servantParty.Servants.Count - 1);

        partyScreen.UpdateMemberSelection(currentMember);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            var selectedMember = servantParty.Servants[currentMember];
            if (selectedMember.HP <= 0)
            {
                partyScreen.SetMessageText("You cannot send out a defeated Servant");
                return;
            }
            if (selectedMember == playerUnit.servantInfo)
            {
                partyScreen.SetMessageText("You cannot switch with the same Servant");
                return;
            }

            partyScreen.gameObject.SetActive(false);
            state = BattleState.Busy;
            StartCoroutine(SwitchServant(selectedMember));
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            partyScreen.gameObject.SetActive(false);
            ActionSelection();
        }
    }

    IEnumerator SwitchServant(Servant newServant)
    {   
        if (playerUnit.servantInfo.HP > 0) 
        { 
            yield return dialogBox.TypeDialog($"Retreat {playerUnit.servantInfo.servantBase.Name}");
            playerUnit.PlayFaintAnimation();

            // To be reworked. 
            // playerUnit.PlayReturnAnimation();
            yield return new WaitForSeconds(2f);
        }
        playerUnit.placementSetUp(newServant);

        dialogBox.SetMoveNames(newServant.Moves);

        yield return dialogBox.TypeDialog($"Go {newServant.servantBase.Name}!");

        StartCoroutine(EnemyMove());
    }

}
