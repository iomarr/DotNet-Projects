using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Freelancer_Task.Data;
using Freelancer_Task.Models;

namespace Freelancer_Task.Service
{
    public class ClientService
    {
        private readonly FreelancerContext _context;

        public ClientService()
        {
            _context = new FreelancerContext();
        }

        public List<Client> GetAllClients(string search = "")
        {
            var query = _context.Clients.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c => c.Name.Contains(search) || c.Company.Contains(search));
            }

            return query.ToList();
        }

        public void AddClient(Client client)
        {
            _context.Clients.Add(client);
            _context.SaveChanges();
        }
    }
}
