using UnityEngine;
using System.Collections;

public class LogicManager : MBehavior {

	public LogicManager() { s_Instance = this; }
	public static LogicManager Instance { get { return s_Instance; } }
	private static LogicManager s_Instance;

	[SerializeField] bool VREnable = false;

	protected override void MAwake ()
	{
		base.MAwake ();

		if (VREnable) {
			gameObject.AddComponent<ViveInputManager> ();
		} else {
			gameObject.AddComponent<PCInputManager> ();
		}

		if (GameObject.Find ("PC"))
			GameObject.Find ("PC").SetActive (!VREnable);
		if (GameObject.Find ("VR"))
			GameObject.Find ("PC").SetActive (VREnable);

	}

}
