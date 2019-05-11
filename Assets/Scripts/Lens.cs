using UnityEngine;
using System.Collections;
using Valve.VR;
using System.Collections.Generic;
using DG.Tweening;

public class Lens : Interactable
{
    // set by container
    public AudioClip momSoundLight { get; set; } 
    public AudioClip momSoundDark { get; set; } 
    public AudioClip davSoundBeg { get; set; } 
    public AudioClip davSoundEnd { get; set; } 

    public AudioSource momSourceLight; 
    private float momSourceLightCooldown = 0f;
    public AudioSource momSourceDark; 
    private float momSourceDarkCoolDown = 0f;
    public AudioSource davSourceBeg; 
    private float davSourceBegCooldown = 0f;
    public AudioSource davSourceEnd;

    private bool momLightPlayed = false;
    private bool momDarkPlayed = false;
    private bool davBegPlayed = false;
    private bool transportEnabled = true; // Just always enable transport. 

    [SerializeField] LogicEvents onFillRaiseEvent;
    [SerializeField] LogicEvents onFillLowerEvent;

    public float Dot { get; set; }
    private bool lightSide = false;
    public bool LightSide { get { return lightSide; } }

    public MeshRenderer[] outlineRenders;
    float outlineWidth = .9f;

    private int sampleDataLength = 1024; 
    private float[] clipSampleData;

    public Color Color { get; set; }

    public static Lens instance;

    private Interactable mom;
    private Interactable davina;

    private float spawnCooldown = 1.0f;

    [SerializeField] private AudioSource _camBootUp;
    private bool _camBootUpPlayed = false;

    [SerializeField]
    private AudioSource _camTakePic;
    private bool _camTakePicPlayed = false;

    [SerializeField]
    private AudioSource _caughtAudio;

    private int outlineRendIndex = 0;

    private void TurnOnNextOutlineRend ( )
    {
        if(outlineRenders.Length > outlineRendIndex )
        {
            outlineRenders [ outlineRendIndex ].enabled = true;
        }
        outlineRendIndex++;
    }

    // Use this for initialization
    public override void Start ( )
    {
        base.Start ( );
        instance = this;
        // transportEnabled = false;
        if( LogicManager.Instance.VRRightHand.GetComponent<Hand> ( ) != null )
        {
            owner = LogicManager.Instance.VRRightHand.GetComponent<Hand> ( );
        }
        else if ( LogicManager.Instance.VRLeftHand.GetComponent<Hand> ( ) != null )
        {
            owner = LogicManager.Instance.VRLeftHand.GetComponent<Hand> ( );
        }
        else
        {
            Debug.Log ( "You need hands for this game." );
        }
        
        Dot = 0.0f;

        mom = GameObject.Find ( "Outline_Teleporter_Mom" ).GetComponent<Interactable> ( );
        davina = GameObject.Find( "Outline_Teleporter_Davina" ).GetComponent<Interactable> ( );

        if( mom == null )
        {
            Debug.Log ( "can't find mom" );
        }
        if ( davina == null )
        {
            Debug.Log ( "can't find davina" );
        }

        clipSampleData = new float [ sampleDataLength ];
        /*
        foreach ( MeshRenderer r in outlineRenders )
        {
            r.material.SetFloat ( "_Outline", outlineWidth );
            r.enabled = true;
        }
        */

        if ( momSoundLight != null )
        {
            momSourceLight = gameObject.AddComponent<AudioSource> ( );
            momSourceLight.playOnAwake = false;
            momSourceLight.loop = false;
            momSourceLight.volume = 1f;
            momSourceLight.spatialBlend = 1f;
            momSourceLight.clip = momSoundLight;
        }
        if ( momSoundDark != null )
        {
            momSourceDark = gameObject.AddComponent<AudioSource> ( );
            momSourceDark.playOnAwake = false;
            momSourceDark.loop = false;
            momSourceDark.volume = 1f;
            momSourceDark.spatialBlend = 1f;
            momSourceDark.clip = momSoundDark;
        }
        if ( davSoundBeg != null )
        {
            davSourceBeg = gameObject.AddComponent<AudioSource> ( );
            davSourceBeg.playOnAwake = false;
            davSourceBeg.loop = false;
            davSourceBeg.volume = 1f;
            davSourceBeg.spatialBlend = 1f;
            davSourceBeg.clip = davSoundBeg;
        }
        if ( davSoundEnd != null )
        {
            davSourceEnd = gameObject.AddComponent<AudioSource> ( );
            davSourceEnd.playOnAwake = false;
            davSourceEnd.loop = false;
            davSourceEnd.volume = 1f;
            davSourceEnd.spatialBlend = 1f;
            davSourceEnd.clip = davSoundEnd;
        }
    }

