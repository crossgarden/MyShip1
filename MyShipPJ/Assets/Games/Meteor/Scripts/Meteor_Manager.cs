using System.Collections;
using System.Collections.Generic;
using GameData;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Meteor_Manager : MonoBehaviour
{

    public static Meteor_Manager instance;

    private void Awake()
    {
        instance = this;
    }

    Game game;
    public float score = 0;
    public int coin = 0;

    public float speed = 6f;
    public int speedUpInterval = 5;

    public Transform playerContainer;
    // public Animator playerAnim;

    public GameObject coinPrefab;

    [Header("GameTopBar & GameOverPanel")]
    public GameObject gameTopBar, topBar, gameOverPanel, reStartBtn, returnBtn;
    public TextMeshProUGUI scoreTxt, coinTxt, overCoinTxt, overTxt, overScoreTxt, overHighScoreTxt;

    void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        GameManager.instance.returnFromGame = true;
        game = DataManager.instance.userData.games[PlayerPrefs.GetInt("CurGame")];
        // 캐릭터 생성
        GameObject playerPrefab = Resources.Load<GameObject>("Prefabs/Characters/" + GameManager.instance.curCharacter.name);
        GameObject player = Instantiate(playerPrefab, new Vector3(0, 0, 0), Unity.Mathematics.quaternion.identity);
        player.transform.SetParent(playerContainer, false);
        // playerAnim = player.GetComponent<Animator>();

        Time.timeScale = 0;

        StartCoroutine("SpeedUpRoutine");
        StartCoroutine("ScoreUpRoutine");
    }

    public void GameStart(GameObject startBtn)
    {
        Time.timeScale = 1;
        startBtn.SetActive(false);
    }

    // 스피드 증가 코루틴
    IEnumerator SpeedUpRoutine()
    {
        while (speed < 22f)
        {
            yield return new WaitForSeconds(speedUpInterval);
            speed += 0.5f;
        }

    }

    // 점수 증가 코루틴
    IEnumerator ScoreUpRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            score += speed / 4;
            scoreTxt.text = "점수: " + (int)score;
        }
    }

    // 코인 증가
    public void GetCoin()
    {
        int coinValue;

        if (speed > 18)
            coinValue = 5;
        if (speed > 12)
            coinValue = 3;
        else
            coinValue = 2;

        DataManager.instance.userData.coin += coinValue;
        coin += coinValue;
        coinTxt.text = coin.ToString();
    }

    // 게임 오버
    public void GameOver(bool isOver)
    {
        DataManager.instance.saveData();
        Time.timeScale = 0;

        StopCoroutine("SpeedUpRoutine");
        StopCoroutine("ScoreUpRoutine");

        overTxt.text = isOver ? "게임오버" : "일시정지";

        topBar.SetActive(true);

        returnBtn.SetActive(!isOver);
        reStartBtn.SetActive(isOver);
        overCoinTxt.text = "+ " + coin;
        overScoreTxt.text = "점수 : " + (int)score;
        if (score > game.high_score)
            game.high_score = (int)score;

        overHighScoreTxt.text = "최고 : " + game.high_score;
        gameOverPanel.SetActive(true);

    }

    // [1-3] 게임 오버 패널 버튼 액션들
    public void ReStartAction()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnAction()
    {
        StartCoroutine("SpeedUpRoutine");
        StartCoroutine("ScoreUpRoutine");

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
