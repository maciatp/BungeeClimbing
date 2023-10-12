using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    bool mustCount = false;
    [SerializeField] float initialTime = 60;
    float currentTime;
    [SerializeField] TMPro.TextMeshProUGUI secondsText;
    [SerializeField] TMPro.TextMeshProUGUI centsText;
    [SerializeField] Image timerBackground;
    [SerializeField] Image timerForeground;
    float topHeightHighScore;
    float currentHeight;
    float currentPlayTopHeight;
    bool highScoreBeaten = false;
    ScoreHeight scoreHeightUI;

    GameObject player;
    SphereCollider playerCollider;

    bool isMuted = false;

    bool isGamePaused = false;
    bool isGameOver = false;
    bool isGameWin = false;


    public float CurrentTime
    {
        get { return currentTime; }
    }
    public float GetCurrentHeight
    {
        get { return currentHeight; }
    }
    public float GetTopHeightHighScore
    {
        get { return topHeightHighScore; }
    }

    public bool GetHighScoreBeaten
    {
        get { return highScoreBeaten; }
    }

    public float GetCurrentPlayTopHeight
    {
        get { return currentPlayTopHeight; }
    }

    public bool IsGamePaused
    {
        get { return isGamePaused; }
        set { isGamePaused = value; }
    }

    public bool IsGameOver
    {
        get { return isGameOver; }
    }

    public bool  IsGameWin
    { get { return isGameWin; } }

    // Start is called before the first frame update
    void Start()
    {
      

        scoreHeightUI = GameObject.Find("UI").GetComponentInChildren<ScoreHeight>();
        player = GameObject.FindGameObjectWithTag("Player").gameObject;
        playerCollider = player.GetComponent<SphereCollider>();

        if (PlayerPrefs.HasKey("TopHeight"))
        {
            topHeightHighScore = PlayerPrefs.GetFloat("TopHeight");
            scoreHeightUI.SetTopHeightText(topHeightHighScore);

        }
        else
        {
            SaveTopHeight(0);
        }

        if(secondsText.gameObject.activeInHierarchy)
        {
            secondsText.gameObject.SetActive(false);
        }
        if (centsText.gameObject.activeInHierarchy)
        {
            centsText.gameObject.SetActive(false);
        }
        if (timerForeground.gameObject.activeInHierarchy)
        {
            timerForeground.gameObject.SetActive(false);
        }
        if (timerBackground.gameObject.activeInHierarchy)
        {
            timerBackground.gameObject.SetActive(false);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(mustCount)
        {
            if(currentTime>0)
            {
                currentTime -= Time.deltaTime;
                if(secondsText.text != Mathf.FloorToInt(currentTime).ToString("000"))
                {
                    secondsText.text = Mathf.FloorToInt(currentTime).ToString("000");
                }
                float centiSeconds = Mathf.Clamp(((currentTime % 1) * 100),0,99);
                centsText.text = "." + string.Format("{0:00}", centiSeconds);


                
                //UI TIMER

                if (timerForeground.fillClockwise == true)
                {
                    timerForeground.fillAmount -= Time.deltaTime; //centiSeconds / 100;
                    if (timerForeground.fillAmount < 0.01f && timerForeground.fillClockwise)
                    {
                        timerForeground.fillClockwise = false;
                        Debug.Log("Fillamount when LAST TRUE Was" + timerForeground.fillAmount);
                    }
                }
                else
                {

                    timerForeground.fillAmount += Time.deltaTime;//1f - (centiSeconds / 100);
                    if (timerForeground.fillAmount > 0.99f && !timerForeground.fillClockwise)
                    {
                        timerForeground.fillClockwise = true;
                        Debug.Log("Fillamount when LAST false Was" + timerForeground.fillAmount);
                    }
                }

            }
            else
            {
                GameOver();
            }
        }


        currentHeight = (player.transform.position.y - playerCollider.radius);

        //SET UI SCOREHEIGHT
        scoreHeightUI.SetHeight(currentHeight);

        //CURRENT TOP HEIGHT
        if(currentHeight > currentPlayTopHeight && currentPlayTopHeight != currentHeight)
        {
            currentPlayTopHeight = currentHeight;
            //scoreHeightUI.SetCurrentTopHeight(currentPlayTopHeight);
        }

        //TOP HEIGHT HIGH SCORE
        if (currentPlayTopHeight > topHeightHighScore && topHeightHighScore != currentPlayTopHeight)
        {
            topHeightHighScore = currentPlayTopHeight;
            scoreHeightUI.SetTopHeightText(currentHeight);
            SaveTopHeight(currentPlayTopHeight);           
            highScoreBeaten = true;

        }
    }

    private void GameOver()
    {
        Time.timeScale = 0.3f;

        Debug.Log("GameOver");
        //DISABLING GAMEPLAY CANVAS
        GameObject.Find("UI").transform.Find("GamePlayCanvas").gameObject.SetActive(false);

        //ENABLING GAMEOVER CANVAS
        GameObject.Find("UI").transform.Find("GameOverCanvas").gameObject.SetActive(true);


        isGameOver = true;




    }


    public void GameWin()
    {
        mustCount = false;
        Time.timeScale = 0.2f;

        Debug.Log("GameWin");
        //DISABLING GAMEPLAY CANVAS
        GameObject.Find("UI").transform.Find("GamePlayCanvas").gameObject.SetActive(false);

        //ENABLING GAMEOVER CANVAS
        GameObject.Find("UI").transform.Find("GameWinCanvas").gameObject.SetActive(true);
        isGameWin = true;
    }

    public void AddToCurrentTime(int secondsToAdd)
    {
        currentTime += secondsToAdd;
    }


    public void ActivateTimer()
    {
        if(!mustCount)
        {
            mustCount = true;
            currentTime = initialTime;
            secondsText.gameObject.SetActive(true);
            centsText.gameObject.SetActive(true);
            timerForeground.gameObject.SetActive(true);
            timerBackground.gameObject.SetActive(true);
            //SOUND?
            Camera.main.gameObject.GetComponent<MusicPlayer>().PlayMusic();
        }
    }

    public void PauseGame()
    {
        if(!isGamePaused)
        {
            GameObject.Find("UI").transform.Find("GamePlayCanvas").transform.Find("PauseScreen").gameObject.SetActive(true);
           Time.timeScale = 0;
            isGamePaused = true;
        }
        else
        {
            GameObject.Find("UI").transform.Find("GamePlayCanvas").transform.Find("PauseScreen").gameObject.SetActive(false);
            Time.timeScale = 1;
            isGamePaused = false;
        }
    }

    public void SaveTopHeight(float height)
    {
        PlayerPrefs.SetFloat("TopHeight", height);
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void ContinuePlayingAfterWin()
    {
        if(isGameWin)
        {
            isGameWin = false;
            mustCount = true;
            Time.timeScale = 1;
            GameObject.Find("UI").transform.Find("GameWinCanvas").gameObject.SetActive(false);
            GameObject.Find("UI").transform.Find("GamePlayCanvas").gameObject.SetActive(true);
        }
    }
    public void SetMute()
    {
        if(!isMuted)
        {
            isMuted = true;
            Camera.main.gameObject.GetComponent<AudioListener>().enabled = false;
            Camera.main.gameObject.GetComponent<MusicPlayer>().PauseMusic();

        }
        else
        {
            isMuted = false;
            Camera.main.gameObject.GetComponent<AudioListener>().enabled = true;
            Camera.main.gameObject.GetComponent<MusicPlayer>().ReturnPlayingMusic();
        }
    }
}
