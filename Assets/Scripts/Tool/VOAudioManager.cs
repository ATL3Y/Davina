using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TriggerAudioFX
    : MonoBehaviour {

	[SerializeField] List<AudioClip> enterClips;
	[SerializeField] List<AudioClip> exitClips;

	private bool onceEnter = false;
	private bool onceExit = false;
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
	/*
	void OnTriggerEnter(Collider other)
	{
		if (!onceEnter && other.gameObject.tag == "Player" && enterClips.Count > 0) { //col.gameObject.tag == "Player"

			StartCoroutine(PlayNextEnter ());
			onceEnter = true;
		} 
	}
	void OnTriggerExit(Collider other)
	{
		if (!onceExit && other.gameObject.tag == "Player" && exitClips.Count > 0) { //col.gameObject.tag == "Player"
			
			StartCoroutine(PlayNextExit ());
			onceExit = true;
		} 
	}
	*/
	protected void OnEnable(){

		M_Event.logicEvents [(int)LogicEvents.Characters] += OnCharacters;
		M_Event.logicEvents [(int)LogicEvents.End] += OnEnd;
	}

	protected void OnDisable(){

		M_Event.logicEvents [(int)LogicEvents.Characters] -= OnCharacters;
		M_Event.logicEvents [(int)LogicEvents.End] -= OnEnd;
	}

	void OnCharacters( LogicArg arg ){
		if (!onceEnter && enterClips.Count > 0) { //col.gameObject.tag == "Player"

			StartCoroutine(PlayNextEnter ());
			onceEnter = true;
		} 
	}

	void OnEnd( LogicArg arg ){
		if (!onceEnter && enterClips.Count > 0) { //col.gameObject.tag == "Player"

			StartCoroutine(PlayNextEnter ());
			onceEnter = true;
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
		} else if (gameObject.tag == "End") {
			LogicArg logicArg = new LogicArg (this);
			M_Event.FireLogicEvent (LogicEvents.Credits, logicArg);
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
