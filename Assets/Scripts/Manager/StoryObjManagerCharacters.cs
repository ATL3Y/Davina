using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StoryObjManagerCharacters : MBehavior {

	private int count = -1;
	[SerializeField] List<GameObject> storyObjA;
	[SerializeField] List<GameObject> storyObjB;
	[SerializeField] List<GameObject> storyObjC;
	[SerializeField] List<GameObject> levelSpecificObjects;
	[SerializeField] List<GameObject> disableOnFinaleObjects;

	private List<GameObject> currentStory = new List<GameObject>();

	protected override void MAwake ()
	{
		base.MAwake ();
		/*
		currentStory = storyObjA;
		for (int i = 0; i < currentStory.Count; i++) {
			currentStory [i].SetActive (true);
		}
		*/

		if (levelSpecificObjects.Count > 0) {
			for (int i = 0; i < levelSpecificObjects.Count; i++) {
				levelSpecificObjects [i].SetActive (false);
			}
		}
		if (disableOnFinaleObjects.Count > 0) {
			for (int i = 0; i < disableOnFinaleObjects.Count; i++) {
				disableOnFinaleObjects [i].SetActive (false);
			}
		}
	}

	protected override void MOnEnable(){

		base.MOnEnable ();
		M_Event.logicEvents [(int)LogicEvents.EnterStory] += OnEnterStory;
		M_Event.logicEvents [(int)LogicEvents.ExitStory] += OnExitStory;
		M_Event.logicEvents [(int)LogicEvents.Characters] += OnCharacters;
		M_Event.logicEvents [(int)LogicEvents.Finale] += OnFinale;
        M_Event.logicEvents [(int)LogicEvents.End] += OnEnd;
        M_Event.logicEvents[ ( int )LogicEvents.Credits ] += OnCredits;
    }

	protected override void MOnDisable(){

		base.MOnDisable ();
		M_Event.logicEvents [(int)LogicEvents.EnterStory] -= OnEnterStory;
		M_Event.logicEvents [(int)LogicEvents.ExitStory] -= OnExitStory;
		M_Event.logicEvents [(int)LogicEvents.Characters] -= OnCharacters;
		M_Event.logicEvents [(int)LogicEvents.Finale] += OnFinale;
        M_Event.logicEvents [(int)LogicEvents.End] -= OnEnd;
        M_Event.logicEvents[ ( int )LogicEvents.Credits ] -= OnCredits;
    }

	void OnEnterStory(LogicArg arg)
	{
		count++;
		if (GetStory () == null) {
			return;
		} 

		currentStory.Clear ();
		currentStory = GetStory ();
		for (int i = 0; i < currentStory.Count; i++) {
			currentStory [i].SetActive (true);
		}
	}

	//exit last story before entering new one 
	void OnExitStory(LogicArg arg)
	{
		for (int i=currentStory.Count-1; i>=0; i--) {

			NiceCollectable niceCollectable = currentStory [i].GetComponent<NiceCollectable>();
			if (niceCollectable != null && niceCollectable != (NiceCollectable)arg.sender) {
				//Debug.Log( "in StoryManTutorial deactivating " + currentStory[ i ].name );
				currentStory [i].SetActive (false);
			}
		}
	}

	//returns the next batch of story obj
	List<GameObject> GetStory()
	{
		switch (count) {
		case 0:
			if (storyObjA.Count > 0) {
				return storyObjA;
			} else {
				return null;
			}
		case 1:
			if (storyObjB.Count > 0) {
				return storyObjB;
			} else {
				return null;
			}
		case 2:
			if (storyObjC.Count >0) {
                return storyObjC;
			} else {
				print ("firing finale");
				LogicArg logicArg = new LogicArg( this );
				M_Event.FireLogicEvent( LogicEvents.Finale, logicArg );
				return null;
			}
		default:
			return null;
		}
	}

	void OnCharacters( LogicArg arg ){
		for (int i = 0; i < levelSpecificObjects.Count; i++) {
			levelSpecificObjects [i].SetActive (true);
		}
		for (int i = 0; i < disableOnFinaleObjects.Count; i++) {
			disableOnFinaleObjects [i].SetActive (true);
		}
		M_Event.FireLogicEvent( LogicEvents.EnterStory, new LogicArg( this ) );
	}

	void OnFinale( LogicArg arg ){
		for (int i = disableOnFinaleObjects.Count - 1; i >=0; i--) {
			disableOnFinaleObjects [i].SetActive (false);
		}
	}
		
	void OnEnd( LogicArg arg )
	{
		for (int i = levelSpecificObjects.Count - 1; i >=0; i--) {
			levelSpecificObjects [i].SetActive (false);
		}
		//disable the trails
		for ( int i = currentStory.Count - 1; i >= 0; i-- )
		{
			currentStory[ i ].SetActive( false );
		}
	}

    void OnCredits( LogicArg arg )
    {

    }		
}
