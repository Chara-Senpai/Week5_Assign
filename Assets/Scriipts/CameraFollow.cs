using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;

    private float zOffset;

    private void Start()
    {
        zOffset = transform.position.z;
    }

    private void LateUpdate()
    {
        Vector3 p = target.position;
        p.z = zOffset;
        transform.position = p;
    }
}