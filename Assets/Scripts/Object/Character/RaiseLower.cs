using UnityEngine;
using System.Collections;
using DG.Tweening;

public class RaiseLower : MonoBehaviour
{
	float t = 0f;
	private bool finale = false;
    [SerializeField] Transform finaleTransform;

	// called when object enabled 
	void OnEnable()
	{
		M_Event.logicEvents[(int)LogicEvents.RaiseFallingCharacter] += OnRaise;
		M_Event.logicEvents[(int)LogicEvents.LowerFallingCharacter] += OnLower;
        M_Event.logicEvents[(int)LogicEvents.Finale] += OnFinale;
    }

	void OnDisable()
	{
		M_Event.logicEvents[(int)LogicEvents.RaiseFallingCharacter] -= OnRaise;
		M_Event.logicEvents[(int)LogicEvents.LowerFallingCharacter] -= OnLower;
        M_Event.logicEvents[(int)LogicEvents.Finale] -= OnFinale;
    }

	void OnRaise(LogicArg arg)
	{
		transform.DOLocalMove(transform.position + new Vector3(0f, .2f, 0f), 2f).SetEase(Ease.InOutSine);
	}

	void OnLower(LogicArg arg)
	{
		transform.DOLocalMove(transform.position + new Vector3(0f, -.2f, 0f), 2f).SetEase(Ease.InOutSine);
	}

    protected void Update()
    {
        if(gameObject.scene.buildIndex == 1) // don't run this shit for the tutorial 
            return;

		if (GetComponent<MCharacter>().IsInInnerWorld)
        {
			t = 0f;
		}

        
		if(finale && !GetComponent<MCharacter>().IsInInnerWorld)
        {
            //Quaternion turn = Quaternion.AngleAxis(10f, Vector3.forward) * transform.localRotation;
            //Vector3 toPlayer = LogicManager.Instance.GetPlayerHeadTransform().position - transform.position;
            //Quaternion look = Quaternion.LookRotation(toPlayer) * transform.localRotation;
            //transform.localRotation = Quaternion.Slerp(transform.localRotation, look, Time.deltaTime * 1.5f);

            Vector3 target = new Vector3(transform.position.x, LogicManager.Instance.GetPlayerHeadTransform().position.y, transform.position.z);
			if(t < 1f)
            {
				t += Time.deltaTime / 500f;
			}
			transform.position = Vector3.Lerp(transform.position, target, t);
        }
    }

    void OnFinale(LogicArg arg)
    {
        //Vector3 rot = transform.rotation.ToEulerAngles();
        //rot = new Vector3(rot.x, rot.y + 180.0f, rot.z);
        //transform.DOLocalRotate(rot, 1f);
        finale = true;
        Vector3 finaleRot = new Vector3(0f, 180f, 0f); // -76.6f, -112.6f, -288.0f);
        transform.DOLocalRotate(finaleRot, 2f).SetEase(Ease.InOutSine);
    }
}
