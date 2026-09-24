using UnityEngine;
using System.Collections;

public class TableInteraction : MonoBehaviour
{
    [Header("Interaction")]
    public float interactionDistance = 3f;

    [Header("UI")]
    public GameObject interactPrompt;
    public CanvasGroup promptCanvasGroup;

    [Header("Fade Settings")]
    public float fadeDuration = 0.5f;

    [Header("Puzzle")]
    public GameObject puzzlePanel;

    private Transform player;
    private bool isNear = false;
    private Coroutine fadeCoroutine;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        if (interactPrompt != null)
        {
            interactPrompt.SetActive(true);
        }

        if (promptCanvasGroup != null)
        {
            promptCanvasGroup.alpha = 0f;
            promptCanvasGroup.interactable = false;
            promptCanvasGroup.blocksRaycasts = false;
        }
    }

    void Update()
    {
        if (player == null)
            return;

        float distance = Vector3.Distance(
            transform.position,
            player.position
        );

        // PLAYER IS NEAR
        if (distance <= interactionDistance)
        {
            if (!isNear)
            {
                isNear = true;
                FadeIn();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                OpenPuzzle();
            }
        }
        // PLAYER IS FAR
        else
        {
            if (isNear)
            {
                isNear = false;
                FadeOut();
            }
        }
    }

    void FadeIn()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadePrompt(1f));
    }

    void FadeOut()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadePrompt(0f));
    }

    IEnumerator FadePrompt(float targetAlpha)
    {
        if (promptCanvasGroup == null)
            yield break;

        float startAlpha = promptCanvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / fadeDuration;

            promptCanvasGroup.alpha = Mathf.Lerp(
                startAlpha,
                targetAlpha,
                t
            );

            yield return null;
        }

        promptCanvasGroup.alpha = targetAlpha;

        if (targetAlpha > 0f)
        {
            promptCanvasGroup.interactable = true;
            promptCanvasGroup.blocksRaycasts = true;
        }
        else
        {
            promptCanvasGroup.interactable = false;
            promptCanvasGroup.blocksRaycasts = false;
        }
    }

    void OpenPuzzle()
    {
        if (puzzlePanel != null)
        {
            puzzlePanel.SetActive(true);
        }

        FadeOut();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}