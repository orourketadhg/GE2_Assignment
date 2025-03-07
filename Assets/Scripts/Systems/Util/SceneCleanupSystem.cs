﻿using ie.TUDublin.GE2.Components.Spaceship;
using ie.TUDublin.GE2.Components.Tags;
using Unity.Entities;

namespace ie.TUDublin.GE2.Systems.Util {

    /// <summary>
    /// System to cleanup the scene
    /// </summary>
    public class SceneCleanupSystem : SystemBase {

        private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;
        
        protected override void OnCreate() {
            _entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate() {

            var ecb = _entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter();
            float timeElapsed = (float) Time.ElapsedTime;
            
            // remove entities with delete Tag
            Entities
                .WithName("DeleteTagCleanup")
                .WithBurst()
                .WithAll<DeleteTag>()
                .ForEach((Entity entity, int entityInQueryIndex) => {
                    ecb.DestroyEntity(entityInQueryIndex, entity);
                }).ScheduleParallel();
            
            // remove bullets after a certain amount of time
            Entities
                .WithName("BulletLifetimeCleanup")
                .WithBurst()
                .ForEach((Entity entity, int entityInQueryIndex, ref ProjectileSpawnData spawnData) => {
                    if (spawnData.DoDespawn == 1 && timeElapsed >= spawnData.SpawnTime + spawnData.ProjectileLifetime) {
                        ecb.DestroyEntity(entityInQueryIndex, entity);
                    }
                }).ScheduleParallel();

            // cleanup bullets that have hit something
            Entities
                .WithName("BulletHealthCleanup")
                .WithBurst()
                .WithAll<ProjectileSpawnData>()
                .ForEach((Entity entity, int entityInQueryIndex, in HealthData healthData) => {
                    if (healthData.Value <= 0) {
                        ecb.DestroyEntity(entityInQueryIndex, entity);
                    }
                }).ScheduleParallel();
            
            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);

        }
    }

}