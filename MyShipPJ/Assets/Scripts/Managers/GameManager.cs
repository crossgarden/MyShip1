using System;
using System.Collections;
using System.Collections.Generic;
using GameData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Character curCharacter;
    public List<Character> CharactersSorted;
    readonly int decayRateFullness = 108; // 초당 감소율. 108: 3시간동안 100 > 0
    readonly int improveRate = 12;  // 20분동안 0 > 100

    private DateTime offTime;

    private void Awake()
    {
        // 싱글톤
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        CharacterInit();
        InitFullness();
        StartCoroutine(DecayFullness());
        StartCoroutine(ImproveEnergy());
    }

    /** [ - ] 시스템 */

    /** 1. 백그라운드 계산을 위한 종료시 현재시간 저장(scence 전환시에도 필요하나?) */
    void OnApplicationQuit()
    {
        // 종료 시 현재 시간을 저장
        PlayerPrefs.SetString("OffTime", DateTime.Now.ToString());
        PlayerPrefs.Save();
    }

    /** 2. 첫 실행 시 증감량 적용 */
    void InitFullness()
    {
        if (PlayerPrefs.HasKey("OffTime"))
        {
            TimeSpan timeDifference = DateTime.Now - DateTime.Parse(PlayerPrefs.GetString("OffTime")); ;
            int elapsedTime = (int)timeDifference.TotalSeconds; // 경과 시간

            print("시간경과: " + elapsedTime + "초 | " + "감소량: " + (elapsedTime / decayRateFullness) + ", 증가량: " + (elapsedTime / improveRate));
            // 포만도 감소
            curCharacter.fullness = Mathf.Max(curCharacter.fullness - (elapsedTime / decayRateFullness), 0);

            // 에너지 증가
            foreach (Character cha in CharactersSorted)
                if (cha.locked == 0)
                    cha.energy = Mathf.Min(cha.energy + (elapsedTime / improveRate), 100);

            if (PlayerPrefs.GetInt("IsLightOn") == 1 || PlayerPrefs.GetInt("CurRoom") != (int)RoomNum.PRIVATE)
                curCharacter.energy = Mathf.Max(curCharacter.energy - (elapsedTime / improveRate), 0);

            DataManager.instance.saveData();
        }
    }

    /** 3. 시간에 따른 메인 캐릭터의 포만도 감소 */
    IEnumerator DecayFullness()
    {
        while (curCharacter.fullness > 0)
        {
            yield return new WaitForSecondsRealtime(decayRateFullness);
            int index = PlayerPrefs.GetInt("CurCharacter", 0);
            curCharacter = DataManager.instance.characterSotred[index];

            curCharacter.fullness -= 1;
            DataManager.instance.saveData();

            UIManager.instance.UpdateCharacterList();
            print("포만도 감소");
        }
    }

    /** 4. 시간에 따른 서브 캐릭터 + 불 꺼둔 메인 캐릭터 에너지 증가 */
    IEnumerator ImproveEnergy()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(improveRate);

            foreach (Character cha in CharactersSorted)
                if (cha.locked == 0)
                    cha.energy = Mathf.Min(cha.energy + 1, 100);

            if (PlayerPrefs.GetInt("IsLightOn") == 1 || PlayerPrefs.GetInt("CurRoom") != (int)RoomNum.PRIVATE)
                curCharacter.energy = Mathf.Max(curCharacter.energy - 1, 0);

            DataManager.instance.saveData();
            UIManager.instance.UpdateCharacterList();
            print("에너지 증가");
        }
    }

    /*********** [1-5] character ***********************************************************/
    /** 1. 캐릭터 초기화 */
    public void CharacterInit()
    {
        CharactersSorted = DataManager.instance.characterSotred;
        int index = PlayerPrefs.GetInt("CurCharacter", 0);
        curCharacter = DataManager.instance.characterSotred[index];
    }

    /** 2. 캐릭터 교체 - index로 */
    public void CharacterChange(int index)
    {
        PlayerPrefs.SetInt("CurCharacter", index);
        curCharacter = DataManager.instance.characterSotred[index];
    }

    /** 2. 캐릭터 교체 - Charater로 */
    public void CharacterChange(Character character)
    {
        for(int i = 0 ; i < CharactersSorted.Count ; i++){
            if(CharactersSorted[i] == character){
                PlayerPrefs.SetInt("CurCharacter", i);
                curCharacter = DataManager.instance.characterSotred[i];
                break;
            }
        }
    }


}

