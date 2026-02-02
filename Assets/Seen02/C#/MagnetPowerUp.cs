using UnityEngine;

public class MagnetPowerUp : MonoBehaviour
{
    [Header("Magnet Settings")]
    [SerializeField] private float _magnetRadius = 5f;
    [SerializeField] private float _magnetSpeed = 10f;
    [SerializeField] private LayerMask _coinLayer;

    public bool magnetActive = false;

    [Header("Renderer")]
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private Color magnetColer = Color.red;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            magnetActive = !magnetActive;
    
        }
        if (magnetActive)
        {
            targetRenderer.material.SetFloat("_ON", 1f);
            targetRenderer.material.SetColor("BottomColor", magnetColer);
        }
        else if (!magnetActive)
        {
            targetRenderer.material.SetFloat("_ON", 0f);
        }

        if (!magnetActive) return;

        Collider[] coins = Physics.OverlapSphere(transform.position, _magnetRadius, _coinLayer);

        foreach (Collider coin in coins)
        {
            if (coin == null) continue;
            Vector3 direction = (transform.position - coin.transform.position).normalized;
            coin.transform.position += direction * _magnetSpeed * Time.deltaTime;
        }
            
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _magnetRadius);
    }
}//Class
