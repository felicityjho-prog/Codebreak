using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class Room2VoiceSubtitleManager : MonoBehaviour
{
    [Header("Voice")]
    public AudioSource voiceAudio;

    [Header("Voice Lines")]
    public VoiceLine[] voiceLines;

    [System.Serializable]
    public class VoiceLine
    {
        public AudioClip voiceClip;
    }

    [Header("Settings")]
    [Tooltip("Small pause between voice lines.")]
    [Range(0f, 0.5f)]
    public float lineGap = 0.1f;

    private bool sequenceStarted = false;
    private Coroutine sequenceCoroutine;

    // =====================================================
    // AWAKE
    // =====================================================

    private void Awake()
    {
        if (voiceAudio == null)
        {
            voiceAudio = GetComponent<AudioSource>();
        }

        // Make sure the old subtitle UI is hidden.
        HideOldSubtitleUI();
    }

    // =====================================================
    // START VOICE
    // =====================================================

    public void StartVoiceSequence()
    {
        Debug.Log("ROOM 2 VOICE STARTED!");

        if (sequenceStarted)
            return;

        if (voiceAudio == null)
        {
            Debug.LogWarning(
                "Room 2 Voice: AudioSource is missing."
            );
            return;
        }

        if (voiceLines == null ||
            voiceLines.Length == 0)
        {
            Debug.LogWarning(
                "Room 2 Voice: No Voice Lines assigned."
            );
            return;
        }

        sequenceStarted = true;

        sequenceCoroutine =
            StartCoroutine(
                PlayVoiceSequence()
            );
    }

    // =====================================================
    // PLAY 7 VOICE CLIPS
    // =====================================================

    private IEnumerator PlayVoiceSequence()
    {
        for (int i = 0; i < voiceLines.Length; i++)
        {
            VoiceLine currentLine =
                voiceLines[i];

            if (currentLine == null ||
                currentLine.voiceClip == null)
            {
                Debug.LogWarning(
                    "Room 2 Voice: Voice Line " +
                    (i + 1) +
                    " has no Audio Clip."
                );

                continue;
            }

            AudioClip clip =
                currentLine.voiceClip;

            // Stop previous clip
            voiceAudio.Stop();

            // Assign current clip
            voiceAudio.clip = clip;

            voiceAudio.time = 0f;

            // Play voice
            voiceAudio.Play();

            Debug.Log(
                "Room 2 Voice Line " +
                (i + 1) +
                " playing: " +
                clip.name
            );

            // Wait for the exact clip duration
            yield return new WaitForSecondsRealtime(
                clip.length
            );

            // Stop current clip
            voiceAudio.Stop();

            // Small gap before next line
            if (lineGap > 0f)
            {
                yield return new WaitForSecondsRealtime(
                    lineGap
                );
            }
        }

        // =================================================
        // COMPLETE
        // =================================================

        voiceAudio.Stop();

        sequenceStarted = false;
        sequenceCoroutine = null;

        Debug.Log(
            "Room 2 Voice sequence completed."
        );
    }

    // =====================================================
    // HIDE OLD SUBTITLE UI
    // =====================================================

    private void HideOldSubtitleUI()
    {
        // Find SubtitleText anywhere under this object
        TMPro.TMP_Text oldSubtitle =
            GetComponentInChildren<TMPro.TMP_Text>(
                true
            );

        if (oldSubtitle != null)
        {
            oldSubtitle.text = "";
            oldSubtitle.gameObject.SetActive(false);
        }

        // Hide the background image without
        // disabling this GameObject, because
        // this GameObject contains the AudioSource.
        UnityEngine.UI.Image panelImage =
            GetComponent<UnityEngine.UI.Image>();

        if (panelImage != null)
        {
            panelImage.enabled = false;
        }
    }

    // =====================================================
    // CLEANUP
    // =====================================================

    private void OnDisable()
    {
        StopAllCoroutines();

        if (voiceAudio != null)
        {
            voiceAudio.Stop();
        }

        sequenceStarted = false;
        sequenceCoroutine = null;
    }
}