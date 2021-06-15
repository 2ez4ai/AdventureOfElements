using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuIndicator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Menu m_menu;
    [SerializeField] int m_index;
    GameObject m_textObj;

    // Start is called before the first frame update
    void Start()
    {
        m_textObj = transform.Find("Indicator").gameObject;
    }

    public void SelectedState(bool state){
        m_textObj.SetActive(state);
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        m_menu.UpdateSelection(m_index);
        // SoundManager.m_instance.PlaySelectSound();
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        //SelectedState(false);
    }
}
