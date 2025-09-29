using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Cross-platform TTS manager using native platform TTS and web fallback.
/// Auto-initializes in the first scene via RuntimeInitializeOnLoadMethod.
/// </summary>
public sealed class WindowsTTSManager : MonoBehaviour
{
	private static WindowsTTSManager _instance;
	private Dictionary<string, AudioClip> _audioCache = new Dictionary<string, AudioClip>();
	private AudioSource _audioSource;
	private string _currentText = "";
	private bool _isInitialized = false;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void Bootstrap()
	{
		if (_instance == null)
		{
			var go = new GameObject("WindowsTTSManager");
			DontDestroyOnLoad(go);
			_instance = go.AddComponent<WindowsTTSManager>();
		}
	}

	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			Destroy(gameObject);
			return;
		}
		_instance = this;
		Initialize();
	}

	private void Initialize()
	{
		try
		{
			_audioSource = gameObject.AddComponent<AudioSource>();
			_audioSource.playOnAwake = false;
			_audioSource.volume = 1.0f;
			_isInitialized = true;
			Debug.Log("WindowsTTSManager: Successfully initialized");
		}
		catch (Exception ex)
		{
			Debug.LogError($"WindowsTTSManager init failed: {ex.Message}");
		}
	}

	public static void Speak(string text, bool interrupt = true)
	{
		if (_instance == null || !_instance._isInitialized)
		{
			Debug.LogWarning("WindowsTTSManager not initialized.");
			return;
		}

		if (string.IsNullOrEmpty(text))
		{
			Debug.LogWarning("Cannot speak empty text.");
			return;
		}

		if (interrupt)
		{
			Stop();
		}

		_instance._currentText = text;

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
		_instance.StartCoroutine(_instance.SpeakWithWindowsSAPI(text));
#elif UNITY_ANDROID && !UNITY_EDITOR
		_instance.StartCoroutine(_instance.SpeakWithAndroidTTS(text));
#elif UNITY_IOS && !UNITY_EDITOR
		_instance.StartCoroutine(_instance.SpeakWithiOSTTS(text));
#else
		// Fallback for Editor and other platforms
		_instance.StartCoroutine(_instance.SpeakWithWebTTS(text));
#endif
	}

	public static void Stop()
	{
		if (_instance == null || !_instance._isInitialized) return;
		
		_instance.StopAllCoroutines();
		if (_instance._audioSource != null && _instance._audioSource.isPlaying)
		{
			_instance._audioSource.Stop();
		}
		
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
		_instance.StopWindowsSAPI();
#elif UNITY_ANDROID && !UNITY_EDITOR
		_instance.StopAndroidTTS();
#elif UNITY_IOS && !UNITY_EDITOR
		_instance.StopiOSTTS();
#endif
		
		Debug.Log("TTS stopped.");
	}

	public static void SetRate(int rate)
	{
		// Rate setting implementation can be added per platform
		Debug.Log($"TTS Rate set to: {rate}");
	}

	public static void SetVolume(int volume)
	{
		if (_instance != null && _instance._audioSource != null)
		{
			_instance._audioSource.volume = Mathf.Clamp01(volume / 100f);
			Debug.Log($"TTS Volume set to: {volume}");
		}
	}

	public static void SetVoice(string voiceName)
	{
		Debug.Log($"TTS Voice set to: {voiceName}");
	}

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
	private IEnumerator SpeakWithWindowsSAPI(string text)
	{
		try
		{
			using (var speaker = new System.Speech.Synthesis.SpeechSynthesizer())
			{
				speaker.SetOutputToDefaultAudioDevice();
				speaker.SpeakAsync(text);
				Debug.Log($"Speaking with Windows SAPI: {text}");
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning($"Windows SAPI failed: {ex.Message}. Using fallback.");
			yield return StartCoroutine(SpeakWithWebTTS(text));
		}
		yield return null;
	}

	private void StopWindowsSAPI()
	{
		try
		{
			using (var speaker = new System.Speech.Synthesis.SpeechSynthesizer())
			{
				speaker.SpeakAsyncCancelAll();
			}
		}
		catch (Exception ex)
		{
			Debug.LogWarning($"Failed to stop Windows SAPI: {ex.Message}");
		}
	}
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
	private IEnumerator SpeakWithAndroidTTS(string text)
	{
		try
		{
			AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaObject tts = new AndroidJavaObject("android.speech.tts.TextToSpeech", activity, null);
			tts.Call<int>("speak", text, 0, null);
			Debug.Log($"Speaking with Android TTS: {text}");
		}
		catch (Exception ex)
		{
			Debug.LogWarning($"Android TTS failed: {ex.Message}. Using fallback.");
			yield return StartCoroutine(SpeakWithWebTTS(text));
		}
		yield return null;
	}

	private void StopAndroidTTS()
	{
		try
		{
			AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaObject tts = new AndroidJavaObject("android.speech.tts.TextToSpeech", activity, null);
			tts.Call("stop");
		}
		catch (Exception ex)
		{
			Debug.LogWarning($"Failed to stop Android TTS: {ex.Message}");
		}
	}
#endif

#if UNITY_IOS && !UNITY_EDITOR
	[System.Runtime.InteropServices.DllImport("__Internal")]
	private static extern void _SpeakText(string text);

	[System.Runtime.InteropServices.DllImport("__Internal")]
	private static extern void _StopSpeaking();

	private IEnumerator SpeakWithiOSTTS(string text)
	{
		try
		{
			_SpeakText(text);
			Debug.Log($"Speaking with iOS TTS: {text}");
		}
		catch (Exception ex)
		{
			Debug.LogWarning($"iOS TTS failed: {ex.Message}. Using fallback.");
			yield return StartCoroutine(SpeakWithWebTTS(text));
		}
		yield return null;
	}

	private void StopiOSTTS()
	{
		try
		{
			_StopSpeaking();
		}
		catch (Exception ex)
		{
			Debug.LogWarning($"Failed to stop iOS TTS: {ex.Message}");
		}
	}
#endif

	private IEnumerator SpeakWithWebTTS(string text)
	{
		if (_audioCache.ContainsKey(text))
		{
			PlayCachedAudio(text);
			yield break;
		}

		// Using Google Translate TTS as fallback (free but requires internet)
		string url = $"https://translate.google.com/translate_tts?ie=UTF-8&client=tw-ob&q={UnityWebRequest.EscapeURL(text)}&tl=en";
		
		using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG))
		{
			yield return www.SendWebRequest();

			if (www.result == UnityWebRequest.Result.Success)
			{
				AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
				if (clip != null)
				{
					_audioCache[text] = clip;
					_audioSource.clip = clip;
					_audioSource.Play();
					Debug.Log($"Speaking with Web TTS: {text}");
				}
			}
			else
			{
				Debug.LogError($"Web TTS failed: {www.error}");
				// Final fallback - just log the text
				Debug.Log($"TTS (Text Only): {text}");
			}
		}
	}

	private void PlayCachedAudio(string text)
	{
		if (_audioCache.ContainsKey(text))
		{
			_audioSource.clip = _audioCache[text];
			_audioSource.Play();
			Debug.Log($"Speaking cached audio: {text}");
		}
	}

	private void OnDestroy()
	{
		foreach (var clip in _audioCache.Values)
		{
			if (clip != null)
			{
				DestroyImmediate(clip);
			}
		}
		_audioCache.Clear();
	}
}


