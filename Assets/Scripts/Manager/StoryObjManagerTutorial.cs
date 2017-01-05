using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StoryObjManagerTutorial : MBehavior {

	private int count = -1;
	[SerializeField] List<GameObject> storyObjA;
	[SerializeField] List<GameObject> storyObjB;
	[SerializeField] List<GameObject> storyObjC;
	[SerializeField] List<GameObject> levelSpecificObjects;

	private List<GameObject> currentStory = new List<GameObject>();

	private bool callOnce = true;

	protected override void MAwake ()
	{
		base.MAwake ();
		//currentStory = storyObjA;
		//currentStory [0].SetActive (true);

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
	}

	protected override void MOnDisable(){

		base.MOnDisable ();
		M_Event.logicEvents [(int)LogicEvents.EnterStoryTutorial] -= OnEnterStoryTutorial;
		M_Event.logicEvents [(int)LogicEvents.ExitStoryTutorial] -= OnExitStoryTutorial;
		M_Event.logicEvents [(int)LogicEvents.Credits] -= OnCredits;
	}

	//first call is by LogicManager
	void OnEnterStoryTutorial(LogicArg arg)
	{
		//iterate count and enter next story upon exiting last one 
		count++;
		//if there is no next story, call next chapter
		if (GetStory () == null && callOnce) {
			M_Event.FireLogicEvent (LogicEvents.Characters, new LogicArg (this));
			callOnce = false;
			return;
		} 
		
		currentStory.Clear ();
		currentStory = GetStory ();
		//currentStory [0].SetActive (true);

		for (int i = 0; i < currentStory.Count; i++) {
			currentStory [i].SetActive (true);
		}

	}
	//not in use
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
	
