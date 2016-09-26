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

	virtual public bool Select()
	{
		SelectObjectManager.AttachToCamera (transform);
		gameObject.layer = LayerMask.NameToLayer ("Hold");
		foreach (Transform t in GetComponentsInChildren<Transform>())
			t.gameObject.layer = LayerMask.NameToLayer ("Hold");
		return true;
	}

	virtual public bool UnSelect()
	{
		return true;
	}

	/// <summary>
	/// Match the object with another object, 
	/// </summary>
	/// <returns><c>true</c>, if this object should be unselect, <c>false</c> otherwise.</returns>
	virtual public bool MatchWithOtherObject( MObject obj )
	{
		return false;
	}
}
