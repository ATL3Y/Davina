using UnityEngine;
using System.Collections;
using Valve.VR;
using System.Collections.Generic;
using DG.Tweening;

public class NiceCollectable : Interactable 
{

	[SerializeField] LogicEvents onFillRaiseEvent;
	[SerializeField] LogicEvents onFillLowerEvent;

	bool m_finished = false;
	Vector3 offset;
	Quaternion rotationOffset;
	public NiceHole niceHole;

	[SerializeField] protected AudioClip hoverSound;
	protected AudioSource hoverSoundSource;
	private float hoverSoundCooldown = 0f;
	[SerializeField] protected AudioClip pickUpSound;
	protected AudioSource pickUpSoundSource;
	[SerializeField] protected AudioClip putDownSound;
	protected AudioSource putDownSoundSource;
	[SerializeField] protected AudioClip storySound;
	protected AudioSource storySoundSource;
	private float storySoundCooldown = 0f;

	Vector3 m_baseScale;


	// Use this for initialization
	void Start () 
	{
		base.Start ();

		m_baseScale = transform.localScale;

		if (hoverSound != null) {
			hoverSoundSource = gameObject.AddComponent<AudioSource> ();
			hoverSoundSource.playOnAwake = false;
			hoverSoundSource.loop = false;
			hoverSoundSource.volume = 1f;
			hoverSoundSource.spatialBlend = 1f;
			hoverSoundSource.clip = hoverSound;
		}
		// set up the select sound
		if (pickUpSound != null) {
			pickUpSoundSource = gameObject.AddComponent<AudioSource> ();
			pickUpSoundSource.playOnAwake = false;
			pickUpSoundSource.loop = false;
			pickUpSoundSource.volume = 1f;
			pickUpSoundSource.spatialBlend = 1f;
			pickUpSoundSource.clip = pickUpSound;
		}
		// set up the unselect sound
		if (putDownSound != null) {
			putDownSoundSource = gameObject.AddComponent<AudioSource> ();
			putDownSoundSource.playOnAwake = false;
			putDownSoundSource.loop = false;
			putDownSoundSource.volume = 1f;
			putDownSoundSource.spatialBlend = 1f;
			putDownSoundSource.clip = putDownSound;
		}
		// set up the story sound
		if (storySound != null) {
			storySoundSource = gameObject.AddComponent<AudioSource> ();
			storySoundSource.playOnAwake = false;
			storySoundSource.loop = false;
			storySoundSource.volume = 1f;
			storySoundSource.spatialBlend = 1f;
			storySoundSource.clip = storySound;
		}
	}

	float EaseOutCubic( float value )
	{
		float start = 0.0f;
		float end = 1.0f;
		value--;
		end -= start;
		return end * (value * value * value + 1) + start;
	}

