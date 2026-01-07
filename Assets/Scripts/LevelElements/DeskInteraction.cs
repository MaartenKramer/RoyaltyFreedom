using UnityEngine;

public class DeskInteraction : MonoBehaviour
{
    public PrinterInteraction printerInteraction;
    private bool playerInside = false;

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
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (playerInside && Input.GetKeyDown(KeyCode.E) && boxCollider != null && boxCollider.enabled)
        {
            if (printerInteraction != null)
            {
                printerInteraction.OnDeskUsed();
            }
        }
    }
}