using UnityEngine;
using System.Collections;
using Valve.VR;
using System.Collections.Generic;
using DG.Tweening;

public class NiceCollectable : Interactable 
{

	[SerializeField] LogicEvents onFillRaiseEvent;
	[SerializeField] LogicEvents onFillLowerEvent;

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
	[SerializeField] protected AudioClip storySoundR;
	protected AudioSource storySoundSourceR;
	private float storySoundCooldownR = 0f;
	[SerializeField] protected AudioClip storySoundL;
	protected AudioSource storySoundSourceL;
	private float storySoundCooldownL = 0f;

	Vector3 m_baseScale;

	bool pinkSideOut = false;

	[SerializeField] protected MeshRenderer[] outlineRenders;
	private Material material;
	private Color color;
	[SerializeField] protected float outlineWidth;

	bool callOnce = true;

	// Use this for initialization
	void Start () {
		base.Start ();

		material = new Material(Shader.Find("Outlined/Silhouette Only"));

		foreach (MeshRenderer r in outlineRenders) {
			r.material = material;
			ColorUtility.TryParseHtmlString ("#FFA59565", out color);
			r.material.SetFloat ("_Outline", outlineWidth);
			r.material.SetVector ("_OutlineColor", color);
		}

		SetOutline (false);

		m_baseScale = transform.localScale; //local
		//print ("base scale " + m_baseScale);
		//print ("lossy scale " + transform.lossyScale);

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
		// set up the story sound R
		if (storySoundR != null) {
			storySoundSourceR = gameObject.AddComponent<AudioSource> ();
			storySoundSourceR.playOnAwake = false;
			storySoundSourceR.loop = false;
			storySoundSourceR.volume = 1f;
			storySoundSourceR.spatialBlend = 1f;
			storySoundSourceR.clip = storySoundR;
		}
		// set up the story sound L
		if (storySoundL != null) {
			storySoundSourceL = gameObject.AddComponent<AudioSource> ();
			storySoundSourceL.playOnAwake = false;
			storySoundSourceL.loop = false;
			storySoundSourceL.volume = 1f;
			storySoundSourceL.spatialBlend = 1f;
			storySoundSourceL.clip = storySoundL;
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

	// Update is called once per frame
	public override void Update () {
		base.Update ();

		if (m_hoverTime > .3f) {
			SetOutline (true);
		} else {
			SetOutline (false);
		}

		storySoundCooldownR -= Time.deltaTime;
		storySoundCooldownL -= Time.deltaTime;
		hoverSoundCooldown -= Time.deltaTime;
		transform.localScale = m_baseScale + m_baseScale.normalized * EaseOutCubic (m_hoverTime); 

		if (m_finished) {
			owner = null;
			return;
		}
		
		if (owner) {
			if (callOnce) {
				TextInstructions.Instance.PickedUpCollectable ();
				callOnce = false;
			}
			bool dropable = true;
			if ( TransportManager.Instance.IsTransporting ) print( "transporting" );
			if (TransportManager.Instance.IsTransporting) {	
				dropable = false;
				transform.SetParent (null);
			}

			Vector3 newPosition =  owner.transform.position + owner.transform.TransformDirection (offset);
			float distanceToTarget = Vector3.Magnitude (newPosition - niceHole.transform.position);
			Quaternion newRotation = owner.transform.rotation * rotationOffset;
			//Debug.DrawLine (newPosition - Vector3.up * 0.05f, newPosition + Vector3.up * 0.05f, Color.red);

			// Play story sound based on which side is facing player
			if (Vector3.Dot (transform.right, LogicManager.Instance.GetPlayerHeadTransform ().forward) > 0.45) {
				//pink side out
				ColorUtility.TryParseHtmlString ("#FFACF9FF", out color);
				SetOutline (true);

				if (storySoundSourceL != null && storySoundSourceL.isPlaying) {
					storySoundSourceL.Stop ();
				}

				if (storySoundSourceR != null && storySoundCooldownR < 0.0f) {
					storySoundSourceR.Play ();
					storySoundCooldownR = storySoundR.length + 3f;
					if (owner.left) {
						ViveInputController.Instance.VibrateController (ViveInputController.Instance.leftControllerIndex);
					} else {
						ViveInputController.Instance.VibrateController (ViveInputController.Instance.rightControllerIndex);
					}
				} 
			} else if (Vector3.Dot (transform.right, LogicManager.Instance.GetPlayerHeadTransform ().forward) < -0.45) {
				//blue side out 
				ColorUtility.TryParseHtmlString ("#00FFFFFF", out color);
				SetOutline (true);

				if (storySoundSourceR != null && storySoundSourceR.isPlaying) {
					storySoundSourceR.Stop ();
				}

				if (storySoundSourceL != null && storySoundCooldownL < 0.0f) {
					storySoundSourceL.Play ();
					storySoundCooldownL = storySoundL.length + 3f;
					if (owner.left) {
						ViveInputController.Instance.VibrateController (ViveInputController.Instance.leftControllerIndex);
					} else {
						ViveInputController.Instance.VibrateController (ViveInputController.Instance.rightControllerIndex);
					}

				} 
			} else {
                SetOutline (false);

			}

			if (distanceToTarget < 0.11f) {
				//AxKDebugLines.AddLine (transform.position, transform.position + transform.right * .1f, Color.red, 0);
				//AxKDebugLines.AddLine (niceHole.transform.position, niceHole.transform.position + niceHole.transform.right * .1f, Color.blue, 0);
				// orient collectable in hole based on relation to hole
				if (Vector3.Dot (transform.right, niceHole.transform.right) > 0f) {
					pinkSideOut = true;
				} else if (Vector3.Dot (transform.right, niceHole.transform.right) <= 0f) {
					pinkSideOut = false;
					//AxKDebugLines.AddSphere (transform.position, .1f, Color.cyan, 0);
				}
				float t = Mathf.Clamp01( 1.0f - ( distanceToTarget * ( 1.0f / 0.1f ) ) );
				t = Ease (t, 0.0f, 1.0f, 1.0f );
				//print (t);
				//t = Mathf.Clamp01 (t + 0.2f);

				Quaternion rot = new Quaternion();
				if (pinkSideOut) { 
					rot = niceHole.transform.rotation; 
				} else {
					rot = Quaternion.AngleAxis (180f, niceHole.transform.up) * niceHole.transform.rotation;
					//AxKDebugLines.AddLine (niceHole.transform.position, niceHole.transform.position + niceHole.transform.up * .1f, Color.green, 0);
				}

				newPosition = Vector3.Lerp (newPosition, niceHole.transform.position, t);
				newRotation = Quaternion.Slerp (newRotation, rot, t);
				//AxKDebugLines.AddLine (newPosition, newPosition + transform.right * .1f, Color.yellow, 0);
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
				useable = false;
				niceHole.useable = false;
				transform.parent = niceHole.gameObject.transform; 
				m_finished = true;
				niceHole.SetFinished(true);

				// disable non-matched collectables //don't need
				/*
				if ( gameObject.tag == "Untagged"){
					M_Event.FireLogicEvent( LogicEvents.ExitStory, new LogicArg( this ) );
				} else if ( gameObject.tag == "Tutorial" ){
					M_Event.FireLogicEvent( LogicEvents.ExitStoryTutorial, new LogicArg( this ) );
				}
				*/

				/*
				// speed through

				*/ 
				Vector3 rot;

				if (pinkSideOut) { 
					rot = new Vector3(0f, 0f, 0f);
					if (storySoundSourceR != null && storySoundSourceR.isPlaying) {
						storySoundSourceR.Stop ();
					}

					if (storySoundSourceR != null) {
						StartCoroutine (DelaySoundClipPlay (niceHole.GetStorySoundSource (), storySoundSourceR));
					} else {
						CallNextEvent ();
					}
				} else {
					rot = new Vector3 (0f, 180f, 0f);
					if (storySoundSourceL != null && storySoundSourceL.isPlaying) {
						storySoundSourceL.Stop ();
					}

					if (storySoundSourceL != null) {
						StartCoroutine (DelaySoundClipPlay (niceHole.GetStorySoundSource (), storySoundSourceL));
					} else {
						CallNextEvent ();
					}
				}

				transform.DOLocalMove (Vector3.zero, 1f).SetEase (DG.Tweening.Ease.InCirc);
				transform.DOLocalRotate ( rot, 1f).SetEase (DG.Tweening.Ease.InCirc); 
				transform.DOScale (1.06f, 1f).SetEase (DG.Tweening.Ease.InCirc);
			} else
            // rotate before picked up
            {
                Quaternion turn = Quaternion.AngleAxis(10f, Vector3.forward) * transform.localRotation;
                transform.localRotation = Quaternion.Slerp(transform.localRotation, turn, Time.deltaTime * 7f);
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

	public override void Use( Hand hand )
	{
		rotationOffset = Quaternion.Inverse (hand.transform.rotation) *  transform.rotation;
		offset = hand.transform.InverseTransformDirection (transform.position - hand.transform.position);
		//offset = transform.position - hand.transform.position;

		owner = hand;
		//transform.parent = owner.transform;

		if (pickUpSound) {
			pickUpSoundSource.Play ();
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

	void CallNextEvent()
	{
		//advance the story
		if ( gameObject.tag == "Untagged" && !pinkSideOut ) {
			M_Event.FireLogicEvent( onFillRaiseEvent, new LogicArg( this ) );
			M_Event.FireLogicEvent( LogicEvents.EnterStory, new LogicArg( this ) );
		} else if ( gameObject.tag == "Untagged" && pinkSideOut ) {
			M_Event.FireLogicEvent( onFillLowerEvent, new LogicArg( this ) );
			M_Event.FireLogicEvent( LogicEvents.EnterStory, new LogicArg( this ) );
		} else if ( gameObject.tag == "Tutorial" ) {
			MetricManagerScript.instance.AddToMatchList( Time.timeSinceLevelLoad + " in call exitstorytutorial "  + "/n");
			M_Event.FireLogicEvent (LogicEvents.EnterStoryTutorial, new LogicArg( this ) );
		}
	}

	void SetOutline( bool isOn )
	{
		foreach (MeshRenderer r in outlineRenders) {
			r.material.SetVector ("_OutlineColor", color);
			r.enabled = isOn;
		}
	}
}
