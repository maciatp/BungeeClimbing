using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverCanvas : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI finalHeightText;
    [SerializeField] TMPro.TextMeshProUGUI highScoreText;
    [SerializeField] TMPro.TextMeshProUGUI highScoreLabel;
    [SerializeField] TMPro.TextMeshProUGUI newRecordLabel;
    GameManager gameManager;

   

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        finalHeightText.text = gameManager.GetCurrentPlayTopHeight.ToString("0000") + " m";

        //IF NEW RECORD
        if(gameManager.GetHighScoreBeaten)
        {
            newRecordLabel.gameObject.SetActive(true);
            highScoreText.gameObject.SetActive(false);
            highScoreLabel.gameObject.SetActive(false);
            

        }
        else
        {
            newRecordLabel.gameObject.SetActive(false);
            highScoreLabel.gameObject.SetActive(true);
            highScoreText.gameObject.SetActive(true);
            highScoreText.text = gameManager.GetTopHeightHighScore.ToString("0000") + " m";
        }
    }

    
}
