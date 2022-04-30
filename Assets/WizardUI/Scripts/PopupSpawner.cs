using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WizardUI
{
    public class PopupSpawner : MonoBehaviour
    {
        public GameObject Prefab;

        public void Spawn()
        {
            Debug.Log("Spawning Prefab");
            SpawnPrefab(Prefab);
        }

        public void SpawnPrefab(GameObject prefab)
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.SpawnUIElement(prefab);
            }
        }
    }
}
