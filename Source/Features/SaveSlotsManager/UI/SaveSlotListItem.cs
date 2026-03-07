using System;
using System.Collections.Generic;
using System.Text;

namespace NKVDebugMod.Features.SaveSlotsManager.UI {
    internal class SaveSlotListItem {
        public string Name { get; set; } = string.Empty;
        public bool IsPinned { get; set; } = false;

        public SaveSlotListItem(string name) {
            Name = name;
        }
    }
}
