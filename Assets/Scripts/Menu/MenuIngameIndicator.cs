using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuIngameIndicator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] IngameMenuButton m_menu;
    [SerializeField] int m_index;

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        m_menu.UpdateSelection(m_index);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        //SelectedState(false);
    }
}
