using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtUser : MonoBehaviour
{
    GameObject user_object;
    void Start()
    {
        user_object=GameObject.Find("PlayerTarget").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        float user_y = user_object.transform.eulerAngles.y;
        gameObject.transform.eulerAngles = new Vector3(gameObject.transform.eulerAngles.x,
            user_y,
            gameObject.transform.eulerAngles.z);
    }
}
