using UnityEngine;

public class TutorialAltProgress : MonoBehaviour
{
    public GameObject PrinterCombatField;
    public GameObject PrinterFire;

    public AudioSource printerAudio;
    public AudioClip printExplodeSound;

    private bool exploded = false;

    void Start()
    {

    }

    void Update()
    {

        if (Progress.Instance.flags.Contains("PrinterCombat") && !Progress.Instance.flags.Contains("PrinterDies"))
        {
            PrinterCombatField.SetActive(true);
        }

        else if (Progress.Instance.flags.Contains("PrinterDead") && exploded == false)
        {
            exploded = true;
            PrinterFire.SetActive(true);
            printerAudio.PlayOneShot(printExplodeSound);

        }
    }
}
