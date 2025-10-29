using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using GameData;

public class SSManager : MonoBehaviour
{
    Game game;
    public float score = 0;
    public int coin = 0;


    public Transform playerContainer;
    public float speed;
    public float power;
    public Joystick joystick;


    public float GameSpeed = 6f;
    public int GameSpeedUpInterval = 5;
    public float EnemySpeed = 1f;

    public GameObject Bullets_0;
    public GameObject Bullets_1;

    public float fireRate = 0.15f;         // �߻� �ֱ�
    private float fireTimer = 0f;

    private GameObject player;

    public static SSManager instance;
    public GameObject coinPrefab;
    [Header("GameTopBar & GameOverPanel")]
    public GameObject gameTopBar, topBar, gameOverPanel, reStartBtn, returnBtn;
    public TextMeshProUGUI scoreTxt, coinTxt, overCoinTxt, overTxt, overScoreTxt, overHighScoreTxt;


    void Awake()
    {
        instance = this;
    }
    public void AddCoin(int amount)
    {
        coin += amount;
        coinTxt.text = coin.ToString(); 
    }
    public class PlayerCoinTrigger : MonoBehaviour
    {
        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Coin"))
            {
                SSManager.instance.AddCoin(1);
                AudioManager.instance.PlaySFX(AudioManager.SFXClip.SUCCESS);
                Destroy(collision.gameObject);
            }
        }
    }

    void Start()
    {
        
        Time.timeScale = 1;

        StartCoroutine("GameSpeedUpRoutine");
        StartCoroutine("ScoreUpRoutine");

        //�������� �߰��϶��Ѱ� 

        GameManager.instance.returnFromGame = true;
        game = DataManager.instance.userData.games[PlayerPrefs.GetInt("CurGame")];

        // �÷��̾� ������ �ҷ�����
        GameObject playerPrefab = Resources.Load<GameObject>("Prefabs/Characters/" + GameManager.instance.curCharacter.name);
        player = Instantiate(playerPrefab, new Vector3(0, -4, 0), Quaternion.identity);
        player.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
        player.transform.SetParent(playerContainer, false);
        // player.SetActive(true);


        if (player.GetComponent<Collider2D>() == null)
        {
            BoxCollider2D col = player.AddComponent<BoxCollider2D>();
            col.isTrigger = true; // OnTriggerEnter2D�� ����
            col.size = new Vector2(2f, 3f);
        }

        // �����ٵ� 
        if (player.GetComponent<Rigidbody2D>() == null)
        {
            Rigidbody2D rb = player.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.isKinematic = false;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        // �±� ����
        player.tag = "Player";

        GameManagerLch.instance.player = player;

        if (player.GetComponent<PlayerCoinTrigger>() == null)
        {
            player.AddComponent<PlayerCoinTrigger>();
        }
    }

    IEnumerator GameSpeedUpRoutine()
    {
        while (GameSpeed < 22f || EnemySpeed < 22f)
        {
            yield return new WaitForSeconds(GameSpeedUpInterval);

            if (GameSpeed < 22f)
            {
                GameSpeed += 0.5f;
                
            }
        }
    }

    IEnumerator ScoreUpRoutine()
    {

        while (true)
        {
            yield return new WaitForSeconds(1);
            score += GameSpeed / 4;
            Debug.Log("���� ����: " + score);
            scoreTxt.text = "����: " + (int)score;
        }
    }



    void Update()
    {
        move();


        fireTimer += Time.deltaTime;
        if (fireTimer >= fireRate)
        {
            fire();
            fireTimer = 0f;
        }


    }

    void move()
    {
        if(player == null)
        {
            return;
        }
        // ���̽�ƽ �̵�
        float h = joystick.Horizontal;
        float v = joystick.Vertical;

        Vector3 curPos = player.transform.position;
        Vector3 nextPos = new Vector3(h, v, 0) * speed * Time.deltaTime;
        Vector3 newPos = curPos + nextPos;

        // ȭ�� �����ڸ� ���� ���
        Vector3 playerHalfSize = Vector3.zero;
        SpriteRenderer sr = player.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
            playerHalfSize = sr.bounds.extents;

        Vector3 min = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 max = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));

        newPos.x = Mathf.Clamp(newPos.x, min.x + playerHalfSize.x, max.x - playerHalfSize.x);
        newPos.y = Mathf.Clamp(newPos.y, min.y + playerHalfSize.y, max.y - playerHalfSize.y);

        player.transform.position = new Vector3(newPos.x, newPos.y, 0);
    }

    void fire()
    {
        if (player == null)
        {
            return;
        }
        Vector3 firePos = player.transform.position + new Vector3(0, 0.5f, 0);


        switch (power)
        {
            case 1:
                // �Ŀ� 1
                GameObject bullet = Instantiate(Bullets_0, firePos, Quaternion.identity);
                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            case 2:
                GameObject bulletR = Instantiate(Bullets_0, firePos + Vector3.right * 0.1f, Quaternion.identity);
                GameObject bulletL = Instantiate(Bullets_0, firePos + Vector3.left * 0.1f, Quaternion.identity);
                Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
                rigidR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            case 3:
                GameObject bulletRR = Instantiate(Bullets_0, firePos + Vector3.right * 0.25f, Quaternion.identity);
                GameObject bulletLL = Instantiate(Bullets_0, firePos + Vector3.left * 0.25f, Quaternion.identity);
                GameObject bulletCC = Instantiate(Bullets_1, firePos, Quaternion.identity);
                Rigidbody2D rigidRR = bulletRR.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidLL = bulletLL.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidCC = bulletCC.GetComponent<Rigidbody2D>();
                rigidRR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidLL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidCC.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;


        }


    }

    // ���� ����
    public void GameOver(bool isOver)
    {
        DataManager.instance.saveData();
        Time.timeScale = 0;

        StopCoroutine("GameSpeedUpRoutine");
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
        StartCoroutine("GameSpeedUpRoutine");
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