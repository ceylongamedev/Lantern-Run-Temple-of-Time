using UnityEngine;

public class CoinRotation : MonoBehaviour
{
    public float rotationSpeed = 50f;
    private bool isStarted = false;

    [SerializeField]  Material[] _EEM;
    [SerializeField] private GameObject _coin;
    private MeshRenderer _meshRenderer;
    private void OnEnable()
    {
        startRotating();
    }

    private void Start()
    {
        _meshRenderer = _coin.GetComponent<MeshRenderer>();
        float value = Random.value;
        if (value < 0.01f)
            EEMaterial();
    }

    void Update()
    {
       // if (!isStarted) return;

        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }

    public void startRotating()
    {
        isStarted = true;
    }

    void EEMaterial()
    {
       _meshRenderer.material = _EEM[Random.Range(0, _EEM.Length)];
    }

}
