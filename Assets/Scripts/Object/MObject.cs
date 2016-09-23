using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class MObject : MBehavior {

	/// <summary>
	/// a varible to record whether the object is being focused
	/// </summary>
	private bool m_isFocus = false;
	public bool isFocused
	{
		get { return isFocused; }
	}

	/// <summary>
	/// Call when the input manager checked that the object is on focus
	/// </summary>
	virtual public void OnFocus()
	{
		m_isFocus = true;
	}

	/// <summary>
	/// Call when the input manager checked that the object is out of focus
	/// </summary>
	virtual public void OnOutofFocus()
	{
		m_isFocus = false;
	}
}
