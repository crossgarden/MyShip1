using System.Collections.Generic;
using UnityEngine;

using Firebase;
using Firebase.Database;
using Firebase.Extensions;

using GameData;
using System.IO;
using System.Linq;
using System;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    public string userId;
    public UserData userData;
    
    public List<Character> characterSotred;

    // firebase
    private DatabaseReference dbref;

    // json
    public string path;    // 읽기/쓰기가 가능한 로컬저장영역

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

        userData = new UserData();
        // systemData = new SystemData();

        path = Application.persistentDataPath + "\\";
    }

    void Start()
    {
        // FirebaseInit();
        loadData();
        LoadHavingFoods();
        CharacterSort();
    }

    void Update()
    {

    }

    // 걍 json
    public void loadData()
    {
        // userData 로드 
        string userJson = File.ReadAllText(path + "userData.json");
        userData = JsonUtility.FromJson<UserData>(userJson);

        // systemData 로드
        // string systemJson = File.ReadAllText(path + "systemData.json");
        // systemData = JsonUtility.FromJson<SystemData>(systemJson);
    }

    public void saveData()
    {
        File.WriteAllText(path + "userData.json", JsonUtility.ToJson(userData, true));
        // File.WriteAllText(path + "systemData.json",JsonUtility.ToJson(systemData, true));
    }

    public void CharacterSort(){
        characterSotred = userData.characters
                            .OrderBy(c => c.locked) // Locked == 0인 요소가 앞에 오도록
                            .ThenBy(c => c.locked == 0 ? DateTime.Parse(c.unlockDate) : DateTime.MaxValue) // Locked == 0인 경우 Date로 정렬
                            .ToList();  
    }

     // count 1 이상 food 리스트 반환 - SelecetedFood.cs 에서 사용
    public List<Food> LoadHavingFoods()
    {
        List<Food> havingFoods = new List<Food>();
        for (int i = 0; i < DataManager.instance.userData.foods.Count; i++)
        {
            if (DataManager.instance.userData.foods[i].count > 0)
                havingFoods.Add(DataManager.instance.userData.foods[i]);
        }
        return havingFoods;
    }

    // 파이어베이스
    public void FirebaseInit()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                // app = Firebase.FirebaseApp.DefaultInstance; 
                // app이 뭐지????????

                // Set a flag here to indicate whether Firebase is ready to use by your app.
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }

    public void InitializeUserData()
    {

    }

   

}
