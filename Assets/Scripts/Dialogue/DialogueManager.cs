using UnityEngine;
using TMPro;
using System.Collections;
using System;

[System.Serializable]
public class DialogueLine
{
    public string speaker;
    [TextArea(2, 4)]
    public string text;
}

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    public GameObject dialogueBox;
    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI textComponent;
    public float textSpeed = 0.05f;

    private DialogueLine[] lines;
    private int index;
    private Action onDialogueComplete;

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
        if (dialogueBox.activeSelf && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E)))
        {
            if (textComponent.text == lines[index].text)
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = lines[index].text;
            }
        }
    }

    // initializes dialogue
    public void ShowDialogue(DialogueLine[] newLines, Action onComplete = null)
    {
        lines = newLines;
        onDialogueComplete = onComplete;
        index = 0;
        dialogueBox.SetActive(true);
        textComponent.text = string.Empty;
        UpdateSpeakerName();
        StartCoroutine(TypeLine());
    }

    void UpdateSpeakerName()
    {
        if (speakerNameText != null)
        {
            if (!string.IsNullOrEmpty(lines[index].speaker))
            {
                speakerNameText.text = lines[index].speaker;
                speakerNameText.gameObject.SetActive(true);
            }
            else
            {
                speakerNameText.gameObject.SetActive(false);
            }
        }
    }

    // types out dialogue in typewriter effect
    IEnumerator TypeLine()
    {
        foreach (char c in lines[index].text.ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    // move to next line
    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            UpdateSpeakerName();
            StartCoroutine(TypeLine());
        }
        else
        {
            dialogueBox.SetActive(false);
            onDialogueComplete?.Invoke();
        }
    }
}