    // Update is called once per frame
    public override void Update ( )
    {
        base.Update ( );
        if ( m_finished )
        {
            owner = null;
            return;
        }

        momSourceLightCooldown -= Time.deltaTime;
        momSourceDarkCoolDown -= Time.deltaTime;
        davSourceBegCooldown -= Time.deltaTime;

        // Play what's facing us
        Dot = Vector3.Dot( transform.up, LogicManager.Instance.GetPlayerHeadTransform().forward );

        if ( Dot < 0.0f ) // light
        {
            lightSide = true;
            //Debug.Log ( "it's light" );

            // Sound vol

            // light level

            // ocean instensity 
        }
        else // dark
        {
            lightSide = false;
            //Debug.Log ( "it's dark" );
        }

        // If we are not playing light music, but it's light now, play light
        if( Dot < -0.9f && !Davina.MyAudioManager.MyInstance.GetBGMLight ( ))
        {
            Davina.MyAudioManager.MyInstance.ChangeBGM ( lightSide );
        }

        // If we are not playing dark music, but it's dark now, play dark
        if ( Dot > 0.9f && Davina.MyAudioManager.MyInstance.GetBGMLight ( ) )
        {
            Davina.MyAudioManager.MyInstance.ChangeBGM ( lightSide );
        }

        // Don't have overlapping audio
        if(!davSourceBeg.isPlaying && !momSourceLight.isPlaying && !momSourceDark.isPlaying )
        {
            // Is the player looking at 
            Vector3 headFor = LogicManager.Instance.GetPlayerHeadTransform().forward;
            Vector3 headPos = LogicManager.Instance.GetPlayerHeadTransform().position - 0.2f * headFor; // move pos back a tad
            Ray headRay = new Ray(headPos, headFor);
            Quaternion headRotation = Quaternion.LookRotation (headFor, LogicManager.Instance.GetPlayerHeadTransform().up);
            Vector3 headScale = 0.1f * LogicManager.Instance.GetPlayerHeadTransform().lossyScale;
            OBB headObb = new OBB(headPos, headRotation, headScale);

            // AxKDebugLines.AddLine( headPos, headPos + headFor, Color.cyan, 0);
            AxKDebugLines.AddOBB ( headObb, Color.red );

            // If Player looks through the lens to Davina, play Davina's story
            if ( this.IsInInteractionRange ( headPos, headRay, headObb ) )
            {
                //Debug.Log ( "lens in int " );
                if ( davina.IsInInteractionRange ( headPos, headRay, headObb ) )
                {
                    //Debug.Log ( "dav in int " );
                    if ( davSourceBegCooldown < 0.0f && !davSourceBeg.isPlaying )
                    {
                        //Debug.Log ( "ready " );
                        if ( Dot > 0.2f || Dot < -0.2f )
                        {
                            //Debug.Log ( "should play " );
                            if ( !davSourceBeg.isPlaying )
                            {
                                TurnOnNextOutlineRend ( );
                                _caughtAudio.Play ( );
                                davSourceBeg.Play ( );
                                davSourceBegCooldown = davSoundBeg.length + 8f;
                                davBegPlayed = true;
                            }
                        }
                    }
                }
            }

            // If Player looks through the lens to Mom, play Mom's story based on the direction of the lens
            if ( davBegPlayed )
            {
                if ( this.IsInInteractionRange ( headPos, headRay, headObb ) )
                {
                    if ( mom.IsInInteractionRange ( headPos, headRay, headObb ) )
                    {
                        if ( Dot > 0.3f ) // play dark
                        {
                            if ( momSourceDarkCoolDown < 0.0f )
                            {
                                if ( !momSourceDark.isPlaying )
                                {
                                    TurnOnNextOutlineRend ( );
                                    _caughtAudio.Play ( );
                                    momSourceDark.Play ( );
                                    momSourceDarkCoolDown = momSoundDark.length + 8f;
                                    momDarkPlayed = true;
                                }

                            }
                        }
                        else if ( Dot < -0.3f ) // light
                        {
                            if ( momSourceLightCooldown < 0.0f )
                            {
                                if ( !momSourceLight.isPlaying )
                                {
                                    TurnOnNextOutlineRend ( );
                                    _caughtAudio.Play ( );
                                    momSourceLight.Play ( );
                                    momSourceLightCooldown = momSoundLight.length + 8f;
                                    momLightPlayed = true;
                                }

                            }
                        }
                    }
                }
            }
        }

        // Spawn pulses for whatever is playing 
        if ( davSourceBeg.isPlaying )
        {
            ClipLoudness ( davSourceBeg, davina );
        }
        else if ( momSourceLight.isPlaying )
        {
            ClipLoudness ( momSourceLight, mom );
        }
        else if ( momSourceDark.isPlaying )
        {
            ClipLoudness ( momSourceDark, mom );
        }

        // Once stories listened to, enable the transport
        // Debug.Log ( "transportEnabled " + transportEnabled + " davBegPlayed " + davBegPlayed + " momLightPlayed " + momLightPlayed + " momDarkPlayed " + momDarkPlayed );
        if ( davBegPlayed && momLightPlayed && momDarkPlayed ) //!transportEnabled && 
        {

            // DO A PULSE UP / PULSE EFFECT.  
            // IF THE PLAYER CLICKS THE LEFT TRIGGER IN THIS STATE, MAKE A PICTURE TAKING SOUND
            // THEN CALL ONfINISHED()
            if ( !_camBootUpPlayed )
            {
                TurnOnNextOutlineRend ( );
                // _camBootUp.Play ( );
                _camBootUpPlayed = true;
            }

            if ( VRInputController.Instance.ReceivedLeftButtonDownSignal ( ))// || VRInputController.Instance.ReceivedRightButtonDownSignal ( ) )
            {
                if ( !_camTakePicPlayed )
                {
                    _camTakePic.Play ( );
                    _camTakePicPlayed = true;
                    OnFinished ( );
                }
            }
            

            // Send message to the Story Manager to transport to enable
            // LensManager.instance.EnableCamera ( );
            // transportEnabled = true;
        }
    }

