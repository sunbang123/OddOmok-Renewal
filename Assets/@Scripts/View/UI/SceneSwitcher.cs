// 2025-08-04 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using UnityEngine;
using UnityEngine.EventSystems; // UI 클릭 이벤트를 처리하기 위해 필요
using UnityEngine.SceneManagement; // 씬 전환을 위해 필요

public class SceneSwitcher : MonoBehaviour, IPointerClickHandler
{
    // UI 요소를 클릭했을 때 호출되는 메서드
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("StartPanel clicked!");
        // CustomSceneManager를 사용하여 "Robby" 씬으로 전환
        CustomSceneManager.Instance.LoadScene("Lobby");
    }
}