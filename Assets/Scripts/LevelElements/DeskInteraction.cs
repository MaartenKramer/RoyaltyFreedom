using UnityEngine;

public class DeskInteraction : MonoBehaviour
{
    public PrinterInteraction printerInteraction;

    public GameObject himIcon;

    private bool playerInside = false;
    private bool doneTalking = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
    }

    void Update()
    {
        if (playerInside && !doneTalking)
        {
            himIcon.SetActive(true);
        }

        else
        {
            himIcon.SetActive(false);
        }

        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (playerInside && Input.GetKeyDown(KeyCode.E) && boxCollider != null && boxCollider.enabled)
        {
            if (printerInteraction != null)
            {
                doneTalking = true;
                printerInteraction.OnDeskUsed();
            }
        }
    }
}