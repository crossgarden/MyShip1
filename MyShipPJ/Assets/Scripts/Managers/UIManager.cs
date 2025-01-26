using System;
using System.Collections;
using System.Collections.Generic;
using GameData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public static UIManager instance;

    private void Awake()
    {
        instance = this;
    }

    [Header("Character")]
    public Character curCharacter;
    List<Character> characters;
    public Transform characterContainer;

    /**  Top bar */
    [Header("top bar")]
    public TextMeshProUGUI coinTxt;
    public Slider fullenssSlider;   // 포만도 값 설정
    public Image fullnessFill;   // 포만도 게이지 색 설정
    public Slider energySlider;
    public Image energyFill;
    public Slider favorSlider;
    public TextMeshProUGUI characterLevelTxt;

    /** 캐릭터 리스트 */
    [Header("Character list")]
    public GameObject CharacterListPanel;
    public Transform CharacterListScrollView;
    public GameObject characterListContent;
    public GameObject CharacterCardPrefab;

    /** 0. 대기실 */

    /** 1. 개인실 */
    [Header("private room")]
    public Sprite[] lightImgs;   // 0-Off, 1-on
    public Image lightImg;
    public GameObject lightOffPanel;

    /** 2. 식당 */
    [Header("refrigerator")]
    public GameObject refrigeratorPanel;
    public GameObject refrigeratorContent;
    public GameObject foodItemPrefab;
    public GameObject foodShopBtn;

    [Header("food shop")]
    public GameObject foodShopPanel;
    public GameObject foodShopContent;
    public GameObject foodShopItemPrefab;

    [Header("selected food")]
    public GameObject selectedFood;
    public TextMeshProUGUI selectedFoodTxt;

    private void Start()
    {
        curCharacter = GameManager.instance.curCharacter;
        characters = DataManager.instance.characterSotred;

        SetFavorUI();
        SetFullnessUI();
        SetEnergyUI();
        SetCoinUI();

        LoadCharacterList();
        FoodChange();

        if (PlayerPrefs.GetInt("CurRoom", 0) == (int)RoomNum.PRIVATE)
        {
            lightImg.sprite = lightImgs[PlayerPrefs.GetInt("LightOn", 0)];
            lightOffPanel.SetActive(PlayerPrefs.GetInt("LightOn", 0) == 0);
        }
    }

    /** [1-4] 공용 */
    /** 1. 팝업 열기 */
    public void OpenPopUP(GameObject popupPanel)
    {
        AudioManager.instance.PlaySFX(SFXClip.CLICK);
        popupPanel.SetActive(true);
    }

    /** 2. 팝업 닫기 */
    public void ClosePopUp(GameObject popupPanel)
    {
        AudioManager.instance.PlaySFX(SFXClip.CLICK);
        popupPanel.SetActive(false);
    }

    /** 3. Slider fill 색깔 */
    public void SetSliderFillColor(Slider slider, Image fill)
    {
        if (slider.value < 30)
            fill.color = Color.red;
        else if (slider.value < 65)
            fill.color = Color.yellow;
        else if (slider.value < 100)
            fill.color = Color.green;
        else
            fill.color = new Color(64 / 255f, 149 / 255f, 255 / 255f);
    }

    /** 4. 캐릭터 체인지 */
    public void CharacterChangeUI()
    {
        curCharacter = GameManager.instance.curCharacter;

        // top bar 변경
        SetFavorUI();
        SetFullnessUI();
        SetEnergyUI();

        // 캐릭터 변경
        characterContainer.GetComponent<MainChaContainer>().ChangeCharacter();

        // 캐릭터 리스트 팝업 닫기
        CharacterListPanel.SetActive(false);
    }



    /*************** [2-6] top bar 세팅 **********************/
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
        fullenssSlider.value = curCharacter.fullness;
        SetSliderFillColor(fullenssSlider, fullnessFill);
    }

    /** 5. 포만도 업 */
    public void CharacterFullnessUp(int fullness)
    {
        curCharacter.fullness = Mathf.Min(curCharacter.fullness + fullness, 100);
        DataManager.instance.saveData();
        SetFullnessUI();
    }

    /** 6. 에너지 UI 업데이트 */
    public void SetEnergyUI()
    {
        energySlider.value = curCharacter.energy;
        SetSliderFillColor(energySlider, energyFill);
    }

    /** 7. 에너지 업 */
    public void CharacterEnergyUp(int energy)
    {
        curCharacter.energy = Mathf.Min(curCharacter.energy + energy, 100);
        DataManager.instance.saveData();
        SetEnergyUI();
    }

    /** 8. coin 세팅 */
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



    /*************** [1-4] 캐릭터 리스트 UI 세팅 *************************/
    /** 1. 캐릭터 리스트 UI 로드 */
    public void LoadCharacterList()
    {

        // 캐릭터 카드 추가
        for (int i = 0; i < characters.Count; i++)
        {
            GameObject characterCard = Instantiate(CharacterCardPrefab, transform.position, Quaternion.identity);
            CharacterCard cardScript = characterCard.GetComponent<CharacterCard>();
            characterCard.name = characters[i].name;
            cardScript.SetUI(characters[i], i);

            characterCard.transform.SetParent(characterListContent.transform, false);
            characterCard.SetActive(true);
        }
    }

    /** 2. 캐릭터 리스트 버튼 액션 */
    public void CharacterListAction()
    {
        AudioManager.instance.PlaySFX(SFXClip.CLICK);

        CharacterListScroll scrollScript = CharacterListScrollView.GetComponent<CharacterListScroll>();
        CharacterListPanel.SetActive(true);
        scrollScript.targetPos = PlayerPrefs.GetInt("CurCharacter", 0);
        scrollScript.isDrag = false;

        scrollScript.contentTransform.GetChild(scrollScript.targetPos).GetChild(1).GetComponent<Scrollbar>().value = 1;
    }

    /** 3. 캐릭터 리스트 갱신 */
    public void UpdateCharacterList()
    {  // 아이템을 재생성 하지 않고 내용만 업데이트
        for (int i = 0; i < characters.Count; i++)
        {
            Transform card = characterListContent.transform.GetChild(i);
            CharacterCard cardScript = card.GetComponent<CharacterCard>();
            cardScript.SetUI(characters[i], i);
        }
    }

    /** 4. 캐릭터 리스트 중 현재 캐릭터만 갱신 */
    public void UpdateCurCharacter()
    {
        int index = PlayerPrefs.GetInt("CurCharacter", 0);
        Transform card = characterListContent.transform.GetChild(index);
        CharacterCard cardScript = card.GetComponent<CharacterCard>();
        cardScript.SetUI(characters[index], index);
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
        int prelight = PlayerPrefs.GetInt("LightOn", 0);
        PlayerPrefs.SetInt("LightOn", prelight ^ 1);
        lightImg.sprite = lightImgs[prelight ^ 1];
        lightOffPanel.SetActive(prelight == 1);
    }

    public void RoomDecoAction()
    {
        print("RoomDecoAction");
    }




    /******************* [1-8] Room 2 - 식당 **************/
    // 1. 냉장고 버튼 액션
    public void RefrigeratorAction()
    {
        AudioManager.instance.PlaySFX(SFXClip.CLICK);
        refrigeratorPanel.SetActive(true);
    }

    /** 2. 냉장고 데이터 로드 */
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

            food.name = foods[i].name;
            foodItem.SetUI(foods[i], i);

            food.transform.SetParent(refrigeratorContent.transform, false);
            food.SetActive(true);
        }

        // 구매 버튼 뒤로
        foodShopBtn.transform.SetAsLastSibling();
    }


    /** 3. food shop 버튼 액션 */
    public void FoodShopAction()
    {
        AudioManager.instance.PlaySFX(SFXClip.CLICK);
        refrigeratorPanel.SetActive(false);
        foodShopPanel.SetActive(true);
    }

    /** 4. food shop 데이터 로드 */
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

    /** 5. food shop (+냉장고) 닫기 버튼 액션 */
    public void CloseFoodShopAction()
    {
        AudioManager.instance.PlaySFX(SFXClip.CLICK);
        foodShopPanel.SetActive(false);
        FoodChange();
    }

    /** 6. 냉장고로 돌아가기 버튼 액션 */
    public void BackToRefrigeratorAction()
    {
        CloseFoodShopAction();
        RefrigeratorAction();
    }

    /** 7. food 내용 변경시 데이터 갱신 */
    public void FoodChange(int fullness = 0, int favor = 0)
    {
        List<Food> foods = DataManager.instance.LoadHavingFoods();
        CharacterFavorUp(favor);
        CharacterFullnessUp(fullness);

        selectedFood.GetComponent<SelectedFood>().ChangeFoodAction(0);

        DataManager.instance.saveData();
        DataManager.instance.LoadHavingFoods();
        LoadRefrigerator();
        LoadFoodShop();

        UpdateCurCharacter();
    }

    /** 8. food 선택 -> SelectedFood에 세팅 */
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
