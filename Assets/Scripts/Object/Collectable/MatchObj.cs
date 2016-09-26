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
//		SelectObjectManager.AttachToStayPasserBy (transform);
		return true;
	}

	public override bool MatchWithOtherObject ( MObject mobj )
	{
		if ( mobj != null && matchSensor) {
			transform.SetParent (mobj.transform);
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			return true;
		}
		return false;
	}

	bool matchSensor = false;
	void OnTriggerEnter( Collider col )
	{
		if (matchTags.Contains (col.gameObject.tag)) {
			matchSensor = true;
		}
	}

	void OnTriggerExit( Collider col )
	{
		if (matchTags.Contains (col.gameObject.tag)) {
			matchSensor = false;
		}
	}

}
