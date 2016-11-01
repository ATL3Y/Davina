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
		transform.DOLocalMove (transform.position + new Vector3 (0f, .15f, 0f), 2f).SetEase (Ease.InOutSine);
	}

	void OnLower(LogicArg arg)
	{
		transform.DOLocalMove (transform.position + new Vector3 (0f, -.15f, 0f), 2f).SetEase (Ease.InOutSine);
	}
}
