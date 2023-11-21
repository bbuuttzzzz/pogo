using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardUtils.Equipment
{
    public interface IEquipmentRoot
    {
        public void OnEquipped();
        public void OnUnequipped();
    }
}
