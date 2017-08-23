using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityStandardAssets.ImageEffects;

public class TransportManager : MBehavior
{
	public TransportManager() { s_Instance = this; }
	public static TransportManager Instance { get { return s_Instance; } }
	private static TransportManager s_Instance;

	[SerializeField] ToColorEffect toColorEffect;
	[SerializeField] BloomAndFlares bloomAndFlares;
	[SerializeField] float fadeTime = .5f;
	[SerializeField] float transportTime = 1.82f;
	[SerializeField] LineRenderer transportLine;
	[SerializeField] ParticleSystem transportCircle;

    private bool finale = false;
	private bool credits = false;

	[SerializeField] FinaleTrailEnable TrailLeft;
	[SerializeField] FinaleTrailEnable TrailRight;

	/// <summary>
	/// For the transport animation
	/// </summary>
	private Sequence transportSequence;
	public bool IsTransporting
    {
		get
        { 
			if(transportSequence != null) 
				return !transportSequence.IsComplete();
			return false;
		}
	}

	protected override void MAwake()
	{
		base.MAwake();
	}

	protected override void MStart()
	{
		base.MStart();
		toColorEffect = LogicManager.Instance.GetPlayerTransform().GetComponentInChildren<ToColorEffect>();
		bloomAndFlares = LogicManager.Instance.GetPlayerTransform().GetComponentInChildren<BloomAndFlares>();
	}

	protected override void MOnEnable()
	{
		base.MOnEnable();
		M_Event.inputEvents[(int)MInputType.Transport] += OnTransport;
		M_Event.inputEvents[(int)MInputType.FocusNewObject] += OnFocusNew;
		M_Event.inputEvents[(int)MInputType.OutOfFocusObject] += OnOutofFocus;
        M_Event.logicEvents[(int)LogicEvents.Finale] += OnFinale;
        M_Event.logicEvents[(int)LogicEvents.End] += OnEnd;
		M_Event.logicEvents[(int)LogicEvents.Credits] += OnCredits; 
	}

	protected override void MOnDisable()
	{
		base.MOnDisable();
		M_Event.inputEvents[(int)MInputType.Transport] -= OnTransport;
		M_Event.inputEvents[(int)MInputType.FocusNewObject] -= OnFocusNew;
		M_Event.inputEvents[(int)MInputType.OutOfFocusObject] -= OnOutofFocus;
        M_Event.logicEvents[(int)LogicEvents.Finale] -= OnFinale;
        M_Event.logicEvents[(int)LogicEvents.End] -= OnEnd;
		M_Event.logicEvents[(int)LogicEvents.Credits] -= OnCredits;
	}

	NiceTeleporter t;
	NiceTeleporter focusTeleporter;
	public void OnFocusNew(InputArg arg)
	{
		if (t != null && t != LogicManager.Instance.StayTeleporter)
        { 
			focusTeleporter = t;
			if(transportLine != null)
            {
				Vector3 transportStart = Camera.main.transform.position;
				Vector3 transportToward = focusTeleporter.GetObservePosition();
				float length = (transportStart - transportToward).magnitude;
				transportStart.y = transportToward.y = .25f; 

				transportLine.enabled = true;
				transportLine.SetPosition(0, transportStart);
				transportLine.SetPosition(1, transportToward);
				transportLine.material.SetVector("_Scale", new Vector4(length * 2f, 1f, 1f, 1f));
			}

			if(transportCircle != null)
            {
				Vector3 transportToward = focusTeleporter.GetObservePosition ();
				transportToward.y = .25f;
				transportCircle.transform.position = transportToward;
				transportCircle.gameObject.SetActive(true);
			}
		}
	}

	public void OnOutofFocus(InputArg arg)
	{
		if(focusTeleporter == t)
        {
			if(transportLine != null)
            {
				transportLine.enabled = false;
			}
			if(transportCircle != null)
            {
				transportCircle.gameObject.SetActive (false);
			}
			focusTeleporter = null;
		}
	}

	private Interactable transportToObject;

