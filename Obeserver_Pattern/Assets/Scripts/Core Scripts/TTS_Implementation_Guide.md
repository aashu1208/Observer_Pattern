# TTS Implementation Guide

## Files Required for TTS System

### 1. WindowsTTSManager.cs
This is the main TTS engine that handles cross-platform text-to-speech functionality.

**Features:**
- Auto-initializes on game start
- Cross-platform support (Windows, Android, iOS, Editor)
- Web fallback using Google Translate TTS
- Audio caching for performance
- Singleton pattern - accessible from anywhere

**Key Methods:**
```csharp
WindowsTTSManager.Speak(string text, bool interrupt = true);
WindowsTTSManager.Stop();
WindowsTTSManager.SetVolume(int volume); // 0-100
WindowsTTSManager.SetRate(int rate);     // -10 to 10
```

### 2. TTSTester.cs  
This is a sample controller script showing how to use the TTS system.

**Features:**
- T key: Speak English message
- H key: Speak Hindi message  
- S key: Stop speaking
- Space: Toggle language modes
- Inspector controls for messages and settings

## Implementation Steps

### Step 1: Copy Scripts
1. Copy `WindowsTTSManager.cs` to any folder in your project
2. Copy `TTSTester.cs` to any folder (optional - for testing)

### Step 2: Setup in Scene
1. Create an empty GameObject
2. Add `TTSTester` component to it
3. Configure messages in Inspector
4. Run and test with T/S keys

### Step 3: Use in Your Scripts
```csharp
// Simple usage in any script
public class MyGameScript : MonoBehaviour 
{
    void Start() 
    {
        // Speak a message
        WindowsTTSManager.Speak("Welcome to my game!");
    }
    
    void OnPlayerDeath()
    {
        // Speak game event
        WindowsTTSManager.Speak("Game Over! Try again.");
    }
    
    void OnPause()
    {
        // Stop any ongoing speech
        WindowsTTSManager.Stop();
    }
}
```

### Step 4: Platform-Specific Settings

#### Android
- No additional setup required
- Uses device's native TTS
- Works offline

#### VR (Quest/PICO)
- Same as Android (since they're Android-based)
- Audio plays through headset
- Consider adding controller input support

#### iOS
- Requires native plugin for full functionality
- Web fallback works in editor/WebGL

#### Windows
- Uses Windows SAPI
- Works offline

## Advanced Usage

### Custom Messages
```csharp
public class DialogueSystem : MonoBehaviour
{
    public void SpeakDialogue(string characterName, string dialogue)
    {
        string fullText = $"{characterName} says: {dialogue}";
        WindowsTTSManager.Speak(fullText, interrupt: true);
    }
}
```

### UI Integration
```csharp
public class UITTSButtons : MonoBehaviour
{
    public Button speakButton;
    public Button stopButton;
    public InputField textInput;
    
    void Start()
    {
        speakButton.onClick.AddListener(() => {
            WindowsTTSManager.Speak(textInput.text);
        });
        
        stopButton.onClick.AddListener(() => {
            WindowsTTSManager.Stop();
        });
    }
}
```

### Game Event Integration
```csharp
public class GameEventTTS : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            WindowsTTSManager.Speak("You entered a new area!");
        }
    }
}
```

## Dependencies
- Unity 2021.3+ (for UnityWebRequest support)
- Internet connection (for web fallback only)
- No external packages required

## Troubleshooting

### Common Issues:
1. **No sound in Editor**: Uses web TTS, check internet connection
2. **Android not working**: Ensure device has TTS engine installed
3. **Volume too low**: Use `WindowsTTSManager.SetVolume(100)`

### Debug Tips:
- Check Console for TTS status messages
- Test with simple English text first
- Verify GameObject with TTSTester is active

## Notes
- System auto-initializes, no manual setup needed
- Thread-safe and performance optimized
- Graceful fallbacks prevent crashes
- Supports multiple languages based on platform TTS capabilities