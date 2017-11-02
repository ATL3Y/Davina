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
    //private float maxAlpha = 1f;
    //private float minAlpha = 0f;

    // called when object enabled 
    void OnEnable()
    {
        //M_Event.logicEvents[(int)LogicEvents.RaiseFallingCharacter] += OnRaise;
        //M_Event.logicEvents[(int)LogicEvents.LowerFallingCharacter] += OnLower;
        M_Event.logicEvents[(int)LogicEvents.End] += OnEnd;
    }

    void OnDisable()
    {
        //M_Event.logicEvents[(int)LogicEvents.RaiseFallingCharacter] -= OnRaise;
        //M_Event.logicEvents[(int)LogicEvents.LowerFallingCharacter] -= OnLower;
        M_Event.logicEvents[(int)LogicEvents.End] -= OnEnd;
    }

    // Use this for initialization
    void Start ()
    {
        if (GetComponent<Image>() != null)
        {
            fadeImage = GetComponent<Image>();
            fadeImage.color = new Color(0f, 0f, 0f, 1.0f);
        }
        else
        {
            Debug.Log("where's the fade image");
        }
    }
    /*
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

        toColor = new Color(0f, 0f, 0f, alpha);
    }
    */
	
	// Update is called once per frame
	void Update ()
    {
        // See if a current lens is active and get it
        Lens currentLens = Lens.instance; //LogicManager.Instance.GetCurrentLens();
        if(currentLens != null )
        {
            alpha = ( currentLens.Dot + 1.0f ) / 2.0f;
            // Debug.Log ( "alpha is " + alpha );
        }
        // Otherwise look average
        else
        {
            // Debug.Log ( "currentLens is null." );
            alpha = 0.5f;
        }

        toColor = new Color ( 0f, 0f, 0f, alpha );

        fadeImage.color = Color.Lerp(fadeImage.color, toColor, fadeSpeed * Time.deltaTime);
        /* 
        if (fadeImage.color.a == alpha)
        {
            Debug.Log("fading is false and alpha is " + alpha);
        }
        */
    }

    /*
    void OnRaise(LogicArg arg)
    {
        // SetScreenFade(Score.Instance.GetScore());
        toColor = new Color(0f, 0f, 0f, minAlpha);
    }

    void OnLower(LogicArg arg)
    {
        // SetScreenFade(Score.Instance.GetScore());
        toColor = new Color(0f, 0f, 0f, maxAlpha);
    }
    */

    void OnEnd(LogicArg arg)
    {
        // the box is hella broken on the bridge so just fade it down 
        toColor = new Color(0f, 0f, 0f, alpha);
    }
}
