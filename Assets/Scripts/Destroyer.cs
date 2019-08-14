using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    public float secondsLeftToBeDestroyed;

    void Start()
    {
        StartCoroutine(DestroyEnum());
    }

    public void StopEnum()
    {
        StopCoroutine(DestroyEnum());
    }

    void OnDespawned()
    {
        StopEnum();
    }

    IEnumerator DestroyEnum()
    {
        yield return new WaitForSeconds(secondsLeftToBeDestroyed);

        Destroy(gameObject);
    }

    void Update()
    {

    }
}
