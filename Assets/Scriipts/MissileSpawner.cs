using UnityEngine;
using UnityEngine.Pool;

public class MissileSpawner : MonoBehaviour
{
    [SerializeField] private HomingMissile missilePrefab;
    [SerializeField] private PlayerController player;
    [SerializeField] private Camera cam;

    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private int baseMissilesPerWave = 1;
    [SerializeField] private float secondsPerDifficultyStep = 10f;
    [SerializeField] private float offscreenMargin = 0.25f;

    [SerializeField] private int defaultCapacity = 20;
    [SerializeField] private int maxPoolSize = 200;

    private ObjectPool<HomingMissile> pool;
    private float timer;

    private void Awake()
    {
        if (cam == null) cam = Camera.main;

        pool = new ObjectPool<HomingMissile>(
            createFunc:      () => Instantiate(missilePrefab, transform),
            actionOnGet:     m => m.gameObject.SetActive(true),
            actionOnRelease: m => m.gameObject.SetActive(false),
            actionOnDestroy: m => Destroy(m.gameObject),
            collectionCheck: false,
            defaultCapacity: defaultCapacity,
            maxSize:         maxPoolSize);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer < spawnInterval) return;
        timer -= spawnInterval;

        int count = baseMissilesPerWave + Mathf.FloorToInt(player.SurvivalTime / secondsPerDifficultyStep);
        for (int i = 0; i < count; i++)
        {
            HomingMissile m = pool.Get();
            m.Init(pool, player, GetOffscreenPosition());
        }
    }

    private Vector3 GetOffscreenPosition()
    {
        float lo = -offscreenMargin;
        float hi = 1f + offscreenMargin;
        Vector2 vp;

        switch (Random.Range(0, 4))
        {
            case 0:  vp = new Vector2(lo, Random.Range(lo, hi)); break;
            case 1:  vp = new Vector2(hi, Random.Range(lo, hi)); break;
            case 2:  vp = new Vector2(Random.Range(lo, hi), lo); break;
            default: vp = new Vector2(Random.Range(lo, hi), hi); break;
        }

        Vector3 world = cam.ViewportToWorldPoint(new Vector3(vp.x, vp.y, -cam.transform.position.z));
        world.z = 0f;
        return world;
    }
}