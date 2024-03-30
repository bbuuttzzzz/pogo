using BSPImporter;
using Pogo.CustomMaps.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static BSPImporter.BSPLoader;

namespace Pogo.CustomMaps.Entities
{
    public class WrappedEntityInstance
    {
        const string Key_RenderStyle = "renderstyle";
        const string Key_Convex = "convex";

        protected string ClassName { get; private set; }
        protected uint SpawnFlags;
        public EntityInstance Instance;
        private IBSPLoaderContext Context;
        public GameObject InstanceGameObject => Instance.gameObject;


        public enum RenderStyles
        {
            Default = 0,
            Invisible = 1,
            UseMapTexture = 2
        }

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
                throw new MapErrorException(new EntityTargetError(null, Instance, key, targetName, "Couldn't find any targets with that name"));
            }
            else
            {
                throw new MapErrorException(new EntityTargetError(null, Instance, key, targetName, $"Found Duplicate targets with that name (total {targets.Count})"));
            }
        }

        public bool GetIsConvex() => GetBoolOrDefault(Key_Convex, true);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <param name="minValue">The minimum allowed value, inclusive</param>
        /// <param name="maxValue">The maximum allowed value, inclusive</param>
        /// <returns></returns>
        /// <exception cref="MapErrorException"></exception>
        public int GetIntOrDefault(string key, int defaultValue, int minValue = int.MinValue, int maxValue = int.MaxValue)
        {
            if (!TryGetValue(key, out string value))
            {
                return defaultValue;
            }

            if (!int.TryParse(value, out var intValue))
            {
                throw new MapErrorException(new EntityBadFormatError(null, Instance, key, $"Failed to parse '{value}' as integer"));
            }

            if (intValue < minValue || intValue > maxValue)
            {
                throw new MapErrorException(new EntityBadFormatError(null, Instance, key, $"Value '{intValue}' out of expected range {minValue}-{maxValue}"));
            }

            return intValue;
        }

        public bool GetBoolOrDefault(string key, bool defaultValue) => GetIntOrDefault(key, defaultValue ? 1 : 0, 0, 1) > 0;

        public string GetValueOrDefault(string key, string defaultValue)
        {
            if (TryGetValue(key, out string value)) return value;
            return defaultValue;
        }

        public string GetValue(string key)
        {
            if (!Instance.entity.TryGetValue(key, out string value))
            {
                throw new MapErrorException(new EntityMissingPropertyError(null, Instance, key));
            }

            return value;
        }

        public bool TryGetValue(string key, out string defaultValue) => Instance.entity.TryGetValue(key, out defaultValue);

        public RenderStyles GetRenderStyle()
        {
            int key = GetIntOrDefault(Key_RenderStyle, 0, 0, (int)RenderStyles.UseMapTexture);
            return (RenderStyles)key;
        }
    }
}
