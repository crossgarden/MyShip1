using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterListScroll : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Scrollbar scrollbar;

    public int characterCount;
    float distance;
    float[] pos;
    public int curPos, targetPos;
    public bool isDrag;

    public Transform contentTransform;

    void Start()
    {
        characterCount = DataManager.instance.characterSotred.Count;
        pos = new float[characterCount];

        // 거리에 따라 0~1인 pos 대입
        distance = 1f / (characterCount - 1);
        for (int i = 0; i < characterCount; i++)
            pos[i] = distance * i;
    }

    public int getPos()
    {
        // 절반 거리를 기준으로 가까운 위치 반환
        for (int i = 0; i < characterCount; i++)
            if (scrollbar.value > pos[i] - distance * 0.5 && scrollbar.value < pos[i] + distance * 0.5)
                return i;
        return -1;
    }

    public void OnBeginDrag(PointerEventData eventData) => curPos = getPos();

    public void OnDrag(PointerEventData eventData) => isDrag = true;

    public void OnEndDrag(PointerEventData eventData)
    {

        // 절반 거리를 넘지 않아도 마우스를 빠르게 이동하면
        if (eventData.delta.x < -15)
            targetPos = Mathf.Min(curPos + 1, characterCount - 1);
        else if (eventData.delta.x > 15)
            targetPos = Mathf.Max(curPos - 1, 0);
        else
            targetPos = getPos();

        isDrag = false;

        // 포커스 카드 내용 위로 올리기 
        if (curPos != targetPos)
            contentTransform.GetChild(targetPos).GetChild(1).GetComponent<Scrollbar>().value = 1;
    }

    void Update()
    {
        if (!isDrag)
            scrollbar.value = Mathf.Lerp(scrollbar.value, pos[targetPos], 0.1f);
    }

    public void CharacterChange()
    {
        // Vector3 minPos = new Vector3(60f, 75f, 0);
        // Vector3 maxPos = new Vector3(126f, 120f, 0);
        // Vector3 pos = Camera.main.WorldToViewportPoint(Input.mousePosition);

        // if (pos.x > minPos.x || pos.x < maxPos.x || pos.y > minPos.y || pos.y < maxPos.y){}   // 이거 수정해야됨 content를 위로 올릴수 있잖아아

        AudioManager.instance.PlaySFX(AudioManager.SFXClip.CLICK);
        UIManager.instance.CharacterChangeUI(targetPos);
    }
}
