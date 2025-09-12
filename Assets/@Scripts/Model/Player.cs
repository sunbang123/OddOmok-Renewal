// 2025-08-04 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using System;
using UnityEditor;
using UnityEngine;

public enum PlayerState
{
    Idle,
    CircleCheck,
    SkillCheck
}

public class Player : MonoBehaviour
{
    private PlayerState currentState = PlayerState.Idle;

    public void SetState(PlayerState newState)
    {
        currentState = newState;
        Debug.Log($"Player state changed to: {currentState}");
    }

    public PlayerState GetState()
    {
        return currentState;
    }
}
