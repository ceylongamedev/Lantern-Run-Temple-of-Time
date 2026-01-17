using UnityEngine;

public class PointLightController : MonoBehaviour
{
    private Light pointLight;

    [Header("Intensity Settings")]
    public float maxIntensity = 5f;       // Maximum allowed intensity
    public float minIntensity = 0f;       // Minimum allowed intensity
    public float transitionDuration = 1f; // How long the fade should take
    public float decayRate = 0.5f;        // Intensity decrease per second

    private float targetIntensity;        // Desired intensity after change
    private float transitionStartIntensity;
    private float transitionTime;
    private bool isTransitioning;

    void Start()
    {
        pointLight = GetComponent<Light>();
        if (pointLight == null || pointLight.type != LightType.Point)
        {
            Debug.LogError("Attach this script to a Point Light object!");
            enabled = false;
            return;
        }

        targetIntensity = pointLight.intensity;
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.I))
        {
            IncreaseIntensity(3f);
        }else if (Input.GetKeyDown(KeyCode.U))
        {
            DecreaseIntensity(1f);
        }

        if (isTransitioning)
        {
            transitionTime += Time.deltaTime;
            float t = transitionTime / transitionDuration;
            pointLight.intensity = Mathf.Lerp(transitionStartIntensity, targetIntensity, t);

            if (t >= 1f)
            {
                isTransitioning = false;
            }
        }

        if (!isTransitioning && pointLight.intensity > minIntensity)
        {
            pointLight.intensity -= decayRate * Time.deltaTime;
            pointLight.intensity = Mathf.Max(pointLight.intensity, minIntensity);
        }
    }

    public void IncreaseIntensity(float amount)
    {
        transitionStartIntensity = pointLight.intensity;
        targetIntensity = Mathf.Min(pointLight.intensity + amount, maxIntensity);
        transitionTime = 0f;
        isTransitioning = true;
    }

    public void DecreaseIntensity(float amount)
    {
        transitionStartIntensity = pointLight.intensity;
        targetIntensity = Mathf.Max(pointLight.intensity - amount, minIntensity);
        transitionTime = 0f;
        isTransitioning = true;
    }
}
