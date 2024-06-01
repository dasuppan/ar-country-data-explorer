using UnityEngine;
using UnityEngine.Events;

namespace GameDesignGame.Utility.GameEvents.Listeners
{
    // Type, Event, UnityEventResponse
    public class BaseGameEventListener<T, E, UER> : MonoBehaviour, IGameEventListener<T>
        where E : BaseGameEvent<T> where UER : UnityEvent<T>
    {
        [SerializeField] private E gameEvent;

        public E GameEvent
        {
            get => gameEvent;
            set => gameEvent = value;
        }

        [SerializeField] private UER unityEventResponse;

        private void OnEnable()
        {
            if (gameEvent == null) return;
            gameEvent.RegisterListener(this);
        }

        private void OnDisable()
        {
            if (gameEvent == null) return;
            gameEvent.UnregisterListener(this);
        }

        public void OnEventRaised(T item)
        {
            if (unityEventResponse != null)
            {
                unityEventResponse.Invoke(item);
            }
        }
    }
}