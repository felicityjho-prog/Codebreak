using UnityEngine;

public class MouseClickSound : MonoBehaviour
{
    [Header("Click Sound")]
    public AudioClip clickSound;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError(
                "MouseClickSound requires an AudioSource!",
                this
            );
        }
    }

    private void Update()
    {
        // Do not register click sounds while the game is paused.
        if (Time.timeScale <= 0f || AudioListener.pause)
        {
            return;
        }

        if (!Input.GetMouseButtonDown(0))
        {
            return;
        }

        // Prevent errors if the AudioSource or clip is missing.
        if (audioSource == null || clickSound == null)
        {
            return;
        }

        audioSource.PlayOneShot(clickSound);
    }
}