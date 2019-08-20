using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using DG.Tweening;
using TMPro;
using EZCameraShake;

public class Player : MonoBehaviour
{
    public bool isDed = false;

    public Vector2 currVel;
    public Vector2 targetVel;
    public float accelSpeed;

    [Header("Boost")]

    public bool isBoosting;
    public float boostExtraSpeed;

    [Header("Perfect")]

    public int currPerfectStreak;
    public float perfectAccuracy;

    [Header("Hardness")]
    public float defMoveSpeed;
    public float moveSpeedIncreasePerScore;
    public float maxMoveSpeed;
    float moveSpeed;

    [Header("Connection & Turning")]
    public Knob currKnob;
    public bool isMakingConnection;
    public float minJointDistance;//The distance it takes to just get away from the knob that it creates a joint
    //public float turningSpeed;
    float memDistance;
    bool isJointed;
    float jointDistance;
    float jointAngle;

    [Header("Turning Speed")]
    public float baseDist = 12;
    public float speedMult = 1;
    float currJointSpeed;

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
    public LineRenderer lineRen;
    public Animator nextStageTextAnim;
    public Animator perfectTextAnim;
    public TextMeshProUGUI perfectText;

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

        correctDirVec = Vector2.up;

        SetMoveSpeed();
    }

    public void SetMoveSpeed()
    {
        moveSpeed = defMoveSpeed + (GameManager.main.currScore * moveSpeedIncreasePerScore);

        if (moveSpeed > maxMoveSpeed)
            moveSpeed = maxMoveSpeed;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Border")
        {
            Die();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!GameManager.main.isGameStarted)
            return;

        string tag = other.tag;

        if (tag == "Knob")
        {
            if (currKnob)
            {
                RemoveConnection();
            }

            currKnob = other.gameObject.GetComponent<Knob>();

            if (!currKnob)
            {
                Debug.LogError("Somehow this knob doesn't have the script or something is wrong :/");
            }
        }
        else if (tag == "BoostStart")
        {
            nextStageTextAnim.SetTrigger("NextStage");
            GameManager.main.ChangeGroundColor();
            isBoosting = true;
        }
        else if (tag == "BoostEnd")
        {
            isBoosting = false;
        }
        else if (tag == "DirectionTriggerGuide")
        {
            correctDirVec = other.transform.forward.ToVector2();

            //Rounding operations for avoiding floating number problems
            if (Mathf.Abs(correctDirVec.x) < 0.5f) correctDirVec.x = 0;
            if (Mathf.Abs(correctDirVec.y) < 0.5f) correctDirVec.y = 0;

            targetAngle = other.transform.eulerAngles.y;

            isCorrectingDirection = true;

            LevelSpawner.main.OnKnobFinished();

            //CameraShaker.Instance.ShakeOnce(1, 10, 0, 0.2f);
        }
        else if (tag == "DirectionTriggerDisabler")
        {
            correctDirVec = Vector3.zero;
            isCorrectingDirection = false;
            releaseAccuracy = 0;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Knob" && currKnob && other.GetComponent<Knob>() == currKnob)
        {
            RemoveConnection();
            currKnob = null;
        }
    }

    void PerfectMade()
    {
        CameraShaker.Instance.ShakeOnce(1.3f, 10, 0, 0.3f);

        currPerfectStreak++;
        perfectTextAnim.SetTrigger("PerfectText");

        perfectText.text = "Perfect x" + currPerfectStreak.ToString();
    }

    void PerfectLost()
    {
        currPerfectStreak = 0;
        //perfectTextAnim.SetTrigger("PerfectLost");
    }

    void Update()
    {
        if (!isDed && GameManager.main.isGameStarted)
        {
            if (isJointed)
                UpdatePosToKnob(currKnob.jointDotTrans.position, jointDistance, jointAngle);

            MoveCar();
            CorrectDirection();
        }

        if (isCorrectingDirection && !isMakingConnection && releaseAccuracy == 0)
        {
            releaseAccuracy = Mathf.DeltaAngle(transform.localEulerAngles.y, targetAngle) - 3;
            print(releaseAccuracy);

            if (Mathf.Abs(releaseAccuracy) < perfectAccuracy)
            {
                PerfectMade();
            }
            else
            {
                PerfectLost();
            }
        }

        if (!isDed && GameManager.main.isGameStarted && Input.GetKey(KeyCode.Mouse0))
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
                AddJoint(currDistance);
            }
            else if (currDistance < memDistance)
            {
                memDistance = currDistance;
            }
        }
    }

    private void FixedUpdate()
    {

    }

    void MoveCar()
    {
        if (isJointed)
        {
            jointAngle += currJointSpeed * Time.deltaTime * (currKnob.isLeft ? 1 : -1);
        }
        else
        {
            targetVel = correctDirVec * moveSpeed;

            if (isBoosting)
                targetVel *= boostExtraSpeed;

            //Increasing the currVel with a vector that is maxMagnitude of accelSpeed  * Time.deltaTime 
            currVel = currVel + (Vector2.ClampMagnitude(targetVel - currVel, accelSpeed) * Time.deltaTime);

            transform.Translate(currVel.ToVector3() * Time.deltaTime, Space.World);
        }
    }

    void CorrectDirection()
    {
        if (isCorrectingDirection && !isMakingConnection)
        {
            transform.localEulerAngles = Vector3.up * targetAngle;
            //float deltaAngle = Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle);

            //print(string.Format("Delta Angle: {0} Torque: {1} Target: {2}", deltaAngle, transform.eulerAngles.y, targetAngle + 90));
            //rb.angularVelocity = Vector3.up * deltaAngle * correctingTorque;
        }

        if (isJointed)
        {
            float angle = -jointAngle;

            if (!currKnob.isLeft)
            {
                angle = angle + 180;
            }

            transform.localEulerAngles = Vector3.up * angle;
        }
    }

    public void Die()
    {
        if (isDed)
            return;

        CameraShaker.Instance.ShakeOnce(5, 5, 0, 0.8f);

        nextStageTextAnim.gameObject.SetActive(false);

        RemoveConnection();

        perfectText.gameObject.SetActive(false);

        isCorrectingDirection = false;

        print("BOOM EXPLODED");

        isDed = true;

        GameManager.main.Loose();
    }

    void MakeConnection(Knob knob)
    {
        if (isMakingConnection)
            return;

        currKnob = knob;
        currKnob.isMakingConnection = true;
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

        if (currKnob)
            currKnob.isMakingConnection = false;

        memDistance = 0;
        UpdateLineRenderer(Vector3.zero);

        RemoveJoint();
    }

    void UpdatePosToKnob(Vector3 center, float dist, float angle)
    {
        Vector2 localVec = angle.DegreeToVector2() * dist;

        transform.position = center + localVec.ToVector3();
    }

    void AddJoint(float dist)
    {
        if (isJointed)
            return;

        isJointed = true;

        float speed = currVel.magnitude;

        currJointSpeed = 90 * (speed / ((baseDist * Mathf.PI) / 2));

        jointDistance = dist;
        jointAngle = (transform.position.ToVector2() - currKnob.jointDotTrans.position.ToVector2()).ToAngle();

        print(string.Format("Created joint dist: {0}, angle: {1}", jointDistance, jointAngle));

        //Adding the hinge joint here

    }

    void RemoveJoint()
    {
        if (!isJointed)
            return;

        isJointed = false;
        currVel = transform.forward.ToVector2() * moveSpeed;
    }
}
