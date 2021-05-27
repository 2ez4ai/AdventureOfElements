using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    [SerializeField] Text m_text;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void UpdateInfo(){
        gameObject.SetActive(true);
        m_text.text = "Test";
    }

    public void CloseInfo(){
        gameObject.SetActive(false);
    }
}
