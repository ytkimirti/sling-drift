using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using DG.Tweening;

public class Player : MonoBehaviour
{

    public float moveSpeed;
    public float accelerateSpeed;

    [Header("Connection & Turning")]
    public Knob currKnob;
    public bool isMakingConnection;
    public float minJointDistance;//The distance it takes to just get away from the knob that it creates a joint
    float memDistance;
    HingeJoint joint;

    [Space]

    public float correctingTorque;
    public float correctingForce;
    [ReadOnly]
    public bool isCorrectingDirection;
    [ReadOnly]
    public Vector2 correctDirVec; //The rough correct direction, this will be used to correct the rotation of the car
    float targetAngle;
    float releaseAccuracy;

    [Header("References")]

    public Transform lineCenterTrans;
    public Transform visual;
    public Transform visualCenterOfMass;
    public HingeJoint visualHingeJoint;
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

        visual.GetComponent<Rigidbody>().centerOfMass = visualCenterOfMass.localPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        string tag = other.tag;

        if (tag == "Knob")
        {
            currKnob = other.gameObject.GetComponent<Knob>();

            if (!currKnob)
            {
                Debug.LogError("Somehow this knob doesn't have the script or something is wrong :/");
            }
        }
        else if (tag == "DirectionTriggerGuide")
        {
            correctDirVec = other.transform.forward.ToVector2();

            //Rounding operations for avoiding floating number problems
            if (Mathf.Abs(correctDirVec.x) < 0.5f) correctDirVec.x = 0;
            if (Mathf.Abs(correctDirVec.y) < 0.5f) correctDirVec.y = 0;

            targetAngle = other.transform.eulerAngles.y;

            print("The angle is " + targetAngle);
            isCorrectingDirection = true;
        }
        else if (tag == "DirectionTriggerDisabler")
        {
            correctDirVec = Vector3.zero;
            isCorrectingDirection = false;
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

            UpdateLineRenderer(currKnob.visualDotTrans.position);

            float currDistance = hardDist(currKnob.jointDotTrans.position);

            float diff = (currDistance - memDistance);

            //print(string.Format("DIFF: {0} MemDist: {1} CurrentDist: {2}", diff, memDistance, currDistance));

            if (diff > minJointDistance)
            {
                //print("GOT IN YEAAA");
                AddJoint(currKnob.jointDotTrans.position);
            }
            else if (currDistance < memDistance)
            {
                memDistance = currDistance;
            }
        }
    }

    private void FixedUpdate()
    {
        if (isCorrectingDirection && !isMakingConnection)
        {
            //Velocity correction

            Vector3 correctDirVec3 = correctDirVec.ToVector3();

            //rb.velocity = Vector3.Scale(rb.velocity, correctDirVec3);

            //Angular correction

            float deltaAngle = Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle);

            //print(string.Format("Delta Angle: {0} Torque: {1} Target: {2}", deltaAngle, transform.eulerAngles.y, targetAngle + 90));

            rb.angularVelocity = Vector3.up * deltaAngle * correctingTorque;
        }
    }

    void MakeConnection(Knob knob)
    {
        if (isMakingConnection)
            return;

        currKnob = knob;
        isMakingConnection = true;

        memDistance = hardDist(knob.jointDotTrans.position);
    }

    float hardDist(Vector3 pos)
    {
        //return (pos.ToVector2() - (transform.position.ToVector2())).magnitude;
        return Vector3.Distance(transform.position, pos);
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

        //print("Created a joint yeaa");

        joint = gameObject.AddComponent<HingeJoint>();

        Vector3 localAnchor = transform.InverseTransformPoint(anchor);

        joint.axis = Vector3.up;
        joint.anchor = localAnchor;
        joint.enablePreprocessing = false;

        float visualTurnAngle = currKnob.isLeft ? -30 : 30;

        JointSpring hingeSpring = visualHingeJoint.spring;

        hingeSpring.targetPosition = visualTurnAngle;

        visualHingeJoint.spring = hingeSpring;

    }

    void RemoveJoint()
    {
        if (!joint)
            return;

        Destroy(joint);

        JointSpring hingeSpring = visualHingeJoint.spring;

        hingeSpring.targetPosition = 0;

        visualHingeJoint.spring = hingeSpring;

        joint = null;
    }
}
