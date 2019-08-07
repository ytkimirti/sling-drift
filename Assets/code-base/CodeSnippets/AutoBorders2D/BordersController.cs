using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BordersController : MonoBehaviour
{
    public float offset;

    public Transform borderR, borderL, borderU, borderD;

    public static BordersController main;

    void Awake()
    {
        main = this;
    }

    void Start()
    {

    }

    public void Rearrange(Vector2 scale)
    {
        borderR.transform.localPosition = Vector2.right * (scale.x + 1 - offset);
        borderL.transform.localPosition = Vector2.right * (-scale.x - 1 + offset);

        borderU.transform.localPosition = Vector2.up * (scale.y + 1 - offset);
        borderD.transform.localPosition = Vector2.up * (-scale.y - 1 + offset);
    }

    void Update()
    {

    }
}
