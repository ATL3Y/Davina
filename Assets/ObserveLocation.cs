using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObserveLocation : MBehavior
{

    protected override void MOnEnable()
    {
        base.MOnEnable();
        M_Event.inputEvents[(int)MInputType.Transport] += OnTransport;
    }

    protected override void MOnDisable()
    {
        base.MOnDisable();
        M_Event.inputEvents[(int)MInputType.Transport] -= OnTransport;
    }

    void OnTransport (InputArg arg)
    {
        transform.position = new Vector3(transform.position.x, .246f, transform.position.z);
        Debug.Log("fixing ground heigh for scene" + gameObject.scene.buildIndex);
    }
}
