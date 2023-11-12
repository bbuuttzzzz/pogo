using System;
using System.Collections.Generic;

namespace WizardUtils.SerializedObjectHelpers
{
    public abstract class SerializedPropertyChangeHandler
    {
        public abstract void Store();
        
        public abstract void Check();
    }

    public class SerializedPropertyChangeHandler<T> : SerializedPropertyChangeHandler
    {
        private EqualityComparer<T> EqualityComparer;
        private Action<SerializedPropertyChangedArgs<T>> OnChangedCallback;
        private Func<T> Get;

        private T cachedValue;

        public SerializedPropertyChangeHandler(Func<T> get, Action<SerializedPropertyChangedArgs<T>> onChangedCallback, EqualityComparer<T> equalityComparer = null)
        {
            OnChangedCallback = onChangedCallback;
            EqualityComparer = equalityComparer ?? EqualityComparer<T>.Default;
            Get = get;
            Store();
        }

        public override void Store()
        {
            cachedValue = Get();
        }

        public override void Check()
        {
            T newValue = Get();
            if (!EqualityComparer.Equals(cachedValue, newValue))
            {
                OnChangedCallback.Invoke(new SerializedPropertyChangedArgs<T>(cachedValue, newValue));
            }
        }
    }
}
