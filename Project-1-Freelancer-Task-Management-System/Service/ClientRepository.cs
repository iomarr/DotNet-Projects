using Freelancer_Task.Data;
using Freelancer_Task.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Freelancer_Task.Services
{
    public class ClientRepository
    {
        private readonly FreelancerContext _context;

        public ClientRepository(FreelancerContext context)
        {
            _context = context;
        }

        public List<Client> GetAllClients()
        {
            return _context.Clients.ToList();
        }

        public void AddClient(Client client)
        {
            _context.Clients.Add(client);
            _context.SaveChanges();
        }

        public void UpdateClient(Client client)
        {
            client.ModifiedDate = DateTime.Now;
            _context.Entry(client).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void DeleteClient(int id)
        {
            var client = _context.Clients.Find(id);
            if (client != null)
            {
                _context.Clients.Remove(client);
                _context.SaveChanges();
            }
        }

        public List<Client> SearchClients(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return _context.Clients.ToList();

            searchTerm = searchTerm.ToLower();

            return _context.Clients
                .Where(c =>
                    c.Name.ToLower().Contains(searchTerm) ||
                    c.Email.ToLower().Contains(searchTerm) ||
                    c.Company.ToLower().Contains(searchTerm) ||
                    c.Phone.Contains(searchTerm))
                .ToList();
        }
    }
}