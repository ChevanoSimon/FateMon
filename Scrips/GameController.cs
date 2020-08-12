using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { FreeRoam, Battle }

public class GameController : MonoBehaviour
{

    [SerializeField] PlayerController playerController = default;
    [SerializeField] BattleSystem battleSystem = default;
    [SerializeField] Camera worldCamera = default;

    GameState state;

    private void Start()
    {
        playerController.onEncounter += StartBattle;
        battleSystem.onBattleOver += EndBattle;
    }

    void StartBattle()
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        var servantParty = playerController.GetComponent<ServantParty>();
        var wildServant = FindObjectOfType<MapArea>().GetComponent<MapArea>().GetRandomWildServant();

        battleSystem.StartBattle(servantParty, wildServant);
    }

    void EndBattle(bool won)
    {
        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
    }


    public void Update()
    {
        if (state == GameState.FreeRoam)
        {
            playerController.HandleUpdate();
        }

        else if (state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }


    }


}
