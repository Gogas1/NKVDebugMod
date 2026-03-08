using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NKVDebugMod.Features.UI.Controls {
    internal class ModSelectControl<T> where T : IComparable {

        public event Action<SelectOption?>? OnSelectChanged;

        private SelectOption? _selected;
        public SelectOption? Selected {
            get => _selected;
            private set {
                if(_selected == value) {
                    return;
                }

                _selected = value;
                OnSelectChanged?.Invoke(_selected);
                _selected?.OnSelect?.Invoke();
            }
        }
        private List<SelectOption> _options = new();

        private bool _showDropdown;
        private Vector2 _scrollPos;

        private const float OPTION_HEIGHT = 24f;
        private const float MAX_POPUP_HEIGHT = OPTION_HEIGHT * 9;


        public void SetSelection(T value) {
            Log.Warning(value);
            if(value == null) {
                Selected = null;
                return;
            }
            
            var targetOption = _options.FirstOrDefault(op => op.Value.Equals(value));
            if(targetOption == null || targetOption == Selected) {
                return;
            }

            Selected = targetOption;
        }

        public void DropSelection() {
            Selected = null;
        }

        public void AddOption(SelectOption option) {
            _options.Add(option);
        }

        public void SetOptions(IEnumerable<SelectOption> options) {
            _options.Clear();
            _options.AddRange(options);
        }

        public void Draw() {
            try {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(Selected == null ? "Not selected" : Selected.Text)) {
                    _showDropdown = !_showDropdown;
                }

                var buttonRect = GUILayoutUtility.GetLastRect();

                if (_showDropdown) {
                    var height = Mathf.Min(_options.Count * OPTION_HEIGHT, MAX_POPUP_HEIGHT);
                    var popupRect = new Rect(buttonRect.x, buttonRect.y + buttonRect.height, buttonRect.width, height);

                    {
                        GUI.Box(popupRect, GUIContent.none);

                        GUI.BeginGroup(popupRect);

                        var scrollViewRect = new Rect(0, 0, popupRect.width, popupRect.height);

                        float contentHeight = OPTION_HEIGHT * (1 + _options.Count);
                        var contentRect = new Rect(0, 0, popupRect.width - 16f, contentHeight);

                        _scrollPos = GUI.BeginScrollView(scrollViewRect, _scrollPos, contentRect);

                        var y = 0f;
                        var notSelRect = new Rect(0, y, popupRect.width, OPTION_HEIGHT);
                        if (GUI.Button(notSelRect, "Not selected")) {
                            Selected = null;
                            _showDropdown = false;
                            _scrollPos = Vector2.zero;
                        }
                        y += OPTION_HEIGHT;

                        for (int i = 0; i < _options.Count; i++) {
                            var option = _options[i];
                            var r = new Rect(0, y + i * OPTION_HEIGHT, popupRect.width, OPTION_HEIGHT);
                            if (GUI.Button(r, option.Text)) {
                                Selected = option;
                                _showDropdown = false;
                                _scrollPos = Vector2.zero;
                            }
                        }

                        GUI.EndScrollView();
                        GUI.EndGroup();
                    }

                    if (Event.current.type == EventType.MouseDown) {
                        Vector2 mouse = Event.current.mousePosition;
                        if (!popupRect.Contains(mouse) && !buttonRect.Contains(mouse)) {
                            _showDropdown = false;
                            Event.current.Use();
                        }
                    }
                }

                GUILayout.EndHorizontal();
            } catch (Exception ex) {
                Log.Exception(ex);
            }
            
        }

        internal record SelectOption(string Text, T Value, Action? OnSelect = null) {
            public string Text { get; private set; } = Text;
            public T Value { get; private set; } = Value;
            public Action? OnSelect { get; private set; } = OnSelect;
        }
    }

    
}
