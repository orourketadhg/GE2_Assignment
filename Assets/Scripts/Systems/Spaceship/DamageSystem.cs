﻿using ie.TUDublin.GE2.Components.Spaceship;
using ie.TUDublin.GE2.Systems.Physics;
using ie.TUDublin.GE2.Systems.Util;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;

namespace ie.TUDublin.GE2.Systems.Spaceship {

    /// <summary>
    /// System to schedule the damage dealing on Enities
    /// </summary>
    [UpdateBefore(typeof(SceneCleanupSystem))]
    public class DamageSystem : SystemBase {

        public JobHandle OutDependency => Dependency;

        public EndSimulationEntityCommandBufferSystem _entityCommandBuffer;

        private StepPhysicsWorld _stepPhysicsWorld;
        private BuildPhysicsWorld _buildPhysicsWorld;

        protected override void OnCreate() {
            _stepPhysicsWorld = World.GetExistingSystem<StepPhysicsWorld>();
            _buildPhysicsWorld = World.GetExistingSystem<BuildPhysicsWorld>();

            _entityCommandBuffer = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate() {
            
            // get dependency of physics world
            Dependency = JobHandle.CombineDependencies(_stepPhysicsWorld.FinalSimulationJobHandle, Dependency);

            var ecb = _entityCommandBuffer.CreateCommandBuffer();

            // Job Handlers
            var damageDataHandler = GetComponentDataFromEntity<DamageData>();
            var healthDataHandler = GetComponentDataFromEntity<HealthData>();

            // Job Declarations            
            var collisionDamageJob = new DamageTriggerJob() {
                ecb = ecb,
                damageData = damageDataHandler,
                healthData = healthDataHandler
            };
            
            // Job Scheduling
            var collisionDamageJobHandle = collisionDamageJob.Schedule(_stepPhysicsWorld.Simulation, ref _buildPhysicsWorld.PhysicsWorld, Dependency);
            Dependency = JobHandle.CombineDependencies(Dependency, collisionDamageJobHandle);
            
            _entityCommandBuffer.AddJobHandleForProducer(Dependency);
            
        }
        
        
    }

}