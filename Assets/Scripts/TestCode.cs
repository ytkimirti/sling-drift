using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCode : MonoBehaviour
{
    public float velocity;

    void Start()
    {

    }

    void Update()
    {
        GetComponent<Rigidbody>().velocity = new Vector3(Input.GetAxis("Horizontal") * velocity, 0,
                                                         Input.GetAxis("Vertical") * velocity);
    }
}
