using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Utils.GameEvents.UnityEvents
{
    [Serializable]
    public class UnityInfoCategoriesEvent : UnityEvent<List<InfoCategory>>
    {
    }
}