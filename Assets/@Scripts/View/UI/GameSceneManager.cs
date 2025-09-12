using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    [SerializeField] private Camera menuCamera;

    public void LoadGameScene(NetworkRunner runner)
    {
        try
        {
            var sceneManager = runner.GetComponent<INetworkSceneManager>();
            if (sceneManager != null)
            {
                var sceneRef = SceneRef.FromIndex(2);
                var asyncOp = sceneManager.LoadScene(sceneRef, new NetworkLoadSceneParameters());
                Debug.Log("NetworkSceneManager로 Game 씬 로드 시작");
            }
            else
            {
                LoadSceneWithUnityManager();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"NetworkSceneManager 로드 실패: {e.Message}");
            LoadSceneWithUnityManager();
        }

        DisableMenuCamera();
    }

    private void LoadSceneWithUnityManager()
    {
        Debug.Log("Unity SceneManager를 사용하여 씬 로드");
        SceneManager.LoadScene("Game", LoadSceneMode.Additive);
    }

    private void DisableMenuCamera()
    {
        if (menuCamera != null)
        {
            menuCamera.enabled = false;
        }
    }
}