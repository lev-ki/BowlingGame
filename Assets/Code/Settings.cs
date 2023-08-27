using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Code.DataContainers;

namespace Code
{
    public class Settings : MonoBehaviour
    {
        [Serializable]
        public struct ResolutionStruct
        {
            public int width;
            public int height;
        }

        [Header("Graphics")]
        [SerializeField] private List<ResolutionStruct> resolutions;

        [Header("Sound")]
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private float minVolume;
        [SerializeField] private float maxVolume;

        private int currentResolutionIndex = -1;
        private bool fullScreen;

        private void Start()
        {
            fullScreen = Screen.fullScreen;
            VerifyResolution();
        }

        private void VerifyResolution()
        {
            ResolutionStruct currentRes = new() { 
                width = Screen.currentResolution.width,
                height = Screen.currentResolution.height 
            };
            currentResolutionIndex = resolutions.FindIndex((res) => res.width == currentRes.width && res.height == currentRes.height);
            if (currentResolutionIndex < 0)
            {
                resolutions.Add(currentRes);
                currentResolutionIndex = resolutions.Count - 1;
            }
            RefreshResolutionText();
        }

        private void RefreshResolutionText()
        {
            UIContainer.Instance.resolutionText.text = $"{resolutions[currentResolutionIndex].width}x{resolutions[currentResolutionIndex].height}";
        }

        private void RefreshFullscreenText()
        {
            UIContainer.Instance.fullscreenText.text = fullScreen ? "No" : "Yes";
        }

        public void ToggleFullscreen()
        {
            fullScreen = !fullScreen;
            Screen.SetResolution(resolutions[currentResolutionIndex].width, resolutions[currentResolutionIndex].height, fullScreen);
            VerifyResolution();
            RefreshFullscreenText();
        }

        public void SwitchResolution(bool positive)
        {
            currentResolutionIndex += positive ? 1 : -1;
            if (currentResolutionIndex < 0)
            {
                currentResolutionIndex = resolutions.Count - 1;
            }
            else if (currentResolutionIndex >= resolutions.Count)
            {
                currentResolutionIndex = 0;
            }
            Screen.SetResolution(resolutions[currentResolutionIndex].width, resolutions[currentResolutionIndex].height, fullScreen);
            RefreshResolutionText();
        }

        public void SetMasterVolume(float volume)
        {
            audioMixer.SetFloat("Master", Mathf.Lerp(minVolume, maxVolume, volume));
        }

        public void SetMusicVolume(float volume)
        {
            audioMixer.SetFloat("Music", Mathf.Lerp(minVolume, maxVolume, volume));
        }

        public void SetSFXVolume(float volume)
        {
            audioMixer.SetFloat("SFX", Mathf.Lerp(minVolume, maxVolume, volume));
        }
    }
}