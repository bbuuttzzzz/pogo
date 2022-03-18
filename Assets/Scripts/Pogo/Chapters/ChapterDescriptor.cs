using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pogo
{
    [CreateAssetMenu(fileName = "ChapterDescriptor", menuName = "ScriptableObjects/ChapterDescriptor", order = 1)]
    public class ChapterDescriptor : ScriptableObject
    {
        public LevelDescriptor Level;

        public GameObject FindStartPoint()
        {
            var startPoints = FindObjectsOfType(typeof(ChapterStartPoint)) as ChapterStartPoint[];
            foreach(var startPoint in startPoints)
            {
                if (startPoint.Chapter == this)
                {
                    return startPoint.gameObject;
                }
            }

            throw new MissingReferenceException($"Could not find ChapterStartPoint for ChapterDescriptor {name}");
        }
    }
}