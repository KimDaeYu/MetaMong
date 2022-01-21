using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosAtCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public float distance = 5f;
    public Vector3 offset = new Vector3(0,0,0);
    // Update is called once per frame
    void Update()
    {
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, distance)) + offset;
    }
}
