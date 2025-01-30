using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using GameData;

public class TopBar : MonoBehaviour
{
    public Character curCharacter;

    public TextMeshProUGUI coinTxt;

    public Slider fullenssSlider;   // 포만도 값 설정
    public Image fullnessFill;   // 포만도 게이지 색 설정

    public Slider energySlider;
    public Image energyFill;

    public Slider favorSlider;
    public TextMeshProUGUI characterLevelTxt;

    void Start()
    {
        curCharacter = GameManager.instance.curCharacter;
        SetUI();
    }

    public void SetUI(){
        SetFavorUI();
        SetFullnessUI();
        SetEnergyUI();
        SetCoinUI();
    }

    // 호감도
    void SetFavorUI()
    {
        characterLevelTxt.text = curCharacter.level.ToString();
        favorSlider.maxValue = DataManager.instance.favorMax[curCharacter.level];
        favorSlider.value = curCharacter.favor;
    }

    // 포만도 
    void SetFullnessUI()
    {
        fullenssSlider.value = curCharacter.fullness;
        SetSliderFillColor(fullenssSlider, fullnessFill);
    }

    // 에너지
    void SetEnergyUI()
    {
        energySlider.value = curCharacter.energy;
        SetSliderFillColor(energySlider, energyFill);
    }

    // 코인
    void SetCoinUI()
    {
        int coin = DataManager.instance.userData.coin;
        string txt = coin.ToString();
        if (coin >= 1000)
        {
            txt = coin / 1000 + "K";
        }
        coinTxt.text = txt;
    }

    void SetSliderFillColor(Slider slider, Image fill)
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
    
}
