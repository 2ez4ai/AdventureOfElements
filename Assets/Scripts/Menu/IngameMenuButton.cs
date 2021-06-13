using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IngameMenuButton : MonoBehaviour
{
    [SerializeField] GameObject m_backBtn;
    [SerializeField] GameObject m_quitBtn;
    [SerializeField] IngameMenu m_menu;

    Text m_backText;
    Text m_quitText;

    Language m_language;

    // Start is called before the first frame update
    void Start()
    {
        m_backText = m_backBtn.GetComponent<Text>();
        m_quitText = m_quitBtn.GetComponent<Text>();
        if(LocalizationManager.m_instance != null){
            m_language = LocalizationManager.m_instance.GetLanguage();
            m_backText.text = LocalizationManager.m_instance.GetLocalisedString("Back");
            m_quitText.text = LocalizationManager.m_instance.GetLocalisedString("Quit");
        }
        else{
            m_language = Language.English;
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateText();
    }

    void UpdateText(){
        Language temp = LocalizationManager.m_instance.GetLanguage();
        if(temp != m_language){
            m_backText.text = LocalizationManager.m_instance.GetLocalisedString("Back");
            m_quitText.text = LocalizationManager.m_instance.GetLocalisedString("Quit");
        }
        m_language = temp;
    }

    public void OnBackClicked(){
        m_menu.CloseMenu();
    }

    public void OnQuitClicked(){
        SceneManager.LoadScene("MainMenu");
    }
}
