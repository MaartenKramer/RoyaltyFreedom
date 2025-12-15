using UnityEngine;

public class TransitionTrigger : MonoBehaviour
{
    public string sceneToLoad;
    public string exitPointID;

    private bool inRange;

    //Sends relevant data to scene fader
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inRange = false;
        }
    }

    void Update()
    {
        if (inRange && Input.GetKeyDown(KeyCode.E) && SceneFader.Instance != null)
        {
            SceneFader.Instance.TransitionToScene(sceneToLoad, exitPointID);
        }
    }
}