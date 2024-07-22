using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Chat : MonoBehaviour
{
    [SerializeField]
    private string _uuid;
    public string uuid{
        get{
            return _uuid;
        }
        set{
            _uuid = value;
            uuidText.text = value;
        }
    }
    [SerializeField]
    private Text responseText, questionText, questionPlaceholder, uuidText;
    
    public int textSize;
    public Font font;
    public Color textColor;
    public string placeHolderText;
    public Image responseImage;
    public Vector2 imageMaxSize = new Vector2(75,75);
    private string _responseImage;

    public string ResponseImage{
        set{
            _responseImage = value;
            if(value!="" && Base64ToImage.Base64ToSprite(value, out Sprite sprite)){
                // Debug.Log("Setting image");
                responseImage.sprite = sprite;
                responseImage.rectTransform.sizeDelta = Base64ToImage.GetTextureScaledSize(sprite.texture, imageMaxSize);
                // responseImage.GetComponent<LayoutElement>().ignoreLayout = false;
            }else{
                // Debug.Log("No image");
                // responseImage.GetComponent<LayoutElement>().ignoreLayout = true;
                responseImage.rectTransform.sizeDelta = Vector2.zero;
            }
        }
        get{
            return _responseImage;
        }
    }


    
    public string ResponseText{
        get => responseText.text;
        set => responseText.text = value;
    }

    public string QuestionText{
        get => questionText.text;
        set => questionText.transform.parent.GetComponent<InputField>().text = value;
    }

    private void OnValidate() {
        SetAllTextStyles();
        if(uuid == ""){
            uuid = System.Guid.NewGuid().ToString();
        }
    }

    private void SetAllTextStyles(){
        SetTextStyle(responseText);
        SetTextStyle(questionText);
        SetTextStyle(questionPlaceholder,placeHolderText);
    }

    private void SetTextStyle(Text text, string str=null){
        text.font = font;
        text.fontSize = textSize;
        text.color = textColor;
        if(str != null){
            text.text = str;
        }
    }

    public void ToggleIgnoresLayout(){
        LayoutElement layoutElement = responseImage.GetComponent<LayoutElement>();
        layoutElement.ignoreLayout = !layoutElement.ignoreLayout;
        if(layoutElement.ignoreLayout){
            responseImage.rectTransform.sizeDelta = Base64ToImage.GetTextureScaledSize(responseImage.sprite.texture, imageMaxSize);
        }
    }
    
    public void SubmitQuestion(){
        print(questionText.text);
        // History.instance.SendQuestion(uuid, questionText.text, responseText);
        Api.instance.SendQuestion(this);
    }

    public void MicrophoneButton(){
        print("MicrophoneButton");
    }



    
}
