using UnityEngine;
using TMPro;
using System.Collections;

public class InteractionMessage : MonoBehaviour
{
    public GameObject messageUI;

    public void ShowMessage()
    {
        StartCoroutine(DisplayMessage());
    }

    IEnumerator DisplayMessage()
    {
        messageUI.SetActive(true);

        yield return new WaitForSeconds(2f);

        messageUI.SetActive(false);
    }
}