using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knob : MonoBehaviour
{
    public bool isLeft;

    [Space]

    public Transform visualDotTrans;
    public Transform jointDotTrans;
    public Transform correctionEnablerTrans;
    public Transform roadTrans;
    public SphereCollider sphereCollider;

    [HideInInspector]
    public Vector2 turnDir;


    void Start()
    {
        turnDir = transform.right;

        UpdateDirection();
    }

    void OnValidate()
    {
        UpdateDirection();
    }

    void UpdateDirection()
    {
        if (isLeft)
        {
            if (roadTrans.localScale.x > 0)
            {
                InverseEverything();
            }
        }
        else
        {
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
        trans.localPosition = new Vector3(-trans.localPosition.x, 0, trans.localPosition.z);
    }

    void Update()
    {

    }
}
