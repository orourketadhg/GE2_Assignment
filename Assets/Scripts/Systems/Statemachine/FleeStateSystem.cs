﻿using ie.TUDublin.GE2.Components.Spaceship;
using ie.TUDublin.GE2.Systems.Util;
using Unity.Entities;

namespace ie.TUDublin.GE2.Systems.Statemachine {

    /// <summary>
    /// System to update a ships Fleeing state
    /// </summary>
    public class FleeStateSystem : SystemBase {
        
        private EndSimulationEntityCommandBufferSystem _entityCommandBuffer;

        protected override void OnCreate() {
            _entityCommandBuffer = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate() {
            
            var ecb = _entityCommandBuffer.CreateCommandBuffer();
            
            Entities
                .WithName("FleeingSystem")
                .WithBurst()
                .ForEach((Entity entity, int entityInQueryIndex, int nativeThreadIndex, in DynamicBuffer<PursuerElementData> evasionBuffer) => {

                    // if the ship is being pursued attempt to evade
                    if (evasionBuffer.IsEmpty) {
                        StatemachineUtil.TransitionFromFleeing(ecb, entityInQueryIndex, entity);
                    }
                    else {
                        StatemachineUtil.TransitionFromFleeing(ecb, entityInQueryIndex, entity);
                    }
                    
                }).Schedule();
            
            _entityCommandBuffer.AddJobHandleForProducer(Dependency);
            
        }
    }

}