using TMPro;
using Unity.XR.XREAL.Samples;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DemoApplication : MonoBehaviour
{
    [SerializeField] private InputActionReference AppButtonAction;
    [SerializeField] private RawImage DebugCameraDisplay;
    [SerializeField] private TextMeshProUGUI DebugTextDisplay;
    [SerializeField] private TextMeshProUGUI DebugVoiceDisplay;
    [SerializeField] private ResourceLocator ResourceLocator;
    [SerializeField] private FaceDetectionTester FaceDetector;

    [SerializeField] private CaptureExample CaptureExample;

    private CameraManager Camera => ResourceLocator.Camera;
    private TextRecognitionManager TextRecognizer => ResourceLocator.TextRecognizer;
    private TranslationManager Translator => ResourceLocator.Translator;
    private SpeechToTextManager SpeechToText => ResourceLocator.SpeechToText;
    private TextToSpeechManager TextToSpeech => ResourceLocator.TextToSpeech;

    private void OnEnable() {
        if (AppButtonAction != null) {
            AppButtonAction.action.Enable();
            AppButtonAction.action.performed += OnAppButtonPressed;
        }
    }

    private void OnDisable() {
        if (AppButtonAction != null) {
            AppButtonAction.action.performed -= OnAppButtonPressed;
            AppButtonAction.action.Disable();
        }
    }

    void Start() {
        TextRecognizer.RegisterCallback(OnTextRecognitionResult);
        Translator.RegisterCallback(OnTranslationResult);
        SpeechToText.RegisterCallback(OnSpeechToTextResult);
        Camera.StartCapture();
    }

    void Update() {
        Texture2D[] yuvTextures = Camera.GetYUVTextures();
        if (yuvTextures[0] != null) {
            DebugCameraDisplay.texture = yuvTextures[0];
            DebugCameraDisplay.material.SetTexture("_UTex", yuvTextures[1]);
            DebugCameraDisplay.material.SetTexture("_VTex", yuvTextures[2]);
        }
    }

    private void OnAppButtonPressed(InputAction.CallbackContext context) {
        if (context.ReadValueAsButton()) {
            SpeechToText.ToggleSpeechRegistration();
        }
    }

    private void OnTextRecognitionResult(TextRecognitionStatus status, string message) {
        switch (status) {
            case TextRecognitionStatus.Error:
                DebugTextDisplay.text = $"[ERROR] Text recognition failed: {message}";
                break;
            case TextRecognitionStatus.Success:
                DebugTextDisplay.text = $"[SUCCESS] {message}";
                Translator.StartTranslation(message);
                break;
        }
    }

    private void OnTranslationResult(TranslationStatus status, string message) {
        switch (status) {
            case TranslationStatus.Error:
                DebugTextDisplay.text = $"[ERROR] Translation failed: {message}";
                break;
            case TranslationStatus.Success:
                DebugTextDisplay.text = $"[SUCCESS] {message}";
                TextToSpeech.Speak($"It says, \"{message}\"");
                break;
        }
    }

    private void OnSpeechToTextResult(SpeechToTextStatus status, string message) {
        switch (status) {
            case SpeechToTextStatus.Error:
                DebugVoiceDisplay.text = $"[ERROR] Speech-to-text failed: error code {message}";
                break;
            case SpeechToTextStatus.Partial:
                DebugVoiceDisplay.text = $"[PARTIAL] {message}";
                break;
            case SpeechToTextStatus.Final:
                DebugVoiceDisplay.text = $"[SUCCESS] {message}";
                ExecuteVoiceCommand(message);
                break;
        }
    }

    private void ExecuteVoiceCommand(string text) {
        text = text.ToLower();

        if (text.Contains("translate")) {
            Texture2D frame = Camera.RenderFrame();
            TextRecognizer.StartTextRecognition(frame);
            // TODO Destroy frame
        } else if (text.Contains("start recording")) {
            TextToSpeech.Speak("Starting video recording");
            CaptureExample.RecordVideo();
            //Camera.StartCapture();
        } else if (text.Contains("stop recording")) {
            TextToSpeech.Speak("Stopping video recording");
            CaptureExample.RecordVideo();
            //Camera.StopCapture();
        } else if (text.Contains("who") || text.Contains("name")) {
            Texture2D frame = Camera.RenderFrame();
            string name = FaceDetector.TryIdentifyFaceFromPhoto(frame);
            TextToSpeech.Speak(name);
            // TODO Destroy frame
        }
    }
}
