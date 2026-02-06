using Unity.Netcode;
using UnityEngine;
using GameDesign.Utils;

namespace GameDesign.Gameplay
{
    public enum Season { Spring, Summer, Autumn, Winter }

    public class EnvironmentManager : Singleton_N<EnvironmentManager>
    {
        [Header("Settings")]
        public int stepsPerDay = 16;
        public int daysPerSeason = 4;

        // --- ͬ����ֵ (Data Layer) ---
        public NetworkVariable<int> TotalSteps = new NetworkVariable<int>(0);
        public NetworkVariable<int> CurrentDay = new NetworkVariable<int>(1);
        public NetworkVariable<Season> CurrentSeason = new NetworkVariable<Season>(Season.Spring);

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (IsServer)
            {
                TotalSteps.OnValueChanged += OnStepsChanged;
            }
        }

        private void OnStepsChanged(int oldSteps, int newSteps)
        {
            // 简单的计算逻辑
            int calculatedDay = (newSteps / stepsPerDay) + 1;
            if (calculatedDay > CurrentDay.Value)
            {
                CurrentDay.Value = calculatedDay;
                UpdateSeason(calculatedDay);
                
                // 触发全局事件
                Game.Core.EventBus.Publish(new Game.Core.DayNightChangedEvent { IsDay = true, DayCount = calculatedDay });
            }
        }

        private void UpdateSeason(int day)
        {
            int seasonIdx = ((day - 1) / daysPerSeason) % 4;
            CurrentSeason.Value = (Season)seasonIdx;
            
            Game.Core.EventBus.Publish(new Game.Core.SeasonChangedEvent { NewSeason = CurrentSeason.Value });
        }

        [ServerRpc(InvokePermission = RpcInvokePermission.Everyone)]
        public void AddGlobalStepsServerRpc(int steps)
        {
            TotalSteps.Value += steps;
        }
    }
}