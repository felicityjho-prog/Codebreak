using UnityEngine;

public class PauseForUI : MonoBehaviour
{
    public GameObject taskPanel;

    public MonoBehaviour playerLook;
    public MonoBehaviour playerMove;

    public void ShowPanel()
    {
        taskPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        playerLook.enabled = false;
        playerMove.enabled = false;
    }

    public void HidePanel()
    {
        taskPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerLook.enabled = true;
        playerMove.enabled = true;
    }
}