using System.Collections;
using System.Collections.Generic;
using GameData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // Room 2 식당
    public GameObject refrigeratorPanel;
    public GameObject refrigeratorContent;
    public GameObject foodItemPrefab;
    public GameObject foodShopBtn;

    public GameObject foodShopContent;
    public GameObject foodShopItemPrefab;

    // bottom bar - room 2 - mid - selectedFood
    public GameObject selectedFood;
    public TextMeshProUGUI selectedFoodTxt;

    public bool foodChanged = true;

    private void Awake()
    {
        // 싱글톤
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        FoodChange();
    }

    public void FoodChange()
    {
        LoadRefrigerator();
        LoadFoodShop();
    }

    void LoadRefrigerator()
    {
        List<Food> foods = DataManager.instance.LoadHavingFoods();

        // 내용 비우기
        foreach (Transform child in refrigeratorContent.transform)
        {
            if (child != foodShopBtn.transform)
                Destroy(child.gameObject);
        }

        // 아이템 추가
        for (int i = 0; i < foods.Count; i++)
        {
            GameObject food = Instantiate(foodItemPrefab, transform.position, Quaternion.identity);
            FoodItem foodItem = food.GetComponent<FoodItem>();

            food.name = name;
            foodItem.SetUI(foods[i], i);

            food.transform.SetParent(refrigeratorContent.transform, false);
            food.SetActive(true);
        }

        // 구매 버튼 뒤로
        foodShopBtn.transform.SetAsLastSibling();
    }

    void LoadFoodShop()
    {
        List<Food> foods = DataManager.instance.userData.foods;

        // 내용 비우기
        foreach (Transform child in foodShopContent.transform)
            Destroy(child.gameObject);

        // 아이템 추가
        for (int i = 0; i < foods.Count; i++)
        {
            GameObject food = Instantiate(foodShopItemPrefab, transform.position, Quaternion.identity);
            FoodShopItem foodShopItem = food.GetComponent<FoodShopItem>();

            food.name = name;
            foodShopItem.SetUI(foods[i]);

            food.transform.SetParent(foodShopContent.transform, false);
            food.SetActive(true);
        }
    }



    // FoodItems.SelectFoodAction()에서 사용.
    public void SelectFoodAction(Food food, int havingFoodIndex)
    {
        selectedFood.GetComponent<SelectedFood>().food = food;
        selectedFood.GetComponent<SelectedFood>().havingFoodIndex = havingFoodIndex;

        selectedFood.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Items/Foods/" + food.name);
        selectedFoodTxt.text = food.kr_name + " x" + food.count;
        PlayerPrefs.SetInt("SelectedFood", havingFoodIndex);
        refrigeratorPanel.SetActive(false);
    }
}
