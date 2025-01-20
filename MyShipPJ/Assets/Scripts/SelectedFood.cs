
using System;
using System.Collections.Generic;
using GameData;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SelectedFood : MonoBehaviour
{
    public Vector3 initPos;
    public TextMeshProUGUI foodTxt;

    public Food food;
    public List<Food> foods;
    public List<Food> havingFoods;
    public int havingFoodIndex;

    private void Start()
    {
        initPos = transform.position;
        havingFoods = DataManager.instance.LoadHavingFoods();

        havingFoodIndex = PlayerPrefs.GetInt("SelectedFood", -1);
        if (havingFoodIndex != -1 && havingFoods.Count > 0)
        {
            food = havingFoods[havingFoodIndex];
            gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Items/Foods/" + food.name);
            foodTxt.text = food.kr_name + " x" + food.count;
        }
        else
        {  // 테스트 안해봄
            gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Items/Foods/null");
            foodTxt.text = null;
        }
    }

    // selectFood 양쪽 화살표 버튼 Action
    public void ChangeFoodAction(int direction)
    {
        if (havingFoods.Count == 0)
        {
            gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Items/Foods/null");
            foodTxt.text = null;
            return;
        }

        havingFoodIndex += direction;

        if(havingFoodIndex == -1)
            havingFoodIndex = havingFoods.Count-1;
        if(havingFoodIndex >= havingFoods.Count)
            havingFoodIndex = 0;

        food = havingFoods[havingFoodIndex];

        gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Items/Foods/" + food.name);
        foodTxt.text = food.kr_name + " x" + food.count;

        PlayerPrefs.SetInt("SelectedFood", havingFoodIndex);
    }

    // [1-2] 음식 주기
    // 1. 음식 드래그
    public void BeginDragAction()
    {
        if (havingFoods.Count == 0)
            return;
        gameObject.transform.position = Input.mousePosition;
    }

    // 2. 음식 주기
    public void EndDragAction()
    {
        if (havingFoods.Count == 0)
            return;

        Vector3 pos = Input.mousePosition;
        if (pos.x > 300 && pos.x < 850 && pos.y > 550 && pos.y < 1150)  // 수정 - 좌표 이렇게 하드코딩 해도 되나?
        {
            print("골인");
            foreach (Food f in DataManager.instance.userData.foods)
            {
                if (f == food)
                {
                    food.count -= 1;
                    break;
                }
            }

            DataManager.instance.saveData();
            havingFoods = DataManager.instance.LoadHavingFoods();

            GameManager.instance.FoodChange();
            
            // 먹인 후
            gameObject.SetActive(false);

            if (food.count == 0) // 해당 food 다 먹은 경우
                ChangeFoodAction(1);
            else
                foodTxt.text = food.kr_name + " x" + food.count;

            gameObject.transform.position = initPos;
            gameObject.SetActive(true);

            /** 
                현재 캐릭터의 fullness, favor + // 나중에 추가. 일단 음식 관련만.
            */
        }
        else
        {
            gameObject.transform.position = initPos;
        }
    }

}
