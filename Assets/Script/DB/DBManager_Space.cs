using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public partial class DBManager : MonoBehaviour
{
    /* ����� */
    /* ����� ���� �� ������Ʈ�� �� �� ����� ���� AuthManager�� DBManager�� �߰� */
    /* ����ϰ� ���� ��ũ��Ʈ�� using Cysharp.Threading.Tasks; �߰� */
    /* ContinueWith(calback) �Ǵ� async/await���� ȣ�� ���� */
    /* api�� �α��� �� ���¿����� ��� ���� */
    /* ���߿� ���� �α��� ����� ����̳� SNSDBTestScene.cs, SpaceDBTestScene.cs ���� */


    // ���� ����(���)
    // data: ����/���帶ũ ����
    // return: ������ ������ ������ ��Ÿ���� ARSpace ��ü, ���н� null
    public partial UniTask<ARSpace> AddSpace(ARSpaceData data);


    // �־��� ��ǥ�κ��� �־��� �Ÿ� �̳��� ��� ���� �˻�
    // x: �浵
    // y: ����
    // distance: �Ÿ�(���ʹ���)
    // return: ������ ���� ���� ��� �������� �迭, ���н� null
    public partial UniTask<ARSpace[]> GetNearSpaces(double x, double y, int distance);


    // AddSpace�� ���� ������ �� �ʿ��� ����
    // �ϳ��� ���� ���� ������ AddSpace ����
    public class ARSpaceData
    {
        /// <summary>
        /// �� ���ڿ��� ��� AddSpace ����
        /// </summary>
        public string name;

        /// <summary>
        /// �浵. -180~180 ������ ����� ���� ��� ����
        /// </summary>
        public double x;

        /// <summary>
        /// ����. -90~90 ������ ����� ���� ��� ����
        /// </summary>
        public double y;

        // AddSpace�� ���ڷ� ���Ǵ� ��� isReadable�� true���� ��
        // NativeGallery ����ϴ� ��� Load�� �� markTextureNonReadable: false �� ����
        public Texture2D image;

        public float tilt;
        public float distance;
        public float compass;

        /// <summary>
        /// ������ �ݰ�. ���ʹ���.
        /// </summary>
        public int radius;      
    }

    // AR��������
    public partial class ARSpace : ARSpaceData
    {
        public string id;

        // GetNearSpaces�� �ֺ� �������� �ҷ��� ��� �̹����� �ε� ���� ���� ���·� image�Ӽ��� null
        // GetImage�� ȣ���ϸ� �̹����� �ε��Ͽ� ��ȯ�ϰ� image�Ӽ��� �� �̹����� ������
        // AddSpace�� ��ȯ���� ��쿡�� ������ data�� image�� ������
        public partial UniTask<Texture2D> GetImage();
    }
}
