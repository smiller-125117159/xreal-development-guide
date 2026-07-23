using System.Collections.Generic;
using UnityEngine;

public enum TextRecognitionStatus
{
    Error,
    Success
}

public delegate void TextRecognitionCallback(TextRecognitionStatus status, string message);

public class TextRecognitionManager : MonoBehaviour
{
    private const string PLUGIN_NAME = "com.uccnasc.xr.TextPlugin";

    private readonly List<TextRecognitionCallback> callbacks = new();

    public void RegisterCallback(TextRecognitionCallback callback) {
        callbacks.Add(callback);
    }

    private void Notify(TextRecognitionStatus status, string message) {
        foreach (var callback in callbacks) {
            callback(status, message);
        }
    }

    public void StartTextRecognition(Texture2D image) {
        using var ocrPlugin = new AndroidJavaClass(PLUGIN_NAME);
        ocrPlugin.CallStatic("processImage", image.EncodeToJPG(100), gameObject.name, "OnTextResult");
    }

    // Plugin callback
    public void OnTextResult(string response) {
        if (!TryParseResponse(response, out string payload)) {
            Notify(TextRecognitionStatus.Error, payload);
            return;
        }

        Notify(TextRecognitionStatus.Success, payload);
    }

    private bool TryParseResponse(string response, out string payload) {
        if (string.IsNullOrEmpty(response)) {
            payload = "Received empty response";
            return false;
        }

        if (response.StartsWith("SUCCESS:")) {
            payload = response["SUCCESS:".Length..];
            return true;
        }

        if (response.StartsWith("ERROR:")) {
            payload = response["ERROR:".Length..];
            return false;
        }

        payload = $"Received malformed response: {response}";
        return false;
    }
}
