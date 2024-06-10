using System.Collections.Generic;
using GameDesignGame.Utility.GameEvents.Listeners;
using Utils.GameEvents.Events;
using Utils.GameEvents.UnityEvents;

namespace Utils.GameEvents.Listeners
{
    public class InfoCategoriesListener : BaseGameEventListener<List<InfoCategory>, InfoCategoriesEvent, UnityInfoCategoriesEvent>
    {
    }
}