using NKVDebugMod.Features.UI;
using NKVDebugMod.Features.UI.Controls;
using NKVDebugMod.Features.UI.Controls.Fields;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static NKVDebugMod.Features.TimeControl.Configuration.TimeControllerConstants;

namespace NKVDebugMod.Features.TimeControl.UI {
    internal class TimeControlUI {
        private ModWindow _timeControlWindow;

        private ModButton _pauseResumeButton;
        private ModButton _toggleTimeScale;
        private ModButton _advanceFramesButton;
        private ModButton _advanceTimeButton;

        private FloatField _timeScaleValueField;
        private FloatField _advanceTimeValueField;
        private IntField _advanceFramesValueField;

        public event Action? OnPauseResume;
        public event Action? OnTimeScaleToggle;
        public event Action? OnAdvanceFrames;
        public event Action? OnAdvanceTime;

        public event Action<int>? OnAdvanceFramesValueChange;
        public event Action<float>? OnAdvanceTimeValueChange;
        public event Action<float>? OnTimeScaleValueChange;

        private bool isPaused;
        public bool IsPaused {
            get => isPaused;
            set {
                isPaused = value;
                if (isPaused) {
                    _pauseResumeButton.SetText("Resume");
                }
                else {
                    _pauseResumeButton.SetText("Play");
                }
            }
        }

        public bool IsTimeScaled { get; set; }
        
        private bool _isUIEnabled;
        public bool IsUIEnabled {
            get => _isUIEnabled;
            set {
                _isUIEnabled = value;
                _timeControlWindow.IsVisible = _isUIEnabled;
            }
        }

        public TimeControlUI() {
            _timeControlWindow = new(898, 0.2f, 0.1f, 0f, 0.5f, DrawTimeControlFunction, "Time controller");

            _pauseResumeButton = new ModButton("Pause", () => OnPauseResume?.Invoke());
            _toggleTimeScale = new ModButton("Time scale", () => OnTimeScaleToggle?.Invoke());
            _advanceFramesButton = new ModButton("Advance frames", () => OnAdvanceFrames?.Invoke());
            _advanceTimeButton = new ModButton("Advance time", () => OnAdvanceTime?.Invoke());

            _timeScaleValueField = new("Time scale", () => _timeControlWindow.WindowSettingRect.width, "Game time scale", 1f);
            _timeScaleValueField.FieldValueChanged += f => OnTimeScaleValueChange?.Invoke(f);

            _advanceTimeValueField = new("Advance time seconds", () => _timeControlWindow.WindowSettingRect.width, "How many seconds will be advanced on call", ADVANCE_TIME_DEFAULT);
            _advanceTimeValueField.FieldValueChanged += f => OnAdvanceTimeValueChange?.Invoke(f);

            _advanceFramesValueField = new("Advance time frames", () => _timeControlWindow.WindowSettingRect.width, "How many frames will be advanced on call", ADVANCE_FRAMES_DEFAULT);
            _advanceFramesValueField.FieldValueChanged += i => OnAdvanceFramesValueChange?.Invoke(i);
        }

        public void Draw() {
            if(IsUIEnabled) {
                _timeControlWindow.Draw();
            }
        }

        private void DrawTimeControlFunction(int id) {
            GUILayout.BeginVertical();
            {
                _pauseResumeButton.Draw();
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label(IsTimeScaled ? "Scaled" : "Not scaled");
                    _toggleTimeScale.Draw();
                }
                GUILayout.EndHorizontal();
                _timeScaleValueField.Draw();
                _advanceFramesButton.Draw();
                _advanceFramesValueField.Draw();
                _advanceTimeButton.Draw();
                _advanceTimeValueField.Draw();
            }
            GUILayout.EndVertical();
        }
    }
}
