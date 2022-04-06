using UnityEngine;

namespace Pogo
{
    public class DifficultyMaterializer : MonoBehaviour
    {
        public Renderer Target;
        public int MaterialIndex = 0;
        public Material HardModeMaterial;
        public Material HardcoreModeMaterial;

        private void Start()
        {
            if (PogoGameManager.GameInstanceIsValid())
            {
                var difficulty = PogoGameManager.PogoInstance.CurrentDifficulty;

                if (difficulty == PogoGameManager.Difficulty.Hard)
                {
                    var mats = Target.materials;
                    mats[MaterialIndex] = HardModeMaterial;
                    Target.materials = mats;
                }
                else if (difficulty == PogoGameManager.Difficulty.Freeplay)
                {
                    var mats = Target.materials;
                    mats[MaterialIndex] = HardcoreModeMaterial;
                    Target.materials = mats;
                }
            }
            
        }
    }
}
