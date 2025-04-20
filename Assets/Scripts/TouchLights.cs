using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchLights : MonoBehaviour
{
    public bool dying;

    public Color color;

    ParticleSystem ps;

    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (dying) {
            if (ps.particleCount == 0) {
                Destroy(gameObject);
            }
            return;
        }
    }

    public void RespondToPressure(float pressureRatio) {
        if (!Input.touchPressureSupported) {
            return;
        }
        if (pressureRatio > 0.5f) {
            ps.Emit(15);
        }
    }

    public void DestroyObject() {
        // stop emission of particle system - wait until the particles are all gone then destroy self
        dying = true;

        var em = ps.emission;
        em.enabled = false;
    }

    public void SetColor(Color newColor) {
        Debug.Log("Setting new color");
        Debug.Log(newColor);
        color = newColor;

        // Setting ColorOverLifetimeModule:
        // Start with alpha and RGB at base (1, color given)
        // 40% move color to blended with white
        // 50% move alpha to 220
        // Set end alpha value to 0

        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(newColor, 0.0f),
                new GradientColorKey(Color.Lerp(newColor, Color.white, 0.5f), 0.4f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1.0f, 0.0f),
                new GradientAlphaKey(0.86f, 0.5f),
                new GradientAlphaKey(0.0f, 1.0f)
            }
        );

        ParticleSystem particles = gameObject.GetComponent<ParticleSystem>();

        var colorOverLifetime = particles.colorOverLifetime;
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);
    }
}
