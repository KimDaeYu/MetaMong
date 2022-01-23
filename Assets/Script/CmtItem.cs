using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CmtItem
{
    public string content;
    public string date;
    public string id;

    public CmtItem(string _content, string _date, string _id){
        this.content  = _content;
        this.date = _date;
        this.id =_id;
    }

    public void Show(){
        Debug.Log(this.content);
        Debug.Log(this.date);
        Debug.Log(this.id);
    }
}
