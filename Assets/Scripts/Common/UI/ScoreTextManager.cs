using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public struct ScoreDisplayValues
{
    public int currentValue;
    public int oldValue;
    public ScoreDisplayValues(int currentValue, int oldValue = 0)
    {
        this.currentValue = currentValue;
        this.oldValue = oldValue;
    }
}

public class ScoreTextManager : MonoBehaviour
{
    TextMeshProUGUI scoreText;

    public float scoreUpdateTime = 0.1f;
    public int scoreUpdateStep = 2;

    public bool useAdaptiveStepping = false;
    public float adaptiveSteppingMaxTime = 1.0f;
    public int minimumScoreUpdateStep = 1;

    private float oldScore;

    // Start is called before the first frame update
    void Start()
    {
        scoreText = GetComponent<TextMeshProUGUI>();
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DisplayScore(float score)
    {
        oldScore = PlayerPrefs.GetInt("oldScore", 0);
        DisplayScore(score, oldScore);
        PlayerPrefs.SetInt("oldScore", (int)score);

    }

    public void DisplayScore(float score, float oldScore)
    {
        if (useAdaptiveStepping)
        {
            int numberOfSteps = (int)(adaptiveSteppingMaxTime / scoreUpdateTime);
            scoreUpdateStep = (int)((score - oldScore) / (float)numberOfSteps);
            if (scoreUpdateStep == 0)
            {
                scoreUpdateStep = minimumScoreUpdateStep;
            }
        }

        StartCoroutine(nameof(DisplayScoreCoroutine), new ScoreDisplayValues((int)score, (int)oldScore));
    }

    IEnumerator DisplayScoreCoroutine(ScoreDisplayValues values)
    {

        if(scoreText == null) scoreText = GetComponent<TextMeshProUGUI>();

        for (int countingValue = values.oldValue; countingValue <= values.currentValue; countingValue += scoreUpdateStep)
        {
            scoreText.text = "" + countingValue;
            yield return new WaitForSeconds(scoreUpdateTime);
        }
        scoreText.text = "" + values.currentValue;

    }

}
