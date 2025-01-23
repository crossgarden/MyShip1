
using System;
using System.Collections.Generic;
using GameData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectedFood : MonoBehaviour
{
    public Vector3 initPos;
    public TextMeshProUGUI foodTxt;

    public Food food;
    public List<Food> havingFoods;
    public int havingFoodIndex;

    public Vector3 goalPosMin;
    public Vector3 goalPosMax;

    private void Start()
    {
        havingFoods = DataManager.instance.havingFoods;
        InitFood();

        goalPosMin = Camera.main.ViewportToWorldPoint(new Vector3(0.29f, 0.35f, 0));
        goalPosMax = Camera.main.ViewportToWorldPoint(new Vector3(0.70f, 0.65f, 0));

    }

    // 음식 초기화
    public void InitFood()
    {
        initPos = transform.position;
        havingFoodIndex = PlayerPrefs.GetInt("SelectedFood", 0);

        if (havingFoods.Count > 0)
        {
            food = havingFoods[havingFoodIndex];
            gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Items/Foods/" + food.name);
            foodTxt.text = food.kr_name + " x" + food.count;
        }
        else
        {
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

        if (havingFoodIndex == -1)
            havingFoodIndex = havingFoods.Count - 1;
        if (havingFoodIndex >= havingFoods.Count)
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

        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;
        gameObject.transform.position = pos;
    }

    // 2. 음식 주기
    public void EndDragAction()
    {
        if (havingFoods.Count == 0)
            return;
        
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        print(Camera.main.WorldToViewportPoint(pos));
        if (pos.x > goalPosMin.x && pos.x < goalPosMax.x && pos.y > goalPosMin.y && pos.y < goalPosMax.y)  // 수정 - 좌표 이렇게 하드코딩 해도 되나?
        {
            // 사운드
            AudioManager.instance.PlaySFX(SFXClip.EATTING);

            foreach (Food f in DataManager.instance.userData.foods)
            {
                if (f == food)
                {
                    food.count -= 1;
                    break;
                }
            }

            DataManager.instance.saveData();
            DataManager.instance.LoadHavingFoods();

            UIManager.instance.FoodChange();

            // 먹인 후
            gameObject.SetActive(false);

            if (food.count == 0) // 해당 food 다 먹은 경우
                ChangeFoodAction(1);
            else
                foodTxt.text = food.kr_name + " x" + food.count;

            gameObject.transform.position = initPos;
            gameObject.SetActive(true);

            // 데이터 갱신
            UIManager.instance.CharacterFavorUp(food.favor);
            UIManager.instance.CharacterFullnessUp(food.fullness);
            UIManager.instance.UpdateCurCharacter();

        }
        else
        {
            AudioManager.instance.PlaySFX(SFXClip.FAIL);
            gameObject.transform.position = initPos;
        }
    }

}
