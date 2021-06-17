using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoseButton : MonoBehaviour
{
    [SerializeField] Button m_retryBtn;
    [SerializeField] Button m_quitBtn;
    [SerializeField] DialogScript m_dialog;

    Text m_backText;
    Text m_quitText;

    Language m_language;

    int m_currentIndex = 0;
    const float k_maxSelectionCooldown = 0.25f;
    float m_selectionCooldown;

    public bool m_activated;

    // Start is called before the first frame update
    void Start()
    {
        m_selectionCooldown = k_maxSelectionCooldown;
        m_activated = false;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSelection();
    }

    public void OnRetryClicked(){
        SoundManager.m_instance.PlayConfirmSound();
        LocalizationManager.m_instance.loadChecker = true;
        SceneManager.LoadScene("GameScene");
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
            m_retryBtn.Select();
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
