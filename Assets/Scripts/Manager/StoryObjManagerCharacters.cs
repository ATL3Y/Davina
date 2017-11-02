using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StoryObjManagerCharacters : MBehavior
{
	[SerializeField] List<GameObject> storyObjA;
	[SerializeField] List<GameObject> storyObjB;
	[SerializeField] List<GameObject> storyObjC;

    [SerializeField] GameObject transport;

    private List<GameObject> currentStory = new List<GameObject>();

    private int count = -1;

    protected override void MOnEnable()
    {
		base.MOnEnable ();
        M_Event.logicEvents [(int)LogicEvents.Tutorial] += OnTutorial;
        M_Event.logicEvents [(int)LogicEvents.EnterStory] += OnEnterStory;
		M_Event.logicEvents [(int)LogicEvents.ExitStory] += OnExitStory;
		M_Event.logicEvents [(int)LogicEvents.Characters] += OnCharacters;
    }

	protected override void MOnDisable()
    {
		base.MOnDisable ();
        M_Event.logicEvents [(int)LogicEvents.Tutorial] -= OnTutorial;
        M_Event.logicEvents [(int)LogicEvents.EnterStory] -= OnEnterStory;
		M_Event.logicEvents [(int)LogicEvents.ExitStory] -= OnExitStory;
		M_Event.logicEvents [(int)LogicEvents.Characters] -= OnCharacters;
    }

	void OnEnterStory(LogicArg arg)
	{
        M_Event.FireLogicEvent(LogicEvents.ExitStory, new LogicArg(this));

        count++;
		if (GetStory() == null)
        {
            if (gameObject.scene.buildIndex == 1) // tutorial scene 
            {
                if(Score.Instance.GetScore() == 0) // they tried both sides
                {
                    // print("firing switch scene to characters");
                    // LogicManager.Instance.IterateState();
                    timer = 3f;
                    runTimer = true;
                    return;
                }
                else // try the other side until the score is 0.
                {
                    GenerateInteraction();
                }
            }
            else if (gameObject.scene.buildIndex == 2) // characters scene
            {
                // M_Event.FireLogicEvent(LogicEvents.Finale, new LogicArg(this)); // START THE FINALE!
                timer = 3f;
                runTimer = true;
                return;
            }   
		}

        //currentStory.Clear();
        for (int i = 0; i < currentStory.Count; i++) // instead of clearing?
        {
            for(int j=0; j < currentStory[i].transform.childCount; j++)
            {
                currentStory[i].transform.GetChild(j).gameObject.SetActive(false);
            }
            currentStory[i].SetActive(false);
        }

        currentStory = GetStory();
		for (int i = 0; i < currentStory.Count; i++)
        {
			currentStory[i].SetActive(true);
		}
	}

    float timer = 0f;
    bool runTimer = false;
    protected override void MUpdate()
    {
        base.MUpdate();

        if(runTimer)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                runTimer = false;
                if (gameObject.scene.buildIndex == 1) // tutorial scene 
                {
                    LogicManager.Instance.IterateState();
                }
                else if (gameObject.scene.buildIndex == 2) // characters scene
                {
                    M_Event.FireLogicEvent(LogicEvents.Finale, new LogicArg(this));
                }
            }
        }
    }

    void GenerateInteraction()
    {
        if (Score.Instance.GetScore() >= 1) // If score is positive, require a negative to get 0
        {
            Score.Instance.ForceSetScore(1);
            count = 1; // Try the last story again
            
            for (int i = 0; i < storyObjB.Count; i++) // Sort of make a new story
            {
                storyObjB[i].SetActive(false);
            }
             
            SpawnCollectibleHoleSet spawn = GetComponent<SpawnCollectibleHoleSet>();
            if (spawn != null)
            {
                HoleContainer holeCont = storyObjB[0].GetComponent<HoleContainer>(); // Set the color
                if (holeCont != null)
                {
                    holeCont.color = spawn.Color();
                }

                CollectableContainer collCont = storyObjB[1].GetComponent<CollectableContainer>(); // Set the audio
                if (collCont != null)
                {
                    collCont.storySoundL = spawn.storySoundR;
                    collCont.storySoundR = spawn.storySoundL;
                }
            } 
        }
        else if (Score.Instance.GetScore() <= -1) // If score is negative, require a positive to get 0
        {
            Score.Instance.ForceSetScore(-1);
            count = 1; // Try the last story again
            
            for (int i = 0; i < storyObjB.Count; i++) // Sort of make a new story
            {
                storyObjB[i].SetActive(false);
            }

            SpawnCollectibleHoleSet spawn = GetComponent<SpawnCollectibleHoleSet>();
            if (spawn != null)
            {
                HoleContainer holeCont = storyObjB[0].GetComponent<HoleContainer>(); // Set the color
                if (holeCont != null)
                {
                    holeCont.color = spawn.Color();
                }

                CollectableContainer collCont = storyObjB[1].GetComponent<CollectableContainer>(); // Set the audio
                if (collCont != null)
                {
                    collCont.storySoundL = spawn.storySoundL;
                    collCont.storySoundR = spawn.storySoundR;
                }
            }
        }
    }

	//exit last story before entering new one 
	void OnExitStory(LogicArg arg)
	{
		for (int i=currentStory.Count-1; i>=0; i--)
        {
			NiceCollectable niceCollectable = currentStory[i].GetComponent<NiceCollectable>();
			if (niceCollectable != null && niceCollectable != (NiceCollectable)arg.sender)
            {
				Debug.Log("in OnExitStory deactivating");
				currentStory[i].SetActive (false);
			}
		}
	}

	//returns the next batch of story obj
	List<GameObject> GetStory()
	{
		switch(count)
        {
		    case 0:
			    if (storyObjA.Count > 0) {
				    return storyObjA;
			    } else {
				    return null;
			    }
		    case 1:
			    if (storyObjB.Count > 0) {
				    return storyObjB;
			    } else {
				    return null;
			    }
            case 2:
                if (storyObjC.Count > 0) {
                    return storyObjC;
                } else {
                    return null;
                }
            case 3:
                return null;
            default:
                return null;
		}
	}

    void OnTutorial(LogicArg arg)
    {
        Init();
        M_Event.FireLogicEvent(LogicEvents.EnterStory, new LogicArg(this));
    }

	void OnCharacters(LogicArg arg)
    {
        Init();
		// M_Event.FireLogicEvent(LogicEvents.EnterStory, new LogicArg(this));
	}

    void Init()
    {
        // set stories to false and level objects to true
        for (int i = 0; i < storyObjA.Count; i++)
        {
            storyObjA[i].SetActive(false);
        }
        for (int i = 0; i < storyObjB.Count; i++)
        {
            storyObjB[i].SetActive(false);
        }
        for (int i = 0; i < storyObjC.Count; i++)
        {
            storyObjC[i].SetActive(false);
        }
    }
}
