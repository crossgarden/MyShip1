//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Firebase.Auth;
//using UnityEngine.UI;

//using Firebase;
//using Firebase.Database;
//using Firebase.Extensions;

//using GameData;
//using System.IO;
//using System.Linq;
//using System;
//using Google;

//using UnityEditor;
//using TMPro;
//using ExitGames.Client.Photon.StructWrapping;

//public class AccountManager : MonoBehaviour
//{

//    // 가입 정보
//    public TMP_InputField inputEmail;
//    public TMP_InputField inputPW;
//    public TMP_InputField inputPW2;

//    // 가입 시 오류 메시지
//    public GameObject emailAlert, pwAlert, pw2Alert;
//    public TextMeshProUGUI emailAlertTxt;

//    // 메시지 팝업
//    public GameObject messagePanel;
//    public TextMeshProUGUI messageTxt;

//    void Start()
//    {

//    }

    
//    // 회원가입
//    public void SignIn()
//    {

//        string email = inputEmail.text.Trim();
//        string pw = inputPW.text.Trim();
//        string pw2 = inputPW2.text.Trim();

//        if (!pw2.Equals(pw))
//            pw2Alert.SetActive(true);
//        if (pw.Length < 6)
//            pwAlert.SetActive(true);
//        if (!CheckEmail(email))
//        {
//            emailAlert.SetActive(true);
//        }

//        if (!CheckEmail(email) || pw.Length < 6 || !pw2.Equals(pw2))
//            return;

//        Debug.Log(DataManager.instance.auth);

//        DataManager.instance.auth.CreateUserWithEmailAndPasswordAsync(email, pw).ContinueWithOnMainThread(task =>
//        {
//            if (task.IsCanceled)
//            {
//                Debug.Log("회원가입 취소");
//                return;
//            }
//            if (task.IsFaulted)
//            {
//                Debug.Log("회원가입 실패");
//                return;
//            }

//            Debug.Log("가입 성공");

//            messageTxt.text = "가입 중...@_@~~.";
//            messagePanel.SetActive(true);

//            DataManager.instance.user = task.Result.User;

//            gameObject.SetActive(false);
//            messageTxt.text = "가입되었습니다!";
//            Invoke("CloseMessagePanel", 2.0f);
//        });
//    }

//    void CloseMessagePanel()
//    {
//        messagePanel.SetActive(false);
//    }

//    bool CheckEmail(string email)
//    {

//        System.Text.RegularExpressions.Regex regex
//           = new System.Text.RegularExpressions.Regex(@"^([0-9a-zA-Z]+)@([0-9a-zA-Z]+)(\.[0-9a-zA-Z]+){1,}$");

//        if (!regex.IsMatch(email))
//        {
//            emailAlertTxt.text = "올바른 이메일 형식을 입력해주세요.";
//            return false;
//        }

//        bool unique = true;

//        DataManager.instance.auth.FetchProvidersForEmailAsync(email).ContinueWithOnMainThread(task =>
//        {

//            if (task.IsCanceled || task.IsFaulted)
//            {
//                Debug.LogError("이메일 중복 확인 실패: " + task.Exception);
//                return;
//            }

//            if (task.Result.Count() > 0)
//            {
//                Debug.Log("중복임");
//                unique = false;
//            }

//        });

//        return unique;

//    }

//    private static bool IsEmail(string email)
//    {
//        System.Text.RegularExpressions.Regex regex
//            = new System.Text.RegularExpressions.Regex(@"^([0-9a-zA-Z]+)@([0-9a-zA-Z]+)(\.[0-9a-zA-Z]+){1,}$");

//        return regex.IsMatch(email);
//    }
//}
