using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EditProfileUI : MonoBehaviour
{
    public RawImage photo;
    public Button photoButton;
    public TMP_InputField nameInput;
    public TMP_InputField bioInput;
    public TMP_Text errorText;
    public Button okButton;
    public Button cancelButton;

    public GameObject profileUI;

    void Init()
    {
        photoButton.onClick.AddListener(SetPhoto);
        okButton.onClick.AddListener(UpdateProfile);
        cancelButton.onClick.AddListener(Cancel);
    }

    void SetPhoto()
    {
        NativeGallery.GetImageFromGallery((path) =>
        {
            if (path != null)
            {
                Texture2D image = NativeGallery.LoadImageAtPath(path, markTextureNonReadable: false);
                photo.texture = image;
            }
        });
    }

    bool Validate()
    {
        if (string.IsNullOrWhiteSpace(nameInput.text))
        {
            errorText.text = "이름을 입력해 주세요";
            return false;
        }
        return true;
    }

    void SetInteractable(bool interactable)
    {
        photoButton.interactable = interactable;
        nameInput.interactable = interactable;
        bioInput.interactable = interactable;
        okButton.interactable = interactable;
        cancelButton.interactable = interactable;
    }

    void UpdateProfile()
    {
        if (Validate())
        {
            SetInteractable(false);
            var profile = new AuthManager.UserProfile { name = nameInput.text, bio = bioInput.text, photo = photo.texture as Texture2D };
            AuthManager.Instance.UpdateProfile(profile, UpdateProfileCallback);
        }
    }

    void UpdateProfileCallback(AuthManager.UpdateProfileError error)
    {
        if (error == AuthManager.UpdateProfileError.None)
        {
            gameObject.SetActive(false);
            profileUI.SetActive(true);
        }
        else
        {
            errorText.text = "네트워크 오류";
        }
        SetInteractable(true);
    }

    void Cancel()
    {
        gameObject.SetActive(false);
        profileUI.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    private void OnEnable()
    {
        photo.texture = AuthManager.Instance.user.photo;
        nameInput.text = AuthManager.Instance.user.name;
        bioInput.text = AuthManager.Instance.user.bio;
        errorText.text = null;
    }
}
