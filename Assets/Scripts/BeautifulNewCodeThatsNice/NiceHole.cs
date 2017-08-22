﻿using UnityEngine;
using System.Collections;

public class NiceHole : Interactable
{

	public AudioClip hoverSound;
    public AudioClip storySound;

    protected AudioSource hoverSoundSource;
	private float hoverSoundCooldown = 0f;
	public AudioSource storySoundSource;
	private float storySoundCooldown = 0f;
	private bool called = false;
	private Vector3 originalScale;

	public MeshRenderer[] outlineRenders;
	float outlineWidth = .9f;

    private int m_sampleDataLength = 1024; // 80ms
    private float[] m_clipSampleData;

    private Color color;
    public Color Color { set { color = value; } }

    // Use this for initialization
    public override void Start ()
    {
        base.Start();

        m_clipSampleData = new float[m_sampleDataLength];

        foreach (MeshRenderer r in outlineRenders)
        {
			r.material.SetFloat ("_Outline", outlineWidth);
		}

		SetOutline (true);

		if (hoverSound != null)
        {
			hoverSoundSource = gameObject.AddComponent<AudioSource> ();
			hoverSoundSource.playOnAwake = false;
			hoverSoundSource.loop = false;
			hoverSoundSource.volume = 1f;
			hoverSoundSource.spatialBlend = 1f;
			hoverSoundSource.clip = hoverSound;
		}

		if (storySound != null)
        {
            storySoundSource = gameObject.AddComponent<AudioSource>();
			storySoundSource.playOnAwake = false;
			storySoundSource.loop = false;
			storySoundSource.volume = 1f;
			storySoundSource.spatialBlend = 1f;
			storySoundSource.clip = storySound;
		}

		originalScale = transform.lossyScale;

		// Need delay for the first story
		if (Time.timeSinceLevelLoad < 20) {
			storySoundCooldown = 18f;
		} else {
			storySoundCooldown = 30f; // HACKLEY making coolDown huge so the story doesn't repeat more than twice 
		}

	}

    // Update is called once per frame
    public override void Update(){
		base.Update ();

		if (!called && storySoundCooldown < 0f)
        {
			float d = Vector3.Distance (LogicManager.Instance.GetPlayerHeadTransform ().position, transform.position);
			if (d < 3f)
            {
				called = true;
				storySoundSource.Play();
				storySoundCooldown = storySound.length + 1000f;
			}
		}

		storySoundCooldown -= Time.deltaTime;
		hoverSoundCooldown -= Time.deltaTime;

        // scale should not change 
		transform.localScale = originalScale; 
	}

	public AudioSource GetStorySoundSource()
    {
		return storySoundSource;
	}

	public override void InUseRange()
	{
		base.InUseRange ();

		//m_finished is set to true and usable set to false by the collectable upon entering the hole
		if (m_finished)
        {
            Debug.Log("hole is finished");
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
			storySoundCooldown = storySound.length + 1000f;
		}
	}

	public void SetOutline( bool isOn )
	{
		foreach (MeshRenderer r in outlineRenders)
        {
            r.enabled = isOn;
		}
	}
}
