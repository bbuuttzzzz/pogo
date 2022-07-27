using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pogo
{
    public class AtmosphereSetter : MonoBehaviour
    {
        public bool SetOnAwake;
        public GameObject PostProcessingPrefab;

        private void Start()
        {
            if (SetOnAwake) SetAtmosphere();
        }

        public void SetAtmosphere()
        {
            PogoGameManager.PogoInstance.ForceAtmosphere(PostProcessingPrefab, true);
        }
    }
}
