using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

public partial class DBManager : MonoBehaviour
{
    public partial class ARSpace : ARSpaceData
    {
        protected string imageLocation;

        public partial async UniTask<Texture2D> GetImage()
        {
            if (image != null)
            {
                return image;
            }
            image = await Instance.DownloadImage(imageLocation);
            return image;
        }
    }

    class ARSpaceImpl : ARSpace
    {
        public ARSpaceImpl(SpaceJson space)
        {
            id = space.id.ToString();
            name = space.name;
            x = space.x;
            y = space.y;
            imageLocation = space.image;
            tilt = space.tilt;
            distance = space.distance;
            compass = space.compass;
            radius = space.radius;
        }

        public ARSpaceImpl(ARSpaceData data, string id, string imageLocation)
        {
            this.id = id;
            name = data.name;
            x = data.x;
            y = data.y;
            image = data.image;
            this.imageLocation = imageLocation;
            tilt = data.tilt;
            distance = data.distance;
            compass = data.compass;
            radius = data.radius;
        }
    }

    class SpaceJson
    {
        public int id { get; set; }
        public string name { get; set; }
        public double x { get; set; }
        public double y { get; set; }
        public string image { get; set; }
        public float tilt { get; set; }
        public float distance { get; set; }
        public float compass { get; set; }
        public int radius { get; set; }
    }

    class GetNearSpacesResponse
    {
        public int status { get; set; }
        public string message { get; set; }
        public List<SpaceJson> spaces { get; set; }
    }

    string serverUrl = "http://mm.imsi.cc:8080";

    /// <summary>
    /// 주어진 좌표로부터 주어진 거리 이내의 모든 공간 검색
    /// </summary>
    /// <param name="x">경도</param>
    /// <param name="y">위도</param>
    /// <param name="distance">거리(미터단위)</param>
    /// <returns></returns>
    public partial async UniTask<ARSpace[]> GetNearSpaces(double x, double y, int distance)
    {
        string result = await HTTPGet(serverUrl + "/spaces", new Dictionary<string, object> {
            {"longitude", x },
            {"latitude", y },
            {"distance", distance },
        });

        if (result == null)
        {
            return null;
        }

        var response = JsonConvert.DeserializeObject<GetNearSpacesResponse>(result);
        if (response.status != 0)
        {
            Debug.Log(response.message);
            return null;
        }

        var spaces = new ARSpace[response.spaces.Count];
        for (int i = 0; i < response.spaces.Count; ++i)
        {
            spaces[i] = new ARSpaceImpl(response.spaces[i]);
        }
        return spaces;
    }

    class AddSpaceRequest
    {
        public string name;
        public double longitude;
        public double latitude;
        public string image;
        public float tilt;
        public float distance;
        public float compass;
        public int radius;

        public AddSpaceRequest(ARSpaceData data, string imageLocation)
        {
            name = data.name;
            longitude = data.x;
            latitude = data.y;
            image = imageLocation;
            tilt = data.tilt;
            distance = data.distance;
            compass = data.compass;
            radius = data.radius;
        }
    }

    string GenerateImageName(string name, double x, double y)
    {
        string planeName = string.Format("{0}-{1}-{2}-{3}", name, x, y, System.DateTime.Now.ToBinary());
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(planeName);
        string encodedName = System.Convert.ToBase64String(bytes);
        string urlSafeName = encodedName.Replace('+', '-').Replace('/', '_');
        return urlSafeName;
    }

    /// <summary>
    /// 공간 생성(등록)
    /// </summary>
    /// <param name="data">공간/랜드마크 정보</param>
    /// <returns>성공시 생성된 공간을 나타내는 ARSpace 객체, 실패시 null</returns>
    public partial async UniTask<ARSpace> AddSpace(ARSpaceData data)
    {
        string imageLocation = "landmarks/" + GenerateImageName(data.name, data.x, data.y);
        var imageRef = await UploadImage(imageLocation, data.image);
        if (imageRef == null)
        {
            return null;
        }

        var requestBody = new AddSpaceRequest(data, imageLocation);
        string result = await HTTPPostWithJson(serverUrl + "/spaces", JsonConvert.SerializeObject(requestBody));

        if (result == null || int.Parse(result) < 0)
        {
            try
            {
                await imageRef.DeleteAsync();
            }
            catch (System.Exception e)
            {
                Debug.Log(e);
            }
            
            return null;
        }

        return new ARSpaceImpl(data, result, imageLocation);
    }
}
