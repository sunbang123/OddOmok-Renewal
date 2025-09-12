using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class NetworkConnectionManager : MonoBehaviour
{
    [SerializeField] private GameObject runnerPrefab;

    private NetworkRunner runner;
    private PlayerManager playerManager; // ← 이 줄 추가

    public NetworkRunner Runner => runner;
    public bool IsConnected => runner != null && runner.IsRunning;

    public event System.Action<string> OnRoomJoined;
    public event System.Action<string> OnRoomCreated;
    public event System.Action<string> OnConnectionFailed;
    // ← 여기에 아래 메서드 추가
    public void SetPlayerManager(PlayerManager manager)
    {
        playerManager = manager;
        Debug.Log("PlayerManager가 NetworkConnectionManager에 설정되었습니다.");
    }
    public async Task<bool> JoinExistingRoom(GameMode gameMode, int maxPlayers)
    {
        Debug.Log("WAITING 상태인 방 검색 중...");

        CreateRunner();

        var joinArgs = new StartGameArgs
        {
            GameMode = gameMode,
            SessionName = null,
            PlayerCount = maxPlayers,
            SceneManager = runner.GetComponent<INetworkSceneManager>(),
            SessionProperties = new Dictionary<string, SessionProperty>
            {
                { "GameState", "WAITING" }
            },
        };

        var joinResult = runner.StartGame(joinArgs);
        await WaitForTask(joinResult);

        if (joinResult.Result.Ok)
        {
            string roomName = runner.SessionInfo.Name;
            Debug.Log($"기존 방 참가 성공: {roomName}");
            OnRoomJoined?.Invoke(roomName);
            return true;
        }

        Debug.LogWarning($"방 참가 실패: {joinResult.Result.ShutdownReason}");
        OnConnectionFailed?.Invoke(joinResult.Result.ShutdownReason.ToString());
        return false;
    }

    public async Task<bool> CreateNewRoom(GameMode gameMode, int maxPlayers)
    {
        string roomName = "Room_" + UnityEngine.Random.Range(1000, 9999);
        Debug.Log($"새 방 생성: {roomName}");

        CleanupRunner();
        await Task.Delay(500);

        CreateRunner();

        var createArgs = new StartGameArgs
        {
            GameMode = gameMode,
            SessionName = roomName,
            PlayerCount = maxPlayers,
            SceneManager = runner.GetComponent<INetworkSceneManager>(),
            SessionProperties = new Dictionary<string, SessionProperty>
            {
                { "GameState", "WAITING" }
            }
        };

        var createResult = runner.StartGame(createArgs);
        await WaitForTask(createResult);

        if (createResult.Result.Ok)
        {
            Debug.Log($"방 생성 성공: {roomName}");
            OnRoomCreated?.Invoke(roomName);
            return true;
        }

        Debug.LogError($"방 생성 실패: {createResult.Result.ShutdownReason}");
        OnConnectionFailed?.Invoke(createResult.Result.ShutdownReason.ToString());
        return false;
    }

    public void UpdateRoomState(string newState)
    {
        if (!IsConnected) return;

        try
        {
            var sessionProperties = new Dictionary<string, SessionProperty>()
            {
                { "GameState", newState }
            };

            runner.SessionInfo.UpdateCustomProperties(sessionProperties);
            Debug.Log($"방 상태 변경: {newState}");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"방 상태 업데이트 실패: {e.Message}");
        }
    }

    public void Disconnect()
    {
        CleanupRunner();
    }

    private void CreateRunner()
    {
        if (runner == null)
        {
            GameObject runnerObject = Instantiate(runnerPrefab);
            runner = runnerObject.GetComponent<NetworkRunner>();

            if (runner == null)
            {
                Debug.LogError("NetworkRunner 컴포넌트를 찾을 수 없습니다!");
            }

            // ← 이 부분 추가
            if (playerManager != null)
            {
                playerManager.RegisterToRunner(runner);
            }
        }
    }

    private void CleanupRunner()
    {
        // ← 이 부분 추가
        if (playerManager != null)
        {
            playerManager.UnregisterFromRunner(runner);
        }

        if (runner != null)
        {
            runner.Shutdown();
            runner = null;
        }
    }

    private async Task WaitForTask(Task<StartGameResult> task)
    {
        while (!task.IsCompleted)
        {
            await Task.Yield();
        }
    }
}
