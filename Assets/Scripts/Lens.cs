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
    private bool transportEnabled; 

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
    


    // Use this for initialization
    public override void Start ( )
    {
        base.Start ( );
        instance = this;
        transportEnabled = false;
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

        foreach ( MeshRenderer r in outlineRenders )
        {
            r.material.SetFloat ( "_Outline", outlineWidth );
            r.enabled = true;
        }

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
        }
        else // dark
        {
            lightSide = false;
            //Debug.Log ( "it's dark" );
        }

        // If we are not playing light music, but it's light now, play light
        if( Dot < -0.9f && !AudioManager.Instance.GetBGMLight ( ))
        {
            AudioManager.Instance.ChangeBGM ( lightSide );
        }

        // If we are not playing dark music, but it's dark now, play dark
        if ( Dot > 0.9f && AudioManager.Instance.GetBGMLight ( ) )
        {
            AudioManager.Instance.ChangeBGM ( lightSide );
        }

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
                    if ( Dot > 0.4f || Dot < -0.4f )
                    {
                        //Debug.Log ( "should play " );
                        davSourceBeg.Play ( );
                        davSourceBegCooldown = davSoundBeg.length + 4f;
                        davBegPlayed = true;
                    }
                }
            }
        }

        // If Player looks through the lens to Mom, play Mom's story based on the direction of the lens
        if ( davBegPlayed )
        {
            if( this.IsInInteractionRange ( headPos, headRay, headObb ) )
            {
                if( mom.IsInInteractionRange ( headPos, headRay, headObb ) )
                {
                    if ( !davSourceBeg.isPlaying )
                    {
                        if ( Dot > 0.4f ) // play dark
                        {
                            if ( momSourceLight.isPlaying )
                            {
                                momSourceLight.Stop ( );
                            }

                            if ( momSourceDarkCoolDown < 0.0f )
                            {
                                momSourceDark.Play ( );
                                momSourceDarkCoolDown = momSoundDark.length + 4f;
                                momDarkPlayed = true;
                            }
                        }
                        else if ( Dot < -0.4f ) // light
                        {
                            if ( momSourceDark.isPlaying )
                            {
                                momSourceDark.Stop ( );
                            }

                            if ( momSourceLightCooldown < 0.0f )
                            {
                                momSourceLight.Play ( );
                                momSourceLightCooldown = momSoundLight.length + 4f;
                                momLightPlayed = true;
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
        Debug.Log ( "transportEnabled " + transportEnabled + " davBegPlayed " + davBegPlayed + " momLightPlayed " + momLightPlayed + " momDarkPlayed " + momDarkPlayed );
        if ( !transportEnabled && davBegPlayed && momLightPlayed && momDarkPlayed )
        {
            // Send message to the Story Manager to transport to enable
            LensManager.instance.EnableCamera ( );
            transportEnabled = true;
        }
    }
    // called by teleporter
    public void OnFinished ( )
    {
        // M_Event.FireLogicEvent ( onFillRaiseEvent, new LogicArg ( this ) );
        // SetOutline ( false );
        m_finished = true;

        if ( LightSide )
        {
            Score.Instance.SetScore ( 1.0f );
            StartCoroutine ( DelaySoundClipPlay ( davSourceBeg, momSourceLight ) );
        }
        else
        {
            Score.Instance.SetScore ( -1.0f );
            StartCoroutine ( DelaySoundClipPlay ( davSourceBeg, momSourceDark ) );
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

        davBeg.Play ( );
        yield return new WaitForSeconds ( davBeg.clip.length );

        momStory.Play ( );
        yield return new WaitForSeconds ( momStory.clip.length );
        CallNextEvent ( );

        /*
        davSourceEnd.Play ( );
        yield return new WaitForSeconds ( davSourceEnd.clip.length );
        */
    }

    public void CallNextEvent ( )
    {
        transform.DOLocalMove ( transform.position + Vector3.up * 100f, 10f ).SetEase ( DG.Tweening.Ease.InCirc );
        TransportManager.Instance.StationaryEffect ( LensManager.instance.observePos.position );
        M_Event.FireLogicEvent ( LogicEvents.EnterStory, new LogicArg ( this ) );
    }

    void SetOutline ( bool isOn )
    {
        foreach ( MeshRenderer r in outlineRenders )
        {
            r.enabled = isOn;
        }
    }

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
                    character.transform.parent.GetComponent<FXOutlinePulse> ( ).SpawnPulse ( );
                }
                else
                {
                    Debug.Log ( "FXOutlinePulse is null." );
                }
            }
        }
    }
}
