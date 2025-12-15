using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    public GameObject dialogueBox;
    public TextMeshProUGUI textComponent;
    public float textSpeed = 0.05f;

    private string[] lines;
    private int index;

    void Awake()
    {
        Instance = this;
        dialogueBox.SetActive(false);
    }

    // checks if dialogue is currently running
    public bool IsDialogueActive()
    {
        return dialogueBox.activeSelf;
    }

    void Update()
    {
        if (dialogueBox.activeSelf && Input.GetMouseButtonDown(0))
        {
            if (textComponent.text == lines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = lines[index];
            }
        }
    }

    // initalizes dialogue
    public void ShowDialogue(string[] newLines)
    {
        lines = newLines;
        index = 0;
        dialogueBox.SetActive(true);
        textComponent.text = string.Empty;
        StartCoroutine(TypeLine());
    }

    // types out dialogue in typewriter effect
    IEnumerator TypeLine()
    {
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }


    // moves over to next line
    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            dialogueBox.SetActive(false);
        }
    }
}