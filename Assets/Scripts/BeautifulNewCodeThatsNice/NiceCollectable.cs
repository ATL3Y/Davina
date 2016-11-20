using UnityEngine;
using System.Collections;
using Valve.VR;

public class NiceCollectable : Interactable {

	bool m_finished = false;
	Vector3 offset;
	Quaternion rotationOffset;
	public NiceHole niceHole;

	public AudioClip hoverSound;
	public AudioSource hoverSoundSource;
	public float hoverSoundCooldown;
	public AudioClip pickUpSound;
	public AudioSource pickUpSoundSource;
	public AudioClip putDownSound;
	public AudioSource putDownSoundSource;
	public AudioClip storySound;
	public AudioSource storySoundSource;

	Vector3 m_baseScale;

	// Use this for initialization
	void Start () {
		m_baseScale = transform.localScale;

		if (hoverSoundSource != null) {
			hoverSoundSource = gameObject.AddComponent<AudioSource> ();
			hoverSoundSource.playOnAwake = false;
			hoverSoundSource.loop = false;
			hoverSoundSource.volume = 1f;
			hoverSoundSource.spatialBlend = 1f;
			hoverSoundSource.clip = hoverSound;
		}
		// set up the select sound
		if (pickUpSoundSource != null) {
			pickUpSoundSource = gameObject.AddComponent<AudioSource> ();
			pickUpSoundSource.playOnAwake = false;
			pickUpSoundSource.loop = false;
			pickUpSoundSource.volume = 1f;
			pickUpSoundSource.spatialBlend = 1f;
			pickUpSoundSource.clip = pickUpSound;
		}
		// set up the unselect sound
		if (putDownSoundSource != null) {
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
	
	// Update is called once per frame
	void Update () {
		hoverSoundCooldown -= Time.deltaTime;
		m_hoverTime = Mathf.Clamp01( m_hoverTime - Time.deltaTime * 2.0f );
		transform.localScale = m_baseScale + m_baseScale.normalized * EaseOutCubic (m_hoverTime) * 0.1f;


		if (m_finished)
			return;
		if (owner) {

			Vector3 newPosition =  owner.transform.position + owner.transform.TransformDirection (offset);
			float distanceToTarget = Vector3.Magnitude (newPosition - niceHole.transform.position);
			Quaternion newRotation = owner.transform.rotation * rotationOffset;
			Debug.DrawLine (newPosition - Vector3.up * 0.05f, newPosition + Vector3.up * 0.05f, Color.red);

			if (distanceToTarget < 0.2f) {
				float t = Mathf.Clamp01( 1.0f - ( distanceToTarget * ( 1.0f / 0.2f ) ) );
				t = Ease (t, 0.0f, 1.0f, 1.0f );
				print (t);
				//t = Mathf.Clamp01 (t + 0.2f);
				newPosition = Vector3.Lerp (newPosition, niceHole.transform.position, t);
				newRotation = Quaternion.Slerp (newRotation, niceHole.transform.rotation, t);
			}

			transform.position = newPosition;
			transform.rotation = newRotation;

			if ((owner.left && ViveInputController.Instance.ReceivedLeftButtonUpSignal ())
			    || (!owner.left && ViveInputController.Instance.ReceivedRightButtonUpSignal ())) {
				owner = null;
				transform.parent = null;
			}
		} else {
			float distanceToHole = Vector3.Magnitude (transform.position - niceHole.transform.position);
			if (distanceToHole < 0.05f) {
				LogicArg logicArg = new LogicArg( this );
				M_Event.FireLogicEvent( LogicEvents.Characters, logicArg );
				m_finished = true;
				useable = false;
			}
		}
	}

	public override void Use( Hands hand )
	{
		rotationOffset =Quaternion.Inverse (hand.transform.rotation) *  transform.rotation;
		offset = hand.transform.InverseTransformDirection (transform.position - hand.transform.position);
		//offset = transform.position - hand.transform.position;

		owner = hand;
		//transform.parent = owner.transform;
	}

	float m_hoverTime = 0.0f;
	public virtual void OnHover()
	{
		
	}

	public override void InUseRange()
	{
		m_hoverTime = Mathf.Clamp01 (m_hoverTime + Time.deltaTime * 4.0f);

		if (hoverSoundCooldown > 0.0f)
			return;

		if (hoverSound) {
			hoverSoundSource.Play ();
			hoverSoundCooldown = hoverSound.length + 0.1f;
		}
	}

}
