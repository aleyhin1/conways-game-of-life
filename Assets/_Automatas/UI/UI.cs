using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public static Action OnBackToSeed;
    public static Action OnStartAutomata;
    [SerializeField] private UIButton _backButton;
    [SerializeField] private UIButton _forwardButton;

    public void BackButton()
    {
        _backButton.gameObject.SetActive(false);
        _forwardButton.gameObject.SetActive(true);
        OnBackToSeed?.Invoke();
    }

    public void ForwardButton()
    {
        _backButton.gameObject.SetActive(true);
        _forwardButton.gameObject.SetActive(false);
        OnStartAutomata?.Invoke();
    }
}
