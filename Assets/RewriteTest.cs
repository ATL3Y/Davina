using UnityEngine;
using System.Collections;

public class RewriteTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.N)) {
			LogicArg logicArg = new LogicArg( this );
			M_Event.FireLogicEvent( LogicEvents.Characters, logicArg );
		}
	}
}
