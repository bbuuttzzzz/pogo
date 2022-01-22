using UnityEngine;
using UnityEngine.Events;

namespace Logic
{
    public abstract class Listener : MonoBehaviour
    {
        public bool ListenOnSpawn = false;
        bool listening = false;

        public UnityEvent OnHeard;

        private void Awake()
        {
            SetListening(ListenOnSpawn);
        }

        public void SetListening(bool isListening)
        {
            if (listening == isListening) return;
            listening = isListening;
            if (listening)
            {
                Listen();
            }
            else
            {
                StopListening();
            }
        }

        protected void Heard()
        {
            OnHeard?.Invoke();
        }

        protected abstract void Listen();
        protected abstract void StopListening();
    }
}
