using NineSolsAPI.Utils;
using NKVDebugMod.Features.UI;
using NKVDebugMod.Features.UI.Controls;
using NKVDebugMod.Features.UI.Controls.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NKVDebugMod.Features.MonsterInspector.UI {
    internal class MonsterInspectorUI {
        private ModWindow InspectorWindow = null!;
        private Vector2 _scrollPosition;
        private bool _isInspectorEnabled = false;
        public bool IsInspectorEnabled {
            get => _isInspectorEnabled;
            set {
                _isInspectorEnabled = value;
                InspectorWindow.IsVisible = value;
            }
        }

        private List<MonsterStateDescriptor> _monsterStates = new List<MonsterStateDescriptor>();

        private ModMonsterData? selectedMonsterData = null;
        public ModMonsterData? SelectedMonsterData {
            get {
                if(selectedMonsterData != null && selectedMonsterData.Monster == null) {
                    SelectedMonsterData = null;
                    return null;
                }

                return selectedMonsterData;
            }

            set {
                if (selectedMonsterData == value) return;

                if (selectedMonsterData != null && value != null && selectedMonsterData.Equals(value)) {
                    return;
                }

                selectedMonsterData = value;
                //_monsterSelect.SetSelection(selectedMonsterData?.Path ?? null);

                if (selectedMonsterData != null) {
                    _monsterNameField.SetValue(selectedMonsterData.Name);
                    _monsterPathField.SetValue(selectedMonsterData.Path);
                    _monsterIdField.SetValue(selectedMonsterData.Monster.MonsterID);
                }
            }
        }

        private ModButton _prevMonsterButton;
        private ModButton _nextMonsterButton;
        private ModButton _selectClosestMonsterButton;
        private StringField _monsterNameField;
        private StringField _monsterPathField;
        private StringField _monsterIdField;
        private ModSelectControl<MonsterBase.States> _monsterStateSelect;

        public event Action? OnPrevMonsterClick;
        public event Action? OnNextMonsterClick;
        public event Action? OnSelectClosestMonsterClick;
        public event Action<MonsterBase.States>? OnStateSelected;

        public MonsterInspectorUI() {
            InspectorWindow = new(897, 0.5f, 0.2f, 0.8f, 0.5f, DrawInspectorFunction, "Monster Inspector");

            _prevMonsterButton = new ModButton("←", () => OnPrevMonsterClick?.Invoke());
            _nextMonsterButton = new ModButton("→", () => OnNextMonsterClick?.Invoke());
            _selectClosestMonsterButton = new ModButton("Closest", () => OnSelectClosestMonsterClick?.Invoke());

            _monsterNameField = new StringField("Name:", () => InspectorWindow.WindowSettingRect.width, null, "No name", true);
            _monsterPathField = new StringField("Path:", () => InspectorWindow.WindowSettingRect.width, null, "No path", true);
            _monsterIdField = new StringField("Id:", () => InspectorWindow.WindowSettingRect.width, null, "No id", true);

            _monsterStateSelect = new();
            _monsterStateSelect.OnSelectChanged += HandleStateSelectChanged;
        }

        private void HandleStateSelectChanged(ModSelectControl<MonsterBase.States>.SelectOption? obj) {
            if (obj != null) {
                OnStateSelected?.Invoke(obj.Value);
            }
        }

        bool wasException = false;
        private void DrawInspectorFunction(int id) {
            try {
                GUILayout.BeginScrollView(_scrollPosition, false, true);

                GUILayout.BeginVertical(GUILayout.ExpandWidth(true));

                GUILayout.BeginHorizontal();
                GUILayout.Label(SelectedMonsterData == null ? "Not selected" : SelectedMonsterData.Name);
                if(NKVDebugMod.IsUnityExplorerPresent) {
                    if (SelectedMonsterData != null) {
                        if(GUILayout.Button("UE Inspect")) {
                            UnityExplorer.InspectorManager.Inspect(SelectedMonsterData.Monster);
                        }
                    }
                } else if(SelectedMonsterData != null) {
                    GUI.enabled = false;
                    GUILayout.Button(new GUIContent("UE Inspect", "Install Unity Explorer to use this feature"));
                    GUI.enabled = true;
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                _prevMonsterButton.Draw();
                _nextMonsterButton.Draw();
                GUILayout.EndHorizontal();
                _selectClosestMonsterButton.Draw();
                GUILayout.EndVertical();

                if(SelectedMonsterData != null) {
                    GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
                    _monsterNameField.Draw();
                    _monsterPathField.Draw();
                    _monsterIdField.Draw();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("State:");
                    GUILayout.Label($"{SelectedMonsterData.Monster.currentMonsterState.name} - {SelectedMonsterData.Monster.CurrentState.ToString()} ({(int)SelectedMonsterData.Monster.CurrentState})");                    
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("State timer:");
                    GUILayout.Label($"{SelectedMonsterData.Monster.currentMonsterState.statusTimer}");
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Select state:");
                    _monsterStateSelect.Draw();
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                } else {
                    GUILayout.Label("No monster selected");
                }

                GUILayout.EndScrollView();
            } catch (Exception ex) {
                if(!wasException) {
                    Log.Exception(ex);
                    wasException = true;
                }
            }
        }

        public void Draw() {
            if(_isInspectorEnabled) {
                InspectorWindow.Draw();
            }
        }

        public void SetMonsterStates(IEnumerable<MonsterStateDescriptor> monsterStates) {
            _monsterStates.Clear();
            _monsterStates.AddRange(monsterStates);

            _monsterStateSelect.DropSelection();
            _monsterStateSelect.SetOptions(_monsterStates.Select(state => new ModSelectControl<MonsterBase.States>.SelectOption(state.Name, state.State)));
        }

        //private void HandleWindowRecalculate(Rect rect) {
        //    _monsterNameField.SetMaxWidth(rect.width);
        //}

        internal record MonsterStateDescriptor(string Name, MonsterBase.States State) {
            public string Name = Name;
            public MonsterBase.States State = State;
        }
    }
}
