using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class WordsWrittenScreen : MonoBehaviour
{
    public Slider slider;

    [SerializeField] private float wordGoal;
    public TextMeshProUGUI goalText;
    [SerializeField] private float wordCount;
    public TextMeshProUGUI countText;

    public int minWords;
    public int maxWords;
    [SerializeField] private float currentWords;
    [SerializeField] private float wordAddition;

    public List<float> goals;
    [SerializeField] private int index;

    public float wordInterval;

    void Start()
    {
        PickRandomGoal();

        StartCoroutine(WriteWords());
    }

    void Update()
    {
        
    }

    private IEnumerator WriteWords()
    {
        wordAddition = Random.Range(minWords, maxWords+1);

        currentWords += wordAddition;

        yield return new WaitForSeconds(wordInterval);

        if (currentWords >= wordGoal)
        {
            currentWords = 0;
            slider.value = currentWords;
            countText.SetText("Count: " + currentWords);
            PickNextGoal();
        }
        else
        {
            slider.value = currentWords;
            countText.SetText("Count: " + currentWords);
        }

        StartCoroutine(WriteWords());
    }

    private void PickRandomGoal()
    {
        index = Random.Range(0, goals.Count + 1);
        wordGoal = goals[index];
        slider.maxValue = wordGoal;
        goalText.SetText("Goal: " + wordGoal);
    }

    private void PickNextGoal()
    {
        index++;
        if (index > goals.Count)
        {
            index = 0;
            wordGoal = goals[index];
            slider.maxValue = wordGoal;
            goalText.SetText("Goal: " + wordGoal);
        }
        else
        {
            wordGoal = goals[index];
            slider.maxValue = wordGoal;
            goalText.SetText("Goal: " + wordGoal);
        }
    }
}
