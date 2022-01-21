using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeObject : MonoBehaviour
{
    public GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void createObj(){
        
        if(true){
            var spawnObject = Instantiate(target, new Vector3(Random.Range(-10.0f, 10.0f),Random.Range(-10.0f, 10.0f),Random.Range(-10.0f, 10.0f)),new Quaternion(0,1,0,0));
            Debug.Log(spawnObject.transform.rotation.ToString());
        }


    }
}
