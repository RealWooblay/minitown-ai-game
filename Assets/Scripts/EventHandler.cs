using UnityEngine;
using System.Collections;

[System.Serializable]
public class Event
{
    public string eventType;
    public string location;
    public string character;
    public string eventExplanation;
}

public class EventHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AIClient aiClient;

    [Header("Event Timing (in seconds)")]
    [SerializeField] private float eventIntervalMin = 30f;
    [SerializeField] private float eventIntervalMax = 60f;

    private void Start()
    {
        StartCoroutine(EventRoutine());
    }

    private IEnumerator EventRoutine()
    {
        while (true)
        {
            // Wait random
            float waitTime = Random.Range(eventIntervalMin, eventIntervalMax);
            yield return new WaitForSeconds(waitTime);

            // Ask AI for event
            aiClient.GenerateEvent();
            yield return new WaitForSeconds(2f);

            // Get event JSON
            string eventJson = aiClient.GetLastEvent();
            if (!string.IsNullOrEmpty(eventJson))
            {
                eventJson = CleanJsonString(eventJson);
                Event theEvent = JsonUtility.FromJson<Event>(eventJson);

                //-------------------- Select the event character --------------------//
                CharacterData cData = GameDataManager.Instance.GetCharacterData(theEvent.character);
                if (cData == null)
                {
                    Debug.LogWarning($"No CharacterData for ID '{theEvent.character}'.");
                    continue;
                }

                // The real scene instance:
                GameObject sceneChar = SpawnManager.Instance.GetSpawnedCharacter(cData.characterID);
                if (sceneChar == null)
                {
                    Debug.LogWarning($"No spawned character for ID '{cData.characterID}'.");
                    continue;
                }

                //-------------------- Select the event location --------------------//
                LocationData locData = GameDataManager.Instance.GetLocationData(theEvent.location);
                if (locData == null)
                {
                    Debug.LogWarning($"No location found for ID '{theEvent.location}'.");
                    continue;
                }

                // Move the character to the location
                Vector3 targetPos = new Vector3(locData.targetPosition.x, locData.targetPosition.y, sceneChar.transform.position.z);
                StartCoroutine(MoveCharacterToPosition(sceneChar, targetPos));

                //-------------------- Handle the event type --------------------//


                // Generate new lore
                aiClient.GenerateLore(theEvent.eventExplanation);

            }
            else
            {
                Debug.LogWarning("No event received from AIClient.");
            }
        }
    }

    private IEnumerator MoveCharacterToPosition(GameObject character, Vector3 targetPosition)
    {
        float speed = 2f;
        while (Vector3.Distance(character.transform.position, targetPosition) > 0.1f)
        {
            character.transform.position = Vector3.MoveTowards(
                character.transform.position,
                targetPosition,
                speed * Time.deltaTime
            );
            yield return null;
        }
        character.transform.position = targetPosition;
    }

    private string CleanJsonString(string json)
    {
        json = json.Replace("```json", "");
        json = json.Replace("```", "");
        return json.Trim();
    }
}