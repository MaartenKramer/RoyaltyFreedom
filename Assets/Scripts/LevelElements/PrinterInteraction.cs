using UnityEngine;
using System.Collections;

public class PrinterInteraction : MonoBehaviour
{
    [Header("Connections")]
    public GameObject desk;
    public AudioSource printerAudio;
    public AudioSource deskAudio;
    public AudioClip printSound;
    public AudioClip printBrokenSound;

    [Header("Settings")]
    public int deskRequirement = 5;
    public float printCooldown = 3f;

    private int printedItems = 0;
    private bool playerInside = false;
    private bool interactionCooldown = false;
    private bool printerBroken = false;
    private bool firstCycleDone = false;

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
        if (playerInside && Input.GetKeyDown(KeyCode.E) && !interactionCooldown)
        {
            if (!firstCycleDone)
            {
                if (printedItems < deskRequirement)
                {
                    StartCoroutine(PrintCooldown());
                }
            }

            else
            {
                if (printedItems < 4)
                {
                    StartCoroutine(PrintCooldown());
                }
                else if (!printerBroken)
                {
                    BreakPrinter();
                }
            }
        }

        if (!firstCycleDone && printedItems >= deskRequirement && desk != null)
        {
            desk.GetComponent<BoxCollider>().enabled = true;
        }
    }

    IEnumerator PrintCooldown()
    {
        interactionCooldown = true;

        if (printSound != null)
            printerAudio.PlayOneShot(printSound);
        else
            printerAudio.Play();

        yield return new WaitForSeconds(printCooldown);

        printedItems++;
        Debug.Log("Printed papers: " + printedItems);

        interactionCooldown = false;
    }

    void BreakPrinter()
    {
        Progress.Instance.flags.Add("PrinterBreaks");
        Debug.Log( string.Join(", ", Progress.Instance.flags));

        if (printBrokenSound != null)
            printerAudio.PlayOneShot(printBrokenSound);

        interactionCooldown = true;
        printerBroken = true;
    }

    public void OnDeskUsed()
    {
        firstCycleDone = true;
        printedItems = 0;

        if (desk != null)
        {
            BoxCollider deskCollider = desk.GetComponent<BoxCollider>();
            if (deskCollider != null)
            {
                deskCollider.enabled = false;
            }

            if (deskAudio != null)
                deskAudio.Play();
        }

        Debug.Log("Cycle 1 Complete. Starting bullshit!");
    }
}