using UnityEngine;

public class CoinRotation : MonoBehaviour
{
    public float rotationSpeed = 50f;
    private bool isStarted = false;

    private void OnEnable()
    {
        startRotating();
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

}
