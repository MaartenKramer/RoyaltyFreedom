using UnityEngine;
using System.Collections.Generic;
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
}