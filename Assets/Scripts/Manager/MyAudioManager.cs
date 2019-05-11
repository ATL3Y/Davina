using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace Davina
{



/// <summary>
/// manage the sound effect
/// play the sound effect when recieve an event 
/// 
/// </summary>
public class MyAudioManager : MBehavior
{
    public MyAudioManager ( ) { s_Instance = this; }
    public static MyAudioManager MyInstance { get { return s_Instance; } }
    private static MyAudioManager s_Instance;

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
    private AudioSource[] bgmSource;

    protected override void MAwake ( )
    {
        base.MAwake ( );
        
    }

    protected override void MStart ( )
    {
        base.MStart ( );


        bgmSource = new AudioSource [ 3 ];
        for ( int i = 0; i < bgmSource.Length; i++ )
        {
            bgmSource [ i ] = gameObject.AddComponent<AudioSource> ( );
            bgmSource [ i ].playOnAwake = false;
            bgmSource [ i ].loop = false;
            bgmSource [ i ].volume = 1f;
            bgmSource [ i ].spatialBlend = 1f;
        }
        if ( bgmSource [ 0 ] != null )
        {
            bgmSource [ 0 ].clip = mainBGM;
        }
        else
        {
            Debug.Log ( "mainBGM not set." );
        }
        if ( bgmSource [ 1 ] != null )
        {
            bgmSource [ 1 ].clip = happyBGM;
        }
        else
        {
            Debug.Log ( "happyBGM not set." );
        }
        if ( bgmSource [ 2 ] != null )
        {
            bgmSource [ 2 ].clip = sadBGM;
        }
        else
        {
            Debug.Log ( "sadBGM not set." );
        }

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

        for ( int i = 0; i < bgmSource.Length; i++ )
        {
            if ( bgmSource [ i ].isPlaying && bgmSource [ i ].volume > .25f )
            {
                if ( bgmSource [ i ].clip == happyBGM )
                {
                    return true;
                }
                else if ( bgmSource [ i ].clip == sadBGM )
                {
                    return false;
                }
                else if ( bgmSource [ i ].clip == mainBGM )
                {
                    return false; // ??
                }

            }
        }
        return false;
    }

    void OnEnd ( LogicArg arg )
    {
        //SwitchBGM(sadBGM);
    }

    void OnCredits ( LogicArg arg )
    {
        //SwitchBGM(happyBGM);
    }

    void SwitchBGM ( AudioClip to ) // HACK2
    {
        /*
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
				bgmSource.Play();
				bgmSource.DOFade(.5f, 1f);
			});
		}
        */

        for ( int i = 0; i < bgmSource.Length; i++ )
        {
            if ( bgmSource [ i ].isPlaying && bgmSource [ i ].clip != to )
            {
                bgmSource [ i ].DOFade ( 0f, 1f ).OnComplete ( delegate
                {
                    bgmSource [ i ].Pause ( );
                } );
            }

            if ( to == bgmSource [ i ].clip && !bgmSource [ i ].isPlaying )
            {
                bgmSource [ i ].DOFade ( .5f, 1f );
                // don't think we need to store pause and play times with pause and separate sources, but check? 
                bgmSource [ i ].Play ( );
            }
        }
    }
}

}

