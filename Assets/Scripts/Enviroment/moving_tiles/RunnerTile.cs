using UnityEngine;

[SerializeField]
public class RunnerTile : MonoBehaviour
{
    public float Length;// { get; private set; }

    void Awake()
    {
        MeshRenderer mr = GetComponentInChildren<MeshRenderer>();
        //BoxCollider collider = GetComponent<BoxCollider>();
        //Length = collider.bounds.size.z;
        Debug.Log(Length);
    }

    public float EndZ => transform.position.z + Length;
}
