// 2025-08-04 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TitleSceneManager : MonoBehaviour
{
    [Header("UI Prefabs")]
    public GameObject loadingPanelPrefab; // LoadingPanel 프리팹
    public GameObject startPanelPrefab;   // StartPanel 프리팹

    private GameObject loadingPanelInstance; // 동적으로 생성된 LoadingPanel 인스턴스
    private GameObject startPanelInstance;   // 동적으로 생성된 StartPanel 인스턴스
    private Slider loadingSlider;            // Slider 컴포넌트
    private Text loadingText;                // 로딩 퍼센트 텍스트 (선택 사항)

    [Header("Loading Settings")]
    public float loadingDuration = 2f;       // 로딩 시간 (초)
    public AnimationCurve loadingCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f); // 로딩 진행 애니메이션 곡선

    void Start()
    {
        // Canvas 찾기 또는 생성
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            // Canvas 동적 생성
            GameObject canvasObject = new GameObject("Canvas");
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
        }

        // StartPanel 동적 생성
        if (startPanelPrefab != null)
        {
            startPanelInstance = Instantiate(startPanelPrefab, canvas.transform);
            startPanelInstance.SetActive(false); // 로딩 중에는 비활성화
        }
        else
        {
            Debug.LogError("StartPanel Prefab is not assigned!");
        }

        // LoadingPanel 동적 생성
        if (loadingPanelPrefab != null)
        {
            loadingPanelInstance = Instantiate(loadingPanelPrefab, canvas.transform);
            loadingSlider = loadingPanelInstance.GetComponentInChildren<Slider>();
            loadingText = loadingPanelInstance.GetComponentInChildren<Text>();

            // RectTransform 초기화
            RectTransform rectTransform = loadingPanelInstance.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = Vector2.zero; // 중앙에 배치
                rectTransform.offsetMin = Vector2.zero;       // 좌측 하단 오프셋 초기화
                rectTransform.offsetMax = Vector2.zero;       // 우측 상단 오프셋 초기화
            }

            // 로딩 시작
            StartCoroutine(LoadingSequence());
        }
        else
        {
            Debug.LogError("LoadingPanel Prefab is not assigned!");
        }
    }

    private IEnumerator LoadingSequence()
    {
        float progress = 0f;
        float elapsedTime = 0f;

        // 슬라이더 값 증가
        while (progress < 1f)
        {
            elapsedTime += Time.deltaTime;
            progress = Mathf.Clamp01(elapsedTime / loadingDuration);

            // 애니메이션 곡선을 사용하여 부드러운 진행
            float curvedProgress = loadingCurve.Evaluate(progress);

            if (loadingSlider != null)
            {
                loadingSlider.value = curvedProgress; // 슬라이더 업데이트
            }

            if (loadingText != null)
            {
                loadingText.text = $"{(curvedProgress * 100):0}%"; // 텍스트 업데이트
            }

            yield return null;
        }

        // 로딩 완료 후 처리
        if (loadingPanelInstance != null)
        {
            Destroy(loadingPanelInstance); // LoadingPanel 제거
        }

        if (startPanelInstance != null)
        {
            startPanelInstance.SetActive(true); // StartPanel 활성화
        }
    }
}
