using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillConfirm : MonoBehaviour
{
    [SerializeField] DialogScript m_dialog;
    [SerializeField] int m_btnType = 0;

    Button m_btn;
    void Start(){
        m_btn = GetComponent<Button>();
        m_btn.onClick.AddListener(OnClick);
    }

    void OnClick(){
        if(m_btnType == 0){
            Debug.Log("Confirm skill selection !");
            m_dialog.TurnOff();
        }
        if(m_btnType == 1){
            Debug.Log("We knew you lose, but it is ok.");
            m_dialog.TurnOff();
        }
    }
}
