using UnityEditor;
using UnityEngine.UIElements;
using WizardUtils.ManifestPattern;

namespace Pogo.Collectibles
{
    [CustomEditor(typeof(CollectibleDescriptor))]
    public class CollectibleDescriptorEditor : Editor
    {
        CollectibleDescriptor self;

        DescriptorManifestAssigner<CollectibleManifest, CollectibleDescriptor> dropdown;

        public override VisualElement CreateInspectorGUI()
        {
            self = target as CollectibleDescriptor;
            dropdown = new DescriptorManifestAssigner<CollectibleManifest, CollectibleDescriptor>();
            return base.CreateInspectorGUI();
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            dropdown.DrawRegisterButtons(self);
        }
    }
}