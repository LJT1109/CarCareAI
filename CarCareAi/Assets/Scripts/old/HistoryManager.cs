using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HistoryManager : MonoBehaviour
{
    public RootHistoriesIndex historiesIndex;
    public UiManager uiManager;

    
    private string path => Application.persistentDataPath + "/histories.json";

    private void Start() {
        Debug.Log(path);
        LoadHistories();
    }

    private void CreateHistory(string uuid){
        History history = new History();
        //if directory not exist create new directory
        if(!System.IO.Directory.Exists(Application.persistentDataPath + "/history/")){
            System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/history/");
        }
        System.IO.File.WriteAllText(Application.persistentDataPath + "/history/" + uuid + ".json", JsonUtility.ToJson(history));
    }

    public void LoadHistory(int index){
        //open file from path+/history/uuid.json
        string uuid = historiesIndex.historiesIndex[index].uuid;
        string historyPath = Application.persistentDataPath + "/history/" + uuid + ".json";
        if(!System.IO.File.Exists(historyPath)){
            Debug.LogError("History file not found");
            return;
        }

        string historyJson = System.IO.File.ReadAllText(historyPath);
        History history = JsonUtility.FromJson<History>(historyJson);
        string question = history.question;
        ResponseResult responseResult = ResponseParse(uuid, question,history.response);

        if(responseResult.texture != null){
            uiManager.SetResult(uuid, question, responseResult.response, responseResult.texture);
        }else{
            uiManager.SetResult(uuid, question, responseResult.response);
        }
        
    }

    public ResponseResult ResponseParse(string uuid, string question ,string response){
        if(response.Contains("<#END#>")){
            string jsonString = "{\"imageJson\":" + response.Split("<#TEXT_END#>")[1].Split("<#IMAGE_END#>")[0] + "}";
            Debug.Log(jsonString);
            RootObject rootObject = JsonUtility.FromJson<RootObject>(jsonString);
            Debug.Log(rootObject.imageJson.img);
            Debug.Log(rootObject.imageJson.filetype);
            byte[] textureBytes = Convert.FromBase64String(rootObject.imageJson.img);
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(textureBytes);
            SaveHistory(uuid, question, response);
            return new ResponseResult{response = response.Split("<#TEXT_END#>")[0], texture = texture};
        }else if(response.Contains("<#IMAGE_END#>")){
            string jsonString = "{\"imageJson\":" + response.Split("<#TEXT_END#>")[1].Split("<#IMAGE_END#>")[0] + "}";
            Debug.Log(jsonString);
            RootObject rootObject = JsonUtility.FromJson<RootObject>(jsonString);
            Debug.Log(rootObject.imageJson.img);
            Debug.Log(rootObject.imageJson.filetype);
            byte[] textureBytes = Convert.FromBase64String(rootObject.imageJson.img);
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(textureBytes);
            return new ResponseResult{response = response.Split("<#TEXT_END#>")[0], texture = texture};
        }else{
            return new ResponseResult{response = response};
        }


        

    }

    public void SaveHistory(string uuid, string question, string rawResponse){
        History history = new History();
        history.question = question;
        history.response = rawResponse;
        System.IO.File.WriteAllText(Application.persistentDataPath + "/history/" + uuid + ".json", JsonUtility.ToJson(history));
    }

    public void LoadHistories()
    {
        //if file not exist create new file
        if(!System.IO.File.Exists(path))
        {
            //add new empty history
            NewEmptyHistory();
            System.IO.File.WriteAllText(path, JsonUtility.ToJson(historiesIndex));
        }else{
            historiesIndex = JsonUtility.FromJson<RootHistoriesIndex>(System.IO.File.ReadAllText(path));
            LoadHistory(historiesIndex.historiesIndex.Count - 1);
        }
    }

    public void SaveHistories()
    {
        System.IO.File.WriteAllText(path, JsonUtility.ToJson(historiesIndex, true));
    }

    public void NewEmptyHistory(){
        HistoriesIndex historiesIndex = new HistoriesIndex();
        historiesIndex.uuid = Guid.NewGuid().ToString();
        historiesIndex.time = DateTime.Now.ToString();
        this.historiesIndex.historiesIndex.Add(historiesIndex);
        CreateHistory(historiesIndex.uuid);
        SaveHistories();
    }

    [Serializable]
    public class RootHistoriesIndex
    {
        public List<HistoriesIndex> historiesIndex;
    }

    [Serializable]
    public class HistoriesIndex
    {
        public string question;
        public string uuid;
        public string time;
    }

    [Serializable]
    public class History
    {
        public string question;
        public string response;
    }

    public class ResponseResult{
        public string uuid;
        public string question;
        public string response;
        public Texture2D texture;
    }

    [Serializable]
    public class ImageJson
    {
        public string img;
        public string filetype;
    }

    [Serializable]
    public class RootObject
    {
        public ImageJson imageJson;
    }
}
