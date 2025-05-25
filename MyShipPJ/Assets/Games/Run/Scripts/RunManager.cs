using System.Collections;
using System.Collections.Generic;
using GameData;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunManager : MonoBehaviour
{
    public static RunManager instance;

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
    Animator anim; // Animator 추가

    public GameObject[] rockPrefabs;
    public Transform[] rockSpawns; // 0, 1, 2 positions
    public Vector3[] rockSpawnsPos = new Vector3[3];
    public float rockSpawnRate = 2f;

    public int[] rockSortingOrders = new int[3] { 1, 2, 3 }; // 각 위치별 sortingOrder 값

    public GameObject coinPrefab;

    [Header("GameTopBar & GameOverPanel")]
    public GameObject gameTopBar, topBar, gameOverPanel, reStartBtn, returnBtn;
    public TextMeshProUGUI scoreTxt, coinTxt, overCoinTxt, overTxt, overScoreTxt, overHighScoreTxt;

    void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        GameManager.instance.returnFromGame = true;
        game = DataManager.instance.userData.games[PlayerPrefs.GetInt("CurGame")];

        GameObject playerPrefab = Resources.Load<GameObject>("Prefabs/Characters/" + GameManager.instance.curCharacter.name);
        GameObject player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        anim = player.GetComponent<Animator>(); // Animator 초기화
        player.transform.SetParent(playerContainer, false);

        // 스폰 위치 초기화
        rockSpawnsPos[0] = rockSpawns[0].position;
        rockSpawnsPos[1] = rockSpawns[1].position;
        rockSpawnsPos[2] = rockSpawns[2].position;

        Time.timeScale = 1;

        StartCoroutine(CreateRockRoutine());
        StartCoroutine(SpeedUpRoutine());
        StartCoroutine(ScoreUpRoutine());
    }

    private void Update()
    {
        // 시작 시 walking 애니메이션 트리거
        anim.SetTrigger("walking");
    }

    // 바위 생성 코루틴
    IEnumerator CreateRockRoutine()
    {
        while (true)
        {
            // 0번 위치는 항상 생성
            CreateRock(0);

            // 1번 위치는 70% 확률로 생성
            if (Random.Range(0, 100) < 70)
            {
                CreateRock(1);

                // 2번 위치는 50% 확률로 생성 (1번이 생성된 경우에만)
                if (Random.Range(0, 100) < 50)
                {
                    CreateRock(2);
                }
            }

            // 코인 생성
            CreateCoin();

            rockSpawnRate = Random.Range(0.8f, 1.8f) / (0.5f + 0.05f * speed);
            yield return new WaitForSeconds(rockSpawnRate);
        }
    }

    // 바위 생성
    void CreateRock(int where)
    {
        GameObject rock = Instantiate(rockPrefabs[Random.Range(0, rockPrefabs.Length)],
                   rockSpawnsPos[where],
                   Quaternion.identity);

        // Sprite Renderer의 sortingOrder 설정
        SpriteRenderer renderer = rock.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.sortingOrder = rockSortingOrders[where];
        }
    }

    // 코인 생성
    void CreateCoin()
    {
        // 50% 확률 체크
        if (Random.Range(0, 100) < 50)
        {
            // 상단 바위(0번 위치)의 위쪽에 코인 생성
            Vector3 coinPos = rockSpawnsPos[0] + new Vector3(0, 1 + rockPrefabs[0].GetComponent<SpriteRenderer>().bounds.size.y / 2, 0);

            GameObject coin = Instantiate(coinPrefab, coinPos, Quaternion.identity);

            // 속도에 따른 코인 크기 조정
            if (speed > 18)
                coin.transform.localScale = Vector3.one * 1.5f;
            else if (speed > 12)
                coin.transform.localScale = Vector3.one * 1.3f;
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

    // 코인 획득 처리
    public void GetCoin()
    {
        int coinValue = speed > 18 ? 5 : (speed > 12 ? 3 : 2);
        DataManager.instance.userData.coin += coinValue;
        coin += coinValue;
        coinTxt.text = coin.ToString();
    }

    // 게임 오버 처리
    public void GameOver(bool isOver)
    {
        DataManager.instance.saveData();
        Time.timeScale = 0;

        StopAllCoroutines();

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

    // 재시작
    public void ReStartAction()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // 게임 재개
    public void ReturnAction()
    {
        StartCoroutine(CreateRockRoutine());
        StartCoroutine(SpeedUpRoutine());
        StartCoroutine(ScoreUpRoutine());

        gameOverPanel.SetActive(false);
        Time.timeScale = 1;
        topBar.SetActive(false);
    }

    // 게임 종료
    public void ExitAction()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainScene");
    }
}