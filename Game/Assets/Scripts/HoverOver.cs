using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public GameObject HoverPanel;
    public void OnPointerEnter(PointerEventData eventData)
    {
        HoverPanel.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HoverPanel.SetActive(false);

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        HoverPanel.SetActive(false);

    }

}
