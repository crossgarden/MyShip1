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
    public Button buyBtn;
    
    public void SetUI(Food food)
    {
        this.food = food;

        img.sprite = Resources.Load<Sprite>(path + food.name);
        nameTxt.text = food.kr_name;
        costTxt.text = food.cost.ToString();
        FullnessTxt.text = food.fullness.ToString();
        favorTxt.text = food.favor.ToString();
        descript.text = food.descript.ToString();
        SetCount();
        SetVisibleBuy();
        gameObject.SetActive(true);
    }

    public void SetCount()
    {
        if (food.count > 0)
            countTxt.text = "x" + food.count;
        else
            countTxt.text = "";
    }

    public void SetVisibleBuy()
    {
        buyBtn.interactable = food.cost <= DataManager.instance.userData.coin;
    }

    public void BuyAction()
    {
        AudioManager.instance.PlaySFX(AudioManager.SFXClip.BUY);

        DataManager.instance.userData.coin -= food.cost;
        food.count += 1;
        SetCount();
        UIManager.instance.SetCoinUI();

        UIManager.instance.FoodChange(food);
        UIManager.instance.SelectFoodAction(food);
    }

}
