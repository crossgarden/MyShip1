using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class STHManager : MonoBehaviour
{
    const int UP = 0;
    const int SHIFT = 1;
    const int LEFT = -1;
    const int RIGHT = 1;

    public GameObject player;
    int playerDir = LEFT;  // -1 - 왼쪽, 1 - 오른쪽
    public int score = 0;

    readonly int tileCount = 40;
    List<int> tiles = new List<int>(40);  // 0: 오르기 , 1:시프트
    Vector3 tileSize;
    int tileDir = LEFT;
    int maxStraight = 0;

    public GameObject tilePrefab;
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

    void Start()
    {
        // 캐릭터 생성
        GameObject playerPrefab = Resources.Load<GameObject>("Prefabs/Characters/" + GameManager.instance.curCharacter.name);
        player = Instantiate(playerPrefab, new Vector3(0, -1, 0), quaternion.identity);
        player.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
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

            GameObject tile = Instantiate(tilePrefab, transform.position, quaternion.identity);
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

            GameObject tile = Instantiate(tilePrefab, transform.position, quaternion.identity);
            tile.transform.position = tilePos;
            tile.transform.SetParent(tilesContainer2, false);
            tile.SetActive(true);
        }
    }

    public void Action(int isShift)
    {
        // if (isShift == 1)
        // {
        //     playerDir *= -1;
        //     Vector3 playerPos = player.transform.localScale;
        //     playerPos.x = -playerPos.x;
        //     player.transform.localScale = playerPos;
        // }

        // // 타일 아래로 내리기
        // Vector3 movement = Vector3.down * tileSize.y + new Vector3(-playerDir, 0, 0) * tileSize.x;
        // tilesContainer1.position += movement;
        // tilesContainer2.position += movement;

        // // 배경 내리기
        // foreach (Transform bg in backgrounds)
        //     bg.position += movement / 4;

        // foreach (Transform bg in backgrounds)
        // {
        //     Vector3 bgPos = bg.position;
        //     if (bg.position.x >= x)
        //         bgPos.x -= bgSize.x * 2;
        //     if (bg.position.x <= -x)
        //         bgPos.x += bgSize.x * 2;
        //     if (bg.position.y <= y)
        //         bgPos.y += bgSize.y * 2;

        //     bg.position = bgPos;
        // }

        // if (bottomContainer != null)
        //     bottomContainer.transform.position += movement;

        // if (isShift == tiles[0])
        //     Success();
        // else
        //     Fail();


        /** 테스트용 무조건 성공 코드*/
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

        if (bottomContainer != null)
            bottomContainer.transform.position += movement;

        Success();

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

            GameObject tile = Instantiate(tilePrefab, tilePos, quaternion.identity);
            tile.transform.SetParent(newContainer, false);
            tile.SetActive(true);
        }

    }

    public void Fail()
    {

    }

}
