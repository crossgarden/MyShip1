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
    public Slider fullnessSlider, energySlider;
    public GameObject scriptContainer, scriptPanelPrefab;

    void Start()
    {

    }

    public void SetUI(Character character, int index)
    {
        this.image.sprite = Resources.Load<Sprite>(path + character.name + "_all");
        // if (PlayerPrefs.GetInt("CurCharacter", 0) == index)
        //     this.selectedChecked.sprite = Resources.Load<Sprite>(path + "checked"); // 아직 이미지 안만듦
        // else
        //     this.selectedChecked.sprite = Resources.Load<Sprite>(path + "unchecked"); // 아직 이미지 안만듦
        this.nameTxt.text = character.kr_name;
        this.dateTxt.text = "획득일: " + character.unlockDate;
        this.introTxt.text = character.introduction;
        this.levelTxt.text = character.level.ToString();
        this.fullnessSlider.value = character.fullness;
        this.energySlider.value = character.energy;

        for (int i = 0; i < character.script.Length; i++)
        {
            GameObject scriptPanel = Instantiate(scriptPanelPrefab, transform.position, Quaternion.identity);
            scriptPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Lv" + (i + 1) + " ▶";

            if (character.level > i)
                scriptPanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = character.script[i];
            else
            {
                scriptPanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "레벨 " + (i + 1) + "달성 시 열람 가능.";
                scriptPanel.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = new Color(255 / 255f, 111 / 255f, 111 / 255f);
            }

            scriptPanel.transform.SetParent(scriptContainer.transform, false);
        }
    }

    void Update()
    {

    }
}
