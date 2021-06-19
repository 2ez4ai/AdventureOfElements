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
    public int m_weakPointDamage = 1;

    [SerializeField] Text m_textHP;
    [SerializeField] Image m_HPBar;
    [SerializeField] List<Sprite> m_HPBarColor;
    [SerializeField] MouseOver m_mouseOverHP;

    int m_damage = 0;
    int m_damagetType = 0;

    public void InitUI(){
        SetHPUI();
    }

    public void TakeDamage(){
        m_HP = m_HP - m_damage < 0? 0: m_HP-m_damage;
        // m_textHP.text = m_HP + " / " + m_maxHP;
        SetHPUI();
        m_damage = 0;
    }

    public bool UpdateStepDamage(int c, int t){
        // update the damage the creature will take in this step
        m_damage += 1;
        m_damagetType = t;
        if(t == m_injureType || m_injureType == -1){
            m_damage += m_weakPointDamage;
            SoundManager.m_instance.PlayWeakPointSound();
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
        string hpInfo = m_HP + "/" + m_maxHP;
        m_textHP.text = hpInfo;
        float ratio = 1.0f * m_HP / m_maxHP;
        if(ratio > 0.65){
            m_HPBar.sprite = m_HPBarColor[0];   // green
        }
        else{
            m_HPBar.sprite = m_HPBarColor[1];
        }
        if(ratio < 0.3f){
            m_HPBar.sprite = m_HPBarColor[2];
        }
        m_HPBar.fillAmount = ratio;
        string effect = LocalizationManager.m_instance.GetLocalisedString("CreatureHPPart1") + hpInfo + LocalizationManager.m_instance.GetLocalisedString("CreatureHPPart2");

        UpdateIconTooltip(m_mouseOverHP, LocalizationManager.m_instance.GetLocalisedString("HP"), "", effect);
    }
}
