using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameData;
using TMPro;
using UnityEngine.SceneManagement;

public class HowToGame : MonoBehaviour
{
    public Game game;

    public TextMeshProUGUI titleTxt, highScoreTxt, descriptTxt;

    public void SetUI(Game game)
    {
        this.game = game;
        titleTxt.text = game.kr_name;
        highScoreTxt.text = "최고 : " + game.high_score;
        descriptTxt.text = game.descript;
    }

    public void PlayAction()
    {
        SceneManager.LoadScene("G_" + game.name);
    }

    public void BackAction()
    {
        gameObject.SetActive(false);
    }
}
