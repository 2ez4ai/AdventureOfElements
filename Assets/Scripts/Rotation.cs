using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    [SerializeField] int m_axis;
    [SerializeField] float m_rotationSpeed = 1.5f;

    // Update is called once per frame
    void FixedUpdate()
    {
        switch(m_axis){
            case 0:
                transform.Rotate(new Vector3(m_rotationSpeed, 0, 0));
                break;
            case 1:
                transform.Rotate(new Vector3(0, m_rotationSpeed, 0));
                break;
            case 2:
                transform.Rotate(new Vector3(0, 0, m_rotationSpeed));
                break;
        }
    }
}
