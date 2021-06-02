using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoseButton : MonoBehaviour
{
    Button m_btn;
    public bool m_activated = false;

    [SerializeField] DialogScript m_dialog;

    void Awake()
    {
        m_btn = gameObject.GetComponent<Button>();
        m_btn.onClick.AddListener(Pressed);
    }

    void Pressed()
    {
        m_dialog.TurnOff();
        m_activated = false;
    }
}
