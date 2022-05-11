using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using Firebase.Database;
using Firebase.Functions;

public partial class DBManager : MonoBehaviour
{
    async UniTask<string> Push(string path, object data)
    {
        var newRef = db.GetReference(path).Push();
        try
        {
            await newRef.SetValueAsync(data);
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
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
            Debug.LogException(e);
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
            Debug.LogException(e);
            return null;
        }
    }

    async UniTask<bool> UploadImage(string location, Texture2D image)
    {
        // Readable한 image만 주어진다고 가정하고 체크 생략

        var newRef = storage.GetReference(location);
        try
        {
            await newRef.PutBytesAsync(image.EncodeToPNG());
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
            return false;
        }
        return true;
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
            Debug.LogException(e);
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
            Debug.LogException(e);
        }

        // Debug.Log(result.Data);
        return result.Data;
    }
}
