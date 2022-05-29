

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

    public GameObject SignInUI;
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
            errorText.text = "�̸����� �Է��� �ּ���";
            return false;
        }
        if (string.IsNullOrWhiteSpace(passwordInput.text))
        {
            errorText.text = "��й�ȣ��? �Է��� �ּ���";
        }
        if (passwordInput.text.Length < 6)
        {
            errorText.text = "��й�ȣ��? 6�ڸ� �̻��̾��? �մϴ�";
            return false;
        }
        if (string.IsNullOrWhiteSpace(nameInput.text))
        {
            errorText.text = "�̸��� �Է��� �ּ���";
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
            //�α��� ����
            gameObject.SetActive(false);
            //profileUI.SetActive(true);
            SignInUI.SetActive(true);
            
        }
        else
        {
            errorText.text = error switch
            {
                AuthManager.SignUpError.InvalidEmail => "�̸��� ������ �ùٸ��� �ʽ��ϴ�",
                AuthManager.SignUpError.EmailAlreadyInUse => "�̹� �������? �̸��� �Դϴ�",
                _ => "��Ʈ��ũ ����"
            };
        }
        SetInteractable(true);
    }

    void Cancel()
    {
        gameObject.SetActive(false);
        SignInUI.SetActive(true);
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
