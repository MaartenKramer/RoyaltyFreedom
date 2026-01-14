using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneFader : MonoBehaviour
{
    public static SceneFader Instance;

    [Header("UI References")]
    public Image fadeImage;

    [Header("Settings")]
    public float fadeSpeed = 1.5f;

    private bool isTransitioning = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            StartCoroutine(FadeIn());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Changes scene
    public void TransitionToScene(string sceneName, string exitPointID)
    {
        if (!isTransitioning)
        {
            StartCoroutine(TransitionSequence(sceneName, exitPointID));
        }
    }

    private IEnumerator TransitionSequence(string sceneName, string exitPointID)
    {
        isTransitioning = true;

        // Freeze player movement
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null) rb.linearVelocity = Vector3.zero;

            // Disables player scripts
            MonoBehaviour[] scripts = player.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour script in scripts)
            {
                if (script != null && script.enabled)
                {
                    script.enabled = false;
                }
            }
        }

        yield return FadeOut();

        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.lastExitPoint = exitPointID;
        }

        // Load new scene
        SceneManager.LoadScene(sceneName);

        yield return null;

        yield return FadeIn();

        isTransitioning = false;
    }

    private IEnumerator FadeOut()
    {
        float timer = 0;
        Color startColor = fadeImage.color;
        startColor.a = 0;
        Color endColor = startColor;
        endColor.a = 1;

        while (timer < 1)
        {
            timer += Time.deltaTime * fadeSpeed;
            fadeImage.color = Color.Lerp(startColor, endColor, timer);
            yield return null;
        }
    }

    private IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(0.2f);
        float timer = 0;
        Color startColor = fadeImage.color;
        startColor.a = 1;
        Color endColor = startColor;
        endColor.a = 0;

        while (timer < 1)
        {
            timer += Time.deltaTime * (fadeSpeed);
            fadeImage.color = Color.Lerp(startColor, endColor, timer);
            yield return null;
        }
    }
}