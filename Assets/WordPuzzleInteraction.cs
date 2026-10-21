
using UnityEngine;

public class WordPuzzleInteraction : MonoBehaviour
{
    [Header("Word Puzzle UI")]
    public GameObject wordScrabblePanel;

    [Header("Interaction")]
    public KeyCode interactionKey = KeyCode.E;

    private bool playerIsNear;
    private bool isOpen;

    private void Start()
    {
        if (wordScrabblePanel != null)
        {
            wordScrabblePanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerIsNear && Input.GetKeyDown(interactionKey))
        {
            ToggleWordPuzzle();
        }
    }

    private void ToggleWordPuzzle()
    {
        if (wordScrabblePanel == null)
        {
            Debug.LogWarning("WordScrabblePanel is not assigned!");
            return;
        }

        isOpen = !isOpen;
        wordScrabblePanel.SetActive(isOpen);

        Cursor.lockState = isOpen
            ? CursorLockMode.None
            : CursorLockMode.Locked;

        Cursor.visible = isOpen;

        Time.timeScale = 1f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNear = false;
        }
    }
}
