﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Data;
using static TaskManager.Common.Common;

namespace TaskManager.Web.Models
{
    public class TaskCreateViewModel
    {
        public int Id { get; set; }
        public string TaskProgress { get; set; }
        [Required(ErrorMessage ="*Title is required")]
        [TaskUniqueValidation]
        public virtual string Title { get; set; }
        public string Description { get; set; }

        private DateTime _date = DateTime.Now;

        [Required(ErrorMessage ="*Target Date is required")]
        [ValidateDateRange]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Target
        {
            get { return _date; }
            set { _date = value; }
        }

        //Hack...
        public DateTime TargetVal
        {
            get { return _date; }
            set { _date = value; }
        }

        public bool IsDeleted { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int PriorityId { get; set; }
        public string AssigneeCode { get; set; }
        public List<SelectListItem> PriorityList { get; set; }
        public List<AssigneeDropDownViewModel> EmployeeList { get; set; }
    }


    public class TaskUniqueValidation : ValidationAttribute
    {
        private TaskManagerContext _context { get; set; }

        public TaskUniqueValidation()
        {
            _context = new TaskManagerContext();
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string taskTitle = (string)value;
            // your validation logic

            Data.Task existingTask = _context.Tasks.Where(x => x.Title == taskTitle && x.IsDeleted == false).SingleOrDefault();
            bool isTaskAlreadyAdded = existingTask != null ? true : false;

            if (!isTaskAlreadyAdded)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("Please enter unique task title");
            }
        }
    }
}
