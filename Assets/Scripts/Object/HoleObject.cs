using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HoleObject : MObject {

	Collider col;
	[SerializeField] List<string> matchTagList = new List<string>();

	protected override void MAwake ()
	{
		base.MAwake ();
		col = GetComponent<Collider> ();
		col.isTrigger = true;
	}

	public override void OnFocus ()
	{
		base.OnFocus ();
	}

	public override void OnOutofFocus ()
	{
		base.OnOutofFocus ();
	}

	GameObject matchObject;
	void OnTriggerEnter(Collider col)
	{
		string tag = col.gameObject.tag;
		if (matchTagList.Contains (tag)) {
			if (SelectObjectManager.Instance.IsSelectObject (col.gameObject)) {
				matchObject = col.gameObject;	

			}
		}
	}

	void OnTriggerExit(Collider col)
	{
		if (matchObject == col.gameObject) {
			matchObject = null;
		}
	}

	void OnMatchObject( LogicArg arg )
	{
		CollectableObj cobj = (CollectableObj) arg.GetMessage (Global.EVENT_LOGIC_MATCH_COBJECT);
		if (cobj != null && cobj.gameObject == matchObject) {
			LogicArg logicArg = new LogicArg(this);
			logicArg.AddMessage(Global.EVENT_LOGIC_UNSELECT_COBJECT,cobj);
			M_Event.FireLogicEvent (LogicEvents.UnselectObject, logicArg);

			cobj.transform.parent = transform;
			cobj.transform.localPosition = Vector3.zero ;
		}
	}

	protected override void MOnEnable ()
	{
		
		base.MOnEnable ();
		M_Event.logicEvents [(int)LogicEvents.MatchObject] += OnMatchObject;
	}

	protected override void MOnDisable ()
	{
		base.MOnDisable ();
		M_Event.logicEvents [(int)LogicEvents.MatchObject] -= OnMatchObject;
	}
}






