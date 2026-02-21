using System;
using System.Collections.Generic;
using System.Text;

namespace NKVDebugMod.Features.UI.Controls.Fields {
    internal class IntField : FieldBase<int> {
        public IntField(string label, float maxWidth, string? tooltip, int defaultValue) : base(label, maxWidth, tooltip, defaultValue) {
        }

        public IntField(string label, Func<float>? maxWidthFunc, string? tooltip, int defaultValue) : base(label, maxWidthFunc, tooltip, defaultValue) {
        }

        protected override string ConvertToString(int input) {
            return input.ToString();
        }

        protected override int ConvertToValue(string input) {
            if (int.TryParse(input, out int value)) {
                return value;
            } else {
                return 0;
            }
        }

        protected override bool Validate(string input) {
            if (!base.Validate(input)) return false;

            return int.TryParse(input, out int value);
        }
    }
}
