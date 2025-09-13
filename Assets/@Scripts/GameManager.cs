using Fusion;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Unity.Collections.Unicode;

public class GameManager : NetworkBehaviour
{
    [SerializeField]
    private Canvas _canvas;
    [SerializeField]
    private Text _text;

    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("GameManager");
                    _instance = obj.AddComponent<GameManager>();
                }
            }
            return _instance;
        }
    }

    // 승리 조건이 충족되었을 때 호출되는 함수 (로컬)
    public void OnGameWin(TriggerHandler.Direction direction)
    {
        Debug.Log($"Game won in direction: {direction}");
        // RPC로 모든 클라이언트에게 게임 종료 알림
        RPC_NotifyGameResult(Runner.LocalPlayer, direction, true);
    }

    // 게임 결과를 모든 클라이언트에게 알리는 RPC
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_NotifyGameResult(PlayerRef winner, TriggerHandler.Direction direction, bool isWin)
    {
        bool isLocalPlayerWinner = Runner.LocalPlayer == winner;
        _canvas.gameObject.SetActive(true);

        if (isLocalPlayerWinner)
        {
            _text.text = $"You Win! Direction: {direction}";
            Debug.Log($"Local player won in direction: {direction}");
        }
        else
        {
            _text.text = $"You Lose! Direction: {direction}";
            Debug.Log($"Local player lost. Winner direction: {direction}");
        }

        // 3초 후 게임 종료 및 씬 전환
        StartCoroutine(EndGameSequence());
    }

    // 게임 종료 시퀀스
    private IEnumerator EndGameSequence()
    {
        // 3초 대기 (결과 화면 표시)
        yield return new WaitForSeconds(3f);

        Debug.Log("Starting game end sequence...");

        // 네트워크 연결 해제
        DisconnectFromNetwork();

        // 잠시 대기 후 씬 전환
        yield return new WaitForSeconds(0.5f);

        // 메인 메뉴나 로비 씬으로 전환 (씬 이름을 적절히 변경하세요)
        SceneManager.LoadScene("MainMenu"); // 또는 "Lobby", "StartScene" 등
    }

    // 네트워크 연결 해제 함수
    private void DisconnectFromNetwork()
    {
        if (Runner != null)
        {
            Debug.Log("Disconnecting from network...");

            // Fusion 네트워크 러너 종료
            Runner.Shutdown();

            // 또는 다음과 같이 할 수도 있습니다:
            // NetworkRunner.Destroy(Runner);
        }
        else
        {
            Debug.LogWarning("NetworkRunner is null, cannot disconnect");
        }
    }

    // 선택사항: 즉시 게임 종료를 위한 함수 (ESC 키 등으로 호출 가능)
    public void ForceQuitGame()
    {
        Debug.Log("Force quitting game...");
        DisconnectFromNetwork();
        SceneManager.UnloadSceneAsync("GameScene");
    }
}