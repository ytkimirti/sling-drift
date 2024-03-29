﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool isGameStarted;
    public bool isLoosed;
    public bool isGameOver;

    [Space]

    public int currScore;

    public Color[] colorThemes;

    [Header("References")]

    public TextMeshProUGUI mainMenuMaxScoreText;
    [Space]
    public TextMeshProUGUI gameOverScoreText;
    public TextMeshProUGUI gameOverMaxScoreText;
    [Space]
    public Animator gameOverAnim;
    public Animator mainMenuAnim;

    public Material groundMaterial;

    public Vector2 testVec;
    public float testAngle;

    int highScore;
    int currColor;
    //REFERENCES ETC CODES ETCETC

    //PRIVATE STUFF


    //SINGLETON
    public static GameManager main;

    private void Awake()
    {
        main = this;
    }
    private void Update()
    {
        testAngle = testVec.ToAngle();
    }
    void Start()//-start
    {
        LoadHighscore();

        SetMenuScoreTexts();

        groundMaterial.color = colorThemes[0];
    }

    public void ChangeGroundColor()
    {
        currColor++;
        Color col = colorThemes[currColor];

        groundMaterial.DOColor(col, 0.6f);
    }

    public void OnScored()
    {
        Player.main.SetMoveSpeed();//Makes the player's speed faster so the game is harder, duh
    }

    void LoadHighscore()
    {
        if (PlayerPrefs.HasKey("highscore"))
        {
            highScore = PlayerPrefs.GetInt("highscore");
        }
        else
        {
            PlayerPrefs.SetInt("highscore", 0);
            highScore = 0;
        }
    }

    void SetHighscore()
    {
        if (currScore > highScore)
        {
            highScore = currScore;
            PlayerPrefs.SetInt("highscore", highScore);
        }
    }

    void SetMenuScoreTexts()
    {
        mainMenuMaxScoreText.text = "Max: " + highScore.ToString();
    }

    void SetGameOverScoreTexts()
    {
        gameOverMaxScoreText.text = "Max: " + highScore.ToString();
        gameOverScoreText.text = currScore.ToString();
    }

    public void StartGame()
    {
        isGameStarted = true;
        mainMenuAnim.SetTrigger("GameStart");
    }

    public void Loose()//-loose
    {
        if (isGameOver)
            return;

        isLoosed = true;

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

        SetHighscore();

        SetGameOverScoreTexts();

        //Game over code
        gameOverAnim.SetTrigger("GameOver");
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

}
