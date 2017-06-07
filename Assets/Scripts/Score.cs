using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour {
    public Score() { s_Instance = this; }
    public static Score Instance { get { return s_Instance; } }
    private static Score s_Instance;

    private float _score = .75f;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetScore(float add)
    {
        _score += add;
    }

    public float GetScore()
    {
        return _score;
    }

    public void OnEnable()
    {
        M_Event.logicEvents[(int)LogicEvents.Characters] += OnCharacters;
        M_Event.logicEvents[(int)LogicEvents.Finale] += OnFinale;
        M_Event.logicEvents[(int)LogicEvents.End] += OnEnd;
    }

    public void OnDisable()
    {
        M_Event.logicEvents[(int)LogicEvents.Characters] -= OnCharacters;
        M_Event.logicEvents[(int)LogicEvents.Finale] -= OnFinale;
        M_Event.logicEvents[(int)LogicEvents.End] -= OnEnd;
    }

    void OnCharacters(LogicArg arg)
    {
        //reset
        _score = .75f;
    }

    void OnFinale(LogicArg arg)
    {

    }

    void OnEnd(LogicArg arg)
    {

    }
}
