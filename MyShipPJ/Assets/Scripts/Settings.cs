using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public void SignUpAction(){

    }

    public void SignInAction(){
        DataManager.instance.SignInWithGoogle();
    }

    public void SignOutAction(){

    }

    public void DeleteIdAction(){

    }
}
