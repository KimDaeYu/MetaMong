using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Gallery : MonoBehaviour
{
    public GameObject ImagePanel;
    public void PickImage( int maxSize )
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery( ( path ) =>
        {
            Debug.Log( "Image path: " + path );
            if( path != null )
            {
                // Create Texture from selected image
                Texture2D texture = NativeGallery.LoadImageAtPath( path, maxSize );
                if( texture == null )
                {
                    Debug.Log( "Couldn't load texture from " + path );
                    return;
                }

                Rect rect;
                Sprite img;
                ImagePanel.SetActive(true);
                rect = new Rect(0, 0, texture.width, texture.height);
                img = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f));
                if(texture.width > texture.height){
                    ImagePanel.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(800,texture.height * 800 / texture.width );
                    gameObject.GetComponent<NewStory>().landscape = true;
                }else{
                    ImagePanel.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(texture.width * 800 / texture.height ,800);
                    gameObject.GetComponent<NewStory>().landscape = false;
                }
                ImagePanel.transform.GetChild(0).GetComponent<Image>().sprite = img;
                
                gameObject.GetComponent<NewStory>().targetImage = img;
                
            }
        } );
        
        GameObject.Find("Log").GetComponent<TextMeshProUGUI>().text += "Permission result: " + permission + "\n";
    }
    
}
