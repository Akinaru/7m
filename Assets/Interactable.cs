// Interactable.cs
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class InteractEvent : UnityEvent<Interactable> { }

public class Interactable : MonoBehaviour
{
    public InteractablePreset preset;
    public InteractEvent OnInteract;

    public string Id => preset != null ? preset.id : null;
    public string DisplayName => preset != null ? preset.displayName : null;
    public string ActionLabel => preset != null ? preset.actionLabel : null;
}
