using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pogo
{
    public class GlobalSoundManager : MonoBehaviour
    {
        public GlobalSoundManifest manifest;

        Dictionary<GlobalSoundDescriptor, AudioSource> soundsDict = new Dictionary<GlobalSoundDescriptor, AudioSource>();

        public void Play(GlobalSoundDescriptor sound)
        {
            AudioSource obj;
            if (!soundsDict.TryGetValue(sound, out obj))
            {
                var newSoundObj = Instantiate(sound.Prefab, transform);
                obj = newSoundObj.GetComponent<AudioSource>();
                soundsDict.Add(sound, obj);
            }
            obj.Play();
        }
    }
}
