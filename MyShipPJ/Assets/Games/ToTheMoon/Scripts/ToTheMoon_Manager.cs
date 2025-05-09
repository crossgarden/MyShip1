using System.Collections;
using System.Collections.Generic;
using GameData;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToTheMoon_Manager : MonoBehaviour
{
    public static ToTheMoon_Manager instance;

    private void Awake()
    {
        instance = this;
    }

    Game game;
    public float score = 0;
    public int coin = 0;

    public float speed = 6f;
    public int speedUpInterval = 5;
    private float pausedTimeScale;

    public float BackgroundMoveSpeed { get; private set; }
    public Transform playerContainer;

    [Header("Game Speed Settings")]
    public float baseTimeScale = 1.0f; // 기본 시간 속도
    public float maxTimeScale = 3.67f; // 최대 시간 속도 (22f / 6f)
    public float timeScaleIncreaseInterval = 5f; // 속도 증가 간격
    public float timeScaleIncrement = 0.0833f; // 속도 증가량

    [Header("Cloud Settings")]
    public GameObject cloudPrefab; // 구름 프리팹
    public float cloudSpawnY = 5.3f; // 구름 생성 y 위치
    public Vector2 cloudSpawnXRange = new Vector2(-2.2f, 2.2f); // 구름 생성 x 범위
    public float cloudSpawnInterval = 1f; // 구름 생성 간격

    [Header("Background")]
    public Transform bg1, bg2; // 두 개의 배경 오브젝트
    private Vector3 bgSize; // 배경 크기
    private readonly float bgYThreshold = -13f; // 배경 재사용 위치
    private Color blueColor = new Color(163 / 255f, 218 / 255f, 1f);
    private Color sunsetColor = new Color(1.0f, 195 / 255f, 87 / 255f);
    private Color nightColor = new Color(35 / 255f, 37 / 255f, 47 / 255f);
    private Color cloudGrey = new Color(169 / 255f, 169 / 255f, 169 / 255f);

    public GameObject coinPrefab;
    public Vector3[] spawnPositions = new Vector3[20];
    private List<int> availablePositions = new List<int>();

    [Header("GameTopBar & GameOverPanel")]
    public GameObject gameTopBar, topBar, gameOverPanel, reStartBtn, returnBtn;
    public TextMeshProUGUI scoreTxt, coinTxt, overCoinTxt, overTxt, overScoreTxt, overHighScoreTxt;

    void Start()
    {
        GameManager.instance.returnFromGame = true;
        game = DataManager.instance.userData.games[PlayerPrefs.GetInt("CurGame")];

        // 캐릭터 생성
        GameObject playerPrefab = Resources.Load<GameObject>("Prefabs/Characters/" + GameManager.instance.curCharacter.name);
        GameObject player = Instantiate(playerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        player.transform.SetParent(playerContainer, false);

        // 배경 초기화
        bgSize = bg1.GetComponent<Renderer>().bounds.size;

        Time.timeScale = baseTimeScale;
        StartCoroutine("ScoreUpRoutine");
        StartCoroutine("IncreaseTimeScaleRoutine");
        StartCoroutine("MoveBackgroundRoutine"); // 배경 이동 코루틴 추가
        StartCoroutine("CreateCloudRoutine"); // 구름 생성 코루틴 추가
    }

    // 시간 속도 증가 코루틴
    IEnumerator IncreaseTimeScaleRoutine()
    {
        while (Time.timeScale < maxTimeScale)
        {
            yield return new WaitForSecondsRealtime(timeScaleIncreaseInterval); // 실제 시간 기준 대기
            Time.timeScale = Mathf.Min(Time.timeScale + timeScaleIncrement, maxTimeScale);
        }
    }

    // 구름 생성 코루틴 (Meteor_Manager 참고)
    IEnumerator CreateCloudRoutine()
    {
        while (true)
        {
            // 랜덤 x 위치에서 구름 생성
            float randomX = Random.Range(cloudSpawnXRange.x, cloudSpawnXRange.y);
            Vector3 spawnPos = new Vector3(randomX, cloudSpawnY, 0);

            // 구름 생성
            GameObject cloud = Instantiate(cloudPrefab, spawnPos, Quaternion.identity);

            // 30% 확률로 구름 위에 코인 생성
            if (Random.Range(0f, 1f) < 0.3f)
            {
                // 코인 위치 계산 (구름 바로 위 중앙)
                Vector3 coinPos = new Vector3(
                    cloud.transform.position.x,
                    cloud.transform.position.y + 0.4f, // 구름 위에 위치하도록 y값 조정
                    0
                );

                Instantiate(coinPrefab, coinPos, Quaternion.identity);
            }

            yield return new WaitForSeconds(cloudSpawnInterval);
        }
    }

    // 배경 이동 코루틴
    IEnumerator MoveBackgroundRoutine()
    {
        while (true)
        {
            yield return null;

            // 배경 이동 속도 계산 (고정 값 사용)
            BackgroundMoveSpeed = 1.5f; // 원하는 배경 기본 속도
            float moveAmount = BackgroundMoveSpeed * Time.deltaTime;

            bg1.position += Vector3.down * moveAmount;
            bg2.position += Vector3.down * moveAmount;

            // 배경 재사용 처리
            if (bg1.position.y < bgYThreshold)
            {
                bg1.position += new Vector3(0, bgSize.y * 2, 0);
            }
            if (bg2.position.y < bgYThreshold)
            {
                bg2.position += new Vector3(0, bgSize.y * 2, 0);
            }

            // 배경 색상 변경
            UpdateBackgroundColor();
        }
    }

    // 점수에 따른 배경 색상 업데이트
    void UpdateBackgroundColor()
    {
        Color targetColor = GetTargetColor((int)score);
        Color targetColorCloud = GetTargetColorCloud((int)score);

        bg1.GetComponent<SpriteRenderer>().color = Color.Lerp(
            bg1.GetComponent<SpriteRenderer>().color, targetColor, Time.deltaTime * 2);
        bg2.GetComponent<SpriteRenderer>().color = Color.Lerp(
            bg2.GetComponent<SpriteRenderer>().color, targetColor, Time.deltaTime * 2);

        // 구름 레이어가 있다면 (bg1과 bg2의 자식 오브젝트로 가정)
        if (bg1.childCount > 0)
        {
            bg1.GetChild(0).GetComponent<SpriteRenderer>().color = Color.Lerp(
                bg1.GetChild(0).GetComponent<SpriteRenderer>().color, targetColorCloud, Time.deltaTime * 2);
            bg2.GetChild(0).GetComponent<SpriteRenderer>().color = Color.Lerp(
                bg2.GetChild(0).GetComponent<SpriteRenderer>().color, targetColorCloud, Time.deltaTime * 2);
        }
    }

    // 배경 색상 계산
    Color GetTargetColor(int score)
    {
        if (score < 50)
            return blueColor;

        if (score < 200)
            return Color.Lerp(blueColor, sunsetColor, (score - 50) / 150f);

        if (score < 250)
            return sunsetColor;

        if (score < 300)
            return Color.Lerp(sunsetColor, nightColor, (score - 250) / 50f);

        if (score < 350)
            return nightColor;

        if (score < 500)
            return Color.Lerp(nightColor, blueColor, (score - 350) / 150f);

        return GetTargetColor(score % 500);
    }

    Color GetTargetColorCloud(int score)
    {
        if (score < 200)
            return Color.white;

        if (score < 300)
            return Color.Lerp(Color.white, cloudGrey, (score - 200) / 100f);

        if (score < 500)
            return Color.Lerp(cloudGrey, Color.white, (score - 300) / 200f);

        return GetTargetColorCloud(score % 500);
    }

    public void GameStart(GameObject startBtn)
    {
        Time.timeScale = 1;
        startBtn.SetActive(false);
    }

    //// 스피드 증가 코루틴
    //IEnumerator SpeedUpRoutine()
    //{
    //    while (speed < 22f)
    //    {
    //        yield return new WaitForSeconds(speedUpInterval);
    //        speed += 0.5f;
    //    }
    //}

    // 점수 증가 코루틴
    IEnumerator ScoreUpRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            score += (6f * Time.timeScale) / 4; // baseSpeed(6f) * timeScale / 4
            scoreTxt.text = "점수: " + (int)score;
        }
    }

    // 코인 증가 (시간 속도 기준)
    public void GetCoin()
    {
        int coinValue;

        if (Time.timeScale > 3f)
            coinValue = 5;
        else if (Time.timeScale > 2f)
            coinValue = 3;
        else
            coinValue = 2;

        DataManager.instance.userData.coin += coinValue;
        coin += coinValue;
        coinTxt.text = coin.ToString();
    }

    // 게임 오버
    public void GameOver(bool isOver)//로그를 남겨야 하는 이유: 에러가 안남, 아마 실행순서와 연관이 있는 듯
    {
        pausedTimeScale = Time.timeScale;
        Time.timeScale = 0; // 게임 일시정지

        Debug.Log("[1] GameOver 호출됨"); // 1. 메서드 진입 확인

        try
        {
            Debug.Log("[2] UI 요소 확인 전");
            if (overTxt == null) Debug.LogError("overTxt null");
            if (topBar == null) Debug.LogError("topBar null");
            if (gameOverPanel == null) Debug.LogError("gameOverPanel null");

            Debug.Log("[3] DataManager 확인");
            if (DataManager.instance == null) Debug.LogError("DataManager null");
            if (game == null) Debug.LogError("game null");

            DataManager.instance.saveData();
            Debug.Log("[4] 저장 완료");

            Time.timeScale = 0;
            Debug.Log("[5] TimeScale 0 설정");

            StopCoroutine("ScoreUpRoutine");
            StopCoroutine("IncreaseTimeScaleRoutine");
            StopCoroutine("MoveBackgroundRoutine");
            StopCoroutine("CreateCloudRoutine");
            Debug.Log("[6] 코루틴 정지");

            overTxt.text = isOver ? "게임오버" : "일시정지";
            Debug.Log("[7] 텍스트 설정 완료");

            topBar.SetActive(true);

            returnBtn.SetActive(!isOver);
            reStartBtn.SetActive(isOver);
            overCoinTxt.text = "+ " + coin;
            overScoreTxt.text = "점수 : " + (int)score;
            if (score > game.high_score)
                game.high_score = (int)score;

            overHighScoreTxt.text = "최고 : " + game.high_score;
            Debug.Log("[8] UI 활성화 완료");

            gameOverPanel.SetActive(true);
            Debug.Log("[9] 게임오버 패널 활성화");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"에러 발생: {e.Message}\n{e.StackTrace}");
        }
    }

    // [1-3] 게임 오버 패널 버튼 액션들
    public void ReStartAction()
    {
        Time.timeScale = baseTimeScale;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnAction()
    {
        Time.timeScale = pausedTimeScale;
        StartCoroutine("ScoreUpRoutine");
        StartCoroutine("IncreaseTimeScaleRoutine");
        StartCoroutine("MoveBackgroundRoutine");
        StartCoroutine("CreateCloudRoutine");

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