
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

    private const string VolumeKey = "MasterVolume";
    private const string FullscreenKey = "FullscreenEnabled";
    private const string SensitivityKey = "MouseSensitivityMultiplier";

    private void Start()
    {
        InitializeMasterVolume();
        InitializeFullscreen();
        InitializeMouseSensitivity();
    }

    // =========================================
    // MASTER VOLUME
    // =========================================

    private void InitializeMasterVolume()
    {
        float savedVolume =
            Mathf.Clamp01(PlayerPrefs.GetFloat(VolumeKey, 1f));

        AudioListener.volume = savedVolume;

        if (masterVolumeSlider == null)
        {
            Debug.LogWarning("Master Volume Slider is not assigned.");
            return;
        }

        masterVolumeSlider.minValue = 0f;
        masterVolumeSlider.maxValue = 1f;

        masterVolumeSlider.SetValueWithoutNotify(savedVolume);

        masterVolumeSlider.onValueChanged.RemoveListener(SetMasterVolume);
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
    }

    public void SetMasterVolume(float value)
    {
        value = Mathf.Clamp01(value);

        AudioListener.volume = value;

        PlayerPrefs.SetFloat(VolumeKey, value);
        PlayerPrefs.Save();
    }

    // =========================================
    // FULLSCREEN
    // =========================================

    private void InitializeFullscreen()
    {
        bool savedFullscreen = true;

        if (fullscreenToggle == null)
        {
            Debug.LogWarning("Fullscreen Toggle is not assigned.");
            return;
        }

        fullscreenToggle.SetIsOnWithoutNotify(savedFullscreen);

        fullscreenToggle.onValueChanged.RemoveListener(SetFullscreen);
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);

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
            Resolution currentResolution = Screen.currentResolution;

            Screen.SetResolution(
                currentResolution.width,
                currentResolution.height,
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

    // =========================================
    // MOUSE SENSITIVITY
    // =========================================

    private void InitializeMouseSensitivity()
    {
        if (mouseSensitivitySlider == null)
        {
            Debug.LogWarning("Mouse Sensitivity Slider is not assigned.");
            return;
        }

        mouseSensitivitySlider.minValue = 0.5f;
        mouseSensitivitySlider.maxValue = 3f;

        float savedSensitivity =
            PlayerPrefs.GetFloat(SensitivityKey, 1f);

        savedSensitivity = Mathf.Clamp(savedSensitivity, 0.5f, 3f);

        mouseSensitivitySlider.SetValueWithoutNotify(savedSensitivity);

        UpdateSensitivityValue(savedSensitivity);

        mouseSensitivitySlider.onValueChanged
            .RemoveListener(UpdateSensitivityValue);

        mouseSensitivitySlider.onValueChanged
            .AddListener(UpdateSensitivityValue);
    }

    public void UpdateSensitivityValue(float value)
    {
        value = Mathf.Clamp(value, 0f, 3f);

        if (mouseSensitivityValue != null)
        {
            mouseSensitivityValue.text = value.ToString("F1");
        }

        PlayerPrefs.SetFloat(SensitivityKey, value);
        PlayerPrefs.Save();
    }

    // =========================================
    // CLEANUP
    // =========================================

    private void OnDestroy()
    {
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.onValueChanged
                .RemoveListener(SetMasterVolume);
        }

        if (fullscreenToggle != null)
        {
            fullscreenToggle.onValueChanged
                .RemoveListener(SetFullscreen);
        }

        if (mouseSensitivitySlider != null)
        {
            mouseSensitivitySlider.onValueChanged
                .RemoveListener(UpdateSensitivityValue);
        }
    }
}
