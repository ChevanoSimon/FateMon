using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerBattleUnit : MonoBehaviour
{ 
    [SerializeField] bool isPlayerUnit = default;
    [SerializeField] BattleHUDUpdate hud;
    public bool IsPlayerUnit
    {
        get { return isPlayerUnit;  }
    }

    public BattleHUDUpdate Hud
    {
        get { return hud; }
    }

    public Servant servantInfo { get; set; }
  
    Image image;
    Vector3 orignalPos;
    Color originalColor;

    private void Awake()
    {
        image = GetComponent<Image>();
        orignalPos = image.transform.localPosition;
        originalColor = image.color;
    }



    public void placementSetUp(Servant servant)
    {
        servantInfo = servant;
        if (isPlayerUnit)
            image.sprite = servantInfo.servantBase.BackSprite;
        else
            image.sprite = servantInfo.servantBase.FrontSprite;

        hud.SetData(servant);

        image.color = originalColor;

        PlayEnterAnimation();
    }

    public void PlayEnterAnimation()
    {
        if (isPlayerUnit)
            image.transform.localPosition = new Vector3(-315f, orignalPos.y);
        else
            image.transform.localPosition = new Vector3(500f, orignalPos.y);

        image.transform.DOLocalMoveX(orignalPos.x, 1f);
    }

    //To be reworked
    public void PlayReturnAnimation()
    {
        if (isPlayerUnit)
            image.transform.localPosition = new Vector3(-315f, orignalPos.y + 315);
        else
            image.transform.localPosition = new Vector3(-500f, orignalPos.y);

        image.transform.DOLocalMoveX(orignalPos.x, 1f);
    }

    public void PlayAttackAnimation()
    {
        var sequence = DOTween.Sequence();
        if (isPlayerUnit)
            sequence.Append(image.transform.DOLocalMoveX(orignalPos.x + 50f, 0.25f));
        else
            sequence.Append(image.transform.DOLocalMoveX(orignalPos.x - 50f, 0.25f));

        sequence.Append(image.transform.DOLocalMoveX(orignalPos.x, 0.25f));
    }

    public void PlayHitAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOColor(Color.gray, 0.1f));
        sequence.Append(image.DOColor(originalColor, 0.1f));
        sequence.Append(image.DOColor(Color.gray, 0.1f));
        sequence.Append(image.DOColor(originalColor, 0.1f));
    }

    public void PlayFaintAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.transform.DOLocalMoveY(orignalPos.y - 150f, 0.5f));
        sequence.Join(image.DOFade(0f, 0.5f));
    }









}
