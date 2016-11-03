using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class FinalHoleObject : MObject
{
	GameObject toMatchObject;
	[SerializeField] GameObject trail;

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

		if (gameObject.name == "Cylinder (left)") {
			toMatchObject = GameObject.Find ("Controller (left)");
		} else {
			toMatchObject = GameObject.Find ("Controller (right)");
		}

		/* 
		FinalMatchObj myComponent = toMatchObject.GetComponent<FinalMatchObj> ();
		myComponent.enabled = true;
		GameObject newGameObj = Instantiate (trail);
		newGameObj.transform.localPosition = new Vector3 (0f, 0f, .2f);
		newGameObj.transform.localRotation = Quaternion.identity;
		newGameObj.transform.localScale = Vector3.zero;
		newGameObj.transform.SetParent (toMatchObject.transform);
		*/
	}
		
	/// <summary>
	/// Call when the input manager checked that the object is on focus
	/// </summary>
	public override void OnFocus()
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
		M_Event.logicEvents [(int)LogicEvents.End] += OnEnd; 

	}

	protected override void MOnDisable ()
	{
		base.MOnDisable ();
		M_Event.logicEvents [(int)LogicEvents.End] -= OnEnd;
	}

	protected override void MUpdate(){
		base.MUpdate ();

		if (toMatchObject == null)
			return;

		//rotate the parent so the pivot is in the hand
		transform.parent.localRotation = Quaternion.Inverse(toMatchObject.transform.localRotation);
	}

	/// <summary>
	/// TODO: find a better way to handle the match object algorithm
	/// </summary>
	protected void OnTriggerEnter(Collider col)
	{
		//works for either controller
		if (col.gameObject.tag == "GameController") {
		//if (col.gameObject.name == toMatchObject.name) {
			LogicArg logicArg = new LogicArg (this);

			M_Event.FireLogicEvent (LogicEvents.End, logicArg);
		} 
	}

	protected void OnTriggerExit(Collider col)
	{
		if (col.gameObject.name == toMatchObject.name) {
			//matchObject = null;
		}
	}

	/// <summary>
	/// react to end event from OnTriggerEnter
	/// </summary>
	/// <param name="arg">Argument.</param>
	protected void OnEnd( LogicArg arg )
	{
		SteamVR_TrackedObject eobj = (SteamVR_TrackedObject)arg.GetMessage (Global.EVENT_LOGIC_END_EOBJECT);
		Debug.Log ("end reached");
		// check that obj sending event is this toMatchObj
		if (eobj != null && eobj.gameObject == toMatchObject) {
			
			// vibrate the controller holding the toMatchObject
			if (toMatchObject.name == "Controller (left)") {
				InputManager.Instance.VibrateController (ViveInputController.Instance.leftControllerIndex);
				Debug.Log ("left controller shakes");
			} else {
				InputManager.Instance.VibrateController (ViveInputController.Instance.rightControllerIndex);
				Debug.Log ("right controller shakes");
			}
		}
	}
}

