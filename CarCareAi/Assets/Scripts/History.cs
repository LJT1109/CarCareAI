using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class History : MonoBehaviour
{
    private static History _instance;
    public static History instance{
        get{
            if(_instance == null){
                _instance = FindObjectOfType<History>();
            }
            return _instance;
        }
    }
    private string filename = "{user}/{uuid}.json";
    public string user = "user";


    public void SaveHistory(string uuid, string data)
    {
        Debug.Log($"{user}, {uuid}, {data}");
        string path = filename.Replace("{user}", user).Replace("{uuid}", uuid);
        path = $"{Application.persistentDataPath}/{path}";
        if (!System.IO.Directory.Exists(path))
        {
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
        }
        System.IO.File.WriteAllText(path, data);
        Debug.Log($"Saved history to {path}");
    }

    public void LoadHistories(string user){
        string path = user;
        path = $"{Application.persistentDataPath}/{path}";
        Debug.Log($"Loading history from {path}");
        if (!System.IO.Directory.Exists(path))
        {
            Debug.Log($"No history found for {user}");
            return;
        }
        string[] files = System.IO.Directory.GetFiles(path);
        // foreach (string file in files)
        // {
        //     string data = System.IO.File.ReadAllText(file);
        //     HistoryData history = JsonUtility.FromJson<HistoryData>(data);
        //     Debug.Log($"Loaded history from {file}");
        //     ScrollSnap.instance.OldChat(history.uuid, history.question, history.response, history.responseImage);

        // }
        List<HistoryData> histories = new List<HistoryData>();
        foreach (string file in files)
        {
            string data = System.IO.File.ReadAllText(file);
            HistoryData history = JsonUtility.FromJson<HistoryData>(data);
            histories.Add(history);
        }

        histories.Sort((b, a) => a.timestamp.CompareTo(b.timestamp));
        foreach (HistoryData history in histories)
        {
            Debug.Log($"Loaded history from {history.uuid}");
            ScrollSnap.instance.OldChat(history.uuid, history.question, history.response, history.responseImage);
        }
        ScrollSnap.instance.SetCenterItem(histories.Count-1);

        if(histories.Count == 0){
            ScrollSnap.instance.NewChat();
        }
    }

    private void Start() {
        LoadHistories(user);
    }

    [Serializable]
    public class HistoryData
    {
        public string uuid;
        public string question;
        public string response;
        public string responseImage;
        public string timestamp;

    }
}
