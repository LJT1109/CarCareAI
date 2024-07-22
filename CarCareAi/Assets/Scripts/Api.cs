using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using UnityEngine.UI; // Add this for using UI elements
using System.Collections.Generic;
using System; // For List

public class Api : MonoBehaviour
{
    private static Api _instance;
    public static Api instance{
        get{
            if(_instance == null){
                _instance = FindObjectOfType<Api>();
            }
            return _instance;
        }
    }
    // FastAPI endpoint URL
    public string apiURL = "http://localhost:8000/ask"; // Replace with your FastAPI server address

    // UI elements for input and output
    public Text chatHistory;

    // API key and knowledge base
    public string apiKey = "YOUR_API_KEY"; // Replace with your actual API key
    public string kb = "YOUR_KB";          // Replace with your knowledge base

    // Store chat history
    private List<string> messageHistory = new List<string>();

    public void SendQuestion(Chat chat)
    {
        chat.ResponseText = "Loading...";
        // Prepare data for the request
        AskData data = new AskData
        {
            api_key = apiKey,
            kb = kb,
            question = chat.QuestionText,
            history = messageHistory // Pass the chat history
        };

        // Convert data to JSON
        string jsonData = JsonUtility.ToJson(data);

        // Send the request
        StartCoroutine(PostRequest(jsonData,chat));
    }

    // Coroutine for sending POST request
    IEnumerator PostRequest(string jsonData,Chat chat)
    {
        // Create a POST request
        using (UnityWebRequest request = new UnityWebRequest(apiURL, "POST"))
        {
            // Set the request headers
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Accept", "application/json");

            // Set the request body
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            // Send the request
            yield return request.SendWebRequest();

            // Check for errors
            if (request.result != UnityWebRequest.Result.Success )
            {
                Debug.LogError(request.error);
                chat.ResponseText = "Error: " + request.error;
            }
            else
            {
                // Get the response
                string response = request.downloadHandler.text;
                
                ResponseData responseData = new ResponseData(response);
                chat.ResponseText = responseData.text;
                if(responseData.image != null)
                {
                    chat.ResponseImage = responseData.image.img;
                }
                // Save the chat history
                History.HistoryData historyData = new History.HistoryData{
                    uuid = chat.uuid,
                    question = chat.QuestionText,
                    response = chat.ResponseText,
                    responseImage = chat.ResponseImage,
                    timestamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss")
                };
                
                History.instance.SaveHistory(chat.uuid, JsonUtility.ToJson(historyData));

            }
        }
    }

    //你好！<#TEXT_END#><#IMAGE_END#><#END#>
    [System.Serializable]
    public class ResponseData{
        public string raw;
        public string text;
        public ImageData image;
        
        public ResponseData(string raw){
            this.raw = raw;
            string[] parts = raw.Split("<#TEXT_END#>");
            text = parts[0];
            string img = parts[1].Split("<#IMAGE_END#>")[0];
            if(img != ""){
                image = JsonUtility.FromJson<ImageData>(img);
            }else{
                image = null;
            }
                
        }

        [Serializable]
        public class ImageData{
            public string img;
            public string filetype;
        }

    }


    // Data structure for the API request
    [System.Serializable]
    public class AskData
    {
        public string api_key;
        public string kb;
        public string question;
        public List<string> history; // Add history to the data structure
    }
}
