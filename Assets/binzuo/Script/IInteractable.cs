using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace binzuo
{

    public interface IInteractable
    {
        void Interact(Action onInteractionComplete);

    }

}
