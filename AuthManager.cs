using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AuthManager : MonoBehaviour
{
    // Firebase를 사용할 수 있는 상황인지를 나타내는 bool
    public bool IsFirebaseReady { get; private set; }
    // 현재 로그인이 진행중인지를 나타내는 bool
    public bool IsSignInOnProgress { get; private set; }

    public InputField emailField;
    public InputField passwordField;
    public Button signInButton;

    /*
     * 다른 스크립트에서 바로 접근할 수 있도록 static으로 선언해 줬지만
     * Singleton 패턴으로 AuthManager를 구현할 수도 있음.
     */
    // Firebase의 총괄적인 Instance
    public static FirebaseApp firebaseApp;
    // Firebase의 Auth를 관리하는 Instance
    public static FirebaseAuth firebaseAuth;
    // 로그인 한 유저 정보
    public static FirebaseUser User;

    public void Start()
    {
        signInButton.interactable = false;

        // Firebase 실행가능 여부 체크 및 App, Auth 초기화
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                var result = task.Result;

                if (result != DependencyStatus.Available)
                {
                    Debug.Log(result.ToString());
                    IsFirebaseReady = false;
                }
                else
                {
                    IsFirebaseReady = true;
                    firebaseApp = FirebaseApp.DefaultInstance;
                    firebaseAuth = FirebaseAuth.DefaultInstance;
                    Debug.Log("Connected successfully");
                }

                signInButton.interactable = IsFirebaseReady;
            }
        );
    }

    public void SignIn()
    {
        if (!IsFirebaseReady || IsSignInOnProgress || User != null)
        {
            return;
        }

        IsSignInOnProgress = true;
        signInButton.interactable = false;

        // Firebase E-mail, password 인증(비동기)
        firebaseAuth.SignInWithEmailAndPasswordAsync(emailField.text, passwordField.text).ContinueWithOnMainThread
        (task =>
            {
                Debug.Log($"Sign in status : {task.Status}");
                IsSignInOnProgress = false;
                signInButton.interactable = true;

                if (task.IsFaulted)
                {
                    Debug.LogError(task.Exception);
                }
                else if (task.IsCanceled)
                {
                    Debug.LogError("Sign-in canceled");
                }
                else
                {
                    // 로그인 성공
                    User = task.Result;
                    Debug.Log(User.Email);
                    // 씬 이동
                    SceneManager.LoadScene("Lobby");
                }
            }
        );
    }
    
    
}