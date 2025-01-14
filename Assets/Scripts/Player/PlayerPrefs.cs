using UnityEngine;

[CreateAssetMenu(fileName = "PlayerPrefs")]
public class PlayerPrefs : ScriptableObject
{
    public bool experiencedTutorial = false;
    public bool detailedDescription = false;
    public bool developerMode = false;
    public bool offImageGen = false;

    public float masterVolume = 1.0f;
    public float bgmVolume = 1.0f;
    public float sfxVolume = 1.0f;

    public void SetExperienceTutrial(bool experiencedTutorial)
    {
        this.experiencedTutorial = experiencedTutorial;
        AudioManager.Instance.PlayOneShot(Sound.SoundName.SettingsClick);
    }

    public void SetDetailedDescrpition(bool detailed)
    {
        detailedDescription = detailed;
        AudioManager.Instance.PlayOneShot(Sound.SoundName.SettingsClick);
    }
    public void SetDeveloperMode(bool devMode)
    {
        developerMode = devMode;
        AudioManager.Instance.PlayOneShot(Sound.SoundName.SettingsClick);
    }
    public void SetImageGen(bool active)
    {
        offImageGen = active;
        AudioManager.Instance.PlayOneShot(Sound.SoundName.SettingsClick);
    }

    public void ResetPrefs()
    {

    }

    public void ResetVolume()
    {
        masterVolume = 1.0f;
        bgmVolume = 1.0f;
        sfxVolume = 1.0f;
    }
}