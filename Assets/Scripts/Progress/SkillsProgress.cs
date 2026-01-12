using Unity.VisualScripting;
using UnityEngine;

public class SkillsProgress : MonoBehaviour
{
    [System.Serializable]
    public class ComboUIElement
    {
        public string comboSequence;
        public GameObject blackoutUI;
    }

    public ComboUIElement[] comboElements;

    public GameObject[] skillsPages;

    private int skillsIndex = 0;

    void Update()
    {
            UpdateSkillsDisplay();
    }

    void UpdateSkillsDisplay()
    {

        foreach (var combo in comboElements)
        {
            string comboFlag = "Combo_" + combo.comboSequence;

            if (Progress.Instance.flags.Contains(comboFlag))
            {
                combo.blackoutUI.SetActive(false);
            }
            else
            {
                combo.blackoutUI.SetActive(true);
            }
        }
    }

    void PageTracker()
    {
        foreach (GameObject SkillsPage in skillsPages) {
            SkillsPage.SetActive(false);
        }

        skillsPages[skillsIndex].SetActive(true);
    }

    public void OnNextButton()
    {
        skillsIndex++;
        PageTracker();
    }

    public void OnPreviousButton()
    {
        skillsIndex--;
        PageTracker();
    }
}