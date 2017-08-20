using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour {
    public Score() { s_Instance = this; }
    public static Score Instance { get { return s_Instance; } }
    private static Score s_Instance;

    private float _score = 0.0f;
    Light[] lights;

    // Use this for initialization
    void Start()
    {
        lights = FindObjectsOfType<Light>();
    }
	
	// Update is called once per frame
	void Update()
    {
		
	}

    public void ForceSetScore(float newScore)
    {
        _score = newScore;
    }

    public void SetScore(float add) // name this better. Somehow, -1 is light and 1 is dark 
    {
        _score += add;

        /*
        for(int i = 0; i < lights.Length; i++)
        {
            lights[i].intensity -= add / 5.0f;
        }
        */

        Debug.Log("new score is " + _score);
    }

    public float GetScore()
    {
        return _score;
    }

    public void OnEnable()
    {
        M_Event.logicEvents[(int)LogicEvents.Tutorial] += OnTutorial;
        M_Event.logicEvents[(int)LogicEvents.Characters] += OnCharacters;
    }

    public void OnDisable()
    {
        M_Event.logicEvents[(int)LogicEvents.Tutorial] -= OnTutorial;
        M_Event.logicEvents[(int)LogicEvents.Characters] -= OnCharacters;
    }

    void OnTutorial(LogicArg arg)
    {
        _score = 0.0f;
    }

    void OnCharacters(LogicArg arg)
    {
        //reset
        _score = 0.0f;
    }
}
