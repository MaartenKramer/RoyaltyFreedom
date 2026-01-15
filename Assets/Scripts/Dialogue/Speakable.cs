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
    private static float globalDialogueCooldown = 0f;

    public GameObject interactionBubble;

    void Update()
    {
        // Reduce global cooldown
        if (globalDialogueCooldown > 0f)
        {
            globalDialogueCooldown -= Time.deltaTime;
        }

        // Don't allow triggering when dialogue is active
        if (DialogueManager.Instance.IsDialogueActive())
        {
            if (interactionBubble != null)
                interactionBubble.SetActive(false);
            return;
        }

        if (playerInRange && CanTriggerDialogue() && !hasTriggered)
        {
            bool canActuallyTrigger = globalDialogueCooldown <= 0f;

            if (autoTrigger)
            {
                if (canActuallyTrigger)
                {
                    DialogueManager.Instance.ShowDialogue(dialogue, OnDialogueComplete);
                    hasTriggered = true;
                }
            }
            else
            {
                if (interactionBubble != null)
                    interactionBubble.SetActive(true);

                if (canActuallyTrigger && (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0)))
                {
                    DialogueManager.Instance.ShowDialogue(dialogue, OnDialogueComplete);
                    hasTriggered = true;
                    if (interactionBubble != null)
                        interactionBubble.SetActive(false);
                }
            }
        }
        else if (playerInRange && interactionBubble != null)
        {
            // Only hide bubble if NO other Speakable on this GameObject can trigger
            bool anyCanTrigger = false;
            foreach (Speakable speakable in GetComponents<Speakable>())
            {
                if (speakable.playerInRange && speakable.CanTriggerDialogue() && !speakable.hasTriggered)
                {
                    anyCanTrigger = true;
                    break;
                }
            }

            if (!anyCanTrigger)
            {
                interactionBubble.SetActive(false);
            }
        }
        else if (interactionBubble != null)
        {
            interactionBubble.SetActive(false);
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

        globalDialogueCooldown = 0.5f;
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
            if (interactionBubble != null)
                interactionBubble.SetActive(false);
        }
    }
}