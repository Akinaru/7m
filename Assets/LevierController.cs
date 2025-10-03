// LevierController.cs
using UnityEngine;

public class LevierController : MonoBehaviour
{
    [Header("Références (drag & drop)")]
    public GameObject Levier1;
    public GameObject Levier2;
    public GameObject Levier3;

    [Header("Configuration visuelle")]
    public float inactiveZPos = 0f;
    public float activeZPos = 0.5f;
    public float inactiveZScale = 1f;
    public float activeZScale = -1f;

    void Awake()
    {
        this.CheckImport();
    }

    void OnEnable()
    {
        this.CheckImport();

        if (GameController.Instance != null)
        {
            GameController.Instance.OnLeverStateChanged += OnLeverStateChanged;
            Apply("levier1", GameController.Instance.GetLeverState("levier1"));
            Apply("levier2", GameController.Instance.GetLeverState("levier2"));
            Apply("levier3", GameController.Instance.GetLeverState("levier3"));
        }
    }

    void OnDisable()
    {
        if (GameController.Instance != null)
            GameController.Instance.OnLeverStateChanged -= OnLeverStateChanged;
    }

    void OnLeverStateChanged(string key, bool value)
    {
        Apply(key, value);
    }

    void Apply(string key, bool isActive)
    {
        GameObject go = key == "levier1" ? Levier1 :
                        key == "levier2" ? Levier2 : key == "levier3" ? Levier3 : null;

        Debug.Log("[LevierController] Apply state change: " + key + " -> " + isActive, this);
        if (go == null) return;

        var t = go.transform;
        var lp = t.localPosition;
        var ls = t.localScale;

        t.localPosition = new Vector3(lp.x, lp.y, isActive ? activeZPos : inactiveZPos);
        t.localScale = new Vector3(ls.x, ls.y, isActive ? activeZScale : inactiveZScale);
    }

    public void ForceSync()
    {
        if (GameController.Instance == null) return;
        Apply("levier1", GameController.Instance.GetLeverState("levier1"));
        Apply("levier2", GameController.Instance.GetLeverState("levier2"));
        Apply("levier3", GameController.Instance.GetLeverState("levier3"));
    }

    void CheckImport()
    {
        if (Levier1 == null)
            Debug.LogError("[LevierController] Missing reference: 'Levier1' is not assigned.", this);
        if (Levier2 == null)
            Debug.LogError("[LevierController] Missing reference: 'Levier2' is not assigned.", this);
        if (Levier3 == null)
            Debug.LogError("[LevierController] Missing reference: 'Levier3' is not assigned.", this);
    }
}
