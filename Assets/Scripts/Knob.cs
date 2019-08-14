using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZ_Pooling;

public class Knob : MonoBehaviour
{
    public bool isLeft;

    [Space]

    public Transform spawnerEndTrans;
    public Transform visualDotTrans;
    public Transform jointDotTrans;
    public Transform correctionEnablerTrans;
    public Transform roadTrans;
    public SphereCollider sphereCollider;

    [HideInInspector]
    public Vector2 turnDir;


    void Start()
    {
        //UpdateDirection();
    }

    void OnValidate()
    {
        UpdateDirection();
    }

    public void UpdateDirection()
    {
        if (isLeft)
        {
            turnDir = -transform.right;

            if (roadTrans.localScale.x > 0)
            {
                InverseEverything();
            }
        }
        else
        {
            turnDir = transform.right;

            if (roadTrans.localScale.x < 0)
            {
                InverseEverything();
            }
        }
    }

    void InverseEverything()
    {
        turnDir = -turnDir;
        roadTrans.localScale = new Vector3(-roadTrans.localScale.x, 1, 1);
        sphereCollider.center = -sphereCollider.center;
        InverseX(visualDotTrans);
        InverseX(correctionEnablerTrans);
        correctionEnablerTrans.forward = -correctionEnablerTrans.forward;
    }

    void InverseX(Transform trans)
    {
        trans.localPosition = new Vector3(-trans.localPosition.x, trans.localPosition.y, trans.localPosition.z);
    }

    void Update()
    {

    }
}
