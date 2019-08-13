using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knob : MonoBehaviour
{
    public Transform dotTrans;
    SphereCollider sphereCollider;

    void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();

        sphereCollider.center = dotTrans.localPosition;
    }

    void Update()
    {

    }
}
