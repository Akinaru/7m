using UnityEngine;

[CreateAssetMenu(fileName = "InteractablePreset", menuName = "Game/Interactable Preset")]
public class InteractablePreset : ScriptableObject
{
    public string id;
    public string displayName;
    public string actionLabel;
}
