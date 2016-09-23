using UnityEngine;
using System.Collections;

public class LogicManager : MBehavior {

	public LogicManager() { s_Instance = this; }
	public static LogicManager Instance { get { return s_Instance; } }
	private static LogicManager s_Instance;

	public bool VREnable = false;

	[SerializeField] GameObject PC;
	[SerializeField] GameObject VR;
	[SerializeField] Transform PCHand;
	[SerializeField] Transform VRHand;

	[SerializeField] GameObject Rain;

	private PasserBy m_stayPasserBy;
	public PasserBy StayPasserBy
	{
		get { return m_stayPasserBy; }
	}

	protected override void MAwake ()
	{
		base.MAwake ();

		if (VREnable) {
			gameObject.AddComponent<ViveInputManager> ();
		} else {
			gameObject.AddComponent<PCInputManager> ();
		}

		if (PC != null) {
			PC.SetActive (!VREnable);
		}
		if (VR != null) {
			VR.SetActive (VREnable);
		}

		Rain.transform.SetParent ( VREnable ? VR.transform : PC.transform);
		Rain.transform.localPosition = Vector3.up * 5f;

		DontDestroyOnLoad (gameObject);

	}

	public Transform GetPlayerTransform()
	{
		return VREnable ? VR.transform : PC.transform;
	}

	public Transform GetHandTransform()
	{
		return VREnable ? VRHand : PCHand;
	}


	protected override void MOnEnable ()
	{
		base.MOnEnable ();
		M_Event.logicEvents [(int)LogicEvents.TransportEnd] += OnTransportToNewObject;
	}

	protected override void MOnDisable ()
	{
		base.MOnDisable ();
		M_Event.logicEvents [(int)LogicEvents.TransportEnd] -= OnTransportToNewObject;
	}

	void OnTransportToNewObject( LogicArg arg )
	{
		var obj = arg.GetMessage (Global.EVENT_LOGIC_TRANSPORTTO_MOBJECT);
		if (obj is PasserBy) {
			m_stayPasserBy = (PasserBy)obj;
		}
	}



}
