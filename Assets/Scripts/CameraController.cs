using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Transform target;

    [Space]

    public Vector2 offset;
    public float angle;

    [Space]

    public float lerpSpeed;

    [Header("References")]

    public Transform holder;
    public Camera cam;

    public static CameraController main;

    Vector3 targetPos;

    private void Awake()
    {
        main = this;
    }

    void Start()
    {

    }

    private void OnValidate()
    {
        UpdateHolder();
    }

    void UpdateHolder()
    {
        holder.localEulerAngles = new Vector3(angle, 0, 0);
        holder.transform.localPosition = new Vector3(0, offset.y, offset.x);
    }

    void FixedUpdate()
    {

        UpdateHolder();

        if (target)
        {
            targetPos = target.position;
        }

        transform.position = Vector3.Lerp(transform.position, targetPos, lerpSpeed * Time.deltaTime);
    }
}
