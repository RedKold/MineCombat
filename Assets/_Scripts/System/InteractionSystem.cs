using UnityEngine;

namespace MineCombat
{
    public class InteractionSystem : Singleton<InteractionSystem>
    {
        public bool IsDragging { get; private set; } = false;
        public bool IsHovering { get; private set; } = false;
        public bool IsLocked { get; private set; } = false; // 用于暂停交互（比如动画中）

        // ======================
        // 状态控制接口
        // ======================

        public void BeginDrag()
        {
            if (IsLocked) return;
            IsDragging = true;
            IsHovering = false;
            CardViewHoverSystem.Instance.Hide();
        }

        public void EndDrag()
        {
            IsDragging = false;
        }

        public void BeginHover()
        {
            if (IsLocked || IsDragging) return;
            IsHovering = true;
        }

        public void EndHover()
        {
            IsHovering = false;
        }

        public void LockInteraction()
        {
            IsLocked = true;
            CardViewHoverSystem.Instance.Hide();
        }

        public void UnlockInteraction()
        {
            IsLocked = false;
        }

        // ======================
        // 状态判断接口
        // ======================
        public bool CanHover()
        {
            return !IsDragging && !IsLocked;
        }

        public bool CanDrag()
        {
            return !IsLocked;
        }
    }
}
