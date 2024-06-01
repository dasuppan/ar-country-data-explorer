using GameDesignGame.Utility.GameEvents;
using UnityEngine;

namespace Utils.GameEvents.Events
{
    [CreateAssetMenu(fileName = "New County Relation Event", menuName = "Game Events/Country Relation Event")]
    public class CountryRelationEvent : BaseGameEvent<CountryRelation>
    {
    }
}