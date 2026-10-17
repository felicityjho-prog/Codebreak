
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

    // ==========================================
    // AWAKE
    // ==========================================

    private void Awake()
    {
        if (voiceAudio == null)
        {
            voiceAudio = GetComponent<AudioSource>();
        }

        HideOldSubtitleUI();
    }

    // ==========================================
    // START VOICE SEQUENCE
    // ==========================================

    public void StartVoiceSequence()
    {
        if (sequenceStarted)
        {
            Debug.Log("Room 2 voice sequence is already running.");
            return;
        }

        if (voiceAudio == null)
        {
            Debug.LogWarning("Room 2 Voice: AudioSource is missing.");
            return;
        }

        if (voiceLines == null || voiceLines.Length == 0)
        {
            Debug.LogWarning("Room 2 Voice: No Voice Lines assigned.");
            return;
        }

        sequenceStarted = true;

        sequenceCoroutine = StartCoroutine(PlayVoiceSequence());

        Debug.Log("ROOM 2 VOICE SEQUENCE STARTED!");
    }

    // ==========================================
    // RESTART VOICE SEQUENCE
    // ==========================================

    public void RestartVoiceSequence()
    {
        Debug.Log("RESTARTING ROOM 2 VOICE SEQUENCE...");

        // Stop any previous sequence.
        if (sequenceCoroutine != null)
        {
            StopCoroutine(sequenceCoroutine);
            sequenceCoroutine = null;
        }

        if (voiceAudio != null)
        {
            voiceAudio.Stop();
            voiceAudio.time = 0f;
        }

        // Reset the guard so the sequence can run again.
        sequenceStarted = false;

        // Start from the first voice line.
        StartVoiceSequence();
    }

    // ==========================================
    // STOP VOICE SEQUENCE
    // ==========================================

    public void StopVoiceSequence()
    {
        if (sequenceCoroutine != null)
        {
            StopCoroutine(sequenceCoroutine);
            sequenceCoroutine = null;
        }

        if (voiceAudio != null)
        {
            voiceAudio.Stop();
            voiceAudio.time = 0f;
        }

        sequenceStarted = false;

        Debug.Log("ROOM 2 VOICE SEQUENCE STOPPED.");
    }

    // ==========================================
    // PLAY VOICE LINES
    // ==========================================

    private IEnumerator PlayVoiceSequence()
    {
        for (int i = 0; i < voiceLines.Length; i++)
        {
            VoiceLine currentLine = voiceLines[i];

            if (currentLine == null || currentLine.voiceClip == null)
            {
                Debug.LogWarning(
                    "Room 2 Voice: Voice Line " +
                    (i + 1) +
                    " has no Audio Clip."
                );

                continue;
            }

            AudioClip clip = currentLine.voiceClip;

            voiceAudio.Stop();
            voiceAudio.clip = clip;
            voiceAudio.time = 0f;
            voiceAudio.Play();

            Debug.Log(
                "Room 2 Voice Line " +
                (i + 1) +
                " playing: " +
                clip.name
            );

            yield return new WaitForSecondsRealtime(clip.length);

            voiceAudio.Stop();

            if (lineGap > 0f)
            {
                yield return new WaitForSecondsRealtime(lineGap);
            }
        }

        if (voiceAudio != null)
        {
            voiceAudio.Stop();
        }

        sequenceStarted = false;
        sequenceCoroutine = null;

        Debug.Log("Room 2 Voice sequence completed.");
    }

    // ==========================================
    // HIDE OLD SUBTITLE UI
    // ==========================================

    private void HideOldSubtitleUI()
    {
        TMPro.TMP_Text oldSubtitle =
            GetComponentInChildren<TMPro.TMP_Text>(true);

        if (oldSubtitle != null)
        {
            oldSubtitle.text = "";
            oldSubtitle.gameObject.SetActive(false);
        }

        UnityEngine.UI.Image panelImage =
            GetComponent<UnityEngine.UI.Image>();

        if (panelImage != null)
        {
            panelImage.enabled = false;
        }
    }

    // ==========================================
    // CLEANUP
    // ==========================================

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
