using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public class Progress : MonoBehaviour
{
    public static Progress Instance;

    public HashSet<string> flags = new HashSet<string>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.KeypadPeriod))
        {
            SceneFader.Instance.TransitionToScene(SceneManager.GetActiveScene().name, "");
        }

        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            SceneFader.Instance.TransitionToScene("LeftHall", "");
            Instance.flags.Add("BenIntro");
            Instance.flags.Add("PrinterDies");
            Instance.flags.Add("PrinterDead");
        }
    }
}