using UnityEngine;
using System.Collections;

public class PasserBy : MObject {

	[SerializeField] MeshRenderer[] UmbrellaBarRenders;
	[SerializeField] MeshRenderer[] UmbrellaFaceRenders;
	[SerializeField] MeshRenderer[] BodyRenders;
	[Range(0,0.0001f)]
	[SerializeField] float outLineWidth = 0.00005f;



	public override void OnFocus ()
	{
		base.OnFocus ();
		gameObject.layer = LayerMask.NameToLayer ("Focus");

		foreach (MeshRenderer r in BodyRenders) {
			r.gameObject.layer = LayerMask.NameToLayer ("Focus");
			foreach( Material m in r.materials )
				m.SetFloat ("_Outline", outLineWidth);
		}
	}

	public override void OnOutofFocus ()
	{
		base.OnOutofFocus ();
		gameObject.layer = LayerMask.NameToLayer ("PasserBy");

		foreach (MeshRenderer r in BodyRenders) {
			r.gameObject.layer = LayerMask.NameToLayer ("PasserBy");
			foreach( Material m in r.materials )
				m.SetFloat ("_Outline", 0);
		}
	}
}
