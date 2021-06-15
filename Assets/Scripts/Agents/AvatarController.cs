using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarController : MonoBehaviour
{
    Animator m_animator;

    [SerializeField] int m_damage;
    [SerializeField] Camera m_cam;
    bool m_dying;

    Vector3 m_originalPos;
    Quaternion m_originalRot;
    bool m_died;

    void Start() {
        m_originalPos = transform.position;
        m_originalRot = transform.rotation;
        m_animator = gameObject.GetComponent<Animator>();
    }

    public void UpdateHealthStatus(float ratio){
        m_animator.SetFloat("Blend", ratio);
    }

    public void DamageToDie(int damage){
        if(damage > 0){
            m_cam.backgroundColor = Color.red;
            m_dying = true;
            m_died = true;
            m_animator.SetBool("Dying", true);
        }
        if(damage > 0 && damage <= 5){
            m_animator.SetBool("Dying0", true);
        }
        else if(damage > 5 && damage <= 10){
            m_animator.SetBool("Dying1", true);
        }
        else if(damage > 10){
            m_animator.SetBool("Dying2", true);
        }

        if(damage == 0){
            m_cam.backgroundColor = Color.white;
            m_dying = false;
            m_animator.SetBool("Dying", false);
            m_animator.SetBool("Dying0", false);
            m_animator.SetBool("Dying1", false);
            m_animator.SetBool("Dying2", false);
        }

        if(!m_dying && m_died){
            transform.position = m_originalPos;
            transform.rotation = m_originalRot;
            m_died = false;
        }
    }
}
