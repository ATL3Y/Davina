using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ScreenFade : MonoBehaviour
{
    private float alpha;
    private Image fadeImage;
    private float fadeSpeed = 1.5f;
    private Color toColor;
    private bool fading = false;
    private float maxAlpha = 1f;
    private float minAlpha = 0f;

    // called when object enabled 
    void OnEnable()
    {
        M_Event.logicEvents[(int)LogicEvents.RaiseFallingCharacter] += OnRaise;
        M_Event.logicEvents[(int)LogicEvents.LowerFallingCharacter] += OnLower;
        M_Event.logicEvents[(int)LogicEvents.End] += OnEnd;
    }

    void OnDisable()
    {
        M_Event.logicEvents[(int)LogicEvents.RaiseFallingCharacter] -= OnRaise;
        M_Event.logicEvents[(int)LogicEvents.LowerFallingCharacter] -= OnLower;
        M_Event.logicEvents[(int)LogicEvents.End] -= OnEnd;
    }

    // Use this for initialization
    void Start ()
    {
        if (GetComponent<Image>() != null)
        {
            fadeImage = GetComponent<Image>();
            fadeImage.color = new Color(0f, 0f, 0f, maxAlpha / 2f);
        }
        else
        {
            Debug.Log("where's the fade image");
        }
    }

    void SetScreenFade(float score)
    {
        if (score <= 0f)
        {
            alpha = minAlpha;
        }
        else
        {
            alpha = maxAlpha;
        }

        /*
        if (score == 0f)
        {
            alpha = .5f;
        }
        else if (score == 1f)
        {
            alpha = .2f;
        }
        else if (score == 2f)
        {
            alpha = .1f;
        }
        else if (score == 3f)
        {
            alpha = 0f;
        }
        else if (score == -1f)
        {
            alpha = .8f;
        }
        else if (score == -2f)
        {
            alpha = .9f;
        }
        else if (score == -3f)
        {
            alpha = 1f;
        }
        */

        toColor = new Color(0f, 0f, 0f, alpha);
        fading = true;
    }
	
	// Update is called once per frame
	void Update ()
    {
        // fade with score
        if(fading)
        {
            fadeImage.color = Color.Lerp(fadeImage.color, toColor, fadeSpeed * Time.deltaTime); 
            if (fadeImage.color.a == alpha)
            {
                fading = false;
                Debug.Log("fading is false and alpha is " + alpha);
            }
        }
    }

    void OnRaise(LogicArg arg)
    {
        // SetScreenFade(Score.Instance.GetScore());
        toColor = new Color(0f, 0f, 0f, minAlpha);
        fading = true;
    }

    void OnLower(LogicArg arg)
    {
        // SetScreenFade(Score.Instance.GetScore());
        toColor = new Color(0f, 0f, 0f, maxAlpha);
        fading = true;
    }

    void OnEnd(LogicArg arg)
    {
        // the box is hella broken on the bridge so just fade it down 
        toColor = new Color(0f, 0f, 0f, minAlpha);
        fading = true;
    }
}
