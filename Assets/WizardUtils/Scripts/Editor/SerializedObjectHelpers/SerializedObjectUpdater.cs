using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace WizardUtils.SerializedObjectHelpers
{
    public class SerializedObjectUpdater
    {
        private List<SerializedPropertyChangeHandler> Handlers;

        public SerializedObjectUpdater()
        {
            Handlers = new List<SerializedPropertyChangeHandler>();
        }

        public void Add<T>(Func<T> get, Action<SerializedPropertyChangedArgs<T>> onChangedCallback, EqualityComparer<T> equalityComparer = null)
        {
            SerializedPropertyChangeHandler<T> item = new SerializedPropertyChangeHandler<T>(get, onChangedCallback, equalityComparer);
            Handlers.Add(item);
        }

        public void Check()
        {
            foreach(var handler in Handlers)
            {
                handler.Check();
            }
        }
    }
}
