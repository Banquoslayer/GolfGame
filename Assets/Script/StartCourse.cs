using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

public class StartCourse : MonoBehaviour
{
    [SerializeField] GameObject holePrefab;
    [SerializeField] GameObject ballPrefab;

    GameObject ball;
    GameObject hole;

    private Transform spawnPoint;

    private bool isHoleSet = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnPoint = new GameObject("SpawnPoint").transform;
        spawnPoint.position = new Vector3(0, 0.03f, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (ball && ball.GetComponent<Ball>().GetHasLanded()  && !ball.GetComponent<Ball>().IsReadyToLaunch())
        {
            Destroy(ball);
            ball = Instantiate(ballPrefab, spawnPoint.position, Quaternion.identity);
        }
    }

    public void StartHole()
    {
        ball = Instantiate(ballPrefab, spawnPoint.position, Quaternion.identity);

        hole = Instantiate(holePrefab, Vector3.zero, Quaternion.identity);

        isHoleSet = true;
    }

    public bool IsHoleStarted()
    {
        return isHoleSet;
    }
}
