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

    public void ToggleLevier()
    {
        if (!TryGetContext(out var ctx))
            return;

        if (GameController.Instance == null)
            return;

        string key = (ctx.Interactable.Id ?? ctx.Interactable.DisplayName ?? string.Empty)
            .Trim()
            .ToLowerInvariant();

        switch (key)
        {
            case "levier1":
            case "levier2":
                GameController.Instance.ToggleLever(key);
                break;
            default:
                return;
        }
    }
}
