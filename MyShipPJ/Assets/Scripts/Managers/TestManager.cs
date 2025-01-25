using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameData;

public class TestManager : MonoBehaviour
{
    public List<Character> characters;
    public void ClearFullnessAllCharacters (){
        characters = DataManager.instance.characterSotred;
        foreach(Character ch in characters){
            ch.fullness = 0;
        }
        DataManager.instance.saveData();
    }

    public void ClearPlayerPrefs(){
        PlayerPrefs.DeleteAll();
    }

}
