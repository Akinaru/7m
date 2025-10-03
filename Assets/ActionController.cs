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

    public void TpOutOfBus()
    {
        if (!TryGetContext(out var ctx))
            return;


        if (PlayerController.Instance != null && PlayerController.Instance.playerRoot != null)
        {
            bool inBus = GameController.Instance.PlayerInBus;

            if (inBus)
            {
                PlayerController.Instance.playerRoot.position = new Vector3(-4.8f, 1.2f, -3.4f);
                GameController.Instance.PlayerInBus = false;
            }
            else
            {
                PlayerController.Instance.playerRoot.position = new Vector3(-1.9f, 2f, -2.2f);
                GameController.Instance.PlayerInBus = true;
            }
        }
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
