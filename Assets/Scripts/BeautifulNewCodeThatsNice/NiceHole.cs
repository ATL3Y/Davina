using UnityEngine;
using System.Collections;

public class NiceHole : Interactable {

	[SerializeField] protected AudioClip hoverSound;
	protected AudioSource hoverSoundSource;
	private float hoverSoundCooldown = 0f;
	[SerializeField] protected AudioClip storySound;
	protected AudioSource storySoundSource;
	private float storySoundCooldown = 0f;

	bool m_finished = false;

	private bool callOnce = true;

	private Vector3 originalScale;

	[SerializeField] protected MeshRenderer[] outlineRenders;
	private Material material;
	private Color color;
	[SerializeField] protected float outlineWidth;

	// Use this for initialization
	void Start () {
		base.Start ();

		material = new Material(Shader.Find("Outlined/Silhouette Only"));

		foreach (MeshRenderer r in outlineRenders) {
			r.material = material;
			ColorUtility.TryParseHtmlString ("#FFFFFFFF", out color);
			r.material.SetFloat ("_Outline", outlineWidth);
			r.material.SetVector ("_OutlineColor", color);
		}

		SetOutline (true);

		if (hoverSound != null) {
			hoverSoundSource = gameObject.AddComponent<AudioSource> ();
			hoverSoundSource.playOnAwake = false;
			hoverSoundSource.loop = false;
			hoverSoundSource.volume = 1f;
			hoverSoundSource.spatialBlend = 1f;
			hoverSoundSource.clip = hoverSound;
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

		originalScale = transform.lossyScale;

	}
	
	// Update is called once per frame
	void Update () {
		base.Update ();

		foreach ( MeshRenderer r in outlineRenders )
		{
		//	r.material.SetFloat( "_Outline", m_hoverTime > 0.5f ? outlineWidth * 2.0f : outlineWidth / 2f );
		}

		storySoundCooldown -= Time.deltaTime;
		hoverSoundCooldown -= Time.deltaTime;

		// lock the scale of holes
		Vector3 temScale = transform.lossyScale;
		if (temScale != originalScale) {
			Vector3 localScale = transform.localScale;
			localScale.x *= originalScale.x / temScale.x;
			localScale.y *= originalScale.y / temScale.y;
			localScale.z *= originalScale.z / temScale.z;
			transform.localScale = localScale;
		}

		//tell Story Object Manager Tutorial that story was heard 
		if (hoverSoundCooldown > 0.0f && hoverSoundCooldown < 0.1f 
			&& callOnce && tag == "Tutorial") {
			M_Event.FireLogicEvent (LogicEvents.Heard, new LogicArg (this));
			callOnce = false;
		}
	}

	public AudioSource GetStorySoundSource(){
		return storySoundSource;
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

		if (storySoundCooldown > 0.0f) {
			return;
		}

		if (storySound && !storySoundSource.isPlaying) {
			storySoundSource.Play ();
			storySoundCooldown = storySound.length + 3f;
		}
	}

	void SetOutline( bool isOn )
	{
		foreach (MeshRenderer r in outlineRenders) {
			r.enabled = isOn;
		}
	}
}
