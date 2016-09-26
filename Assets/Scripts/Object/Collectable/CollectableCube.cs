using UnityEngine;
using System.Collections;
using DG.Tweening;

public class CollectableCube : CollectableObj {


	protected override void MStart ()
	{
		base.MStart ();
	}

	public override bool Select ()
	{
		base.Select ();
		SelectObjectManager.AttachToCamera (transform);
		return true;
	}

	public override bool UnSelect ()
	{
		base.UnSelect ();
		SelectObjectManager.AttachToStayPasserBy (transform);
		return true;

	}
}
