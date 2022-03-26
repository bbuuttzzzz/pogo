using UnityEngine;

namespace Pogo
{
    public class ChapterUnlocker : MonoBehaviour
    {
        public ChapterDescriptor Chapter;

        public void SetUnlocked(bool newValue)
        {
            Chapter.IsUnlocked = newValue;
        }
    }
}
