using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine;

namespace WizardUtils.Equipment.Inspector
{
    [CustomEditor(typeof(Equipper))]
    public class EquipperEditor : Editor
    {
        Equipper self;

        public override void OnInspectorGUI()
        {
            self = target as Equipper;
            DrawEquipmentSlots();
            DrawEquipment();

            DrawDefaultInspector();
        }

        private void DrawEquipDropdown()
        {
            if (EditorGUILayout.DropdownButton(new GUIContent("Equip Item..."), FocusType.Passive))
            {
                GenericMenu menu = new GenericMenu();

                var equipments = AssetDatabase.FindAssets($"t:{nameof(EquipmentDescriptor)}")
                    .Select(id => AssetDatabase.LoadAssetAtPath<EquipmentDescriptor>(AssetDatabase.GUIDToAssetPath(id)));
                foreach (var equipment in equipments)
                {
                    // skip equipment this can't use 
                    EquipmentSlot slot = self.FindSlot(equipment.SlotType);
                    if (slot == null) continue;

                    menu.AddItem(new GUIContent($"{equipment.SlotType.name}/{equipment.name}"), false, () =>
                    {
                        Undo.SetCurrentGroupName("Change Equipment");
                        int undoGroup = Undo.GetCurrentGroup();

                        self.EquipInEditor(equipment);
                        Undo.RecordObject(self, "Change Equipment");
                        PrefabUtility.RecordPrefabInstancePropertyModifications(self);

                        Undo.CollapseUndoOperations(undoGroup);
                    });
                }
                menu.DropDown(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 0f, 0f));
            }
        }


        private void DrawAddSlotDropdown()
        {
            if (EditorGUILayout.DropdownButton(new GUIContent("Add Slot..."), FocusType.Passive))
            {
                GenericMenu menu = new GenericMenu();

                var equipmentTypes = AssetDatabase.FindAssets($"t:{nameof(EquipmentTypeDescriptor)}")
                    .Select(id => AssetDatabase.LoadAssetAtPath<EquipmentTypeDescriptor>(AssetDatabase.GUIDToAssetPath(id)));
                foreach (var equipmentType in equipmentTypes)
                {
                    if (self.HasSlot(equipmentType)) continue;

                    menu.AddItem(new GUIContent($"{equipmentType.name}"), false, () =>
                    {
                        List<EquipmentSlot> newSlots = new List<EquipmentSlot>();
                        newSlots.AddRange(self.EquipmentSlots);
                        newSlots.Add(new EquipmentSlot(equipmentType));
                        self.EquipmentSlots = newSlots.ToArray();
                        
                        Undo.RecordObject(self, "Add Slot");
                        PrefabUtility.RecordPrefabInstancePropertyModifications(self);
                    });
                }
                menu.DropDown(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 0f, 0f));
            }
        }

        bool slotsToggle;
        private void DrawEquipmentSlots()
        {
            slotsToggle = EditorGUILayout.BeginFoldoutHeaderGroup(slotsToggle, "Equipment Slots");
            if (slotsToggle)
            {
                int slotIndexToDelete = -1;
                GUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Slot Type");
                    EditorGUILayout.LabelField("Parent Transform");
                }
                GUILayout.EndHorizontal();

                for (int i = 0; i < self.EquipmentSlots.Length; i++)
                {
                    EquipmentSlot slot = self.EquipmentSlots[i];
                    GUILayout.BeginHorizontal();
                    {
                        slot.EquipmentType = (EquipmentTypeDescriptor)EditorGUILayout.ObjectField(slot.EquipmentType, typeof(EquipmentTypeDescriptor), false);
                        slot.PrefabInstantiationParent = (Transform)EditorGUILayout.ObjectField(slot.PrefabInstantiationParent, typeof(Transform), true);
                        if (GUILayout.Button("x", GUILayout.ExpandWidth(false)))
                        {
                            // mark this slot to delete after we finish drawing the GUI
                            slotIndexToDelete = i;
                        }
                    }
                    GUILayout.EndHorizontal();
                    if (slot.PrefabInstantiationParent == null)
                    {
                        EditorGUILayout.LabelField("Set a Prefab Parent", EditorStyles.helpBox);
                    }
                }
                EditorGUILayout.Space();

                if (slotIndexToDelete >= 0)
                {
                    self.UnEquipInEditor(self.EquipmentSlots[slotIndexToDelete]);
                    ArrayHelper.DeleteAndResize(ref self.EquipmentSlots, slotIndexToDelete);
                    Undo.RecordObject(self, "Delete Equip Slot");
                    PrefabUtility.RecordPrefabInstancePropertyModifications(self);
                }

                DrawAddSlotDropdown();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            EditorGUILayout.Space();
        }

        bool equipmentToggle;

        private void DrawEquipment()
        {
            EditorGUILayout.LabelField("Equipment", EditorStyles.boldLabel);
            for (int i = 0; i < self.EquipmentSlots.Length; i++)
            {
                EquipmentSlot slot = self.EquipmentSlots[i];
                GUILayout.BeginHorizontal();
                {
                    var newEquip = (EquipmentDescriptor)EditorGUILayout.ObjectField(slot.Equipment, typeof(EquipmentDescriptor), false);
                    if (slot.ObjectInstance == null)
                    {
                        EditorGUI.BeginDisabledGroup(self.EquipmentSlots == null);
                        {
                            if (GUILayout.Button("Apply Equip"))
                            {
                                if (slot.Equipment != null)
                                {
                                    self.DeApplySlotInEditor(slot);
                                }
                                slot.Equipment = newEquip;
                                self.ApplySlotInEditor(slot);

                                Undo.RecordObject(self, "Apply Equipment");
                                PrefabUtility.RecordPrefabInstancePropertyModifications(self);
                            }
                        }
                        EditorGUI.EndDisabledGroup();
                    }
                    else
                    {
                        EditorGUILayout.LabelField(slot.ObjectInstance.name);

                        if (GUILayout.Button("x", GUILayout.ExpandWidth(false)))
                        {
                            self.DeApplySlotInEditor(slot);

                            Undo.RecordObject(self, "DeApply Equipment");
                            PrefabUtility.RecordPrefabInstancePropertyModifications(self);
                        }
                    }

                    if (newEquip != slot.Equipment)
                    {
                        self.EquipInEditor(newEquip);

                        Undo.RecordObject(self, "Change Equipment");
                        PrefabUtility.RecordPrefabInstancePropertyModifications(self);
                    }
                }
                GUILayout.EndHorizontal();
            }
        }

    }
}