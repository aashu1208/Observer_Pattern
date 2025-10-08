using UnityEngine;

public class TTSTester : MonoBehaviour
{
	[SerializeField] private string message = "Hello! This is a text to speech test message.";
	[SerializeField] private string hindiMessage = "Namaste! Yeh text to speech ka test message hai.";
	[SerializeField] private KeyCode speakKey = KeyCode.T;
	[SerializeField] private KeyCode stopKey = KeyCode.S;
	[SerializeField] private KeyCode hindiKey = KeyCode.H;
	[SerializeField] [Range(-10,10)] private int rate = 0;
	[SerializeField] [Range(0,100)] private int volume = 100;
	[SerializeField] private bool useHindiMessage = false;

	void Start()
	{
		WindowsTTSManager.SetRate(rate);
		WindowsTTSManager.SetVolume(volume);
		
		// Display instructions in console
		Debug.Log("=== TTS Controls ===");
		Debug.Log("Press T to speak English message");
		Debug.Log("Press H to speak Hindi message");
		Debug.Log("Press S to stop speaking");
		Debug.Log("================");
	}

	void Update()
	{
		// T key - Speak current message
		if (Input.GetKeyDown(speakKey))
		{
			string textToSpeak = useHindiMessage ? hindiMessage : message;
			Debug.Log($"Speaking: {textToSpeak}");
			WindowsTTSManager.Speak(textToSpeak, interrupt: true);
		}
		
		// H key - Speak Hindi message
		if (Input.GetKeyDown(hindiKey))
		{
			Debug.Log($"Speaking Hindi: {hindiMessage}");
			WindowsTTSManager.Speak(hindiMessage, interrupt: true);
		}
		
		// S key - Stop speaking
		if (Input.GetKeyDown(stopKey))
		{
			Debug.Log("Stopping TTS...");
			WindowsTTSManager.Stop();
		}
		
		// Space key - Toggle between English and Hindi
		if (Input.GetKeyDown(KeyCode.Space))
		{
			useHindiMessage = !useHindiMessage;
			string lang = useHindiMessage ? "Hindi" : "English";
			Debug.Log($"Switched to {lang} mode. Press T to test.");
		}
	}

	// Public methods for UI or other scripts to call
	public void SpeakEnglish()
	{
		WindowsTTSManager.Speak(message, interrupt: true);
	}

	public void SpeakHindi()
	{
		WindowsTTSManager.Speak(hindiMessage, interrupt: true);
	}

	public void StopSpeaking()
	{
		WindowsTTSManager.Stop();
	}

	public void SetMessage(string newMessage)
	{
		message = newMessage;
	}

	public void SetHindiMessage(string newHindiMessage)
	{
		hindiMessage = newHindiMessage;
	}

	// Update volume in real-time
	void OnValidate()
	{
		if (Application.isPlaying)
		{
			WindowsTTSManager.SetRate(rate);
			WindowsTTSManager.SetVolume(volume);
		}
	}
}