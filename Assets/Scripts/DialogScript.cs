using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogScript : MonoBehaviour
{
    [SerializeField] bool m_enable = false;
    [SerializeField] Button m_btn;

    // disable other interaction
    [SerializeField] Collider m_rayBlocker;
    [SerializeField] Tooltip m_tooltip;
    [SerializeField] MouseOver m_mouseOverWeakness;
    [SerializeField] MouseOver m_mouseOverAttack;

    int m_skillSelection = -1;

    private void Start() {
        // add listener
        m_btn.onClick.AddListener(TurnOff);
    }

    private void Update() {
        if(m_enable){
            TurnOn();
        }
        else{
            TurnOff();
        }
    }

    void GenerateSkills(){
        // generate selections
    }

    void SelectSkill(int choice){
    }

    public void TurnOn(){
        gameObject.SetActive(true);
        m_tooltip.m_disabled = true;
        m_rayBlocker.enabled = true;
        m_mouseOverWeakness.enabled = false;
        m_mouseOverAttack.enabled = false;
    }

    public void TurnOff(){
        gameObject.SetActive(false);
        m_tooltip.m_disabled = false;
        m_rayBlocker.enabled = false;
        m_mouseOverWeakness.enabled = true;
        m_mouseOverAttack.enabled = true;
    }
}
