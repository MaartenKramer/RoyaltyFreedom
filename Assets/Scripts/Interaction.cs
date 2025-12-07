using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class Interaction : MonoBehaviour
{
    public GameObject printer;
    public GameObject desk;
    public int printedItems = 0;
    private bool playerInside = false;
    private bool interactionCooldown = false;
    private string currentObject = "";

    public AudioSource printerAudio;
    public AudioSource deskAudio;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Printer"))
        {
            currentObject = "Printer";
            playerInside = true;
        }

        if (other.CompareTag("Desk"))
        {
            currentObject = "Desk";
            playerInside = true;
        }
    }

    void Update()
    {
        if (playerInside && Input.GetKeyDown(KeyCode.E))
        {
            if (currentObject == "Printer" && interactionCooldown == false && printedItems < 5)
            {
                StartCoroutine(Cooldown());
                printedItems++;
                
            }
            if (currentObject == "Desk" && interactionCooldown == false)
            {
                printedItems = 0;
                desk.GetComponent<BoxCollider>().enabled = false;

                deskAudio.Play();
            }
        }

        if (printedItems == 5)
        {
            desk.GetComponent<BoxCollider>().enabled = true;
        }
    }

    IEnumerator Cooldown()
    {
        interactionCooldown = true;
        printerAudio.Play();
        yield return new WaitForSeconds(3f);
        interactionCooldown = false;
        Debug.Log(printedItems);
    }
}