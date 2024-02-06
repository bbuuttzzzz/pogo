using BSPImporter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BSPImporter.BSPLoader;
using UnityEditor.Rendering.LookDev;

namespace Pogo.CustomMaps.Entities
{
    public abstract class WrappedCreatedEntity
    {
        protected abstract string ClassName { get; }
        protected uint SpawnFlags;
        protected BSPLoader.EntityCreatedCallbackData Data;

        public WrappedCreatedEntity(BSPLoader.EntityCreatedCallbackData data)
        {
            if (data.Instance.entity.ClassName != ClassName)
            {
                throw new ArgumentException($"Tried to spawn WrappedEntity {GetType()} but got unexpected ClassName {Data.Instance.entity.ClassName}");
            }
            Data = data;
            SpawnFlags = Data.Instance.entity.Spawnflags;
        }

        public bool GetSpawnFlag(uint flagValue) => (SpawnFlags & flagValue) != 0;

        public EntityInstance GetSingleTarget(string key = "target")
        {
            string targetName = Data.Instance.entity[key];
            var targets = Data.Context.GetNamedEntities(targetName);
            if (targets.Count == 1)
            {
                return targets[0];
            }
            else if (targets.Count == 0)
            {
                throw new FormatException($"{Data.Instance.entity.ClassName} named {Data.Instance.entity.Name} couldn't find target named {Data.TargetName}");
            }
            else
            {
                throw new FormatException($"{Data.Instance.entity.ClassName} named {Data.Instance.entity.Name} found multiple targets named {Data.TargetName}. expected only 1");
            }
        }
    }
}
