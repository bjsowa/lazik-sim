using System;
using UnityEngine;

[Serializable]
public class MyCamera
{
    public string Name;
    public Camera Camera;
    public bool IsPng;

    public byte[] CaptureFrame()
    {
        //force camera update 
        Camera.Render();

        RenderTexture targetTexture = Camera.targetTexture;
        RenderTexture.active = targetTexture;

        Texture2D texture2D = new Texture2D(
            targetTexture.width, targetTexture.height, TextureFormat.RGB24, false);
        texture2D.ReadPixels(new Rect(0, 0, targetTexture.width, targetTexture.height), 0, 0);
        texture2D.Apply();

        byte[] image = IsPng ? texture2D.EncodeToPNG() : texture2D.EncodeToJPG();
        UnityEngine.Object.Destroy(texture2D); // Required to prevent leaking the texture

        return image;
    }
}
