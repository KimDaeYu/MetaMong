using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceInstance : MonoBehaviour
{
    public static SpaceInstance Instance { get; private set; }
    public string name = "space";
    public Texture2D image = null;
    public double x = 126.653848;
    public double y = 37.446937;
    public float tilt = 1.1f;
    public float distance = 2.2f;
    public float compass = 3.3f;
    public float radius = 50;

    float lastdistance = 0f;
    private void Awake () {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if(lastdistance != distance){
            Debug.Log("Cone Update!!");
            lastdistance = distance;
            Vector3 _Target = new Vector3(compass,tilt,distance); //compass, updown, distance
            GameObject.Find("AnchorManager").GetComponent<OrientationArrow>().Target = _Target;
        }
    }
}
