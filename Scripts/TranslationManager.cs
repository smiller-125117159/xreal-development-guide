using System.Collections.Generic;
using UnityEngine;

public enum TranslationStatus
{
    Error,
    Success
}

public delegate void TranslationCallback(TranslationStatus status, string message);

public class TranslationManager : MonoBehaviour
{
    private const string PLUGIN_NAME = "com.uccnasc.xr.TranslatePlugin";

    private readonly List<TranslationCallback> callbacks = new();

    public void RegisterCallback(TranslationCallback callback) {
        callbacks.Add(callback);
    }

    private void Notify(TranslationStatus status, string message) {
        foreach (var callback in callbacks) {
            callback(status, message);
        }
    }

    public void StartTranslation(string text) {
        if (string.IsNullOrEmpty(text)) return;

        using var translatePlugin = new AndroidJavaClass(PLUGIN_NAME);
        translatePlugin.CallStatic("translateText", text, gameObject.name, "OnTranslateResult");
    }

    // Plugin callback
    public void OnTranslateResult(string response) {
        if (!TryParseResponse(response, out string payload)) {
            Notify(TranslationStatus.Error, payload);
            return;
        }

        Notify(TranslationStatus.Success, payload);
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