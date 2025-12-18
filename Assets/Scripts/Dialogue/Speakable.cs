using UnityEngine;

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
    private bool hasTriggered = false; // Prevent auto-trigger from repeating

    // check if player is close enough and pressing the right key
    void Update()
    {
        if (playerInRange && !DialogueManager.Instance.IsDialogueActive() && CanTriggerDialogue())
        {
            if (autoTrigger)
            {
                // Auto-trigger once when entering range
                if (!hasTriggered)
                {
                    DialogueManager.Instance.ShowDialogue(dialogue, OnDialogueComplete);
                    hasTriggered = true;
                }
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                // Manual trigger with E key
                DialogueManager.Instance.ShowDialogue(dialogue, OnDialogueComplete);
            }
        }
    }

    // check if all required flags are present
    bool CanTriggerDialogue()
    {
        foreach (string flag in requiredFlags)
        {
            if (!Progress.Instance.flags.Contains(flag))
            {
                return false;
            }
        }

        // check if any blocked flags are present
        foreach (string flag in blockedByFlags)
        {
            if (Progress.Instance.flags.Contains(flag))
            {
                return false;
            }
        }

        return true;
    }

    // Called when dialogue finishes - sets flags AFTER completion
    void OnDialogueComplete()
    {
        foreach (string flag in flagsToSet)
        {
            Progress.Instance.flags.Add(flag);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            hasTriggered = false; // Reset trigger state when player enters
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            hasTriggered = false; // Reset trigger state when player leaves
        }
    }
}