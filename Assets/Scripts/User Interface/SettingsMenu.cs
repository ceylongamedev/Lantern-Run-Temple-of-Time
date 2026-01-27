using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{

    [Header("Video Settings")]
    [SerializeField] TMP_Dropdown resolutionDropdown;
    [SerializeField] TMP_Dropdown graphicsDropdown;
    Resolution[] resolutions;

    [Header("Audio Settings")]
    [SerializeField] AudioMixer audioMixer;
    [SerializeField] AudioMixerGroup masterGroup;
    [SerializeField] AudioMixerGroup musicGroup;
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider musicSlider;

    void Start()
    {
        LoadResolutions();
        LoadGraphicsSettings();
        LoadAudio();

        if (resolutionDropdown != null)
            resolutionDropdown.onValueChanged.AddListener(SetResolution);

        if ((graphicsDropdown != null))
        {
            graphicsDropdown.onValueChanged.AddListener(SetGraphics);
        }
       
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
    }

    void LoadResolutions()
    {
#if UNITY_WEBGL
        if(resolutionDropdown != null)
        {
        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(new System.Collections.Generic.List<string>() { "Browser Controlled" });
        resolutionDropdown.value = 0;
        resolutionDropdown.interactable = false;
        }
       
#else
        if (resolutionDropdown == null) return;
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        int currentIndex = 0;
        var options = new System.Collections.Generic.List<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            string res = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(res);
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
                currentIndex = i;
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = PlayerPrefs.GetInt("resolutionIndex", currentIndex);
        resolutionDropdown.RefreshShownValue();
#endif
    }

    void LoadGraphicsSettings()
    {
        if(graphicsDropdown == null) return;
        graphicsDropdown.ClearOptions();
        graphicsDropdown.AddOptions(new System.Collections.Generic.List<string>() { "Low", "Medium", "High", "Ultra" });

        int level = PlayerPrefs.GetInt("graphicsQuality", QualitySettings.GetQualityLevel());
        graphicsDropdown.value = level;

        QualitySettings.SetQualityLevel(level);
    }

    void LoadAudio()
    {
        float master = PlayerPrefs.GetFloat("masterVolume", 1f);
        float music = PlayerPrefs.GetFloat("musicVolume", 1f);

        masterSlider.value = master;
        musicSlider.value = music;

        SetMasterVolume(master);
        SetMusicVolume(music);
    }

    public void SetMasterVolume(float value)
    {
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        dB = Mathf.Clamp(dB, -80f, 0f);

        audioMixer.SetFloat("Master", dB);
        audioMixer.SetFloat(masterGroup.name, dB);

        PlayerPrefs.SetFloat("masterVolume", value);
    }

    public void SetMusicVolume(float value)
    {
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        dB = Mathf.Clamp(dB, -80f, 0f);

        audioMixer.SetFloat("Music", dB);
        audioMixer.SetFloat(musicGroup.name, dB);

        PlayerPrefs.SetFloat("musicVolume", value);
    }

    public void SetResolution(int index)
    {
#if UNITY_WEBGL
        Debug.Log("WebGL does not support changing screen resolutions.");
        return;
#else
        Resolution res = resolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);

        PlayerPrefs.SetInt("resolutionIndex", index);
#endif
    }

    public void SetGraphics(int index)
    {
        QualitySettings.SetQualityLevel(index);
        PlayerPrefs.SetInt("graphicsQuality", index);
    }



}
