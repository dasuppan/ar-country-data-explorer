using System;
using UnityEngine.Events;
using Void = GameDesignGame.Utility.GameEvents.Void;

namespace Utils.GameEvents.UnityEvents
{
    [Serializable]
    public class UnityVoidEvent : UnityEvent<Void>
    {
    }
}