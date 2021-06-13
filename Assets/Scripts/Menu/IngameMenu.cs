using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngameMenu : MonoBehaviour
{
    [SerializeField] GameObject m_menu;
    [SerializeField] Collider m_rayBlocker;

    [SerializeField] Sprite m_menuEng;
    [SerializeField] Sprite m_menuChs;

    [SerializeField] Tooltip m_battleTooltip;
    [SerializeField] Tooltip m_skillSelectionTooltip;

    bool m_opened = false;

    // Update is called once per frame
    void Update()
    {
        // if controller.start is pressed
        // CallOutMenu();
    }

    public void MenuController(){
        if(m_opened){
            CloseMenu();
        }
        else{
            CallOutMenu();
        }
    }

    public void CallOutMenu(){
        m_opened = true;
        m_rayBlocker.enabled = true;
        m_battleTooltip.m_disabled = true;
        m_skillSelectionTooltip.m_disabled = true;
        Image img = m_menu.GetComponent<Image>();
        if(LocalizationManager.m_instance.GetLanguage() == Language.Chinese){
            img.sprite = m_menuChs;
        }
        else{
            img.sprite = m_menuEng;
        }
        m_menu.SetActive(true);
    }

    public void CloseMenu(){
        m_opened = false;
        m_rayBlocker.enabled = false;
        m_battleTooltip.m_disabled = false;
        m_skillSelectionTooltip.m_disabled = false;
        m_menu.SetActive(false);
    }
}
