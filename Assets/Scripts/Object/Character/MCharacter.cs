using UnityEngine;
using System.Collections;

public class MCharacter : MObject {

	[SerializeField] Transform outBody;
	[SerializeField] Transform innerWorld;

	// for the inner world
	[SerializeField] AudioClip innerWorldClip;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	/// <summary>
	/// set this game object and all the body renders to a specific layer
	/// </summary>
	/// <param name="layer">Layer.</param>
	void SetToLayer( string layer )
	{
		gameObject.layer = LayerMask.NameToLayer (layer);
		foreach (Transform t in outBody.GetComponentsInChildren<Transform>()) {
			t.gameObject.layer = LayerMask.NameToLayer (layer);
		}
		foreach (Transform t in innerWorld.GetComponentsInChildren<Transform>()) {
			t.gameObject.layer = LayerMask.NameToLayer (layer);
		}
	}

	public override void OnFocus ()
	{
		base.OnFocus ();
	}

	public override void OnOutofFocus ()
	{
		base.OnOutofFocus ();

	}

	public void EnterInnerWorld( Collider col )
	{
		LogicArg arg = new LogicArg (this);
		arg.AddMessage (Global.EVENT_LOGIC_ENTERINNERWORLD_BGM, innerWorldClip);
		arg.AddMessage (Global.EVENT_LOGIC_ENTERINNERWORLD_CHARACTER, this);
		M_Event.FireLogicEvent (LogicEvents.EnterInnerWorld, arg);
	}

	public void ExitInnerWorld( Collider col )
	{
		LogicArg arg = new LogicArg (this);

		arg.AddMessage (Global.EVENT_LOGIC_EXITINNERWORLD_CHARACTER, this);
		M_Event.FireLogicEvent (LogicEvents.ExitInnerWorld, arg);
	}
}
