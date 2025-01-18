using System.Collections;
using System.Collections.Generic;
using GameData;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public UserData userData;
    public SystemData systemData;

    public GameObject RefrigeratorPanel;
    public GameObject refrigeratorContent;
    public GameObject foodItemPrefab;
    
    private void Start() {
        userData = DataManager.instance.userData;
        systemData = DataManager.instance.systemData;
    }

    // Room 0 - 대기실
    public void ExitAction()
    {
        print("ExitAction");
    }

    public void PlayAction()
    {
        print("PlayAction");
    }

    // Room 1 - 개인실 
    public void ClothesAction()
    {
        print("ClothesAction");
    }

    public void LightAction()
    {
        print("LightAction");
    }

    public void RoomDecoAction()
    {
        print("RoomDecoAction");
    }

    // Room 2 - 식당 
    public void refrigeratorAction()
    {
        for(int i = 0; i < userData.food.Length ; i++){

            GameObject food = Instantiate(foodItemPrefab, transform.position, Quaternion.identity);
            food.name = userData.food[i].name;
            FoodItem foodItem = food.GetComponent<FoodItem>();
            foodItem.SetUI(food.name, userData.food[i].count.ToString(), "","","");

            food.transform.SetParent(refrigeratorContent.transform, false);
            food.SetActive(true);
        }
        RefrigeratorPanel.SetActive(true);
    }

    public void FeedAction()
    {
        print("FeedAction");
    }
}
