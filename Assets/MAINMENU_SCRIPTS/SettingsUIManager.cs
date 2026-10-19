
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsUIManager : MonoBehaviour
{
    [Header("Master Volume")]
    public Slider masterVolumeSlider;

    [Header("Fullscreen")]
    public Toggle fullscreenToggle;

    [Header("Mouse Sensitivity")]
    public Slider mouseSensitivitySlider;
    public TMP_Text mouseSensitivityValue;

    [Header("Player Controller")]
    [SerializeField] private PlayerController playerController;

    private const string VolumeKey = "MasterVolume";
    private const string FullscreenKey = "FullscreenEnabled";
    private const string SensitivityKey = "MouseSensitivityMultiplier";

    private void Start()
    {
        // Find the player in MainGame if one exists.
        if (playerController == null)
        {
            playerController = FindFirstObjectByType<PlayerController>();
        }

        InitializeMasterVolume();
        InitializeFullscreen();
        InitializeMouseSensitivity();
    }

    // ---------------- MASTER VOLUME ----------------

    private void InitializeMasterVolume()
    {
        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 1f);

        AudioListener.volume = savedVolume;

        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.minValue = 0f;
            masterVolumeSlider.maxValue = 1f;

            masterVolumeSlider.SetValueWithoutNotify(savedVolume);
            masterVolumeSlider.onValueChanged.RemoveListener(SetMasterVolume);
            masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        }
    }

    public void SetMasterVolume(float value)
    {
        value = Mathf.Clamp01(value);

        AudioListener.volume = value;

        PlayerPrefs.SetFloat(VolumeKey, value);
        PlayerPrefs.Save();
    }

    // ---------------- FULLSCREEN ----------------

    private void InitializeFullscreen()
    {
        bool savedFullscreen =
            PlayerPrefs.GetInt(FullscreenKey, 1) == 1;

        if (fullscreenToggle != null)
        {
            fullscreenToggle.SetIsOnWithoutNotify(savedFullscreen);
            fullscreenToggle.onValueChanged.RemoveListener(SetFullscreen);
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        }

        // Apply the saved setting in a built game.
        if (!Application.isEditor)
        {
            ApplyFullscreen(savedFullscreen);
        }
    }

    public void SetFullscreen(bool enabled)
    {
        PlayerPrefs.SetInt(FullscreenKey, enabled ? 1 : 0);
        PlayerPrefs.Save();

        if (!Application.isEditor)
        {
            ApplyFullscreen(enabled);
        }
    }

    private void ApplyFullscreen(bool enabled)
    {
        if (enabled)
        {
            Resolution resolution = Screen.currentResolution;

            Screen.SetResolution(
                resolution.width,
                resolution.height,
                FullScreenMode.FullScreenWindow
            );
        }
        else
        {
            Screen.SetResolution(
                1280,
                720,
                FullScreenMode.Windowed
            );
        }
    }

    // ---------------- MOUSE SENSITIVITY ----------------

    private void InitializeMouseSensitivity()
    {
        float savedSensitivity = Mathf.Clamp(
            PlayerPrefs.GetFloat(SensitivityKey, 1f),
            0.5f,
            3f
        );

        if (mouseSensitivitySlider != null)
        {
            mouseSensitivitySlider.minValue = 0.5f;
            mouseSensitivitySlider.maxValue = 3f;

            mouseSensitivitySlider.SetValueWithoutNotify(savedSensitivity);
            mouseSensitivitySlider.onValueChanged.RemoveListener(UpdateSensitivityValue);
            mouseSensitivitySlider.onValueChanged.AddListener(UpdateSensitivityValue);
        }

        UpdateSensitivityLabel(savedSensitivity);
        ApplySensitivityToPlayer(savedSensitivity);
    }

    public void UpdateSensitivityValue(float value)
    {
        value = Mathf.Clamp(value, 0.5f, 3f);

        UpdateSensitivityLabel(value);

        PlayerPrefs.SetFloat(SensitivityKey, value);
        PlayerPrefs.Save();

        ApplySensitivityToPlayer(value);
    }

    private void UpdateSensitivityLabel(float value)
    {
        if (mouseSensitivityValue != null)
        {
            mouseSensitivityValue.text = value.ToString("F1");
        }
    }

    private void ApplySensitivityToPlayer(float multiplier)
    {
        if (playerController == null)
        {
            return;
        }

        playerController.SetSensitivityMultiplier(multiplier);
    }

    private void OnDestroy()
    {
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.onValueChanged.RemoveListener(SetMasterVolume);
        }

        if (fullscreenToggle != null)
        {
            fullscreenToggle.onValueChanged.RemoveListener(SetFullscreen);
        }

        if (mouseSensitivitySlider != null)
        {
            mouseSensitivitySlider.onValueChanged.RemoveListener(UpdateSensitivityValue);
        }
    }
}
