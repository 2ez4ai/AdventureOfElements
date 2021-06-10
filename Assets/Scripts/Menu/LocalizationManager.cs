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
    public static LocalizationManager m_Instance = null;

    [SerializeField]
    Language m_Language = Language.English;

    Dictionary<string, TextAsset> m_LocalizationFiles = new Dictionary<string, TextAsset>();
    Dictionary<string, string> m_LocalizationText = new Dictionary<string, string>();

    // Use this for initialization
    void Awake()
    {
        SetupLocalizationFiles();
        SetupLocalizationXMLSingleton();
        SetupLocalization();
    }

    void SetupLocalizationXMLSingleton()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
        }
        else if (m_Instance != this)
        {
            Destroy(gameObject);
        }
        //DontDestroyOnLoad(gameObject);
    }

    public void SetupLocalizationFiles()
    {
        // Search for each Language defined in the Language Enum
        foreach (Language language in Language.GetValues(typeof(Language)))
        {
            string textAssetPath = "Localization/" + language.ToString();
            TextAsset textAsset = (TextAsset)Resources.Load(textAssetPath);
            if (textAsset)
            {
                m_LocalizationFiles[textAsset.name] = textAsset;
                Debug.Log("Text Asset: " + textAsset.name);
            }
            else
            {
                Debug.LogError("TextAssetPath not found: " + textAssetPath);
            }
        }
    }

    public void SetupLocalization()
    {
        TextAsset textAsset;
        // Search for the specified language file
        if (m_LocalizationFiles.ContainsKey(m_Language.ToString()))
        {
            Debug.Log("Selected language: " + m_Language);
            textAsset = m_LocalizationFiles[m_Language.ToString()];
        }
        // If we can't find the specific language default to English
        else
        {
            Debug.LogError("Couldn't find localization file for: " + m_Language);
            textAsset = m_LocalizationFiles[Language.English.ToString()];
        }

        // Load XML document
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(textAsset.text);

        // Get all elements called "Entry"
        XmlNodeList entryList = xmlDocument.GetElementsByTagName("Entry");

        // Iterate over each Entry element and store them in the Dictionary
        foreach (XmlNode entry in entryList)
        {
            if (!m_LocalizationText.ContainsKey(entry.FirstChild.InnerText))
            {
                Debug.Log("Added Key: " + entry.FirstChild.InnerText + " with value: " + entry.LastChild.InnerText);
                m_LocalizationText.Add(entry.FirstChild.InnerText, entry.LastChild.InnerText);
            }
            else
            {
                Debug.LogError("Duplicate Localization key detected: " + entry.FirstChild.InnerText);
            }
        }
    }

    public string GetLocalisedString(string key)
    {
        string localisedString = "";
        if (!m_LocalizationText.TryGetValue(key, out localisedString))
        {
            localisedString = "LOC KEY: " + key;
        }

        return localisedString;
    }
}
