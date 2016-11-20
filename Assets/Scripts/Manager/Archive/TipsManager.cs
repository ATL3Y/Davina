using UnityEngine;
using System.Collections;

/// <summary>
/// Not useful
/// </summary>
public class TipsManager : MBehavior {

	public TipsManager() { s_Instance = this; }
	public static TipsManager Instance { get { return s_Instance; } }
	private static TipsManager s_Instance;

	[SerializeField] LineRenderer lineTips;
	[SerializeField] bool disableLineOnPC = true;

	protected override void MAwake ()
	{
		base.MAwake ();
		InitMainRayLineTIps ();
	}


	protected override void MUpdate ()
	{
		base.MUpdate ();
		UpdateMainRayLineTips ();
	}


	void InitMainRayLineTIps()
	{
		if (lineTips == null)
			lineTips = GetComponentInChildren<LineRenderer> ();
		if (disableLineOnPC & !LogicManager.Instance.VREnable)
			lineTips.enabled = false;

	}

	void UpdateMainRayLineTips()
	{
		if (lineTips != null ) {
			/// for VR, this only takes the left controller
			Vector3 startPosition = InputManager.Instance.GetCenterRayCast ()[0].origin;
			if ((startPosition - Camera.main.transform.position).magnitude < 0.1f) {
				// the ray start from the camera
				startPosition += Vector3.down ;
			}
			lineTips.SetPosition (0, startPosition);
			lineTips.SetPosition (1, startPosition + InputManager.Instance.GetCenterRayCast ()[0].direction * 0.1f);

		}
	}
}
