using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum LocStringKey{

}

[System.Serializable]
public class UIElement{
    public LocStringKey m_locStringKey;
    public GameObject m_gameObject;
}

public class Menu : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnStartClicked(){
        // UIManager.m_Instance.PlayConfirmSound();
        SceneManager.LoadScene("GameScene");
    }

    public void OnLoadClicked(){
        // Havent implemented yet.
        Debug.Log("Load nothing.");
    }

    public void OnOptionClicked(){
        // Havent implemented yet.
        Debug.Log("Settings.");
    }

    public void OnQuitClicked(){
        // play confirm sound
        Debug.Log("Quit.");
        Application.Quit();
    }

}
