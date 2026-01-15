using UnityEngine;
using TMPro;

public class LeftHallProgress : MonoBehaviour
{
    public GameObject regularHarold;
    public GameObject defeatedHarold;
    public GameObject fightTrigger;

    public TextMeshProUGUI questTitle;
    public TextMeshProUGUI questText;

    public AudioSource overworldMusic;

    void Start()
    {
        GameObject musicObject = GameObject.Find("OverworldMusic"); // Replace with your actual GameObject name
        if (musicObject != null)
        {
            overworldMusic = musicObject.GetComponent<AudioSource>();
        }

        if (Progress.Instance.flags.Contains("Harold_Defeated"))
        {
            regularHarold.SetActive(false);
            defeatedHarold.SetActive(true);

            if (overworldMusic != null && !overworldMusic.isPlaying)
            {
                overworldMusic.Play();
            }
        }
        else
        {
            regularHarold.SetActive(true);
            defeatedHarold.SetActive(false);
        }

        if (Progress.Instance.flags.Contains("NeedClearance") && !Progress.Instance.flags.Contains("FindX") && !Progress.Instance.flags.Contains("GotForms"))
        {
            questTitle.SetText("Get a clearance card");
            questText.SetText("- Ask around to see if someone knows how to get a clearance card.");
        }

        if (Progress.Instance.flags.Contains("FindX") && !Progress.Instance.flags.Contains("GotForms"))
        {
            questTitle.SetText("Get the Clearance Forms");
            questText.SetText("- Find Hulda in the conference rooms.");
        }

        if (Progress.Instance.flags.Contains("GotForms") && !Progress.Instance.flags.Contains("FilledInForms"))
        {
            questTitle.SetText("Fill out the forms");
            questText.SetText("- Go to your desk and fill out the Clearance Forms.");
        }

        if (Progress.Instance.flags.Contains("FilledInForms") && !Progress.Instance.flags.Contains("DeliverForms"))
        {
            questTitle.SetText("Give the forms to Yanique");
            questText.SetText("- Find Yanique in the caferteria.");
        }

        if (Progress.Instance.flags.Contains("DeliverForms") && !Progress.Instance.flags.Contains("GotClearance"))
        {
            questTitle.SetText("Grab a Clearance Card");
            questText.SetText("- Put the forms on Yanique's desk and get the card.");
        }

        if (Progress.Instance.flags.Contains("GotClearance"))
        {
            questTitle.SetText("Return to Seth");
            questText.SetText("- Talk to Seth in the manager's office.");
        }
    }

    void Update()
    {
        if (Progress.Instance.flags.Contains("HaroldDialogue1") && !Progress.Instance.flags.Contains("Harold_Defeated"))
        {
            fightTrigger.SetActive(true);
        }

        if (Progress.Instance.flags.Contains("FindX") && !Progress.Instance.flags.Contains("GotForms"))
        {
            questTitle.SetText("Get the Clearance Forms");
            questText.SetText("- Find Hulda in the conference rooms.");
        }
    }
}
