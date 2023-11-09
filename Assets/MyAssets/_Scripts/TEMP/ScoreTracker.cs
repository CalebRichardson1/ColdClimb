using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ScoreTracker : MonoBehaviour
{
    public TMP_Text scoreText;
    public TMP_Text highScoreText;

    float score;
    
    void Start()
    {
        float tempScore = PlayerPrefs.GetFloat("highScore");
        highScoreText.text = tempScore.ToString();
        ResourceLoader.InputManager.ReturnReloadAction().performed += SetHighScore;
        ResourceLoader.InputManager.ReturnInteractAction().performed += ResetHighscore;
    }

    // Update is called once per frame
    void Update()
    {
        score += 1f * Time.deltaTime;
        scoreText.text = score.ToString();
    }

    public void SetHighScore(InputAction.CallbackContext context)
    {
        float tempScore = score;
        float tempHighScore = PlayerPrefs.GetFloat("highScore");

        if(tempScore > tempHighScore)
        {
            PlayerPrefs.SetFloat("highScore", score);
            highScoreText.text = tempScore.ToString();
        }

    }
    public void ResetHighscore(InputAction.CallbackContext context)
    {
        PlayerPrefs.SetFloat("highScore", 0f);
        float tempScore = PlayerPrefs.GetFloat("highScore");
        highScoreText.text = tempScore.ToString();
    }
}
