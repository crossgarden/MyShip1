using System.Collections;
using System.Collections.Generic;
using GameData;
using UnityEditor;
using UnityEngine;

public class MainChaContainer : MonoBehaviour
{
    public List<GameObject> characterPrefabs;
    public GameObject curCharacterObj;
    readonly string path = "Prefabs/Characters/";

    void Start()
    {
        // 캐릭터 프리팹 List에 프리팹 추가
        foreach (Character cha in DataManager.instance.characterSotred)
            characterPrefabs.Add(Resources.Load<GameObject>(path + cha.name));

        ChangeCharacter();

        // blink 애니메이션 코루틴 시작
        StartCoroutine(BlinkAnimation());

        // 시작할 때 불 상태 확인
        CheckLightStatus();
    }

    void Update()
    {
        // 실시간으로 불 상태와 방 변화 감지
        CheckLightStatus();

        if (Input.GetMouseButtonDown(0)) // 터치/클릭 감지
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hitCollider = Physics2D.OverlapPoint(worldPos);

            if (hitCollider != null && hitCollider.transform.IsChildOf(this.transform))
            {
                //Debug.Log("캐릭터 터치 감지!");

                // 터치된 오브젝트의 자식에서 Animator 찾기
                Animator animator = hitCollider.GetComponentInChildren<Animator>();

                if (animator != null && !animator.GetBool("IsSleeping"))
                {
                    //Debug.Log("Touch 트리거 발동!");
                    animator.SetTrigger("Touch");
                }
                else
                {
                    //Debug.Log("Animator를 찾을 수 없습니다!");
                }
            }
        }
    }

    void CheckLightStatus()
    {
        if (gameObject.transform.childCount > 0)
        {
            Transform currentCharacter = gameObject.transform.GetChild(0);
            Animator animator = currentCharacter.GetComponent<Animator>();

            if (animator != null)
            {
                bool isInPrivateRoom = PlayerPrefs.GetInt("CurRoom", 0) == (int)RoomNum.PRIVATE;
                bool isLightOff = PlayerPrefs.GetInt("LightOn", 0) == 0;
                bool shouldSleep = isInPrivateRoom && isLightOff;

                animator.SetBool("IsSleeping", shouldSleep);
            }
        }
    }

    public void ChangeCharacter()
    {
        if (gameObject.transform.childCount > 0)
        {
            Destroy(gameObject.transform.GetChild(0).gameObject);
        }

        GameObject curCharacterObj = Instantiate(characterPrefabs[PlayerPrefs.GetInt("CurCharacter", 0)], transform.position, Quaternion.identity);
        curCharacterObj.transform.SetParent(gameObject.transform, false);
        curCharacterObj.SetActive(true);

        // 캐릭터 변경 후 blink 애니메이션 다시 시작
        StopCoroutine(BlinkAnimation());
        StartCoroutine(BlinkAnimation());

        // 캐릭터 변경 후 즉시 상태 체크
        CheckLightStatus();
    }

    // 5초마다 blink 애니메이션 실행
    IEnumerator BlinkAnimation()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f); // 5초 대기

            // 현재 활성화된 캐릭터의 Animator 찾기
            if (gameObject.transform.childCount > 0)
            {
                Transform currentCharacter = gameObject.transform.GetChild(0);
                Animator animator = currentCharacter.GetComponent<Animator>();

                 // 잠들지 않은 상태에서만 깜빡임
                if (animator != null && !animator.GetBool("IsSleeping"))
                {
                    animator.SetTrigger("Blink");
                }
            }
        }
    }
}