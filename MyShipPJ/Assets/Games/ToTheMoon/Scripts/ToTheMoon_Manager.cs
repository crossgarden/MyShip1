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
    public float baseTimeScale = 1.0f; // �⺻ �ð� �ӵ�
    public float maxTimeScale = 3.67f; // �ִ� �ð� �ӵ� (22f / 6f)
    public float timeScaleIncreaseInterval = 5f; // �ӵ� ���� ����
    public float timeScaleIncrement = 0.0833f; // �ӵ� ������

    [Header("Cloud Settings")]
    public GameObject cloudPrefab; // ���� ������
    public float cloudSpawnY = 5.3f; // ���� ���� y ��ġ
    public Vector2 cloudSpawnXRange = new Vector2(-2.2f, 2.2f); // ���� ���� x ����
    public float cloudSpawnInterval = 1f; // ���� ���� ����

    [Header("Background")]
    public Transform bg1, bg2; // �� ���� ��� ������Ʈ
    private Vector3 bgSize; // ��� ũ��
    private readonly float bgYThreshold = -13f; // ��� ���� ��ġ
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

        // ĳ���� ����
        GameObject playerPrefab = Resources.Load<GameObject>("Prefabs/Characters/" + GameManager.instance.curCharacter.name);
        GameObject player = Instantiate(playerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        player.transform.SetParent(playerContainer, false);

        // ��� �ʱ�ȭ
        bgSize = bg1.GetComponent<Renderer>().bounds.size;

        Time.timeScale = baseTimeScale;
        StartCoroutine("ScoreUpRoutine");
        StartCoroutine("IncreaseTimeScaleRoutine");
        StartCoroutine("MoveBackgroundRoutine"); // ��� �̵� �ڷ�ƾ �߰�
        StartCoroutine("CreateCloudRoutine"); // ���� ���� �ڷ�ƾ �߰�
    }

    // �ð� �ӵ� ���� �ڷ�ƾ
    IEnumerator IncreaseTimeScaleRoutine()
    {
        while (Time.timeScale < maxTimeScale)
        {
            yield return new WaitForSecondsRealtime(timeScaleIncreaseInterval); // ���� �ð� ���� ���
            Time.timeScale = Mathf.Min(Time.timeScale + timeScaleIncrement, maxTimeScale);
        }
    }

    // ���� ���� �ڷ�ƾ (Meteor_Manager ����)
    IEnumerator CreateCloudRoutine()
    {
        while (true)
        {
            // ���� x ��ġ���� ���� ����
            float randomX = Random.Range(cloudSpawnXRange.x, cloudSpawnXRange.y);
            Vector3 spawnPos = new Vector3(randomX, cloudSpawnY, 0);

            // ���� ����
            GameObject cloud = Instantiate(cloudPrefab, spawnPos, Quaternion.identity);

            // 30% Ȯ���� ���� ���� ���� ����
            if (Random.Range(0f, 1f) < 0.3f)
            {
                // ���� ��ġ ��� (���� �ٷ� �� �߾�)
                Vector3 coinPos = new Vector3(
                    cloud.transform.position.x,
                    cloud.transform.position.y + 0.4f, // ���� ���� ��ġ�ϵ��� y�� ����
                    0
                );

                Instantiate(coinPrefab, coinPos, Quaternion.identity);
            }

            yield return new WaitForSeconds(cloudSpawnInterval);
        }
    }

    // ��� �̵� �ڷ�ƾ
    IEnumerator MoveBackgroundRoutine()
    {
        while (true)
        {
            yield return null;

            // ��� �̵� �ӵ� ��� (���� �� ���)
            BackgroundMoveSpeed = 1.5f; // ���ϴ� ��� �⺻ �ӵ�
            float moveAmount = BackgroundMoveSpeed * Time.deltaTime;

            bg1.position += Vector3.down * moveAmount;
            bg2.position += Vector3.down * moveAmount;

            // ��� ���� ó��
            if (bg1.position.y < bgYThreshold)
            {
                bg1.position += new Vector3(0, bgSize.y * 2, 0);
            }
            if (bg2.position.y < bgYThreshold)
            {
                bg2.position += new Vector3(0, bgSize.y * 2, 0);
            }

            // ��� ���� ����
            UpdateBackgroundColor();
        }
    }

    // ������ ���� ��� ���� ������Ʈ
    void UpdateBackgroundColor()
    {
        Color targetColor = GetTargetColor((int)score);
        Color targetColorCloud = GetTargetColorCloud((int)score);

        bg1.GetComponent<SpriteRenderer>().color = Color.Lerp(
            bg1.GetComponent<SpriteRenderer>().color, targetColor, Time.deltaTime * 2);
        bg2.GetComponent<SpriteRenderer>().color = Color.Lerp(
            bg2.GetComponent<SpriteRenderer>().color, targetColor, Time.deltaTime * 2);

        // ���� ���̾ �ִٸ� (bg1�� bg2�� �ڽ� ������Ʈ�� ����)
        if (bg1.childCount > 0)
        {
            bg1.GetChild(0).GetComponent<SpriteRenderer>().color = Color.Lerp(
                bg1.GetChild(0).GetComponent<SpriteRenderer>().color, targetColorCloud, Time.deltaTime * 2);
            bg2.GetChild(0).GetComponent<SpriteRenderer>().color = Color.Lerp(
                bg2.GetChild(0).GetComponent<SpriteRenderer>().color, targetColorCloud, Time.deltaTime * 2);
        }
    }

    // ��� ���� ���
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

    //// ���ǵ� ���� �ڷ�ƾ
    //IEnumerator SpeedUpRoutine()
    //{
    //    while (speed < 22f)
    //    {
    //        yield return new WaitForSeconds(speedUpInterval);
    //        speed += 0.5f;
    //    }
    //}

    // ���� ���� �ڷ�ƾ
    IEnumerator ScoreUpRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            score += (6f * Time.timeScale) / 4; // baseSpeed(6f) * timeScale / 4
            scoreTxt.text = "����: " + (int)score;
        }
    }

    // ���� ���� (�ð� �ӵ� ����)
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

    // ���� ����
    public void GameOver(bool isOver)//�α׸� ���ܾ� �ϴ� ����: ������ �ȳ�, �Ƹ� ��������� ������ �ִ� ��
    {
        pausedTimeScale = Time.timeScale;
        Time.timeScale = 0; // ���� �Ͻ�����

        Debug.Log("[1] GameOver ȣ���"); // 1. �޼��� ���� Ȯ��

        try
        {
            Debug.Log("[2] UI ��� Ȯ�� ��");
            if (overTxt == null) Debug.LogError("overTxt null");
            if (topBar == null) Debug.LogError("topBar null");
            if (gameOverPanel == null) Debug.LogError("gameOverPanel null");

            Debug.Log("[3] DataManager Ȯ��");
            if (DataManager.instance == null) Debug.LogError("DataManager null");
            if (game == null) Debug.LogError("game null");

            DataManager.instance.saveData();
            Debug.Log("[4] ���� �Ϸ�");

            Time.timeScale = 0;
            Debug.Log("[5] TimeScale 0 ����");

            StopCoroutine("ScoreUpRoutine");
            StopCoroutine("IncreaseTimeScaleRoutine");
            StopCoroutine("MoveBackgroundRoutine");
            StopCoroutine("CreateCloudRoutine");
            Debug.Log("[6] �ڷ�ƾ ����");

            overTxt.text = isOver ? "���ӿ���" : "�Ͻ�����";
            Debug.Log("[7] �ؽ�Ʈ ���� �Ϸ�");

            topBar.SetActive(true);

            returnBtn.SetActive(!isOver);
            reStartBtn.SetActive(isOver);
            overCoinTxt.text = "+ " + coin;
            overScoreTxt.text = "���� : " + (int)score;
            if (score > game.high_score)
                game.high_score = (int)score;

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

            Debug.Log("[8] UI Ȱ��ȭ �Ϸ�");

            gameOverPanel.SetActive(true);
            Debug.Log("[9] ���ӿ��� �г� Ȱ��ȭ");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"���� �߻�: {e.Message}\n{e.StackTrace}");
        }
    }

    // [1-3] ���� ���� �г� ��ư �׼ǵ�
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