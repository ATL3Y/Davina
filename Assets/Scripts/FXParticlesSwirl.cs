using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXParticlesSwirl : MonoBehaviour
{
    public NiceHole Hole { get; set; }
    [SerializeField] ParticleSystem particleSystem;
    private ParticleSystem.Particle[] particles = new ParticleSystem.Particle[1000];
    private Color color;
    private NiceCollectable collectable;
    [SerializeField] bool gray;

    // Use this for initialization
    void OnEnable()
    {
        particleSystem.SetParticles(particles, 1000);
        particleSystem.Play();

        M_Event.logicEvents[(int)LogicEvents.ExitStory] += OnExitStory;
        M_Event.logicEvents[(int)LogicEvents.Finale] += OnFinale;
    }

    void OnDisable()
    {
        M_Event.logicEvents[(int)LogicEvents.ExitStory] -= OnExitStory;
        M_Event.logicEvents[(int)LogicEvents.Finale] -= OnFinale;
    }

    void OnExitStory(LogicArg arg)
    {
        // Turn off particles 
        particleSystem.Stop();
    }

    void OnFinale(LogicArg arg)
    {
        particleSystem.Stop();
        gameObject.SetActive(false);
    }

    private void Start()
    {
        HoleContainer holeContainer = Hole.transform.parent.GetComponent<HoleContainer>();
        if (holeContainer != null)
        {
            color = holeContainer.color;
            particleSystem.startColor = color;
        }
        else
        {
            Debug.Log("color is null");
        }

        collectable = transform.parent.parent.GetComponent<NiceCollectable>();

        if (gray && collectable != null)
        {
            particleSystem.startColor = collectable.LightSideOut ? Color.white : Color.black;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(collectable == null)
            Debug.Log("collectable is null");

        if (gray)
            color = collectable.LightSideOut ? Color.white : Color.black;
        
        int particleLength = particleSystem.GetParticles(particles);

        for (int i = 0; i < particleLength; i++)
        {
            Vector3 direction = Vector3.Normalize(Hole.transform.position - particles[i].position);
            Vector3 targetPos = particles[i].position + direction;
            particles[i].position = Vector3.Lerp(particles[i].position, targetPos, .008f);
            particles[i].startColor = color;
        }
        
        particleSystem.SetParticles(particles, particleLength);
    }
}
