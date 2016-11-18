using UnityEngine;
using System.Collections;
using DG.Tweening;

public class RaiseLower : MonoBehaviour {

	//GameObject newParent;
	float t = 0f;
	private bool finale = false;

	// called when object enabled 
	void OnEnable()
	{
		M_Event.logicEvents [(int)LogicEvents.RaiseFallingCharacter] += OnRaise;
		M_Event.logicEvents [(int)LogicEvents.LowerFallingCharacter] += OnLower;
        M_Event.logicEvents[ ( int )LogicEvents.Finale ] += OnFinale;

		//test
		/*
		newParent = new GameObject ();
		newParent.transform.position = transform.position;
		newParent.transform.rotation = transform.rotation;
		newParent.transform.localScale = transform.localScale;
		transform.SetParent (newParent.transform);
		height = newParent.transform.position.y;
		*/
    }

	void OnDisable()
	{
		M_Event.logicEvents [(int)LogicEvents.RaiseFallingCharacter] -= OnRaise;
		M_Event.logicEvents [(int)LogicEvents.LowerFallingCharacter] -= OnLower;
        M_Event.logicEvents[ ( int )LogicEvents.Finale ] -= OnFinale;
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

    protected void Update( )
    {
		if (GetComponent<MCharacter> ().IsInInnerWorld) {
			t = 0f;
		}

		if ( finale && !GetComponent<MCharacter>().IsInInnerWorld)
        {
            //transform.position = new Vector3( transform.position.x, height, transform.position.z );
			//could acc height based on controller distance squared
			//Vector3 target = new Vector3 (newParent.transform.position.x, LogicManager.Instance.GetPlayerTransform ().position.y + .7f, newParent.transform.position.z); //+ Mathf.Sin(Time.timeSinceLevelLoad / 1000f)
			Vector3 target = new Vector3 (transform.position.x, LogicManager.Instance.GetPlayerTransform ().position.y + 1f, transform.position.z);

			if (t < 1f) {
				t += Time.deltaTime / 500f;
			}
			//transform.position = Vector3.Lerp (newParent.transform.position, target, t);
			transform.position = Vector3.Lerp (transform.position, target, t);
        }
    }

    void OnFinale( LogicArg arg )
    {
		// raise a new parent that won't be affected by change scale
		/*
		newParent = new GameObject ();
		newParent.transform.position = transform.position;
		newParent.transform.rotation = transform.rotation;
		newParent.transform.localScale = transform.localScale;
		transform.SetParent (newParent.transform);
		height = newParent.transform.position.y;
		*/
		finale = true;
    }
}
