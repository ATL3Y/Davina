using UnityEngine;
using System.Collections;

public class CollectableObj : MObject {
	
	[SerializeField] MeshRenderer[] outlineRenders;

	protected override void MAwake ()
	{
		base.MAwake ();
		SetOutline (false);
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
	void SetOutline( bool isOn )
	{
		foreach (MeshRenderer r in outlineRenders) {
			r.enabled = isOn;
		}
	}

	virtual public void Select()
	{
	}

	virtual public void UnSelect()
	{
	}
}
