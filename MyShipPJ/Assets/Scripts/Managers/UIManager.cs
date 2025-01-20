using System.Collections;
using System.Collections.Generic;
using GameData;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // Room 2 식당
    public GameObject refrigeratorPanel;
    public GameObject foodShopPanel;

    private void Start()
    {

    }

    public void OpenPopUP(GameObject popupPanel)
    {
        popupPanel.SetActive(true);
    }

    public void ClosePopUp(GameObject popupPanel)
    {
        popupPanel.SetActive(false);
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

    // [1-2] Room 2 - 식당 
    // 1. 냉장고 열기 액션
    public void RefrigeratorAction()
    {
        refrigeratorPanel.SetActive(true);
    }

    // 2. food shop 열기 액션
    public void FoodShopAction()
    {
        foodShopPanel.SetActive(true);
    }

}
