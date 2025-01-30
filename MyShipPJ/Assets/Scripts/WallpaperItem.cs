using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameData;
using TMPro;
using UnityEngine.UI;
using System;

public class WallpaperItem : MonoBehaviour
{

    string path = "Sprites/Items/Wallpapers/";
    public Wallpaper wallpaper;

    public Image img;
    public TextMeshProUGUI nameTxt, discrptTxt, howToGetTxt, getDateTxt, lockedHowToGetTxt, costTxt;
    public GameObject checkedImg, lockedPanel, howToGet;
    public Button buyBtn;

    public void SetUI(Wallpaper wallpaper)
    {
        this.wallpaper = wallpaper;
        img.sprite = Resources.Load<Sprite>(path + (int)wallpaper.roomNum + "_" + wallpaper.name);
        nameTxt.text = wallpaper.name;
        discrptTxt.text = "▶ " + wallpaper.descript;
        howToGetTxt.text = wallpaper.howToGet;
        getDateTxt.text = "획득일: " + wallpaper.getDate;
        lockedHowToGetTxt.text = wallpaper.howToGet;
        costTxt.text = wallpaper.cost.ToString();

        lockedHowToGetTxt.gameObject.SetActive(wallpaper.cost == 0);
        buyBtn.gameObject.SetActive(wallpaper.cost != 0);
        SetBuyBtn();
        lockedPanel.SetActive(wallpaper.locked == 1);

        checkedImg.SetActive(PlayerPrefs.GetInt((int)wallpaper.roomNum + "_wallpaper", 0) == wallpaper.id);
    }

    // 구매 버튼 세팅(coin 보유시)
    void SetBuyBtn() =>
        buyBtn.interactable = DataManager.instance.userData.coin >= wallpaper.cost;

    // 벽지 구매
    public void BuyBtnAction(){
        AudioManager.instance.PlaySFX(AudioManager.SFXClip.SUCCESS);

        DataManager.instance.userData.coin -= wallpaper.cost;
        UIManager.instance.SetCoinUI();

        wallpaper.locked = 0;
        string getDate = DateTime.Now.ToString("yyyy-MM-dd");
        wallpaper.getDate = getDate;

        DataManager.instance.saveData();
        GetWallpaper();
    }

    // 벽지 얻었을 때 lock 해제. 획득일 업데이트
    public void GetWallpaper() 
    {
        getDateTxt.text = "획득일: " + wallpaper.getDate;
        lockedPanel.SetActive(false);
    }

    // 벽지 선택
    public void SelecteWallpaper(){
        AudioManager.instance.PlaySFX(AudioManager.SFXClip.CLICK);
        PlayerPrefs.SetInt((int)wallpaper.roomNum + "_wallpaper", wallpaper.id);
        UIManager.instance.ChangeWallpaper(wallpaper);
        checkedImg.SetActive(true);
    }


}
