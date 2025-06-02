using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerLch : MonoBehaviour
{
    public static GameManagerLch instance; //ㅅㄱㅌ

    public GameObject[] enemyObjs;
    public GameObject[] spawnPoints;
    public GameObject spawnPointGroup;
    public float maxSpawnDelay =  3f;
    public float curSpawnDelay;

    private float A = 0.01f;

    public float minSpawnDelay = 0.5f;

    public GameObject player; //ㅅㄱㅌ

   
    void Awake()
    {
        instance = this;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        spawnPointGroup = GameObject.FindGameObjectWithTag("SpawnPointGroup");

        if (spawnPointGroup == null)
        {
            return;
        }
   
        for (int i = 0; i < spawnPointGroup.transform.childCount; i++)
        {
            Transform child = spawnPointGroup.transform.GetChild(i);
            spawnPoints[i] = child.gameObject;
        }
    }

    void Update()
    {
        curSpawnDelay += Time.deltaTime;

        if (curSpawnDelay > maxSpawnDelay)
        {
            SpawnEnemy();
            curSpawnDelay = 0;

            // maxSpawnDelay 점점 줄이기
            maxSpawnDelay -= 0.1f;
            if (maxSpawnDelay < minSpawnDelay)
            {
                maxSpawnDelay = minSpawnDelay;
            }
        }
    }

    void SpawnEnemy()
    {
        int ranEnemy = Random.Range(0, 3);
        int ranPoint = Random.Range(0, 9);

        if (enemyObjs[ranEnemy] == null || spawnPoints[ranPoint] == null)
        {
            return;
        }



        GameObject enemy = Instantiate(enemyObjs[ranEnemy],
                                        spawnPoints[ranPoint].transform.position,
                                         spawnPoints[ranPoint].transform.rotation);

        Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();
        Enemy enemyLogic = enemy.GetComponent<Enemy>();
        enemyLogic.player = player; //싱글턴에서 player 참조

        if (ranPoint == 5 || ranPoint == 6)
        { //오른
            rigid.velocity = new Vector2(enemyLogic.speed * (-1), -1);
            enemy.transform.Rotate(Vector3.back * 70);
        }
        else if (ranPoint == 7 || ranPoint == 8)
        {  //왼 
            rigid.velocity = new Vector2(enemyLogic.speed, -1);
            enemy.transform.Rotate(Vector3.forward * 70);
        }
        else
        { // 직선구간
            rigid.velocity = new Vector2(0, enemyLogic.speed * (-1-A));
            A -= 0.01f;
        }
    }
}