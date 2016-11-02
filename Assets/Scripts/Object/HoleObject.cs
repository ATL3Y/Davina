using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class HoleObject : MObject {

	Collider col;
	[SerializeField] List<string> matchTagList = new List<string>();
	[SerializeField] float fixInTime = 1f;

	// i want the holes to have an outline too
	[SerializeField] MeshRenderer[] outlineRenders;

	protected override void MAwake ()
	{
		base.MAwake ();
		col = GetComponent<Collider> ();
		col.isTrigger = true;
	}

	public override void OnFocus ()
	{
		base.OnFocus ();
		SetOutline (true);
	}

	public override void OnOutofFocus ()
	{
		base.OnOutofFocus ();
		SetOutline (false);
	}

	/// <summary>
	/// Set the outline render on or off(enable)
	/// </summary>
	/// <param name="isOn">If set to <c>true</c> is on.</param>
	protected void SetOutline( bool isOn )
	{
		foreach (MeshRenderer r in outlineRenders) {
			r.enabled = isOn;
		}
	}

	/// <summary>
	/// Shake this object 
	/// </summary>
	protected void Shake( )
	{
		Vector3 strength = new Vector3 (50f, 50f, 0f); // scale of 0-1
		transform.DOShakeRotation(1f, strength, 10, 40f, true);
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

	/// <summary>
	/// TODO: find a better way to handle the match object algorithm
	/// </summary>
	GameObject matchObject;
	protected virtual void OnTriggerEnter(Collider col)
	{
		string tag = col.gameObject.tag;
		if (matchTagList.Contains (tag) && SelectObjectManager.Instance.IsSelectObject (col.gameObject)) {
			matchObject = col.gameObject;	
			CollectableObj cobj = matchObject.GetComponent<CollectableObj> ();
			if (cobj != null) {
				// make it so the next click will not trigger Unselect's transform change in CollectableObject
				cobj.matched = true;
			} 
		} else if (tag == "GameController" && matchObject == null) {
			bool shake = true;
			foreach (Transform child in col.gameObject.transform) {
				if (child.gameObject.layer.ToString() == "17") { // 17 is "Hold"
					shake = false;
				}
			}
			if (shake) {
				Shake ();
			}
		}
	}

	protected virtual void OnTriggerExit(Collider col)
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
	protected virtual void OnMatchObject( LogicArg arg )
	{
		//CollectableObj cobj = matchObject.GetComponent<CollectableObj>();
		CollectableObj cobj = (CollectableObj) arg.GetMessage (Global.EVENT_LOGIC_MATCH_COBJECT);

		// if the match succeeds
		if (cobj != null && cobj.gameObject == matchObject) {
			// vibrate the controller holding the matchObject

			if (cobj.transform.gameObject.name == "Controller (left)") {
				InputManager.Instance.VibrateController (ViveInputController.Instance.leftControllerIndex);
			} else {
				InputManager.Instance.VibrateController (ViveInputController.Instance.rightControllerIndex);
			}

			// change the transform parent and position, scale, rotation of the object
			cobj.transform.parent = transform; //.parent;
			cobj.transform.DOLocalMove (Vector3.zero, fixInTime).SetEase (Ease.InCirc);
			cobj.transform.DOLocalRotate (Vector3.zero, fixInTime).SetEase (Ease.InCirc);
			cobj.transform.DOScale (1.02f, fixInTime).SetEase (Ease.InCirc);

			gameObject.layer = 18; //change layer from Focus (16) to Done (18)

			// tell the object it is filled in the hole
			cobj.OnFill ();

			//this.gameObject.SetActive (false);
		}
	}
}






