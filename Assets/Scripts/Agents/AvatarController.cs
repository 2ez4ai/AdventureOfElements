using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarController : MonoBehaviour
{
    public Animator animator;

    [SerializeField] int m_damage;
    [SerializeField] Camera m_cam;
    bool m_dying;

    Vector3 m_originalPos;
    Quaternion m_originalRot;
    bool m_died;
    const float k_injureDurationTime = 0.5f;
    float m_injureTime = 0.0f;
    float m_blinkTimeFactor = 0.8f;
    int m_avatarColor = 0;
    bool m_injured = false;
    bool m_colorChanged = false;
    Color m_colorDefault;

    void Start() {
        m_originalPos = transform.position;
        m_originalRot = transform.rotation;
        m_colorDefault = new Color32(231, 231, 231, 255);
        animator = gameObject.GetComponent<Animator>();
    }

    void FixedUpdate(){
        if(m_injured){
            m_injured = false;
            m_colorChanged = true;
            m_blinkTimeFactor = 0.75f;
            m_avatarColor = 1;
            m_cam.backgroundColor = m_avatarColor == 0 ? m_colorDefault : Color.red;
            m_injureTime = k_injureDurationTime;
        }

        m_injureTime -= Time.deltaTime;

        if(m_injureTime < m_blinkTimeFactor * k_injureDurationTime && m_colorChanged){
            m_avatarColor = m_avatarColor ^ 1;
            m_cam.backgroundColor = m_avatarColor == 0 ? m_colorDefault : Color.red;
            m_blinkTimeFactor -= 0.2f;
        }

        if(m_blinkTimeFactor < 0.1f && m_colorChanged){
            m_colorChanged = false;
            m_cam.backgroundColor = m_colorDefault;
        }
    }

    public void UpdateHealthStatus(float ratio){
        animator.SetFloat("Blend", ratio);
    }

    public void GetInjured(float ratio){
        m_injured = true;
        if(ratio < 0.3f){
            animator.SetBool("InjuredInIdling", true);
        }
        else{
            animator.SetBool("InjuredInRunning", true);
        }
    }

    public void DamageToDie(int damage){
        if(damage > 0){
            m_cam.backgroundColor = Color.red;
            m_dying = true;
            m_died = true;
            animator.SetBool("Dying", true);
        }
        if(damage > 0 && damage <= 15){
            animator.SetBool("Dying0", true);
        }
        else if(damage > 15 && damage <= 22){
            animator.SetBool("Dying1", true);
        }
        else if(damage > 22){
            animator.SetBool("Dying2", true);
        }

        if(damage == 0){
            m_cam.backgroundColor = m_colorDefault;
            m_dying = false;
            animator.SetBool("Dying", false);
            animator.SetBool("Dying0", false);
            animator.SetBool("Dying1", false);
            animator.SetBool("Dying2", false);
        }

        if(!m_dying && m_died){
            transform.position = m_originalPos;
            transform.rotation = m_originalRot;
            m_died = false;
        }
    }
}
