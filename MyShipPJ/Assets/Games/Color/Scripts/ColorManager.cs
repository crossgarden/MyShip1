using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using GameData;

public class ColorManager : MonoBehaviour
{
    Game game;

    [Header("TopBar")]
    public GameObject topBar;

    [Header("GameTopBar")]
    public GameObject gameTopBar;
    public TextMeshProUGUI scoreTxt, coinTxt, overCoinTxt; // 코인 표시용 텍스트 추가
    int coin = 0;  // 코인 변수 추가
    public int score = 0;

    [Header("Player")]
    public GameObject player;
    public Transform playerContainer;

    [Header("Pause & GameOver")]
    public GameObject gameOverPanel, reStartBtn, returnBtn;
    public TextMeshProUGUI overTxt, overScoreTxt, HighScoreTxt, overCoin;

    [Header("Timer")]
    // public TextMeshProUGUI timerTxt;  // (있다면 유지)
    public Image timerBarImage;       // <- 타이머 바용 Image 추가

    [Header("Timer UI")]
    public Image timerImage;

    bool pause = false;

    [Header("Color Match Game")]
    public Image targetColorImage;
    public Button redBtn, blackBtn, whiteBtn;
    public float timeLimit = 60f; // <- 1분으로 설정
    private float timeRemaining;
    private string currentTargetColor;
    private bool gameActive = true;
    public Sprite redSprite;
    public Sprite blackSprite;
    public Sprite whiteSprite;

    void Start()
    {
        GameManager.instance.returnFromGame = true;
        game = DataManager.instance.userData.games[PlayerPrefs.GetInt("CurGame")];

        redBtn.onClick.AddListener(() => CheckColor("red"));
        blackBtn.onClick.AddListener(() => CheckColor("black"));
        whiteBtn.onClick.AddListener(() => CheckColor("white"));

        timeRemaining = timeLimit;
        SetNewTargetColor();
        UpdateTimerUI();
    }

    void Update()
    {
        if (!gameActive || pause) return;

        timeRemaining -= Time.deltaTime;
        if (timeRemaining < 0) timeRemaining = 0;

        UpdateTimerUI();

        if (timeRemaining <= 0)
        {
            gameActive = false;
            Fail(true); // 타이머 종료 시 게임 오버
        }
    }

    void UpdateTimerUI()
    {
        int seconds = Mathf.CeilToInt(timeRemaining);

        // 타이머 바 이미지 업데이트
        if (timerBarImage != null)
            timerBarImage.fillAmount = timeRemaining / timeLimit;
        else
            Debug.LogWarning("timerBarImage가 연결되지 않았습니다!");

        // 점수 텍스트 업데이트
        if (scoreTxt != null)
            scoreTxt.text = "점수: " + score;
        else
            Debug.LogWarning("scoreTxt가 연결되지 않았습니다!");

        // 코인 텍스트 업데이트
        if (coinTxt != null)
            coinTxt.text = "" + coin;
        else
            Debug.LogWarning("coinTxt가 연결되지 않았습니다!");
    }

    void CheckColor(string selectedColor)
    {
        if (!gameActive || pause) return;

        if (selectedColor == currentTargetColor)
        {
            score += 1;

            // 코인은 점수의 절반만큼 증가
            coin = score / 2;

            Debug.Log("점수: " + score + ", 코인: " + coin);

            // 점수와 코인 UI 업데이트
            scoreTxt.text = "Score: " + score;
            coinTxt.text = "Coin: " + coin;

            SetNewTargetColor();
        }
        else
        {
            gameActive = false;
            Fail(true);
        }
    }

    void SetNewTargetColor()
    {
        string[] colors = { "black", "red", "white" };
        currentTargetColor = colors[UnityEngine.Random.Range(0, colors.Length)];

        switch (currentTargetColor)
        {
            case "red":
                targetColorImage.sprite = redSprite;
                break;
            case "black":
                targetColorImage.sprite = blackSprite;
                break;
            case "white":
                targetColorImage.sprite = whiteSprite;
                break;
        }
    }

    public void Fail(bool isFail)
    {
        pause = true;
        Time.timeScale = 0;

        topBar.SetActive(true);

        if (score > game.high_score)
        {
            game.high_score = score;
        }

        overTxt.text = isFail ? "게임오버" : "일시정지";
        reStartBtn.SetActive(isFail);
        returnBtn.SetActive(!isFail);

        overScoreTxt.text = "점수: " + score;
        HighScoreTxt.text = "최고: " + game.high_score;

        // 코인 텍스트도 함께 표시
        if (overCoinTxt != null)
            overCoinTxt.text = "" + coin;
        else
            Debug.LogWarning("overCoinTxt가 연결되지 않았습니다!");

        gameOverPanel.SetActive(true);
        DataManager.instance.saveData();
    }


    public void ReStartAction()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnAction()
    {
        pause = false;
        gameOverPanel.SetActive(false);
        Time.timeScale = 1;
        topBar.SetActive(false);
    }

    public void ExitAction()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainScene");
    }
}
