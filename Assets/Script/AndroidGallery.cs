using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;


#if UNITY_ANDROID
using UnityEngine.Android;
#endif

public class AndroidGallery : MonoBehaviour
{
    // string path = "/storage/emulated/0/DCIM/MetaMong";
    // private bool _willTakeScreenShot = false;
    // private string file_name;

    /***********************************************************************
    *                               Public Fields
    ***********************************************************************/
    #region .

    
    public string folderName = "ScreenShots";
    public string fileName = "MyScreenShot";
    public string extName = "png";
    public ScreenShotFlash flash;
    
    private bool _willTakeScreenShot = false;
    #endregion
    /***********************************************************************
    *                               Fields & Properties
    ***********************************************************************/
    #region .
    private Texture2D _imageTexture; // imageToShow의 소스 텍스쳐

    private string RootPath
    {
        get
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            return Application.dataPath;
#elif UNITY_ANDROID
            return $"/storage/emulated/0/DCIM/{Application.productName}/";
            //return Application.persistentDataPath;
#endif
        }
    }
    private string FolderPath => $"{RootPath}/{folderName}";
    private string TotalPath => $"{FolderPath}/{fileName}_{DateTime.Now.ToString("MMdd_HHmmss")}.{extName}";

    private string lastSavedPath;
    #endregion

    public void ClickCapture(){
        Debug.Log("Click!!");
        #if UNITY_ANDROID
                CheckAndroidPermissionAndDo(Permission.ExternalStorageWrite, () => _willTakeScreenShot = true);
        #else
                _willTakeScreenShot = true;
        #endif
    }

    // private void OnPostRender()
    // {   
    //     if (_willTakeScreenShot)
    //     {
    //         Debug.Log("Capture!!");
    //         _willTakeScreenShot = false;
            
    //         // Screen Capture Code Here //
    //         Texture2D screenTex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
    //         Rect area = new Rect(0f, 0f, Screen.width, Screen.height);
    //         screenTex.ReadPixels(area, 0, 0);

    //         // // 폴더가 존재하지 않으면 새로 생성
    //         // if (Directory.Exists("./Cam") == false)
    //         // {
    //         //     Directory.CreateDirectory("./Cam");
    //         // }

    //         // 폴더가 존재하지 않으면 새로 생성
    //         if (Directory.Exists(path) == false)
    //         {
    //             Directory.CreateDirectory(path);
    //         }

    //         // 스크린샷 저장
    //         File.WriteAllBytes(path+ file_name, screenTex.EncodeToPNG());

    //     }
    // }

    // UI 제외하고 현재 카메라가 렌더링하는 모습 캡쳐
    private void OnPostRender()
    {
        if (_willTakeScreenShot)
        {
            _willTakeScreenShot = false;
            CaptureScreenAndSave();
        }
    }


#if UNITY_ANDROID
    /// <summary> 안드로이드 - 권한 확인하고, 승인시 동작 수행하기 </summary>
    private void CheckAndroidPermissionAndDo(string permission, Action actionIfPermissionGranted)
    {
        // 안드로이드 : 저장소 권한 확인하고 요청하기
        if (Permission.HasUserAuthorizedPermission(permission) == false)
        {
            PermissionCallbacks pCallbacks = new PermissionCallbacks();
            // pCallbacks.PermissionGranted += str => Debug.Log($"{str} 승인");
            // pCallbacks.PermissionGranted += str => AndroidToast.I.ShowToastMessage($"{str} 권한을 승인하셨습니다.");
            // pCallbacks.PermissionGranted += _ => actionIfPermissionGranted(); // 승인 시 기능 실행

            // pCallbacks.PermissionDenied += str => Debug.Log($"{str} 거절");
            // pCallbacks.PermissionDenied += str => AndroidToast.I.ShowToastMessage($"{str} 권한을 거절하셨습니다.");

            // pCallbacks.PermissionDeniedAndDontAskAgain += str => Debug.Log($"{str} 거절 및 다시는 보기 싫음");
            // pCallbacks.PermissionDeniedAndDontAskAgain += str => AndroidToast.I.ShowToastMessage($"{str} 권한을 격하게 거절하셨습니다.");

            Permission.RequestUserPermission(permission, pCallbacks);
        }
        else
        {
            actionIfPermissionGranted(); // 바로 기능 실행
        }
    }
