using UnityEngine;
using System.Collections;
using Valve.VR;
using System.Collections.Generic;
using DG.Tweening;

public class NiceCollectable : Interactable
{
    // set by container
    public AudioClip storySoundL { get; set; } // cap later - light
    public AudioClip storySoundR { get; set; } // cap later - dark
    public NiceHole niceHole { get; set; } // cap later

    public AudioSource storySoundSourceL; // light
    private float storySoundCooldownL = 0f;
    public AudioSource storySoundSourceR; // dark
    private float storySoundCooldownR = 0f;

    [SerializeField] LogicEvents onFillRaiseEvent;
    [SerializeField] LogicEvents onFillLowerEvent;

    Vector3 offset;
    Quaternion rotationOffset;

    [SerializeField] protected AudioClip hoverSound;
    protected AudioSource hoverSoundSource;
    private float hoverSoundCooldown = 0f;
    [SerializeField] protected AudioClip pickUpSound;
    protected AudioSource pickUpSoundSource;
    [SerializeField] protected AudioClip putDownSound;
    protected AudioSource putDownSoundSource;

    Vector3 originalScale;

    private bool lightSideOut = false;
    public bool LightSideOut{ get { return lightSideOut; } } 
    private bool untouched = true;

	public MeshRenderer[] outlineRenders;
	float outlineWidth = .9f;

	bool called = false;
    bool called2 = false;

    private int m_sampleDataLength = 1024; // 80ms
    private float[] m_clipSampleData;

    private Color color;
    public Color Color { set { color = value; } }

    // Use this for initialization
    public override void Start()
    {
		base.Start();

        m_clipSampleData = new float[m_sampleDataLength];

        foreach (MeshRenderer r in outlineRenders)
        {
			r.material.SetFloat ("_Outline", outlineWidth);
            r.enabled = true;
        }

		originalScale = transform.lossyScale; 

		if (hoverSound != null)
        {
			hoverSoundSource = gameObject.AddComponent<AudioSource> ();
			hoverSoundSource.playOnAwake = false;
			hoverSoundSource.loop = false;
			hoverSoundSource.volume = 1f;
			hoverSoundSource.spatialBlend = 1f;
			hoverSoundSource.clip = hoverSound;
		}
		// set up the select sound
		if (pickUpSound != null)
        {
			pickUpSoundSource = gameObject.AddComponent<AudioSource> ();
			pickUpSoundSource.playOnAwake = false;
			pickUpSoundSource.loop = false;
			pickUpSoundSource.volume = 1f;
			pickUpSoundSource.spatialBlend = 1f;
			pickUpSoundSource.clip = pickUpSound;
		}
		// set up the unselect sound
		if (putDownSound != null)
        {
			putDownSoundSource = gameObject.AddComponent<AudioSource> ();
			putDownSoundSource.playOnAwake = false;
			putDownSoundSource.loop = false;
			putDownSoundSource.volume = 1f;
			putDownSoundSource.spatialBlend = 1f;
			putDownSoundSource.clip = putDownSound;
		}
		// set up the story sound R
		if (storySoundR != null)
        {
            storySoundSourceR = gameObject.AddComponent<AudioSource>();
			storySoundSourceR.playOnAwake = false;
			storySoundSourceR.loop = false;
			storySoundSourceR.volume = 1f;
			storySoundSourceR.spatialBlend = 1f;
			storySoundSourceR.clip = storySoundR;
		}
		// set up the story sound L
		if (storySoundL != null)
        {
            if (storySoundSourceL == null)
            {
                storySoundSourceL = gameObject.AddComponent<AudioSource>();
            }

			storySoundSourceL.playOnAwake = false;
			storySoundSourceL.loop = false;
			storySoundSourceL.volume = 1f;
			storySoundSourceL.spatialBlend = 1f;
			storySoundSourceL.clip = storySoundL;
		}
	}

	float EaseOutCubic(float value)
	{
		float start = 0.0f;
		float end = 1.0f;
		value--;
		end -= start;
		return end * (value * value * value + 1) + start;
	}

	float Ease( float t, float b, float c, float d )
	{
		float ts = ( t /= d ) * t;
		float tc = ts * t;
		return b + c * ( 0.95f * tc * ts + -5.1425f * ts * ts + 10.29f * tc + -10.395f * ts + 5.297f * t);
	}

    private bool calledReleaseInstructions = false;

