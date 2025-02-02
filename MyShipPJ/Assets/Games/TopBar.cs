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
    public Slider fullenssSlider;
    public Image fullnessFill;
    public Slider energySlider;
    public Image energyFill;
    public Slider favorSlider;
    public TextMeshProUGUI characterLevelTxt;

    public GameObject settingsPanel;
    public GameObject coinShopPanel;

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

    // 호감도 UI
    void SetFavorUI()
    {
        characterLevelTxt.text = curCharacter.level.ToString();
        favorSlider.maxValue = DataManager.instance.favorMax[curCharacter.level];
        favorSlider.value = curCharacter.favor;
    }

    // 포만도 UI
    void SetFullnessUI()
    {
        fullenssSlider.value = curCharacter.fullness;
        SetSliderFillColor(fullenssSlider, fullnessFill);
    }

    // 에너지 UI
    void SetEnergyUI()
    {
        energySlider.value = curCharacter.energy;
        SetSliderFillColor(energySlider, energyFill);
    }

    // 코인 UI
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

    // 슬라이더 컬러
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
    
    // 코인샵 오픈
    public void OpenCoinShop(){
        coinShopPanel.SetActive(true);
    }

    // 코인샵 액션
    public void BuyCoin(int value){

    }

    public void OpenSettingsPanel(){
        settingsPanel.SetActive(true);
    }

    
}
