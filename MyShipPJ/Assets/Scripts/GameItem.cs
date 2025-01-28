using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using GameData;
using UnityEngine.SceneManagement;

public class GameItem : MonoBehaviour
{
    public Game game;
    public string path = "Sprites/Items/Games/";
    public Image image;
    public TextMeshProUGUI nameTxt;

    public void SetUI(Game game)
    {
        this.game = game;
        image.sprite = Resources.Load<Sprite>(path + game.name);
        nameTxt.text = game.kr_name;
        gameObject.GetComponent<Button>().onClick.AddListener(StartGame);
    }

    public void StartGame()
    {
        PlayerPrefs.SetInt("CurGame", game.id);
        SceneManager.LoadScene("G_" + game.name);
    }

}
