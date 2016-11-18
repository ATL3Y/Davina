using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VOEventAudio : MonoBehaviour {

	[SerializeField] List<AudioClip> clips1;
	[SerializeField] List<AudioClip> clips2;

	protected int i=0;
	protected int j=0;

    protected AudioSource source;
	private bool end = false;
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
	
	// Update is called once per frame
	void Update () 
	{

	}

	protected void OnEnable(){

		M_Event.logicEvents [(int)LogicEvents.Characters] += OnCharacters;
		M_Event.logicEvents [(int)LogicEvents.End] += OnEnd;
	}

	protected void OnDisable(){

		M_Event.logicEvents [(int)LogicEvents.Characters] -= OnCharacters;
		M_Event.logicEvents [(int)LogicEvents.End] -= OnEnd;
	}

	void OnCharacters( LogicArg arg ){
		if ( clips1.Count > 0) { 
			StartCoroutine(PlayNext ( clips1 ) );
		} 
	}

	void OnEnd( LogicArg arg ){
		if ( clips2.Count > 0) { 
			end = true;
			StartCoroutine(PlayNext ( clips2 ) );
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
