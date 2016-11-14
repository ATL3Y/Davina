using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class FinalHoleObject : MObject
{
	GameObject toMatchObject;
	[SerializeField] GameObject trail;

	Collider col;
	[SerializeField] float fixInTime = 1f;

	// i want the holes to have an outline too
	[SerializeField] MeshRenderer[] outlineRenders;

	[SerializeField] protected AudioClip storySound;
	protected AudioSource storySoundSource;

	private int count = 0;

	protected override void MAwake ()
	{
		base.MAwake ();
		col = GetComponent<Collider> ();
		col.isTrigger = true;

		// set up the story sound
		if (storySound != null) {
			storySoundSource = gameObject.AddComponent<AudioSource> ();
			storySoundSource.playOnAwake = false;
			storySoundSource.loop = false;
			storySoundSource.volume = 1f;
			storySoundSource.spatialBlend = 1f;
			storySoundSource.clip = storySound;
		}

        //set up rotation to match a controller
        if(gameObject.tag == "Left" )
        {
            toMatchObject = GameObject.Find( "Controller (left)" );
        } else
        {
            toMatchObject = GameObject.Find( "Controller (right)" );
        }

	}
		
	/// <summary>
	/// Call when the input manager checked that the object is on focus
	/// </summary>
	public override void OnFocus()
	{
		base.OnFocus ();
	}

	public override void OnOutofFocus ()
	{
		base.OnOutofFocus ();
	}

	/// <summary>
	/// Set the outline render on or off(enable)
	/// </summary>
	/// <param name="isOn">If set to <c>true</c> is on.</param>
	protected void SetOutline( bool isOn )
	{
	}

	protected override void MOnEnable ()
	{
		base.MOnEnable ();
		transform.SetParent (toMatchObject.transform);
		if ( storySoundSource != null && !storySoundSource.isPlaying)
			storySoundSource.Play ();
	}

	protected override void MOnDisable ()
	{
		base.MOnDisable ();
		if ( storySoundSource != null && !storySoundSource.isPlaying)
			storySoundSource.Play ();
	}

	protected override void MUpdate(){
		base.MUpdate ();

		//rotate the parent so the pivot is in the hand
		//transform.parent.localRotation = Quaternion.Inverse(toMatchObject.transform.localRotation);
		//transform.parent.localRotation = toMatchObject.transform.localRotation;
	}

	/// <summary>
	/// TODO: find a better way to handle the match object algorithm
	/// </summary>
	protected void OnTriggerEnter(Collider col)
	{

	}

	protected void OnTriggerExit(Collider col)
	{

	}

}
	