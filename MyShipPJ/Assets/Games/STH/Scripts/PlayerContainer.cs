using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerContainer : MonoBehaviour
{
    public TextMeshProUGUI coinTxt, overCoinTxt;
    int coin = 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Coin")
        {
            AudioManager.instance.PlaySFX(AudioManager.SFXClip.SUCCESS);
            GetCoin(1);
            Destroy(other.gameObject);
        }
    }

    void GetCoin(int coinValue)
    {
        DataManager.instance.userData.coin += coinValue;
        coin += coinValue;
        coinTxt.text = coin.ToString();
        overCoinTxt.text = "+ " + coin.ToString();

    }
}
