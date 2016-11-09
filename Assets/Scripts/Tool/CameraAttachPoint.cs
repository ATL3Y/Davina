using UnityEngine;
using System.Collections;

public class CameraAttachPoint : MBehavior {

	public static CameraAttachPoint activePoint;

	protected override void MStart ()
	{
		base.MStart ();

		if (activePoint != this) {
			activePoint = this;
			M_Event.FireLogicEvent (LogicEvents.CameraAttachPointChange, new LogicArg (this));
		}
	}
}
