using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterInfoScroll : ScrollRect
{

    bool forParent;
    CharacterListScroll parentScrollScript;
    ScrollRect parentScrollRect;


    protected override void Start()
    {
        parentScrollScript = GameObject.FindWithTag("ParentScroll").GetComponent<CharacterListScroll>();
        parentScrollRect = GameObject.FindWithTag("ParentScroll").GetComponent<ScrollRect>();
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        forParent = Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y); // 수평이동 > 부모 스크롤

        if (forParent)
        {
            parentScrollScript.OnBeginDrag(eventData);
            parentScrollRect.OnBeginDrag(eventData);
        }
        else
            base.OnBeginDrag(eventData);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (forParent)
        {
            parentScrollScript.OnDrag(eventData);
            parentScrollRect.OnDrag(eventData);
        }
        else
            base.OnDrag(eventData);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (forParent)
        {
            parentScrollScript.OnEndDrag(eventData);
            parentScrollRect.OnEndDrag(eventData);
        }
        else
            base.OnEndDrag(eventData);
    }
}
