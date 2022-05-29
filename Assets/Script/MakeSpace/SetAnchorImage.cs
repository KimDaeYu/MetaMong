using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetAnchorImage : MonoBehaviour
{
    public Camera _mcamera;
    public Camera _camera;
    public Texture2D _screenShot;

    public void TakeScreenShot()
    {
        //yield return new WaitForEndOfFrame();
        
        var resWidth = Screen.width;
        var resHeight = Screen.height;
        RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
        _camera.targetTexture = rt;
        _screenShot= new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
        _camera.Render();
        RenderTexture.active = rt;
        _screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
        _screenShot.Apply();
        _camera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        _mcamera.targetTexture = null;
        Sprite tempSprite = Sprite.Create(_screenShot,new Rect(0,0,resWidth,resHeight),new Vector2(0,0));
        gameObject.GetComponent<Image>().sprite = tempSprite;
    }
}
