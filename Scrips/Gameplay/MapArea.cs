using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    [SerializeField] List<Servant> wildServants = default;

    public Servant GetRandomWildServant()
    {
        var wildServant = wildServants[Random.Range(0, wildServants.Count)];
        wildServant.Init();
        return wildServant;
    }

}
