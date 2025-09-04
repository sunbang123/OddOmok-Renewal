// 2025-08-04 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchmakingManager : MonoBehaviour
{
    [SerializeField]
    private Camera _menucamera;

    public static MatchmakingManager Instance { get; private set; }

    public GameObject matchingPanelPrefab; // 매칭 UI 패널 Prefab
    private GameObject matchingPanelInstance;

    private bool isMatchmakingInProgress = false; // 매칭 중인지 확인하는 플래그

    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 이미 인스턴스가 존재하면 파괴
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않도록 설정
    }

    public void StartMatchmaking()
    {
        // 매칭 중복 실행 방지
        if (isMatchmakingInProgress)
        {
            Debug.Log("매칭이 이미 진행 중입니다!");
            return;
        }

        isMatchmakingInProgress = true; // 매칭 시작 상태로 설정

        // 매칭 패널 생성 및 활성화
        if (matchingPanelPrefab != null)
        {
            matchingPanelInstance = Instantiate(matchingPanelPrefab, FindObjectOfType<Canvas>().transform);
            matchingPanelInstance.SetActive(true);

            // MatchCancelButton의 클릭 이벤트 등록
            var cancelButton = matchingPanelInstance.transform.Find("MatchCancelButton");
            if (cancelButton != null)
            {
                cancelButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(CancelMatchmaking);
            }
        }

        // 5초 후 매칭 완료 처리
        Invoke(nameof(OnMatchmakingComplete), 5f);
    }

    private void CancelMatchmaking()
    {
        // 매칭 취소 처리
        if (!isMatchmakingInProgress)
        {
            Debug.Log("매칭이 진행 중이 아닙니다!");
            return;
        }

        Debug.Log("매칭이 취소되었습니다.");
        isMatchmakingInProgress = false;

        // 매칭 패널 제거
        if (matchingPanelInstance != null)
        {
            Destroy(matchingPanelInstance);
        }

        // 매칭 완료 호출 예약 취소
        CancelInvoke(nameof(OnMatchmakingComplete));
    }

    private void OnMatchmakingComplete()
    {
        // 매칭 완료 상태 처리
        if (!isMatchmakingInProgress) return; // 매칭이 취소된 경우 실행하지 않음

        Debug.Log("매칭 완료!");

        // 매칭 상태 초기화
        isMatchmakingInProgress = false;

        Destroy(matchingPanelInstance);
        // Game Scene으로 이동
        SceneManager.LoadScene("Game", LoadSceneMode.Additive);
        _menucamera.enabled = false;
    }
}
