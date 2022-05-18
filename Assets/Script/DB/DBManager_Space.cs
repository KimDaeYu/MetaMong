using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public partial class DBManager : MonoBehaviour
{
    /* 사용방법 */
    /* 사용할 씬에 빈 오브젝트를 두 개 만들고 각각 AuthManager와 DBManager를 추가 */
    /* 사용하고 싶은 스크립트에 using Cysharp.Threading.Tasks; 추가 */
    /* ContinueWith(calback) 또는 async/await으로 호출 가능 */
    /* api는 로그인 된 상태에서만 사용 가능 */
    /* 개발용 수동 로그인 방법은 노션이나 SNSDBTestScene.cs, SpaceDBTestScene.cs 참고 */


    // 공간 생성(등록)
    // data: 공간/랜드마크 정보
    // return: 성공시 생성된 공간을 나타내는 ARSpace 객체, 실패시 null
    public partial UniTask<ARSpace> AddSpace(ARSpaceData data);


    // 주어진 좌표로부터 주어진 거리 이내의 모든 공간 검색
    // x: 경도
    // y: 위도
    // distance: 거리(미터단위)
    // return: 성공시 범위 내의 모든 공간들의 배열, 실패시 null
    public partial UniTask<ARSpace[]> GetNearSpaces(double x, double y, int distance);


    // AddSpace로 공간 생성할 때 필요한 정보
    // 하나라도 빠진 값이 있으면 AddSpace 실패
    public class ARSpaceData
    {
        /// <summary>
        /// 빈 문자열인 경우 AddSpace 실패
        /// </summary>
        public string name;

        /// <summary>
        /// 경도. -180~180 범위를 벗어나는 값인 경우 실패
        /// </summary>
        public double x;

        /// <summary>
        /// 위도. -90~90 범위를 벗어나는 값인 경우 실패
        /// </summary>
        public double y;

        // AddSpace의 인자로 사용되는 경우 isReadable이 true여야 함
        // NativeGallery 사용하는 경우 Load할 때 markTextureNonReadable: false 를 설정
        public Texture2D image;

        public float tilt;
        public float distance;
        public float compass;

        /// <summary>
        /// 공간의 반경. 미터단위.
        /// </summary>
        public int radius;      
    }

    // AR공간정보
    public partial class ARSpace : ARSpaceData
    {
        public string id;

        // GetNearSpaces로 주변 공간들을 불러온 경우 이미지는 로드 되지 않은 상태로 image속성은 null
        // GetImage를 호출하면 이미지를 로드하여 반환하고 image속성이 그 이미지로 설정됨
        // AddSpace의 반환값인 경우에는 제공된 data의 image로 설정됨
        public partial UniTask<Texture2D> GetImage();
    }
}
