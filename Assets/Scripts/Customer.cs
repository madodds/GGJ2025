using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Customer: MonoBehaviour
{
    public string introText;
    public string goodEvalText;
    // TODO: other eval text types depending on performance.

    public float speechPeriodS = 0.1f;  // Number of seconds to wait between speech characters/segments
    public Color textColor;

    public List<Sprite> phoneBackgrounds;

    private bool isTalking;

    private float preSpeechDelayS = 2.5f;
    private float afterSpeechDelayS = 2.5f;

    void Start()
    {
        isTalking = false;
    } 

    public bool IsTalking()
    {
        return isTalking;
    }

    public void StartIntroSpeech(Label speechLabelElement)
    {
        isTalking = true;
        StartCoroutine(DoSpeech(speechLabelElement, introText));

    }

    public void StartEvaluationSpeech(Label speechLabelElement)
    {
        isTalking = true;
        StartCoroutine(DoSpeech(speechLabelElement, goodEvalText));
    }


    // Ouput the contents of speechText to speechLabelElement in
    // an animated manner.
    private IEnumerator DoSpeech(Label speechLabelElement, string speechText)
    {
        yield return new WaitForSeconds(preSpeechDelayS);
        speechLabelElement.style.color = textColor;
        string speechOutput;
        for(int i=0; i<=speechText.Length; i++)
        {
            speechOutput = speechText.Substring(0, i);
            speechLabelElement.text = speechOutput;
            yield return new WaitForSeconds(speechPeriodS);
        }
        yield return new WaitForSeconds(afterSpeechDelayS);
        speechLabelElement.text = "";
        isTalking = false;
    }
}