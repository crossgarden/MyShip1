using System;
using System.Collections;
using System.Collections.Generic;
using GameData;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterCard : MonoBehaviour
{
    public Character character;
    string path = "Sprites/Items/Characters/";
    public Image image, selectedChecked;
    public TextMeshProUGUI nameTxt, dateTxt, introTxt, levelTxt;
    public Transform scriptContainer;
    public Slider fullnessSlider, energySlider, favorSlider;
    public Image fullnessFill, energyFill;
    public GameObject spotlight;

    public GameObject lockedPanel;
    public Button buyBtn;
    public TextMeshProUGUI howToGetTxt;


    public void SetUI(Character character)
    {
        this.character = character;
        image.sprite = Resources.Load<Sprite>(path + character.name);
        nameTxt.text = character.kr_name;
        introTxt.text = character.introduction;
        levelTxt.text = character.level.ToString();

        favorSlider.maxValue = DataManager.instance.favorMax[character.level];
        favorSlider.value = character.favor;
        fullnessSlider.value = character.fullness;
        energySlider.value = character.energy;
        UIManager.instance.SetSliderFillColor(fullnessSlider, fullnessFill);
        UIManager.instance.SetSliderFillColor(energySlider, energyFill);

        spotlight.SetActive(character == GameManager.instance.curCharacter);

        if (character.locked == 0)
        {
            this.dateTxt.text = "획득일: " + character.unlockDate;
        }
        else
        {
            dateTxt.text = "획득일: " + "-";
        }

        for (int i = 0; i < character.script.Length; i++)
        {
            scriptContainer.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Lv" + (i + 1) + " ▶";
            if (character.level > i)
                scriptContainer.GetChild(i).GetChild(1).GetComponent<TextMeshProUGUI>().text = character.script[i];
            else
            {
                scriptContainer.GetChild(i).GetChild(1).GetComponent<TextMeshProUGUI>().text = "레벨 " + (i + 1) + "달성 시 열람 가능.";
                scriptContainer.GetChild(i).GetChild(1).GetComponent<TextMeshProUGUI>().color = new Color(255 / 255f, 111 / 255f, 111 / 255f);
            }
        }

        if (character.locked == 1)
        {
            if (character.cost == 0)
            {
                howToGetTxt.text = character.howToGet;
                buyBtn.gameObject.SetActive(false);
                howToGetTxt.gameObject.SetActive(true);
            }
            else
            {
                bool canBuy = DataManager.instance.userData.coin >= character.cost;
                buyBtn.interactable = canBuy;
                buyBtn.image.color = canBuy ? Color.green : Color.grey;
                buyBtn.gameObject.SetActive(true);
                howToGetTxt.gameObject.SetActive(false);
            }

            lockedPanel.SetActive(true);
            gameObject.GetComponent<ScrollRect>().vertical = false;
        }
        else
        {
            lockedPanel.SetActive(false);
            gameObject.GetComponent<ScrollRect>().vertical = true;
        }

    }

    public void ChangeSpotlight()
    {
        spotlight.SetActive(spotlight.activeSelf == false);
    }

    public void UpdateEnergy()
    {
        energySlider.value = character.energy;
        UIManager.instance.SetSliderFillColor(energySlider, energyFill);
    }

    // 캐릭터 구매 버튼 액션
    public void BuyAction(){
        if(DataManager.instance.userData.coin >= character.cost){
            character.locked = 0;
            character.unlockDate = DateTime.Now.ToString("yyyy-MM-dd");
            DataManager.instance.saveData();
            dateTxt.text = character.unlockDate;
            lockedPanel.SetActive(false);
        }
    }

    //infoScroll 에서 클릭이벤트시 실행
    public void Click()
    {
        if (IsMouseOver(buyBtn.gameObject)){
            AudioManager.instance.PlaySFX(AudioManager.SFXClip.SUCCESS);
            buyBtn.onClick.Invoke();
        }
        
        else if (character.locked == 0 && IsMouseOver(image.gameObject))
        {
            if (character.locked == 1)
                return;
            AudioManager.instance.PlaySFX(AudioManager.SFXClip.CLICK);
            UIManager.instance.CharacterChangeUI(DataManager.instance.characterSotred.FindIndex(x => x.id == character.id));
        }
    }
    
    // 클릭한 시점의 마우스가 이미지 or 버튼 위인지 확인
    private bool IsMouseOver(GameObject obj)
    {
        // RectTransform 가져오기
        RectTransform buttonRect = obj.GetComponent<RectTransform>();
        // 마우스 포인터가 RectTransform 안에 있는지 확인
        return obj.activeSelf && RectTransformUtility.RectangleContainsScreenPoint(buttonRect, Camera.main.ScreenToWorldPoint(Input.mousePosition), null);
    }

}
