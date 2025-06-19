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
    }

    // Update is called once per frame
    void Update()
    {
        beginGame();
    }

    void beginGame()
    {
        if (CourseManager.isHoleStarted() == false)
        {
            CourseManager.StartHole();
        }
    }
}
