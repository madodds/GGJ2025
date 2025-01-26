using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Customer: MonoBehaviour
{
    public string introText;
    public string goodEvalText;
    // TODO: other eval text types depending on performance.

    public Color textColor;

    public List<Sprite> phoneBackgrounds;

    private bool isTalking;

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
        StartCoroutine(DoIntroSpeech(speechLabelElement));

    }

    public void StartEvaluationSpeech(Label speechLabelElement)
    {
        isTalking = true;
        StartCoroutine(DoEvaluationSpeech(speechLabelElement));
    }



    private IEnumerator DoIntroSpeech(Label speechLabelElement)
    {
        speechLabelElement.text = introText;
        yield return new WaitForSeconds(5);
        speechLabelElement.text = "";
        isTalking = false;
    }

    private IEnumerator DoEvaluationSpeech(Label speechLabelElement)
    {
        speechLabelElement.text = goodEvalText;
        yield return new WaitForSeconds(5);
        speechLabelElement.text = "";
        isTalking = false;
    }
}