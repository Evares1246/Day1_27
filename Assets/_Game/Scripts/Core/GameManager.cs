using UnityEngine;
using GameDesign.Utils;

namespace Game.Core
{
    public enum GameState
    {
        Bootstrap,
        Menu,
        Lobby,
        Gameplay,
        GameOver
    }

    public class GameManager : Singleton<GameManager>
    {
        public GameState CurrentState { get; private set; }
        
        // EnvironmentManager 负责处理具体的天数/季节计算逻辑
        // GameManager 负责高层的游戏状态切换

        protected override void Awake()
        {
            base.Awake(); // 设置 Singleton 和 DontDestroyOnLoad
        }

        public void ChangeState(GameState newState)
        {
            CurrentState = newState;
            EventBus.Publish(new GameStateChangedEvent { NewState = newState });
            Debug.Log($"[GameManager] 状态已更改为: {newState}");
        }
    }
}
