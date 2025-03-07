﻿using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace ie.TUDublin.GE2.Components.Spawning {

    /// <summary>
    /// Authoring Class for ship spawning
    /// </summary>
    public class ShipSpawningSettingsAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs {

        [SerializeField] private Vector3 position;
        [SerializeField] private Vector3 rotation;
        [SerializeField] private Vector3 spawnArea;
        [SerializeField] private float shipCountMultiplier = 1; 
        
        [SerializeField] private List<ShipSettings> ships;
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {

            dstManager.AddComponentData(entity, new ShipSpawningSettingsData() {
                position = position,
                rotation = rotation,
                spawnArea = spawnArea,
                shipCountMultiplier = shipCountMultiplier
            });
            
            var shipBuffer = dstManager.AddBuffer<ShipSpawnElement>(entity);
            foreach (var ship in ships) {
                shipBuffer.Add(new ShipSpawnElement() {
                    prefab = conversionSystem.GetPrimaryEntity(ship.prefab),
                    numberOfShips = ship.numberOfShips,
                });
            }

        }

        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs) => referencedPrefabs.AddRange(ships.Select(ship => ship.prefab));
    }

    /// <summary>
    /// Details about a ship that can be spawned
    /// </summary>
    [System.Serializable]
    public class ShipSettings {
        public string name;
        public GameObject prefab;
        public int2 numberOfShips;
    }

    /// <summary>
    /// Settings about the ship spawning positions 
    /// </summary>
    public struct ShipSpawningSettingsData : IComponentData {
        public float3 position;
        public float3 rotation; 
        public float3 spawnArea;
        public float shipCountMultiplier;
    }

    /// <summary>
    /// Buffer Element data about ships a spawner can spawn
    /// </summary>
    public struct ShipSpawnElement : IBufferElementData {
        public Entity prefab;
        public int2 numberOfShips;
    }

}