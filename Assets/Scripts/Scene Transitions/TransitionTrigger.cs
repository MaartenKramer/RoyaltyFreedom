using UnityEngine;

public class TransitionTrigger : MonoBehaviour
{
    public string sceneToLoad;
    public string exitPointID;

    public bool autoTransition;
    private bool inRange;

    public GameObject interactionBubble;

    //Sends relevant data to scene fader
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inRange = true;
            interactionBubble.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inRange = false;
            interactionBubble.SetActive(false);
        }
    }

    void Update()
    {
        if (inRange && Input.GetKeyDown(KeyCode.E) && SceneFader.Instance != null)
        {
            SceneFader.Instance.TransitionToScene(sceneToLoad, exitPointID);
        }

        if (inRange && autoTransition && SceneFader.Instance != null)
        {
            SceneFader.Instance.TransitionToScene(sceneToLoad, exitPointID);
        }
    }
}