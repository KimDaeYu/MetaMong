using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Database;
using Firebase.Storage;
using Firebase.Functions;
using Newtonsoft.Json.Linq;

public partial class DBManager : MonoBehaviour
{
    public static DBManager Instance { get; private set; }

    string dbUrl = "https://metamong-c173d-default-rtdb.asia-southeast1.firebasedatabase.app/";

    FirebaseApp app;
    FirebaseDatabase db;
    FirebaseStorage storage;
    FirebaseFunctions functions;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            Init();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Init()
    {
        app = FirebaseApp.DefaultInstance;
        db = FirebaseDatabase.GetInstance(app, dbUrl);
        db.SetPersistenceEnabled(false);
        storage = FirebaseStorage.GetInstance(app);
        functions = FirebaseFunctions.GetInstance(app, "asia-northeast3");
    }

    // Start is called before the first frame update
    void Start()
    {
        InitSNS();
    }

    async UniTask<string> Push(string path, object data)
    {
        var newRef = db.GetReference(path).Push();
        try
        {
            await newRef.SetValueAsync(data);
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
            return null;
        }
        return newRef.Key;
    }

    async UniTask<bool> SetValue(DatabaseReference reference, object data)
    {
        try
        {
            await reference.SetValueAsync(data);
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
            return false;
        }
        return true;
    }

    async UniTask<DataSnapshot> GetValue(DatabaseReference reference)
    {
        try
        {
            return await reference.GetValueAsync();
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
            return null;
        }
    }

    async UniTask<StorageReference> UploadImage(string location, Texture2D image)
    {
        // Readable한 image만 주어진다고 가정하고 체크 생략

        var newRef = storage.GetReference(location);
        try
        {
            await newRef.PutBytesAsync(image.EncodeToPNG());
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
            return null;
        }
        return newRef;
    }

    async UniTask<Texture2D> DownloadImage(string location)
    {
        System.Uri url;
        try
        {
            url = await storage.GetReference(location).GetDownloadUrlAsync();
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
            return null;
        }

        var www = UnityWebRequestTexture.GetTexture(url);
        await www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            return null;
        }
        else
        {
            return DownloadHandlerTexture.GetContent(www);
        }
    }

    async UniTask<object> CallFunction(HttpsCallableReference reference, object data)
    {
        HttpsCallableResult result = null;
        try
        {
            result = await reference.CallAsync(data);
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
        }

        // Debug.Log(result.Data);
        return result.Data;
    }

    async UniTask<string> HTTPGet(string url, Dictionary<string, object> query = null)
    {
        if (query != null)
        {
            url = url + '?';
            foreach (var entry in query)
            {
                url = url + entry.Key + '=' + entry.Value + '&';
            }
        }
        UnityWebRequest www = UnityWebRequest.Get(url);
        try
        {
            await www.SendWebRequest();
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
            return null;
        }
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            return null;
        }
        else
        {
            return www.downloadHandler.text;
        }
    }

    async UniTask<string> HTTPPostWithJson(string url, string json)
    {
        UnityWebRequest www = new UnityWebRequest(url, "POST");
        www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));
        www.SetRequestHeader("Content-Type", "application/json");
        www.downloadHandler = new DownloadHandlerBuffer();
        try
        {
            await www.SendWebRequest();
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
            return null;
        }
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            return null;
        }
        else
        {
            return www.downloadHandler.text;
        }
    }
}
