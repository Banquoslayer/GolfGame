using UnityEngine;

[CreateAssetMenu(fileName = "NewClub", menuName = "Golf/Club")]
public class ClubData : ScriptableObject
{
    public string clubName;
    public float launchAngle = 10f; // degrees, realistic driver angle
    public float clubHeadSpeed = 44.7f; // m/s
    public float smashFactor = 1.5f; // efficiency
}