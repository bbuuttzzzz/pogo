using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pogo.Levels
{
    public abstract class LevelStateSubListener : MonoBehaviour
    {
        public LevelStateListener Parent;

        protected virtual void Awake()
        {
            Parent.OnLevelStateChanged.AddListener(Parent_OnStateChanged);
        }

        protected abstract void Parent_OnStateChanged(LevelStateChangedArgs e);
    }
}
