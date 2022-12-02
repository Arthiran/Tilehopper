using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AndroidAudioBypass;

public class NativeAudioSystemClick : MonoBehaviour
{
    [SerializeField] NativeAudioManager AudioManagerInstance;
    int buttonClickSoundId = 0;
    string soundFileName = "Button.wav";

    // Start is called before the first frame update
    void Start()
    {
        buttonClickSoundId = AudioManagerInstance.RegisterSoundFile(soundFileName);
    }
    public void PlayButtonClickSound()
    {
        AudioManagerInstance.PlaySound(buttonClickSoundId, 1, 1, 1, 0, 1);
    }
}
