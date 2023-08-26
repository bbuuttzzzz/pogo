using UnityEngine;

namespace Pogo
{
    public class DifficultyMaterializer : MonoBehaviour
    {
        public Renderer Target;
        public int MaterialIndex = 0;
        public Material HardModeMaterial;
        public Material ExpertModeMaterial;
        public Material FreeplayModeMaterial;

        private void Start()
        {
            if (PogoGameManager.GameInstanceIsValid())
            {
                var difficulty = PogoGameManager.PogoInstance.CurrentDifficulty;

                if (difficulty == PogoGameManager.Difficulty.Hard && HardModeMaterial != null)
                {
                    var mats = Target.materials;
                    mats[MaterialIndex] = HardModeMaterial;
                    Target.materials = mats;
                }
                else if (difficulty == PogoGameManager.Difficulty.Expert && ExpertModeMaterial != null)
                {
                    var mats = Target.materials;
                    mats[MaterialIndex] = ExpertModeMaterial;
                    Target.materials = mats;
                }
                else if (difficulty == PogoGameManager.Difficulty.Freeplay && FreeplayModeMaterial != null)
                {
                    var mats = Target.materials;
                    mats[MaterialIndex] = FreeplayModeMaterial;
                    Target.materials = mats;
                }
            }
            
        }
    }
}
