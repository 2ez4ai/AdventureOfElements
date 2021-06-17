using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITextLocalisation : MonoBehaviour
{
    [SerializeField] string m_key;
    Text m_text;
    Language m_language;

    void Start() {
        m_text = GetComponent<Text>();
        m_text.text = LocalizationManager.m_instance.GetLocalisedString(m_key);
        m_language = LocalizationManager.m_instance.GetLanguage();
    }

    // Update is called once per frame
    void Update()
    {
        if(m_language != LocalizationManager.m_instance.GetLanguage()){
            m_language = LocalizationManager.m_instance.GetLanguage();
            m_text.text = LocalizationManager.m_instance.GetLocalisedString(m_key);
        }
    }
}
