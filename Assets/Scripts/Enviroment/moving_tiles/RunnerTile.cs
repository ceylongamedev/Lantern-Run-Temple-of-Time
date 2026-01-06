using UnityEngine;

public class RunnerTile : MonoBehaviour
{
    public float Length { get; private set; }

    void Awake()
    {
        MeshRenderer mr = GetComponentInChildren<MeshRenderer>();
        Length = mr.bounds.size.z;
    }

    public float EndZ => transform.position.z + Length;
}
