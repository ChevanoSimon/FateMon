using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour
{
    [SerializeField] Text nameText = default;
    [SerializeField] Text levelText = default;
    [SerializeField] HPBar hpBar = default;

    [SerializeField] Color highlightedColor = default;

    Servant _servant;

    public void SetData(Servant servantInfo)
    {
        _servant = servantInfo;

        nameText.text = servantInfo.servantBase.Name;
        levelText.text = "Lvl " + servantInfo.Level;
        hpBar.SetHP((float)servantInfo.HP / servantInfo.MaxHP);
    }

    public void SetSelected(bool selected)
    {
        if (selected)
            nameText.color = highlightedColor;
        else
            nameText.color = Color.black;
    }

}
