using UnityEngine;
using System.Collections;

public class CameraAttachPoint : MBehavior {

	public static CameraAttachPoint activePoint;

    protected override void MOnEnable()
    {
        base.MOnEnable();
        M_Event.logicEvents[(int)LogicEvents.Tutorial] += OnTutorial;
        M_Event.logicEvents[(int)LogicEvents.Characters] += OnCharacters;
        M_Event.logicEvents[(int)LogicEvents.End] += OnEnd;
    }

    protected override void MOnDisable()
    {
        base.MOnDisable();
        M_Event.logicEvents[(int)LogicEvents.Tutorial] -= OnTutorial;
        M_Event.logicEvents[(int)LogicEvents.Characters] -= OnCharacters;
        M_Event.logicEvents[(int)LogicEvents.End] -= OnEnd;
    }

    void OnTutorial(LogicArg arg)
    {
        if (activePoint != this)
        {
            activePoint = this;
            Debug.Log("Running camera attach pt fyi");

            M_Event.FireLogicEvent(LogicEvents.CameraAttachPointChange, new LogicArg(this));
        }
    }
    void OnCharacters(LogicArg arg)
    {
        if (activePoint != this)
        {
            activePoint = this;
            Debug.Log("Running camera attach pt fyi");

            M_Event.FireLogicEvent(LogicEvents.CameraAttachPointChange, new LogicArg(this));
        }
    }
    void OnEnd(LogicArg arg)
    {
        if (activePoint != this)
        {
            activePoint = this;
            Debug.Log("Running camera attach pt fyi");

            M_Event.FireLogicEvent(LogicEvents.CameraAttachPointChange, new LogicArg(this));
        }
    }
}
