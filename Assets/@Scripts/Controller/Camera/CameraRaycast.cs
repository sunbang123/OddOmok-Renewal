using DG.Tweening.Core.Easing;
using UnityEngine;
using Fusion;

public class CameraRaycast : MonoBehaviour
{
    public RaycastHit? LastHit { get; private set; } // 마지막 Raycast 결과저장
    public delegate void RaycastHitEventHandler(RaycastHit hit);
    public event RaycastHitEventHandler OnRaycastHit;

    public delegate void NetworkSkillEffectHandler(RaycastHit hit);
    public event NetworkSkillEffectHandler OnSkillHit;


    private Camera mainCamera;

    void Start()
    {
        // Camera.main 대신 캐시된 참조 사용 (성능 향상)
        mainCamera = Camera.main;
        if (mainCamera == null)
            mainCamera = FindObjectOfType<Camera>();
    }

    void Update()
    {
        bool inputDetected = false;
        Vector3 inputPosition = Vector3.zero;

        // 플랫폼별 입력 처리
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
        // PC/Mac/Editor 환경: 마우스 입력
        if (Input.GetMouseButtonDown(0))
        {
            inputDetected = true;
            inputPosition = Input.mousePosition;
        }
#elif UNITY_ANDROID || UNITY_IOS
        // 모바일 환경: 터치 입력
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            inputDetected = true;
            inputPosition = Input.GetTouch(0).position;
        }
#else
        // 기타 플랫폼: 마우스와 터치 모두 지원
        if (Input.GetMouseButtonDown(0))
        {
            inputDetected = true;
            inputPosition = Input.mousePosition;
        }
        else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            inputDetected = true;
            inputPosition = Input.GetTouch(0).position;
        }
#endif

        if (inputDetected)
        {
            PerformRaycast(inputPosition);
        }
    }

    private void PerformRaycast(Vector3 screenPosition)
    {
        if (mainCamera == null)
        {
            Debug.LogWarning("Main camera is null!");
            return;
        }

        Ray ray = mainCamera.ScreenPointToRay(screenPosition);
        Debug.DrawRay(mainCamera.transform.position, ray.direction * 150.0f, Color.red, 1.0f);

        LayerMask mask = LayerMask.GetMask("Spawn") | LayerMask.GetMask("Board") | LayerMask.GetMask("Enemy");
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 150.0f, mask))
        {
            LastHit = hit;
            Debug.Log($"Raycast Camera {hit.collider.gameObject.name}");

            if (hit.collider.gameObject.layer == 7)
            {
                Debug.Log("Hit a Spawn object!");
                // Stone 레이어에 대한 추가 처리 로직
                return;
            }

            if (hit.collider.gameObject.layer == 8)
            {
                Debug.Log("Hit a Enemy object!");
                return;
            }

            OnRaycastHit?.Invoke(hit);
        }
        else
        {
            LastHit = null;
            Debug.Log("Raycast Camera No Hit");
        }
    }
}