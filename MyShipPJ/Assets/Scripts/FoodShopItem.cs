using GameData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FoodShopItem : MonoBehaviour
{

    public Food food;

    string path = "Sprites/Items/Foods/";
    public Image img;
    public TextMeshProUGUI nameTxt, countTxt, FullnessTxt, favorTxt, costTxt, descript;

    public void SetUI(Food food)
    {
        this.food = food;

        this.nameTxt.text = food.kr_name;
        if(food.count > 0)
            this.countTxt.text = "x" + food.count;
        this.costTxt.text = food.cost.ToString();
        this.FullnessTxt.text = food.fullness.ToString();
        this.favorTxt.text = food.favor.ToString();
        this.descript.text = food.descript.ToString();

        print("path" + path);
        img.sprite = Resources.Load<Sprite>(path + food.name);
    }

    public void BuyAction()
    {

    }

}
