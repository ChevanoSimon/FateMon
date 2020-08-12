using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUDUpdate : MonoBehaviour
{
    [SerializeField] Text nameText = default;
    [SerializeField] Text levelText = default;
    [SerializeField] HPBar hpBar = default;

    Servant _servant;

    public void SetData(Servant servantInfo)
    {
        _servant = servantInfo;

        nameText.text = servantInfo.servantBase.Name;
        levelText.text = "Lvl " + servantInfo.Level;
        hpBar.SetHP((float)servantInfo.HP / servantInfo.MaxHP);
    }

    public IEnumerator UpdateHP()
    {
       yield return hpBar.SetHPSmooth((float)_servant.HP / _servant.MaxHP);
    }



}
