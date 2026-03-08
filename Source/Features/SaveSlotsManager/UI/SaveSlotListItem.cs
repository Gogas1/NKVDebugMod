using System;
using System.Collections.Generic;
using System.Text;

namespace NKVDebugMod.Features.SaveSlotsManager.UI {
    internal class SaveSlotListItem {
        public string Name { get; set; } = string.Empty;
        public bool IsPinned { get; set; } = false;
        public DateTime CreatedAt { get; set; }

        public SaveSlotListItem(string name, DateTime createdAt, bool isPinned = false) {
            Name = name;
            CreatedAt = createdAt;
            IsPinned = isPinned;
        }
    }
}
