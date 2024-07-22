using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollSnap : MonoBehaviour
{
    private static ScrollSnap _instance;
    public static ScrollSnap instance{
        get{
            if(_instance == null){
                _instance = FindObjectOfType<ScrollSnap>();
            }
            return _instance;
        }
    }
    public RectTransform Content;
    public HorizontalLayoutGroup LayoutGroup;
    public EventTrigger EventTrigger;
    public Vector2 targetPosition;

    public int currentIndex = 0;
    public GameObject prefab;
    
    bool valuechanged = false;
    bool pointUp = false;

    public void SetCenterItem(int index){
        currentIndex = index;
        Debug.Log($"Setting center item to {index}");
        SetCenterItem();
    }

    private void SetCenterItem(){
        Vector2 childSize = LayoutGroup.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta;
        targetPosition = new Vector2(-currentIndex*(childSize.x+LayoutGroup.spacing), 0);
        //print($"{LayoutGroup.padding.left},{childSize.x},{LayoutGroup.spacing},{currentIndex}");
        Content.anchoredPosition = targetPosition;
    }

    public int GetCenterChildIndex(){
        Vector2 childSize = LayoutGroup.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta;
        int closestIndex = 0;
        float closestDistance = float.MaxValue;
        for (int i = 0; i < Content.childCount; i++)
        {
            float distance = Mathf.Abs(childSize.x/2+LayoutGroup.padding.left + Content.GetChild(i).InverseTransformPoint(Content.transform.parent.position).x);
            if(distance < closestDistance){
                closestDistance = distance;
                closestIndex = i;
            }
        }

        return closestIndex;
        
    }

    public void PointDown(){
        //print("PointDown");
        pointUp = false;
    }

    public void PointUp(){
        //print("PointUp");
        pointUp = true;
    }

    public void ValueChanged(){
        //print("ValueChanged");
        valuechanged = true;
    }

    public void NewChat(){
        if(Content.childCount > 1){
            Chat chat = Content.GetChild(Content.childCount-2).GetComponent<Chat>();
            if(chat.QuestionText == ""){
                currentIndex--;
                return;
            }
        }
        

        Transform lastChat = Content.GetChild(Content.childCount-1);
        GameObject newChat = Instantiate(prefab, Content);
        newChat.transform.parent = Content;
        lastChat.SetAsLastSibling();
        newChat.GetComponent<Chat>().uuid = System.Guid.NewGuid().ToString();
    }

    public void OldChat(string uuid ,string question, string response, string image){
        // Transform firstobj = Content.GetChild(0);
        GameObject newChat = Instantiate(prefab, Content);
        newChat.transform.parent = Content;
        newChat.transform.SetAsFirstSibling();
        // firstobj.SetAsFirstSibling();
        newChat.GetComponent<Chat>().QuestionText = question;
        newChat.GetComponent<Chat>().ResponseText = response;
        newChat.GetComponent<Chat>().ResponseImage = image;
        newChat.GetComponent<Chat>().uuid = uuid;
    }

    private void Update() {
        if(valuechanged && pointUp){
            currentIndex = GetCenterChildIndex();
            if(currentIndex == Content.childCount-1)
                NewChat();
            SetCenterItem();
        }
    }


}
