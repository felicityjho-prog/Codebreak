
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUIController : MonoBehaviour
{
    [Header("How To Play Popup")]
    [SerializeField] private GameObject howToPlayPanel;

    [Header("Settings Popup")]
    [SerializeField] private GameObject settingsPanel;

    public void OpenHowToPlay()
    {
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("HowToPlayPanel is not assigned.");
        }
    }

    public void CloseHowToPlay()
    {
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(false);
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("MainGame");
    }

    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("SettingsPanel is not assigned.");
        }
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    public void ExitGame()
    {
        Debug.Log("Exiting CodeBreak...");

        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
