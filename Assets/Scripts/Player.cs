using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using DG.Tweening;

public class Player : MonoBehaviour
{

    public float moveSpeed;
    public float accelerateSpeed;

    [HideInInspector]
    public Rigidbody rb;

    bool isTookOff;

    public static Player main;

    private void Awake()
    {
        main = this;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!isTookOff && rb.velocity.z < moveSpeed)
        {
            rb.velocity = rb.velocity + (Vector3.forward * Time.deltaTime * accelerateSpeed);

            if (rb.velocity.z > moveSpeed)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0, moveSpeed);
                isTookOff = true;
            }
        }


    }
}
