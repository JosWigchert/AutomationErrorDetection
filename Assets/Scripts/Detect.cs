using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class Detect : MonoBehaviour
{
    public bool isEnabled = true;
    public bool writeToDataset = true;
    public string normalDirectory = "dataset/normal/";
    public string abnormalDirectory = "dataset/abnormal/";

    public enum Orientation { LSL = 0, SSL = 1 }

    public string apiUrl = "https://localhost:7059/api/v1/segment";
    public Orientation correctOrientation;
    public RenderTexture segmentationTexture;


    private void Start()
    {
        StartCoroutine(DetectImage());
    }

    IEnumerator DetectImage()
    {
        while (true)
        {
            if (!isEnabled)
            {
                yield return new WaitForSeconds(1);
                continue;
            }

            yield return new WaitForEndOfFrame();

            var screen = ScreenCapture.CaptureScreenshotAsTexture();
            byte[] image = screen.EncodeToJPG(80);
            //File.WriteAllBytes("send.jpg", image);

            DetectParams detectParams = new DetectParams() { ImageBase64 = Convert.ToBase64String(image) };

            string json = JsonConvert.SerializeObject(detectParams);
            //Debug.Log(json);
            File.WriteAllText("test.json", json);

            using (UnityWebRequest webRequest = UnityWebRequest.Post(apiUrl, "POST"))
            {
                byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
                webRequest.uploadHandler = new UploadHandlerRaw(jsonBytes);
                webRequest.downloadHandler = new DownloadHandlerBuffer();

                webRequest.SetRequestHeader("Accept", "application/json");
                webRequest.SetRequestHeader("Content-Type", "application/json");

                // Send the request and wait for a response
                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError("Error: " + webRequest.error);
                }
                else
                {
                    // Process the response data
                    string responseData = webRequest.downloadHandler.text;
                    Root segmentationResult = JsonConvert.DeserializeObject<Root>(responseData);
                    Debug.Log("API Response: " + segmentationResult.speed);
                    Draw(screen, segmentationResult);


                    if (writeToDataset)
                    {
                        if (!Directory.Exists(abnormalDirectory))
                        {
                            Directory.CreateDirectory(abnormalDirectory);
                        }
                        if (!Directory.Exists(normalDirectory))
                        {
                            Directory.CreateDirectory(normalDirectory);
                        }

                        if (segmentationResult.boxes.Any((box) => { return box.@class.id != (int)correctOrientation; }))
                        {
                            File.WriteAllBytes($"{abnormalDirectory}{DateTime.Now.ToString("yyyyMMdd_HHmmssff")}.jpg", image);
                        }
                        else
                        {
                            File.WriteAllBytes($"{normalDirectory}{DateTime.Now.ToString("yyyyMMdd_HHmmssff")}.jpg", image);
                        }
                    }
                 }
            }

            yield return null;
        }
    }

    public void Draw(Texture2D texture, Root result)
    {
        if (segmentationTexture != null)
        {
            foreach (var box in result.boxes)
            {
                DrawLabeledBox(texture, box.bounds, box.@class.id == (uint)correctOrientation ? Color.green : Color.red);
                texture.Apply();
            }


            Graphics.Blit(texture, segmentationTexture);
        }
        else
        {
            Debug.LogError("segmentationTexture or texture is null");
        }
    }

    void DrawLabeledBox(Texture2D texture, Bounds boxRect, Color color)
    {
        int x = Mathf.RoundToInt(boxRect.x);
        int y = Mathf.RoundToInt(boxRect.y);
        int width = Mathf.RoundToInt(boxRect.width);
        int height = Mathf.RoundToInt(boxRect.height);

        for (int i = x; i < x + width; i++)
        {
            for (int j = y; j < y + height; j++)
            {
                texture.SetPixel(i, j, color);
            }
        }
    }

    void DrawTextAboveBox(Texture2D texture, string text, Bounds boxRect, Color color)
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = color;
        style.fontSize = 20;

        float textWidth = style.CalcSize(new GUIContent(text)).x;
        float textHeight = style.CalcSize(new GUIContent(text)).y;

        float textX = boxRect.x + (boxRect.width - textWidth) / 2;
        float textY = boxRect.y - textHeight - 5; // Adjust the distance above the box

        Rect rect = new Rect(textX, textY, texture.width, texture.height);
        GUI.Label(rect, text, style);
    }
}
