using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MatchObj : CollectableObj {

	[SerializeField] MCharacter parent;
	[SerializeField] List<string> matchTags;
	[SerializeField] LogicEvents onFillEvent;

	public override bool Select (ClickType clickType)
	{
		Debug.Log ("Select");
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

		M_Event.FireLogicEvent (onFillEvent, new LogicArg (this));

		///LogicManager.Instance.
	}

}
