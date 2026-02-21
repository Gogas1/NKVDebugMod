using System;
using System.Collections.Generic;
using System.Text;

namespace NKVDebugMod.Features.MonsterInspector.UI {
    internal class ModMonsterData {
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public MonsterBase Monster { get; set; }

        public ModMonsterData(MonsterBase monster) {
            Monster = monster;
        }

        public override bool Equals(object obj) {
            if (obj is ModMonsterData data) {
                return Name == data.Name && Path == data.Path && Monster == data.Monster;
            }

            return false;
        }

        public override int GetHashCode() {
            return HashCode.Combine(Name, Path);
        }
    }
}
