using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using DG.Tweening;

public class Player : MonoBehaviour
{

    public float moveSpeed;
    public float accelerateSpeed;

    [Header("Connection")]
    public Knob currKnob;
    public bool isMakingConnection;
    float memDistance;
    HingeJoint joint;

    [Header("References")]

    public Transform lineCenterTrans;
    public Transform visual;
    public LineRenderer lineRen;

    [HideInInspector]
    public Rigidbody rb;

    bool isTookOff;

    public static Player main;

    private void Awake()
    {
        main = this;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            visual.transform.position = transform.position;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Knob")
        {
            currKnob = other.gameObject.GetComponent<Knob>();

            if (!currKnob)
            {
                Debug.LogError("Somehow this knob doesn't have the script or something is wrong :/");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Knob" && currKnob)
        {
            currKnob = null;
            RemoveConnection();
        }
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

        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (currKnob)
                MakeConnection(currKnob);
        }
        else
        {
            if (isMakingConnection)
                RemoveConnection();
        }

        if (isMakingConnection && currKnob)
        {

            UpdateLineRenderer(currKnob.dotTrans.position);

            float currDistance = hardDist(currKnob.dotTrans.position);

            print(string.Format("MemDist: {0} CurrentDist: {1}", memDistance, currDistance));

            if (currDistance > memDistance)
            {
                AddJoint(currKnob.dotTrans.position);
            }

            memDistance = hardDist(currKnob.dotTrans.position);
        }
    }

    void MakeConnection(Knob knob)
    {
        if (isMakingConnection)
            return;

        currKnob = knob;
        isMakingConnection = true;

        memDistance = hardDist(knob.dotTrans.position);
    }

    float hardDist(Vector3 pos)
    {
        return (pos.ToVector2() - (transform.position.ToVector2())).magnitude;
    }

    void UpdateLineRenderer(Vector3 pos)
    {
        if (isMakingConnection)
        {
            lineRen.enabled = true;

            lineRen.SetPosition(0, lineCenterTrans.position);
            lineRen.SetPosition(1, pos);
        }
        else
        {
            lineRen.enabled = false;
        }
    }

    void RemoveConnection()
    {
        isMakingConnection = false;

        memDistance = 0;
        UpdateLineRenderer(Vector3.zero);

        RemoveJoint();
    }

    void AddJoint(Vector3 anchor)
    {
        if (joint)
            return;

        joint = gameObject.AddComponent<HingeJoint>();

        Vector3 localAnchor = anchor - transform.position;

        joint.axis = Vector3.up;
        joint.anchor = localAnchor;
        joint.enablePreprocessing = false;
    }

    void RemoveJoint()
    {
        if (!joint)
            return;

        Destroy(joint);

        joint = null;
    }
}
