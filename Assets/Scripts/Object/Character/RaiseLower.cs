using UnityEngine;
using System.Collections;
using DG.Tweening;

public class RaiseLower : MonoBehaviour {

    private float height = -1f;

	// called when object enabled 
	void OnEnable()
	{
		M_Event.logicEvents [(int)LogicEvents.RaiseFallingCharacter] += OnRaise;
		M_Event.logicEvents [(int)LogicEvents.LowerFallingCharacter] += OnLower;
        M_Event.logicEvents[ ( int )LogicEvents.Finale ] += OnFinale;
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

        if ( height > 0f && height < 100f )
        {
            height += .001f;
            transform.position = new Vector3( transform.position.x, height, transform.position.z );
        }

    }

    void OnFinale( LogicArg arg )
    {
        height = transform.position.y;

    }
}
