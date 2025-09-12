// 2025-09-07 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using System;
using UnityEditor;
using UnityEngine;

public class ChangeLayerOnTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") == true)
            return;

        // other는 Object2를 가리킴
        Debug.Log($"{gameObject}: {other}가 들어왔다!");

        // Layer 이름을 "Enemy"로 변경
        gameObject.layer = 8;
    }
}
