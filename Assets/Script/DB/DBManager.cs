using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Database;
using Firebase.Storage;
using Newtonsoft.Json.Linq;

public partial class DBManager : MonoBehaviour
{
    public static DBManager Instance { get; private set; }

    string serverAddr = "http://192.168.35.79:8080";
    string dbUrl = "https://metamong-c173d-default-rtdb.asia-southeast1.firebasedatabase.app/";

    FirebaseApp app;
    FirebaseDatabase db;
    FirebaseStorage storage;

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
    }

    // Start is called before the first frame update
    void Start()
    {
        InitSNS();
    }

    public class ARSpace
    {
        public string id;
        private string imageName;
        public string name;
        public double x;
        public double y;

        public async UniTask GetImage(System.Action<Texture2D> callback)
        {
            var imageRef = Instance.storage.GetReference("landmarks/" + imageName);
            System.Uri imageUrl = null;
            try
            {
                imageUrl = await imageRef.GetDownloadUrlAsync();
            }
            catch (StorageException e) when (e.ErrorCode == StorageException.ErrorObjectNotFound)
            {
                // Debug.Log("ErrorObjectNotFound");
            }
        }
    }

    void GetImage(string location)
    {

    }

    struct ARSPaceResponse
    {
        public string id;
        public string imageName;
        public string name;
        public double x;
        public double y;
    }

    public enum CreateARSpaceError
    {
        None,
        NetworkError,
    }

    string GenerateImageName(string name, double x, double y)
    {
        string planeName = string.Format("{0}-{1}-{2}", name, x, y);
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(planeName);
        string encodedName = System.Convert.ToBase64String(bytes);
        string urlSafeName = encodedName.Replace('+', '-').Replace('/', '_');
        return urlSafeName;
    }

    struct CreateARSpaceResult
    {
        public CreateARSpaceError error;
        public ARSpace space;
    }

    async UniTask<CreateARSpaceResult> CreateARSpace(Texture2D image, string name, double x, double y)
    {
        string imageName = GenerateImageName(name, x, y);
        await storage.GetReference("landmarks/" + imageName).PutBytesAsync(image.EncodeToPNG());

        var body = new Dictionary<string, string>()
        {
            {"name", name },
            {"image-name", imageName },
            {"latitude", x.ToString() },
            {"longitude", y.ToString() }
        };
        UnityWebRequest www = UnityWebRequest.Post(serverAddr + "/spaces", body);
        await www.SendWebRequest();
        
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error");
            return new CreateARSpaceResult { error = CreateARSpaceError.NetworkError };
        }

        JObject json = JObject.Parse(www.downloadHandler.text);
        if (json["status"].Value<string>() != "success")
        {
            Debug.Log(json["message"].Value<string>());
            return new CreateARSpaceResult { error = CreateARSpaceError.NetworkError };
        }

        bool isAdded = json["isAdded"].Value<bool>();
        ARSPaceResponse spaceResponse = json["space"].ToObject<ARSPaceResponse>();
        ARSpace space = new ARSpace
        {
            id = spaceResponse.id,
            name = spaceResponse.name,
            x = spaceResponse.x,
            y = spaceResponse.y,
        };
        if (!isAdded)
        {
            // storage.GetReference()
        }

        return new CreateARSpaceResult { error = CreateARSpaceError.None, space = new ARSpace { } };
    }
}
