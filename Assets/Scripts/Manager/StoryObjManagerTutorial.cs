using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StoryObjManagerTutorial : MBehavior {

	private int count = 0;
	[SerializeField] List<GameObject> storyObjA;
	[SerializeField] List<GameObject> storyObjB;
	[SerializeField] List<GameObject> storyObjC;
	[SerializeField] List<GameObject> levelSpecificObjects;

	private List<GameObject> currentStory = new List<GameObject>();

	private bool callOnce = true;

	protected override void MAwake ()
	{
		base.MAwake ();
		currentStory = storyObjA;
		currentStory [0].SetActive (true);

		if (levelSpecificObjects.Count > 0) {
			for (int i = 0; i < levelSpecificObjects.Count; i++) {
				levelSpecificObjects [i].SetActive (true);
			}
		}
	}

	protected override void MOnEnable(){

		base.MOnEnable ();
		M_Event.logicEvents [(int)LogicEvents.EnterStoryTutorial] += OnEnterStoryTutorial;
		M_Event.logicEvents [(int)LogicEvents.ExitStoryTutorial] += OnExitStoryTutorial;
		M_Event.logicEvents [(int)LogicEvents.Credits] += OnCredits;
		M_Event.logicEvents [(int)LogicEvents.Heard] += OnHeard;
	}

	protected override void MOnDisable(){

		base.MOnDisable ();
		M_Event.logicEvents [(int)LogicEvents.EnterStoryTutorial] -= OnEnterStoryTutorial;
		M_Event.logicEvents [(int)LogicEvents.ExitStoryTutorial] -= OnExitStoryTutorial;
		M_Event.logicEvents [(int)LogicEvents.Credits] -= OnCredits;
		M_Event.logicEvents [(int)LogicEvents.Heard] -= OnHeard;
	}

	void OnEnterStoryTutorial(LogicArg arg)
	{
		//iterate count and enter next story upon exiting last one 
		count++;
		if (GetStory () == null && callOnce) {
			LogicArg logicArg = new LogicArg (this);
			M_Event.FireLogicEvent (LogicEvents.Characters, logicArg);
			callOnce = false;
			return;
		} 
		
		currentStory.Clear ();
		currentStory = GetStory ();
		currentStory [0].SetActive (true);
		/*
		for (int i = 0; i < currentStory.Count; i++) {
			currentStory [i].SetActive (true);
		}
		*/
	}

	void OnExitStoryTutorial(LogicArg arg)
	{
		for (int i=currentStory.Count-1; i>=0; i--) {
			NiceCollectable niceCollectable = currentStory [i].GetComponent<NiceCollectable>();
			if (niceCollectable != null && niceCollectable != (NiceCollectable)arg.sender) {
				//Debug.Log( "in StoryManTutorial deactivating " + currentStory[ i ].name );
				currentStory [i].SetActive (false);
			}
		}
	}

	void OnHeard(LogicArg arg){
		for (int i = 1; i < currentStory.Count; i++) {
			currentStory [i].SetActive (true);
		}
	}

	void MUpdate(){
		base.MUpdate ();

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
			if (storyObjC.Count > 0) {
				return storyObjC;
			} else {
				return null;
			}
		default:
			return null;
		}
	}

	public void OnCredits(LogicArg arg)
	{
		//disable all so the space is clear for credits
		for ( int i = levelSpecificObjects.Count - 1; i >= 0; i-- )
		{
			levelSpecificObjects[ i ].SetActive( false );
		}

		if (storyObjA != null) {
			for (int i = storyObjA.Count - 1; i >= 0; i--) {
				storyObjA [i].SetActive (false);
			}
		}
		if (storyObjB != null) {
			for (int i = storyObjB.Count - 1; i >= 0; i--) {
				storyObjB [i].SetActive (false);
			}
		}
		if (storyObjC != null) {
			for (int i = storyObjC.Count - 1; i >= 0; i--) {
				storyObjC [i].SetActive (false);
			}
		}
	}
		
}

/*
 * 	//exit last story before entering new one 
void OnExitStoryTutorial(LogicArg arg){
	//disable remaining objects in mother
	//Debug.Log("length of current story obj = " + currentStory.Count);
	for (int i=currentStory.Count-1; i>=0; i--) {
		//if (currentStory [i].layer == LayerMask.NameToLayer( "Focus" )) { 
		//CollectableObj cobj = currentStory[i].GetComponent<CollectableObj>();
		//if (cobj != null && !cobj.matched ) { 
		//Debug.Log( "in StoryManTutorial deactivating " + currentStory[ i ].name );
		currentStory [i].SetActive (false);
		//}
	}

	//iterate count and enter next story upon exiting last one 
	count++;
	if (GetStory () != null) {
		//Debug.Log( "in StoryManCharacters enter next story " );
		LogicArg logicArg = new LogicArg (this);
		M_Event.FireLogicEvent (LogicEvents.EnterStoryTutorial, logicArg);
	} else if (GetStory () == null && callOnce) {
		LogicArg logicArg = new LogicArg( this );
		M_Event.FireLogicEvent( LogicEvents.Characters, logicArg );
		callOnce = false;
	}
}
*/
