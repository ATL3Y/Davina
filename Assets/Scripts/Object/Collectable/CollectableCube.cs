using UnityEngine;
using System.Collections;
using DG.Tweening;

public class CollectableCube : CollectableObj {


	protected override void MStart ()
	{
		base.MStart ();
	}

	public override bool Select (ClickType clickType)
	{
		base.Select (clickType);
		SelectObjectManager.AttachToCamera (transform, clickType);
		return true;
	}

	public override bool UnSelect ()
	{
		base.UnSelect ();
		SelectObjectManager.AttachToStayPasserBy (transform);
		return true;

	}
}
