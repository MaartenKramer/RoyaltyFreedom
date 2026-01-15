using UnityEngine;
using TMPro;

public class TutorialProgress : MonoBehaviour
{
    public TextMeshProUGUI questTitle;
    public TextMeshProUGUI questText;

    void Start()
    {
        if (Progress.Instance.flags.Contains("NeedClearance") && !Progress.Instance.flags.Contains("FindX") && !Progress.Instance.flags.Contains("GotForms"))
        {
            questTitle.SetText("Get a clearance card");
            questText.SetText("- Ask around to see if someone knows how to get a clearance card.");
        }

        if (Progress.Instance.flags.Contains("FindX") && !Progress.Instance.flags.Contains("GotForms"))
        {
            questTitle.SetText("Get the clearance forms");
            questText.SetText("- Find X in the converence rooms.");
        }

        if (Progress.Instance.flags.Contains("GotForms") && !Progress.Instance.flags.Contains("FilledInForms"))
        {
            questTitle.SetText("Fill the forms in");
            questText.SetText("- Go to your desk and fill the clearance forms in.");
        }

        if (Progress.Instance.flags.Contains("DeliverForms") && !Progress.Instance.flags.Contains("GotClearance"))
        {
            questTitle.SetText("Grab a clearance card");
            questText.SetText("- Put the forms on Y's desk and get the card.");
        }

        if (Progress.Instance.flags.Contains("GotClearance"))
        {
            questTitle.SetText("Talk to Seth again");
            questText.SetText("- Talk to Seth in the manager's office.");
        }
    }

    void Update()
    {
        if (Progress.Instance.flags.Contains("FilledInForms") && !Progress.Instance.flags.Contains("DeliverForms"))
        {
            questTitle.SetText("Give the forms to Y");
            questText.SetText("- Find Y in the caferteria.");
        }
    }
}
