using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MatchObj : CollectableObj {

	[SerializeField] MCharacter parent;
	[SerializeField] List<string> matchTags; //tags of the item this will match to
	[SerializeField] LogicEvents onFillRaiseEvent;
	[SerializeField] LogicEvents onFillLowerEvent;


	public override bool Select (ClickType clickType)
	{
		base.Select (clickType);
		return true;
	}

	public override bool UnSelect ()
	{
		base.UnSelect ();
		return true;
	}

	public override void OnFill ()
	{
		base.OnFill ();
		gameObject.layer = 18; //change layer from Hold (17) to Done (18)

		//print ("in match on fill");
		// other option: one "on fill" event, bool on arg for up or down, check bool in MCharacter
		if (gameObject.tag == "Raise") {
			M_Event.FireLogicEvent (onFillRaiseEvent, new LogicArg (this));
			LogicArg logicArg = new LogicArg (this);
			M_Event.FireLogicEvent (LogicEvents.ExitStory, logicArg);
		} else if (gameObject.tag == "Lower") {
			M_Event.FireLogicEvent (onFillLowerEvent, new LogicArg (this));
			LogicArg logicArg = new LogicArg (this);
			M_Event.FireLogicEvent (LogicEvents.ExitStory, logicArg);
		} else if (gameObject.tag == "TutorialRight" || gameObject.tag == "TutorialLeft"){
			LogicArg logicArg = new LogicArg (this);
			M_Event.FireLogicEvent (LogicEvents.ExitStoryTutorial, logicArg);
		}
	}

}
