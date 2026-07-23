using UnityEngine;

public class ResourceLocator : MonoBehaviour
{
    public CameraManager Camera => GetComponent<CameraManager>();
    public TextRecognitionManager TextRecognizer => GetComponent<TextRecognitionManager>();
    public TranslationManager Translator => GetComponent<TranslationManager>();
    public SpeechToTextManager SpeechToText => GetComponent<SpeechToTextManager>();
    public TextToSpeechManager TextToSpeech => GetComponent<TextToSpeechManager>();
}
