/*********************************************************************
Bachelor of Software Engineering
Media Design School
Auckland
New Zealand
(c) 2022 Media Design School
File Name : SFXManager.cs
Description : used for music and sound, in an array which can be called anywhere
Author : Allister Hamilton
Mail : allister.hamilton @mds.ac.nz
**************************************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SFXManager : MonoBehaviour
{
    public string AudioName;

    public bool SkipToNearEnd;

    [Header("Indexes")]
    public int PoolPosition;

    [Header("Lists")]
    public List<SFXData> SFXList = new List<SFXData>();

    [System.Serializable]
    public class SFXData
    {
        public AudioSource Audio;
        public Transform SpatialTransform;
        public float SFXLoudness = 1;
        public float SFXQuieterOrLouder;

        public int SceneNumber;
        public float TransitionTime;
        public bool isPaused;
    }

    public void Awake()
    {
        name = AudioName + " Holder";

        for (int i = 0; i < 20; i++)
        {
            AudioSource audioCreated = new GameObject().AddComponent<AudioSource>();
            audioCreated.transform.SetParent(transform);
            audioCreated.loop = false;
            audioCreated.playOnAwake = false;

            audioCreated.name = AudioName + ": " + i.ToString("00");
            //audioCreated.rolloffMode = AudioRolloffMode.Logarithmic;

            //audioCreated.SetCustomCurve(AudioSourceCurveType.CustomRolloff, SetupManager.singleton.animationCurve);

            audioCreated.maxDistance = 20;
            audioCreated.dopplerLevel = 0;
            SFXList.Add(new SFXData());
            SFXList[SFXList.Count - 1].Audio = audioCreated;
        }

        RefreshAudioVolumes();
    }

    private void Update()
    {
        if (SkipToNearEnd)
        {
            SkipToNearEnd = false;
            SFXList[PoolPosition].Audio.time = SFXList[PoolPosition].Audio.clip.length - SFXList[PoolPosition].TransitionTime - 1f;
        }

        for (int i = 0; i < SFXList.Count; i++)
        {
            if (SFXList[i].SFXQuieterOrLouder != 0)
            {
                SFXList[i].Audio.volume += (ReturnActualVolume(SFXList[i]) / SFXList[i].SFXQuieterOrLouder) * Time.unscaledDeltaTime;

                if (SFXList[i].SFXQuieterOrLouder < 0)
                {



                    if (SFXList[i].Audio.volume <= 0)
                    {
                        SFXList[i].Audio.volume = 0;

                        PauseAudio(SFXList[i]);
                        SFXList[i].SFXQuieterOrLouder = 0;
                    }
                }
                else
                {
                    if (SFXList[i].Audio.volume >= ReturnActualVolume(SFXList[i]))
                    {
                        SFXList[i].Audio.volume = ReturnActualVolume(SFXList[i]);

                        SFXList[i].SFXQuieterOrLouder = 0;
                    }
                }
            }

            if (SFXList[i].SpatialTransform)
            {
                SFXList[i].Audio.transform.position = SFXList[i].SpatialTransform.position;
            }
        }
    }


    public void RefreshAudioVolumes()
    {
        for (int i = 0; i < SFXList.Count; i++)
        {
            SFXList[i].Audio.volume = PlayerPrefs.GetFloat(AudioName) * SFXList[i].SFXLoudness;
        }
    }

    public float ReturnActualVolume(SFXData data)
    {
        return data.SFXLoudness * PlayerPrefs.GetFloat(AudioName);
    }

    public bool ReturnActiveAudio(SFXData data)
    {
        if (data == null || data.Audio == null)
            return false;

        if (data.SceneNumber == SceneManager.GetActiveScene().buildIndex && data.Audio.clip && (data.Audio.isPlaying || data.isPaused || data.Audio.time > 0f && data.Audio.time < data.Audio.clip.length))
        {

            return true;
        }
        else
        {
            data.Audio.Stop();
            return false;
        }
    }

    public void DynamicVolumeChange(float RequestedQuieterOrLouder = 1, int SpecificPosition = -1)
    {

        for (int i = 0; i < SFXList.Count; i++)
        {
            if (SpecificPosition == -1 || i == SpecificPosition)
            {
                if (ReturnActiveAudio(SFXList[i])) //negative means it cant decrease volume
                {
                    if (RequestedQuieterOrLouder > 0)
                    {
                        PauseAudio(SFXList[i], false);
                    }

                    SFXList[i].SFXQuieterOrLouder = RequestedQuieterOrLouder;
                }
            }
        }
    }

    void PauseAudio(SFXData audioData, bool Pause = true)
    {
        audioData.isPaused = Pause;

        if (Pause)
        {
            audioData.Audio.Pause();

            if (AudioName == "Sound")
            {
                SetupAudioData(audioData, null);
            }
        }
        else
        {
            if (audioData.Audio.time == 0)
            {
                audioData.Audio.time = 0;
                audioData.Audio.Play();
            }
            else
            {
                audioData.Audio.UnPause();
            }
        }
    }

    SFXData SetupAudioData(SFXData audioData, AudioClip RequestedAudio, bool isLooped = false)
    {
        audioData.SceneNumber = SceneManager.GetActiveScene().buildIndex;
        audioData.Audio.Stop();
        audioData.Audio.clip = RequestedAudio;
        audioData.Audio.volume = 0;
        audioData.SFXLoudness = 1;
        audioData.SFXQuieterOrLouder = 0;
        audioData.Audio.loop = isLooped;
        audioData.TransitionTime = 5;
        audioData.isPaused = false;
        return audioData;
    }


    public int PlaySound(AudioClip clip, float RequestedVolume = 1, bool ChangePitch = true, int Piority = 0, bool isLooped = false, Transform SpatialTransform = null)
    {

        if (clip)
        {
            bool ArrayIsFull = true;

            for (int i = 0; i < SFXList.Count; i++)
            {
                if (SpatialTransform && SFXList[i].SpatialTransform == SpatialTransform && SFXList[i].Audio == clip)
                {
                    return i; //stops same sound running
                }
            }

            for (int i = PoolPosition; i < SFXList.Count + PoolPosition; i++)
            {
                int CurrentCycle = (int)GameManager.ReturnThresholds(i + 1, SFXList.Count - 1);

                if (!ReturnActiveAudio(SFXList[CurrentCycle]))
                {
                    PoolPosition = CurrentCycle;
                    ArrayIsFull = false;
                    break;
                }
            }

            if (ArrayIsFull)
            {
                Debug.LogError("Array full!");
                return 0;
            }

            if (ChangePitch)
            {
                SFXList[PoolPosition].Audio.pitch = Random.Range(0.98f, 1.02f);
            }
            else
            {
                SFXList[PoolPosition].Audio.pitch = 1;
            }

            if (SpatialTransform) //default is Vector3.zero
            {
                if (Vector3.Distance(SpatialTransform.position, PlayerController.global.transform.position) > 40)
                {
                    return 0;//too far away
                }
                SFXList[PoolPosition].Audio.spatialBlend = 0.5f;
                SFXList[PoolPosition].Audio.transform.position = SpatialTransform.position; //stops the high pitch sound
            }
            else
            {
                SFXList[PoolPosition].Audio.transform.position = Vector3.zero;
                SFXList[PoolPosition].Audio.spatialBlend = 0;
            }
            //  Debug.Log(clip);
            SetupAudioData(SFXList[PoolPosition], clip, isLooped);
            SFXList[PoolPosition].SFXLoudness = RequestedVolume;
            SFXList[PoolPosition].SpatialTransform = SpatialTransform;

            SFXList[PoolPosition].Audio.volume = ReturnActualVolume(SFXList[PoolPosition]);

            SFXList[PoolPosition].Audio.Play();
        }

        return PoolPosition;
    }

    //When Music is called, it turns off all other music and plays this one, until finished then returning down the queue

    public void PlayMusic(AudioClip RequestedMusic, float volume = 1)
    {
        //transition time is -1 then no songs will be played afterwards for that scene

        if (RequestedMusic)
        {

            DynamicVolumeChange(-1);

            for (int i = 0; i < SFXList.Count; i++)
            {
                if (ReturnActiveAudio(SFXList[i]) && SFXList[i].Audio.clip == RequestedMusic)
                {
                    //just continues playing that music
                    PoolPosition = i;
                    DynamicVolumeChange(0.5f, PoolPosition);

                    return;
                }
            }



            PoolPosition = (int)GameManager.ReturnThresholds(PoolPosition - 1, SFXList.Count - 1, 0);

            SetupAudioData(SFXList[PoolPosition], RequestedMusic, true);
            SFXList[PoolPosition].TransitionTime = 0;
            SFXList[PoolPosition].SFXLoudness = volume;

            SFXList[PoolPosition].Audio.Play();

            DynamicVolumeChange(0.5f, PoolPosition);
        }
    }



    public void StopCurrentMusic()
    {

        DynamicVolumeChange(-0.2f, PoolPosition);

    }

    public void StopSelectedSound(AudioClip audioClip)
    {
        for (int i = 0; i < SFXList.Count; i++)
        {
            if (SFXList[i].Audio.clip == audioClip)
            {
                DynamicVolumeChange(-1, i);
            }
        }
    }
}
