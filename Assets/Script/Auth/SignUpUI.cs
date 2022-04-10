using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SignUpUI : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_InputField nameInput;
    public TMP_Text errorText;
    public Button signUpButton;
    public Button cancelButton;

    public GameObject entryUI;
    public GameObject profileUI;

    void Init()
    {
        signUpButton.onClick.AddListener(SignUp);
        cancelButton.onClick.AddListener(Cancel);
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
        }
        if (passwordInput.text.Length < 6)
        {
            errorText.text = "비밀번호는 6자리 이상이어야 합니다";
            return false;
        }
        if (string.IsNullOrWhiteSpace(nameInput.text))
        {
            errorText.text = "이름을 입력해 주세요";
            return false;
        }
        
        return true;
    }

    void SetInteractable(bool interactable)
    {
        emailInput.interactable = interactable;
        passwordInput.interactable = interactable;
        nameInput.interactable = interactable;
        signUpButton.interactable = interactable;
        cancelButton.interactable = interactable;
    }

    void SignUp()
    {
        if (Validate())
        {
            SetInteractable(false);
            AuthManager.Instance.SignUp(emailInput.text, passwordInput.text, nameInput.text, SignUpCallback);
        }
    }

    void SignUpCallback(AuthManager.SignUpError error)
    {
        if (error == AuthManager.SignUpError.None)
        {
            gameObject.SetActive(false);
            profileUI.SetActive(true);
        }
        else
        {
            errorText.text = error switch
            {
                AuthManager.SignUpError.InvalidEmail => "이메일 형식이 올바르지 않습니다",
                AuthManager.SignUpError.EmailAlreadyInUse => "이미 사용중인 이메일 입니다",
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
        emailInput.text = null;
        passwordInput.text = null;
        nameInput.text = null;
        errorText.text = null;
    }
}
