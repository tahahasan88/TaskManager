using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.Data.Repositories
{
    public interface IFollowUpResponseRepository
    {
        List<TaskFollowUpResponse> GetFollowUpResponseByFollowUpId(int followupId);
    }
}
