using UnityEngine;
using System.Collections;

public class Speakable : MonoBehaviour
{
    public DialogueLine[] dialogue;

    [Header("Flag Requirements")]
    [Tooltip("What flags are required for dialogue to begin?")]
    public string[] requiredFlags;
    [Tooltip("What flags prevent dialogue from beginning?")]
    public string[] blockedByFlags;

    [Header("Flag Triggers")]
    [Tooltip("What flags does completing this dialogue create?")]
    public string[] flagsToSet;

    [Header("Trigger Settings")]
    [Tooltip("If true, dialogue starts automatically when player enters range. If false, requires pressing E.")]
    public bool autoTrigger = false;

    private bool playerInRange = false;
    private bool hasTriggered = false;
    private bool cooldown = false;

    public GameObject interactionBubble;

    void Update()
    {
        if (playerInRange && !DialogueManager.Instance.IsDialogueActive() && CanTriggerDialogue() && !cooldown)
        {
            if (autoTrigger)
            {
                if (!hasTriggered)
                {
                    DialogueManager.Instance.ShowDialogue(dialogue, OnDialogueComplete);
                    hasTriggered = true;
                }
            }

            else {

                interactionBubble.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
                {
                    DialogueManager.Instance.ShowDialogue(dialogue, OnDialogueComplete);
                    cooldown = true;
                    interactionBubble.SetActive(false);
                }
            }
        }
    }

    bool CanTriggerDialogue()
    {
        foreach (string flag in requiredFlags)
        {
            if (!Progress.Instance.flags.Contains(flag))
            {
                return false;
            }
        }

        foreach (string flag in blockedByFlags)
        {
            if (Progress.Instance.flags.Contains(flag))
            {
                return false;
            }
        }

        return true;
    }

    void OnDialogueComplete()
    {
        foreach (string flag in flagsToSet)
        {
            Progress.Instance.flags.Add(flag);
        }

        StartCoroutine(ResetTriggerCooldown());
    }

    IEnumerator ResetTriggerCooldown()
    {
        yield return new WaitForSeconds(0.3f);
        cooldown = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            hasTriggered = false;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            hasTriggered = false;
            interactionBubble.SetActive(false);
        }
    }
}