using DG.Tweening.Core.Easing;
using UnityEngine;

public class CameraRaycast : MonoBehaviour
{
    public RaycastHit? LastHit { get; private set; } // 마지막 Raycast 결과저장

    public delegate void RaycastHitEventHandler(RaycastHit hit);
    public event RaycastHitEventHandler OnRaycastHit;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            Debug.DrawRay(Camera.main.transform.position, ray.direction * 150.0f, Color.red, 1.0f);

            LayerMask mask = LayerMask.GetMask("Stone") | LayerMask.GetMask("Board");

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 150.0f, mask))
            {
                LastHit = hit;
                Debug.Log($"Raycast Camera {hit.collider.gameObject.name}");

                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Stone"))
                {
                    Debug.Log("Hit a Stone object!");
                    // Stone 레이어에 대한 추가 처리 로직
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
}