	// Update is called once per frame
	public override void Update()
    {
		base.Update();

        storySoundCooldownR -= Time.deltaTime;
		storySoundCooldownL -= Time.deltaTime;
		hoverSoundCooldown -= Time.deltaTime;
		
		if (m_finished)
        {
			owner = null;
			return;
		}

        //AxKDebugLines.AddLine(transform.position, transform.position + transform.up * .3f, Color.green, 0);
        //AxKDebugLines.AddLine(niceHole.transform.position, niceHole.transform.position + niceHole.transform.up * .3f, Color.green, 0);

        transform.localScale = originalScale + originalScale.normalized * EaseOutCubic(m_hoverTime * .1f);

        // If we're at Davina, play what's facing Davina
        float dot = dot = Vector3.Dot(transform.up, niceHole.transform.up);

        if (dot > 0.0f) // light -- the white side is in the +y direction
            lightSideOut = true;
        else // dark
            lightSideOut = false;

        if (owner)
        {
            untouched = false;
			if (!called)
            {
				TextInstructions.Instance.PickedUpCollectable();
				called = true;
			}
			bool dropable = true;
			//if ( TransportManager.Instance.IsTransporting ) print( "transporting" );
			if (TransportManager.Instance.IsTransporting)
            {	
				dropable = false;
				transform.SetParent(null);
			}

			Vector3 newPosition =  owner.transform.position + owner.transform.TransformDirection (offset);
			float distanceToTarget = Vector3.Magnitude (newPosition - niceHole.transform.position);
			Quaternion newRotation = owner.transform.rotation * rotationOffset;

			// Play story sound dot
			if (dot < -0.2f) // dark
            {
				if (storySoundSourceL != null && storySoundSourceL.isPlaying)
                {
					storySoundSourceL.Stop();
				}

				if (storySoundSourceR != null && storySoundCooldownR < 0.0f && !niceHole.storySoundSource.isPlaying)
                {
					storySoundSourceR.Play();
					storySoundCooldownR = storySoundR.length + 6f;
				} 
			}
            else if (dot > 0.2f) // light
            {
				if (storySoundSourceR != null && storySoundSourceR.isPlaying)
                {
					storySoundSourceR.Stop();
				}

				if (storySoundSourceL != null && storySoundCooldownL < 0.0f && !niceHole.storySoundSource.isPlaying)
                {
					storySoundSourceL.Play();
					storySoundCooldownL = storySoundL.length + 6f;
				} 
			} 

			if (distanceToTarget < 0.4f)
            {
                if (!calledReleaseInstructions)
                {
                    TextInstructions.Instance.OnReleaseRange();
                    calledReleaseInstructions = true;
                }

                // orient collectable in hole based on relation to hole
                float t = Mathf.Clamp01(1.0f - (distanceToTarget * (1.0f / 0.1f)));
                t = Ease(t, 0.0f, 1.0f, 1.0f);

                Quaternion rot = new Quaternion();
                
                if (Vector3.Dot(transform.up, niceHole.transform.up) > 0f)
                {
                    rot = niceHole.transform.rotation;
                }
                else
                {
                    rot = Quaternion.AngleAxis(180f, niceHole.transform.right) * niceHole.transform.rotation;
                }

				newPosition = Vector3.Lerp (newPosition, niceHole.transform.position, t);
				newRotation = Quaternion.Slerp (newRotation, rot, t);

                float dist = 5000f / (distanceToTarget * 100f);
                ushort distUS = (ushort)dist;

                if (owner.left)
                {
                    SteamVR_Controller.Input(ViveInputController.Instance.leftControllerIndex).TriggerHapticPulse(distUS);
                }
                else
                {
                    SteamVR_Controller.Input(ViveInputController.Instance.rightControllerIndex).TriggerHapticPulse(distUS);
                }
            }

			transform.position = newPosition;
			transform.rotation = newRotation;

			if ( dropable && ( (owner.left && !ViveInputController.Instance.ReceivedLeftButtonPressSignal ())
				|| (!owner.left && !ViveInputController.Instance.ReceivedRightButtonPressSignal ())))
            {
				owner = null;
				if (putDownSound)
                {
					putDownSoundSource.Play();
				}
			}
		}
        else
        {
			float distanceToHole = Vector3.Magnitude (transform.position - niceHole.transform.position);

			if (distanceToHole < 0.5f)
            {
				useable = false;
				niceHole.useable = false;

				m_finished = true;
				niceHole.SetFinished(true);

                SetOutline(false);
                niceHole.SetOutline(false);

                Vector3 rot;
				if (Vector3.Dot(transform.up, niceHole.transform.up) > 0f) // light
                {
                    lightSideOut = true;

                    if (!called2)
                    {
                        // Immediate FX
                        Score.Instance.SetScore(1f); 
                        M_Event.FireLogicEvent(onFillRaiseEvent, new LogicArg(this));
                        called2 = true;
                    }
                    
                    rot = new Vector3(0f, 0f, 0f);
					if (storySoundSourceR != null && storySoundSourceR.isPlaying)
                    {
						storySoundSourceR.Stop();
					}

                    CallNextEvent();

                    /*
					if (storySoundSourceL != null)
                    {
						StartCoroutine (DelaySoundClipPlay (niceHole.GetStorySoundSource (), storySoundSourceL));
					}
                    else
                    {
						CallNextEvent();
					}
                    */
                }
                else // dark
                {
                    lightSideOut = false;

                    if (!called2)
                    {
                        // Immediate FX
                        Score.Instance.SetScore(-1f);
                        M_Event.FireLogicEvent(onFillLowerEvent, new LogicArg(this));
                        called2 = true;
                    }

                    rot = new Vector3 (0f, 0f, 180f);
					if (storySoundSourceL != null && storySoundSourceL.isPlaying)
                    {
						storySoundSourceL.Stop();
					}

                    CallNextEvent();
                    /*
					if (storySoundSourceR != null)
                    {
						StartCoroutine (DelaySoundClipPlay (niceHole.GetStorySoundSource(), storySoundSourceR));
					}
                    else
                    {
						CallNextEvent();
					}
                    */
                }
                transform.parent = niceHole.gameObject.transform;
                float newScale = transform.localScale.x / transform.parent.localScale.x;
                transform.DOScale(newScale, 1f).SetEase(DG.Tweening.Ease.InCirc); // 1.06f
                transform.DOLocalRotate ( rot, 1f).SetEase (DG.Tweening.Ease.InCirc);
                transform.DOLocalMove(Vector3.zero, 1f).SetEase(DG.Tweening.Ease.InCirc);
            }

            if(!owner && untouched)
            {

            }

            if (!owner && !untouched)
            {
                // rotate continuously after player grabbed object the first time.
                Quaternion turn = Quaternion.AngleAxis(10f, Vector3.forward) * transform.localRotation;
                transform.localRotation = Quaternion.Slerp(transform.localRotation, turn, Time.deltaTime * 1.5f);
            }
        }       
	}

