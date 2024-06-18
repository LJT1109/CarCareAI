using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class UiManager : MonoBehaviour
{
    //public 
    public HistoryManager historyManager;
    public UIDocument uiDocument;
    public Text responseText;
    public InputField questionInputField;
    public string placeHolderText;
    public TextStyle textStyle;
    public RawImage ragImage;
    public GameObject prefab;
    public Transform spawnParent;
    public List<GameObject> histories;
    public Text UuidText;

    public UnityEvent<string> OnQuestionSubmit;

    //private
    private string question => questionInputField.text;
    private string response => responseText.text;
    private string rawResponse = "";
    private string placeHolder => questionInputField.placeholder.GetComponent<Text>().text;
    private VisualElement fontSizeDragger => uiDocument.rootVisualElement.Q<VisualElement>("fontSizeDragger");
    private SliderInt slider => uiDocument.rootVisualElement.Q<VisualElement>("FontSize").Q<SliderInt>();
    private UnityEngine.UIElements.Button addHistoryButton => uiDocument.rootVisualElement.Q<UnityEngine.UIElements.Button>("addHistoryButton");
    private int historyCount => historyManager.historiesIndex.historiesIndex.Count;

    private void Start() {
        
        textStyle.SetFontSize();
        slider.RegisterCallback<ChangeEvent<int>>(OnSliderValueChanged);
        slider.value = textStyle.fontSize;
        addHistoryButton.clicked += AddHistory;
        
    }

    private void Update() {
        if(historyCount < histories.Count && historyCount > 0){
            Destroy(histories[0]);
        }else if(historyCount > histories.Count){
            GameObject newHistory = Instantiate(prefab, spawnParent);
            newHistory.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(() => {
                historyManager.LoadHistory(histories.IndexOf(newHistory));
            });
            histories.Add(newHistory);
        }
    }

    public void AddHistory(){
        historyManager.NewEmptyHistory();
    }

    private void OnValidate() {
        questionInputField.placeholder.GetComponent<Text>().text = placeHolderText;
    }

    public void PressQuestionButton(){
        OnQuestionSubmit.Invoke(question);
        historyManager.SaveHistory(UuidText.text, question, rawResponse);
    }

    private void OnSliderValueChanged(ChangeEvent<int> evt)
    {
        textStyle.SetFontSize(evt.newValue);
    }

    public void Response(string res){ //回傳的結果傳進來
        rawResponse = res;
        historyManager.SaveHistory(UuidText.text, question, res);
        SetResult(UuidText.text, question, res);
    }

    public void ClearResult(){
        responseText.text = "";
        questionInputField.text = "";
        ragImage.texture = null;
    }

    public void SetResult(string uuid ,string question, string response, Texture2D texture=null){
        UuidText.text = uuid;
        questionInputField.text = question;
        responseText.text = response;
        ragImage.gameObject.SetActive(texture != null);
        ragImage.texture = texture;
    }



    [Serializable]
    public class TextStyle{
        public Font font;
        public int fontSize;

        public void SetFontSize(){
            SetFontSize(fontSize);
        }

        public void SetFontSize(int size){
            fontSize = size;
            Debug.Log("SetFontSize: " + size);
            foreach(GameObject tagObj in GameObject.FindGameObjectsWithTag("GlobalTextStyle")){
                Debug.Log(tagObj.name);
                tagObj.GetComponent<Text>().fontSize = size;
            }
        }
        

    }
}
