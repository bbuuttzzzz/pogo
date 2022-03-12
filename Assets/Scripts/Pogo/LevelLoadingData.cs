using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pogo
{
    public class LevelLoadingData
    {
        public List<AsyncOperation> LoadingSceneTasks;
        public List<AsyncOperation> UnloadingSceneTasks;

        public LevelLoadingData(List<AsyncOperation> loadingSceneTasks, List<AsyncOperation> unloadingSceneTasks)
        {
            LoadingSceneTasks = loadingSceneTasks;
            UnloadingSceneTasks = unloadingSceneTasks;
        }
    }
}
