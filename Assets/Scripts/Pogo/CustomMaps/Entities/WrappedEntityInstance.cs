using BSPImporter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BSPImporter.BSPLoader;

namespace Pogo.CustomMaps.Entities
{
    public abstract class WrappedEntityInstance
    {
        protected string ClassName { get; private set; }
        protected uint SpawnFlags;
        protected EntityInstance Instance;
        private IBSPLoaderContext Context;

        public WrappedEntityInstance(string className, EntityInstance instance, IBSPLoaderContext context)
        {
            ClassName = className;
            Context = context;
            Instance = instance;
            if (Instance.entity.ClassName != ClassName)
            {
                throw new ArgumentException($"Tried to spawn WrappedEntity {GetType()} but got unexpected ClassName {Instance.entity.ClassName}");
            }
            SpawnFlags = Instance.entity.Spawnflags;
        }

        public bool GetSpawnFlag(uint flagValue) => (SpawnFlags & flagValue) != 0;

        public EntityInstance GetSingleTarget(string key = "target")
        {
            string targetName = Instance.entity[key];
            var targets = Context.GetNamedEntities(targetName);
            if (targets.Count == 1)
            {
                return targets[0];
            }
            else if (targets.Count == 0)
            {
                throw new FormatException($"{Instance.entity.ClassName} named {Instance.entity.Name} couldn't find {key} named {targetName}");
            }
            else
            {
                throw new FormatException($"{Instance.entity.ClassName} named {Instance.entity.Name} found multiple {key}s named {targetName}. expected only 1");
            }
        }
    }
}
