using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Players.Visuals.ModelAttachments
{
    public interface IPlayerAttachPointSnappable
    {
        public PlayerModelAttachPoints AttachPoint { get; }

        public void SnapToTransform(Transform target);
    }
}
