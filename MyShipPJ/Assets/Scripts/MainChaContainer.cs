using System.Collections;
using System.Collections.Generic;
using GameData;
using UnityEditor;
using UnityEditor.iOS;
using UnityEngine;

public class MainChaContainer : MonoBehaviour
{
    public List<GameObject> characterPrefabs;
    public GameObject curCharacterObj;
    readonly string path = "Prefabs/Characters/";

    void Start()
    {
        foreach(Character cha in DataManager.instance.characterSotred)
            characterPrefabs.Add(Resources.Load<GameObject>(path + cha.name));
        
        GameObject curCharacterObj = Instantiate(characterPrefabs[PlayerPrefs.GetInt("CurCharacter",0)], transform.position, Quaternion.identity);
        curCharacterObj.transform.SetParent(gameObject.transform, false);
        curCharacterObj.SetActive(true);
    }

    public void ChangeCharacter(){
        Destroy(gameObject.transform.GetChild(0));
        
        GameObject curCharacterObj = Instantiate(characterPrefabs[PlayerPrefs.GetInt("CurCharacter",0)], transform.position, Quaternion.identity);
        curCharacterObj.transform.SetParent(gameObject.transform, false);
        curCharacterObj.SetActive(true);
    }

}
