using Unity;
using Unity.Entities;
using Unity.Collections;
using UnityEngine;

using Unity.Assertions;

namespace Game.Tests
{
    public class ResourceTest
    {
        public const int startResourceValue = 100;

        public void InitResourcesCase()
        {
            World testWorld = new World("ResourceTestWorld", WorldFlags.Simulation);
            var resourceSystem = testWorld.CreateSystem<PlayerResourceSystem>();

            // NativeArray
            var playerEntitiesArray = resourceSystem.EntityManager.CreateEntityQuery(typeof(PlayerTag))
                                                                   .ToEntityArray(Allocator.Temp);

            // One player                                                     
            var resourceDataBuffer = resourceSystem.EntityManager.GetBuffer<PlayerResourceData>(playerEntitiesArray[0]);

            foreach(var resource in resourceDataBuffer)
            {
                if(resource.resource.type == ResourceTypes.None)
                    continue;
                
                Assert.AreEqual(startResourceValue, resource.resource.value);
            }
            //
        }
    }
}
