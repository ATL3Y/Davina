using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StoryObjManagerEnd : MBehavior {

	private int count = 0;
	[SerializeField] List<GameObject> levelSpecificObjects;

	private bool callOnce = true;

	protected override void MAwake ()
	{
		base.MAwake ();

		if (levelSpecificObjects.Count > 0) {
			for (int i = 0; i < levelSpecificObjects.Count; i++) {
				levelSpecificObjects [i].SetActive (false);
			}
		}
	}

	protected override void MOnEnable(){

		base.MOnEnable ();
		M_Event.logicEvents [(int)LogicEvents.End] += OnEnd;
		M_Event.logicEvents [(int)LogicEvents.Credits] += OnCredits;

	}

	protected override void MOnDisable(){

		base.MOnDisable ();
		M_Event.logicEvents [(int)LogicEvents.End] -= OnEnd;
		M_Event.logicEvents [(int)LogicEvents.Credits] -= OnCredits;

	}

	void OnEnd( LogicArg arg ){
		for (int i = 0; i < levelSpecificObjects.Count; i++) {
			levelSpecificObjects [i].SetActive (true);
		}
	}

	void OnCredits(LogicArg arg){
        for ( int i = levelSpecificObjects.Count - 1; i >= 0; i-- )
        {
            levelSpecificObjects[ i ].SetActive( false );
        }
    }
		
}
