using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public static class EventManager
{
    public static event Action<string> StackIsComplete;
    public static event Action StackIsWrong;
    public static event Action StackIsIncomplete;

    public static event Action AnchorEnabled;
    public static event Action CardSwiped;

    public static event Action OnLevelStarted;

    public static event Action EnableSwiping;
    public static event Action DisableSwiping;

    public static event Action DisableColliders;
    public static event Action EnableColliders;


    public static event Action CardConfig;
    public static event Action FixRotation;

    public static event Action<float> OnSensitivityChanged;

    public static void StackIsCompleteEvent(string cardTag)
    {
        StackIsComplete?.Invoke(cardTag);
    }

    public static void StackIsWrongEvent()
    {
        StackIsWrong?.Invoke();
    }

    public static void StackIsIncompleteEvent()
    {
        StackIsIncomplete?.Invoke();
    }

    public static void AnchorEnabledEvent()
    {
        AnchorEnabled?.Invoke();
    }

    public static void CardSwipedEvent()
    {
        CardSwiped?.Invoke();
    }

    public static void OnLevelStartedEvent()
    {
        OnLevelStarted?.Invoke();
    }

    public static void EnableSwipingEvent()
    {
        EnableSwiping?.Invoke();
    }

    public static void DisableSwipingEvent()
    {
        DisableSwiping?.Invoke();
    }

    public static void CardConfigEvent()
    {
        CardConfig?.Invoke();
    }

    public static void FixRotationEvent()
    {
        FixRotation?.Invoke();
    }

    public static void OnSensitivityChangedEvent(float sensitivity)
    {
        OnSensitivityChanged?.Invoke(sensitivity);
    }

    public static void DisableCollidersEvent()
    {
        DisableColliders?.Invoke();
    }

    public static void EnableCollidersEvent()
    {
        EnableColliders?.Invoke();
    }

    public static void CleanEvents()
    {
        StackIsComplete = null;
        StackIsWrong = null;
        StackIsIncomplete = null;
        AnchorEnabled = null;
        CardSwiped = null;
        OnLevelStarted = null;
        EnableSwiping = null;
        DisableSwiping = null;
        DisableColliders = null;
        EnableColliders = null;
        CardConfig = null;
        FixRotation = null;
        OnSensitivityChanged = null;
    }
}

