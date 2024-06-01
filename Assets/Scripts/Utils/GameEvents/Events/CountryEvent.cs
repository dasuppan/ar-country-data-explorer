using GameDesignGame.Utility.GameEvents;
using UnityEngine;

namespace Utils.GameEvents.Events
{
    [CreateAssetMenu(fileName = "New County Event", menuName = "Game Events/Country Event")]
    public class CountryEvent : BaseGameEvent<Country>
    {
    }
}