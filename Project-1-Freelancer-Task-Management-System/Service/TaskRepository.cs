    using Freelancer_Task.Data;
using Freelancer_Task.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Freelancer_Task.Services
{
    public class TaskRepository
    {
        private readonly FreelancerContext _context;

        public TaskRepository(FreelancerContext context)
        {
            _context = context;
        }

        public void AddTask(PTask task)
        {
            _context.Tasks.Add(task);
            _context.SaveChanges();
        }

        public void UpdateTask(PTask task)
        {
            _context.Tasks.Update(task);
            _context.SaveChanges();
        }

        public void DeleteTask(int id)
        {
            var task = _context.Tasks.Find(id);
            if (task != null)
            {
                _context.Tasks.Remove(task);
                _context.SaveChanges();
            }
        }

        public PTask GetTaskById(int id) => _context.Tasks.Find(id);

        public List<PTask> GetTasksByProject(int projectId)
        {
            return _context.Tasks
                .Where(t => t.ProjectId == projectId)
                .OrderByDescending(t => t.TimerStart)
                .ToList();
        }

        public List<PTask> GetTasksByStatus(PTaskStatus status)
        {
            return _context.Tasks
                .Where(t => t.Status == status)
                .ToList();
        }

        public void UpdateTaskStatus(int taskId, PTaskStatus newStatus)
        {
            var task = _context.Tasks.Find(taskId);
            if (task != null)
            {
                task.Status = newStatus;
                _context.SaveChanges();
            }
        }

        public void LogTime(int taskId, TimeSpan timeSpent)
        {
            var task = _context.Tasks.Find(taskId);
            if (task != null)
            {
                task.TotalTimeSpent = (task.TotalTimeSpent ?? TimeSpan.Zero) + timeSpent;
                _context.SaveChanges();
            }
        }
    }
}