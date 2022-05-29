using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SignInUI : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_Text errorText;
    public Button signInButton;
    public Button signUpButton;
    public GameObject signUpUI;
    public Button cancelButton;

    public GameObject entryUI;
    public GameObject profileUI;

    void Init()
    {
        signInButton.onClick.AddListener(SignIn);
        //cancelButton.onClick.AddListener(Cancel);
        signUpButton.onClick.AddListener(OnSignUpButtonClick);
    }

    bool Validate()
    {
        if (string.IsNullOrWhiteSpace(emailInput.text))
        {
            errorText.text = "이메일을 입력해 주세요";
            return false;
        }
        if (string.IsNullOrWhiteSpace(passwordInput.text))
        {
            errorText.text = "비밀번호를 입력해 주세요";
            return false;
        }

        return true;
    }

    void OnSignUpButtonClick()
    {
        gameObject.SetActive(false);
        signUpUI.SetActive(true);
    }

    void SetInteractable(bool interactable)
    {
        emailInput.interactable = interactable;
        passwordInput.interactable = interactable;
        signInButton.interactable = interactable;
        //cancelButton.interactable = interactable;
    }

    void SignIn()
    {
        if (Validate())
        {
            SetInteractable(false);
            AuthManager.Instance.SignIn(emailInput.text, passwordInput.text, SignInCallback);
        }
    }

    void SignInCallback(AuthManager.SignInError error)
    {
        if (error == AuthManager.SignInError.None)
        {
            gameObject.SetActive(false);
            SceneManager.LoadScene("Scenes/Map/MapScene");
            //로그인 시, 바로 이동하는 것으로.


            //profileUI.SetActive(true);
            //entryUI.SetActive(true);
        }
        else
        {
            errorText.text = error switch
            {
                AuthManager.SignInError.InvalidEmail => "이메일 형식이 올바르지 않습니다",
                AuthManager.SignInError.WrongPassword => "비밀번호가 일치하지 않습니다",
                AuthManager.SignInError.UserNotFound => "존재하지 않는 이메일 입니다",
                _ => "네트워크 오류"
            };
        }
        SetInteractable(true);
    }

    void Cancel()
    {
        gameObject.SetActive(false);
        entryUI.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void OnEnable()
    {
        passwordInput.text = null;
        errorText.text = null;
    }
}