	public void OnTransport(InputArg arg)
	{
        // Debug.Log("OnTransport");
        if(InputManager.Instance.FocusedObject != null && InputManager.Instance.FocusedObject is NiceTeleporter)
        {
			// do not make a mutiple transport
			if(IsTransporting)
				return;

			transportToObject = InputManager.Instance.FocusedObject;

			// do not transport to myself
			if(transportToObject == LogicManager.Instance.StayTeleporter)
				return;

			if(t == null)
				return;
            // Debug.Log("OnTransport - TransportStart");

            // fire the transport start event
            LogicArg logicArg = new LogicArg(this);
			logicArg.AddMessage(Global.EVENT_LOGIC_TRANSPORTTO_MOBJECT, transportToObject);
			M_Event.FireLogicEvent(LogicEvents.TransportStart, logicArg);

			// set up the animation sequence
			transportSequence = DOTween.Sequence();
			// add the vfx if there is the image effect in the camera
			if (toColorEffect != null && bloomAndFlares != null)
            {
				transportSequence.Append(DOTween.To(() => toColorEffect.rate, (x) => toColorEffect.rate = x, 1f, fadeTime));
				transportSequence.Join(DOTween.To(() => bloomAndFlares.bloomIntensity, (x) => bloomAndFlares.bloomIntensity = x, 8f, fadeTime));
			}

            Vector3 target = t.GetObservePosition();
			MetricManagerScript.instance.AddToMatchList (Time.timeSinceLevelLoad + "; TransportStart to: " + target + "/n");

			transportSequence.Append(LogicManager.Instance.GetPlayerTransform().DOMove(target, transportTime)); // move room to the target 

            Vector3 playerCenter = Vector3.zero; 
            transportSequence.Append(LogicManager.Instance.GetPlayerPersonTransform().DOLocalMove(playerCenter, transportTime / 10.0f)); // move the player to the center of the room, keeping player height the same
            // add the vfx if there is the image effect in the camera
            if (toColorEffect != null && bloomAndFlares != null)
            {
				transportSequence.Append(DOTween.To(() => toColorEffect.rate, (x) => toColorEffect.rate = x, 0f, fadeTime));
				transportSequence.Join(DOTween.To(() => bloomAndFlares.bloomIntensity, (x) => bloomAndFlares.bloomIntensity = x, 0f, fadeTime));
			}

			transportSequence.OnComplete(OnTransportComplete);
		}
	}

	void OnTransportComplete()
	{
        // fire the transport end event
        if(transportToObject != null)
        {
            //Debug.Log("OnTransportComplete - transportToObject is " + transportToObject.name);
			LogicArg arg = new LogicArg(this);
			arg.AddMessage(Global.EVENT_LOGIC_TRANSPORTTO_MOBJECT, transportToObject);
			M_Event.FireLogicEvent(LogicEvents.TransportEnd, arg);
		}
		transportSequence = null;
		transportToObject = null;

        if (finale)
        {
            Debug.Log("Calling iterate to end state from finale in transport.");
            LogicManager.Instance.IterateState();
        }
    }

	public void SetTeleporter(NiceTeleporter teleporter)
	{
		t = teleporter;
	}

    protected override void MUpdate()
    {
        base.MUpdate();

        if (credits)
        {
			float distance = (TrailLeft.GetDistance() + TrailRight.GetDistance()) / 170f;
			Vector3 target = new Vector3(transform.position.x, transform.position.y + distance, transform.position.z);
			transform.position = Vector3.Lerp(transform.position, target, 1f);
        } 
    }

    void OnFinale(LogicArg arg)
    {
        finale = true;
        // SIMULATE TRANSPORT JUST FOR THE FINALE!!

        // set up the animation sequence
        transportSequence = DOTween.Sequence();
        
        // add the vfx if there is the image effect in the camera
        if (toColorEffect != null && bloomAndFlares != null)
        {
            transportSequence.Append(DOTween.To(() => toColorEffect.rate, (x) => toColorEffect.rate = x, 1f, fadeTime));
            transportSequence.Join(DOTween.To(() => bloomAndFlares.bloomIntensity, (x) => bloomAndFlares.bloomIntensity = x, 3f, fadeTime));
        }

        GameObject targetGO = GameObject.Find("endPos");
        Vector3 target = new Vector3(0f, 7.2f, -2.3f);
        if (targetGO != null)
        {
            target = targetGO.transform.position;
        }

        MetricManagerScript.instance.AddToMatchList(Time.timeSinceLevelLoad + "; TransportStart to: " + target + "/n");

        transportSequence.Append(LogicManager.Instance.GetPlayerTransform().DOMove(target, transportTime * 3)); // move room to the target 

        Vector3 playerCenter = new Vector3(0.0f, LogicManager.Instance.GetPlayerPersonTransform().position.y, 0.0f);
        transportSequence.Append(LogicManager.Instance.GetPlayerPersonTransform().DOLocalMove(playerCenter, transportTime)); // move the player to the center of the room, keeping player height the same

        // add the vfx if there is the image effect in the camera
        if (toColorEffect != null && bloomAndFlares != null)
        {
            transportSequence.Append(DOTween.To(() => toColorEffect.rate, (x) => toColorEffect.rate = x, 0f, fadeTime));
            transportSequence.Join(DOTween.To(() => bloomAndFlares.bloomIntensity, (x) => bloomAndFlares.bloomIntensity = x, 0f, fadeTime));
        }

        transportSequence.OnComplete(OnTransportComplete);
    }

    void OnEnd(LogicArg arg)
    {

	}

    void OnCredits(LogicArg arg)
    {
        credits = true;
    }
}
