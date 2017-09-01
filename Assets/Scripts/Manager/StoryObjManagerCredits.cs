using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StoryObjManagerCredits : MBehavior
{
	[SerializeField] List<GameObject> levelSpecificObjects;

    protected override void MAwake()
	{
		base.MAwake();

        if (levelSpecificObjects.Count > 0) 
        {
            levelSpecificObjects[0].SetActive(false);
            levelSpecificObjects[1].SetActive(true);
        }
	}

	protected override void MOnEnable()
    {
		base.MOnEnable();
		M_Event.logicEvents [(int)LogicEvents.Credits] += OnCredits;
	}

	protected override void MOnDisable()
    {
		base.MOnDisable ();
		M_Event.logicEvents [(int)LogicEvents.Credits] -= OnCredits;
	}

	void OnCredits(LogicArg arg)
    {
        levelSpecificObjects[0].SetActive(true);
        levelSpecificObjects[1].SetActive(false);
    }

    protected override void MUpdate()
    {
        base.MUpdate();
    }
}
