using UnityEngine;

public class TriggerEvent : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("TriggerEvent activated by " + other.name);
        gameObject.SetActive(false);
    }
}
