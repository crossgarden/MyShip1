using System;
using System.Collections;
using System.Collections.Generic;
using GameData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Character curCharacter;
    public GameObject[] Characters;
    public float decayRateFullness = 108f; // 초당 감소율. 108: 3시간동안 100 > 0
    private DateTime offTime;

    /************ UI ***********/
    public TextMeshProUGUI coinTxt;
    public Slider fullenssSlider;   // 포만도 값 설정
    public Image fullnessImg;   // 포만도 게이지 색 설정
    public Slider favorSlider;
    public TextMeshProUGUI characterLevelTxt;

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
        CharacterInit();
        InitFullness();
        StartCoroutine(DecayFullness());

        FoodChange();
        SetFavorUI();
        SetFullnessUI();
        SetCoinUI();
    }

    /** [ - ] 시스템 */
    /** 1. 시간에 따른 메인 캐릭터의 포만도 감소 */
    IEnumerator DecayFullness()
    {
        while (curCharacter.fullness > 0)
        {
            yield return new WaitForSecondsRealtime(decayRateFullness);
            int index = PlayerPrefs.GetInt("CurCharacter", 0);
            curCharacter = DataManager.instance.userData.characters[index];

            curCharacter.fullness -= 1;
            DataManager.instance.saveData();
            SetFullnessUI();
        }
    }

    /** 2. 백그라운드에서 감소를 위한 종료시 현재시간 저장(scence 전환시에도 필요하나?) */
    void OnApplicationQuit()
    {
        // 종료 시 현재 시간을 저장
        PlayerPrefs.SetString("OffTime", DateTime.Now.ToString());
        PlayerPrefs.Save();
    }

    /** 3. 첫 실행 시 백그라운드 감소량 적용 */
    void InitFullness()
    {
        if (PlayerPrefs.HasKey("OffTime"))
        {
            offTime = DateTime.Parse(PlayerPrefs.GetString("OffTime"));

            TimeSpan timeDifference = DateTime.Now - offTime;
            float elapsedTime = (float)timeDifference.TotalSeconds;

            curCharacter.fullness = Mathf.Max(curCharacter.fullness - (int)(elapsedTime * decayRateFullness), 0); ;
            DataManager.instance.saveData();
        }
    }

    /*********** [1-5] character ***********************************************************/
    /** 1. 캐릭터 초기화 */
    public void CharacterInit()
    {
        int index = PlayerPrefs.GetInt("CurCharacter", 0);
        curCharacter = DataManager.instance.userData.characters[index];
    }

    /** 2. 호감도 UI 업데이트 */
    public void SetFavorUI()
    {
        characterLevelTxt.text = curCharacter.level.ToString();
        favorSlider.maxValue = DataManager.instance.userData.favor_max[curCharacter.level];
        favorSlider.value = curCharacter.favor;
    }

    /** 3. 호감도 업 */
    public void CharacterFavorUp(int favor)
    {
        if (curCharacter.level == 8)
            return;

        int fixFavor = curCharacter.favor + favor;
        if (fixFavor >= DataManager.instance.userData.favor_max[curCharacter.level])
        {
            curCharacter.favor = fixFavor - DataManager.instance.userData.favor_max[curCharacter.level];
            curCharacter.level += 1;

        }
        else
        {
            curCharacter.favor = fixFavor;
        }
        DataManager.instance.saveData();
        SetFavorUI();
    }

    /** 4. 포만도 UI 업데이트 */
    public void SetFullnessUI()
    {
        fullenssSlider.value = GameManager.instance.curCharacter.fullness;
        if (fullenssSlider.value < 30)
            fullnessImg.color = Color.red;
        else if (fullenssSlider.value < 65)
            fullnessImg.color = Color.yellow;
        else if (fullenssSlider.value < 100)
            fullnessImg.color = Color.green;
        else
            fullnessImg.color = new Color(64 / 255f, 149 / 255f, 255 / 255f);
    }

    /** 5. 포만도 업 */
    public void CharacterFullnessUp(int fullness)
    {
        curCharacter.fullness = Mathf.Min(curCharacter.fullness + fullness, 100);
        DataManager.instance.saveData();
        SetFullnessUI();
    }

    /** 6. coin 세팅 */
    public void SetCoinUI()
    {
        int coin = DataManager.instance.userData.coin;
        string txt = coin.ToString();
        if (coin >= 1000)
        {
            txt = coin / 1000 + "K";
        }
        coinTxt.text = txt;
    }


    /*********** [1-4] food ***********************************************************/
    /** 1. food 내용 변경시 냉장고, 상점 데이터 리로드 */
    public void FoodChange()
    {
        DataManager.instance.saveData();
        DataManager.instance.LoadHavingFoods();
        LoadRefrigerator();
        LoadFoodShop();
    }

    /** 2. 냉장고 데이터 로드 */
    void LoadRefrigerator()
    {
        List<Food> foods = DataManager.instance.havingFoods;

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

            food.name = foods[i].name;
            foodItem.SetUI(foods[i], i);

            food.transform.SetParent(refrigeratorContent.transform, false);
            food.SetActive(true);
        }

        // 구매 버튼 뒤로
        foodShopBtn.transform.SetAsLastSibling();
    }

    /** 3. 상점 데이터 로드 */
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

            food.name = foods[i].name;
            foodShopItem.SetUI(foods[i]);

            food.transform.SetParent(foodShopContent.transform, false);
            food.SetActive(true);
        }
    }

    /** 4. food 선택 -> SelectedFood에 세팅 */
    /** FoodItems.SelectFoodAction()에서 사용. */
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

