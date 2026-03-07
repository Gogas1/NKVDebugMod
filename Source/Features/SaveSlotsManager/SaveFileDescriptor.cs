using System;
using System.Collections.Generic;
using System.Text;

namespace NKVDebugMod.Features.SaveSlotsManager {
    internal record SaveFileDescriptor(string Name, string FileName, DateTime CreatedAt, DateTime LastTimeUsed) {
        public string Name { get; set; } = Name;
        public string FileName { get; set; } = FileName;
        public DateTime CreatedAt { get; set; } = CreatedAt;
        public DateTime LastTimeUsed { get; set; } = LastTimeUsed;

    }
}
