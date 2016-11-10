using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StoryObjManagerCharacters : MBehavior {

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

		if (levelSpecificObjects.Count > 0) {
			for (int i = 0; i < levelSpecificObjects.Count; i++) {
				levelSpecificObjects [i].SetActive (false);
			}
		}
	}

	protected override void MOnEnable(){

		base.MOnEnable ();
		M_Event.logicEvents [(int)LogicEvents.EnterStory] += OnEnterStory;
		M_Event.logicEvents [(int)LogicEvents.ExitStory] += OnExitStory;
		M_Event.logicEvents [(int)LogicEvents.Characters] += OnCharacters;
		M_Event.logicEvents [(int)LogicEvents.End] += OnEnd;
	}

	protected override void MOnDisable(){

		base.MOnDisable ();
		M_Event.logicEvents [(int)LogicEvents.EnterStory] -= OnEnterStory;
		M_Event.logicEvents [(int)LogicEvents.ExitStory] -= OnExitStory;
		M_Event.logicEvents [(int)LogicEvents.Characters] -= OnCharacters;
		M_Event.logicEvents [(int)LogicEvents.End] -= OnEnd;
	}

	void OnEnterStory(LogicArg arg){
		
		currentStory.Clear ();
		currentStory = GetStory ();
		if (currentStory != null) {
			for (int i = 0; i < currentStory.Count; i++) {
				currentStory [i].SetActive (true);
			}
		}
	}

	//exit last story before entering new one 
	void OnExitStory(LogicArg arg){
		//disable remaining objects in mother
		//Debug.Log("length of current story obj = " + currentStory.Count);
		for (int i=currentStory.Count-1; i>=0; i--) {
			if (currentStory [i].layer == 16) { // Focus is layer 16
				currentStory [i].SetActive (false);
			}
		}

		//iterate count and enter next story upon exiting last one 
		count++;
		if (GetStory () != null) {
			LogicArg logicArg = new LogicArg (this);
			M_Event.FireLogicEvent (LogicEvents.EnterStory, logicArg);
		} else if (GetStory () == null && callOnce) {
			StartCoroutine (ToEndDelay (5f));
			callOnce = false;
		}
	}

	//returns the next batch of story obj
	List<GameObject> GetStory(){
		switch (count) {
		case 0:
			if (storyObjA.Count > 0) {
				return storyObjA;
			} else {
				return null;
			}
			break;
		case 1:
			if (storyObjB.Count > 0) {
				return storyObjB;
			} else {
				return null;
			}
			break;
		case 2:
			if (storyObjC.Count > 0) {
				return storyObjC;
			} else {
				return null;
			}
			break;
		default:
			print ("default");
			return null;
			break;
		}
		return null;
	}

	void OnCharacters( LogicArg arg ){
		for (int i = 0; i < levelSpecificObjects.Count; i++) {
			levelSpecificObjects [i].SetActive (true);
		}
	}
		
	void OnEnd( LogicArg arg ){
		for (int i = 0; i < levelSpecificObjects.Count; i++) {
			levelSpecificObjects [i].SetActive (false);
		}
	}

	IEnumerator ToEndDelay( float delay ){
		yield return new WaitForSeconds (delay);
		LogicArg logicArg = new LogicArg (this);
		M_Event.FireLogicEvent (LogicEvents.End, logicArg);
	}
}
