using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "GameData/Character")]
public class CharacterData : ScriptableObject
{
    [Tooltip("Unique ID for the character (e.g., lionGladiator)")]
    public string characterID;

    [Tooltip("Display name for the character")]
    public string characterName;

    [Tooltip("Reference to the character GameObject in the scene or prefab")]
    public GameObject characterPrefab;
}