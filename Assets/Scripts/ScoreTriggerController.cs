using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScoreTriggerController : MonoBehaviour
{
    public Knob knob;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            knob.OnPlayerScored();
        }
    }
}
