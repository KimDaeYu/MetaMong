using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> ½ºÅ©¸°¼¦ ÂïÀ» ¶§ È­¸é ¹øÂ½ÀÌ±â </summary>
public class ScreenShotFlash : MonoBehaviour
{
    public float duration = 0.3f;

    private UnityEngine.UI.Image _image;
    private float _currentAlpha = 1f;

    private void Awake()
    {
        _image = GetComponent<UnityEngine.UI.Image>();
    }
    private void Update()
    {
        Color col = _image.color;
        col.a = _currentAlpha;
        _image.color = col;

        _currentAlpha -= Time.unscaledDeltaTime / duration;

        if (_currentAlpha < 0f)
            gameObject.SetActive(false);
    }

    public void Show()
    {
        _currentAlpha = 1f;
        gameObject.SetActive(true);
    }
}
