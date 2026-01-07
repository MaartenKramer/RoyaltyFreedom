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

    [Header("Audio")]
    public AudioSource typeAudioSource;
    public AudioClip typeSound;

    private bool isTyping = false;

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
            if (isTyping)
            {
                StopAllCoroutines();
                textComponent.text = lines[index].text;
                isTyping = false;
            }
            else if (textComponent.text == lines[index].text)
            {
                NextLine();
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
        isTyping = true;

        foreach (char c in lines[index].text.ToCharArray())
        {
            textComponent.text += c;
            if (typeAudioSource != null && typeSound != null)
            {
                typeAudioSource.PlayOneShot(typeSound);
            }
            yield return new WaitForSeconds(textSpeed);
        }

        isTyping = false;
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
            if (speakerNameText != null)
            {
                speakerNameText.text = string.Empty;
                speakerNameText.gameObject.SetActive(false);
            }
            dialogueBox.SetActive(false);
            onDialogueComplete?.Invoke();
        }
    }
}