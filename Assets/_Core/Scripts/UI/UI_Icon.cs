using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Icon : MonoBehaviour
{
    public Image iconImage;

    public void OnPointerEnter(BaseEventData baseEventData)
    {
        Debug.Log("OnPointerEnter");
    }

    public void OnPointerExit(BaseEventData baseEventData)
    {
        Debug.Log("OnPointerExit");
    }

    public void OnClick(BaseEventData baseEventData)
    {
        Debug.Log("OnClick");
    }
}
