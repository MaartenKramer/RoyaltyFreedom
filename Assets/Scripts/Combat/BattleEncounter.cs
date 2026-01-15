using UnityEngine;
using UnityEngine.SceneManagement;

public enum EnemyChoice
{
    Printer,
    Harold
}

public class BattleEncounter : MonoBehaviour
{


    public EnemyChoice enemyChoice;

    void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player") && enemyChoice == EnemyChoice.Printer)
        {
            SceneFader.Instance.TransitionToScene("TutRoomCombat", "");

        }

        else if (other.CompareTag("Player") && enemyChoice == EnemyChoice.Harold)
        {
            SceneFader.Instance.TransitionToScene("LunCombat3", "");

        }
    }
}