using Unity;
using Unity.Entities;


public class BuildingConstructionSystem : SystemBase
{
    private double NextWorkUpdate = 0.0f;
    private double WorkUpdateRate = 1.0f;

    protected override void OnCreate()
    {
    
    }

    protected override void OnUpdate()
    {
        // Remove Construction Component with isDone
        // Or not?

        double current_time = World.Time.ElapsedTime;

        if(NextWorkUpdate < current_time)
        {
        // One second later
        NextWorkUpdate = current_time + WorkUpdateRate;

        // +1 to work completion
        Entities
        .WithAll<BuildingConstructionComponent>()
        .ForEach(
            // writeable component
            (ref BuildingConstructionComponent c) =>
            {
                if(c.workCurrent == c.workToComplete)
                    c.isDone = true;
                
                if(c.isDone)
                    return;
            
                c.workCurrent += 1;
            }
        ).ScheduleParallel();
        }

    }


}


