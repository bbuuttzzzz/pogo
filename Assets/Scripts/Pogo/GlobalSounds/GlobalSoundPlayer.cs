using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pogo
{
    public class GlobalSoundPlayer : MonoBehaviour
    {
        public GlobalSoundDescriptor Sound;
        public void Play()
        {
            PogoGameManager.PogoInstance.PlayGlobalSound(Sound);
        }

        public void PlayOneShot(GlobalSoundDescriptor sound)
        {
            PogoGameManager.PogoInstance.PlayGlobalSound(sound);
        }
    }
}
