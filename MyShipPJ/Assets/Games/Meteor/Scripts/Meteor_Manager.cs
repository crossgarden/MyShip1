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

    public GameObject meteorPrefab;
    public GameObject coinPrefab;
    public Vector3[] spawnPositions = new Vector3[20]; // 20���� ���� ��ġ
    private List<int> availablePositions = new List<int>(); // ��� ������ ��ġ �ε���

    [Header("GameTopBar & GameOverPanel")]
    public GameObject gameTopBar, topBar, gameOverPanel, reStartBtn, returnBtn;
    public TextMeshProUGUI scoreTxt, coinTxt, overCoinTxt, overTxt, overScoreTxt, overHighScoreTxt;

    void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        GameManager.instance.returnFromGame = true;
        game = DataManager.instance.userData.games[PlayerPrefs.GetInt("CurGame")];
        // ĳ���� ����
        GameObject playerPrefab = Resources.Load<GameObject>("Prefabs/Characters/" + GameManager.instance.curCharacter.name);
        GameObject player = Instantiate(playerPrefab, new Vector3(0, 0, 0), Unity.Mathematics.quaternion.identity);
        player.transform.SetParent(playerContainer, false);
        // playerAnim = player.GetComponent<Animator>();

        // 20���� ���� ��ġ �ʱ�ȭ (���� ��ġ, ���� ���ӿ� �°� ���� �ʿ�)
        for (int i = 0; i < 20; i++)
        {
            spawnPositions[i] = new Vector3(Random.Range(-10f, 10f), 6f, 0); // (X����, Y����, Z����)
        }

        Time.timeScale = 1;

        StartCoroutine("CreateMeteorAndCoinRoutine");
        StartCoroutine("SpeedUpRoutine");
        StartCoroutine("ScoreUpRoutine");
    }

    public void GameStart(GameObject startBtn)
    {
        Time.timeScale = 1;
        startBtn.SetActive(false);
    }

    // � & ���� ���� �ڷ�ƾ
    IEnumerator CreateMeteorAndCoinRoutine()
    {
        while (true)
        {
            // ��� ������ ��ġ ����Ʈ �ʱ�ȭ
            availablePositions.Clear();
            for (int i = 0; i < spawnPositions.Length; i++)
            {
                availablePositions.Add(i);
            }

            // speed�� ���� ��� ���� ���� ����
            int meteorCount = speed > 18 ? 13 : (speed > 12 ? 9 : 5);
            int coinCount = speed > 18 ? 3 : (speed > 12 ? 2 : 1);

            // � ����
            for (int i = 0; i < meteorCount && availablePositions.Count > 0; i++)
            {
                int randomIndex = Random.Range(0, availablePositions.Count);
                int posIndex = availablePositions[randomIndex];
                availablePositions.RemoveAt(randomIndex);

                Instantiate(meteorPrefab, spawnPositions[posIndex], Quaternion.identity);
            }

            // ���� ����
            for (int i = 0; i < coinCount && availablePositions.Count > 0; i++)
            {
                int randomIndex = Random.Range(0, availablePositions.Count);
                int posIndex = availablePositions[randomIndex];
                availablePositions.RemoveAt(randomIndex);

                GameObject coin = Instantiate(coinPrefab, spawnPositions[posIndex], Quaternion.identity);

                // speed�� ���� ���� ũ�� ����
                if (speed > 18)
                    coin.transform.localScale = coin.transform.localScale * 1.5f;
                else if (speed > 12)
                    coin.transform.localScale = coin.transform.localScale * 1.3f;
            }

            yield return new WaitForSeconds(1f); // ���� ���� ���� ����
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

    // ���� ����
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

    // ���� ����
    public void GameOver(bool isOver)
    {
        DataManager.instance.saveData();
        Time.timeScale = 0;

        StopCoroutine("CreateMeteorAndCoinRoutine");
        StopCoroutine("SpeedUpRoutine");
        StopCoroutine("ScoreUpRoutine");

        overTxt.text = isOver ? "���ӿ���" : "�Ͻ�����";

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
    }

    // [1-3] ���� ���� �г� ��ư �׼ǵ�
    public void ReStartAction()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnAction()
    {
        StartCoroutine("CreateMeteorAndCoinRoutine");
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