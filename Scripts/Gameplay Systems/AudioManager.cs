using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public enum SoundType { SFX, Music };
    public enum AudioInstance { MenuMusic = 0, LevelMusic = 1, HitEffect = 2, Laser = 3, Explosion = 4, PowerUp = 5, Button = 6 };

    public static AudioManager singletonInstance;

    public Audio[] soundArray;

    private float sfxVolume, musicVolume;

    private void Awake()
    {
        //Make this a singleton
        //Important to destroy duplicate as this gameobject plays audio between scenes
        if (singletonInstance == null)
        {
            singletonInstance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (singletonInstance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //Load volume settings from savedata
        sfxVolume = GameData.instance.saveData.volumeSFX;
        musicVolume = GameData.instance.saveData.volumeMusic;

        //Create audiosource components for each clip
        foreach (Audio currentAudio in soundArray)
        {
            currentAudio.source = gameObject.AddComponent<AudioSource>();
            currentAudio.source.clip = currentAudio.clip;
            currentAudio.source.playOnAwake = false;

            //Apply volume based on sound type
            switch (currentAudio.type)
            {
                case (SoundType.SFX):
                    currentAudio.source.volume = sfxVolume;
                    currentAudio.source.loop = false;
                    break;
                case (SoundType.Music):
                    currentAudio.source.volume = musicVolume;
                    currentAudio.source.loop = true;
                    break;
            }
        }
    }

    public void PlayAudio(AudioInstance audioInstance, bool randomPitch)
    {
        //Pitch sound randomly if needs be
        if (randomPitch)
        {
            soundArray[((int)audioInstance)].source.pitch = Random.Range(1f, 2f);
        }

        soundArray[((int)audioInstance)].source.Play();
    }

    public void StopMusic()
    {
        //Stops all instances of music playing
        foreach (Audio currentAudio in soundArray)
        {
            if (currentAudio.type == AudioManager.SoundType.Music)
            {
                currentAudio.source.Stop();
            }
        }
    }

    public void UpdateVolumes()
    {
        //Loads volume settings and reapplies them
        sfxVolume = GameData.instance.saveData.volumeSFX;
        musicVolume = GameData.instance.saveData.volumeMusic;

        foreach (Audio currentAudio in soundArray)
        {
            switch (currentAudio.type)
            {
                case (SoundType.SFX):
                    currentAudio.source.volume = sfxVolume;
                    break;
                case (SoundType.Music):
                    currentAudio.source.volume = musicVolume;
                    break;
            }
        }
    }
}

[System.Serializable]
public class Audio
{
    public string name;
    public AudioManager.SoundType type;
    public AudioClip clip;
    public AudioSource source;
}