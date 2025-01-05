using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    public static Action OnBackToSeed;

    public void CloseButton()
    {
        Application.Quit();
    }

    public void BackButton()
    {
        OnBackToSeed?.Invoke();
    }
}
