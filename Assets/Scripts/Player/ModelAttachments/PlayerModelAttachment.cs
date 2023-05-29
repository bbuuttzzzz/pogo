using Players.Visuals.ModelAttachments;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Players.Visuals
{
    public class PlayerModelAttachment : MonoBehaviour, IPlayerModelAttachment
    {
        public PlayerModelAttachPoints AttachPoint;
        PlayerModelAttachPoints IPlayerAttachPointSnappable.AttachPoint => AttachPoint;

        string IPlayerModelAttachment.Name => name;

        public UnityEvent<PlayerController> OnAttach;

        void IPlayerModelAttachment.OnAttach(PlayerController parent)
        {
            OnAttach?.Invoke(parent);
        }

        void IPlayerAttachPointSnappable.SnapToTransform(Transform target)
        {
            transform.position = target.position;
            transform.rotation = target.rotation;
        }
    }
}