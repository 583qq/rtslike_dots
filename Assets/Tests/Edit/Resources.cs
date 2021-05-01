using Unity;
using Unity.Collections;

using Unity.Entities;

using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;

namespace Game.Tests
{
    public class ResourceTest
    {
        public const int startResourceValue = 100;
        private WorldFlags testWorldFlag = WorldFlags.Live;

        [Test]
        public void InitResourcesCase()
        {
            World testWorld = new World("ResourceTestWorld", testWorldFlag);
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

        [Test]
        public void SpendSomeResourcesCase()
        {
            World testWorld = new World("SpendingSomeResourcesWorld", testWorldFlag);
            var resourceSystem = testWorld.CreateSystem<PlayerResourceSystem>();

            var newTransactionEntity = resourceSystem.EntityManager.CreateEntity();

            // New transaction to spend 50 gold
            
            resourceSystem.SpendResource(ResourceTypes.Gold, 50);

            // Get actual resource buffer

            var playerEntitiesArray = resourceSystem.EntityManager.CreateEntityQuery(typeof(PlayerTag))
                                                                  .ToEntityArray(Allocator.Temp);

            var resourceDataBuffer = resourceSystem.EntityManager.GetBuffer<PlayerResourceData>(playerEntitiesArray[0]);

            foreach(var resource in resourceDataBuffer)
            {
              if(resource.resource.type != ResourceTypes.Gold)
                continue;

              Assert.AreEqual(startResourceValue - 50, resource.resource.value);
            }
        }
    }
}
