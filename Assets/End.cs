using UnityEngine;
using System.Collections;

public class End : MBehavior {

	// Use this for initialization
	protected override void MOnEnable () {
		base.MOnEnable ();
		M_Event.logicEvents [(int)LogicEvents.End] += OnEnd; 
	}

	protected override void MOnDisable() {
		base.MOnDisable ();
		M_Event.logicEvents [(int)LogicEvents.End] -= OnEnd; 
	}
	
	// Update is called once per frame
	protected override void MUpdate () {
		base.MUpdate ();
	}

	void OnEnd( LogicArg arg ){
		Debug.Log (" at end");
	}
}
