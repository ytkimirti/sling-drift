using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZ_Pooling;
using DG.Tweening;
using TMPro;

public class Knob : MonoBehaviour
{
    public bool isLeft;
    public bool isMakingConnection;

    [Space]

    public Transform spawnerEndTrans;
    public Transform visualDotTrans;
    public Transform jointDotTrans;
    public Transform correctionEnablerTrans;
    public Transform roadTrans;
    public Transform dotTopTrans;
    public SphereCollider sphereCollider;
    public TextMeshPro scoreText;

    [HideInInspector]
    public Vector2 turnDir;

    int myScore;


    void Start()
    {
        //UpdateDirection();
    }

    void OnValidate()
    {
        UpdateDirection();
    }

    public void SetScore(int score)
    {
        scoreText.color = Color.grey;
        scoreText.text = score.ToString();
        myScore = score;
    }

    public void OnPlayerScored()
    {
        GameManager.main.currScore = myScore;
        GameManager.main.OnScored();
        scoreText.DOColor(Color.yellow, 0.3f).SetLoops(1, LoopType.Yoyo);
        scoreText.transform.DOPunchScale(Vector3.one * 0.6f, 0.4f, 4);
    }

    public void UpdateDirection()
    {
        scoreText.transform.eulerAngles = new Vector3(90, 0, 0);

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
        InverseX(scoreText.transform);
        correctionEnablerTrans.forward = -correctionEnablerTrans.forward;
    }

    void InverseX(Transform trans)
    {
        trans.localPosition = new Vector3(-trans.localPosition.x, trans.localPosition.y, trans.localPosition.z);
    }

    void Update()
    {
        dotTopTrans.localPosition = Vector3.up * Mathf.Lerp(dotTopTrans.localPosition.y, isMakingConnection ? 0 : -2, 5 * Time.deltaTime);//QUICK POLISH, just a biiit dirty
    }
}
