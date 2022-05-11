using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Cysharp.Threading.Tasks;

public class DBUtils
{
    public static async UniTask<bool> SetValue(DatabaseReference reference, object data)
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
}
