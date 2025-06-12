using UnityEngine;
using UnityEngine.InputSystem;

public class GameLoop : MonoBehaviour
{
    [SerializeField] MonoBehaviour StartCourse;

    StartCourse CourseManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CourseManager = StartCourse.GetComponent<StartCourse>();
        CourseManager.StartHole();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
