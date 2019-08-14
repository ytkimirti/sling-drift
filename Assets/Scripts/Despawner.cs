using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZ_Pooling;

public class Despawner : MonoBehaviour
{
    public float secondsLeftToDespawn;

    void Start()
    {
        StartCoroutine(DespawnEnum());
    }

    public void StopEnum()
    {
        StopCoroutine(DespawnEnum());

    }

    void OnDespawned()
    {
        StopEnum();
    }

    IEnumerator DespawnEnum()
    {
        yield return new WaitForSeconds(secondsLeftToDespawn);

        EZ_PoolManager.Despawn(transform);
    }

    void Update()
    {

    }
}
