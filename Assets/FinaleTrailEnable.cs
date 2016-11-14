using UnityEngine;
using System.Collections;

public class FinaleTrailEnable : MBehavior {

	protected override void MAwake ()
	{
		base.MAwake ();

	}

	protected override void MOnEnable(){

		base.MOnEnable ();
		M_Event.logicEvents[ ( int )LogicEvents.Finale ] += OnFinale;
		M_Event.logicEvents[ ( int )LogicEvents.End ] += OnEnd;

	}

	protected override void MOnDisable(){

		base.MOnDisable ();
		M_Event.logicEvents[ ( int )LogicEvents.End ] -= OnEnd;

	}

	void OnFinale(LogicArg arg){
		GetComponent<TrailRenderer> ().enabled = true;
	}

	void OnEnd(LogicArg arg){
		GetComponent<TrailRenderer> ().enabled = false;
	}
}
