using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using TMPro;

public class AIClient : MonoBehaviour
{
    [Header("Server Settings")]
    [SerializeField]
    private string serverUrl = "http://127.0.0.1:3000";
    [SerializeField]
    private string apiKey = "my_secret_key";

    [Header("UI Display (Optional)")]
    [SerializeField]
    private TMP_Text loreTextUI;

    // Holds the latest lore (JSON string) returned by the AI server
    private string currentLore = "";

    void Start()
    {
        // Automatically generate the initial lore when the game starts.
        GenerateLore();
    }

    //=============================//
    //         1) AskAgent        //
    //=============================//
    /// Sends an arbitrary question to the /ask endpoint, logs result to Console.
    public void AskAgent(string question)
    {
        StartCoroutine(AskAgentRoutine(question));
    }

    private IEnumerator AskAgentRoutine(string question)
    {
        string endpoint = serverUrl + "/ask";
        // The server expects { "question": "...", "session_id": "...optional..." }
        string jsonPayload = "{\"question\": \"" + question + "\"}";

        using (UnityWebRequest request = new UnityWebRequest(endpoint, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("X-Api-Key", apiKey);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseText = request.downloadHandler.text;
                Debug.Log("[AIClient] /ask response: " + responseText);
            }
            else
            {
                Debug.LogError("[AIClient] /ask error: " + request.error);
            }
        }
    }

    //=============================//
    //       2) GenerateLore      //
    //=============================//
    /// Fetches or updates lore from /generate_lore.
    /// Optionally pass a recent event if you want the AI to incorporate it.
    public void GenerateLore(string recentEvent = "")
    {
        StartCoroutine(GenerateLoreRoutine(recentEvent));
    }

    private IEnumerator GenerateLoreRoutine(string recentEvent)
    {
        string endpoint = serverUrl + "/generate_lore";

        // We'll send the current lore plus any recent event so the AI can weave them together.
        LoreRequest req = new LoreRequest
        {
            recent_event = recentEvent,
            lore = currentLore
        };
        string jsonPayload = JsonUtility.ToJson(req);

        using (UnityWebRequest request = new UnityWebRequest(endpoint, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("X-Api-Key", apiKey);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseText = request.downloadHandler.text;
                LoreResponse resp = JsonUtility.FromJson<LoreResponse>(responseText);

                // The AI server returns resp.lore which can be plain text or JSON.
                currentLore = resp.lore;
                Debug.Log("[AIClient] /generate_lore => " + currentLore);

                // Display the new lore in the UI (if assigned).
                if (loreTextUI != null)
                {
                    loreTextUI.text = currentLore;
                }
            }
            else
            {
                Debug.LogError("[AIClient] /generate_lore error: " + request.error);
            }
        }
    }

    //=============================//
    //       3) GenerateEvent     //
    //=============================//
    /// Creates a random event via /generate_event, logs it, 
    /// then optionally merges it back into lore.
    public void GenerateEvent()
    {
        StartCoroutine(GenerateEventRoutine());
    }

    public string lastEvent = "";
    public string GetLastEvent() => lastEvent;

    private IEnumerator GenerateEventRoutine()
    {
        string endpoint = serverUrl + "/generate_event";

        // Get game data from GameDataManager
        string gameDataJson = GameDataManager.Instance.GetGameDataAsJson();
        Debug.Log("Game data sent to server: " + gameDataJson);

        // Prepare payload with both current lore and game data.
        EventRequestPayload payloadData = new EventRequestPayload
        {
            lore = currentLore,
            gameData = gameDataJson
        };
        string jsonPayload = JsonUtility.ToJson(payloadData);

        using (UnityWebRequest request = new UnityWebRequest(endpoint, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("X-Api-Key", apiKey);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseText = request.downloadHandler.text;
                EventResponse resp = JsonUtility.FromJson<EventResponse>(responseText);

                lastEvent = resp.@event;  // Store the event for external use.
                Debug.Log("[AIClient] /generate_event => " + lastEvent);
            }
            else
            {
                Debug.LogError("[AIClient] /generate_event error: " + request.error);
            }
        }
    }

    //=============================//
    //       4) GenerateDialogue  //
    //=============================//
    /// Creates random dialogue via /generate_dialogue, logs it,
    /// then optionally merges it back into lore.
    public void GenerateDialogue()
    {
        StartCoroutine(GenerateDialogueRoutine());
    }

    private IEnumerator GenerateDialogueRoutine()
    {
        string endpoint = serverUrl + "/generate_dialogue";

        // We pass the current lore so the AI can tailor the dialogue.
        LoreData payload = new LoreData { lore = currentLore };
        string jsonPayload = JsonUtility.ToJson(payload);

        using (UnityWebRequest request = new UnityWebRequest(endpoint, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("X-Api-Key", apiKey);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseText = request.downloadHandler.text;
                EventResponse resp = JsonUtility.FromJson<EventResponse>(responseText);

                Debug.Log("[AIClient] /generate_dialogue => " + resp.@event);
            }
            else
            {
                Debug.LogError("[AIClient] /generate_dialogue error: " + request.error);
            }
        }
    }

    //=============================//
    //         Data Classes       //
    //=============================//
    [System.Serializable]
    private class LoreRequest
    {
        public string recent_event;
        public string lore;
    }

    [System.Serializable]
    private class LoreResponse
    {
        public string lore;
    }

    [System.Serializable]
    private class EventResponse
    {
        // 'event' is a reserved keyword in some contexts, so we name it '@event'.
        public string @event;
    }

    [System.Serializable]
    private class LoreData
    {
        public string lore;
    }

    [System.Serializable]
    private class EventRequestPayload
    {
        public string lore;
        public string gameData;
    }

    //=============================//
    //       (Optional) Helpers   //
    //=============================//

    /// <summary>
    /// Gets the currently stored lore (raw string).
    /// </summary>
    public string GetCurrentLore() => currentLore;

    /// <summary>
    /// Manually sets the stored lore (if you need to override it).
    /// Also updates the UI text if present.
    /// </summary>
    public void SetCurrentLore(string newLore)
    {
        currentLore = newLore;
        if (loreTextUI != null)
        {
            loreTextUI.text = currentLore;
        }
    }
}