using System;
using System.Collections.Generic;
using UnityEditor;

namespace WizardUtils.SerializedObjectHelpers
{
    public class SerializedObjectUpdater
    {
        private SerializedObject Target;
        private List<SerializedPropertyChangeHandler> Handlers;

        public SerializedObjectUpdater(SerializedObject target)
        {
            Target = target;
            Handlers = new List<SerializedPropertyChangeHandler>();
        }

        public void Add<T>(Func<T> get, Action<SerializedPropertyChangedArgs<T>> onChangedCallback, EqualityComparer<T> equalityComparer = null)
        {
            SerializedPropertyChangeHandler<T> item = new SerializedPropertyChangeHandler<T>(get, onChangedCallback, equalityComparer);
            Handlers.Add(item);
        }

        public void ApplyModifiedProperties()
        {
            Target.ApplyModifiedProperties();

            foreach(var handler in Handlers)
            {
                handler.Check();
            }
        }
    }
}
