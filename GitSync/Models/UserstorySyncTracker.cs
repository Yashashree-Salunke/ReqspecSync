using System;
using System.Collections.Generic;
using System.Text;

namespace GitSync.Models
{
    public class UserstorySyncTracker
    {
        public int Id { get; set; }
        public int UserstorySyncActionTypeId { get; set; }
        public virtual UserstorySyncActionType UserstorySyncActionType { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
