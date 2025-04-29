using UnityEngine;

[CreateAssetMenu(fileName = "LocationData", menuName = "GameData/Location")]
public class LocationData : ScriptableObject
{
    [Tooltip("A unique identifier for this location (e.g., holyTree)")]
    public string locationID;

    [Tooltip("Display name for this location")]
    public string locationName;

    [Tooltip("Reference to Position of location in the sceee")]
    public Vector2 targetPosition;
}