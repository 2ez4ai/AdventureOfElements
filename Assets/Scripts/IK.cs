using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
public class IK : MonoBehaviour
{
    Animator m_animator;
    NavMeshAgent m_navMeshAgent;

    [SerializeField] Transform m_leftHandTarget;
    [SerializeField] Transform m_leftFootTransform;
    [SerializeField] Transform m_rightFootTransform;

    [SerializeField] Transform m_navStart;
    [SerializeField] Transform m_navDes;

    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_navMeshAgent = GetComponent<NavMeshAgent>();

        m_navMeshAgent.SetDestination(m_navDes.position);
    }

    void Update(){
        // Debug.Log(Vector3.Distance(transform.position, m_navDes.position));
        if(Vector3.Distance(transform.position, m_navDes.position) < 0.65f){
            m_navMeshAgent.isStopped = true;
            m_animator.SetBool("IsTouching", true);
        }
    }

    void OnAnimatorIK(int layerIndex)
    {
        // Left-Foot and Right-Foot IK
        UpdateFootIK(m_leftFootTransform, AvatarIKGoal.LeftFoot);
        UpdateFootIK(m_rightFootTransform, AvatarIKGoal.RightFoot);

        // Left-Hand IK
        UpdateHandIK(m_leftHandTarget, AvatarIKGoal.LeftHand);
    }

    void UpdateHandIK(Transform target, AvatarIKGoal avatarIKGoal)
    {
        m_animator.SetIKPositionWeight(avatarIKGoal, 0);
        m_animator.SetIKRotationWeight(avatarIKGoal, 0);
    }

    void UpdateFootIK(Transform bone, AvatarIKGoal avatarIKGoal)
    {
        Vector3 rayCastPos = bone.transform.position;
        rayCastPos.y += 1.0f;
        Ray ray = new Ray(rayCastPos, -Vector3.up);
        RaycastHit hit;
        LayerMask layerMask = LayerMask.GetMask("Default");
        if (Physics.Raycast(ray, out hit, 2.0f, layerMask))
        {
            Vector3 footPos = hit.point;
            footPos.y += 0.1f;

            m_animator.SetIKPositionWeight(avatarIKGoal, 1);
            m_animator.SetIKRotationWeight(avatarIKGoal, 1);
            m_animator.SetIKPosition(avatarIKGoal, footPos);
            m_animator.SetIKRotation(avatarIKGoal, hit.collider.gameObject.transform.rotation * bone.rotation);
        }
        else
        {
            m_animator.SetIKPositionWeight(avatarIKGoal, 0);
            m_animator.SetIKRotationWeight(avatarIKGoal, 0);
        }
    }
}
