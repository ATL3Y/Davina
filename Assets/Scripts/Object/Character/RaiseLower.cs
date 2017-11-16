using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.SceneManagement;

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
        // Make sure we don't get out of range in tutorial
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            return;
        }

        transform.DOLocalMove(transform.position + new Vector3(0f, .13f, 0f), 2f).SetEase(Ease.InOutSine);
	}

	void OnLower(LogicArg arg)
	{
        // Make sure we don't get out of range in tutorial
        if(SceneManager.GetActiveScene().buildIndex == 1)
        {
            return;
        }

        transform.DOLocalMove(transform.position + new Vector3(0f, -.13f, 0f), 2f).SetEase(Ease.InOutSine);
	}

    protected void Update()
    {
        if(gameObject.scene.buildIndex == 1) // don't run this shit for the tutorial 
            return;

		if (GetComponent<MCharacter>().IsInInnerWorld)
        {
			t = 0f;
		}

		if( finale && !GetComponent<MCharacter>().IsInInnerWorld )
        {
            Vector3 target = new Vector3(transform.position.x, LogicManager.Instance.GetPlayerHeadTransform().position.y, transform.position.z);
			if(t < 1f)
            {
				t += Time.deltaTime / 20f; // lag so Davina doesn't jump up at first
			}
			transform.position = Vector3.Lerp(transform.position, target, t);
        }
    }

    void OnFinale(LogicArg arg)
    {
        finale = true;
    }
}
