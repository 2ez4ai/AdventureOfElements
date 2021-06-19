using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITextLocalisation : MonoBehaviour
{
    [SerializeField] string m_key;
    [SerializeField] bool m_fontMatters;
    [SerializeField] Font m_chs;
    [SerializeField] Font m_eng;
    Text m_text;
    Language m_language;

    void Start() {
        m_text = GetComponent<Text>();
        m_text.text = LocalizationManager.m_instance.GetLocalisedString(m_key);
        m_language = LocalizationManager.m_instance.GetLanguage();
        if(m_fontMatters){
            if(m_language == Language.Chinese){
                m_text.font = m_chs;
            }
            else{
                m_text.font = m_eng;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(m_language != LocalizationManager.m_instance.GetLanguage()){
            m_language = LocalizationManager.m_instance.GetLanguage();
            m_text.text = LocalizationManager.m_instance.GetLocalisedString(m_key);
            if(m_fontMatters){
                if(m_language == Language.Chinese){
                    m_text.font = m_chs;
                }
                else{
                    m_text.font = m_eng;
                }
            }
        }
    }
}
