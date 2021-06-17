using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationDone : MonoBehaviour
{
    AvatarController m_avatarController;

    // Start is called before the first frame update
    void Start()
    {
        m_avatarController = FindObjectOfType<AvatarController>();
    }

    public void InjuredInIdlingDone(){
        m_avatarController.animator.SetBool("InjuredInIdling", false);
    }

    public void InjuredInRunningDone(){
        m_avatarController.animator.SetBool("InjuredInRunning", false);
    }
}
