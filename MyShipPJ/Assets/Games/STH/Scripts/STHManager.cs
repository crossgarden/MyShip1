using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameData;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class STHManager : MonoBehaviour
{
    Game game;
    public TextMeshProUGUI scoreTxt;
    public int score = 0;

    const int UP = 0;
    const int SHIFT = 1;
    const int LEFT = -1;
    const int RIGHT = 1;

    public GameObject player;
    public Transform playerContainer;
    int playerDir = LEFT;  // -1 - 왼쪽, 1 - 오른쪽

    readonly int tileCount = 40;
    public List<int> tiles = new List<int>(40);  // 0: 오르기 , 1:시프트
    Vector3 tileSize;
    int tileDir = LEFT;
    int maxStraight = 0;

    public GameObject tilePrefab;
    public GameObject coinTilePrefab;
    public Transform firstTile;
    Vector3 tilePos;
    public Transform tilesContainer1;
    public Transform tilesContainer2;
    public GameObject bottomContainer;
    int whichTiles = 2;

    public List<Transform> backgrounds;
    Vector3 bgSize;
    readonly float x = 6.25f;
    readonly float y = -13f;

    public Slider HPSlider;
    public Image HPSliderFill;
    public float stayTime = 0;
    float limit = 2f;
    bool start = false;

    readonly float[] timeLimit = { 2.2f, 2f, 1.8f, 1.6f, 1.4f, 1.2f, 1f, 0.8f, 0.8f, 0.8f, 0.8f };    // 10초 경과할 때마다 제한 올리기기
    int timeLimitIndex = 0;

    public GameObject gameOverPanel, reStartBtn, returnBtn;
    public TextMeshProUGUI overTxt, overScoreTxt, HighScoreTxt;
    bool pause = false;

    void Start()
    {
        game = DataManager.instance.userData.games[PlayerPrefs.GetInt("CurGame")];
        // 캐릭터 생성
        GameObject playerPrefab = Resources.Load<GameObject>("Prefabs/Characters/" + GameManager.instance.curCharacter.name);
        player = Instantiate(playerPrefab, new Vector3(0, -1, 0), quaternion.identity);
        player.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        player.transform.SetParent(playerContainer, false);
        player.SetActive(true);

        // 타일 큐 생성
        maxStraight = 0;

        tiles.Add(UP);
        for (int i = 0; i < tileCount - 1; i++)
        {
            int x = UnityEngine.Random.Range(0, 5);
            if (x == 4 || maxStraight == 5)
            {
                tiles.Add(SHIFT);
                maxStraight = 0;
            }
            else
            {
                tiles.Add(UP);
                maxStraight++;
            }
        }

        tileSize = tilePrefab.GetComponent<Renderer>().bounds.size;
        bgSize = backgrounds[0].GetComponent<Renderer>().bounds.size;

        InitTile();
        StartCoroutine("UpTimeLimit");

        Time.timeScale = 0;
    }

    void Update()
    {
        if (pause) return;
        
        stayTime += Time.deltaTime;
        HPSlider.value -= Time.deltaTime;
        SetSliderColor();

        if (stayTime > limit)
        {
            Fail(true);
        }

        // 키보드 입력으로도 action()
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Action(0);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Action(1);
        }
    }

    IEnumerator UpTimeLimit()
    {
        while (timeLimitIndex < timeLimit.Length)
        {
            yield return new WaitForSeconds(10);
            HPSlider.maxValue = timeLimit[timeLimitIndex];
            limit = timeLimit[timeLimitIndex];

            timeLimitIndex++;
        }
    }

    void SetSliderColor()
    {
        float red = Mathf.Round(255 * (1 - HPSlider.value / HPSlider.maxValue)); // 값이 0에 가까울수록 빨강
        float green = Mathf.Round(255 * HPSlider.value / HPSlider.maxValue);    // 값이 1에 가까울수록 초록

        HPSliderFill.color = new Color(red / 255, green / 255, 0);
    }

    void InitTile()
    {
        tilePos = firstTile.position;

        // tileContainer 1 채우기
        for (int i = 0; i < tileCount / 2; i++)
        {
            if (tiles[i] == 1)  // 시프트면 방향 바꾸기
                tileDir *= -1;

            tilePos += new Vector3(tileDir, 0, 0) * tileSize.x;
            tilePos += Vector3.up * tileSize.y;

            GameObject tile;

            if (UnityEngine.Random.Range(0, 3) == 0)
                tile = Instantiate(coinTilePrefab, transform.position, quaternion.identity);
            else
                tile = Instantiate(tilePrefab, transform.position, quaternion.identity);

            tile.transform.SetParent(tilesContainer1, false);
            tile.transform.position = tilePos;
            tile.SetActive(true);
        }

        // tileContainer 2 채우기
        tilesContainer2.transform.position = new Vector3(0, tilePos.y, 0);
        tilePos = new Vector3(tilePos.x, 0, 0);

        for (int i = tileCount / 2; i < tileCount; i++)
        {
            if (tiles[i] == 1)  // 시프트면 방향 바꾸기
                tileDir *= -1;

            tilePos += new Vector3(tileDir, 0, 0) * tileSize.x;
            tilePos += Vector3.up * tileSize.y;

            GameObject tile;

            if (UnityEngine.Random.Range(0, 3) == 0)
                tile = Instantiate(coinTilePrefab, transform.position, quaternion.identity);
            else
                tile = Instantiate(tilePrefab, transform.position, quaternion.identity);

            tile.transform.position = tilePos;
            tile.transform.SetParent(tilesContainer2, false);
            tile.SetActive(true);
        }
    }

    public void Action(int isShift)
    {
        if (!start)
        {
            start = true;
            Time.timeScale = 1;
        }

        stayTime = 0;
        HPSlider.value = HPSlider.maxValue;

        AudioManager.instance.PlaySFX(AudioManager.SFXClip.CLICK);
        if (isShift == 1)
        {
            playerDir *= -1;
            Vector3 playerPos = player.transform.localScale;
            playerPos.x = -playerPos.x;
            player.transform.localScale = playerPos;
        }

        // 타일 아래로 내리기
        Vector3 movement = Vector3.down * tileSize.y + new Vector3(-playerDir, 0, 0) * tileSize.x;
        tilesContainer1.position += movement;
        tilesContainer2.position += movement;

        if (bottomContainer != null)
            bottomContainer.transform.position += movement;

        // 배경 내리기
        foreach (Transform bg in backgrounds)
            bg.position += movement / 4;

        foreach (Transform bg in backgrounds)
        {
            Vector3 bgPos = bg.position;
            if (bg.position.x >= x)
                bgPos.x -= bgSize.x * 2;
            if (bg.position.x <= -x)
                bgPos.x += bgSize.x * 2;
            if (bg.position.y <= y)
                bgPos.y += bgSize.y * 2;

            bg.position = bgPos;
        }

        if (isShift == tiles[0])
            Success();
        else
            Fail(true);

        /** 테스트용 무조건 성공 코드*/
        /**
        if (tiles[0] == 1)
        {
            playerDir *= -1;
            Vector3 playerPos = player.transform.localScale;
            playerPos.x = -playerPos.x;
            player.transform.localScale = playerPos;
        }

        // 타일 아래로 내리기
        Vector3 movement = Vector3.down * tileSize.y + new Vector3(-playerDir, 0, 0) * tileSize.x;
        tilesContainer1.position += movement;
        tilesContainer2.position += movement;

        if (bottomContainer != null)
            bottomContainer.transform.position += movement;

        // 배경 내리기
        foreach (Transform bg in backgrounds)
            bg.position += movement / 4;

        foreach (Transform bg in backgrounds)
        {
            Vector3 bgPos = bg.position;
            if (bg.position.x >= x)
                bgPos.x -= bgSize.x * 2;
            if (bg.position.x <= -x)
                bgPos.x += bgSize.x * 2;
            if (bg.position.y <= y)
                bgPos.y += bgSize.y * 2;

            bg.position = bgPos;
        }
        Success();
        */

    }

    public void Success()
    {
        tiles.RemoveAt(0);
        int x = UnityEngine.Random.Range(0, 5);
        if (x == 4 || maxStraight == 5)
        {
            tiles.Add(SHIFT);
            maxStraight = 0;
        }
        else
        {
            tiles.Add(UP);
            maxStraight++;
        }

        // 점수 올리기
        score += 1;
        scoreTxt.text = "점수 : " + score;

        if (score % (tileCount / 2) == 0)
        { // 30점 올릴때마다 타일 재생성
            if (whichTiles == 2)
            {
                CreateTileContainer(tilesContainer1, tilesContainer2);
                whichTiles = 1;
            }
            else
            {
                CreateTileContainer(tilesContainer2, tilesContainer1);
                whichTiles = 2;
            }
        }
    }

    // 타일 재생성 후 위로 올리기 
    void CreateTileContainer(Transform newContainer, Transform oldContainer)
    {
        if (bottomContainer != null)
            Destroy(bottomContainer);

        bottomContainer = Instantiate(newContainer.gameObject, newContainer.position, quaternion.identity);
        bottomContainer.SetActive(true);

        for (int i = 0; i < newContainer.childCount; i++)
        {
            Destroy(newContainer.GetChild(i).gameObject);
        }

        tilePos = oldContainer.GetChild(oldContainer.childCount - 1).position;
        newContainer.position = new Vector3(0, tilePos.y, 0);
        tilePos = new Vector3(tilePos.x, 0, 0);

        for (int i = tileCount / 2; i < tileCount; i++)
        {
            if (tiles[i] == 1)  // 시프트면 방향 바꾸기
                tileDir *= -1;

            tilePos += new Vector3(tileDir, 0, 0) * tileSize.x;
            tilePos += Vector3.up * tileSize.y;

            GameObject tile;

            if (UnityEngine.Random.Range(0, 3) == 0)
                tile = Instantiate(coinTilePrefab, tilePos, quaternion.identity);
            else
                tile = Instantiate(tilePrefab, tilePos, quaternion.identity);

            tile.transform.SetParent(newContainer, false);
            tile.SetActive(true);
        }
    }

    public void Fail(bool isFail)
    {
        if (score > game.high_score)
        {
            game.high_score = score;
        }

        if (isFail)
        {
            overTxt.text = "게임오버";
            reStartBtn.SetActive(true);
            returnBtn.SetActive(false);
        }
        else
        {
            overTxt.text = "일시정지";
            reStartBtn.SetActive(false);
            returnBtn.SetActive(true);
        }

        overScoreTxt.text = "점수: " + score;
        HighScoreTxt.text = "최고: " + game.high_score;
        gameOverPanel.SetActive(true);
        DataManager.instance.saveData();
        Time.timeScale = 0;
    }

    public void ReStartAction()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PauseAction()
    {
        pause = true;
        Time.timeScale = 0;
        StopCoroutine("UpTimeLimit");
        Fail(false);
    }

    public void returnAction(){
        pause = false;
        Time.timeScale = 1;
        StartCoroutine("UpTimeLimit");
        gameOverPanel.SetActive(false);
    }

    public void ExitAction()
    {
        SceneManager.LoadScene("MainScene");
    }
}
