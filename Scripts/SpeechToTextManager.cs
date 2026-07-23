using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public enum SpeechToTextStatus
{
    Error,
    Partial,
    Final
}

public delegate void SpeechToTextCallback(SpeechToTextStatus status, string message);

public class SpeechToTextManager : MonoBehaviour
{

    private const string PLUGIN_NAME = "com.uccnasc.xr.VoicePlugin";
    private bool isListening = false;

    private readonly List<SpeechToTextCallback> callbacks = new();

    public bool IsListening() => isListening;

    void Start() {
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone)) {
            Permission.RequestUserPermission(Permission.Microphone);
        }
    }

    public void RegisterCallback(SpeechToTextCallback callback) {
        callbacks.Add(callback);
    }

    private void Notify(SpeechToTextStatus status, string message) {
        foreach (var callback in callbacks) {
            callback(status, message);
        }
    }

    public void ToggleSpeechRegistration() {
        if (!isListening) {
            StartSpeechToText();
        } else {
            StopSpeechToText();
        }
    }

    private void StartSpeechToText() {
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone)) {
            return;
        }

        isListening = true;
        using var voicePlugin = new AndroidJavaClass(PLUGIN_NAME);
        voicePlugin.CallStatic("startListening", gameObject.name, "OnVoiceResult");
    }

    private void StopSpeechToText() {
        isListening = false;
        using var voicePlugin = new AndroidJavaClass(PLUGIN_NAME);
        voicePlugin.CallStatic("stopListening");
    }

    public void OnVoiceResult(string rawPayload) {
        if (rawPayload.StartsWith("PARTIAL:")) {
            string text = rawPayload["PARTIAL:".Length..];
            Notify(SpeechToTextStatus.Partial, text);
        }
        else if (rawPayload.StartsWith("FINAL:")) {
            isListening = false;
            string text = rawPayload["FINAL:".Length..];
            Notify(SpeechToTextStatus.Final, text);
        }
        else if (rawPayload.StartsWith("ERROR:")) {
            string errorCode = rawPayload["ERROR:".Length..];

            // Safely ignore code 5 since it indicates it was already terminated
            if (errorCode != "5") {
                Notify(SpeechToTextStatus.Error, errorCode);
            }

            isListening = false;
        }
    }
}
