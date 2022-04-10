using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProfileUI : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text emailText;
    public TMP_Text bioText;
    public RawImage photo;
    public Button editProfileButton;
    public Button signOutButton;

    public GameObject editProfileUI;
    public GameObject entryUI;

    void Init()
    {
        editProfileButton.onClick.AddListener(EditProfile);
        signOutButton.onClick.AddListener(SignOut);
    }

    void EditProfile()
    {
        gameObject.SetActive(false);
        editProfileUI.SetActive(true);
    }

    void SignOut()
    {
        AuthManager.Instance.SignOut();
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
        photo.texture = AuthManager.Instance.user.photo;
        nameText.text = AuthManager.Instance.user.name;
        emailText.text = AuthManager.Instance.user.email;
        bioText.text = AuthManager.Instance.user.bio;
    }
}