    IEnumerator DelaySoundClipPlay(AudioSource holeStory, AudioSource collectableStory)
	{
        // HACKLEY uhh... try to make sure other things aren't playing 
        if (storySoundSourceL != null && storySoundSourceL.isPlaying)
        {
            storySoundSourceL.Stop();
        }
        if (storySoundSourceR != null && storySoundSourceR.isPlaying)
        {
            storySoundSourceR.Stop();
        }

        if (holeStory == null)
        {
            Debug.Log("need hole story");
        }
        else
        {
            holeStory.Play();
            yield return new WaitForSeconds(holeStory.clip.length);
        }

        if (storySoundSourceL != null && storySoundSourceL.isPlaying)
        {
            storySoundSourceL.Stop();
        }
        if (storySoundSourceR != null && storySoundSourceR.isPlaying)
        {
            storySoundSourceR.Stop();
        }

        if (collectableStory == null)
        {
            Debug.Log("need collectable story");
        }
        else
        {
            collectableStory.Play();
            yield return new WaitForSeconds(collectableStory.clip.length);
            CallNextEvent();
        }
	}

	public override void Use(Hand hand)
	{
		rotationOffset = Quaternion.Inverse (hand.transform.rotation) *  transform.rotation;
		offset = hand.transform.InverseTransformDirection (transform.position - hand.transform.position);

		owner = hand;

		if (pickUpSound)
        {
			pickUpSoundSource.Play();
		}
	}
		
	public override void InUseRange()
	{
		base.InUseRange();

		if (m_finished)
        {
			owner = null;
			return;
		}

		if (hoverSoundCooldown > 0.0f)
			return;

		if (hoverSound && !hoverSoundSource.isPlaying)
        {
			hoverSoundSource.Play();
			hoverSoundCooldown = hoverSound.length + 1f;
		}
    }

	void CallNextEvent()
	{
        M_Event.FireLogicEvent(LogicEvents.EnterStory, new LogicArg(this));
	}

	void SetOutline(bool isOn)
	{
        foreach (MeshRenderer r in outlineRenders)
        {
			r.enabled = isOn;
		}
	}
}
