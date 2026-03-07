using NKVDebugMod.Features.UI;
using NKVDebugMod.Features.UI.Controls.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NKVDebugMod.Features.SaveSlotsManager.UI {
    internal class SaveManagerUI {
        private ModWindow _window;
        private Vector2 _scrollPosition = Vector2.zero;
        private string? _renamingSave = null;
        private string _renamedSaveName = string.Empty;
        private string _searchText = string.Empty;

        private StringField _newSaveNameField;
        private StringField _renameSaveField;
        private StringField _searchField;

        public List<SaveSlotListItem> SaveSlots { get; set; } = new();

        public event Action? OnSaveButtonClicked;
        public event Action<string>? OnLoadSaveClicked;
        public event Action<string>? OnDeleteClicked;
        public event Action<string, string>? OnRenameConfirmed;
        public event Action<string, SaveSlotsManager.SavesDisplayMode>? OnSearch;


        private bool _isEnabled;
        public bool IsEnabled {
            get => _isEnabled;
            set {
                _isEnabled = value;
                _window.IsVisible = _isEnabled;
            }
        }

        public SaveSlotsManager.SavesDisplayMode DisplayMode { get; set; } = SaveSlotsManager.SavesDisplayMode.OrderedByCreationTimeAsc;

        public string NewSaveName { get; private set; } = string.Empty;
        

        public SaveManagerUI() {
            _window = new(999, 0.5f, 0.6f, 0.2f, 0.5f, DrawWindow, "Save manager");

            _newSaveNameField = new StringField(string.Empty,() => _window.WindowSettingRect.width / 4, string.Empty, string.Empty);
            _newSaveNameField.FieldValueChanged += HandleNewSaveNameValueChange;

            _renameSaveField = new StringField(string.Empty, () => _window.WindowSettingRect.width / 4, string.Empty, string.Empty);
            _renameSaveField.FieldValueChanged += HandleRenameFieldChange;

            _searchField = new StringField(string.Empty, () => _window.WindowSettingRect.width / 4, string.Empty, string.Empty);
            _searchField.FieldValueChanged += HandleSearchFieldChange;
        }

        public void SetItems(IEnumerable<SaveSlotListItem> items) {
            SaveSlots = new(items);
        }

        private void HandleSearchFieldChange(string? value) {
            _searchText = value ?? string.Empty;
            OnSearch?.Invoke(_searchText, DisplayMode);
        }

        private void HandleDisplayModeChange(SaveSlotsManager.SavesDisplayMode mode) {
            DisplayMode = mode;
            OnSearch?.Invoke(_searchText, DisplayMode);
        }

        private void HandleRenameFieldChange(string? value) {
            if(!string.IsNullOrEmpty(value)) {
                _renamedSaveName = value;
            }
        }

        private void HandleNewSaveNameValueChange(string? value) {
            if (string.IsNullOrEmpty(value)) {
                return;
            }

            NewSaveName = value;
        }

        private void HandleRenameConfirm() {
            if(string.IsNullOrEmpty(_renamingSave)) {
                return;
            }

            OnRenameConfirmed?.Invoke(_renamingSave, _renamedSaveName);
            _renamingSave = null;
        }

        private void HandleRenameCancel() {
            _renamingSave = null;
        }

        private void DrawWindow(int id) {
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, false, true);
            {
                GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
                {
                    GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                    {
                        _newSaveNameField.Draw();
                        if (GUILayout.Button("Save")) {
                            OnSaveButtonClicked?.Invoke();
                        }
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                    {
                        GUILayout.Label("Display:", GUILayout.MaxWidth(_window.WindowSettingRect.width / 6));
                        if (DisplayMode == SaveSlotsManager.SavesDisplayMode.Pinned) GUI.enabled = false;
                        if (GUILayout.Button("Pinned", GUILayout.MaxWidth(_window.WindowSettingRect.width / 6))) {
                            HandleDisplayModeChange(SaveSlotsManager.SavesDisplayMode.Pinned);
                        }
                        GUI.enabled = true;
                        if (DisplayMode == SaveSlotsManager.SavesDisplayMode.OrderedByCreationTimeDesc) GUI.enabled = false;
                        if (GUILayout.Button("Creation time ↓", GUILayout.MaxWidth(_window.WindowSettingRect.width / 6))) {
                            HandleDisplayModeChange(SaveSlotsManager.SavesDisplayMode.OrderedByCreationTimeDesc);
                        }
                        GUI.enabled = true;
                        if (DisplayMode == SaveSlotsManager.SavesDisplayMode.OrderedByCreationTimeAsc) GUI.enabled = false;
                        if (GUILayout.Button("Creation time ↑", GUILayout.MaxWidth(_window.WindowSettingRect.width / 6))) {
                            HandleDisplayModeChange(SaveSlotsManager.SavesDisplayMode.OrderedByCreationTimeAsc);
                        }
                        GUI.enabled = true;
                        if (DisplayMode == SaveSlotsManager.SavesDisplayMode.CreatedDuringSession) GUI.enabled = false;
                        if (GUILayout.Button("Recently created", GUILayout.MaxWidth(_window.WindowSettingRect.width / 6))) {
                            HandleDisplayModeChange(SaveSlotsManager.SavesDisplayMode.CreatedDuringSession);
                        }
                        GUI.enabled = true;
                        if (DisplayMode == SaveSlotsManager.SavesDisplayMode.LatestUsedDuringSession) GUI.enabled = false;
                        if (GUILayout.Button("Last used", GUILayout.MaxWidth(_window.WindowSettingRect.width / 6))) {
                            HandleDisplayModeChange(SaveSlotsManager.SavesDisplayMode.LatestUsedDuringSession);
                        }
                        GUI.enabled = true;
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                    {
                        GUILayout.Label("Search:");
                        _searchField.Draw();
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();
                    if (SaveSlots.Count > 0) {
                        foreach (var saveSlot in SaveSlots) {
                            GUILayout.BeginHorizontal();
                            {
                                if (_renamingSave != saveSlot.Name) {
                                    if (GUILayout.Button("Pin")) {

                                    }

                                    GUILayout.Label(saveSlot.Name);

                                    GUILayout.FlexibleSpace();

                                    GUILayout.BeginHorizontal(GUILayout.MaxWidth(_window.WindowSettingRect.width / 2), GUILayout.ExpandWidth(true));
                                    {
                                        if (GUILayout.Button("Rename", GUILayout.MaxWidth(_window.WindowSettingRect.width / 2 / 3))) {
                                            _renamingSave = saveSlot.Name;
                                            _renameSaveField.SetValue(saveSlot.Name);
                                            _renamedSaveName = saveSlot.Name;
                                        }

                                        if (string.IsNullOrEmpty(_renamingSave)) {
                                            if (GUILayout.Button("Load", GUILayout.MaxWidth(_window.WindowSettingRect.width / 2 / 3))) {
                                                OnLoadSaveClicked?.Invoke(saveSlot.Name);
                                            }
                                        }

                                        if (GUILayout.Button("Delete", GUILayout.MaxWidth(_window.WindowSettingRect.width / 2 / 3))) {
                                            OnDeleteClicked?.Invoke(saveSlot.Name);
                                        }
                                    }
                                    GUILayout.EndHorizontal();
                                } else {
                                    _renameSaveField.Draw();
                                    GUILayout.BeginHorizontal(GUILayout.MaxWidth(_window.WindowSettingRect.width / 2), GUILayout.ExpandWidth(true));
                                    {
                                        if (GUILayout.Button("Save", GUILayout.MaxWidth(_window.WindowSettingRect.width / 2 / 2))) {
                                            HandleRenameConfirm();
                                        }
                                        if (GUILayout.Button("Cancel", GUILayout.MaxWidth(_window.WindowSettingRect.width / 2 / 2))) {
                                            HandleRenameCancel();
                                        }

                                    }
                                    GUILayout.EndHorizontal();
                                }
                            }
                            GUILayout.EndHorizontal();
                        }
                    } else {
                        GUILayout.Label("No saves");
                    }
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndScrollView();
        }

        public void Draw() {
            if(_isEnabled) {
                _window.Draw();
            }
        }
    }
}
