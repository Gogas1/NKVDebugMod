using System;
using System.Collections.Generic;
using System.Text;

namespace NKVDebugMod.Features.UI.Controls.Fields {
    internal class StringField : FieldBase<string> {
        public StringField(string label, float maxWidth, string? tooltip, string defaultValue, bool nonChangeable = false) : base(label, maxWidth, tooltip, defaultValue, nonChangeable) {
        }

        public StringField(string label, Func<float>? maxWidthFunc, string? tooltip, string defaultValue, bool nonChangeable = false) : base(label, maxWidthFunc, tooltip, defaultValue, nonChangeable) {
        }

        protected override string ConvertToString(string? input) {
            return input ?? string.Empty;
        }

        protected override string ConvertToValue(string input) {
            return input;
        }
    }
}
