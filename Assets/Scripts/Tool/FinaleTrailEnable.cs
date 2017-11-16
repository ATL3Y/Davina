using UnityEngine;
using System.Collections;

public class FinaleTrailEnable : MBehavior
{
	private Vector3 lastPos = Vector3.zero;
	private float distance = 0f;

	protected override void MAwake()
	{
		base.MAwake();
		lastPos = transform.position + transform.forward;
	}

	protected override void MOnEnable()
    {
		base.MOnEnable();
        M_Event.logicEvents[(int)LogicEvents.Finale] += OnFinale;
        M_Event.logicEvents[(int)LogicEvents.Credits] += OnCredits;
	}

	protected override void MOnDisable()
    {
		base.MOnDisable();
        M_Event.logicEvents[(int)LogicEvents.Finale] -= OnFinale;
        M_Event.logicEvents[(int)LogicEvents.Credits] -= OnCredits;
	}

	protected override void MUpdate()
    {
		Vector3 currentPos = transform.position + transform.forward;
		distance = Vector3.Distance (lastPos, currentPos);
		lastPos = currentPos;
	}

	public float GetDistance()
    {
		return distance;
	}

    void OnFinale(LogicArg arg)
    {
        GetComponent<TrailRenderer>().enabled = true;
    }

    void OnCredits(LogicArg arg)
    {
		GetComponent<TrailRenderer> ().enabled = false;
	}
}
