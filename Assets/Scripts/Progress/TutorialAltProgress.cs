using UnityEngine;

public class TutorialAltProgress : MonoBehaviour
{
    public GameObject Printer;
    public GameObject PrinterCombatField;
    public GameObject PrinterFire;
    public AudioSource printerAudio;
    public AudioClip printExplodeSound;
    public GameObject dawnUI;
    public GameObject doorTrigger;

    private bool exploded = false;

    void Start()
    {
        if (Progress.Instance.flags.Contains("PrinterDies"))
        {
            dawnUI.SetActive(true);
            PrinterFire.SetActive(true);
            doorTrigger.SetActive(true);
        }
    }

    void Update()
    {

        if (Progress.Instance.flags.Contains("PrinterCombat") && !Progress.Instance.flags.Contains("PrinterDies"))
        {
            PrinterCombatField.SetActive(true);
        }


        else if (Progress.Instance.flags.Contains("PrinterDead") && exploded == false)
        {
            //exploded = true;
            //PrinterFire.SetActive(true);
            //printerAudio.PlayOneShot(printExplodeSound);

        }

        if (Progress.Instance.flags.Contains("PrinterDies"))
        {
            BoxCollider printerCollider = Printer.GetComponent<BoxCollider>();
            printerCollider.enabled = false;
        }
    }
}
