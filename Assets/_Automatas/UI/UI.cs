using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public static Action OnClear;
    public static Action OnBackToSeed;
    public static Action OnStartAutomata;
    [SerializeField] private UIButton _backButton;
    [SerializeField] private UIButton _forwardButton;
    [SerializeField] private UIButton _clearButton;

    private void Awake()
    {
        RandomSeed.OnRandomSeed += DisableSelf;
    }

    public void BackButton()
    {
        _backButton.gameObject.SetActive(false);
        _forwardButton.gameObject.SetActive(true);
        _clearButton.gameObject.SetActive(true);
        OnBackToSeed?.Invoke();
    }

    public void ForwardButton()
    {
        _backButton.gameObject.SetActive(true);
        _forwardButton.gameObject.SetActive(false);
        _clearButton.gameObject.SetActive(false);
        OnStartAutomata?.Invoke();
    }

    public void ClearButton()
    {
        OnClear?.Invoke();
    }

    private void DisableSelf()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        RandomSeed.OnRandomSeed -= DisableSelf;
    }
}
