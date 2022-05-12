using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeHandler : MonoBehaviour
{  
    public GameObject AnchorHandler;
    public GameObject MainHandler;
    private bool flag = true; //true : Anchor, false : Main
    // Start is called before the first frame update
    public void Change(){
        if(flag){
            flag = false;
            AnchorHandler.SetActive(false);
            MainHandler.SetActive(true);
        }else{
            flag = true;
            AnchorHandler.SetActive(true);
            MainHandler.SetActive(false);
        }
    }
}
