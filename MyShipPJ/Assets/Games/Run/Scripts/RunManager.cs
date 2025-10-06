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
    Animator anim; // Animator �߰�

    public GameObject[] rockPrefabs;
    public Transform[] rockSpawns; // 0, 1, 2 positions
    public Vector3[] rockSpawnsPos = new Vector3[3];
    public float rockSpawnRate = 2f;

    public int[] rockSortingOrders = new int[3] { 1, 2, 3 }; // �� ��ġ�� sortingOrder ��

    public GameObject coinPrefab;

    [Header("GameTopBar & GameOverPanel")]
    public GameObject gameTopBar, topBar, gameOverPanel, reStartBtn, returnBtn;
    public TextMeshProUGUI scoreTxt, coinTxt, overCoinTxt, overTxt, overScoreTxt, overHighScoreTxt ;

    void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        GameManager.instance.returnFromGame = true;
        game = DataManager.instance.userData.games[PlayerPrefs.GetInt("CurGame")];

        GameObject playerPrefab = Resources.Load<GameObject>("Prefabs/Characters/" + GameManager.instance.curCharacter.name);
        GameObject player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        anim = player.GetComponent<Animator>(); // Animator �ʱ�ȭ
        player.transform.SetParent(playerContainer, false);

        // ���� ��ġ �ʱ�ȭ
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
        // ���� �� walking �ִϸ��̼� Ʈ����
        anim.SetTrigger("walking");
    }

    // ���� ���� �ڷ�ƾ
    IEnumerator CreateRockRoutine()
    {
        while (true)
        {
            // 0�� ��ġ�� �׻� ����
            CreateRock(0);

            // 1�� ��ġ�� 70% Ȯ���� ����
            if (Random.Range(0, 100) < 70)
            {
                CreateRock(1);

                // 2�� ��ġ�� 50% Ȯ���� ���� (1���� ������ ��쿡��)
                if (Random.Range(0, 100) < 50)
                {
                    CreateRock(2);
                }
            }

            // ���� ����
            CreateCoin();

            rockSpawnRate = Random.Range(0.8f, 1.8f) / (0.5f + 0.05f * speed);
            yield return new WaitForSeconds(rockSpawnRate);
        }
    }

    // ���� ����
    void CreateRock(int where)
    {
        GameObject rock = Instantiate(rockPrefabs[Random.Range(0, rockPrefabs.Length)],
                   rockSpawnsPos[where],
                   Quaternion.identity);

        // Sprite Renderer�� sortingOrder ����
        SpriteRenderer renderer = rock.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.sortingOrder = rockSortingOrders[where];
        }
    }

    // ���� ����
    void CreateCoin()
    {
        // 50% Ȯ�� üũ
        if (Random.Range(0, 100) < 50)
        {
            // ��� ����(0�� ��ġ)�� ���ʿ� ���� ����
            Vector3 coinPos = rockSpawnsPos[0] + new Vector3(0, 1 + rockPrefabs[0].GetComponent<SpriteRenderer>().bounds.size.y / 2, 0);

            GameObject coin = Instantiate(coinPrefab, coinPos, Quaternion.identity);

            // �ӵ��� ���� ���� ũ�� ����
            if (speed > 18)
                coin.transform.localScale = Vector3.one * 1.5f;
            else if (speed > 12)
                coin.transform.localScale = Vector3.one * 1.3f;
        }
    }

    // ���ǵ� ���� �ڷ�ƾ
    IEnumerator SpeedUpRoutine()
    {
        while (speed < 22f)
        {
            yield return new WaitForSeconds(speedUpInterval);
            speed += 0.5f;
        }
    }

    // ���� ���� �ڷ�ƾ
    IEnumerator ScoreUpRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            score += speed / 4;
            scoreTxt.text = "����: " + (int)score;
        }
    }

    // ���� ȹ�� ó��
    public void GetCoin()
    {
        int coinValue = speed > 18 ? 5 : (speed > 12 ? 3 : 2);
        DataManager.instance.userData.coin += coinValue;
        coin += coinValue;
        coinTxt.text = coin.ToString();
    }

    // ���� ���� ó��
    public void GameOver(bool isOver)
    {
        DataManager.instance.saveData();
        Time.timeScale = 0;

        StopAllCoroutines();

        overTxt.text = isOver ? "���ӿ���" : "�Ͻ�����";
        topBar.SetActive(true);
        returnBtn.SetActive(!isOver);
        reStartBtn.SetActive(isOver);
        overCoinTxt.text = "+ " + coin;
        overScoreTxt.text = "���� : " + (int)score;

        if (score > game.high_score)
            game.high_score = (int)score;

        // ���� ���� 
        List<(string name, int score)> rankList = new List<(string, int)>
{
    (game.other_name, game.other_score),
    (game.other1_name, game.other1_score),
    ("��", game.high_score)
};

        // ���� �������� ����
        rankList.Sort((a, b) => b.score.CompareTo(a.score));

        // ��� ���ڿ� ����
        string rankText = "���� ����\n\n";
        for (int i = 0; i < rankList.Count; i++)
        {
            rankText += $"{i + 1}�� {rankList[i].name} : {rankList[i].score}\n";
        }

        overHighScoreTxt.text = rankText;

        gameOverPanel.SetActive(true);
    }

    // �����
    public void ReStartAction()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // ���� �簳
    public void ReturnAction()
    {
        StartCoroutine(CreateRockRoutine());
        StartCoroutine(SpeedUpRoutine());
        StartCoroutine(ScoreUpRoutine());

        gameOverPanel.SetActive(false);
        Time.timeScale = 1;
        topBar.SetActive(false);
    }

    // ���� ����
    public void ExitAction()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainScene");
    }
}