
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
    public List<Food> foods;

    public Vector3 goalPosMin;
    public Vector3 goalPosMax;

    private void Start()
    {
        foods = DataManager.instance.userData.foods;
        InitFood();

        goalPosMin = Camera.main.ViewportToWorldPoint(new Vector3(0.29f, 0.35f, 0));
        goalPosMax = Camera.main.ViewportToWorldPoint(new Vector3(0.70f, 0.65f, 0));
    }

    // 음식 초기화
    public void InitFood()
    {
        initPos = transform.position;

        bool hasFood = false;

        foreach (Food food in foods)
            if (food.count > 0)
            {
                hasFood = true;
                break;
            }

        if (hasFood)
        {
            food = foods[PlayerPrefs.GetInt("SelectedFood")];
            gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Items/Foods/" + food.name);
            foodTxt.text = food.kr_name + " x" + food.count;
        }
        else
        {
            food = null;
            gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Items/Foods/null");
            foodTxt.text = "";
        }
    }

    public void SetUI(Food food)
    {
        gameObject.name = food.name;
        this.food = food;
        gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Items/Foods/" + food.name);
        foodTxt.text = food.kr_name + " x" + food.count;
    }

    // selectFood 양쪽 화살표 버튼 Action
    public void ChangeFoodAction(int direction)
    {
        // 갖고 있는 음식 종류 개수
        int foodCount = 0;

        foreach (Food food in foods)
            if (food.count > 0)
                foodCount += 1;

        // 보유 음식이 0 이면 텅 표시
        if (foodCount == 0)
        {
            gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Items/Foods/null");
            food = null;
            foodTxt.text = null;
            return;
        }

        // 보유 음식이 둘 이상일 시 실행
        int targetFoodIndex = food.index;
        do
        {
            targetFoodIndex += direction;

            if (targetFoodIndex < 0)
                targetFoodIndex = foods.Count - 1;

            if (targetFoodIndex >= foods.Count)
                targetFoodIndex = 0;

        } while (foods[targetFoodIndex].count < 1);

        SetUI(foods[targetFoodIndex]);
        PlayerPrefs.SetInt("SelectedFood", food.index);

    }

    // [1-2] 음식 주기
    // 1. 음식 드래그
    public void BeginDragAction()
    {
        if (food is null)
            return;

        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;
        gameObject.transform.position = pos;
    }

    // 2. 음식 주기
    public void EndDragAction()
    {
        if (food is null)
            return;

        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (GameManager.instance.curCharacter.fullness < 100 && pos.x > goalPosMin.x && pos.x < goalPosMax.x && pos.y > goalPosMin.y && pos.y < goalPosMax.y)  // 수정 - 좌표 이렇게 하드코딩 해도 되나?
        {
            // 사운드
            AudioManager.instance.PlaySFX(AudioManager.SFXClip.EATTING);

            food.count -= 1;

            // 먹인 후 - 캐릭터
            UIManager.instance.CharacterFavorUp(food.favor);
            UIManager.instance.CharacterFullnessUp(food.fullness);


            //애니메이션 재생
            TriggerTouchAnimation();

            // 먹인 후 - 음식
            gameObject.SetActive(false);
            UIManager.instance.FoodChange(food);

            if (food.count == 0) // 해당 food 다 먹은 경우
                ChangeFoodAction(1);
            else
                foodTxt.text = food.kr_name + " x" + food.count;

            gameObject.transform.position = initPos;
            gameObject.SetActive(true);

        }
        else
        {
            if (GameManager.instance.curCharacter.fullness >= 100)
            {
                print("배부러링");
            }

            AudioManager.instance.PlaySFX(AudioManager.SFXClip.FAIL);
            gameObject.transform.position = initPos;
        }
    }

    //애니메이션 메서드 추가
    void TriggerTouchAnimation()
    {
        // CharacterContainer 찾기
        GameObject characterContainer = GameObject.Find("CharacterContainer");
        if (characterContainer != null && characterContainer.transform.childCount > 0)
        {
            Transform character = characterContainer.transform.GetChild(0);
            Animator animator = character.GetComponent<Animator>();

            if (animator != null)
            {
                animator.SetTrigger("Touch");
            }
        }
    }

}
