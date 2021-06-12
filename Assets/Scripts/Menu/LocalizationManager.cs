using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;

public enum Language
{
    English,
    Chinese
}

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager m_instance = null;

    [SerializeField] Language m_language = Language.Chinese;

    Dictionary<string, TextAsset> m_localizationFiles = new Dictionary<string, TextAsset>();
    Dictionary<string, string> m_localizationText = new Dictionary<string, string>();

    // Use this for initialization
    void Awake()
    {
        DontDestroyOnLoad(this);
        SetupLocalizationFiles();
        SetupLocalizationXMLSingleton();
        SetupLocalization(m_language);
    }

    void SetupLocalizationXMLSingleton()
    {
        if (m_instance == null)
        {
            m_instance = this;
        }
        else if (m_instance != this)
        {
            Destroy(gameObject);
        }
        //DontDestroyOnLoad(gameObject);
    }

    public Language GetLanguage(){
        return m_language;
    }

    public void SetupLocalizationFiles()
    {
        // Search for each Language files defined in the Language Enum
        foreach (Language language in Language.GetValues(typeof(Language)))
        {
            string textAssetPath = "Localization/" + language.ToString();
            TextAsset textAsset = (TextAsset)Resources.Load(textAssetPath);
            if (textAsset)
            {
                m_localizationFiles[textAsset.name] = textAsset;
                Debug.Log("Text Asset: " + textAsset.name);
            }
            else
            {
                Debug.LogError("TextAssetPath not found: " + textAssetPath);
            }
        }
    }

    public void SetupLocalization(Language l)
    {
        m_language = l;

        TextAsset textAsset;
        // Search for the specified language file
        if (m_localizationFiles.ContainsKey(m_language.ToString()))
        {
            Debug.Log("Selected language: " + m_language);
            textAsset = m_localizationFiles[m_language.ToString()];
        }
        // If we can't find the specific language default to English
        else
        {
            Debug.LogError("Couldn't find localization file for: " + m_language);
            textAsset = m_localizationFiles[Language.English.ToString()];
        }

        // Load XML document
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(textAsset.text);

        // Get all elements called "Entry"
        XmlNodeList entryList = xmlDocument.GetElementsByTagName("Entry");

        // Iterate over each Entry element and store them in the Dictionary
        foreach (XmlNode entry in entryList)
        {
            if (!m_localizationText.ContainsKey(entry.FirstChild.InnerText))
            {
                Debug.Log("Added Key: " + entry.FirstChild.InnerText + " with value: " + entry.LastChild.InnerText);
                m_localizationText.Add(entry.FirstChild.InnerText, entry.LastChild.InnerText);
            }
            else
            {
                m_localizationText[entry.FirstChild.InnerText] = entry.LastChild.InnerText;
                // Debug.LogError("Duplicate Localization key detected: " + entry.FirstChild.InnerText);
            }
        }
    }

    public string GetLocalisedString(string key)
    {
        string localisedString = "";
        if (!m_localizationText.TryGetValue(key, out localisedString))
        {
            localisedString = "LOC KEY: " + key;
        }

        return localisedString;
    }
}
