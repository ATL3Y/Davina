using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StoryObjManager : MBehavior {

	public StoryObjManager() { s_Instance = this; }
	public static StoryObjManager Instance { get { return s_Instance; } }
	private static StoryObjManager s_Instance;

	private int count = 0;
	[SerializeField] List<GameObject> storyObjA;
	[SerializeField] List<GameObject> storyObjB;
	[SerializeField] List<GameObject> storyObjC;

	private List<GameObject> currentStory = new List<GameObject>();

	protected override void MOnEnable(){

		base.MOnEnable ();
		M_Event.logicEvents [(int)LogicEvents.EnterStory] += OnEnterStory;
		M_Event.logicEvents [(int)LogicEvents.ExitStory] += OnExitStory;
	}

	protected override void MOnDisable(){

		base.MOnDisable ();
		M_Event.logicEvents [(int)LogicEvents.EnterStory] -= OnEnterStory;
		M_Event.logicEvents [(int)LogicEvents.ExitStory] -= OnExitStory;
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
		for (int i = 0; i < currentStory.Count; i++) {
			if (currentStory [i].layer.ToString() == "Focus") {
				currentStory [i].SetActive (false);
			}
		}

		//iterate count and enter next story upon exiting last one 
		count++;
		LogicArg logicArg = new LogicArg (this);
		//logicArg.AddMessage (Global.EVENT_LOGIC_ENTERSTORYOBJ);
		M_Event.FireLogicEvent (LogicEvents.EnterStory, logicArg);
	}

	//returns the next batch of story obj
	List<GameObject> GetStory(){
		switch (count) {
		case 0:
			if (storyObjA != null) {
				return storyObjA;
			}
			break;
		case 1:
			if (storyObjB != null) {
				return storyObjB;
			}
			break;
		case 2:
			if (storyObjC != null) {
				return storyObjC;
			}
			break;
		default: 
			return null;
			break;
		}
		return null;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
