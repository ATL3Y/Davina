using UnityEngine;
using System.Collections;

public class LogicManager : MBehavior {

	public LogicManager() { s_Instance = this; }
	public static LogicManager Instance { get { return s_Instance; } }
	private static LogicManager s_Instance;

	public bool VREnable = false;

	[SerializeField] GameObject PC;
	[SerializeField] GameObject VR;

	[SerializeField] GameObject Rain;

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

}
