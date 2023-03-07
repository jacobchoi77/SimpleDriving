using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour{
    [SerializeField]
    private TMP_Text highScoreText;

    [SerializeField]
    private TMP_Text energyText;

    [SerializeField]
    private Button playButton;

    [SerializeField]
    private int maxEnergy = 5;

    [SerializeField]
    private int energyRechargeDuration = 1;

    [SerializeField]
    private AndroidNotificationHandler androidNotificationHandler;

    [SerializeField]
    private IOSNotificationHandler iosNotificationHandler;

    private int energy;

    private const string EnergyKey = "Energy";
    private const string EnergyReadKey = "EnergyReady";

    private void Start(){
        OnApplicationFocus(true);
    }

    private void OnApplicationFocus(bool hasFocus){
        if (!hasFocus){
            return;
        }

        CancelInvoke();
        highScoreText.text = $"High Score: {PlayerPrefs.GetInt(ScoreSystem.HighScoreKey, 0).ToString()}";
        energy = PlayerPrefs.GetInt(EnergyKey, maxEnergy);
        if (energy == 0){
            var energyReadyString = PlayerPrefs.GetString(EnergyReadKey, string.Empty);
            if (energyReadyString == string.Empty){
                return;
            }

            var energyReady = DateTime.Parse(energyReadyString);
            if (DateTime.Now > energyReady){
                energy = maxEnergy;
                PlayerPrefs.SetInt(EnergyKey, energy);
            }
            else{
                playButton.interactable = false;
                Invoke(nameof(EnergyRecharged), (energyReady - DateTime.Now).Seconds);
            }
        }

        energyText.text = $"Play ({energy})";
    }

    private void EnergyRecharged(){
        playButton.interactable = true;
        energy = maxEnergy;
        PlayerPrefs.SetInt(EnergyKey, energy);
        energyText.text = $"Play ({energy})";
    }

    public void Play(){
        if (energy < 1){
            return;
        }

        energy--;
        PlayerPrefs.SetInt(EnergyKey, energy);
        if (energy == 0){
            var energyReady = DateTime.Now.AddMinutes(energyRechargeDuration);
            PlayerPrefs.SetString(EnergyReadKey, energyReady.ToString());
#if UNITY_ANDROID
            androidNotificationHandler.ScheduleNotification(energyReady);
#elif UNITY_IOS
            iosNotificationHandler.ScheduleNotification(energyRechargeDuration);
#endif
        }

        SceneManager.LoadScene("Scene_Game");
    }
}