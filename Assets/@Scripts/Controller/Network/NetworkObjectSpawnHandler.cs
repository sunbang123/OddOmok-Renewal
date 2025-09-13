using Fusion;
using UnityEngine;

public class NetworkObjectSpawnHandler : NetworkBehaviour
{
    public static int SpawnedTeamCount = 0; // 생성된 네트워크 객체 수를 추적하는 정적 변수
    public static int SpawnedEnemyCount = -1; // 생성된 네트워크 객체 수를 추적하는 정적 변수

    public override void Spawned()
    {
        base.Spawned();
        Debug.Log($"Network object spawned! Object: {gameObject.name}, Owner: {Object.InputAuthority}");

        // 삼항 연산자를 사용한 간결한 구현
        bool isLocalPlayer = Object.HasInputAuthority;

        // 카운트 업데이트 및 로그
        if (isLocalPlayer) SpawnedTeamCount++; else SpawnedEnemyCount++;
        Debug.Log($"This object was spawned by the {(isLocalPlayer ? "local" : "remote")} player.");

        // 레이어 변경
        ChangeLayerByRaycast(isLocalPlayer ? "Spawn" : "Enemy");
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        base.Despawned(runner, hasState);
        Debug.Log($"Network object despawned! Object: {gameObject.name}");
    }

    private void ChangeLayerByRaycast(string targetLayerName)
    {
        // Raycast 실행
        bool raycastHit = Physics.Raycast(transform.position, Vector3.up * 2, out RaycastHit hit, 10f);
        int layerIndex = LayerMask.NameToLayer(targetLayerName);
        bool validLayer = layerIndex != -1;

        // 결과 처리
        Debug.Log(raycastHit
            ? $"Raycast hit object: {hit.collider.gameObject.name}"
            : "Raycast did not hit any object.");

        if (raycastHit && validLayer)
        {
            hit.collider.gameObject.layer = layerIndex;
            Debug.Log($"Changed layer of {hit.collider.gameObject.name} to {targetLayerName}");
        }
        else if (raycastHit && !validLayer)
        {
            Debug.LogWarning($"Layer '{targetLayerName}' does not exist!");
        }
    }
}