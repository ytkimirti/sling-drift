using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Transform target;

    [Space]

    public Vector3 offset;
    public Vector2 dynamicOffset;
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
        holder.transform.localPosition = offset;
    }

    void LateUpdate()
    {

        UpdateHolder();

        if (target)
        {
            targetPos = target.position;

            targetPos = targetPos + Vector3.Scale(Player.main.transform.forward, dynamicOffset.ToVector3());
        }

        transform.position = Vector3.Lerp(transform.position, targetPos, lerpSpeed * Time.deltaTime);
    }
}
