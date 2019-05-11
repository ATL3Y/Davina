using UnityEngine;
using System.Collections;
using DG.Tweening;

/// <summary>
/// manage the sound effect
/// play the sound effect when recieve an event 
/// 
/// </summary>
public class AudioManager : MBehavior
{
    public AudioManager ( ) { s_Instance = this; }
    public static AudioManager Instance { get { return s_Instance; } }
    private static AudioManager s_Instance;

    /// <summary>
    /// input pair for recording the input sound effect
    /// </summary>
    [System.Serializable]
    public struct InputClipPair
    {
        public MInputType input;
        public AudioClip clip;
    };

    [SerializeField] InputClipPair[] InputClipPairs;

    /// <summary>
    /// for pairing the logic event and the sound effect
    /// </summary>
    [System.Serializable]
    public struct LogicClipPair
    {
        public LogicEvents type;
        public AudioClip clip;
    };

    [SerializeField] LogicClipPair[] LogicClipPairs;
    [SerializeField] AudioClip mainBGM;
    [SerializeField] AudioClip sadBGM;
    [SerializeField] AudioClip happyBGM;
    private AudioSource bgmSource;

    protected override void MAwake ( )
    {
        base.MAwake ( );
        SwitchBGM ( mainBGM );
    }

    protected override void MOnEnable ( )
    {
        base.MOnEnable ( );
        for ( int i = 0; i < System.Enum.GetNames ( typeof ( MInputType ) ).Length; ++i )
        {
            M_Event.inputEvents [ i ] += OnInputEvent;
        }

        for ( int i = 0; i < System.Enum.GetNames ( typeof ( LogicEvents ) ).Length; ++i )
        {
            M_Event.logicEvents [ i ] += OnLogicEvent;
        }

        M_Event.logicEvents [ ( int ) LogicEvents.EnterInnerWorld ] += OnEnterInnerWorld;
        M_Event.logicEvents [ ( int ) LogicEvents.ExitInnerWorld ] += OnExitInnerWorld;
        M_Event.logicEvents [ ( int ) LogicEvents.Finale ] += OnFinale;
        M_Event.logicEvents [ ( int ) LogicEvents.End ] += OnEnd;
        M_Event.logicEvents [ ( int ) LogicEvents.Credits ] += OnCredits;
    }

    protected override void MOnDisable ( )
    {
        base.MOnDisable ( );
        for ( int i = 0; i < System.Enum.GetNames ( typeof ( MInputType ) ).Length; ++i )
        {
            M_Event.inputEvents [ i ] -= OnInputEvent;
        }

        for ( int i = 0; i < System.Enum.GetNames ( typeof ( LogicEvents ) ).Length; ++i )
        {
            M_Event.logicEvents [ i ] -= OnLogicEvent;
        }

        M_Event.logicEvents [ ( int ) LogicEvents.EnterInnerWorld ] -= OnEnterInnerWorld;
        M_Event.logicEvents [ ( int ) LogicEvents.ExitInnerWorld ] -= OnExitInnerWorld;
        M_Event.logicEvents [ ( int ) LogicEvents.Finale ] -= OnFinale;
        M_Event.logicEvents [ ( int ) LogicEvents.End ] -= OnEnd;
        M_Event.logicEvents [ ( int ) LogicEvents.Credits ] -= OnCredits;
    }

    void OnInputEvent ( InputArg input )
    {
        foreach ( InputClipPair pair in InputClipPairs )
        {
            if ( pair.input == input.type )
            {
                StartCoroutine ( PlayerClip ( pair.clip ) );
            }
        }
    }

    void OnLogicEvent ( LogicArg logicEvent )
    {
        foreach ( LogicClipPair pair in LogicClipPairs )
        {
            if ( pair.type == logicEvent.type )
            {
                StartCoroutine ( PlayerClip ( pair.clip ) );
            }
        }
    }

    IEnumerator PlayerClip ( AudioClip clip )
    {
        if ( clip == null )
            yield break;

        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.playOnAwake = source.loop = false;
        if(!source.isPlaying)
            source.Play ( );

        while ( source.isPlaying )
        {
            yield return null;
        }

        Destroy ( source );
    }

    void OnEnterInnerWorld ( LogicArg arg )
    {
        AudioClip clip = (AudioClip)arg.GetMessage (Global.EVENT_LOGIC_ENTERINNERWORLD_CLIP);
        if ( clip != null )
        {
            SwitchBGM ( clip );
        }
    }

    void OnExitInnerWorld ( LogicArg arg )
    {
        SwitchBGM ( mainBGM );
    }

    void OnFinale ( LogicArg arg )
    {
        SwitchBGM ( happyBGM );
    }

    public void ChangeBGM ( bool light )
    {
        if ( light )
        {
            SwitchBGM ( happyBGM );
        }
        else
        {
            SwitchBGM ( sadBGM );
        }
    }
    public bool GetBGMLight ( )
    {
        if(bgmSource.clip == happyBGM )
        {
            return true;
        }
        else if(bgmSource.clip == sadBGM )
        {
            return false;
        }
        return false;
    }

    void OnEnd(LogicArg arg)
    {
		//SwitchBGM(sadBGM);
	}

	void OnCredits(LogicArg arg)
    {
		//SwitchBGM(happyBGM);
	}

	void SwitchBGM(AudioClip to)
	{
		if (bgmSource == null)
        {
			bgmSource = gameObject.AddComponent<AudioSource> ();
			bgmSource.loop = true;
			bgmSource.volume = .5f;
			bgmSource.spatialBlend = 1f;
		}

		if (bgmSource != null)
        {
			bgmSource.DOFade (0, 1f).OnComplete (delegate {
				bgmSource.clip = to;
				bgmSource.time = Random.Range (0, bgmSource.clip.length);
                if(!bgmSource.isPlaying)
                    bgmSource.Play();
				bgmSource.DOFade(.5f, 1f);
			});
		}
	}
}

