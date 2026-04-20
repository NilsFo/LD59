using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private MusicManager _musicManager;
    public Slider slider;

    private void Awake()
    {
        _musicManager = FindFirstObjectByType<MusicManager>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _musicManager.Stop();
        _musicManager.Play(0);

        slider.value = MusicManager.userDesiredMasterVolume;
    }

    // Update is called once per frame
    void Update()
    {
        MusicManager.userDesiredMasterVolume = slider.value;
    }

    public void StartGame()
    {
        SceneManager.LoadScene("GamePlay");
    }
}