using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WizardUI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TmproLinkForwarder : MonoBehaviour, IPointerClickHandler
    {
        public LinkPair[] Links;

        public void OnPointerClick(PointerEventData eventData)
        {
            TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();

            if (TryGetTextLink(text, eventData, out string id))
            {
                string link = Links.Where(l => l.Id == id)
                    .Select(l => l.Link)
                    .FirstOrDefault();

                if (link == null)
                {
                    throw new KeyNotFoundException($"Couldn't find link for linkId {id}");
                }

                Application.OpenURL(link);
            }
        }

        private static bool TryGetTextLink(TextMeshProUGUI textMesh, PointerEventData eventData, out string result)
        {
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(textMesh, eventData.position, eventData.pressEventCamera);
            if (linkIndex == -1)
            {
                result = null;
                return false;
            }

            TMP_LinkInfo linkInfo = textMesh.textInfo.linkInfo[linkIndex];
            result = linkInfo.GetLinkID();
            return true;
        }



        [System.Serializable]
        public struct LinkPair
        {
            public string Id;
            public string Link;
        }
    }
}