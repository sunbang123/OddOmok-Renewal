using UnityEngine;
using UnityEngine.UI;

public class MatchmakingUIManager : MonoBehaviour
{
    [SerializeField] private GameObject textPanel;
    [SerializeField] private Text roomInfoText;
    [SerializeField] private Text clientInfoText;
    [SerializeField] private GameObject matchingPanelPrefab;

    private GameObject matchingPanelInstance;

    public event System.Action OnMatchmakingCancelled;

    public void ShowMatchingPanel()
    {
        if (matchingPanelPrefab != null)
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                matchingPanelInstance = Instantiate(matchingPanelPrefab, canvas.transform);
                matchingPanelInstance.SetActive(true);

                var cancelButton = matchingPanelInstance.transform.Find("MatchCancelButton");
                if (cancelButton != null)
                {
                    cancelButton.GetComponent<Button>().onClick.AddListener(CancelMatching);
                }
            }
        }
    }

    public void HideMatchingPanel()
    {
        if (matchingPanelInstance != null)
        {
            Destroy(matchingPanelInstance);
            matchingPanelInstance = null;
        }
    }

    public void ShowRoomInfo(string roomName)
    {
        if (textPanel != null)
        {
            textPanel.SetActive(true);

            if (roomInfoText != null)
            {
                roomInfoText.text = $"πÊ ¿Ã∏ß: {roomName}";
            }
        }
    }

    public void HideRoomInfo()
    {
        if (textPanel != null)
        {
            textPanel.SetActive(false);
        }
    }

    public void UpdateClientInfo(string clientInfo)
    {
        if (clientInfoText != null)
        {
            clientInfoText.text = clientInfo;
        }
    }

    private void CancelMatching()
    {
        OnMatchmakingCancelled?.Invoke();
    }
}