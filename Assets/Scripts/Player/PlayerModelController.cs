using Players.Visuals;
using Players.Visuals.ModelAttachments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Player
{
    public class PlayerModelController : MonoBehaviour
    {
        List<IPlayerModelAttachment> Attachments = new List<IPlayerModelAttachment>();

        public Transform HeadBone;
        public Transform BackBone;
        private PlayerController Parent;

        public UnityEvent OnLoadAsDisplayModel;

        public UnityEvent OnDisjoint;

        private void Awake()
        {
            Parent = GetComponent<PlayerController>();
        }
        private void Update()
        {
            UpdateAttachments();
        }

        private void OnDestroy()
        {
            if (Parent != null)
            {
                UnLink();
            }
        }

        public void Link(PlayerController parent)
        {
            if (Parent != null)
            {
                UnLink();
            }
            Parent = parent;
            Parent.OnDisjoint.AddListener(Parent_OnDisjoint);
        }

        private void UnLink()
        {
            Parent.OnDisjoint.RemoveListener(Parent_OnDisjoint);
        }

        private void Parent_OnDisjoint()
        {
            OnDisjoint?.Invoke();
        }

        

        #region Model Attachments
        private void UpdateAttachments()
        {
            for (int attachmentIndex = 0; attachmentIndex < Attachments.Count; attachmentIndex++)
            {
                IPlayerModelAttachment attachment = Attachments[attachmentIndex];
                try
                {
                    ApplyAttachmentOnce(attachment);
                }
                catch (MissingReferenceException)
                {
#if DEBUG
                    _AttachmentDetailData data = _AttachmentDetails[attachmentIndex];
                    Debug.LogError($"Tried to update DESTROYED attachment named {data.Name} @ {data.AttachPoint}", this);
#else
                    Debug.LogError($"Tried to update DESTROYED attachment", this);
#endif
                }
            }
        }


        public void AddAttachment(IPlayerModelAttachment attachment)
        {
            if (attachment == null) throw new NullReferenceException();

            Attachments.Add(attachment);
            attachment.OnAttach();
#if DEBUG
            _AddAttachmentDetails(attachment);
#endif
        }

        public void RemoveAttachment(IPlayerModelAttachment attachment)
        {
            if (attachment == null) throw new NullReferenceException();

            attachment.OnDetach();
#if DEBUG
            int index = Attachments.IndexOf(attachment);
            if (index != -1)
            {
                _RemoveAttachmentDetailsAt(index);
                Attachments.RemoveAt(index);
            }
            else
            {
                var data = new _AttachmentDetailData(attachment);
                Debug.LogWarning($"Tried to remove an un-added IPlayerModelAttachment {data.Name} @ {data.AttachPoint}");
            }
#else
            Attachments.Remove(attachment);
#endif
        }

        public bool HasAttachment(IPlayerModelAttachment attachment) => Attachments.Contains(attachment);

        public void ApplyAttachmentOnce(IPlayerAttachPointSnappable attachment)
        {
            switch (attachment.AttachPoint)
            {
                case PlayerModelAttachPoints.Transform:
                    UpdateTransformAttachment(attachment, transform);
                    break;
                case PlayerModelAttachPoints.Head:
                    UpdateTransformAttachment(attachment, HeadBone);
                    break;
                case PlayerModelAttachPoints.Back:
                    UpdateTransformAttachment(attachment, BackBone);
                    break;
                case PlayerModelAttachPoints.Custom:
                default:
                    break;
            }
        }

        private void UpdateTransformAttachment(IPlayerAttachPointSnappable attachment, Transform bone)
        {
            attachment.SnapToTransform(bone);
        }

#if DEBUG
        private struct _AttachmentDetailData
        {
            public string Name;
            public PlayerModelAttachPoints AttachPoint;
            public _AttachmentDetailData(IPlayerModelAttachment attachment)
            {
                if (attachment is PlayerModelAttachment playerModelAttachment)
                {
                    Name = playerModelAttachment.name;
                }
                else
                {
                    Name = attachment.Name;
                }
                AttachPoint = attachment.AttachPoint;
            }
        }
        List<_AttachmentDetailData> _AttachmentDetails = new List<_AttachmentDetailData>();

        private void _AddAttachmentDetails(IPlayerModelAttachment attachment)
        {
            _AttachmentDetails.Add(new _AttachmentDetailData(attachment));
        }
        private void _RemoveAttachmentDetailsAt(int index)
        {
            _AttachmentDetails.RemoveAt(index);
        }

#endif
        #endregion

    }
}
