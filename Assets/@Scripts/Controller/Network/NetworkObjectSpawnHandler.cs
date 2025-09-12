using Fusion;
using UnityEngine;

public class NetworkObjectSpawnHandler : NetworkBehaviour
{
    public static int SpawnedTeamCount = 0; // 생성된 네트워크 객체 수를 추적하는 정적 변수
    public static int SpawnedEnemyCount = -1; // 생성된 네트워크 객체 수를 추적하는 정적 변수
    public override void Spawned()
    {
        base.Spawned();

        // 네트워크 객체가 생성된 순간 실행되는 코드
        Debug.Log($"Network object spawned! Object: {gameObject.name}, Owner: {Object.InputAuthority}");

        // 특정 로직 실행 (예: 생성된 객체의 초기화)
        if (Object.HasInputAuthority)
        {
            Debug.Log("This object was spawned by the local player.");
            SpawnedTeamCount++;
        }
        else
        {
            Debug.Log("This object was spawned by a remote player.");
            SpawnedEnemyCount++;
        }
    }
}