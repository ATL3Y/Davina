using UnityEngine;
using System.Collections;

public class PasserBy : MObject {

	[SerializeField] MeshRenderer[] BodyRenders;
	[Range(0,0.0001f)]
	[SerializeField] float outLineWidth = 0.00005f;

	protected override void MOnEnable ()
	{
		base.MOnEnable ();
		M_Event.logicEvents [(int)LogicEvents.TransportStart] += OnTransportStart;
		M_Event.logicEvents [(int)LogicEvents.TransportEnd] += OnTransportEnd;
	}

	protected override void MOnDisable ()
	{
		base.MOnDisable ();
		M_Event.logicEvents [(int)LogicEvents.TransportStart] -= OnTransportStart;
		M_Event.logicEvents [(int)LogicEvents.TransportEnd] -= OnTransportEnd;
	}

	void OnTransportStart( LogicArg arg )
	{
		var obj = arg.GetMessage (Global.EVENT_LOGIC_TRANSPORTTO_MOBJECT);
		if (obj is PasserBy) {
			PasserBy pass = (PasserBy)obj;
			if (pass == this) {
				SetToLayer ("Focus");

			} else {
				SetToLayer ("PasserBy");
			}
		}
	}

	void OnTransportEnd( LogicArg arg )
	{
		
	}

	void SetToLayer( string layer )
	{
		Debug.Log (name + "Set layer to " + layer);
		gameObject.layer = LayerMask.NameToLayer (layer);
		foreach (MeshRenderer r in BodyRenders) {
			r.gameObject.layer = LayerMask.NameToLayer (layer);
			foreach (Transform t in r.transform.GetComponentsInChildren<Transform>()) {
				t.gameObject.layer = LayerMask.NameToLayer (layer);
			}
		}

	}


	public override void OnFocus ()
	{
		base.OnFocus ();
	
		if (this != LogicManager.Instance.StayPasserBy) {
				foreach (MeshRenderer r in BodyRenders) {
					foreach (Material m in r.materials)
						m.SetFloat ("_Outline", outLineWidth);
				}
		}
	}

	public override void OnOutofFocus ()
	{
		base.OnOutofFocus ();
		foreach (MeshRenderer r in BodyRenders) {
			foreach( Material m in r.materials )
				m.SetFloat ("_Outline", 0);
		}

	}
}
