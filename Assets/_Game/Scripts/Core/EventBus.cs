using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core
{
    /// <summary>
    /// 一个简单、类型安全的事件总线，用于解耦通信。
    /// 用法: EventBus.Publish(new MyEvent());
    ///       EventBus.Subscribe<MyEvent>(OnMyEvent);
    /// </summary>
    public static class EventBus
    {
        private static readonly Dictionary<Type, List<Delegate>> _subscribers = new();

        public static void Subscribe<T>(Action<T> callback)
        {
            var type = typeof(T);
            if (!_subscribers.ContainsKey(type))
            {
                _subscribers[type] = new List<Delegate>();
            }
            _subscribers[type].Add(callback);
        }

        public static void Unsubscribe<T>(Action<T> callback)
        {
            var type = typeof(T);
            if (_subscribers.ContainsKey(type))
            {
                _subscribers[type].Remove(callback);
            }
        }

        public static void Publish<T>(T eventMessage)
        {
            var type = typeof(T);
            if (_subscribers.TryGetValue(type, out var callbacks))
            {
                // 克隆列表以允许在事件处理期间取消订阅
                var callbacksCopy = new List<Delegate>(callbacks);
                foreach (var callback in callbacksCopy)
                {
                    (callback as Action<T>)?.Invoke(eventMessage);
                }
            }
        }
    }

    // --- 核心事件定义 ---
    public struct GameStateChangedEvent { public GameState NewState; }
    // DayNight 和 Season 事件移交给 EnvironmentManager 处理，或在这里保留定义但由 EnvManager 触发
    public struct DayNightChangedEvent { public bool IsDay; public int DayCount; }
    public struct SeasonChangedEvent { public GameDesign.Gameplay.Season NewSeason; }
}
