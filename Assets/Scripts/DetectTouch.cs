using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectTouch : MonoBehaviour
{

    public GameObject lightPrefab;

    public Dictionary<int, GameObject> activeLights;

    public Dictionary<int, float> touchSpeeds;

    public Color[] colorPicks;

    public float speedThreshold;

    public Color defaultColor;

    public List<Color> activeColors;

    public GameObject firework;

    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        activeLights = new Dictionary<int, GameObject>();
        touchSpeeds = new Dictionary<int, float>();
        activeColors = new List<Color>();
    }

    // Update is called once per frame
    void Update()
    {

        for (int i = 0; i < Input.touchCount; ++i) {
            Touch currentTouch = Input.GetTouch(i);

            // Use fingerId to distinguish touches and track across frames

            if (currentTouch.phase == TouchPhase.Began) {                
                MakeNewLight(currentTouch);
            } else if (currentTouch.phase == TouchPhase.Ended) {
                GameObject currentLight = activeLights[currentTouch.fingerId];
                TouchLights currentLightTouch = currentLight.GetComponent<TouchLights>(); 

                // if prev touch speed above speed threshold then instantiate firework
                if (touchSpeeds[currentTouch.fingerId] > speedThreshold) {
                    GameObject fireworkObj = Instantiate(firework, cam.ScreenToWorldPoint(new Vector3(currentTouch.position.x, currentTouch.position.y, cam.WorldToScreenPoint(currentLight.transform.position).z)), transform.rotation);
                    Firework fireworkComp = fireworkObj.GetComponent<Firework>();
                    fireworkComp.SetColor(currentLightTouch.color);
                }

                activeColors.Remove(currentLightTouch.color);

                currentLightTouch.DestroyObject();

                activeLights.Remove(currentTouch.fingerId);
            } else if (currentTouch.phase == TouchPhase.Moved || currentTouch.phase == TouchPhase.Stationary) {
                GameObject light = activeLights[currentTouch.fingerId];

                TouchLights currentLightTouch = light.GetComponent<TouchLights>();

                currentLightTouch.RespondToPressure(currentTouch.pressure / currentTouch.maximumPossiblePressure);
                
                light.transform.position = cam.ScreenToWorldPoint(new Vector3(currentTouch.position.x, currentTouch.position.y, cam.WorldToScreenPoint(light.transform.position).z));

                // measure touch speed and record (lifted touch will always be 0)
                float touchSpeed = currentTouch.deltaPosition.magnitude / currentTouch.deltaTime;
                Debug.Log(touchSpeed);
                if (currentTouch.deltaPosition.magnitude > 10) {
                    touchSpeeds[currentTouch.fingerId] = touchSpeed;
                }
            }
        }
    }

    void MakeNewLight(Touch currentTouch) {

        // Choose random color from colorPicks - if all colors are taken then go to default

        int randIdx = Random.Range(0, colorPicks.Length-1);

        Color newColor = colorPicks[randIdx];

        int tries = 0;

        while (activeColors.Contains(newColor)) {
            tries++;

            if (tries == colorPicks.Length) {
                newColor = defaultColor;
                break;
            }

            if (randIdx == colorPicks.Length - 1) {
                randIdx = 0;
            } else {
                randIdx += 1;
            }

            newColor = colorPicks[randIdx];
        }

        activeColors.Add(newColor);

        // Add variance in depth
        float depth = 100;
        depth += Random.Range(-70, 10);

        GameObject newLight = Instantiate(lightPrefab, cam.ScreenToWorldPoint(new Vector3(currentTouch.position.x, currentTouch.position.y, depth)), transform.rotation);

        activeLights[currentTouch.fingerId] = newLight;

        TouchLights newLightSystem = newLight.GetComponent<TouchLights>();
        newLightSystem.SetColor(newColor);
    }
}
