using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FoodItemScrollTossRefrigerator : ScrollRect
{
    bool forParent;
    FoodItem foodItem;
    ScrollRect refrigeratorScrollRect;

    protected override void Start()
    {
        refrigeratorScrollRect = GameObject.FindWithTag("RefrigeratorScrollRect").GetComponent<ScrollRect>();
        foodItem = gameObject.transform.GetComponent<FoodItem>();
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        forParent = Mathf.Abs(eventData.delta.y) > Mathf.Abs(eventData.delta.x); // 수직이동 -> 냉장고 스크롤

        if (forParent)
            refrigeratorScrollRect.OnBeginDrag(eventData);
        else
            foodItem.SelectFoodAction();
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (forParent)
            refrigeratorScrollRect.OnDrag(eventData);
        else
            foodItem.SelectFoodAction();
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (forParent)
            refrigeratorScrollRect.OnEndDrag(eventData);
        else
            foodItem.SelectFoodAction();
    }

}
