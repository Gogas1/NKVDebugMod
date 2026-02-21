using NKVDebugMod.Features.TimeControl.Configuration;
using NKVDebugMod.Features.TimeControl.UI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static NKVDebugMod.Features.TimeControl.Configuration.TimeControllerConstants;

namespace NKVDebugMod.Features.TimeControl {
    internal class TimeController : MonoBehaviour {
        private float _timeScale = 1.0f;
        private int _advanceFramesValue = ADVANCE_FRAMES_DEFAULT;
        private float _advanceTimeValue = ADVANCE_TIME_DEFAULT;

        private bool _isTimeScaled = false;
        private bool _isPaused = false;

        private float _timeToAdvance = 0;
        private int _framesToAdvance = 0;

        private float _timer;
        private int _framesTimer;

        private bool _isEnabled;

        private TimeControlUI? _timeControlWindow;

        internal static TimeController? Instance;

        public bool IsEnabled {
            get => _isEnabled;
            set {
                _isEnabled = value;
                if(_timeControlWindow != null) {
                    _timeControlWindow.IsUIEnabled = _isEnabled;
                }
                
            }
        }

        public bool IsPaused {
            get => _isPaused;
            set {
                _isPaused = value;
                if (_timeControlWindow != null) {
                    _timeControlWindow.IsPaused = _isPaused;
                }
            }
        }

        public bool IsTimeScaled {
            get => _isTimeScaled;
            set {
                _isTimeScaled = value;
                if (_timeControlWindow != null) {
                    _timeControlWindow.IsTimeScaled = _isTimeScaled;
                }
            }
        }

        private void Awake() {
            Instance = this;
            TimeControllerConfiguration.Init();

            _timeControlWindow = new();
            _timeControlWindow.OnPauseResume += ToggleTime;
            _timeControlWindow.OnAdvanceFrames += AdvanceFrames;
            _timeControlWindow.OnAdvanceTime += AdvanceTime;
            _timeControlWindow.OnAdvanceFramesValueChange += ChangeAdvanceFramesValue;
            _timeControlWindow.OnAdvanceTimeValueChange += ChangeAdvanceTimeValue;
            _timeControlWindow.OnTimeScaleToggle += ToggleTimeScale;
            _timeControlWindow.OnTimeScaleValueChange += ChangeTimeScale;

            TimeControllerConfiguration.OnToggleTimeControlInvoked += ToggleUI;
            TimeControllerConfiguration.OnResumePausedInvoked += ToggleTime;
            TimeControllerConfiguration.OnAdvanceFramesInvoked += AdvanceFrames;
            TimeControllerConfiguration.OnAdvanceTimeInvoked += AdvanceTime;
            TimeControllerConfiguration.OnToggleTimeScaleInvoked += ToggleTimeScale;
        }


        private bool _firstFrameSkipped;
        private void Update() {
            if (!IsEnabled) return;

            if(IsPaused) {
                if (_timer >= _timeToAdvance && _timeToAdvance != 0) {
                    _timer = 0;
                    _timeToAdvance = 0;
                } else if(_timer != 0 || _timeToAdvance != 0) {

                    if (_timer > 0) {
                        _timer += RCGTime.unscaledDeltaTime;
                    }
                    else {
                        _timer += float.Epsilon;
                    }

                    RCGTime.timeScale = 1;
                    return;
                }

                if (_framesTimer >= _framesToAdvance && _framesToAdvance != 0) {
                    _framesTimer = 0;
                    _framesToAdvance = 0;
                    _firstFrameSkipped = false;
                } else if(_framesTimer != 0 || _framesToAdvance != 0) {

                    if(!_firstFrameSkipped) {
                        _framesTimer += 1;
                    } else {
                        _firstFrameSkipped = true;
                    }
                    RCGTime.timeScale = 1;
                    return;
                }

                RCGTime.timeScale = 0;
                return;
            }

            if(IsTimeScaled) {
                if(PlayerInputBinder.Instance.IsAction) {
                    RCGTime.timeScale = _timeScale;
                }
            }
        }

        private void OnGUI() {
            _timeControlWindow?.Draw();
        }

        private void ChangeTimeScale(float value) {
            _timeScale = value;
        }

        private void ChangeAdvanceTimeValue(float value) {
            _advanceTimeValue = value;
        }

        private void ChangeAdvanceFramesValue(int value) {
            _advanceFramesValue = value;
        }

        private void ToggleTimeScale() {
            IsTimeScaled = !IsTimeScaled;
        }

        private void AdvanceTime() {
            if(IsPaused) {
                _timeToAdvance += _advanceTimeValue;
            }
        }

        private void AdvanceFrames() {
            if (IsPaused) {
                _framesToAdvance += _advanceFramesValue;
            }
        }

        private void ToggleTime() {
            IsPaused = !IsPaused;
        }

        private void ToggleUI() {
            IsEnabled = !IsEnabled;
        }

        private void OnDestroy() {
            TimeControllerConfiguration.OnToggleTimeControlInvoked -= ToggleUI;
            TimeControllerConfiguration.OnResumePausedInvoked -= ToggleTime;
            TimeControllerConfiguration.OnAdvanceFramesInvoked -= AdvanceFrames;
            TimeControllerConfiguration.OnAdvanceTimeInvoked -= AdvanceTime;
            TimeControllerConfiguration.OnToggleTimeScaleInvoked -= ToggleTimeScale;
        }
    }
}
