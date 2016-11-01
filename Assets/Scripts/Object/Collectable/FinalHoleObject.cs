using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FinalHoleObject : HoleObject
{
	GameObject toMatchObject;

	[SerializeField] GameObject trail;

	protected override void MAwake ()
	{
		base.MAwake ();
	}

	public override void OnFocus ()
	{
		base.OnFocus ();
	}

	public override void OnOutofFocus ()
	{
		base.OnOutofFocus ();
	}

	protected override void MOnEnable ()
	{
		//base.MOnEnable ();
		if (gameObject.name == "Cylinder (left)") {
			toMatchObject = GameObject.Find ("Controller (left)");
		} else {
			toMatchObject = GameObject.Find ("Controller (right)");
		}

		FinalMatchObj myComponent = toMatchObject.GetComponent<FinalMatchObj> ();
		myComponent.enabled = true;

		/* position not right
		GameObject newGameObj = Instantiate (trail);
		newGameObj.transform.localPosition = new Vector3 (0f, 0f, .2f);
		newGameObj.transform.localRotation = Quaternion.identity;
		newGameObj.transform.localScale = Vector3.zero;
		newGameObj.transform.SetParent (toMatchObject.transform);
		*/

		M_Event.logicEvents [(int)LogicEvents.MatchObject] += OnMatchObject; 
	}

	protected override void MOnDisable ()
	{
		//base.MOnDisable ();
		M_Event.logicEvents [(int)LogicEvents.MatchObject] -= OnMatchObject; 
	}

	protected override void MUpdate(){
		base.MUpdate ();

		if (toMatchObject == null)
			return;

		//rotate the parent so the pivot is in the hand
		transform.parent.localRotation = toMatchObject.transform.localRotation;
	}

	/// <summary>
	/// TODO: find a better way to handle the match object algorithm
	/// </summary>
	protected override void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.name == toMatchObject.name) {
			CollectableObj cobj = toMatchObject.GetComponent<CollectableObj>();
			if (cobj != null) {
				// make it so the next click will not trigger Unselect's transform change in CollectableObject
				cobj.matched = true;
			}
		}
	}

	protected override void OnTriggerExit(Collider col)
	{
		if (col.gameObject.name == toMatchObject.name) {
			//matchObject = null;
		}
	}

	/// <summary>
	/// react to the match object event sent from Final Match Obj on Select
	/// try to match the object
	/// </summary>
	/// <param name="arg">Argument.</param>
	protected override void OnMatchObject( LogicArg arg )
	{
		Debug.Log ("in onmatchobject finalHoleObj");
		//CollectableObj cobj = matchObject.GetComponent<CollectableObj>();
		CollectableObj cobj = (CollectableObj) arg.GetMessage (Global.EVENT_LOGIC_MATCH_COBJECT);

		// if the match succeeds
		if (cobj != null && cobj.gameObject == toMatchObject) {
			StoryObjManager.Instance.endMatchCount++;
			Debug.Log ("Match succeeds from finalHoleObj");
			// vibrate the controller holding the matchObject
			if (cobj.transform.gameObject.name == "Controller (left)") {
				InputManager.Instance.VibrateController (ViveInputController.Instance.leftControllerIndex);
			} else {
				InputManager.Instance.VibrateController (ViveInputController.Instance.rightControllerIndex);
			}

			//Hole disappears...
			this.gameObject.SetActive(false);
		}
	}
}

