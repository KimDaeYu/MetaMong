using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScenes : MonoBehaviour
{
    // Start is called before the first frame update

    public void Change_Scene(string scene_path){
        // main -> map
        

        if(GameObject.Find("PassData")!=null){
            Destroy(GameObject.Find("PassData"));
        }
        SceneManager.LoadScene(scene_path);
    }

    public void change_to_map(){
        
        // make -> map
        //ar location provider.

        SceneManager.LoadScene("Scenes/Main/MapScene");
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
