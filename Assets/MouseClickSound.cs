
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MouseClickSound : MonoBehaviour
{
    [Header("Click Sound")]
    public AudioClip clickSound;

    private AudioSource audioSource;

    // Records which frame should suppress the normal click sound.
    private static int suppressedClickFrame = -1;

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

    // Called by ClickableObject when an item has its own sound.
    public static void SuppressClickSoundForCurrentFrame()
    {
        suppressedClickFrame = Time.frameCount;
    }

    private void Update()
    {
        if (Time.timeScale <= 0f || AudioListener.pause)
            return;

        if (!Input.GetMouseButtonDown(0))
            return;

        if (audioSource == null || clickSound == null)
            return;

        // Wait until other click handlers have had a chance to run.
        StartCoroutine(PlayClickSoundAfterMouseEvents(Time.frameCount));
    }

    private IEnumerator PlayClickSoundAfterMouseEvents(int clickFrame)
    {
        yield return new WaitForEndOfFrame();

        // A special item sound takes priority for this click.
        if (suppressedClickFrame == clickFrame)
            yield break;

        // Recheck pause state before playing.
        if (Time.timeScale <= 0f || AudioListener.pause)
            yield break;

        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
}
