using System.Collections;
using System.Collections.Generic;
using GameData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FoodItem : MonoBehaviour
{

    public Food food;
    public int havingFoodIndex;

    string path = "Sprites/Items/Foods/";
    public Image img;
    public TextMeshProUGUI countTxt, FullnessTxt, favorTxt, descript; // 추가 - descript는 나중에 꾹 누르면 말풍선 팝업으로 뜨게

    // UIManager.refrigeratorAction()에서 냉장고 스크롤뷰 컨텐츠 아이템 동적 추가시 사용.
    public void SetUI(Food food, int havingFoodIndex)
    {
        this.food = food;
        this.havingFoodIndex = havingFoodIndex;
        
        this.countTxt.text = "x" + food.count;
        this.FullnessTxt.text = food.fullness.ToString();
        this.favorTxt.text = food.favor.ToString();
        this.descript.text = food.descript.ToString();

        img.sprite = Resources.Load<Sprite>(path + food.name);
    }

    public void ViewDescriptionAction()
    {

    }

    public void SelectFoodAction()
    {
        UIManager.instance.SelectFoodAction(food, havingFoodIndex);
    }
}
