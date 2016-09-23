using UnityEngine;
using System.Collections;

public class CollectableObj : MObject {

	[SerializeField] MeshRenderer[] bodyRenders;
	[SerializeField] float outLineWidth = 0.0005f;

	public override void OnFocus ()
	{
		base.OnFocus ();
		Debug.Log ("On focuse " + name);
		SetFloatParameter ("_Outline", outLineWidth);
	}

	public override void OnOutofFocus ()
	{
		base.OnOutofFocus ();
		SetFloatParameter ("_Outline", 0);
	}
		
	void SetFloatParameter( string parameter, float toValue)
	{
		foreach (MeshRenderer r in bodyRenders) {
			foreach (Material m in r.materials) {
				m.SetFloat (parameter, toValue);
			}
		}
	}

	virtual public void Select()
	{
	}

	virtual public void UnSelect()
	{
	}
}
