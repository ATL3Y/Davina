using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class VOEventAudio : MonoBehaviour
{
	[SerializeField] List<AudioClip> tutorialClips;
	[SerializeField] List<AudioClip> charactersClips;
	[SerializeField] List<AudioClip> endWhiteClips;
    [SerializeField] List<AudioClip> endBlackClips;

    protected int i=0;

    protected AudioSource source;
	private bool end = false;

	private bool called0 = false;
    private bool called1 = false;
    public static VOEventAudio instance;
	// Use this for initialization
	void Start () 
	{
        instance = this;
		if (source == null)
        {
            source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = source.loop = false;
            source.volume = 1f;
            source.spatialBlend = 1f;
		}
	}

    private void Update()
    {

    }

    protected void OnEnable()
    {
		M_Event.logicEvents [(int)LogicEvents.Tutorial] += OnTutorial;
		M_Event.logicEvents [(int)LogicEvents.Characters] += OnCharacters;
        M_Event.logicEvents [(int)LogicEvents.End] += OnEnd;
    }

	protected void OnDisable()
    {
		M_Event.logicEvents [(int)LogicEvents.Tutorial] -= OnTutorial;
		M_Event.logicEvents [(int)LogicEvents.Characters] -= OnCharacters;
        M_Event.logicEvents [(int)LogicEvents.End] -= OnEnd;
    }

	void OnTutorial(LogicArg arg)
    {
		if (!called0 && tutorialClips.Count > 0)
        { 
			StartCoroutine(PlayNext(tutorialClips));
            called0 = true;
        } 
	}

	void OnCharacters(LogicArg arg)
    {
        i = 0;
		if (!called1 && charactersClips.Count > 0)
        { 
			StartCoroutine(PlayNext(charactersClips));
			called1 = true;
		} 
	}

    public void PlayOnWhiteEnding ( )
    {
        if ( endWhiteClips.Count > 0 )
        {
            end = true;
            StartCoroutine ( PlayNext ( endWhiteClips ) );
        }
    }

	void OnEnd(LogicArg arg )
    {
        if (SceneManager.GetActiveScene().buildIndex == 4)
        {
            if (endBlackClips.Count > 0)
            {
                end = true;
                StartCoroutine(PlayNext(endBlackClips));
            }
        }
	}

	IEnumerator PlayNext(List<AudioClip> clips)
    {
        //yield return new WaitForSeconds(2f); //wait unitl match VO is over
        // have the grandfather's line play from that game object
        if (SceneManager.GetActiveScene().buildIndex == 4 && i == 3)
        {
            AudioSourceTexture[] audioSourceTextures = GameObject.FindObjectsOfType<AudioSourceTexture>();
            for(int j = 0; j < audioSourceTextures.Length; j++)
            {
                if(audioSourceTextures[j].GetComponent<AudioSource>() != null)
                {
                    audioSourceTextures[j].GetComponent<AudioSource>().clip = clips[i];
                    audioSourceTextures[j].GetComponent<AudioSource>().Play();
                }
                else
                {
                    Debug.Log("There's no audiosource on the game object.");
                }
            }
        }
        else
        {
            source.clip = clips[i];
            source.Play();
        }
        
		yield return new WaitForSeconds (source.clip.length);
		i++;
        if (i < clips.Count)
        {
            StartCoroutine(PlayNext(clips));
        }
        else
        {
            i = 0;
			if (end)
            {
                Debug.Log("firing on credits");
                LogicArg logicArg = new LogicArg(this);
                M_Event.FireLogicEvent(LogicEvents.Credits, logicArg);
            }
        }
	}
}
