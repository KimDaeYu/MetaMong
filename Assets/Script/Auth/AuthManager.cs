using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Storage;
using Firebase.Extensions;
using Cysharp.Threading.Tasks;

public class AuthManager : MonoBehaviour
{
    public static AuthManager Instance { get; private set; }

    FirebaseApp app;
    FirebaseAuth auth;
    FirebaseDatabase db;
    FirebaseStorage storage;

    public bool isLoaded { get; private set; } = false;
    bool isLoading = false;
    System.Action<bool> onLoad;

    public interface IUser
    {
        public string userId { get; }
        public string email { get; }
        public string name { get; }
        public string bio { get; }
        public Texture2D photo { get; }
    }

    class User : IUser
    {
        public FirebaseUser user;
        public string userId { get; set; }
        public string email { get; set; }

        UserProfile profile;
        public string name { get => profile.name; }
        public string bio { get => profile.bio; }
        public Texture2D photo { get => profile.photo; }

        //public User() { }

        public User(FirebaseUser user, UserProfile profile)
        {
            this.user = user;

            userId = user.UserId;
            email = user.Email;

            UpdateProfile(profile);
        }

        public void UpdateProfile(UserProfile profile)
        {
            this.profile = profile;
        }
    }

    public struct UserProfile
    {
        public string name;
        public string bio;
        public Texture2D photo;
    }

    User _user;
    public IUser user { get => _user; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            Load().Forget();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void Load(System.Action<bool> callback = null)
    {
        if (isLoaded)
        {
            callback?.Invoke(true);
            return;
        }

        onLoad += callback;
        if (isLoading)
        {
            return;
        }
        Load().Forget();
    }

    async UniTask<bool> Load()
    {
        if (isLoaded)
        {
            return true;
        }

        if (isLoading)
        {
            await UniTask.WaitUntil(() => !isLoading);
            return isLoaded;
        }

        isLoading = true;
        try
        {
            var status = await FirebaseApp.CheckAndFixDependenciesAsync();
            if (status != DependencyStatus.Available)
            {
                return false;
            }

            app = FirebaseApp.DefaultInstance;
            auth = FirebaseAuth.GetAuth(app);
            db = FirebaseDatabase.GetInstance(app, "https://metamong-c173d-default-rtdb.asia-southeast1.firebasedatabase.app/");
            storage = FirebaseStorage.GetInstance(app);

            if (auth.CurrentUser != null)
            {
                await LoadUser(auth.CurrentUser);
            }
            isLoaded = true;
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
        }
        finally
        {
            isLoading = false;
            onLoad?.Invoke(isLoaded);
            onLoad = null;
        }
        return isLoaded;
    }

    async UniTask LoadUser(FirebaseUser user)
    {
        var profile = await LoadProfile(user);
        _user = new User(user, profile);
    }

    async UniTask<Texture2D> DownloadTexture(System.Uri uri)
    {
        var www = UnityWebRequestTexture.GetTexture(uri);
        await www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            return null;
        }
        else
        {
            return DownloadHandlerTexture.GetContent(www);
        }
    }

    async UniTask<UserProfile> LoadProfile(FirebaseUser user)
    {
        var profileDBRef = db.GetReference("users/" + user.UserId);
        var profileDBSnapshot = await profileDBRef.GetValueAsync();
        var profile = new UserProfile
        {
            name = profileDBSnapshot.Child("name").Value as string,
            bio = profileDBSnapshot.Child("bio").Value as string,
        };

        var profilePhotoRef = storage.GetReference("users/" + user.UserId + "/photo");
        System.Uri profilePhotoUri = null;
        try
        {
            profilePhotoUri = await profilePhotoRef.GetDownloadUrlAsync();
        }
        catch (StorageException e) when (e.ErrorCode == StorageException.ErrorObjectNotFound)
        {
            // Debug.Log("ErrorObjectNotFound");
        }

        
        if (profilePhotoUri != null)
        {
            profile.photo = await DownloadTexture(profilePhotoUri);
        }

        return profile;
    }

    public enum SignInError
    {
        None,
        InvalidEmail,       // 이메일 형식 오류
        MissingEmail,       // 이메일 없음
        WrongPassword,      // 패스워드 오류
        MissingPassword,    // 비밀번호 없음
        UserNotFound,       // 없는 이메일
        NetworkError,       // 그 외
    }

