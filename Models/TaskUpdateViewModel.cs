using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TaskManager.Data;
using static TaskManager.Common.Common;

namespace TaskManager.Web.Models
{
    public class TaskUpdateViewModel 
    {
        public int Id { get; set; }
        public string TaskProgress { get; set; }
        [Required(ErrorMessage = "*Title is required")]
        [TaskUpdateUniqueValidation]
        public virtual string Title { get; set; }
        [Required(ErrorMessage ="*Description is required")]
        public string Description { get; set; }

        private DateTime _date = DateTime.Now;

        [Required(ErrorMessage = "*Target Date is required")]
        [ValidateDateRange]
        [DataType(DataType.Date)]
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

    public class TaskUpdateUniqueValidation : ValidationAttribute
    {
        private TaskManagerContext _context { get; set; }

        public TaskUpdateUniqueValidation()
        {
            _context = new TaskManagerContext();
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            object instance = validationContext.ObjectInstance;
            Type type = instance.GetType();
            PropertyInfo property = type.GetProperty("Id");
            object propertyValue = property.GetValue(instance);
            int taskId = (int)propertyValue;

            string taskTitle = (string)value;
            // your validation logic

            Data.Task existingTask = _context.Tasks.Where(x => x.Title == taskTitle
            && x.Id != taskId
            && x.IsDeleted == false).SingleOrDefault();
            bool isTaskValid = existingTask != null ? false : true;

            if (isTaskValid)
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
