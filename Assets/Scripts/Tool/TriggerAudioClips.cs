using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TriggerAudioClips: MonoBehaviour {
    
	[SerializeField] List<AudioClip> enterClips;
	[SerializeField] List<AudioClip> exitClips;

	private bool callOnceEnter = true;
	private bool callOnceExit = true;
	protected int i=0;
	protected int j=0;

	protected AudioSource sourceEnter;
	protected AudioSource sourceExit;

	// Use this for initialization
	void Start () 
	{

		if (enterClips != null) {
			sourceEnter = gameObject.AddComponent<AudioSource> ();
			sourceEnter.playOnAwake = sourceEnter.loop = false;
			sourceEnter.volume = 1f;
			sourceEnter.spatialBlend = 1f;
		}

		if (exitClips != null) {
			sourceExit = gameObject.AddComponent<AudioSource> ();
			sourceExit.playOnAwake = sourceExit.loop = false;
			sourceExit.volume = 1f;
			sourceExit.spatialBlend = 1f;
		}

	}
	
	// Update is called once per frame
	void Update () 
	{

	}
	
	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player" && enterClips.Count > 0) { //callOnceEnter &&

			StartCoroutine(PlayNextEnter ());
			callOnceEnter = false;
		} 
	}
	void OnTriggerExit(Collider other)
	{
		if ( callOnceExit && other.gameObject.tag == "Player" && exitClips.Count > 0) { 
			
			StartCoroutine(PlayNextExit ());
			callOnceExit = false;
		} 
	}

	IEnumerator PlayNextEnter(){
			
		sourceEnter.clip = enterClips [i];
		sourceEnter.Play ();
		//print ("playing clips " + sourceEnter.clip.name);
		yield return new WaitForSeconds (sourceEnter.clip.length);
		i++;
		if (i < enterClips.Count) {
			StartCoroutine (PlayNextEnter ());
		} 
	}

	IEnumerator PlayNextExit(){

		sourceExit.clip = exitClips [j];
		sourceExit.Play ();
		//print ("playing clips at " + j); // + sourceEnter.clip.name);
		yield return new WaitForSeconds (sourceExit.clip.length);
		j++;
		if (j < exitClips.Count) {
			StartCoroutine(PlayNextExit ());
		}
	}
}
