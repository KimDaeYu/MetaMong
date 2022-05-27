using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageEnlargement : MonoBehaviour
{   
    Vector3 scale = Vector3.one; 
    Vector3 TargetScale = new Vector3(3f, 3f, 3f);
    Vector3 position = Vector3.zero; 
    
    private void Start() {
        GameObject.Find("AnchorImage").GetComponent<Image>().texture = GameObject.Find("PassData").GetComponent<SetData>().spaceData.image;
    }
    public void Click(){
        Debug.Log("Clicked!");
        if (scale == TargetScale){
            scale = Vector3.one;
            position = Vector3.zero; 
        }
        else{
            scale = TargetScale;
            position = new Vector3(0, -500, 0);
        }
    }

    private void Update() {
        var t = Time.deltaTime * 10;
        gameObject.transform.localScale = Vector3.Lerp(gameObject.transform.localScale, scale, t);
        transform.localPosition = Vector3.Lerp(transform.localPosition, position, t);
    }
}
