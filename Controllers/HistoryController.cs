using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskManager.Common;
using TaskManager.Data;
using TaskManager.Web.Models;


namespace TaskManager.Web.Controllers
{
    public class HistoryController
    {
        private readonly TaskManagerContext _context;
        private string currentUserName;

        public HistoryController(TaskManagerContext context)
        {
            _context = context;
            currentUserName = "tahahasan";
        }

        public async Task<IActionResult> Index()
        {
            return null;
        }
    }
}
