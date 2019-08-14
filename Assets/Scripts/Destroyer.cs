using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    public float secondsLeftToBeDestroyed;
    bool startedEnum;


    public void StopEnum()
    {
        StopCoroutine(DestroyEnum());
    }

    IEnumerator DestroyEnum()
    {
        yield return new WaitForSeconds(secondsLeftToBeDestroyed);

        Destroy(gameObject);
    }

    void Update()
    {
        if (GameManager.main.isGameStarted && !startedEnum)
        {
            startedEnum = true;
            StartCoroutine(DestroyEnum());
        }

    }
}
