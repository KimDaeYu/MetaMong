using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SignInUI : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_Text errorText;
    public Button signInButton;
    public Button cancelButton;

    public GameObject entryUI;
    public GameObject profileUI;

    void Init()
    {
        signInButton.onClick.AddListener(SignIn);
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
            errorText.text = "��й�ȣ�� �Է��� �ּ���";
            return false;
        }

        return true;
    }

    void SetInteractable(bool interactable)
    {
        emailInput.interactable = interactable;
        passwordInput.interactable = interactable;
        signInButton.interactable = interactable;
        cancelButton.interactable = interactable;
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
            profileUI.SetActive(true);
        }
        else
        {
            errorText.text = error switch
            {
                AuthManager.SignInError.InvalidEmail => "�̸��� ������ �ùٸ��� �ʽ��ϴ�",
                AuthManager.SignInError.WrongPassword => "��й�ȣ�� ��ġ���� �ʽ��ϴ�",
                AuthManager.SignInError.UserNotFound => "�������� �ʴ� �̸��� �Դϴ�",
                _ => "��Ʈ��ũ ����"
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
