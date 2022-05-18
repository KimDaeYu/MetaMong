using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class SpaceDBTestScene : MonoBehaviour
{
    public RawImage image;

    AuthManager auth;
    DBManager db;
       
    void Start()
    {
        auth = AuthManager.Instance;
        db = DBManager.Instance;

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
                        test();
                    }
                    else
                    {
                        Debug.Log(error);
                    }
                });
            }
        });
    }

    void test()
    {
        NativeGallery.GetImageFromGallery((path) =>
        {
            if (path != null)
            {
                Texture2D image = NativeGallery.LoadImageAtPath(path, markTextureNonReadable: false);
                check(image);
            }
        });
    }

    void check(Texture2D image)
    {
        db.AddSpace(new DBManager.ARSpaceData
        {
            name = "test",
            image = image,
            x = 127.35,
            y = 35.127,
            tilt = 1.1f,
            distance = 2.2f,
            compass = 3.3f,
            radius = 50,
        }).ContinueWith((space) =>
        {
            if (space == null)
            {
                Debug.Log("Failed to add space");
            }
            else
            {
                Debug.Log(space);
            }
            db.GetNearSpaces(127.35, 35.127, 500).ContinueWith((spaces) =>
            {
                if (spaces == null)
                {
                    Debug.Log("Failed to get near spaces");
                }
                else
                {
                    spaces[0].GetImage().ContinueWith((img) =>
                    {
                        this.image.texture = img;
                    });
                    Debug.Log(spaces.Length);
                    Debug.Log(spaces[0].id);
                    Debug.Log(spaces[0].name);
                    Debug.Log(spaces[0].x);
                    Debug.Log(spaces[0].y);
                    Debug.Log(spaces[0].tilt);
                    Debug.Log(spaces[0].distance);
                    Debug.Log(spaces[0].compass);
                    Debug.Log(spaces[0].radius);
                }
            });
        });
    }
}
