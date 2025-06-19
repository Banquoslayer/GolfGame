using System;
using UnityEditor;
using UnityEngine;

public class StartCourse : MonoBehaviour
{
    [SerializeField] GameObject holePrefab;
    [SerializeField] GameObject ballPrefab;

    private Transform spawnPoint;

    bool isHoleSet = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnPoint = new GameObject("SpawnPoint").transform;
        spawnPoint.position = new Vector3(0, 0.03f, 0);
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void StartHole()
    {
        Instantiate(ballPrefab, spawnPoint.position, Quaternion.identity);

        Instantiate(holePrefab, Vector3.zero, Quaternion.identity);

        isHoleSet = true;
    }

    public bool isHoleStarted()
    {
        return isHoleSet;
    }
}
