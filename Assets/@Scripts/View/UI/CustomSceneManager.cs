// 2025-08-04 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomSceneManager : MonoBehaviour
{
    private static CustomSceneManager _instance;
    // 싱글톤 인스턴스
    public static CustomSceneManager Instance
    {
        get
        {
            if (_instance == null)
            {
                var obj = new GameObject("CustomSceneManager");
                _instance = obj.AddComponent<CustomSceneManager>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }

    // 씬 전환 메서드
    public void LoadScene(string sceneName)
    {
        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError($"Scene '{sceneName}' cannot be loaded. Make sure it is added to the Build Settings.");
        }
    }

    // 현재 씬 다시 로드
    public void ReloadCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    // 비동기 씬 로드
    public void LoadSceneAsync(string sceneName)
    {
        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            StartCoroutine(LoadSceneAsyncCoroutine(sceneName));
        }
        else
        {
            Debug.LogError($"Scene '{sceneName}' cannot be loaded. Make sure it is added to the Build Settings.");
        }
    }

    private System.Collections.IEnumerator LoadSceneAsyncCoroutine(string sceneName)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncOperation.isDone)
        {
            // 로딩 진행률 출력 (0~1)
            Debug.Log($"Loading progress: {asyncOperation.progress * 100}%");
            yield return null;
        }
    }
}
