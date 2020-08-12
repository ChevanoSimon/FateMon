using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using UnityEngine;

public class HPBar : MonoBehaviour
{
    [SerializeField] GameObject health = default;

    public void SetHP(float hpNormalized) 
    {
        health.transform.localScale = new Vector3(hpNormalized, 1f);

    }

    public IEnumerator SetHPSmooth (float newHp)
    {
        float currentHp = health.transform.localScale.x;
        float changeAmount = currentHp - newHp;

        while (currentHp - newHp > Mathf.Epsilon)
        {
            currentHp -= changeAmount * Time.deltaTime;
            health.transform.localScale = new Vector3(currentHp, 1f);
            yield return null;
        }

        health.transform.localScale = new Vector3(newHp, 1f); 

    }

}