#endif

    /// <summary> 스크린샷을 찍고 경로에 저장하기 </summary>
    private void CaptureScreenAndSave()
    {
        string totalPath = TotalPath; // 프로퍼티 참조 시 시간에 따라 이름이 결정되므로 캐싱

        Texture2D screenTex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        Rect area = new Rect(0f, 0f, Screen.width, Screen.height);

        // 현재 스크린으로부터 지정 영역의 픽셀들을 텍스쳐에 저장
        screenTex.ReadPixels(area, 0, 0);

        bool succeeded = true;
        try
        {
            // 폴더가 존재하지 않으면 새로 생성
            if (Directory.Exists(FolderPath) == false)
            {
                Directory.CreateDirectory(FolderPath);
            }

            // 스크린샷 저장
            File.WriteAllBytes(totalPath, screenTex.EncodeToPNG());
        }
        catch (Exception e)
        {
            succeeded = false;
            Debug.LogWarning($"Screen Shot Save Failed : {totalPath}");
            Debug.LogWarning(e);
        }

        // 마무리 작업
        Destroy(screenTex);

        if (succeeded)
        {
            Debug.Log($"Screen Shot Saved : {totalPath}");
            flash.Show(); // 화면 번쩍
            lastSavedPath = totalPath; // 최근 경로에 저장
        }

        // 갤러리 갱신
        RefreshAndroidGallery(totalPath);
    }

    [System.Diagnostics.Conditional("UNITY_ANDROID")]
    private void RefreshAndroidGallery(string imageFilePath)
    {
#if !UNITY_EDITOR
        AndroidJavaClass classPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject objActivity = classPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaClass classUri = new AndroidJavaClass("android.net.Uri");
        AndroidJavaObject objIntent = new AndroidJavaObject("android.content.Intent", new object[2]
        { "android.intent.action.MEDIA_SCANNER_SCAN_FILE", classUri.CallStatic<AndroidJavaObject>("parse", "file://" + imageFilePath) });
        objActivity.Call("sendBroadcast", objIntent);
#endif
    }

    // 가장 최근에 저장된 이미지 보여주기
    /// <summary> 경로로부터 저장된 스크린샷 파일을 읽어서 이미지에 보여주기 </summary>
    private void ReadScreenShotFileAndShow(Image destination)
    {
        string folderPath = FolderPath;
        string totalPath = lastSavedPath;

        if (Directory.Exists(folderPath) == false)
        {
            Debug.LogWarning($"{folderPath} 폴더가 존재하지 않습니다.");
            return;
        }
        if (File.Exists(totalPath) == false)
        {
            Debug.LogWarning($"{totalPath} 파일이 존재하지 않습니다.");
            return;
        }

        // 기존의 텍스쳐 소스 제거
        if (_imageTexture != null)
            Destroy(_imageTexture);
        if (destination.sprite != null)
        {
            Destroy(destination.sprite);
            destination.sprite = null;
        }

        // 저장된 스크린샷 파일 경로로부터 읽어오기
        try
        {
            byte[] texBuffer = File.ReadAllBytes(totalPath);

            _imageTexture = new Texture2D(1, 1, TextureFormat.RGB24, false);
            _imageTexture.LoadImage(texBuffer);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"스크린샷 파일을 읽는 데 실패하였습니다.");
            Debug.LogWarning(e);
            return;
        }

        // 이미지 스프라이트에 적용
        Rect rect = new Rect(0, 0, _imageTexture.width, _imageTexture.height);
        Sprite sprite = Sprite.Create(_imageTexture, rect, Vector2.one * 0.5f);
        destination.sprite = sprite;
    }
}
