using TMPro;
using Unity.XR.CoreUtils;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{

    public static UIManager Instance;

    private GameObject GameplaySettingPanel;
    private GameObject SoundSettingPanel;
    private GameObject MainSettingPanel;

    public bool ActiveSubtitle = true;
    public bool ActiveCameraShake = true;
    public bool ActiveSpeedTimer = false;

    public int MasterVolume = 100;
    public int VoiceVolume = 100;
    public int MusicVolume = 100;
    public int SFXVolume = 100;
    public int UIVolume = 100;

    private void Awake()
    {
        if (Instance == null)
        { 
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
      
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        MainSettingPanel = GameObject.Find("MainSettingPanel");
        SoundSettingPanel = GameObject.Find("SoundSettingPanel");
        GameplaySettingPanel = GameObject.Find("GamePlaySettingPanel");
        GameplaySettingPanel.SetActive(false);
        SoundSettingPanel.SetActive(false);
    }

    public void ClosePanel(GameObject targetPanel)
    {
        targetPanel.SetActive(false);
    }
    public void ActiveMainPanel()
    {
        MainSettingPanel.SetActive(true);
    }
    public void ActiveGameSettingPanel()
    {
        GameplaySettingPanel.SetActive(true);
    }
    public void ActiveSoundPanel()
    {
        SoundSettingPanel.SetActive(true);
    }
    public void SetActiveSubtitle()
    {
        if (GameplaySettingPanel.transform.GetChild(1).GetComponent<TMP_Dropdown>().value == 0)
            ActiveSubtitle = true;
        else
            ActiveSubtitle = false;
    }
    public void SetActiveCameraShake()
    {
        if (GameplaySettingPanel.transform.GetChild(2).GetComponent<TMP_Dropdown>().value == 0)
            ActiveCameraShake = true;
        else
            ActiveCameraShake = false;
    }
    public void SetActiveSpeedTimer()
    {
        if (GameplaySettingPanel.transform.GetChild(3).GetComponent<TMP_Dropdown>().value == 0)
            ActiveSpeedTimer = false;
        else
            ActiveSpeedTimer = true;
    }
    public void SetVolume_Master()
    {
        Transform Slider = SoundSettingPanel.transform.GetChild(1);
        MasterVolume = (int)Slider.GetComponent<Slider>().value;

        Transform scrollArea = Slider.Find("Handle Slide Area");
        Transform handle = scrollArea.Find("Handle");
        Transform textTMP = handle.Find("Text (TMP)");
        TextMeshProUGUI textComponent = textTMP.GetComponent<TextMeshProUGUI>();
        textComponent.text = MasterVolume.ToString();
    }
    public void SetVolume_Voice()
    {
        Transform Slider = SoundSettingPanel.transform.GetChild(2);
        VoiceVolume = (int)Slider.GetComponent<Slider>().value;

        Transform scrollArea = Slider.Find("Handle Slide Area");
        Transform handle = scrollArea.Find("Handle");
        Transform textTMP = handle.Find("Text (TMP)");
        TextMeshProUGUI textComponent = textTMP.GetComponent<TextMeshProUGUI>();
        textComponent.text = VoiceVolume.ToString();
    }
    public void SetVolume_Music()
    {
        Transform Slider = SoundSettingPanel.transform.GetChild(3);
        MusicVolume = (int)Slider.GetComponent<Slider>().value;

        Transform scrollArea = Slider.Find("Handle Slide Area");
        Transform handle = scrollArea.Find("Handle");
        Transform textTMP = handle.Find("Text (TMP)");
        TextMeshProUGUI textComponent = textTMP.GetComponent<TextMeshProUGUI>();
        textComponent.text = MusicVolume.ToString();
    }

    public void SetVolume_SFX()
    {
        Transform Slider = SoundSettingPanel.transform.GetChild(4);
        SFXVolume = (int)Slider.GetComponent<Slider>().value;

        Transform scrollArea = Slider.Find("Handle Slide Area");
        Transform handle = scrollArea.Find("Handle");
        Transform textTMP = handle.Find("Text (TMP)");
        TextMeshProUGUI textComponent = textTMP.GetComponent<TextMeshProUGUI>();
        textComponent.text = SFXVolume.ToString();
    }

    public void SetVolume_UI()
    {
        Transform Slider = SoundSettingPanel.transform.GetChild(5);
        UIVolume = (int)Slider.GetComponent<Slider>().value;

        Transform scrollArea = Slider.Find("Handle Slide Area");
        Transform handle = scrollArea.Find("Handle");
        Transform textTMP = handle.Find("Text (TMP)");
        TextMeshProUGUI textComponent = textTMP.GetComponent<TextMeshProUGUI>();
        textComponent.text = UIVolume.ToString();
    }
}
