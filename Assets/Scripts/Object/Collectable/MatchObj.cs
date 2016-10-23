using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MatchObj : CollectableObj {

	[SerializeField] MCharacter parent;
	[SerializeField] List<string> matchTags;

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

}
