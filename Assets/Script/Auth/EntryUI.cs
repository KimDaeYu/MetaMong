using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntryUI : MonoBehaviour
{
    public Button signInButton;
    public Button signUpButton;

    public GameObject signInUI;
    public GameObject signUpUI;
    public GameObject profileUI;

    void Init()
    {
        signInButton.onClick.AddListener(OnSignInButtonClick);
        signUpButton.onClick.AddListener(OnSignUpButtonClick);

        if (!AuthManager.Instance.isLoaded)
        {
            gameObject.SetActive(false);
            AuthManager.Instance.Load(OnAuthManagerLoad);
        }
    }

    void OnSignInButtonClick()
    {
        gameObject.SetActive(false);
        signInUI.SetActive(true);
    }

    void OnSignUpButtonClick()
    {
        gameObject.SetActive(false);
        signUpUI.SetActive(true);
    }

    void Start()
    {
        Init();
    }

    void OnAuthManagerLoad(bool loaded)
    {
        if (loaded)
        {
            if (AuthManager.Instance.user != null)
            {
                profileUI.SetActive(true);
            }
            else
            {
                gameObject.SetActive(true);
            }
        }
        else
        {
            Debug.Log("Auth Loading Error");
        }
    }
}
