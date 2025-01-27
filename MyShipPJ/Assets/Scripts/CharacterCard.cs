using System.Collections;
using System.Collections.Generic;
using GameData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCard : MonoBehaviour
{
    public Character character;
    string path = "Sprites/Characters/";
    public Image image, selectedChecked;
    public TextMeshProUGUI nameTxt, dateTxt, introTxt, levelTxt;
    public Transform scriptContainer;
    public Slider fullnessSlider, energySlider, favorSlider;
    public Image fullnessFill, energyFill;
    public GameObject spotlight;

    public void SetUI(Character character, int index)
    {
        this.character = character;
        image.sprite = Resources.Load<Sprite>(path + character.name + "_all");
        // if (PlayerPrefs.GetInt("CurCharacter", 0) == index)
        //     this.selectedChecked.sprite = Resources.Load<Sprite>(path + "checked"); // 아직 이미지 안만듦
        // else
        //     this.selectedChecked.sprite = Resources.Load<Sprite>(path + "unchecked"); // 아직 이미지 안만듦
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
    }

    // public void Click()
    // {
    //     print("click");
    // }

    public void ChangeSpotlight()
    {
        spotlight.SetActive(spotlight.activeSelf == false);
    }

    public void UpdateEnergy(){
        energySlider.value = character.energy;
        UIManager.instance.SetSliderFillColor(energySlider, energyFill);
    }
}