    async UniTask<SignInError> SignIn(string email, string password)
    {
        try
        {
            var user = await auth.SignInWithEmailAndPasswordAsync(email, password);
            await LoadUser(user);
        }
        catch (System.AggregateException ae)
        {
            foreach (var e in ae.Flatten().InnerExceptions)
            {
                if (e is FirebaseException fe)
                {
                    Debug.Log(string.Format("[{0}: {1}]: {2}", fe.ErrorCode, ((AuthError)fe.ErrorCode).ToString(), fe.Message));
                    SignInError error = (AuthError)fe.ErrorCode switch
                    {
                        AuthError.InvalidEmail => SignInError.InvalidEmail,
                        AuthError.MissingEmail => SignInError.MissingEmail,
                        AuthError.WrongPassword => SignInError.WrongPassword,
                        AuthError.MissingPassword => SignInError.MissingPassword,
                        AuthError.UserNotFound => SignInError.UserNotFound,
                        _ => SignInError.NetworkError
                    };
                    return error;
                }
            }

            Debug.Log(ae);
            return SignInError.NetworkError;
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
            return SignInError.NetworkError;
        }

        return SignInError.None;
    }

    public void SignIn(string email, string password, System.Action<SignInError> callback = null)
    {
        SignIn(email, password).ContinueWith(callback);
    }

    public enum SignUpError
    {
        None,
        InvalidEmail,       // 이메일 형식 오류
        MissingEmail,       // 이메일 없음
        WeakPassword,       // 비밀번호가 6자리 미만
        MissingPassword,    // 비밀번호 없음
        EmailAlreadyInUse,  // 이미 사용중인 이메일
        MissingName,        // 이름 없음
        NetworkError,       // 그 외
    }

    async UniTask<SignUpError> SignUp(string email, string password, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return SignUpError.MissingName;
        }

        try
        {
            var user = await auth.CreateUserWithEmailAndPasswordAsync(email, password);
            var profile = db.GetReference("users/" + user.UserId);
            var dict = new Dictionary<string, object>
            {
                {"name", name }
            };
            await profile.SetValueAsync(dict);
            _user = new User(user, new UserProfile { name = name });
        }
        catch (System.AggregateException ae)
        {
            foreach (var e in ae.Flatten().InnerExceptions)
            {
                if (e is FirebaseException fe)
                {
                    Debug.Log(string.Format("[{0}: {1}]: {2}", fe.ErrorCode, ((AuthError)fe.ErrorCode).ToString(), fe.Message));
                    SignUpError error = ((AuthError)fe.ErrorCode) switch
                    {
                        AuthError.InvalidEmail => SignUpError.InvalidEmail,
                        AuthError.MissingEmail => SignUpError.MissingEmail,
                        AuthError.WeakPassword => SignUpError.WeakPassword,
                        AuthError.MissingPassword => SignUpError.MissingPassword,
                        AuthError.EmailAlreadyInUse => SignUpError.EmailAlreadyInUse,
                        _ => SignUpError.NetworkError
                    };
                    return error;
                }
            }

            Debug.Log(ae);
            return SignUpError.NetworkError;
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
            return SignUpError.NetworkError;
        }

        return SignUpError.None;
    }

    public void SignUp(string email, string password, string name, System.Action<SignUpError> callback = null)
    {
        SignUp(email, password, name).ContinueWith(callback);
    }

    public void SignOut()
    {
        if (user != null)
        {
            auth.SignOut();
            _user = null;
        }
    }

    public enum UpdateProfileError
    {
        None,
        NotSignedIn,        // 로그인되지 않음
        MissingName,        // 이름이 없음
        PhotoNotReadable,   // Texture2D의 readable이 false
        NetworkError,       // 그 외
    }

    async UniTask<UpdateProfileError> UpdateProfile(UserProfile profile)
    {
        if (_user == null)
        {
            return UpdateProfileError.NotSignedIn;
        }
        if (string.IsNullOrWhiteSpace(profile.name))
        {
            return UpdateProfileError.MissingName;
        }
        if (profile.photo != null && !profile.photo.isReadable)
        {
            return UpdateProfileError.PhotoNotReadable;
        }

        var dict = new Dictionary<string, object>();
        if (name != profile.name)
        {
            dict.Add("name", profile.name);
        }
        if (profile.bio != _user.bio)
        {
            dict.Add("bio", profile.bio);
        }

        try
        {
            if (dict.Count > 0)
            {
                var profileDBRef = db.GetReference("users/" + _user.userId);
                await profileDBRef.UpdateChildrenAsync(dict);
            }

            if (profile.photo != _user.photo)
            {
                var photoRef = storage.GetReference("users/" + _user.userId + "/photo");
                if (profile.photo == null)
                {
                    await photoRef.DeleteAsync();
                }
                else
                {
                    await photoRef.PutBytesAsync(profile.photo.EncodeToPNG());
                }
            }

            _user.UpdateProfile(profile);
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
            return UpdateProfileError.NetworkError;
        }
        

        return UpdateProfileError.None;
    }

    public void UpdateProfile(UserProfile profile, System.Action<UpdateProfileError> callback = null)
    {
        UpdateProfile(profile).ContinueWith(callback);
    }
}
