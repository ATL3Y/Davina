using UnityEngine;
using System.Collections;
using DG.Tweening;

public class CollectableCube : CollectableObj {


	protected override void MStart ()
	{
		base.MStart ();
	}

	public override void Select ()
	{
		base.Select ();
		SelectObjectManager.AttachToCamera (transform);
	}

	public override void UnSelect ()
	{
		base.UnSelect ();
		SelectObjectManager.AttachToStayPasserBy (transform);

	}
}
