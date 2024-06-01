using GameDesignGame.Utility.GameEvents;
using UnityEngine;

namespace Utils.GameEvents.Events
{
    [CreateAssetMenu(fileName = "New County Renderer Event", menuName = "Game Events/Country Renderer Event")]
    public class CountryRendererEvent : BaseGameEvent<CountryRenderer>
    {
    }
}