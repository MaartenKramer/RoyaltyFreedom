using UnityEngine;

public class LeftHallProgress : MonoBehaviour
{
    public GameObject regularHarold;
    public GameObject defeatedHarold;
    public GameObject fightTrigger;

    void Start()
    {
        if (Progress.Instance.flags.Contains("Harold_Defeated"))
        {
            regularHarold.SetActive(false);
            defeatedHarold.SetActive(true);
        }
        else
        {
            regularHarold.SetActive(true);
            defeatedHarold.SetActive(false);
        }
    }

    void Update()
    {
        if (Progress.Instance.flags.Contains("HaroldDialogue1") && !Progress.Instance.flags.Contains("Harold_Defeated"))
        {
            fightTrigger.SetActive(true);
        }
    }
}
