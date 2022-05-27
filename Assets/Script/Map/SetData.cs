using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  UnityEngine.UI;
using static DBManager;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

public class SetData : MonoBehaviour
{
    // Start is called before the first frame update

    public ARSpace spaceData;



    public void ChangeScene(){

        //이미지가 다불러 와지면 이제 씬 변경
        SceneManager.LoadScene("Scenes/Anchor_sync_test");

    }

    public void UpdateSpaceImg(ARSpace space_data){
        //공간 진입시, 이미지를 다운 받아서, don't destroy에 삽입.
        //캐싱처리도 해야함.
        spaceData = space_data;
        spaceData.GetImage().ContinueWith((img) =>
        {
            spaceData.image = img;
            Debug.Log("스페이스 이미지 로드완료.");
        }).Forget();        

    }


    void Start()
    {
        
    }

    private void Awake(){
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
