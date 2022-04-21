using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ChgValByField : MonoBehaviour
{
    GameObject Anchor;
    // Start is called before the first frame update
    void Start()
    {
        Anchor = GameObject.Find("Anchor").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    public void Xedit(){
        Anchor.GetComponent<OrientationArrow>().TargetUpdate(new Vector3(float.Parse(gameObject.GetComponent<TMP_InputField>().text), 0, 0), Vector3.right);
    }

    public void Yedit(){
        Anchor.GetComponent<OrientationArrow>().TargetUpdate(new Vector3(0, float.Parse(gameObject.GetComponent<TMP_InputField>().text), 0), Vector3.up);
    }

    public void Zedit(){
        Anchor.GetComponent<OrientationArrow>().TargetUpdate(new Vector3(0, 0, float.Parse(gameObject.GetComponent<TMP_InputField>().text)), Vector3.forward);
    }
}
