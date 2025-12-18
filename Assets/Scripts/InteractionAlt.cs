using UnityEngine;
using System.Collections;

public class InteractionAlt : MonoBehaviour
{
    public GameObject printer;
    public int printedItems = 0;

    private bool playerInside = false;
    private bool interactionCooldown = false;
    bool printerBroken = false;

    public AudioSource printerAudio;
    public AudioClip printSound;
    public AudioClip printBrokenSound;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Printer"))
        {
            playerInside = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Printer"))
        {
            playerInside = false;
        }
    }

    void Update()
    {
        if (playerInside && Input.GetKeyDown(KeyCode.E))
        {
            if (!interactionCooldown && printedItems < 4)
            {
                StartCoroutine(Cooldown());
            }

            else if (printedItems >= 4 && printerBroken == false)
            {
                Progress.Instance.flags.Add("PrinterBreaks");
                Debug.Log(Progress.Instance.flags);
                printerAudio.PlayOneShot(printBrokenSound);
                interactionCooldown = true;
                printerBroken = true;
            }
        }


        
    }

    IEnumerator Cooldown()
    {
        interactionCooldown = true;
        printerAudio.PlayOneShot(printSound);
        yield return new WaitForSeconds(3f);
        interactionCooldown = false;
        printedItems++;
        Debug.Log(printedItems);
    }
}