using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static bool IsOnUI { get; private set; }

    public void OnPointerEnter(PointerEventData e)
    {
        IsOnUI = true;
    }

    public void OnPointerExit(PointerEventData e)
    {
        IsOnUI = false;
    }
}
