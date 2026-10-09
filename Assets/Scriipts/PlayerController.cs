using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float forwardSpeed = 6f;
    [SerializeField] private float turnSpeed = 70f;
    [SerializeField] private float rotationSmoothing = 8f;
    [SerializeField] private int maxHits = 5;

    public int Hits { get; private set; }
    public int MaxHits => maxHits;
    public float SurvivalTime { get; private set; }

    private float heading;

    private void Start()
    {
        heading = transform.eulerAngles.z;
    }

    private void Update()
    {
        SurvivalTime += Time.deltaTime;

        float input = Input.GetAxisRaw("Horizontal");
        heading -= input * turnSpeed * Time.deltaTime;

        Quaternion target = Quaternion.AngleAxis(heading, Vector3.forward);
        float t = 1f - Mathf.Exp(-rotationSmoothing * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, target, t);

        transform.position += target * Vector3.right * (forwardSpeed * Time.deltaTime);
    }

    public void TakeHit()
    {
        Hits++;
        if (Hits >= maxHits)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 300, 25), $"Hits: {Hits}/{maxHits}");
        GUI.Label(new Rect(10, 30, 300, 25), $"Time: {SurvivalTime:F1}s");
    }
}