	float Ease( float t, float b, float c, float d )
	{
		float ts = ( t /= d ) * t;
		float tc = ts * t;
		return b + c * ( 0.95f * tc * ts + -5.1425f * ts * ts + 10.29f * tc + -10.395f * ts + 5.297f * t);
	}
	bool m_hasDroppedDuringTransportation = false;
	// Update is called once per frame
	public override void Update () {
		base.Update ();

		if (m_hoverTime > .5f) {
			SetOutline (true);
		} else {
			SetOutline (false);
		}

		storySoundCooldown -= Time.deltaTime;
		hoverSoundCooldown -= Time.deltaTime;
		transform.localScale = m_baseScale + m_baseScale.normalized * EaseOutCubic (m_hoverTime);

		if (m_finished) {
			owner = null;
			return;
		}
		
		if (owner) {
			bool dropable = true;
			if ( TransportManager.Instance.IsTransporting ) print( "transporting" );
			if (TransportManager.Instance.IsTransporting) {	
				dropable = false;
				AxKDebugLines.AddSphere (owner.transform.position, 0.1f, Color.yellow);
				/*
				if ((owner.left && ViveInputController.Instance.ReceivedLeftButtonUpSignal ())
				    || (!owner.left && ViveInputController.Instance.ReceivedRightButtonUpSignal ())) {
					m_hasDroppedDuringTransportation = true;
				}

				if ((owner.left && ViveInputController.Instance.ReceivedLeftButtonDownSignal ())
				    || (!owner.left && ViveInputController.Instance.ReceivedRightButtonDownSignal ())) {
					m_hasDroppedDuringTransportation = false;
				}
			} else if (m_hasDroppedDuringTransportation) {
				m_hasDroppedDuringTransportation = false;
				owner = null;

				if (putDownSound) {
					putDownSoundSource.Play ();
				}

				return;*/
			}

			Vector3 newPosition =  owner.transform.position + owner.transform.TransformDirection (offset);
			float distanceToTarget = Vector3.Magnitude (newPosition - niceHole.transform.position);
			Quaternion newRotation = owner.transform.rotation * rotationOffset;
			Debug.DrawLine (newPosition - Vector3.up * 0.05f, newPosition + Vector3.up * 0.05f, Color.red);

			if (distanceToTarget < 0.11f) {
				float t = Mathf.Clamp01( 1.0f - ( distanceToTarget * ( 1.0f / 0.1f ) ) );
				t = Ease (t, 0.0f, 1.0f, 1.0f );
				//print (t);
				//t = Mathf.Clamp01 (t + 0.2f);
				newPosition = Vector3.Lerp (newPosition, niceHole.transform.position, t);
				newRotation = Quaternion.Slerp (newRotation, niceHole.transform.rotation, t);
			}

			transform.position = newPosition;
			transform.rotation = newRotation;

			if ( dropable && ( (owner.left && !ViveInputController.Instance.ReceivedLeftButtonPressSignal ())
				|| (!owner.left && !ViveInputController.Instance.ReceivedRightButtonPressSignal ()))) {
				owner = null;

				if (putDownSound) {
					putDownSoundSource.Play ();
				}
			}
		} else {
			float distanceToHole = Vector3.Magnitude (transform.position - niceHole.transform.position);
			if (distanceToHole < 0.03f) {
				//gameObject.layer = LayerMask.NameToLayer( "Done" ); 
				//niceHole.gameObject.layer = LayerMask.NameToLayer( "Done" ); 
				useable = false;
				niceHole.useable = false;
				transform.parent = niceHole.gameObject.transform; 
				transform.DOLocalMove (Vector3.zero, 1f).SetEase (DG.Tweening.Ease.InCirc);
				transform.DOLocalRotate ( GetOriginalRot( ), 1f).SetEase (DG.Tweening.Ease.InCirc);
				transform.DOScale (1.04f, 1f).SetEase (DG.Tweening.Ease.InCirc);

				m_finished = true;

				// disable non-matched collectables
				if ( gameObject.tag == "Raise" || gameObject.tag == "Lower"){
					M_Event.FireLogicEvent( LogicEvents.ExitStory, new LogicArg( this ) );
				} else if ( gameObject.tag == "TutorialRight" || gameObject.tag == "TutorialLeft" ){
					M_Event.FireLogicEvent( LogicEvents.ExitStoryTutorial, new LogicArg( this ) );
				}

				/*
				// speed through
				if ( gameObject.tag == "Raise" ) {
					M_Event.FireLogicEvent( onFillRaiseEvent, new LogicArg( this ) );
					M_Event.FireLogicEvent( LogicEvents.EnterStory, new LogicArg( this ) );
				} else if ( gameObject.tag == "Lower" ) {
					M_Event.FireLogicEvent( onFillLowerEvent, new LogicArg( this ) );
					M_Event.FireLogicEvent( LogicEvents.EnterStory, new LogicArg( this ) );
				} else if ( gameObject.tag == "TutorialRight" || gameObject.tag == "TutorialLeft" ) {
					MetricManagerScript.instance.AddToMatchList( Time.timeSinceLevelLoad + " in call exitstorytutorial "  + "/n");
					M_Event.FireLogicEvent (LogicEvents.EnterStoryTutorial, new LogicArg( this ) );
				}
				*/

				//play matched audio 
				if (storySoundSource != null && storySoundSource.isPlaying) {
					storySoundSource.Stop ();
				}

				if (storySoundSource != null ) {
					StartCoroutine( DelaySoundClipPlay( niceHole.GetStorySoundSource(), storySoundSource ) );
				} else {
					CallNextEvent( );
				}
			}
		}
	}

	IEnumerator DelaySoundClipPlay( AudioSource holeStory, AudioSource collectableStory )
	{
		holeStory.Play( );
		yield return new WaitForSeconds( holeStory.clip.length );
		collectableStory.Play( );
		yield return new WaitForSeconds( collectableStory.clip.length );
		CallNextEvent( );
	}

	public override void Use( Hands hand )
	{
		rotationOffset = Quaternion.Inverse (hand.transform.rotation) *  transform.rotation;
		offset = hand.transform.InverseTransformDirection (transform.position - hand.transform.position);
		//offset = transform.position - hand.transform.position;

		owner = hand;
		//transform.parent = owner.transform;

		if (pickUpSound) {
			pickUpSoundSource.Play ();
		}

		if (storySoundCooldown > 0.0f) {
			return;
		}

		if (storySound && !storySoundSource.isPlaying) {
			storySoundSource.Play ();
			storySoundCooldown = storySound.length + 3f;
		}
	}
		
	public override void InUseRange()
	{
		base.InUseRange ();

		if (m_finished) {
			owner = null;
			return;
		}

		if (hoverSoundCooldown > 0.0f)
			return;

		if (hoverSound && !hoverSoundSource.isPlaying) {
			hoverSoundSource.Play ();
			hoverSoundCooldown = hoverSound.length + 1f;
		}
	}

	void CallNextEvent( )
	{
		
		//advance the story
		if ( gameObject.tag == "Raise" ) {
			M_Event.FireLogicEvent( onFillRaiseEvent, new LogicArg( this ) );
			M_Event.FireLogicEvent( LogicEvents.EnterStory, new LogicArg( this ) );
		} else if ( gameObject.tag == "Lower" ) {
			M_Event.FireLogicEvent( onFillLowerEvent, new LogicArg( this ) );
			M_Event.FireLogicEvent( LogicEvents.EnterStory, new LogicArg( this ) );
		} else if ( gameObject.tag == "TutorialRight" || gameObject.tag == "TutorialLeft" ) {
			MetricManagerScript.instance.AddToMatchList( Time.timeSinceLevelLoad + " in call exitstorytutorial "  + "/n");
			M_Event.FireLogicEvent (LogicEvents.EnterStoryTutorial, new LogicArg( this ) );
		}


	}
}
