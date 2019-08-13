using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knob : MonoBehaviour
{
    public Transform visualDotTrans;
    public Transform jointDotTrans;
    SphereCollider sphereCollider;

    void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();

        sphereCollider.center = jointDotTrans.localPosition;
    }

    void Update()
    {

    }
}
