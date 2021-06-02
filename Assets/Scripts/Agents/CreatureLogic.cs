using System.Security.Cryptography.X509Certificates;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatureLogic : MonoBehaviour
{
    public int m_HP = 100;
    public int m_maxHP = 100;
    public int m_injureType = 0;

    [SerializeField] Text m_textHP;
    [SerializeField] MouseOver m_mouseOverHP;

    int damage = 0;

    public void InitUI(){
        SetHPUI();
    }

    public void TakeDamage(){
        m_HP = m_HP - damage < 0? 0: m_HP-damage;
        m_textHP.text = ": " + m_HP + " / " + m_maxHP;
        // if(m_HP < 1){
        //     Debug.Log("You win!");
        // }
        damage = 0;
    }

    public bool UpdateStepDamage(int c, int t){
        // update the damage the creature will take in this step
        damage += 1;
        if(t == m_injureType || m_injureType == -1){
            damage += 1;
            return true;
        }
        return false;
    }

    void UpdateIconTooltip(MouseOver script, string name, string level, string effect){
        script.m_name = name;
        script.m_level = level;
        script.m_effect = effect;
    }

    void SetHPUI(){
        string hpInfo = m_HP + " / " + m_maxHP;
        m_textHP.text = ": " + hpInfo;
        string effect = "The creature has " + hpInfo + " HP left. You will win the battle when it is 0.";
        UpdateIconTooltip(m_mouseOverHP, "HP", "", effect);
    }
}
