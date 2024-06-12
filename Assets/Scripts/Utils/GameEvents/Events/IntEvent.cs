using UnityEngine;
using Utils.GameEvents;

namespace GameDesignGame.Utility.GameEvents.Events
{
    [CreateAssetMenu(fileName = "New Int Event", menuName = "Game Events/Int Event")]
    public class IntEvent : BaseGameEvent<int>
    {
    }
}