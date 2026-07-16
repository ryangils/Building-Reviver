using Game;
using Game.Buildings;
using Game.Common;
using Game.Simulation;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;

namespace BuildingReviver
{
    /// <summary>
    /// Periodically scans for abandoned buildings and revives them: the Abandoned
    /// component is removed, the building's condition is reset so it doesn't
    /// immediately relapse, and the property is put back on the market so new
    /// occupants can move in.
    /// </summary>
    public partial class BuildingReviverSystem : GameSystemBase
    {
        /// <summary>Simulation frames per in-game day.</summary>
        public const int kFramesPerDay = 262144;

        /// <summary>Upper bound of the "sweeps per day" setting; also the system's base tick rate.</summary>
        public const int kMaxSweepsPerDay = 64;

        /// <summary>Revival counter since game start (shown in options UI).</summary>
        public static int TotalRevived;

        private SimulationSystem m_SimulationSystem;
        private EntityQuery m_AbandonedQuery;

        private uint m_LastSweepFrame;

        public static void ResetStatistics()
        {
            TotalRevived = 0;
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            m_SimulationSystem = World.GetOrCreateSystemManaged<SimulationSystem>();

            m_AbandonedQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new[]
                {
                    ComponentType.ReadOnly<Building>(),
                    ComponentType.ReadOnly<Abandoned>(),
                },
                None = new[]
                {
                    ComponentType.ReadOnly<Destroyed>(),
                    ComponentType.ReadOnly<Deleted>(),
                    ComponentType.ReadOnly<Temp>(),
                },
            });
        }

        public override int GetUpdateInterval(SystemUpdatePhase phase)
        {
            // Tick at the fastest configurable rate; OnUpdate throttles down to the
            // configured sweeps-per-day so the slider takes effect immediately.
            return kFramesPerDay / kMaxSweepsPerDay;
        }

        protected override void OnUpdate()
        {
            var setting = Mod.Setting;
            if (setting == null || !setting.EnableMod)
            {
                return;
            }

            var frame = m_SimulationSystem.frameIndex;
            var sweepsPerDay = Clamp(setting.SweepsPerDay, 1, kMaxSweepsPerDay);
            var sweepInterval = (uint)(kFramesPerDay / sweepsPerDay);

            // frame < m_LastSweepFrame means a different (older) save was loaded; sweep now.
            if (frame >= m_LastSweepFrame && frame - m_LastSweepFrame < sweepInterval)
            {
                return;
            }

            m_LastSweepFrame = frame;

            if (setting.ReviveAbandoned)
            {
                TotalRevived += ReviveAbandoned();
            }
        }

        private int ReviveAbandoned()
        {
            if (m_AbandonedQuery.IsEmptyIgnoreFilter)
            {
                return 0;
            }

            var entities = m_AbandonedQuery.ToEntityArray(Allocator.Temp);
            var revived = 0;

            foreach (var entity in entities)
            {
                EntityManager.RemoveComponent<Abandoned>(entity);

                // A relapsed building keeps its rock-bottom condition; reset it so the
                // vanilla systems don't immediately abandon it again.
                ResetCondition(entity);

                // Put the property back on the rental market so new occupants move in.
                if (!EntityManager.HasComponent<PropertyOnMarket>(entity) &&
                    !EntityManager.HasComponent<PropertyToBeOnMarket>(entity))
                {
                    EntityManager.AddComponent<PropertyToBeOnMarket>(entity);
                }

                // Nudge the game to refresh the building's visuals and state.
                if (!EntityManager.HasComponent<Updated>(entity))
                {
                    EntityManager.AddComponent<Updated>(entity);
                }

                revived++;
            }

            entities.Dispose();

            if (revived > 0)
            {
                Mod.Log.Info($"Revived {revived} abandoned building(s). Session total: {TotalRevived + revived}.");
            }

            return revived;
        }

        private void ResetCondition(Entity entity)
        {
            if (EntityManager.HasComponent<BuildingCondition>(entity))
            {
                var condition = EntityManager.GetComponentData<BuildingCondition>(entity);
                if (condition.m_Condition < 0)
                {
                    condition.m_Condition = 0;
                    EntityManager.SetComponentData(entity, condition);
                }
            }
        }

        private static int Clamp(int value, int min, int max)
        {
            return value < min ? min : value > max ? max : value;
        }
    }
}
