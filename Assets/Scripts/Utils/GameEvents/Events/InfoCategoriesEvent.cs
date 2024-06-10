using System.Collections.Generic;
using GameDesignGame.Utility.GameEvents;
using UnityEngine;

namespace Utils.GameEvents.Events
{
    [CreateAssetMenu(fileName = "New InfoCategory Event", menuName = "Game Events/Info Categories Event")]
    public class InfoCategoriesEvent : BaseGameEvent<List<InfoCategory>>
    {
    }
}