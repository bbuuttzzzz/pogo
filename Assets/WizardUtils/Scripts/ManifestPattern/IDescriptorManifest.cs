using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WizardUtils.ManifestPattern
{
    public interface IDescriptorManifest<T> where T : ScriptableObject
    {
        public bool Contains(T descriptor);
        public void Add(T descriptor);
        public void Remove(T descriptor);
    }
}
