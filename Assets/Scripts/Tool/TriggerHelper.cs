using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class TriggerHelper : MBehavior {
	[SerializeField] List<string> senseTags;
	[SerializeField] MTriggerEvent enterEvent;
	[SerializeField] MTriggerEvent stayEvent;
	[SerializeField] MTriggerEvent exitEvent;

	protected override void MAwake ()
	{
		base.MAwake ();
		Collider col = GetComponent<Collider> ();
		if (col != null)
			col.isTrigger = true;
	}

	void OnTriggerEnter( Collider col )
	{
		if ( enterEvent != null && col.gameObject != null && senseTags.Contains( col.gameObject.tag )  )
		{
			enterEvent.Invoke (col);
		}
	}

	void OnTriggerStay( Collider col )
	{
		if ( stayEvent != null && col.gameObject != null && senseTags.Contains( col.gameObject.tag )  )
		{
			stayEvent.Invoke (col);
		}
	}

	void OnTriggerExit( Collider col )
	{
		if ( exitEvent != null && col.gameObject != null && senseTags.Contains( col.gameObject.tag )  )
		{
			exitEvent.Invoke (col);
		}
	}
}


[System.Serializable]
public class MTriggerEvent : UnityEvent<Collider>{}