using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class UImanager : MonoBehaviour
{
    //menu
    public Button btn1,btn2,btn3,btn4;
    public GameObject optionsekraný, creditsekraný, menuekraný;
    //OPTÝON
    public AudioMixer audioMixer;
    public TMP_Dropdown resolutionDropdown;
    public Button btn5;
    //CREDÝT
    public Button btn6;
    //
    Resolution[] resolutions;

    void Start()
    {
        optionsekraný.SetActive(false);
        creditsekraný.SetActive(false);


        //OPTÝONS**************************
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height) 
            {
                currentResolutionIndex = i;
            }
        } 
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        //**************************************
       
    }

    public void don() {  }
    public void s() { }


    public void crdon() { creditsekraný.SetActive(false); }
    public void opdon(){
        menuekraný.SetActive(true);
        optionsekraný.SetActive(false);}
    public void exit() { Application.Quit(); }
    public void cutscenegec()  {  SceneManager.LoadScene (1);}
    public void opgit() {
        menuekraný.SetActive(false);
        optionsekraný.SetActive(true);
    }
    public void crgit(){ creditsekraný.SetActive(true); }


    //OPTÝONS*************************************
    public void SetResulation(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
    //*****************************************
}
