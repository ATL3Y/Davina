using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MatchObj : CollectableObj {

	[SerializeField] MCharacter parent;
	[SerializeField] List<string> matchTags;

	public override bool Select ()
	{
		Debug.Log ("Select");
		base.Select ();
		return true;
	}

	public override bool UnSelect ()
	{
		base.UnSelect ();
		return true;
	}

}
