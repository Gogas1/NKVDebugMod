using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace NKVDebugMod.Features.UI.Controls {
    internal class ModButton {
        private bool _wasPressed;

        private string _text;
        public event Action? OnClick;
        public event Action? OnRelease;

        public ModButton(string text, Action? onClick = null, Action? onRelease = null) {
            _text = text;

            OnClick = onClick;
            OnRelease = onRelease;
        }

        public void SetText(string text) {
            _text = text;
        }


        public void Draw() {
            var pressed = GUILayout.Button(_text);
            if (pressed) {
                if(!_wasPressed) {
                    _wasPressed = true;
                    OnClick?.Invoke();
                }
            } else {
                if(_wasPressed) {
                    _wasPressed = false;
                    OnRelease?.Invoke();
                }
            }

        }
    }
}
