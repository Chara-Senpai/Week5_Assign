using UnityEngine;
using UnityEngine.Pool;

public class HomingMissile : MonoBehaviour
{
    [SerializeField] private float speed = 8f;
    [SerializeField] private float turnSpeed = 2f;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private float hitRadius = 0.5f;

    private ObjectPool<HomingMissile> pool;
    private PlayerController target;
    private float age;
    private bool released;

    public void Init(ObjectPool<HomingMissile> ownerPool, PlayerController player, Vector3 position)
    {
        pool = ownerPool;
        target = player;
        age = 0f;
        released = false;

        position.z = 0f;
        transform.position = position;

        Vector3 dir = player.transform.position - position;
        dir.z = 0f;
        transform.rotation = dir.sqrMagnitude > 0.0001f
            ? Quaternion.LookRotation(Vector3.forward, dir)
            : Quaternion.identity;
    }

    private void Update()
    {
        if (released) return;

        age += Time.deltaTime;
        if (age >= lifetime)
        {
            Release();
            return;
        }

        Vector3 toTarget = target.transform.position - transform.position;
        toTarget.z = 0f;

        if (toTarget.sqrMagnitude <= hitRadius * hitRadius)
        {
            target.TakeHit();
            Release();
            return;
        }

        Quaternion desired = Quaternion.LookRotation(Vector3.forward, toTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, desired, turnSpeed * Time.deltaTime);
        transform.position += transform.up * (speed * Time.deltaTime);
    }

    private void Release()
    {
        released = true;
        pool.Release(this);
    }
}