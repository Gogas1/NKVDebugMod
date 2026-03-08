using Cysharp.Threading.Tasks;
using HarmonyLib;
using NKVDebugMod.Features.SaveSlotsManager.Configuration;
using NKVDebugMod.Features.SaveSlotsManager.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NKVDebugMod.Features.SaveSlotsManager {
    internal class SaveSlotsManager : MonoBehaviour {
        private Dictionary<string, SaveFileDescriptor> _savesCache = new();
        
        private static int _saveSlotIndex = 5;
        internal static bool IsSaveSlotLoadRequested = false;
        internal static string _rootPath = Application.persistentDataPath;
        internal static string _savesRoot = Path.Combine(_rootPath, "NKVDebug_saves");
        internal static string _debugSavePath = Path.Combine(_rootPath, $"saveslot{_saveSlotIndex}");
        internal static string _pinnedSavesFilePath = Path.Combine(_savesRoot, "pinned.txt");

        private object _lock = new object();
        private bool _saveOnPosition => SaveManagerConfiguration.SaveOnPosition;
        private bool _isPlaying => GameCore.IsPlayingReady();
        private string? _lastSave;
        private SaveManagerUI? _managerUi;

        private HashSet<string> _pinnedSaves = new();
        private HashSet<string> _createdDuringSession = new();
        private HashSet<string> _lastUsed = new();

        //https://stackoverflow.com/questions/62771/how-do-i-check-if-a-given-string-is-a-legal-valid-file-name-under-windows/62888#comment61988418_62888
        private readonly Regex _invalidFileNameRegex = new("^(?!^(?:PRN|AUX|CLOCK\\$|NUL|CON|COM\\d|LPT\\d)(?:\\..+)?$)(?:\\.*?(?!\\.))[^\\x00-\\x1f\\\\?*:\\\";|\\/<>]+(?<![\\s.])$");

        public static SaveSlotsManager? Instance { get; private set; }

        private static MethodInfo _readMetaInSaveMethod = AccessTools.Method(typeof(SaveManager), "ReadMetaInSave", [typeof(string), typeof(bool)]);

        private void Awake() {
            Instance = this;
            _debugSavePath = Path.Combine(_rootPath, $"saveslot{_saveSlotIndex}");
            EnsureDirectoriesPresent();
            _managerUi = new SaveManagerUI();
            _managerUi.OnLoadSaveClicked += HandleLoadSaveEvent;
            _managerUi.OnSaveButtonClicked += HandleSave;
            _managerUi.OnRenameConfirmed += HandleRename;
            _managerUi.OnDeleteClicked += HandleDelete;
            _managerUi.OnSearch += HandleSearch;
            _managerUi.OnPinClicked += HandlePin;
            _managerUi.OnUnpinClicked += HandleUnpin;

            
            FindSaves();
        }

        private void HandleUnpin(string key) {
            if(!_pinnedSaves.Contains(key)) {
                return;
            }
            RemovePinnedSave(key);
        }

        private void HandlePin(string key) {
            if(_pinnedSaves.Contains(key)) {
                return;
            }
            AddPinnedSave(key);
        }

        private void EnsureDirectoriesPresent() {
            if (!Directory.Exists(_savesRoot)) {
                Directory.CreateDirectory(_savesRoot);
                
            }

            if(!Directory.Exists(_debugSavePath)) {
                Directory.CreateDirectory(_debugSavePath);
            }

            if(!File.Exists(_pinnedSavesFilePath)) {
                File.Create(_pinnedSavesFilePath);
            }
            else {
                _pinnedSaves = new(File.ReadAllLines(_pinnedSavesFilePath));
            }
        }

        private void HandleSearch(string searchText, SavesDisplayMode displayMode) {
            if (_managerUi == null) {
                return;
            }

            var result = PerformSearch(searchText, displayMode);

            _managerUi.SetSaveSlotsList(ProduceSaveSlotListItems(result));
        }

        private List<SaveFileDescriptor> PerformSearch(string searchNameText, SavesDisplayMode displayMode) {
            var result = new List<SaveFileDescriptor>();

            IEnumerable<SaveFileDescriptor> query = displayMode switch {
                SavesDisplayMode.Pinned => _savesCache.Values.Where(i => _pinnedSaves.Contains(i.Name)),
                SavesDisplayMode.CreatedDuringSession => _savesCache.Values.Where(i => _createdDuringSession.Contains(i.Name)),
                SavesDisplayMode.LatestUsedDuringSession => _lastUsed.Select(lu => _savesCache[lu]).OrderByDescending(i => i.LastTimeUsed),
                SavesDisplayMode.OrderedByCreationTimeDesc => _savesCache.Values.OrderByDescending(i => i.CreatedAt),
                SavesDisplayMode.OrderedByCreationTimeAsc => _savesCache.Values.OrderBy(i => i.CreatedAt),
                _ => _savesCache.Values
            };

            if (!string.IsNullOrEmpty(searchNameText)) {
                var exact = new List<SaveFileDescriptor>();
                var starts = new List<SaveFileDescriptor>();
                var contains = new List<SaveFileDescriptor>();

                foreach (var item in query) {
                    if (item.Name == searchNameText) exact.Add(item);
                    else if (item.Name.StartsWith(searchNameText)) starts.Add(item);
                    else if (item.Name.Contains(searchNameText)) contains.Add(item);
                }

                result = new List<SaveFileDescriptor>(exact.Count + starts.Count + contains.Count);
                result.AddRange(exact);
                result.AddRange(starts);
                result.AddRange(contains);
            } else {
                result.AddRange(query);
            }

            return result;
        }

        private void HandleDelete(string saveName) {
            if (!_savesCache.TryGetValue(saveName, out var save)) {
                Log.Error($"Tried to delete a save {saveName} but couldn't find it");
                return;
            }

            RemoveFileFromCache(saveName);
            Directory.Delete(save.FileName, true);
        }

        private void HandleRename(string oldName, string newName) {
            if (!_savesCache.TryGetValue(oldName, out var save)) {
                Log.Error($"Tried to rename a save {oldName} but couldn't find it");
                return;
            }

            if(!ValidateSaveName(newName)) {
                if (_managerUi != null) {
                    _managerUi.RenameValidationError = "Incorrect file name - OS doesn't allow files with such names or special symbols";
                    return;
                }
            }            

            if(save.Name == newName) {
                return;
            }

            Directory.Move(save.FileName, Path.Combine(_savesRoot, newName));
            if(_createdDuringSession.Contains(oldName)) {
                _createdDuringSession.Remove(oldName);
                _createdDuringSession.Add(newName);
            }

            if (_lastUsed.Contains(oldName)) {
                _lastUsed.Remove(oldName);
                _lastUsed.Add(newName);
            }

            if (_pinnedSaves.Contains(oldName)) {
                RemovePinnedSave(oldName);
                AddPinnedSave(newName);
            }

            FindSaves();
        }

        private void HandleSave() {
            try {
                string newSaveName = _managerUi?.NewSaveName ?? string.Empty;

                if (string.IsNullOrEmpty(newSaveName)) {
                    newSaveName = GetSceneSaveName();
                }

                SaveGame(newSaveName);
            } catch(Exception ex) {
                Log.Exception(ex);
            }
        }

        private string GetSceneSaveName() {
            var result = string.Empty;
            var sceneName = SceneManager.GetActiveScene().name;
            var lastSave = Directory.EnumerateDirectories(_savesRoot).Select(sv => new { Name = Path.GetFileName(sv), CreationDate = Directory.GetCreationTime(sv) }).Where(sv => sv.Name.StartsWith(sceneName)).OrderByDescending(sv => sv.CreationDate).FirstOrDefault();

            if (lastSave != null) {
                result = $"{lastSave.Name} (1)";
            } else {
                result = $"{sceneName} - Save";
            }

            return result;
        }

        private void HandleLoadSaveEvent(string saveName) {
            LoadSave(saveName);
        }

        public void Hook() {
            SaveManagerConfiguration.OnQuickSaveInvoked += HandleQuickSave;
            SaveManagerConfiguration.OnQuickloadInvoked += HandleQuickLoad;
            SaveManagerConfiguration.OnOpenManagerInvoked += HandleManagerToggle;
        }

        private void HandleManagerToggle() {
            if(_managerUi != null) {
                _managerUi.IsEnabled = !_managerUi.IsEnabled;
            }
        }

        private void HandleQuickLoad() {
            if(string.IsNullOrEmpty(_lastSave)) {
                return;
            }

            LoadSave(_lastSave);
        }

        private void HandleQuickSave() {
            SaveGame(GetSceneSaveName());
        }

        public void LoadSave(string name) {
            try {
                if (!_savesCache.TryGetValue(name, out var descriptor)) {
                    Log.Error($"A save named \"{name}\" isn't present in the cache");
                    return;
                }

                var saveFiles = Directory.GetFiles(descriptor.FileName);
                var oldSaveFiles = Directory.GetFiles(_debugSavePath, "*.*");
                foreach (var file in oldSaveFiles) {
                    try {
                        File.Delete(file);

                    } catch (Exception ex) {
                    }
                }

                foreach (var file in saveFiles) {
                    try {
                        File.Copy(file, Path.Combine(_debugSavePath, Path.GetFileName(file)));

                    } catch (Exception ex) {
                    }
                }

                descriptor.LastTimeUsed = DateTime.Now;
                _lastUsed.Add(descriptor.Name);
                NotifySavesCollectionChanged();

                SingletonBehaviour<ApplicationUIGroupManager>.Instance.PopAll();
                EffectReceiver.EffectReceiverCache.Clear();
                IsSaveSlotLoadRequested = true;
                if(GameCore.IsAvailable()) {
                    SingletonBehaviour<GameCore>.Instance.gameLevel.gameObject.SetActive(false);
                    SingletonBehaviour<GameCore>.Instance.gameLevel.SetLevelDestroy(false);
                }
                SaveManager.Instance.SetSlot(5);
                SceneManager.LoadScene("ClearTransition");
                if(ApplicationCore.Instance.soundManager != null && GameCore.IsAvailable()) {
                    ApplicationCore.Instance.soundManager.SetListenerTarget(ApplicationCore.Instance.transform);
                }
            } catch (Exception ex) {
                Log.Exception(ex);
            }
        }

        public void SaveGame(string name) {
            if (!_isPlaying) {
                if (_managerUi != null) {
                    _managerUi.NewSaveNameValidationError = "No save loaded";
                }
                return;
            }

            if (!ValidateSaveName(name)) {

                if (_managerUi != null) {
                    _managerUi.NewSaveNameValidationError = "Incorrect file name - OS doesn't allow files with such names or special symbols";
                }

                return;
            }

            if (_savesCache.ContainsKey(name)) {

                if (_managerUi != null) {
                    _managerUi.NewSaveNameValidationError = "This name is already taken";
                }

                return;
            }

            var savePath = Path.Combine(_savesRoot, name);
            var saveFileDescriptor = new SaveFileDescriptor(name, savePath, DateTime.Now, DateTime.MinValue);

            if (SaveManagerConfiguration.SaveOnPosition) {
                SaveManager.Instance.ForceSaveAt(SaveManager.SaveSceneScheme.CurrentSceneAndPos);
            }
            if (!SaveManagerConfiguration.SaveOnPosition) {
                SaveManager.Instance.ForceSaveAt(SaveManager.SaveSceneScheme.LastTouchedSavePoint);
            }
            SaveManager.Instance.SaveAllFlagsAndMeta(savePath);

            AddFileToCache(saveFileDescriptor);
            _createdDuringSession.Add(saveFileDescriptor.Name);
            _lastSave = saveFileDescriptor.Name;
            NotifySavesCollectionChanged();

        }

        private void AddFileToCache(SaveFileDescriptor descriptor) {
            _savesCache[descriptor.Name] = descriptor;
            NotifySavesCollectionChanged();
        }

        private void RemoveFileFromCache(string key) {
            _savesCache.Remove(key);
            _createdDuringSession.Remove(key);
            _lastUsed.Remove(key);
            RemovePinnedSave(key);
            NotifySavesCollectionChanged();
        }

        private void AddPinnedSave(string key) {
            _pinnedSaves.Add(key);
            NotifySavesCollectionChanged();
            Task.Run(() => {
                lock(_lock) {
                    File.WriteAllLines(_pinnedSavesFilePath, _pinnedSaves);
                }
            });
        }

        private void RemovePinnedSave(string key) {
            _pinnedSaves.Remove(key);
            NotifySavesCollectionChanged();
            Task.Run(() => {
                lock (_lock) {
                    File.WriteAllLines(_pinnedSavesFilePath, _pinnedSaves);
                }
            });
        }

        private void FindSaves() {
            _savesCache.Clear();
            var saves = Directory.EnumerateDirectories(_savesRoot);
            foreach (var save in saves) {
                try {
                    var name = Path.GetFileName(save);
                    var createdAt = Directory.GetCreationTime(save);
                    AddFileToCache(new SaveFileDescriptor(name, save, createdAt, DateTime.MinValue));
                } catch (Exception ex) {
                    Log.Exception(ex);
                }
            }
        }

        private void NotifySavesCollectionChanged() {
            if(_managerUi != null) {
                _managerUi.SetSaveSlotsList(ProduceSaveSlotListItems(PerformSearch(_managerUi.SearchText, _managerUi.DisplayMode)));
            }
        }
        private List<SaveSlotListItem> ProduceSaveSlotListItems(IEnumerable<SaveFileDescriptor> descriptors) {
            return descriptors.Select(d => new SaveSlotListItem(d.Name, d.CreatedAt, _pinnedSaves.Contains(d.Name))).ToList();
        }

        private void OnGUI() {
            _managerUi?.Draw();
        }

        private bool ValidateSaveName(string name) {
            if (string.IsNullOrEmpty(name)) {
                return false;
            }
            
            return _invalidFileNameRegex.IsMatch(name);
        }   

        internal enum SavesDisplayMode {
            Pinned,
            OrderedByCreationTimeDesc,
            OrderedByCreationTimeAsc,
            LatestUsedDuringSession,
            CreatedDuringSession
        }
    }
}