    public void OnFinished ( )
    {
        // M_Event.FireLogicEvent ( onFillRaiseEvent, new LogicArg ( this ) );
        // SetOutline ( false );
        m_finished = true;

        if ( LightSide )
        {
            Score.Instance.SetScore ( 1.0f );
            // StartCoroutine ( DelaySoundClipPlay ( davSourceBeg, momSourceLight ) );
            CallNextEvent ( );
        }
        else
        {
            Score.Instance.SetScore ( -1.0f );
            // StartCoroutine ( DelaySoundClipPlay ( davSourceBeg, momSourceDark ) );
            CallNextEvent ( );
        }
    }

    IEnumerator DelaySoundClipPlay ( AudioSource davBeg, AudioSource momStory )
    {
        // Clear the slate
        if ( davSourceBeg.isPlaying )
        {
            davSourceBeg.Stop ( );
        }
        if ( momSourceLight.isPlaying )
        {
            momSourceLight.Stop ( );
        }
        if ( momSourceDark.isPlaying )
        {
            momSourceDark.Stop ( );
        }

        if ( !davBeg.isPlaying )
        {
            davBeg.Play ( );
            yield return new WaitForSeconds ( davBeg.clip.length );
        }

        if ( !momStory.isPlaying )
        {
            momStory.Play ( );
            yield return new WaitForSeconds ( momStory.clip.length );
        }


        CallNextEvent ( );

        /*
        davSourceEnd.Play ( );
        yield return new WaitForSeconds ( davSourceEnd.clip.length );
        */
    }

    public void CallNextEvent ( )
    {
        transform.DOLocalMove ( transform.position + Vector3.up * 100f, 10f ).SetEase ( DG.Tweening.Ease.InCirc );
        TransportManager.Instance.StationaryEffect ( LensManager.instance.observePos.position, false );
        M_Event.FireLogicEvent ( LogicEvents.EnterStory, new LogicArg ( this ) );
    }
    /*
    void SetOutline ( bool isOn )
    {
        foreach ( MeshRenderer r in outlineRenders )
        {
            r.enabled = isOn;
        }
    }
    */

    private void ClipLoudness ( AudioSource source, Interactable character )
    {
        spawnCooldown -= Time.deltaTime;

        if(spawnCooldown < 0.0f )
        {
            source.clip.GetData ( clipSampleData, source.timeSamples );
            float clipLoudness = 0.0f;
            foreach ( float sample in clipSampleData )
            {
                clipLoudness += Mathf.Abs ( sample );
            }

            if ( clipLoudness > 100.0f )
            {
                // Debug.Log ( "clip loudness " + clipLoudness );
                spawnCooldown = 1.0f;
                if( character.transform.parent.GetComponent<FXOutlinePulse> ( )  != null )
                {
                    // character.transform.parent.GetComponent<FXOutlinePulse> ( ).SpawnPulse ( );
                }
                else
                {
                    Debug.Log ( "FXOutlinePulse is null." );
                }
            }
        }
    }
}
