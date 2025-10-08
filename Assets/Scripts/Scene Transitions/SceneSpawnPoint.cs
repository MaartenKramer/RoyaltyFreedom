using UnityEngine;

public class SceneSpawnPoint : MonoBehaviour
{
    public string spawnPointID;

    private void Start()
    {
        if (SceneTransitionManager.Instance != null)
        {
            // Check if spawn point is correct
            if (SceneTransitionManager.Instance.lastExitPoint == spawnPointID)
            {
                // Moves player to spawn point
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    player.transform.position = transform.position;
                }
            }
        }
    }
}
