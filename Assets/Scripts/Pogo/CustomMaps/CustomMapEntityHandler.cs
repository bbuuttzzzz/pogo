using BSPImporter;
using System;
using UnityEngine;

namespace Pogo.CustomMaps
{
    public class CustomMapEntityHandler
    {
        public string ClassName { get; private set; }
        public Action<BSPLoader.EntityCreatedCallbackData> SetupAction { get; private set; }
        public GameObject Prefab { get; private set; }

        public CustomMapEntityHandler(
            string entityName,
            Action<BSPLoader.EntityCreatedCallbackData> setupAction,
            GameObject prefab = null)
        {
            ClassName = entityName;
            SetupAction = setupAction;
            Prefab = prefab;
        }
    }
}
