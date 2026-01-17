using UnityEngine;

public class RunnerTile : MonoBehaviour
{
    public float Length { get; private set; }

    void Awake()
    {
        MeshRenderer mr = GetComponentInChildren<MeshRenderer>();
        BoxCollider collider = GetComponent<BoxCollider>();
        Debug.Log("Length from collider : "+collider.bounds.size.z);
        Length = collider.bounds.size.z;
        Debug.Log(Length);
    }

    public float EndZ => transform.position.z + Length;
}
