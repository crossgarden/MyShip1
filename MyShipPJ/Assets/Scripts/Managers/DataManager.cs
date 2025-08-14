using System.Collections.Generic;
using UnityEngine;

//using Firebase;
//using Firebase.Database;
//using Firebase.Extensions;

using GameData;
using System.IO;
using System.Linq;
using System;
//using Google;
//using UnityEngine.UI;
//using Firebase.Auth;
using UnityEditor;
using TMPro;
using System.Threading.Tasks;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    public string userId;
    public UserData userData;

    public readonly int[] favorMax = { 0, 50, 100, 150, 200, 250, 300, 400 };

    public List<Character> characterSotred;

    // firebase
    //DatabaseReference dbref;
    //Firebase.FirebaseApp app;

    //private string webClientId = "369182402609-laecu4int0pnrrj9u269dqvbmedr360b.apps.googleusercontent.com";

    //public Firebase.Auth.FirebaseAuth auth;
    //public FirebaseUser user;
    //GoogleSignInConfiguration configuration;

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
            Destroy(gameObject);

        //configuration = new GoogleSignInConfiguration { WebClientId = webClientId, RequestEmail = true, RequestIdToken = true };
        //CheckFirebaseDependencies();
    }

    void Start()
    {
        userData = new UserData();
        path = Application.persistentDataPath + "/";
        Debug.Log(path);

        // FirebaseInit();
        loadData();
        CharacterSort();

    }


    /** 
    void CheckFirebaseDependencies()
    {
        Debug.Log(path);
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
           {
               if (task.IsCompleted)
               {
                   if (task.Result == DependencyStatus.Available)
                   {
                       auth = FirebaseAuth.DefaultInstance;
                       Debug.Log(auth + "Firebase 초기화 성공!");
                   }

                   else
                       Debug.LogError("Firebase dependency 문제 발생: " + "Could not resolve all Firebase dependencies: " + task.Result.ToString());

               }
               else
                   Debug.LogError("Dependency check was not completed. Error : " + "Firebase 초기화 실패: " + task.Exception?.Message);

           });
    }

    public void SignInWithGoogle() { OnSignIn(); }
    public void SignOutFromGoogle() { OnSignOut(); }

    private void OnSignIn()
    {

        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        Debug.Log("Calling SignIn");
        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished, TaskScheduler.FromCurrentSynchronizationContext());


        // GoogleSignIn.Configuration = new GoogleSignInConfiguration
        // {
        //     WebClientId = webClientId,  // Firebase에서 제공하는 Web Client ID
        //     RequestIdToken = true,
        //     RequestEmail = true,
        //     UseGameSignIn = false
        // };

        // Debug.Log("Calling SignIn");

        // GoogleSignIn.DefaultInstance.SignIn().ContinueWith(task =>
        // {
        //     if (task.IsFaulted)
        //     {
        //         Debug.Log("Sign-in Failed");
        //         foreach (Exception e in task.Exception.InnerExceptions)
        //             Debug.LogError("Exception: " + e.Message + " StackTrace: " + e.StackTrace);

        //     }
        //     else if (task.IsCanceled)
        //         Debug.Log("Sign-in Canceled");

        //     else
        //         Debug.Log("Sign-in Success: " + task.Result.DisplayName);

        // });


        // GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished);
    }

    private void OnSignOut()
    {
        Debug.Log("Calling SignOut");
        GoogleSignIn.DefaultInstance.SignOut();
    }

    public void OnDisconnect()
    {
        Debug.Log("Calling Disconnect");
        GoogleSignIn.DefaultInstance.Disconnect();
    }

    internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted)
        {
            using (IEnumerator<Exception> enumerator = task.Exception.InnerExceptions.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    GoogleSignIn.SignInException error = (GoogleSignIn.SignInException)enumerator.Current;
                    Debug.Log("Got Error: " + error.Status + " " + error.Message);
                }
                else
                {
                    Debug.Log("Got Unexpected Exception?!?" + task.Exception);
                }
            }
        }
        else if (task.IsCanceled)
        {
            Debug.Log("Canceled");
        }
        else
        {
            Debug.Log("Welcome: " + task.Result.DisplayName + "!");
            Debug.Log("Email = " + task.Result.Email);
            Debug.Log("Google ID Token = " + task.Result.IdToken);
            Debug.Log("Email = " + task.Result.Email);
            SignInWithGoogleOnFirebase(task.Result.IdToken);
        }
    }

    private void SignInWithGoogleOnFirebase(string idToken)
    {
        Credential credential = GoogleAuthProvider.GetCredential(idToken, null);

        auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
        {
            AggregateException ex = task.Exception;
            if (ex != null)
            {
                if (ex.InnerExceptions[0] is FirebaseException inner && (inner.ErrorCode != 0))
                    Debug.Log("\nError code = " + inner.ErrorCode + " Message = " + inner.Message);
            }
            else
            {
                Debug.Log("Sign In Successful.");
            }
        });
    }

    public void OnSignInSilently()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        Debug.Log("Calling SignIn Silently");

        GoogleSignIn.DefaultInstance.SignInSilently().ContinueWith(OnAuthenticationFinished);
    }

    public void OnGamesSignIn()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = true;
        GoogleSignIn.Configuration.RequestIdToken = false;

        Debug.Log("Calling Games SignIn");

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished);
    }

     */


    // 걍 json

    //빌드 시에 데이터관련 오류로 loadData를 수정하고 CreateDefaultUserData를 추가했습니다.
    public void loadData()
    {
        string fullPath = path + "userData.json";

        // userData.json 파일이 없으면 초기화 JSON 생성
        if (!File.Exists(fullPath))
        {
            string defaultJsonPath = Application.streamingAssetsPath + "/defaultUserData.json";

            try
            {
                string defaultJsonContent;

                // Android에서는 UnityWebRequest를 사용해야 함
                if (Application.platform == RuntimePlatform.Android)
                {
                    // Android에서 StreamingAssets 읽기
                    UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(defaultJsonPath);
                    www.SendWebRequest();

                    while (!www.isDone)
                    {
                        // 동기적으로 대기
                    }

                    if (www.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
                    {
                        defaultJsonContent = www.downloadHandler.text;
                    }
                    else
                    {
                        Debug.LogError("defaultUserData.json을 읽을 수 없습니다: " + www.error);
                        CreateDefaultUserData(fullPath);
                        return;
                    }
                }
                else
                {
                    // PC에서는 File.ReadAllText 사용 가능
                    if (File.Exists(defaultJsonPath))
                    {
                        defaultJsonContent = File.ReadAllText(defaultJsonPath);
                    }
                    else
                    {
                        Debug.LogError("defaultUserData.json 파일이 없습니다.");
                        CreateDefaultUserData(fullPath);
                        return;
                    }
                }

                // 읽어온 내용을 userData.json으로 저장
                File.WriteAllText(fullPath, defaultJsonContent);
                Debug.Log("defaultUserData.json에서 초기 데이터를 복사했습니다.");
            }
            catch (System.Exception e)
            {
                Debug.LogError("초기 데이터 생성 중 오류: " + e.Message);
                CreateDefaultUserData(fullPath);
                return;
            }
        }

        // userData 로드
        string userJson = File.ReadAllText(fullPath);
        userData = JsonUtility.FromJson<UserData>(userJson);
    }

    // 기본 데이터를 코드로 생성하는 백업 방법
    private void CreateDefaultUserData(string fullPath)
    {
        Debug.Log("기본 userData를 코드로 생성합니다.");

        // 여기에 기본 데이터 생성 로직
        userData = new UserData();
        userData.ship_level = 1;
        userData.coin = 0;
        userData.foods = new List<Food>(); // 기본 음식 데이터 추가
        userData.characters = new List<Character>(); // 기본 캐릭터 데이터 추가
        userData.games = new List<Game>(); // 기본 게임 데이터 추가

        // JSON으로 저장
        File.WriteAllText(fullPath, JsonUtility.ToJson(userData, true));
    }

    public void saveData()
    {
        File.WriteAllText(path + "userData.json", JsonUtility.ToJson(userData, true));
        // File.WriteAllText(path + "systemData.json",JsonUtility.ToJson(systemData, true));
    }

    public void CharacterSort()
    {
        characterSotred = userData.characters
                            .OrderBy(c => c.locked) // Locked == 0인 요소가 앞에 오도록
                            .ThenBy(c => c.locked == 0 ? DateTime.Parse(c.unlockDate) : DateTime.MaxValue) // Locked == 0인 경우 Date로 정렬
                            .ToList();
    }

    // public void GetGoogleToken(string googleIdToken, string googleAccessToken)
    // {
    //     Firebase.Auth.Credential credential =
    //         Firebase.Auth.GoogleAuthProvider.GetCredential(googleIdToken, googleAccessToken);

    //     auth.SignInAndRetrieveDataWithCredentialAsync(credential).ContinueWith(task =>
    //     {
    //         if (task.IsCanceled)
    //         {
    //             Debug.LogError("SignInAndRetrieveDataWithCredentialAsync was canceled.");
    //             return;
    //         }
    //         if (task.IsFaulted)
    //         {
    //             Debug.LogError("SignInAndRetrieveDataWithCredentialAsync encountered an error: " + task.Exception);
    //             return;
    //         }

    //         Firebase.Auth.AuthResult result = task.Result;
    //         Debug.LogFormat("User signed in successfully: {0} ({1})",
    //             result.User.DisplayName, result.User.UserId);
    //     });
    // }

    // public void SignInAccount()
    // {
    //     Firebase.Auth.FirebaseUser user = auth.CurrentUser;
    //     if (user != null)
    //     {
    //         string name = user.DisplayName;
    //         string email = user.Email;
    //         System.Uri photo_url = user.PhotoUrl;
    //         // The user's Id, unique to the Firebase project.
    //         // Do NOT use this value to authenticate with your backend server, if you
    //         // have one; use User.TokenAsync() instead.
    //         string uid = user.UserId;
    //     }
    // }

    // public void SignOut()
    // {
    //     auth.SignOut();
    // }

    // public void InitializeUserData()
    // {

    // }

}
