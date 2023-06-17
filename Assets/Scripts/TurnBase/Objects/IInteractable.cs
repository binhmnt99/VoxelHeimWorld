using System;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public interface IInteractable
    {
        void Interact(Action onInteractionComplete);
    }

}