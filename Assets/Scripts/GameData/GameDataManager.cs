using UnityEngine;
using System.Collections.Generic;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance;

    [Header("Locations")]
    public List<LocationData> locations;

    [Header("Characters")]
    public List<CharacterData> characters;

    public List<string> allowedEvents = new List<string>() { "moves" };

    // Dictionaries for fast lookups
    private Dictionary<string, LocationData> locationDict;
    private Dictionary<string, CharacterData> characterDict;

    private void Awake()
    {
        // Singleton instance
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Build dictionaries
        locationDict = new Dictionary<string, LocationData>();
        foreach (var loc in locations)
        {
            if (!string.IsNullOrEmpty(loc.locationID))
            {
                string key = loc.locationID.Trim().ToLower();
                locationDict[key] = loc;
            }
        }

        characterDict = new Dictionary<string, CharacterData>();
        foreach (var ch in characters)
        {
            if (!string.IsNullOrEmpty(ch.characterID))
            {
                string key = ch.characterID.Trim().ToLower();
                characterDict[key] = ch;
            }
        }
    }

    // Returns a JSON representation of the available game data.
    public string GetGameDataAsJson()
    {
        // Build a container with the location/character/event data
        GameDataContainer container = new GameDataContainer
        {
            locations = new List<SimpleLocation>(),
            characters = new List<SimpleCharacter>(),
            events = allowedEvents
        };

        // Fill locations
        foreach (var loc in locations)
        {
            container.locations.Add(new SimpleLocation
            {
                id = loc.locationID,
                name = loc.locationName,
                position = new float[] { loc.targetPosition.x, loc.targetPosition.y }
            });
        }

        // Fill characters
        foreach (var ch in characters)
        {
            container.characters.Add(new SimpleCharacter
            {
                id = ch.characterID,
                name = ch.characterName
            });
        }

        return JsonUtility.ToJson(container, true);
    }

    // Lookup a location by its ID
    public LocationData GetLocationData(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        locationDict.TryGetValue(id.Trim().ToLower(), out LocationData loc);
        return loc;
    }

    // Lookup a character by ID
    public CharacterData GetCharacterData(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        characterDict.TryGetValue(id.Trim().ToLower(), out CharacterData ch);
        return ch;
    }
}

[System.Serializable]
public class GameDataContainer
{
    public List<SimpleLocation> locations;
    public List<SimpleCharacter> characters;
    public List<string> events;
}

[System.Serializable]
public class SimpleLocation
{
    public string id;
    public string name;
    public float[] position; // [x, y]
}

[System.Serializable]
public class SimpleCharacter
{
    public string id;
    public string name;
}