using UnityEngine;
using System.Collections;

public class CollectableObj : MObject {
	
	[SerializeField] MeshRenderer[] outlineRenders;
	[SerializeField] protected AudioClip selectSound;
	private AudioSource selectSoundSource;
	[SerializeField] protected AudioClip unselectSound;
	private AudioSource unselectSoundSource;

	protected override void MAwake ()
	{
		base.MAwake ();
		SetOutline (false);
		if (selectSound != null) {
			selectSoundSource = gameObject.AddComponent<AudioSource> ();
			selectSoundSource.playOnAwake = false;
			selectSoundSource.loop = false;
			selectSoundSource.volume = 0.5f;
			selectSoundSource.spatialBlend = 1f;
			selectSoundSource.clip = selectSound;
		}
		if (unselectSound != null) {
			unselectSoundSource = gameObject.AddComponent<AudioSource> ();
			unselectSoundSource.playOnAwake = false;
			unselectSoundSource.loop = false;
			unselectSoundSource.volume = 0.5f;
			unselectSoundSource.spatialBlend = 1f;
			unselectSoundSource.clip = unselectSound;
		}
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
		if ( selectSoundSource != null )
			selectSoundSource.Play ();
		return true;
	}

	virtual public bool UnSelect()
	{
		if ( unselectSoundSource != null )
			unselectSoundSource.Play ();
		return true;
	}

}
