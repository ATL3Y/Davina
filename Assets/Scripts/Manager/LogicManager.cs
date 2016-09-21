using UnityEngine;
using System.Collections;

public class LogicManager : MBehavior {

	public LogicManager() { s_Instance = this; }
	public static LogicManager Instance { get { return s_Instance; } }
	private static LogicManager s_Instance;

	[SerializeField] bool VREnable = false;

	[SerializeField] GameObject PC;
	[SerializeField] GameObject VR;

	protected override void MAwake ()
	{
		base.MAwake ();

		if (VREnable) {
			gameObject.AddComponent<ViveInputManager> ();
		} else {
			gameObject.AddComponent<PCInputManager> ();
		}

		if (PC != null)
			PC.SetActive (!VREnable);
		if (VR != null)
			VR.SetActive (VREnable);

	}

}
