using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowManager : MonoBehaviour
{
    // https://mystyle.tistory.com/11
    

    private GameObject target;
    void Start()
    {

        target=GameObject.Find("ArSpaceManager").GetComponent<AddSpace>().get_current_space();
        if(target==null){
            
            gameObject.SetActive(false);
            

        }
        
    }

    // Update is called once per frame
    void Update()
    {
        //target=GameObject.Find("circle(Clone)");
        //-72.22086 5 5.208385
        //var target_position=target.transform.position;

        target=GameObject.Find("ArSpaceManager").GetComponent<AddSpace>().get_current_space();
        if(target==null){
            
            gameObject.SetActive(false);
            return;

        }

        Vector3 dir = target.transform.position - gameObject.transform.position;
        dir.y=0;
        Debug.Log(dir);
        
        Quaternion rot=Quaternion.LookRotation(dir.normalized);
        gameObject.transform.rotation = rot;
    }
}
