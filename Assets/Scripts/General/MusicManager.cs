using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class MusicManager : MonoBehaviour
{
    public GameObject temporalAudioPlayerPrefab;
    public static float userDesiredMusicVolume = 1f;
    public static float userDesiredSoundVolume = 1f;
    public static float userDesiredMasterVolume = 1f;

    [Header("Custom sound level balance")] [Range(0, 1)]
    public float baselineMusicVolume = 1.0f;

    [Range(0, 1)] public float baselineSoundVolume = 1.0f;
    [Range(0, 1)] public float baselineMasterVolume = 1.0f;

    public float UserDesiredMusicVolumeDB =>
        Mathf.Log10(Mathf.Clamp(userDesiredMusicVolume * baselineMusicVolume, 0.0001f, 1.0f)) * 20;

    public float UserDesiredSoundVolumeDB =>
        Mathf.Log10(Mathf.Clamp(userDesiredSoundVolume * baselineSoundVolume, 0.0001f, 1.0f)) * 20;

    public float UserDesiredMasterVolumeDB =>
        Mathf.Log10(Mathf.Clamp(userDesiredMasterVolume * baselineMasterVolume, 0.0001f, 1.0f)) * 20;

    [Header("Mixer")] public AudioMixer audioMixer;
    public string masterTrackName = "master";
    public string musicTrackName = "music";
    public string soundEffectsTrackName = "sfx";

    [Header("Config")] public float binningVolumeMult = 0.15f;
    public float musicFadeSpeed = 1;
    public bool autoStopSilentMusic = false;

    [Header("Playlist")] public List<AudioSource> initiallyKnownSongs;

    [Header("Events")] public UnityEvent<int> onMusicStopped;

    // ############
    // internal states
    private AudioListener _listener;

    private List<AudioSource> _playList;
    private List<int> _desiredMixingVolumes;
    private List<int> _lastKnownPlayingStates;

    // Audio Binning
    private Dictionary<string, float> _audioJail;

    private void Awake()
    {
        _playList = new List<AudioSource>();
        _desiredMixingVolumes = new List<int>();
        _lastKnownPlayingStates = new List<int>();

        foreach (AudioSource song in initiallyKnownSongs)
        {
            _playList.Add(song);
            song.Stop();
            song.volume = 0;
            _desiredMixingVolumes.Add(0);
        }

        SkipFade();

        _listener = FindFirstObjectByType<AudioListener>();
        _audioJail = new Dictionary<string, float>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (onMusicStopped == null)
        {
            onMusicStopped = new UnityEvent<int>();
        }
    }

    public void Play(int index, bool fromBeginning = false)
    {
        Stop();

        if (fromBeginning)
        {
            _playList[index].time = 0;
        }

        _desiredMixingVolumes[index] = 1;

        if (!_playList[index].isPlaying)
        {
            _playList[index].Play();

            if (_lastKnownPlayingStates.Contains(index))
            {
                _lastKnownPlayingStates.Add(index);
            }

            Debug.Log("MusicManager: Now Playing: " + _playList[index].gameObject.name);
        }
    }

    public void SkipFade()
    {
        for (var i = 0; i < _playList.Count; i++)
        {
            _playList[i].volume = _desiredMixingVolumes[i];
        }
    }

    public void Stop()
    {
        for (var i = 0; i < _playList.Count; i++)
        {
            _desiredMixingVolumes[i] = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // ################################
        // Forcing Mixer Settings
        audioMixer.SetFloat(musicTrackName, UserDesiredMusicVolumeDB);
        audioMixer.SetFloat(soundEffectsTrackName, UserDesiredSoundVolumeDB);
        audioMixer.SetFloat(masterTrackName, UserDesiredMasterVolumeDB);

        // if setup failed, silently fail
        if (_audioJail == null) return;

        // following the audio listener
        transform.position = _listener.transform.position;
        userDesiredSoundVolume = MathF.Min(userDesiredMusicVolume * 1.0f, 1.0f);

        // ################################
        // MUSIC FADE
        for (var i = 0; i < _playList.Count; i++)
        {
            AudioSource audioSource = _playList[i];
            int volumeMixing = _desiredMixingVolumes[i];

            float trueVolume = Mathf.MoveTowards(audioSource.volume,
                volumeMixing,
                Time.deltaTime * musicFadeSpeed);

            if (trueVolume - Time.deltaTime * musicFadeSpeed <= 0 && volumeMixing == 0)
            {
                trueVolume = 0;
            }

            // setting the audio source volume
            audioSource.volume = trueVolume;

            // if the volume is 0, we can stop
            if (trueVolume == 0 && autoStopSilentMusic)
            {
                audioSource.Stop();
                Debug.Log("MuscManager: Auto stopping: " + audioSource.gameObject.name);
            }

            // if the song is not looping and not playing audio, we can tune down the volume
            if (!audioSource.isPlaying && !audioSource.loop)
            {
                _desiredMixingVolumes[i] = 0;
                audioSource.volume = 0;
            }
        }

        // ################################
        // DETECTING IF A SONG HAS FINISHED
        for (var i = 0; i < _playList.Count; i++)
        {
            AudioSource audioSource = _playList[i];

            // checking if we stopped (ether because of natural reasons or because the audio source has run out)
            if (audioSource.isPlaying && _lastKnownPlayingStates.Contains(i))
            {
                _lastKnownPlayingStates.Remove(i);
                Debug.Log("MuscManager: Stopped playing: " + audioSource.gameObject.name);
                OnMusicStoppedPlaying(i);
            }
        }

        // ################################
        // AUDIO BINNING
        var keys = _audioJail.Keys.ToArrayPooled().ToList();
        List<string> releaseKeys = new List<string>();
        if (keys.Count > 0)
        {
            for (var i = 0; i < keys.Count; i++)
            {
                string key = keys[i];
                float timeout = _audioJail[key];
                timeout -= Time.deltaTime;
                _audioJail[key] = timeout;

                if (timeout < 0)
                {
                    releaseKeys.Add(key);
                }
            }
        }

        foreach (var releaseKey in releaseKeys)
        {
            _audioJail.Remove(releaseKey);
        }

        string pg = "";
        foreach (var audioSource in _playList)
        {
            pg += " - " + audioSource.time;
        }
    }

    public void OnMusicStoppedPlaying(int index)
    {
        onMusicStopped.Invoke(index);
    }

    public bool IsPlayingMusic()
    {
        for (var i = 0; i < _playList.Count; i++)
        {
            if (_desiredMixingVolumes[i] > 0)
            {
                return true;
            }
        }

        return false;
    }

    private void LateUpdate()
    {
        userDesiredMusicVolume = Mathf.Clamp(userDesiredMusicVolume, 0f, 1f);
        userDesiredSoundVolume = Mathf.Clamp(userDesiredSoundVolume, 0f, 1f);
        userDesiredMasterVolume = Mathf.Clamp(userDesiredMasterVolume, 0f, 1f);
        baselineMusicVolume = Mathf.Clamp(baselineMusicVolume, 0f, 1f);
        baselineSoundVolume = Mathf.Clamp(baselineSoundVolume, 0f, 1f);
        baselineMasterVolume = Mathf.Clamp(baselineMasterVolume, 0f, 1f);
    }

    public float GetVolumeMusic()
    {
        audioMixer.GetFloat(musicTrackName, out float volume);
        return volume;
    }

    public float GetVolumeSound()
    {
        audioMixer.GetFloat(soundEffectsTrackName, out float volume);
        return volume;
    }

    public float GetMasterSound()
    {
        audioMixer.GetFloat(masterTrackName, out float volume);
        return volume;
    }

    public GameObject CreateAudioClip(AudioClip audioClip,
        Vector3 position,
        float pitchRange = 0.0f,
        float soundVolume = 1.0f,
        bool threeDimensional = false,
        bool respectBinning = false)
    {
        // Registering in the jail
        string clipName = audioClip.name;
        float jailTime = audioClip.length * 0.42f;
        float binningMult = 1.0f;

        if (_audioJail.ContainsKey(clipName))
        {
            _audioJail[clipName] = jailTime;
            if (respectBinning)
            {
                binningMult = binningVolumeMult;
                // return;
            }
        }
        else
        {
            _audioJail.Add(clipName, jailTime);
        }

        // Instancing the sound
        GameObject soundInstance = Instantiate(temporalAudioPlayerPrefab);
        soundInstance.transform.position = position;
        AudioSource source = soundInstance.GetComponent<AudioSource>();
        TimedLife life = soundInstance.GetComponent<TimedLife>();
        life.aliveTime = audioClip.length * 2;

        if (threeDimensional)
        {
            source.spatialBlend = 1;
        }
        else
        {
            source.spatialBlend = 0;
        }

        source.clip = audioClip;
        source.pitch = 1.0f + Random.Range(-pitchRange, pitchRange);
        source.volume = MathF.Min(soundVolume * binningMult, 1.0f);
        source.Play();

        return soundInstance;
    }

    public float AudioBinExternalSoundMult(AudioClip audioClip)
    {
        string clipName = audioClip.name;
        float jailTime = audioClip.length * 0.42f;
        if (_audioJail.ContainsKey(clipName))
        {
            return binningVolumeMult;
        }
        else
        {
            _audioJail.Add(clipName, jailTime);
            return 1.0f;
        }
    }
}