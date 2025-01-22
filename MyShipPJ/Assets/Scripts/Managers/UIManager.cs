using System.Collections;
using System.Collections.Generic;
using GameData;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject CharacterListPanel;
    public GameObject characterListContent;
    public GameObject CharacterCardPrefab;
    
    // Room 2 식당
    public GameObject refrigeratorPanel;
    public GameObject foodShopPanel;

    public GameObject selectedFood;

    private void Start()
    {
    }

    public void OpenPopUP(GameObject popupPanel)
    {
        AudioManager.instance.PlaySFX(SFXClip.CLICK);
        popupPanel.SetActive(true);
    }

    public void ClosePopUp(GameObject popupPanel)
    {
        AudioManager.instance.PlaySFX(SFXClip.CLICK);
        popupPanel.SetActive(false);
    }

    /** 캐릭터 리스트 UI 세팅 */
    public void LoadCharacterList()
    {
        List<Character> characters = DataManager.instance.userData.characters;

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

    public void CharacterListAction(){
        LoadCharacterList();
        CharacterListPanel.SetActive(true);
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
        AudioManager.instance.PlaySFX(SFXClip.CLICK);
        refrigeratorPanel.SetActive(true);
    }

    // 2. food shop 열기 액션
    public void FoodShopAction()
    {
        AudioManager.instance.PlaySFX(SFXClip.CLICK);
        refrigeratorPanel.SetActive(false);
        foodShopPanel.SetActive(true);
    }

    public void CloseFoodShopAction()
    {
        AudioManager.instance.PlaySFX(SFXClip.CLICK);
        foodShopPanel.SetActive(false);
        GameManager.instance.FoodChange();
        selectedFood.GetComponent<SelectedFood>().foodTxt.text = selectedFood.GetComponent<SelectedFood>().food.kr_name + " x" + selectedFood.GetComponent<SelectedFood>().food.count;
    }

    public void BackToRefrigeratorAction()
    {
        CloseFoodShopAction();
        RefrigeratorAction();
    }


}
