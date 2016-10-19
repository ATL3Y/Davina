using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class HoleObject : MObject {

	Collider col;
	[SerializeField] List<string> matchTagList = new List<string>();
	[SerializeField] float fixInTime = 1f;

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

	/// <summary>
	/// TODO: find a better way to handle the match object algorithm
	/// </summary>
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

	/// <summary>
	/// react to the match object event
	/// try to match the object
	/// </summary>
	/// <param name="arg">Argument.</param>
	void OnMatchObject( LogicArg arg )
	{
		CollectableObj cobj = (CollectableObj) arg.GetMessage (Global.EVENT_LOGIC_MATCH_COBJECT);
		// if the match succeeds
		if (cobj != null && cobj.gameObject == matchObject) {
			// unselect the hold object
			LogicArg logicArg = new LogicArg(this);
			logicArg.AddMessage(Global.EVENT_LOGIC_UNSELECT_COBJECT,cobj);
			M_Event.FireLogicEvent (LogicEvents.UnselectObject, logicArg);

			// change the transform parent and position, scale, rotation of the object
			cobj.transform.parent = transform;
			cobj.transform.DOLocalMove (Vector3.zero, fixInTime).SetEase (Ease.InCirc);
			cobj.transform.DOLocalRotate (Vector3.zero, fixInTime).SetEase (Ease.InCirc);
			cobj.transform.DOScale (1.05f, fixInTime).SetEase (Ease.InCirc);
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






