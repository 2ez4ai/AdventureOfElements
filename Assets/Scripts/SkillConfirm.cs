using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillConfirm : MonoBehaviour
{
    [SerializeField] DialogScript m_dialog;
    Button m_btn;
    void Start(){
        m_btn = GetComponent<Button>();
        m_btn.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick(){
        Debug.Log("Confirm!");
        m_dialog.TurnOff();
    }
}
