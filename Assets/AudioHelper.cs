using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioHelper : MonoBehaviour {

	[SerializeField] protected List<AudioClip> finalClips;

	private bool once = false;

	protected int i=0;


	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{

	}

	private void OnTriggerEnter(Collider other)
	{
		if (!once && other.gameObject.tag == "Player") {
			print ("player desends");
			PlayClips ();
			once = true;
		} 

	}

	private void PlayClips(){
		while (finalClips [i] != null) {
			AudioSource source = gameObject.AddComponent<AudioSource> ();
			source.clip = finalClips [i];
			source.playOnAwake = source.loop = false;
			source.volume = 1f;
			source.spatialBlend = 1f;

			source.Play ();
			while (source.isPlaying) {
				continue;
			}

			Destroy (source);
			i++;
		}
	}
}
