using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Firebase;
using Firebase.Database;
using Firebase.Extensions;

using GameData;
using System.IO;

public class DataManager : MonoBehaviour
{

    public static DataManager instance;

    public string userId;
    public UserData userData;
    public SystemData systemData;

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
        systemData = new SystemData();

        path = Application.persistentDataPath;
    }

    void Start()
    {
        // FirebaseInit();
        loadData();
        print(userData.ToString());
    }

    void Update()
    {

    }

    // 걍 json
    public void loadData()
    {
        // userData 로드 
        string jsonData = File.ReadAllText(path + "\\userData.json");
        print(jsonData);
        userData = JsonUtility.FromJson<UserData>(jsonData);
    }

    public void saveData()
    {
        // userData 저장
        string jsonData = JsonUtility.ToJson(userData, true);
        File.WriteAllText(path + "\\userData.json", jsonData);
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
