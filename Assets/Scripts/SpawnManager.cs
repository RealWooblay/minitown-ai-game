using UnityEngine;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;
    private Dictionary<string, GameObject> spawnedCharacters = new Dictionary<string, GameObject>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // For each character in GameDataManager
        foreach (var cData in GameDataManager.Instance.characters)
        {
            // cData.characterPrefab is the prefab in the Project
            if (cData.characterPrefab != null)
            {
                Vector3 spawnPos = Vector3.zero;
                Quaternion spawnRot = Quaternion.identity;

                // Instantiate the prefab
                GameObject newObj = Instantiate(cData.characterPrefab, spawnPos, spawnRot);
                newObj.name = cData.characterName;

                // Store the new scene instance in a dictionary
                spawnedCharacters[cData.characterID.ToLower()] = newObj;
            }
        }
    }

    public GameObject GetSpawnedCharacter(string charID)
    {
        if (spawnedCharacters.TryGetValue(charID.ToLower(), out GameObject obj))
        {
            return obj;
        }
        return null;
    }
}