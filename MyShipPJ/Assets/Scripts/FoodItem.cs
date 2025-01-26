using System.Collections;
using System.Collections.Generic;
using GameData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FoodItem : MonoBehaviour
{

    public Food food;

    string path = "Sprites/Items/Foods/";
    public Image img;
    public TextMeshProUGUI countTxt, FullnessTxt, favorTxt, descript; // 추가 - descript는 나중에 꾹 누르면 말풍선 팝업으로 뜨게

    public void SetUI(Food food)
    {
        gameObject.name = food.name;
        this.food = food;

        FullnessTxt.text = food.fullness.ToString();
        favorTxt.text = food.favor.ToString();
        descript.text = food.descript.ToString();
        img.sprite = Resources.Load<Sprite>(path + food.name);
        SetCount();
    }

    public void SetCount()
    {
        countTxt.text = "x" + food.count;
        gameObject.SetActive(food.count > 0);
    }

    public void ViewDescriptionAction()
    {

    }

    public void SelectFoodAction()
    {
        UIManager.instance.SelectFoodAction(food);
    }
}
