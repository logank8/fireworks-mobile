using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firework : MonoBehaviour
{
    public ParticleSystem ps;

    // Start is called before the first frame update
    void Start()
    {
        ps = gameObject.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ps.particleCount == 0) {
            Destroy(gameObject);
        }
    }

    public void SetColor(Color color) {
        // Only need to set color over lifetime module in particle system 

        ParticleSystem particles = GetComponent<ParticleSystem>();
        
        // 4 alpha keys
        // 
        // 2 color keys
        Gradient gradient = new Gradient();

        gradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(color, 0.0f),
                new GradientColorKey(Color.Lerp(color, Color.white, 0.9f), 0.45f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1, 0.0f),
                new GradientAlphaKey(1, 0.45f),
                new GradientAlphaKey(0.85f, 0.92f),
                new GradientAlphaKey(0, 1)
            }
        );

        var colorOverLifetime = particles.colorOverLifetime;

        colorOverLifetime.color =  new ParticleSystem.MinMaxGradient(gradient);
    }
}
