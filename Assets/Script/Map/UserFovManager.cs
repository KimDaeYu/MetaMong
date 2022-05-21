using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserFovManager : MonoBehaviour
{
    /*
    유저 시야각 만들어주는 class

    참조: https://nicotina04.tistory.com/197
    */
    [Range(0f,360f)][SerializeField] float ViewAngle  = 0f;
    [SerializeField] float ViewRadius = 1f;
    public float meshResolution;//샘플링 비율을 조정하는 변수. 삼각형 수에 영향

    Mesh viewMesh;
    public MeshFilter viewMeshFilter;    



    // y축 오일러 각을 3차원 방향 벡터로 변환한다.
    // 원본과 구현이 살짝 다름에 주의. 결과는 같다.
    public Vector3 DirFromAngle(float angleDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleDegrees += transform.eulerAngles.y;
        }

        return new Vector3(Mathf.Cos((-angleDegrees + 90) * Mathf.Deg2Rad), 0, Mathf.Sin((-angleDegrees + 90) * Mathf.Deg2Rad));
    }
    
    void DrawFieldOfView()
    {
        // 샘플링할 점과 샘플링으로 나뉘어지는 각의 크기를 구함
        int stepCount = Mathf.RoundToInt(ViewAngle * meshResolution);
        float stepAngleSize = ViewAngle / stepCount;


        List<Vector3> viewPoints = new List<Vector3>();
        



        // 샘플링한 점으로 향하는 좌표를 계산해 stepCount 만큼의 반직선을 쏨
        for (int i = 0; i <= stepCount; i++)
        {
             float angle = transform.eulerAngles.y - ViewAngle / 2 + stepAngleSize * i;

            viewPoints.Add(transform.position + DirFromAngle(angle, true) * ViewRadius);         
            
        }
        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];
        vertices[0] = Vector3.zero;
        

        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);//월드 좌표 -> 로컬좌표
            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }


        }
        //viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
        Debug.Log("stepcount : "+stepCount.ToString());

    }

    void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
        DrawFieldOfView();
    }
    void LateUpdate()
    {
        
        //DrawFieldOfView();
    }



    void Update()
    {

    
    }
}
