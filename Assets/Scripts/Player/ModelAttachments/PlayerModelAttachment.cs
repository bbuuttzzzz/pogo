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

        public UnityEvent OnAttach;
        public UnityEvent OnDetach;

        void IPlayerModelAttachment.OnAttach()
        {
            OnAttach?.Invoke();
        }

        void IPlayerModelAttachment.OnDetach()
        {
            OnDetach?.Invoke();
        }

        void IPlayerAttachPointSnappable.SnapToTransform(Transform target)
        {
            transform.position = target.position;
            transform.rotation = target.rotation;
        }
    }
}