using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ScreenFade : MonoBehaviour
{
    private float alpha;
    private float newAlpha;
    private Image fadeImage;
    private float fadeSpeed = 1.5f;
    private Color toColor;

    // called when object enabled 
    void OnEnable()
    {
        M_Event.logicEvents[(int)LogicEvents.End] += OnEnd;
        M_Event.logicEvents [ ( int ) LogicEvents.Credits ] += OnCredits;
    }

    void OnDisable()
    {
        M_Event.logicEvents[(int)LogicEvents.End] -= OnEnd;
        M_Event.logicEvents [ ( int ) LogicEvents.Credits ] -= OnCredits;
    }

    // Use this for initialization
    void Start ()
    {
        alpha = 0.5f;
        if (GetComponent<Image>() != null)
        {
            fadeImage = GetComponent<Image>();
            fadeImage.color = new Color(0f, 0f, 0f, alpha);
        }
        else
        {
            Debug.Log("where's the fade image");
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        // See if a current lens is active and get it
        Lens currentLens = Lens.instance; 
        if(currentLens != null )
        {
            alpha = ( currentLens.Dot + 1.0f ) / 2.0f;
        }

        toColor = new Color ( 0f, 0f, 0f, alpha );

        fadeImage.color = Color.Lerp(fadeImage.color, toColor, fadeSpeed * Time.deltaTime);
    }

    void OnEnd(LogicArg arg)
    {
        newAlpha = 0.0f;
    }

    void OnCredits ( LogicArg arg )
    {
        newAlpha = 1.0f;
    }
}
