using GitSync.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Scheduler.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler.Persistence
{
    public class ReqspecScheduleContextSeed
    {
        public async Task SeedAsync(ReqspecScheduleContext context)
        {
            await JobTypesSeed(context);
            await UserstorySyncActionTypesSeed(context);
            await JobSyncTrackersSeed(context);
        }

        private async Task JobSyncTrackersSeed(ReqspecScheduleContext context)
        {
            if (await context.JobSyncTrackers.AnyAsync() == false)
            {
                context.JobSyncTrackers.Add(new JobSyncTracker { JobTypeId = 1, LastModifiedOn = DateTime.UtcNow });
                await context.SaveChangesAsync();
            }
        }

        private async Task UserstorySyncActionTypesSeed(ReqspecScheduleContext context)
        {
            if (await context.UserstorySyncActionTypes.AnyAsync() == false)
            {
                context.UserstorySyncActionTypes.Add(new UserstorySyncActionType { Code = "ADD" });
                context.UserstorySyncActionTypes.Add(new UserstorySyncActionType { Code = "DELETE" });
                context.UserstorySyncActionTypes.Add(new UserstorySyncActionType { Code = "UPDATE" });
                context.UserstorySyncActionTypes.Add(new UserstorySyncActionType { Code = "MOVE" });
                await context.SaveChangesAsync();
            }
        }

        private async Task JobTypesSeed(ReqspecScheduleContext context)
        {
            if (await context.JobTypes.AnyAsync() == false)
            {
                context.JobTypes.Add(new JobType { Code = "USERSTORYSYNC", Title = "Userstory sync job" });
                await context.SaveChangesAsync();
            }
        }
    }
}
