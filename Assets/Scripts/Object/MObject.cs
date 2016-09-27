using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class MObject : MBehavior {

	[SerializeField] protected AudioClip focusSound;
	private AudioSource focusSoundSource;

	protected override void MAwake ()
	{
		base.MAwake ();
		if (focusSound != null) {
			focusSoundSource = gameObject.AddComponent<AudioSource> ();
			focusSoundSource.playOnAwake = false;
			focusSoundSource.loop = false;
			focusSoundSource.volume = 0.5f;
			focusSoundSource.spatialBlend = 1f;
			focusSoundSource.clip = focusSound;
		}
	}

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
		if (focusSoundSource != null)
			focusSoundSource.Play ();
	}

	/// <summary>
	/// Call when the input manager checked that the object is out of focus
	/// </summary>
	virtual public void OnOutofFocus()
	{
		m_isFocus = false;
	}
}
