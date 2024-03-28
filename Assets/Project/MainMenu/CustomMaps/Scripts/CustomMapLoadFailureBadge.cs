using Pogo.CustomMaps.Indexing;
using TMPro;
using UnityEngine;

namespace Pogo.CustomMaps.UI
{
    public class CustomMapLoadFailureBadge : MonoBehaviour
    {
        public TextMeshProUGUI HeaderText;
        public TextMeshProUGUI MessageText;

        private GenerateMapHeaderResult result;
        public GenerateMapHeaderResult Result
        {
            get => result;
            set
            {
                result = value;
                ResultChanged();
            }
        }

        private void ResultChanged()
        {
            string mapName = MapHeaderHelper.GetMapName(Result.LoadData.FolderPath);
            HeaderText.text = $"Error Loading {mapName} ({Result.LoadData.Source})";
            MessageText.text = result.GetErrorText();
        }
    }
}
