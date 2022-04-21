using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class SetLandmark : MonoBehaviour
{
    public bool Show;
    public bool ShowObjectInfo;
    // Start is called before the first frame update
    private GameObject canvas;
    private GameObject canvas2;
    public GameObject arrow;
    public GameObject SetCoordBtn;

    private GameObject btn1;
    private Text btn1Text;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        canvas = GameObject.Find(gameObject.name + "/Canvas");
        canvas2 = GameObject.Find(gameObject.name + "/ObjectInfoCanvas");
        btn1 = GameObject.Find(gameObject.name + "/ButtonCanvas/ToggleInfoButton");

        if (btn1)
        {
            btn1Text = btn1.GetComponentInChildren<Text>();
        }

        UpdateInfo();
    }

    private void UpdateInfo()
    {
        if (!canvas || !canvas2) return;

        canvas.SetActive(Show);
        canvas2.SetActive(ShowObjectInfo);

        var message = Show ?  "Hide Info Overlay" : "Show Info Overlay";

        if (btn1Text)
        {
            btn1Text.text = message;
        }
    }


    public void Toggle()
    {
        Show = !Show;
        UpdateInfo();
    }

}