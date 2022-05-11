using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public partial class DBManager : MonoBehaviour
{
    public partial class Post
    {
        public string id;
        public string userId;
        public string userName;
        public System.DateTime date;
        public ContentType type;

        ///<summary>
        ///<see cref="type"/>�� <see cref="ContentType.Text"/>�̸� <see cref="string"/><br/>
        ///<see cref="type"/>�� <see cref="ContentType.Image"/>�̸� <see cref="Texture2D"/>
        ///</summary>
        // type�� Text�̸� string, Image�̸� Texture2D
        public object content;

        public Vector3 position;

        ///<summary>���ƿ� ��</summary>
        public int likes;

        ///<summary>��� ��</summary>
        public int comments;

        ///<summary>����ڰ� ���ƿ� �ߴ��� ����</summary>
        public bool like;
    }

    public enum ContentType
    {
        Text,
        Image
    }

    public class Comment
    {
        public string id;
        public string userId;
        public string userName;
        public System.DateTime date;
        public string content;
    }

    /// <summary>
    /// ���� ����� ������ ID <br/>
    /// ������ ������� ���� ��� null
    /// </summary>
    public string currentSpaceId { get; private set; }

    // AR������ ���� �õ�
    // spaceId: ������ ���� ID
    // postAddrListener: ������ ����Ʈ�� �߰��Ǿ��� �� ȣ��� �Լ�
    // callback: ���� ����/���н� ȣ��� �Լ�. ���� ���ΰ� ���ڷ� �־���
    // �ٸ� ������ ����� ���¿��� ȣ��� ���� ���� �� ���� �õ�
    public partial void ConnectARSpaceNode(string spaceId, System.Action<Post> postAddedListener, System.Action<bool> callback);

    // AR���� ���� ����
    // ����Ǿ� ���� ���� ���¿��� ȣ��� �ƹ� �ϵ� ���� ����.
    public partial void DisconnectARSpaceNode();


    /* ���� �Լ����� ���� AR������ ������ �ڿ� ȣ���ؾ� �� */

    // ���� ���� �� ������ ����Ʈ �߰�
    // content: ����Ʈ ����(string or Texture2D)
    //          Texture2D�� ��� isReadable�� true���� ��
    //          NativeGallery ����ϴ� ��� Load�� �� markTextureNonReadable: false �� ����
    // position: ��ǥ��
    // return: ������ �߰��� ����Ʈ, ���н� null
    public partial UniTask<Post> AddPost(string content, Vector3 position);
    public partial UniTask<Post> AddPost(Texture2D content, Vector3 position);


    // ����Ʈ�� ��� �߰�
    // postId: ����Ʈ ID
    // content: ��� ����
    // return: ������ ����Ʈ�� ��ü ��� �迭, ���н� null
    public partial UniTask<Comment[]> AddComment(string postId, string content);

    // ����Ʈ�� ��ü ��� ��������
    // postId: ����Ʈ ID
    // return: ������ ����Ʈ�� ��ü ��� �迭, ���н� null
    public partial UniTask<Comment[]> GetComments(string postId);


    // ����Ʈ�� ���ƿ�/����
    // postId: ����Ʈ ID
    // like: ���ƿ� - true, ���� - false
    // return: ������ ����Ʈ�� ��ü ���ƿ� ��, ���н� -1
    public partial UniTask<int> SetLike(string postId, bool like);


    // ����Ʈ�� ��ü ���ƿ� �� ��������
    // postId: ����Ʈ ID
    // return: ������ ����Ʈ�� ��ü ��� �迭, ���н� null
    public partial UniTask<int> GetLikes(string postId);
}
