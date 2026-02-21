using NineSolsAPI.Utils;
using NKVDebugMod.Features.MonsterInspector.Configuration;
using NKVDebugMod.Features.MonsterInspector.HarmonyPatches;
using NKVDebugMod.Features.MonsterInspector.UI;
using NKVDebugMod.Features.UI;
using NKVDebugMod.Features.UI.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NKVDebugMod.Features.MonsterInspector {
    internal class MonsterInspector : MonoBehaviour {
        internal static MonsterInspector? Instance { get; set; }

        private MonsterInspectorUI? _inspectorWindow;
        private Dictionary<string, MonsterBase> _monstersCache = new();

        private MonsterBase? _selectedMonster;
        public MonsterBase? SelectedMonster {
            get {
                return _selectedMonster;
            }

            set {
                try {
                    if (_selectedMonster == value) {
                        return;
                    }

                    _selectedMonster = value;
                    if (_inspectorWindow != null) {
                        _inspectorWindow.SelectedMonsterData = CreateMonsterData(value);

                        if (SelectedMonster == null) {
                            _inspectorWindow.SetMonsterStates([]);
                        } else {
                            _inspectorWindow.SetMonsterStates(SelectedMonster.fsm._stateMapping.getAllStates.Select(state => new MonsterInspectorUI.MonsterStateDescriptor(state.stateBehavior.name, state.state)));
                        }
                    }
                } catch (Exception ex) {
                    Log.Exception(ex);
                }
            }
        }



        private void Awake() {
            try {
                Instance = this;
                MonsterInspectorConfiguration.Init();
            
                _inspectorWindow = new MonsterInspectorUI();
                _inspectorWindow.OnStateSelected += HandleMonsterStateSelected;
                _inspectorWindow.OnNextMonsterClick += HandleNextMonsterSelection;
                _inspectorWindow.OnPrevMonsterClick += HandlePrevMonsterSelection;
                _inspectorWindow.OnSelectClosestMonsterClick += HandleClosestMonsterSelection;

                MonsterInspectorConfiguration.OnToggleMonsterInspectorInvoked += ToggleUI;
                MonsterInspectorPatches.OnRegisterPostifx += HandleMonsterRegister;
                MonsterInspectorPatches.OnUnregisterPostifx += HandleMonsterUnregister;
            } catch (Exception ex) {
                Log.Exception(ex);
            }
        }

        private void HandleMonsterStateSelected(MonsterBase.States state) {
            if(SelectedMonster != null) {
                SelectedMonster.ChangeStateIfValid(state);
            }
        }

        private void HandleClosestMonsterSelection() {
            if (MonsterManager.IsAvailable()) {
                SelectedMonster = MonsterManager.Instance.ClosetMonster;
            }
        }

        private void HandlePrevMonsterSelection() {
            if (SelectedMonster == null) {
                SelectedMonster = _monstersCache.Values.Last();
                return;
            }

            var keys = _monstersCache.Keys.ToList();
            var currentIndex = keys.IndexOf(ObjectUtils.ObjectPath(SelectedMonster.gameObject));

            if(currentIndex == 0) {
                SelectedMonster = _monstersCache.Values.Last();
                return;
            }

            SelectedMonster = _monstersCache[keys[currentIndex - 1]];
        }

        private void HandleNextMonsterSelection() {
            if (SelectedMonster == null) {
                SelectedMonster = _monstersCache.Values.First();
                return;
            }

            var keys = _monstersCache.Keys.ToList();
            var currentIndex = keys.IndexOf(ObjectUtils.ObjectPath(SelectedMonster.gameObject));

            if (currentIndex == keys.Count - 1) {
                SelectedMonster = _monstersCache.Values.First();
                return;
            }

            SelectedMonster = _monstersCache[keys[currentIndex + 1]];
        }

        private void HandleMonsterRegister(MonsterManager manager, MonsterBase monster) {
            MonstersListChanged(manager);
        }
        private void HandleMonsterUnregister(MonsterManager manager, MonsterBase monster) {
            MonstersListChanged(manager);
        }

        private void MonstersListChanged(MonsterManager manager) {
            _monstersCache.Clear();
            foreach (var monster in manager.monsterDict.Values) {
                _monstersCache.TryAdd(ObjectUtils.ObjectPath(monster.gameObject), monster);
            }

            if(SelectedMonster != null && _monstersCache.ContainsKey(ObjectUtils.ObjectPath(SelectedMonster.gameObject))) {
                SelectedMonster = null;
            }
        }

        private void ToggleUI() {
            if (_inspectorWindow != null) {
                _inspectorWindow.IsInspectorEnabled = !_inspectorWindow.IsInspectorEnabled;
            }
        }

        private ModMonsterData? CreateMonsterData(MonsterBase? monster) {
            if (monster == null) {
                return null;
            }

            var monsterData = new ModMonsterData(monster);
            monsterData.Name = monster.name;
            monsterData.Path = ObjectUtils.ObjectPath(monster.gameObject);

            return monsterData;
        }

        private void OnGUI() {
            _inspectorWindow?.Draw();
            if(_inspectorWindow?.IsInspectorEnabled ?? false) {
                foreach (var monster in _monstersCache.Values) {
                    var collider = monster.myCollider;
                    var color = SelectedMonster == monster ? Color.green : Color.red;
                    HitboxDrawer.DrawCollider2D(collider, color);
                }
            }
        }

        private void OnDestroy() {
            MonsterInspectorConfiguration.OnToggleMonsterInspectorInvoked -= ToggleUI;
            MonsterInspectorPatches.OnRegisterPostifx -= HandleMonsterRegister;
            MonsterInspectorPatches.OnUnregisterPostifx -= HandleMonsterUnregister;
        }
    }
}
