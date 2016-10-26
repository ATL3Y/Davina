using UnityEngine;
using System.Collections;

public class HeartItemBehavior : MonoBehaviour 
{
	private AudioSource m_AudioSource;
	public AudioClip click;
	public AudioClip pop;

	[SerializeField] LayerMask TestController;

	private bool attachToHand = false;
	private bool attachToHeart = false;
	private Transform m_StickTo;

	// Use this for initialization
	void Start () 
	{
		m_AudioSource = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (m_StickTo != null) 
		{
			if (attachToHeart) {
				print ("heart");
				Quaternion rot = Quaternion.Lerp (transform.localRotation, m_StickTo.localRotation, .07f);
				Vector3 pos = Vector3.Lerp (transform.position, m_StickTo.position, .07f);

				transform.localRotation = rot;
				transform.position = pos;
				return;
			} else if (attachToHand) {
				print ("hand");
				Quaternion rot = Quaternion.Lerp (transform.localRotation, m_StickTo.localRotation * Quaternion.Euler(0, 90, 0), .07f);
				Vector3 pos = Vector3.Lerp (transform.position, m_StickTo.position + m_StickTo.forward * .2f - m_StickTo.up * .2f, .07f);

				transform.localRotation = rot;
				transform.position = pos;
			}
		}
	}

	void OnTriggerEnter( Collider col )
	{
		if (col.gameObject.tag == "HeartHole" && !attachToHand) 
		{
			print ("hole hit");
			transform.SetParent (col.gameObject.transform);
			attachToHand = false;
			attachToHeart = true;

			if (!m_AudioSource.isPlaying) 
			{
				m_AudioSource.clip = pop;
				m_AudioSource.volume = 0.7f;
				m_AudioSource.Play ();
			}
		}
		if (col.gameObject.tag == "GameController" && !attachToHeart) 
		{
			print ("collision hit");
			transform.SetParent (m_StickTo);
			m_StickTo = col.gameObject.transform;
			attachToHand = true;
			attachToHeart = false;

			if (!m_AudioSource.isPlaying) 
			{
				m_AudioSource.clip = click;
				m_AudioSource.volume = 0.7f;
				m_AudioSource.Play ();
			}
		}
	}
}
