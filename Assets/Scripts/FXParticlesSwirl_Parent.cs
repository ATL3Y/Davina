using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FXParticlesSwirl_Parent: MonoBehaviour
{
    public NiceHole Hole { get; set; }
    private bool finished = false;

    [SerializeField] Color c1;
    [SerializeField] Color c2;
    public int lengthOfLineRenderer = 2;

    void OnEnable()
    {

    }

	// Update is called once per frame
	void FixedUpdate ()
    {

        /*
        if (finished)
            return;

        if(destination.Finished && !finished)
        {
            finished = true;
            Stop();
        }
        */

        Vector3 direction = Hole.transform.position - transform.position;
        Quaternion temp = Quaternion.AngleAxis(Time.timeSinceLevelLoad * 180.0f, direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, temp, Time.deltaTime * 20.0f);
    }
    /*
    private IEnumerator Stop()
    {
        for (int i = 0; i < particleSystems.Count; i++)
        {
            ParticleSystem.EmissionModule emission = particleSystems[i].emission;
            emission.enabled = false;
        }

        yield return new WaitForSeconds(10.0f);

        Destroy(gameObject);
    }

    void InitLine()
    {
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
        lineRenderer.widthMultiplier = 0.01f;
        lineRenderer.positionCount = lengthOfLineRenderer;

        // A simple 2 color gradient with a fixed alpha of 1.0f.
        float alpha = 1.0f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(c1, 0.0f), new GradientColorKey(c2, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
            );
        lineRenderer.colorGradient = gradient;
    }

    void UpdateLine(Vector3 startPos, Vector3 dir)
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        var t = Time.time;
        for (int i = 0; i < lengthOfLineRenderer; i++)
        {
            Vector3 pos = startPos + dir * i;
            lineRenderer.SetPosition(i, pos);
        }
    }
    */
}
