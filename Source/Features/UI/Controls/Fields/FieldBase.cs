using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace NKVDebugMod.Features.UI.Controls.Fields {
    internal abstract class FieldBase<T> : ILabeledField<T> {
        protected T? value;
        private readonly bool nonChangeable;
        private string _label = "";
        private string? _tooltip = null;
        private float maxWidth = 0;
        private Func<float>? maxWidthFunc;

        protected string oldText = "";


        public string Label {
            get => _label;
            set => _label = value;
        }

        public string? Tooltip {
            get => _tooltip;
            set => _tooltip = value;
        }

        public event Action<object?>? ValueChanged;
        public event Action<T?>? FieldValueChanged;

        protected virtual bool Validate(string input) {
            return !nonChangeable;
        }

        protected abstract T ConvertToValue(string input);
        protected abstract string ConvertToString(T? input);

        protected FieldBase(string label, float maxWidth, string? tooltip, T defaultValue, bool nonChangeable = false) {
            value = defaultValue;
            this.nonChangeable = nonChangeable;
            oldText = ConvertToString(value);
            _label = label;
            this.maxWidth = maxWidth;
            _tooltip = tooltip;
        }

        protected FieldBase(string label, Func<float>? maxWidthFunc, string? tooltip, T defaultValue, bool nonChangeable = false) {
            value = defaultValue;
            this.nonChangeable = nonChangeable;
            oldText = ConvertToString(value);
            Label = label;
            this.maxWidthFunc = maxWidthFunc;
            Tooltip = tooltip;
        }

        public void Draw() {
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));

            if (!string.IsNullOrEmpty(_label)) {
                GUILayout.Label(new GUIContent(_label, _tooltip), GetLabelOptions());
            }

            string newText = GUILayout.TextField(oldText, GetFieldOptions());

            if (newText != oldText) {
                UpdateValueIfValid(newText);
            }

            GUILayout.EndHorizontal();
        }

        protected void UpdateValueIfValid(string input) {
            if (Validate(input)) {
                value = ConvertToValue(input);
                ValueChanged?.Invoke(value);
                FieldValueChanged?.Invoke(value);
                oldText = ConvertToString(value);
            }
        }


        public void AddValueChangeHandler(Action<object?> handler) {
            ValueChanged += handler;
        }

        public void AddValueChangeHandler(Action<T?> handler) {
            FieldValueChanged += handler;
        }

        public void SetLabel(string label) {
            _label = label;
        }

        public void SetTooltip(string tooltip) {
            _tooltip = tooltip;
        }

        public void SetMaxWidth(float maxWidth) {
            this.maxWidth = maxWidth;
        }
        public void SetMaxWidthFunction(Func<float> func) {
            maxWidthFunc = func;
        }
        
        public void SetValue(T newValue) {
            value = newValue;
            oldText = ConvertToString(value);
        }

        protected GUILayoutOption[] GetLabelOptions() {
            List<GUILayoutOption> options = new List<GUILayoutOption>{ GUILayout.ExpandWidth(true) };

            var maxWidth = maxWidthFunc?.Invoke() ?? this.maxWidth;

            float halfMaxWidth = maxWidth / 2;
            options.Add(GUILayout.MaxWidth(halfMaxWidth));

            return options.ToArray();
        }

        protected GUILayoutOption[] GetFieldOptions() {
            List<GUILayoutOption> options = new List<GUILayoutOption> { GUILayout.ExpandWidth(true) };

            var maxWidth = maxWidthFunc?.Invoke() ?? this.maxWidth;

            float fieldMaxWidth = string.IsNullOrEmpty(_label) ? maxWidth : maxWidth / 2;
            options.Add(GUILayout.MaxWidth(fieldMaxWidth));

            return options.ToArray();
        }

    }
}
