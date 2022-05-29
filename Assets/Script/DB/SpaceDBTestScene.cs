using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using Mapbox.Utils;
using TMPro;

public class SpaceDBTestScene : MonoBehaviour
{
    public RawImage image;

    public TMP_Dropdown create_dropdown;
    public TMP_Dropdown search_dropdown;

    public GameObject create_button;
    public GameObject search_button;


    public List<space_data> Arspace_list;
    

    private List<string> name_list = new List<string>();



    AuthManager auth;
    DBManager db;

    [System.Serializable]
    public class space_data
    {
        public string ArSpaceName;
        public Vector2d ArSpaceGps;
    }

       
    void Start()
    {
        auth = AuthManager.Instance;
        db = DBManager.Instance;
        create_dropdown.ClearOptions();
        search_dropdown.ClearOptions();


        foreach (var space_var in Arspace_list){
            name_list.Add(space_var.ArSpaceName);
        }

        create_dropdown.AddOptions(name_list);
        search_dropdown.AddOptions(name_list);

        auth.Load((loaded) =>
        {
            if (!loaded)
            {
                Debug.Log("Failed to load auth manager");
            }
            else
            {
                string email = "user1@test.com";
                string password = "121212";
                auth.SignIn(email, password, (error) =>
                {
                    if (error == AuthManager.SignInError.None)
                    {
                        Debug.Log("생성 버튼을 클릭해서, 데이터를 추가해주세요.");
                    }
                    else
                    {
                        Debug.Log(error);
                    }
                });
            }
        });
    }

    void test(Vector2d space_gps, string space_name)
    {
        NativeGallery.GetImageFromGallery((path) =>
        {
            if (path != null)
            {
                Texture2D image = NativeGallery.LoadImageAtPath(path, markTextureNonReadable: false);
                check(image,space_gps,space_name);
            }
        });
    }

    void check(Texture2D image,Vector2d space_gps, string space_name)
    {

        db.AddSpace(new DBManager.ARSpaceData
        {
            name = space_name,
            image = image,
            x = space_gps.x,
            y = space_gps.y,
            tilt = 1.1f,
            distance = 2.2f,
            compass = 3.3f,
            radius = 50,
        }).ContinueWith((space) =>
        {
            if (space == null)
            {
                Debug.Log("Failed to add space");
                Debug.Log(space_gps);
            }
            else
            {
                Debug.Log(space);
            }

        }).Forget();
        
    }

    public void create_button_clicked(){
        string finding=create_dropdown.options[create_dropdown.value].text;
        Debug.Log(finding);
        Vector2d find_gps = new Vector2d(0,0);
        foreach(space_data space_var in Arspace_list){
            if(space_var.ArSpaceName==finding){
                find_gps = space_var.ArSpaceGps; 
            }
        }

        test(find_gps,finding);

    }

    public void search_button_clicked(){
        string finding=search_dropdown.options[search_dropdown.value].text;
        Debug.Log(finding);
        search(finding);
    }

    void search(string searched_name){
            
            //60.19202222,24.964615042211662
            //126.656745, 37.451401 
            //(126.653848,37.446937) 기숙사

            db.GetNearSpaces(126.654129,37.44646, 1000).ContinueWith((spaces) =>
            {
                if (spaces == null)
                {
                    Debug.Log("주변에 공간이 없습니다.");
                }
                else
                {
                    Debug.Log(spaces.Length);
                    for(int index =0 ; index<spaces.Length ;index++){
                        if(spaces[index].name==searched_name){
                            spaces[index].GetImage().ContinueWith((img) =>
                            {
                                this.image.texture = img;
                            }).Forget();
                            Debug.Log(spaces.Length);
                            Debug.Log(spaces[index].id);
                            Debug.Log(spaces[index].name);
                            Debug.Log(spaces[index].x);
                            Debug.Log(spaces[index].y);
                            Debug.Log(spaces[index].tilt);
                            Debug.Log(spaces[index].distance);
                            Debug.Log(spaces[index].compass);
                            Debug.Log(spaces[index].radius);
                        }

                    }



                }
            }).Forget();
    }

}
