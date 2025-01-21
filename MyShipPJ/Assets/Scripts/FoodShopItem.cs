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

    private void Update()
    {
        if (food.cost > DataManager.instance.userData.coin)
            buyBtn.interactable = false;
    }

    public void SetUI(Food food)
    {
        this.food = food;

        this.nameTxt.text = food.kr_name;
        if (food.count > 0)
            this.countTxt.text = "x" + food.count;
        this.costTxt.text = food.cost.ToString();
        this.FullnessTxt.text = food.fullness.ToString();
        this.favorTxt.text = food.favor.ToString();
        this.descript.text = food.descript.ToString();

        if (food.cost > DataManager.instance.userData.coin)
            buyBtn.interactable = false;

        print("path" + path);
        img.sprite = Resources.Load<Sprite>(path + food.name);
    }

    public void BuyAction()
    {
        AudioManager.instance.PlaySFX(SFXClip.BUY);
        DataManager.instance.userData.coin -= food.cost;
        food.count += 1;
        countTxt.text = "x" + food.count.ToString();
        GameManager.instance.SetCoinUI();
    }

}
