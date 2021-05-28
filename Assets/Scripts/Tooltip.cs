using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    [SerializeField] Text m_name;
    [SerializeField] Text m_level;
    [SerializeField] Text m_description;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void UpdateInfo(string name, string level, string description){
        m_name.text = name;
        m_level.text = level;
        m_description.text = description;
        gameObject.SetActive(true);
    }

    public void CloseInfo(){
        gameObject.SetActive(false);
    }
}
