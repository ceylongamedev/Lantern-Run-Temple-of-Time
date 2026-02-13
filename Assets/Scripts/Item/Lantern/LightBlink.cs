using UnityEngine;

public class LightBlink : MonoBehaviour
{
    [SerializeField]
    private float _maxValue, _minValue, _time;
    private Light _light;
    void Start()
    {
        _light = GetComponent<Light>();
    }

    
    void Update()
    {
        float t = Mathf.Sin(Time.deltaTime * _time) / 2f;
        _light.intensity = Mathf.Lerp(_minValue, _maxValue, t);
    }
}
