using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IngameMenuButton : MonoBehaviour
{
    [SerializeField] GameObject m_backObj;
    [SerializeField] GameObject m_quitObj;
    [SerializeField] IngameMenu m_menu;

    Text m_backText;
    Text m_quitText;
    Button m_backBtn;
    Button m_quitBtn;

    Language m_language;

    int m_currentIndex = 0;
    const float k_maxSelectionCooldown = 0.25f;
    float m_selectionCooldown;

    // Start is called before the first frame update
    void Start()
    {
        m_backText = m_backObj.GetComponent<Text>();
        m_quitText = m_quitObj.GetComponent<Text>();
        m_backBtn = m_backObj.GetComponent<Button>();
        m_quitBtn = m_quitObj.GetComponent<Button>();
        if(LocalizationManager.m_instance != null){
            m_language = LocalizationManager.m_instance.GetLanguage();
            m_backText.text = LocalizationManager.m_instance.GetLocalisedString("Back");
            LocalizationManager.m_instance.SetLocalisedFont(m_backText);
            m_quitText.text = LocalizationManager.m_instance.GetLocalisedString("Quit");
            LocalizationManager.m_instance.SetLocalisedFont(m_quitText);
        }
        else{
            m_language = Language.English;
        }
        m_selectionCooldown = k_maxSelectionCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateText();
        UpdateSelection();
    }

    void UpdateText(){
        Language temp = LocalizationManager.m_instance.GetLanguage();
        if(temp != m_language){
            m_backText.text = LocalizationManager.m_instance.GetLocalisedString("Back");
            LocalizationManager.m_instance.SetLocalisedFont(m_backText);
            m_quitText.text = LocalizationManager.m_instance.GetLocalisedString("Quit");
            LocalizationManager.m_instance.SetLocalisedFont(m_quitText);
        }
        m_language = temp;
    }

    public void OnBackClicked(){
        m_menu.CloseMenu();
        SoundManager.m_instance.PlayBackSound();
    }

    public void OnQuitClicked(){
        SceneManager.LoadScene("MainMenu");
        SoundManager.m_instance.PlayQuitSound();
    }

    public void UpdateSelection(int mouseParam = -1){
        // mouseParam: 0 -> back; 1 ->quit
        if(mouseParam != -1){
            SoundManager.m_instance.PlaySelectSound();
            m_currentIndex = mouseParam;
        }
        if(m_currentIndex == 0){
            m_backBtn.Select();
        }
        else{
            m_quitBtn.Select();
        }
        if(m_selectionCooldown < 0.0f){
            bool makeSelection = false;
            float horizontalInput = Input.GetAxis("Horizontal");
            if(horizontalInput > 0.1f){
                m_currentIndex = 1;
                makeSelection = true;
            }
            if(horizontalInput < -0.1f){
                m_currentIndex = 0;
                makeSelection = true;
            }

            if(makeSelection){
                SoundManager.m_instance.PlaySelectSound();
                m_selectionCooldown = k_maxSelectionCooldown;
            }
        }
        else{
            m_selectionCooldown -= Time.deltaTime;
        }
    }
}
