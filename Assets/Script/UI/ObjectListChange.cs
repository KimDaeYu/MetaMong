using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectListChange : MonoBehaviour
{
    
    private int page = 1;
    private int max_page = 3;

    public GameObject ObjectList;

    public void ChangePage(){
        for(int i=0; i < max_page; i++){
            ObjectList.transform.GetChild(i).gameObject.SetActive(false);
        }
        ObjectList.transform.GetChild(page).gameObject.SetActive(true);
        page += 1;
        if(page > 2)
            page = 0;
    }
}
