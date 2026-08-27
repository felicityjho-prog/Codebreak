using UnityEngine;

public class MouseClickSound : MonoBehaviour
{
    public AudioClip clickSound;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
}