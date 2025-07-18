using Freelancer_Task.Data;
using Freelancer_Task.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Freelancer_Task.Services
{
    public class ProjectRepository
    {
        private readonly FreelancerContext _context;

        public ProjectRepository(FreelancerContext context)
        {
            _context = context;
        }

        public void AddProject(Project project) => _context.Projects.Add(project);
        public void UpdateProject(Project project) => _context.Projects.Update(project);
        public void DeleteProject(int id) => _context.Projects.Remove(_context.Projects.Find(id));
        public Project GetProjectById(int id) => _context.Projects.Find(id);
        public List<Project> GetAllProjects() => _context.Projects.OrderByDescending(p => p.CreatedAt).ToList();

        public List<Project> GetProjectsByStatus(ProjectStatus status)
        {
            return _context.Projects
                .Where(p => p.Status == status)
                .ToList();
        }

        public void SaveChanges() => _context.SaveChanges();


        public Project GetProjectWithTasks(int projectId)
        {
            return _context.Projects
                .Include(p => p.Tasks)
                .FirstOrDefault(p => p.Id == projectId);
        }
    }
}