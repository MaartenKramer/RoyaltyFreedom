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
    public GameObject continueIcon;
    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI textComponent;
    public float textSpeed = 0.05f;
    private DialogueLine[] lines;
    private int index;
    private Action onDialogueComplete;
    private float inputCooldown = 0f;

    [Header("Audio")]
    public AudioSource typeAudioSource;
    public AudioClip typeSound;
    private bool isTyping = false;

    void Awake()
    {
        Instance = this;
        dialogueBox.SetActive(false);
    }

    public bool IsDialogueActive()
    {
        return dialogueBox.activeSelf;
    }

    void Update()
    {
        if (inputCooldown > 0f)
        {
            inputCooldown -= Time.deltaTime;
            return;
        }

        if (dialogueBox.activeSelf && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E)))
        {
            if (lines == null || index >= lines.Length)
                return;

            if (isTyping)
            {
                StopAllCoroutines();
                textComponent.text = lines[index].text;
                continueIcon.SetActive(true);
                isTyping = false;
            }
            else if (textComponent.text == lines[index].text)
            {
                continueIcon.SetActive(false);
                NextLine();
            }
        }
    }

    public void ShowDialogue(DialogueLine[] newLines, Action onComplete = null)
    {
        if (newLines == null || newLines.Length == 0)
        {
            Debug.LogWarning("ShowDialogue called with null or empty dialogue lines!");
            onComplete?.Invoke();
            return;
        }

        lines = newLines;
        onDialogueComplete = onComplete;
        index = 0;
        dialogueBox.SetActive(true);
        textComponent.text = string.Empty;
        continueIcon.SetActive(false);
        inputCooldown = 0.2f;
        UpdateSpeakerName();
        StartCoroutine(TypeLine());
    }

    void UpdateSpeakerName()
    {
        if (lines == null || index >= lines.Length)
        {
            if (speakerNameText != null)
                speakerNameText.gameObject.SetActive(false);
            return;
        }

        if (speakerNameText != null)
        {
            if (!string.IsNullOrEmpty(lines[index].speaker))
            {
                speakerNameText.text = lines[index].speaker;
                speakerNameText.gameObject.SetActive(true);

                if (lines[index].speaker == "Dawn")
                {
                    typeAudioSource.pitch = 1.5f;
                }
                else if (lines[index].speaker == "Printer 335")
                {
                    typeAudioSource.pitch = 3f;
                }
                else
                {
                    typeAudioSource.pitch = 0.5f;
                }
            }
            else
            {
                speakerNameText.gameObject.SetActive(false);
            }
        }
    }

    IEnumerator TypeLine()
    {
        if (lines == null || index >= lines.Length)
            yield break;

        isTyping = true;
        foreach (char c in lines[index].text.ToCharArray())
        {
            textComponent.text += c;
            if (typeAudioSource != null && typeSound != null)
            {
                typeAudioSource.PlayOneShot(typeSound);
            }

            if (c == '.' || c == '!' || c == '?')
            {
                yield return new WaitForSeconds(0.2f);
            }

            if (c == ',')
            {
                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(textSpeed);
        }
        continueIcon.SetActive(true);
        isTyping = false;
    }

    void NextLine()
    {
        if (lines == null || index >= lines.Length)
            return;

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
            continueIcon.SetActive(false);
            dialogueBox.SetActive(false);
            onDialogueComplete?.Invoke();
        }
    }
}