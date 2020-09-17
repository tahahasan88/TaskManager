using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.Data.Repositories.FollowUpResponse
{
    public class FollowUpResponseRepository : IFollowUpResponseRepository
    {
        public readonly TaskManagerContext _context;
        public FollowUpResponseRepository(TaskManagerContext context)
        {
            _context = context;
        }

        public List<TaskFollowUpResponse> GetFollowUpResponseByFollowUpId(int followupId)
        {
            throw new NotImplementedException();
        }
    }
}
