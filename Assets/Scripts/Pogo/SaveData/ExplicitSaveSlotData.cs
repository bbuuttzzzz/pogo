using UnityEngine;

namespace Pogo.Saving
{
    [CreateAssetMenu(fileName = "saveSlotData_", menuName = "Pogo/SaveSlotDataExplicit", order = 1)]
    public class ExplicitSaveSlotData : ScriptableObject
    {
        public SaveSlotData Data;
    }
}
