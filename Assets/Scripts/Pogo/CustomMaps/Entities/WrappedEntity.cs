using BSPImporter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pogo.CustomMaps.Entities
{
    public abstract class WrappedEntity
    {
        protected abstract string ClassName { get; }
        protected uint SpawnFlags;
        protected BSPLoader.EntityInstance Instance;

        public WrappedEntity(BSPLoader.EntityInstance instance)
        {
            if (instance.entity.ClassName != ClassName)
            {
                throw new ArgumentException($"Tried to spawn WrappedEntity {GetType()} but got unexpected ClassName {instance.entity.ClassName}");
            }
            Instance = instance;
            SpawnFlags = instance.entity.Spawnflags;
        }

        public bool GetSpawnFlag(uint flagValue) => (SpawnFlags & flagValue) != 0;
    }
}
