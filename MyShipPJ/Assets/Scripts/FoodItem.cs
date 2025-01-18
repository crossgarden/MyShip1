using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FoodItem : MonoBehaviour
{
    string path = "Sprites/Items/Foods/";
    public Image img;
    public TextMeshProUGUI countTxt, FullnessTxt, favorTxt, descript; // descript는 나중에 꾹 누르면 말풍선 팝업으로 뜨게

    public void SetUI(string name, string count, string fullness, string favor, string descript){

        this.countTxt.text = "x" + count;
        this.FullnessTxt.text = fullness;
        this.favorTxt.text = favor;
        this.descript.text = descript;
        print("path" + path);
        img.sprite = Resources.Load<Sprite>(path+name);
    }
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
