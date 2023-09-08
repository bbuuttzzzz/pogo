using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pogo.Saving
{
    public class QuickLoadBoxController : MonoBehaviour
    {
        public Image IconImage;
        public Button Button;

        public void SetData(ChapterDescriptor chapter)
        {
            IconImage.sprite = chapter.Icon;
        }
    }
}
