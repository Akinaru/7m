// ActionController.cs
using UnityEngine;

public class ActionController : BaseController<ActionController>
{
    public struct InteractionContext
    {
        public RaycastHit Hit;
        public Interactable Interactable;
    }

    private InteractionContext? _latestContext;

    public void SetContext(InteractionContext ctx)
    {
        _latestContext = ctx;
    }

    public bool TryGetContext(out InteractionContext ctx)
    {
        if (_latestContext.HasValue)
        {
            ctx = _latestContext.Value;
            return true;
        }
        ctx = default;
        return false;
    }

    void Start()
    {
    }

    void Update()
    {
    }

    public void ToggleLevier()
    {
        if (!TryGetContext(out var ctx))
            return;

        var targetTransform = ctx.Hit.collider != null ? ctx.Hit.collider.transform : ctx.Interactable.transform;

        targetTransform.Rotate(0f, 180f, 0f, Space.Self);

        if (GameController.Instance == null)
            return;

        string key = (ctx.Interactable.Id ?? ctx.Interactable.DisplayName ?? string.Empty)
            .Trim()
            .ToLowerInvariant();

        switch (key)
        {
            case "levier1":
                GameController.Instance.FirstLevierActivated = !GameController.Instance.FirstLevierActivated;
                break;
            case "levier2":
                GameController.Instance.SecondLevierActivated = !GameController.Instance.SecondLevierActivated;
                break;
            case "levier3":
                GameController.Instance.ThirdLevierActivated = !GameController.Instance.ThirdLevierActivated;
                break;
            default:
                break;
        }

        Debug.Log(
            $"Leviers -> First:{GameController.Instance.FirstLevierActivated} | " +
            $"Second:{GameController.Instance.SecondLevierActivated} | " +
            $"Third:{GameController.Instance.ThirdLevierActivated}"
        );
    }


}
