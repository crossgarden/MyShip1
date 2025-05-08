using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToTheMoon_Coin : MonoBehaviour
{

    void Update()
    {
        if (ToTheMoon_Manager.instance != null)
        {
            // Manager에서 배경 이동 속도 가져오기
            float moveAmount = ToTheMoon_Manager.instance.BackgroundMoveSpeed * Time.deltaTime;
            transform.position += Vector3.down * moveAmount;
        }

        CheckFallOffScreen();
    }

    void CheckFallOffScreen()
    {
        // 화면 아래로 떨어졌는지 확인
        float screenBottom = Camera.main.transform.position.y - Camera.main.orthographicSize;
        if (transform.position.y < screenBottom - 1f) // 1f 여유 공간
        {
            Destroy(gameObject);
        }
    }

}
