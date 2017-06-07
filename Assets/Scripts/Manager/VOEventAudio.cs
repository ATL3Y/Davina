using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VOEventAudio : MonoBehaviour {

	[SerializeField] List<AudioClip> tutorialClips;
	[SerializeField] List<AudioClip> charactersClips;
	[SerializeField] List<AudioClip> endClips;

	protected int i=0;
	protected int j=0;

    protected AudioSource source;
	private bool end = false;

	private bool called0 = false;
    private bool called1 = false;

	// Use this for initialization
	void Start () 
	{
		if ( source == null) {
            source = gameObject.AddComponent<AudioSource> ();
            source.playOnAwake = source.loop = false;
            source.volume = 1f;
            source.spatialBlend = 1f;
		}
	}

	protected void OnEnable(){
		M_Event.logicEvents [(int)LogicEvents.EnterStoryTutorial] += OnEnterStoryTutorial;
		M_Event.logicEvents [(int)LogicEvents.Characters] += OnCharacters;
		M_Event.logicEvents [(int)LogicEvents.End] += OnEnd;
	}

	protected void OnDisable(){
		M_Event.logicEvents [(int)LogicEvents.EnterStoryTutorial] -= OnEnterStoryTutorial;
		M_Event.logicEvents [(int)LogicEvents.TransportEnd] -= OnCharacters;
		M_Event.logicEvents [(int)LogicEvents.End] -= OnEnd;
	}

	void OnEnterStoryTutorial( LogicArg arg ){
		if ( !called0 && tutorialClips.Count > 0) { 
			StartCoroutine(PlayNext ( tutorialClips ) );
            called0 = true;
        } 
	}

	void OnCharacters( LogicArg arg ){
		if (!called1 && charactersClips.Count > 0) { 
			StartCoroutine(PlayNext (charactersClips) );
			called1 = true;
		} 
	}

	void OnEnd( LogicArg arg ){
		if ( endClips.Count > 0) { 
			end = true;
			StartCoroutine(PlayNext (endClips) );
		} 
	}

	IEnumerator PlayNext( List<AudioClip> clips )
    {
        //yield return new WaitForSeconds( 2f ); //wait unitl match VO is over
		source.clip = clips[i];
		source.Play ();
		yield return new WaitForSeconds (source.clip.length);
		i++;
        if ( i < clips.Count )
        {
            StartCoroutine( PlayNext( clips ) );
        }
        else
        {
            i = 0;
			if ( end )
            {
                LogicArg logicArg = new LogicArg( this );
                M_Event.FireLogicEvent( LogicEvents.Credits, logicArg );
            }
        }
	}
}
