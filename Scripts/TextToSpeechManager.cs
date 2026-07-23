using UnityEngine;

public class TextToSpeechManager : MonoBehaviour
{
    private const string PLUGIN_NAME = "com.uccnasc.xr.TtsPlugin";

    void Start() {
        using AndroidJavaClass ttsPlugin = new(PLUGIN_NAME);
        ttsPlugin.CallStatic("initialize");
    }

    public void Speak(string text) {
        if (string.IsNullOrEmpty(text)) return;

        using AndroidJavaClass ttsPlugin = new(PLUGIN_NAME);
        ttsPlugin.CallStatic("speak", text);
    }

    private void OnDestroy() {
        using AndroidJavaClass ttsPlugin = new(PLUGIN_NAME);
        ttsPlugin.CallStatic("shutdown");
    }
}
