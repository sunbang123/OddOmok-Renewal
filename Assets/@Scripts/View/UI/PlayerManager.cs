using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerManager : NetworkBehaviour, INetworkRunnerCallbacks
{
    private NetworkConnectionManager connectionManager;
    private int maxPlayersPerRoom;

    public event System.Action OnPlayerCountChanged;
    public event System.Action OnRoomFull;
    public event System.Action OnPlayerKicked;

    public int CurrentPlayerCount => connectionManager.IsConnected ?
        connectionManager.Runner.ActivePlayers.Count() : 0;

    private void Start()
    {
        // NetworkRunner에 PlayerManager 등록 (백업용)
        if (connectionManager != null && connectionManager.Runner != null)
        {
            connectionManager.Runner.AddCallbacks(this);
        }
    }

    private void OnDestroy()
    {
        // NetworkRunner에서 PlayerManager 해제 (백업용)
        if (connectionManager != null && connectionManager.Runner != null)
        {
            connectionManager.Runner.RemoveCallbacks(this);
        }
    }

    public void Initialize(NetworkConnectionManager connManager, int maxPlayers)
    {
        connectionManager = connManager;
        maxPlayersPerRoom = maxPlayers;
    }

    // ← 여기에 아래 메서드들 추가
    public void RegisterToRunner(NetworkRunner runner)
    {
        if (runner != null)
        {
            runner.AddCallbacks(this);
            Debug.Log("PlayerManager가 NetworkRunner에 등록되었습니다.");
        }
    }

    public void UnregisterFromRunner(NetworkRunner runner)
    {
        if (runner != null)
        {
            runner.RemoveCallbacks(this);
            Debug.Log("PlayerManager가 NetworkRunner에서 해제되었습니다.");
        }
    }

    private void CheckPlayerCount()
    {
        if (!connectionManager.IsConnected) return;

        int currentCount = CurrentPlayerCount;
        Debug.Log($"현재 플레이어 수: {currentCount}/{maxPlayersPerRoom}");

        if (currentCount > maxPlayersPerRoom)
        {
            HandleExcessPlayers();
        }
        else if (currentCount >= maxPlayersPerRoom)
        {
            OnRoomFull?.Invoke();
        }
    }

    private void HandleExcessPlayers()
    {
        var runner = connectionManager.Runner;
        if (runner == null) return;

        var allPlayers = runner.ActivePlayers.ToList();
        int playersToKick = allPlayers.Count - maxPlayersPerRoom;

        Debug.Log($"최대 인원 초과! {playersToKick}명 제거 필요");

        var sortedPlayers = allPlayers.OrderByDescending(p => p.PlayerId).Take(playersToKick);

        foreach (var playerToKick in sortedPlayers)
        {
            if (runner.LocalPlayer == playerToKick)
            {
                Debug.Log("로컬 플레이어가 추방됩니다.");
                OnPlayerKicked?.Invoke();
                return;
            }
        }

        StartCoroutine(DelayedPlayerCountCheck());
    }

    private System.Collections.IEnumerator DelayedPlayerCountCheck()
    {
        yield return new WaitForSeconds(0.5f);

        if (connectionManager.IsConnected && CurrentPlayerCount >= maxPlayersPerRoom)
        {
            OnRoomFull?.Invoke();
        }
    }

    public string GetPlayersInfo()
    {
        if (!connectionManager.IsConnected) return "";

        string info = "";
        foreach (var player in connectionManager.Runner.ActivePlayers)
        {
            info += $"Client ID: {player}\n";
        }
        return info;
    }
    // INetworkRunnerCallbacks 인터페이스 구현
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"플레이어 참가: {player}");
        OnPlayerCountChanged?.Invoke();
        CheckPlayerCount();
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"플레이어 퇴장: {player}");
        OnPlayerCountChanged?.Invoke();
    }

    // 필수 구현 메서드들 (정확한 시그니처)
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, System.ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
}