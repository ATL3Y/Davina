using UnityEngine;
using System.Collections;
using DG.Tweening;

public class RaiseLower : MonoBehaviour {

	// called when object enabled 
	void OnEnable()
	{
		M_Event.logicEvents [(int)LogicEvents.RaiseFallingCharacter] += OnRaise;
		M_Event.logicEvents [(int)LogicEvents.LowerFallingCharacter] += OnLower;
	}

	void OnDisable()
	{
		M_Event.logicEvents [(int)LogicEvents.RaiseFallingCharacter] -= OnRaise;
		M_Event.logicEvents [(int)LogicEvents.LowerFallingCharacter] -= OnLower;
	}

	void OnRaise( LogicArg arg )
	{
		//something obvious here
		transform.DOLocalMove (transform.position + new Vector3 (0f, .3f, 0f), 1f).SetEase (Ease.InCirc);

		/*
		bool isUp = (bool)arg.GetMessage ("isUp");

		if (isUp) {
			transform.position += new Vector3 (0f, .2f, 0f);
		} else {
			transform.position -= new Vector3 (0f, .2f, 0f);
		}
		Debug.Log ( name + " Raise the character " + isUp);
		*/
	}

	void OnLower(LogicArg arg)
	{
		transform.DOLocalMove (transform.position + new Vector3 (0f, -.3f, 0f), 1f).SetEase (Ease.InCirc);
	}
}
