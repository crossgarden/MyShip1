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

    public int curRoom;

    void Start()
    {
        for (int i = 1; i < backgrounds.Count; i++)
        {
            backgrounds[i].transform.position = new Vector3(bgDistance * i, 0, 0);
        }
        curRoom = PlayerPrefs.GetInt("CurRoom", 0);
        bgController.position = new Vector3(curRoom * -bgDistance, 0, 0);
    }

    // 문제: 영원히 실행됨. 좀 낭비일지도.
    void Update()
    {
        Vector3 targetPos = new Vector3((curRoom) * -bgDistance, 0, 0);
        bgController.position = Vector3.Lerp(bgController.position, targetPos, Time.deltaTime * 10);

    }
    
    public void MoveRoomAction(int next)
                                // next - 1, pre - -1
    {                           
        curRoom = Mathf.Clamp(curRoom + next, 0, backgrounds.Count - 1);
    }
}
