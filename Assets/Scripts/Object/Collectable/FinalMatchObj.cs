using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FinalMatchObj : CollectableObj {

	public override bool Select (ClickType clickType)
	{
		//base.Select (clickType);

		if ( selectSoundSource != null )
			selectSoundSource.Play ();
		if ( storySoundSource != null )
			storySoundSource.Play ();

		if (matched) {
			//fires match object event on pressing trigger instead of unselect
			LogicArg logicArg = new LogicArg (this);
			logicArg.AddMessage(Global.EVENT_LOGIC_MATCH_COBJECT, this);
			M_Event.FireLogicEvent (LogicEvents.MatchObject, logicArg);
			Debug.Log ("match sent from FinalMatchObj");
			return true;
		}
		
		return false;
	}

	public override bool UnSelect ()
	{
		//base.UnSelect ();
		// play the sound effect
		if ( unselectSoundSource != null )
			unselectSoundSource.Play ();

		if ( storySoundSource != null && storySoundSource.isPlaying)
			storySoundSource.Stop ();


		return true;
	}

	public override void OnFill ()
	{
		//base.OnFill ();

		//gameObject.layer = 18; //change layer from Hold (17) to Done (18)

		//LogicArg logicArg = new LogicArg (this);
		//logicArg.AddMessage (Global.EVENT_LOGIC_EXITSTORYOBJ);
		//M_Event.FireLogicEvent (LogicEvents.ExitStory, logicArg);
	}

}

