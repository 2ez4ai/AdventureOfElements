using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogScript : MonoBehaviour
{
    public bool m_stateOn;

    // disable other interaction
    [SerializeField] Collider m_rayBlocker;
    [SerializeField] MeshRenderer m_rayRenderer;
    [SerializeField] Tooltip m_tooltip;
    [SerializeField] MouseOver m_mouseOverWeakness;
    [SerializeField] MouseOver m_mouseOverAttack;

    // win or lose
    [SerializeField] GameObject m_winBoard;
    [SerializeField] GameObject m_loseBoard;

    public void TurnOn(bool win){
        m_tooltip.m_disabled = true;
        m_rayBlocker.enabled = true;
        // m_rayRenderer.enabled = true;
        m_mouseOverWeakness.enabled = false;
        m_mouseOverAttack.enabled = false;
        gameObject.SetActive(true);
        if(win){
            m_winBoard.SetActive(true);
        }
        else{
            m_loseBoard.SetActive(true);
        }
    }

    public void TurnOff(){
        gameObject.SetActive(false);
        m_winBoard.SetActive(false);
        m_loseBoard.SetActive(false);
        m_tooltip.m_disabled = false;
        m_rayBlocker.enabled = false;
        // m_rayRenderer.enabled = false;
        m_mouseOverWeakness.enabled = true;
        m_mouseOverAttack.enabled = true;
    }
}
