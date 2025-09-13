using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class TriggerHandler : MonoBehaviour
{
    private const int WIN_COUNT = 5;

    [Header("Raycast Settings")]
    public float raycastDistance = 1f;
    public LayerMask targetLayerMask = -1; // 모든 레이어

    [Header("Debug Settings")]
    public float debugRayDuration = 2f;

    public enum Direction
    {
        Horizontal,      // 가로 (←→)
        Vertical,        // 세로 (↑↓)
        DiagonalMain,    // 주대각선 (↙↗)  
        DiagonalAnti     // 반대각선 (↖↘)
    }

    public static readonly Vector3[] directionVectors = new Vector3[]
    {
        Vector3.right,                      // Horizontal
        Vector3.forward,                    // Vertical
        new Vector3(1, 0, 1).normalized,   // DiagonalMain
        new Vector3(-1, 0, 1).normalized   // DiagonalAnti
    };

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") == true)
        {
            WinChecker();
            return;
        }

        // other는 Object2를 가리킴
        Debug.Log($"{gameObject}: {other}가 들어왔다!");

        // Layer 이름을 "Object"로 변경
        gameObject.layer = 9;
    }

    private void WinChecker()
    {
        Debug.Log("Player가 들어왔다!");
        // 모든 방향 체크 (비동기가 아닌 순차적으로 처리)
        CheckAllDirectionsCombined();
    }

    private void CheckAllDirectionsCombined()
    {
        Debug.Log("=== 양방향 통합 연속성 체크 ===");

        for (int i = 0; i < directionVectors.Length; i++)
        {
            Direction dir = (Direction)i;
            Vector3 direction = directionVectors[i];

            // 양방향을 통합해서 체크
            int totalConsecutive = CheckBidirectionalConsecutive(dir, direction);

            if (totalConsecutive >= WIN_COUNT)
            {
                Debug.Log($"🎉 승리! {dir} 방향에서 총 {totalConsecutive}개 연속!");
                OnWinConditionMet(dir, direction);

                // 승리한 방향 시각화
                VisualizeBidirectionalWin(direction);
            }
        }
    }

    private int CheckBidirectionalConsecutive(Direction dirType, Vector3 direction)
    {
        Vector3 rayOrigin = transform.position;
        Color rayColor = GetDirectionColor(dirType);

        // 정방향 체크
        List<RaycastHit> forwardHits = GetConsecutiveHits(rayOrigin, direction, rayColor);

        // 역방향 체크
        List<RaycastHit> backwardHits = GetConsecutiveHits(rayOrigin, -direction, rayColor);

        // 현재 위치도 포함 (자기 자신)
        int totalCount = 1; // 자기 자신
        totalCount += forwardHits.Count;
        totalCount += backwardHits.Count;

        Debug.Log($"[{dirType}] 정방향: {forwardHits.Count}개, 역방향: {backwardHits.Count}개, 총: {totalCount}개");

        return totalCount;
    }

    private List<RaycastHit> GetConsecutiveHits(Vector3 origin, Vector3 direction, Color debugColor)
    {
        List<RaycastHit> consecutiveHits = new List<RaycastHit>();

        // Debug Ray 그리기
        Debug.DrawRay(origin, direction * raycastDistance, debugColor, debugRayDuration);

        // 모든 충돌 객체 가져오기
        RaycastHit[] hits = Physics.RaycastAll(origin, direction, raycastDistance, targetLayerMask);

        if (hits.Length > 0)
        {
            Debug.Log($"{hits}");
            // 거리 순으로 정렬
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            // 연속된 "Player" 태그만 수집
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.CompareTag("Player"))
                {
                    consecutiveHits.Add(hit);
                }
                else
                {
                    // 다른 태그를 만나면 연속성 중단
                    break;
                }
            }

            // Hit 지점 시각화
            DrawHitDebugRays(hits, debugColor);
        }

        return consecutiveHits;
    }

    private void DrawHitDebugRays(RaycastHit[] hits, Color baseColor)
    {
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];

            // Hit 지점에서 위아래로 작은 선 그리기
            Vector3 hitPoint = hit.point;
            Vector3 upDirection = Vector3.up * 0.2f;

            // Hit된 순서에 따라 색상 변화
            Color hitColor = Color.Lerp(baseColor, Color.white, i * 0.3f);

            Debug.DrawRay(hitPoint - upDirection, upDirection * 2, hitColor, debugRayDuration);
        }
    }

    private void VisualizeBidirectionalWin(Vector3 direction)
    {
        Vector3 origin = transform.position;

        // 승리 라인을 더 굵고 밝게 표시
        Debug.DrawRay(origin, direction * raycastDistance, Color.white, debugRayDuration * 3);
        Debug.DrawRay(origin, -direction * raycastDistance, Color.white, debugRayDuration * 3);

        // 승리 위치에 십자 표시
        Debug.DrawRay(origin - Vector3.up * 0.5f, Vector3.up * 1f, Color.magenta, debugRayDuration * 3);
        Debug.DrawRay(origin - Vector3.right * 0.5f, Vector3.right * 1f, Color.magenta, debugRayDuration * 3);
    }

    private void OnWinConditionMet(Direction direction, Vector3 directionVector)
    {
        Debug.Log($"=== 승리 확정! ===");
        Debug.Log($"승리 방향: {direction}");
        Debug.Log($"승리 벡터: {directionVector}");

        // 여기에 승리 처리 로직 추가
        // 예: 이펙트 재생, UI 표시, 게임 종료 등

        // 게임 매니저에 승리 알림 (예시)
        GameManager.Instance?.OnGameWin(direction);
    }

    private Color GetDirectionColor(Direction dir)
    {
        switch (dir)
        {
            case Direction.Horizontal: return Color.red;
            case Direction.Vertical: return Color.blue;
            case Direction.DiagonalMain: return Color.green;
            case Direction.DiagonalAnti: return Color.yellow;
            default: return Color.white;
        }
    }
}