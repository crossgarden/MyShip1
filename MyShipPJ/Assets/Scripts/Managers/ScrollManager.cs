using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollManager : MonoBehaviour
{

    // 배경 크기 1080 x 1920에 PPU 192
    // 가로 길이는 Orthographic Size = 5 기준 5.625

    public Transform bgController;
    public List<GameObject> backgrounds;
    float bgDistance = 5.625f;

    float bgChangeEndDistance = 3f;
    bool needUIChange = false;

    public int roomCount;
    public int curRoom; // PlayerPrefs - "CurRoom"
    public List<GameObject> leftBotUI;
    public List<GameObject> midBotUI;
    public List<GameObject> rightBotUI;


    void Start()
    {
        roomCount = backgrounds.Count;
        for (int i = 1; i < roomCount; i++)
        {
            backgrounds[i].transform.position = new Vector3(bgDistance * i, 0, 0);
        }
        curRoom = PlayerPrefs.GetInt("CurRoom", 0);
        bgController.position = new Vector3(curRoom * -bgDistance, 0, 0);
        BottomBarUIChange();
    }


    void Update()
    {
        // room 이동.
        Vector3 targetPos = new Vector3((curRoom) * -bgDistance, 0, 0);
        bgController.position = Vector3.Lerp(bgController.position, targetPos, Time.deltaTime * 10);

        if (needUIChange && Mathf.Abs(bgController.position.x - targetPos.x) < bgChangeEndDistance)
        {
            needUIChange = false;
            BottomBarUIChange();
        }

    }

    // room 체인지 버튼 이벤트
    public void MoveRoomAction(int next) // next: 1, pre: -1
    {
        if(curRoom + next < 0 || curRoom + next >= backgrounds.Count)
            return;

        AudioManager.instance.PlaySFX(GameData.SFXClip.SLIDE);
        curRoom = Mathf.Clamp(curRoom + next, 0, backgrounds.Count - 1);
        PlayerPrefs.SetInt("CurRoom", curRoom);
        needUIChange = true;
        
    }

    public void BottomBarUIChange()
    {
        for (int i = 0; i < roomCount; i++)
        {
            leftBotUI[i].SetActive(false);
            midBotUI[i].SetActive(false);
            rightBotUI[i].SetActive(false);
        }
        
        leftBotUI[curRoom].SetActive(true);
        midBotUI[curRoom].SetActive(true);
        rightBotUI[curRoom].SetActive(true);
    }
}
