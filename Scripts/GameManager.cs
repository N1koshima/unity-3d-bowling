using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;
using System.Collections;
using JetBrains.Annotations;

public class GameManager : MonoBehaviour
{
    //счет, фрейм, спавн кеглей, рестарт
    public static GameManager instance;
    public int pinsLeft = 10;
    public int score = 0;
    public int throwCount = 0;
    public int frame = 1;
    public int maxFrames = 10;
    public int frameStartPins = 10;
    public TMP_Text frameText;
    public TMP_Text scoreText;
    public TMP_Text throwText;
    public TMP_Text resultTMP;
    public TMP_Text pinsLeftText;
    public TMP_Text scoreBoardText;
    public GameObject newFrameText;
    public GameObject gameovertext;
    public GameObject pinPrefab;
    public GameObject resultText;
    public Transform pinsParent;
    public GameObject restartButton;
    private int[,] frameResult = new int[10, 2];
    //начало броска
    private int pinsBeforeThrow;
    
    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        SpawnPins();
        UpdateScoreUI();
        UpdateFrameUI();
        UpdateThrowUI();
    }
    public void PinDestroyed() 
    {
        pinsLeft--;
        UpdatePinsUI();
        score++;
        UpdateScoreUI();

        CheckStrike();
        CheckSpare();
    }

    void CheckStrike()
    {
        if (throwCount == 0 && pinsLeft == 0)
        {
            ShowResult("STRIKE!");
        }
    }

    void CheckSpare()
    {
        if (throwCount == 1 && pinsLeft == 0)
        {
            ShowResult("SPARE!");
        }
    }

    void ShowResult(string text)
    {
        StartCoroutine(ResultRoutine(text));
    }

    IEnumerator ResultRoutine(string text)
    {
        resultTMP.text = text;
        resultText.SetActive(true);
        yield return new WaitForSeconds(2f);
        resultText.SetActive(false);
    }

    void UpdateScoreUI() 
    {
        scoreText.text = "Score: " + score;
    }

    void UpdatePinsUI()
    {
        pinsLeftText.text = "Pins Left: " + pinsLeft;
    }

    public void WinGame() 
    {
        gameovertext.SetActive(true);
        scoreBoardText.gameObject.SetActive(true);
        restartButton.SetActive(true);
    }

    public void RestartGame() 
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void StartThrow()
    {
        pinsBeforeThrow = pinsLeft;
    }
    public void SpawnPins() 
    {
        foreach (Transform child in pinsParent) 
        { 
            Destroy(child.gameObject);
        }

        Vector3[] position =
            {
                new Vector3(0,1,10),

                new Vector3(-0.5f,1,11),
                new Vector3(0.5f,1,11),

                new Vector3(-1,1,12),
                new Vector3(0,1,12),
                new Vector3(1,1,12),

                new Vector3(-1.5f,1,13),
                new Vector3(-0.5f,1,13),
                new Vector3(0.5f,1,13),
                new Vector3(1.5f,1,13),
        };

        foreach (Vector3 pos in position)
        {
            Instantiate(
                pinPrefab,
                pos,
                Quaternion.identity,
                pinsParent
            );
        }

        pinsLeft = 10;
        UpdatePinsUI();
        frameStartPins = pinsLeft;
    }

    //возврат шара
    public void BallReturned()
    {
        Debug.Log($"Frame={frame}, Throw={throwCount}");
        if (frame > maxFrames)
            return;
        int knockedPins = pinsBeforeThrow - pinsLeft;
        if (knockedPins == 10 && throwCount == 0)
        {
            frameResult[frame - 1, 0] = 10;
            frameResult[frame - 1, 1] = -1;

            frame++;
            throwCount = 0;

            UpdateScoreBoard();
            if (frame > maxFrames)
            {
                UpdateScoreBoard();
                WinGame();
                return;
            }

            UpdateFrameUI();
            StartCoroutine(StartNewFrame());

            return;
        }
        frameResult[frame-1,throwCount] = knockedPins;
        UpdateScoreBoard();
        throwCount++;
        if (frame > maxFrames)
        {
            UpdateScoreBoard();
            WinGame();
            return;
        }
        if (throwCount >= 2)
        {
            throwCount = 0;
            frame++;
            if (frame > maxFrames)
            {
                UpdateScoreBoard();
                WinGame();
                return;
            }
            UpdateFrameUI();
            StartCoroutine(StartNewFrame());
        }
        UpdateThrowUI();
    }

    //вывод итога бросков
    void UpdateScoreBoard()
    {
        string bottomRow = "";
        string topRow = "";
        for (int i = 0; i < maxFrames; i++)
        {
            string throw1, throw2;
            topRow += $"F{i + 1}\t";
            //страйк
            if (frameResult[i, 0] == 10)
            {
                throw1 = "X";
                throw2 = "";
            }
            //спейр
            else if (frameResult[i, 0] + frameResult[i, 1] == 10 && frameResult[i, 0] > 0)
            {
                throw1 = frameResult[i, 0].ToString();
                throw2 = "/";
            }
            else
            {

                throw1 = frameResult[i, 0] == 0 ? "-" : frameResult[i, 0].ToString();
                throw2 = frameResult[i, 1] == 0 ? "-" : frameResult[i, 1].ToString();
            }
            bottomRow += $"{throw1}|{throw2}\t";
        }

        scoreBoardText.text = topRow + "\n" + bottomRow;
        Debug.Log(scoreBoardText.text);
    }

    IEnumerator StartNewFrame()
    {
        newFrameText.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        newFrameText.SetActive(false);
        SpawnPins();
        UpdateThrowUI();
    }
    void UpdateThrowUI()
    {
        throwText.text = "Throw: " + (throwCount + 1) + "/2";
    }

    void UpdateFrameUI()
    {
        frameText.text = "Frame: " + frame;
    }
}
