// 2025-08-05 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using Fusion;
using System;
using UnityEditor;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class CameraRaycastSpawner : NetworkBehaviour
{
    public NetworkPrefabRef prefabToInstantiate; // 생성할 프리팹
    private CameraRaycast cameraRaycast;

    void Start()
    {
        // 메인 카메라에서 CameraRaycast 컴포넌트 찾기
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            cameraRaycast = mainCamera.GetComponent<CameraRaycast>();
        }

        if (cameraRaycast == null)
        {
            Debug.LogError("CameraRaycast component is missing on the Main Camera.");
        }
    }

    void OnEnable()
    {
        // 메인 카메라에서 CameraRaycast 컴포넌트 찾기
        if (cameraRaycast == null)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("Main Camera is missing or does not have the 'MainCamera' tag.");
                return;
            }

            cameraRaycast = mainCamera.GetComponent<CameraRaycast>();
            if (cameraRaycast == null)
            {
                Debug.LogError("CameraRaycast component is missing on the Main Camera.");
                return;
            }
        }

        // CameraRaycast의 OnRaycastHit 이벤트 구독
        cameraRaycast.OnRaycastHit += HandleRaycastHit;
    }

    void OnDisable()
    {
        // cameraRaycast가 null인지 확인 후 이벤트 구독 해제
        if (cameraRaycast != null)
        {
            cameraRaycast.OnRaycastHit -= HandleRaycastHit;
        }
    }


    private void HandleRaycastHit(RaycastHit hit)
    {
        UnityEngine.Transform hitTransform = hit.transform;
        // Prefab 생성
        NetworkObject spawnedPrefab = Runner.Spawn(prefabToInstantiate, hitTransform.position, Quaternion.identity, Runner.LocalPlayer);

        // Raycast로 충돌한 오브젝트의 LayerMask를 "Stone"으로 변경
        hitTransform.gameObject.layer = LayerMask.NameToLayer("Stone");
    }
}
