using Freelancer_Task.Models;
using System.Net.Mail;

namespace Freelancer_Task.Services
{
    public class ClientValidator
    {
        public static bool IsValid(Client client, out string error)
        {
            error = "";

            if (string.IsNullOrWhiteSpace(client.Name))
            {
                error = "Name is required";
                return false;
            }

            if (!IsValidEmail(client.Email))
            {
                error = "Invalid email format";
                return false;
            }

            return true;
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}