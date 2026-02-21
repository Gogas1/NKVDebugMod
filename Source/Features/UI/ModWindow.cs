using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace NKVDebugMod.Features.UI {
    internal class ModWindow {
        private bool _isVisible;
        public bool IsVisible {
            get => _isVisible;
            set {
                _isVisible = value;

                if(_isVisible) {
                    CalculateWindowRect();
                }
            }
        }

        private float _verticalRatio;
        private float _horizontalRatio;

        private float _verticalOffsetRatio;
        private float _horizontalOffsetRatio;

        public int Id { get; private set; }
        public Rect WindowSettingRect { get; private set; }

        private GUI.WindowFunction _windowDrawFunction;
        private string _title;

        //public event Action<Rect>? OnRecalculate;

        public ModWindow(int id, float verticalRatio, float horizontalRatio, float offsetRatioX, float offsetRatioY, GUI.WindowFunction windowDrawFunction, string title) {
            _verticalRatio = Mathf.Min(1, verticalRatio);
            _horizontalRatio = Mathf.Min(1, horizontalRatio);
            _horizontalOffsetRatio = Mathf.Min(1, offsetRatioX);
            _verticalOffsetRatio = Mathf.Min(1, offsetRatioY);
            this.Id = id;
            _windowDrawFunction = windowDrawFunction;
            _title = title;
        }

        public void Draw() {
            if (IsVisible) {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                var background = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                background.SetPixel(0, 0, new Color(0.5f, 0.5f, 0.5f, 0.8f));
                background.Apply();
                GUI.Box(WindowSettingRect, GUIContent.none, new GUIStyle { normal = new GUIStyleState { background = background } });

                var rect = GUILayout.Window(Id, WindowSettingRect, _windowDrawFunction, _title);
            }
        }

        private void CalculateWindowRect() {
            var width = Screen.width * _horizontalRatio;
            var height = Screen.height * _verticalRatio;

            var offsetX = Mathf.RoundToInt((float)((Screen.width) * _horizontalOffsetRatio));
            var offsetY = Mathf.RoundToInt((float)((Screen.height - height) * _verticalOffsetRatio));

            WindowSettingRect = new Rect(offsetX, offsetY, width, height);
            //OnRecalculate?.Invoke(WindowSettingRect);
        }
    }
}
