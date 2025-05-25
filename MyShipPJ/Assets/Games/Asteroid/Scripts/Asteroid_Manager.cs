using System.Collections;
using System.Collections.Generic;
using GameData;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Asteroid_Manager : MonoBehaviour
{
    public static Asteroid_Manager instance;

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

    public GameObject asteroidsPrefab;
    public GameObject coinPrefab;
    public Camera mainCamera; // 화면 경계 계산을 위한 카메라 참조

    [Header("GameTopBar & GameOverPanel")]
    public GameObject gameTopBar, topBar, gameOverPanel, reStartBtn, returnBtn;
    public TextMeshProUGUI scoreTxt, coinTxt, overCoinTxt, overTxt, overScoreTxt, overHighScoreTxt;

    void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        GameManager.instance.returnFromGame = true;
        game = DataManager.instance.userData.games[PlayerPrefs.GetInt("CurGame")];

        // 플레이어 생성
        GameObject playerPrefab = Resources.Load<GameObject>("Prefabs/Characters/" + GameManager.instance.curCharacter.name);
        GameObject player = Instantiate(playerPrefab, new Vector3(0, 0, 0), Unity.Mathematics.quaternion.identity);
        player.transform.SetParent(playerContainer, false);

        // 메인 카메라 참조
        mainCamera = Camera.main;

        Time.timeScale = 1;

        StartCoroutine("CreateAsteroidsAndCoinRoutine");
        StartCoroutine("SpeedUpRoutine");
        StartCoroutine("ScoreUpRoutine");
    }

    // 화면 경계 밖의 랜덤 위치 생성 (4면 중 하나)
    public Vector3 GetRandomSpawnPosition()
    {
        float screenWidth = mainCamera.orthographicSize * mainCamera.aspect;
        float screenHeight = mainCamera.orthographicSize;

        // 0: 상, 1: 하, 2: 좌, 3: 우
        int side = Random.Range(0, 4);

        switch (side)
        {
            case 0: // 상단
                return new Vector3(Random.Range(-screenWidth, screenWidth), screenHeight + 1, 0);
            case 1: // 하단
                return new Vector3(Random.Range(-screenWidth, screenWidth), -screenHeight - 1, 0);
            case 2: // 좌측
                return new Vector3(-screenWidth - 1, Random.Range(-screenHeight, screenHeight), 0);
            case 3: // 우측
                return new Vector3(screenWidth + 1, Random.Range(-screenHeight, screenHeight), 0);
            default:
                return Vector3.zero;
        }
    }

    // 소행성 & 코인 생성 루틴
    IEnumerator CreateAsteroidsAndCoinRoutine()
    {
        while (true)
        {
            // speed에 따라 소행성과 코인 개수 결정
            int meteorCount = speed > 18 ? 13 : (speed > 12 ? 9 : 5);
            int coinCount = speed > 18 ? 3 : (speed > 12 ? 2 : 1);

            // 소행성 생성
            for (int i = 0; i < meteorCount; i++)
            {
                Vector3 spawnPos = GetRandomSpawnPosition();
                GameObject asteroid = Instantiate(asteroidsPrefab, spawnPos, Quaternion.identity);

                // 소행성의 이동 방향 설정 (생성 위치의 반대 방향)
                Asteroid_Obstacle obstacle = asteroid.GetComponent<Asteroid_Obstacle>();
                if (obstacle != null)
                {
                    obstacle.moveDirection = (Vector3.zero - spawnPos).normalized;
                }
            }

            // 코인 생성
            for (int i = 0; i < coinCount; i++)
            {
                Vector3 spawnPos = GetRandomSpawnPosition();
                GameObject coinObj = Instantiate(coinPrefab, spawnPos, Quaternion.identity);

                // 코인의 이동 방향 설정 (생성 위치의 반대 방향)
                Asteroid_Obstacle coin = coinObj.GetComponent<Asteroid_Obstacle>();
                if (coin != null)
                {
                    coin.moveDirection = (Vector3.zero - spawnPos).normalized;
                }

                // speed에 따라 코인 크기 조정
                if (speed > 18)
                    coinObj.transform.localScale = coinObj.transform.localScale * 1.5f;
                else if (speed > 12)
                    coinObj.transform.localScale = coinObj.transform.localScale * 1.3f;
            }

            yield return new WaitForSeconds(3f);
        }
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
        else if (speed > 12)
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

        StopCoroutine("CreateAsteroidsAndCoinRoutine");
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
        StartCoroutine("CreateAsteroidsAndCoinRoutine");
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