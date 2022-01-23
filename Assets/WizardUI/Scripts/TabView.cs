using System;
using UnityEngine;
using UnityEngine.UI;

namespace WizardUI
{
    [AddComponentMenu("WizardUI/TabView")]
    public class TabView : WizardUIElement
    {
        const int TAB_NONE = -1;
        public int InitialTabIndex = -1;

        public int ActiveTabIndex
        {
            get
            {
                return activeTabIndex;
            }
            set
            {
                if (activeTabIndex != TAB_NONE)
                {
                    deactivateTab(value);
                }
                if (value != TAB_NONE)
                {
                    activateTab(value);
                }
                activeTabIndex = value;
            }
        }

        private void activateTab(int value)
        {
            throw new NotImplementedException();
        }

        private void deactivateTab(int value)
        {
            throw new NotImplementedException();
        }

        private int activeTabIndex;
        public TabViewItem[] Tabs;

        private void Awake()
        {
            activeTabIndex = InitialTabIndex;
            for(int n = 0; n < Tabs.Length; n++)
            {
                var item = Tabs[n];
                if (item.button == null)
                {
                    throw new MissingComponentException($"{item.Name} TabViewItem missing {nameof(item.button)} reference");
                }

                item.button.onClick.AddListener(() => onTabClicked(n));
            }
        }

        private void onTabClicked(int index)
        {
            ActiveTabIndex = index;
        }
    }
}
