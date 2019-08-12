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

    private void Awake()
    {
        main = this;
    }

    void Start()
    {

    }

    void FixedUpdate()
    {
        holder.localEulerAngles = new Vector3(angle, 0, 0);

        Vector3 targetPos = new Vector3(0, offset.y, target.position.z + offset.x);

        transform.position = Vector3.Lerp(transform.position, targetPos, lerpSpeed * Time.deltaTime);
    }
}
