using System.Collections;
using System.Collections.Generic;
using MineCombat;
using UnityEngine;

public class InteractionSystem : Singleton<InteractionSystem>
{
    public bool PlayerIsDragging { get; set; } = false;
    public bool PlayerCanInteract()
    {
        if (!CardDragSystem.Instance.IsDragging)
        {
            // update the dragging state
            PlayerIsDragging = true;
            return true;
        }
        else return false;
    }

    public bool PlayerCanHover()
    {
        if (CardDragSystem.Instance.IsDragging) return false;
        else return true;
    }
}
