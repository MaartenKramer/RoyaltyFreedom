using UnityEngine;

public class TransitionTrigger : MonoBehaviour
{
    public string sceneToLoad;
    public string exitPointID;

    //Sends relevant data to scene fader
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && SceneFader.Instance != null)
        {
            SceneFader.Instance.TransitionToScene(sceneToLoad, exitPointID);
        }
    }
}