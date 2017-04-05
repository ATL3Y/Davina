using UnityEngine;
using System.Collections;

public class NiceHole : Interactable {

	[SerializeField] protected AudioClip hoverSound;
	protected AudioSource hoverSoundSource;
	private float hoverSoundCooldown = 0f;
	[SerializeField] protected AudioClip storySound;
	protected AudioSource storySoundSource;
	private float storySoundCooldown = 0f;

	private bool callOnce0 = true;
	private bool callOnce1 = true;

	private Vector3 originalScale;

	[SerializeField] protected MeshRenderer[] outlineRenders;
	private Material material;
	float outlineWidth = .9f;

	//m_finished is set to true and usable set to false by the collectible upon entering the hole

	// Use this for initialization
	void Start () {
		base.Start ();

		material = new Material(Shader.Find("Outlined/Silhouette Only"));

		foreach (MeshRenderer r in outlineRenders) {
			r.material = material;
			r.material.SetFloat ("_Outline", outlineWidth);
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

		originalScale = new Vector3 (1f, 1f, 1f); //transform.lossyScale;

		//need a greater delay for the first story
		if (Time.timeSinceLevelLoad < 20) {
			storySoundCooldown = 18f;
		} else {
			storySoundCooldown = 1f;
		}

	}
	
	// Update is called once per frame
	void Update () {
		base.Update ();

		if (callOnce0 && storySoundCooldown < 0f) {
			float d = Vector3.Distance (LogicManager.Instance.GetPlayerHeadTransform ().position, transform.position);
			if (d < 3f) {
				callOnce0 = false;
				storySoundSource.Play ();
				storySoundCooldown = storySound.length + 3f;
			}
		}

		storySoundCooldown -= Time.deltaTime;
		hoverSoundCooldown -= Time.deltaTime;

		transform.localScale = originalScale; 

		if (storySoundCooldown > 0.0f && storySoundCooldown < 0.1f && callOnce1 && tag == "Tutorial") {
			callOnce1 = false;
		}
	}


	public AudioSource GetStorySoundSource(){
		return storySoundSource;
	}


	public override void InUseRange()
	{
		base.InUseRange ();

		//m_finished is set to true and usable set to false by the collectible upon entering the hole
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
        Color color;
		foreach (MeshRenderer r in outlineRenders) {
            ColorUtility.TryParseHtmlString("#00FFFFFF", out color);
            r.material.SetVector("_OutlineColor", color);
            r.enabled = isOn;
		}
	}
}
