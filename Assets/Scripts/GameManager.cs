using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool isLoosed;
    public bool isGameOver;

    [Space]

    public float currScore;

    [Header("References")]

    public TextMeshProUGUI currScoreText;
    //REFERENCES ETC CODES ETCETC

    //PRIVATE STUFF


    //SINGLETON
    public static GameManager main;

    private void Awake()
    {
        main = this;
    }

    void Start()//-start
    {

    }

    public void Loose()//-loose
    {
        if (isGameOver)
            return;

        isLoosed = true;

        Invoke("RestartGame", 2f);

        print("LOOSE");

        GameOver();
    }

    public void Win()//-win
    {
        if (isGameOver)
            return;

        isLoosed = false;

        print("WIN");

        GameOver();
    }

    void GameOver()//-gameover
    {
        if (isGameOver)
            return;

        isGameOver = true;

        //Game over code
    }

    public void RestartGame()//-restart
    {
        if (Fader.main)
        {
            Fader.main.FadeIn();

            Invoke("ReloadScene", Fader.main.fadeSpeed);
        }
        else
        {
            ReloadScene();
        }
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(0);
    }

    void Update()//-update
    {
        if (currScoreText)
            currScoreText.text = "Score: " + currScore.ToString();
    }
}
