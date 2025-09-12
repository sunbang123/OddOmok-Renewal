using Fusion;
using UnityEngine;

public class MatchmakingManager : MonoBehaviour
{
    [Header("Network Settings")]
    [SerializeField] private GameObject runnerPrefab;
    [SerializeField] private int maxPlayersPerRoom = 2;

    [Header("Managers")]
    [SerializeField] private NetworkConnectionManager connectionManager;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private MatchmakingUIManager uiManager;
    [SerializeField] private GameSceneManager sceneManager;

    private bool isMatchmakingInProgress = false;

    public static MatchmakingManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeManagers();
    }

    private void InitializeManagers()
    {
        // 매니저들 초기화
        if (connectionManager == null)
            connectionManager = GetComponent<NetworkConnectionManager>();
        if (playerManager == null)
            playerManager = GetComponent<PlayerManager>();
        if (uiManager == null)
            uiManager = GetComponent<MatchmakingUIManager>();
        if (sceneManager == null)
            sceneManager = GetComponent<GameSceneManager>();

        // PlayerManager 초기화 및 ConnectionManager에 등록
        playerManager.Initialize(connectionManager, maxPlayersPerRoom);
        connectionManager.SetPlayerManager(playerManager);

        // 이벤트 연결
        SetupEventHandlers();
    }

    private void SetupEventHandlers()
    {
        // Connection Manager 이벤트
        connectionManager.OnRoomJoined += OnRoomJoined;
        connectionManager.OnRoomCreated += OnRoomCreated;
        connectionManager.OnConnectionFailed += OnConnectionFailed;

        // Player Manager 이벤트
        playerManager.OnPlayerCountChanged += OnPlayerCountChanged;
        playerManager.OnRoomFull += OnMatchmakingComplete;
        playerManager.OnPlayerKicked += OnPlayerKicked;

        // UI Manager 이벤트
        uiManager.OnMatchmakingCancelled += CancelMatchmaking;
    }

    public async void StartMatchmaking()
    {
        if (isMatchmakingInProgress)
        {
            Debug.Log("매칭이 이미 진행 중입니다!");
            return;
        }

        isMatchmakingInProgress = true;
        uiManager.ShowMatchingPanel();

        Debug.Log("매칭 시작");

        // 기존 방 참가 시도
        bool joinSuccess = await connectionManager.JoinExistingRoom(GameMode.Shared, maxPlayersPerRoom);

        if (!joinSuccess)
        {
            // 새 방 생성
            bool createSuccess = await connectionManager.CreateNewRoom(GameMode.Shared, maxPlayersPerRoom);

            if (!createSuccess)
            {
                OnMatchmakingFailed();
            }
        }
    }

    public void CancelMatchmaking()
    {
        if (!isMatchmakingInProgress) return;

        Debug.Log("매칭 취소");
        isMatchmakingInProgress = false;

        uiManager.HideMatchingPanel();
        uiManager.HideRoomInfo();
        connectionManager.Disconnect();
    }

    private void OnRoomJoined(string roomName)
    {
        uiManager.ShowRoomInfo(roomName);
        uiManager.UpdateClientInfo(playerManager.GetPlayersInfo());
    }

    private void OnRoomCreated(string roomName)
    {
        uiManager.ShowRoomInfo(roomName);
        uiManager.UpdateClientInfo(playerManager.GetPlayersInfo());
    }

    private void OnConnectionFailed(string reason)
    {
        Debug.LogError($"연결 실패: {reason}");
        OnMatchmakingFailed();
    }

    private void OnPlayerCountChanged()
    {
        uiManager.UpdateClientInfo(playerManager.GetPlayersInfo());
    }

    private void OnMatchmakingComplete()
    {
        if (!isMatchmakingInProgress) return;

        Debug.Log("매칭 완료!");

        connectionManager.UpdateRoomState("PLAYING");
        isMatchmakingInProgress = false;

        uiManager.HideMatchingPanel();
        sceneManager.LoadGameScene(connectionManager.Runner);
    }

    private void OnPlayerKicked()
    {
        Debug.Log("방이 가득 찼습니다. 다시 매칭을 시도해주세요.");

        isMatchmakingInProgress = false;
        uiManager.HideMatchingPanel();
        connectionManager.Disconnect();
    }

    private void OnMatchmakingFailed()
    {
        isMatchmakingInProgress = false;
        uiManager.HideMatchingPanel();
    }

    private void OnDestroy()
    {
        // 이벤트 해제
        if (connectionManager != null)
        {
            connectionManager.OnRoomJoined -= OnRoomJoined;
            connectionManager.OnRoomCreated -= OnRoomCreated;
            connectionManager.OnConnectionFailed -= OnConnectionFailed;
        }

        if (playerManager != null)
        {
            playerManager.OnPlayerCountChanged -= OnPlayerCountChanged;
            playerManager.OnRoomFull -= OnMatchmakingComplete;
            playerManager.OnPlayerKicked -= OnPlayerKicked;
        }

        if (uiManager != null)
        {
            uiManager.OnMatchmakingCancelled -= CancelMatchmaking;
        }
    }
}