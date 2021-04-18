using Unity;
using Unity.Entities;


public class ConstructionProgressSystem : SystemBase
{
    private double nextWorkUpdate = 0.0f;
    private double workUpdateRate = 1.0f;

    protected override void OnCreate()
    {
    
    }

    protected override void OnUpdate()
    {
        // Remove Construction Component with isDone
        // Or not?

        double currentTime = World.Time.ElapsedTime;

        if(nextWorkUpdate < currentTime)
        {
        // One second later
        nextWorkUpdate = currentTime + workUpdateRate;

        // +1 to work completion
        Entities
        .ForEach(
            // Writeable component
            (ref BuildingConstructionComponent construction) =>
            {
                if(construction.workCurrent == construction.workToComplete)
                    construction.isDone = true;
                
                if(construction.isDone)
                    return;
            
                construction.workCurrent += 1;
            }
        ).ScheduleParallel();
        }

    }


}


