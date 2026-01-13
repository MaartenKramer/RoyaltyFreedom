using UnityEngine;

public class HubProgress : MonoBehaviour
{

    public GameObject Harold;


    void Start()
    {
       if (Progress.Instance.flags.Contains("Harold_Defeated")) {
            Harold.SetActive(false);
       }

       else
        {
            return;
        }
    }

}
