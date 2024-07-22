using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base64ToImage : MonoBehaviour
{
    public static Texture2D Base64ToTexture2D(string base64)
    {
        byte[] bytes = System.Convert.FromBase64String(base64);
        Texture2D texture = new Texture2D(1, 1);
        texture.LoadImage(bytes);
        return texture;
    }

    public static Vector2 GetTextureSize(Texture2D texture)
    {
        return new Vector2(texture.width, texture.height);
    }

    public static void SetBase64Image(string base64, Renderer target)
    {
        Texture2D texture = Base64ToTexture2D(base64);
        target.material.mainTexture = texture;
    }

    public static bool Base64ToSprite(string base64, out Sprite sprite)
    {
        try{
            Texture2D texture = Base64ToTexture2D(base64);
            sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            return sprite != null;
        }catch{
            sprite = null;
            return false;
        }

    }

    public static Vector2 GetTextureScaledSize(Texture2D texture, Vector2 maxSize){
        float ratio = (float)texture.width / texture.height;
        float width = maxSize.x;
        float height = maxSize.y;
        if(ratio > 1){
            height = width / ratio;
        }else{
            width = height * ratio;
        }
        return new Vector2(width, height);
    }
}
