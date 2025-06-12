using UnityEngine;

public class StartCourse : MonoBehaviour
{
    [SerializeField] GameObject holePrefab;
    [SerializeField] GameObject ballPrefab;

    private Ball currentBall;

    private Transform spawnPoint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnPoint = new GameObject("SpawnPoint").transform;
        spawnPoint.position = new Vector3(0, 0.5f, 0);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartHole()
    {
        GameObject newBall = Instantiate(ballPrefab, spawnPoint.position, Quaternion.identity);
        currentBall = newBall.GetComponent<Ball>();

        GameObject newHole = Instantiate(holePrefab, Vector3.zero, Quaternion.identity);
    }
}
