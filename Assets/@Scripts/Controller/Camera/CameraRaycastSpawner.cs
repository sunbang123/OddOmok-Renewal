// 2025-08-05 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using Fusion;
using System;
using UnityEditor;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class CameraRaycastSpawner : NetworkBehaviour
{
    [Header("플레이어별 프리팹")]
    [SerializeField]
    private NetworkPrefabRef[] playerPrefabs; // 플레이어별 프리팹

    [SerializeField]
    private NetworkPrefabRef prefabToInstantiate; // 생성할 프리팹
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

    private NetworkPrefabRef GetPlayerPrefab()
    {
        if (Runner != null)
        {
            // PlayerRef 값을 인덱스로 사용
            int playerIndex = Runner.LocalPlayer.PlayerId % playerPrefabs.Length;
            return playerPrefabs[playerIndex];
        }

        return prefabToInstantiate;
    }
    private void HandleRaycastHit(RaycastHit hit)
    {
        if (!TurnChecker())
            return;

        NetworkPrefabRef prefabToUse = GetPlayerPrefab();

        UnityEngine.Transform hitTransform = hit.transform;
        // Prefab 생성
        NetworkObject spawnedPrefab = Runner.Spawn(prefabToUse, hitTransform.position, Quaternion.identity, Runner.LocalPlayer);
        spawnedPrefab.tag = "Player";

        // Raycast로 충돌한 오브젝트의 LayerMask를 "Stone"으로 변경
        hitTransform.gameObject.layer = 7;
    }

    private bool TurnChecker()
    {
        if (Runner.LocalPlayer.PlayerId == 1 && NetworkObjectSpawnHandler.SpawnedTeamCount > NetworkObjectSpawnHandler.SpawnedEnemyCount)
            return false;

        if (Runner.LocalPlayer.PlayerId == 2 && NetworkObjectSpawnHandler.SpawnedTeamCount == NetworkObjectSpawnHandler.SpawnedEnemyCount)
            return false;

        return true;
    }
}
