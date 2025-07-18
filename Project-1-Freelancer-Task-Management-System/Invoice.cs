using Freelancer_Task.Models;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Freelancer_Task.Services
{
    public class InvoiceService
    {
        public void GenerateInvoice(Project project, decimal hourlyRate, string outputPath)
        {
            if (!project.Tasks.Any())
            {
                throw new Exception("No tasks found for this project");
            }

            // Convert all TimeSpan values to decimal hours first
            var totalHours = (decimal)project.Tasks
                .Sum(t => t.TotalTimeSpent?.TotalHours ?? 0);

            var subtotal = totalHours * hourlyRate;
            var tax = subtotal * 0.15m; // 15% tax
            var total = subtotal + tax;

            var invoice = new StringBuilder();
            invoice.AppendLine($"INVOICE #{DateTime.Now:yyyyMMdd}-{project.Id}");
            invoice.AppendLine($"Date: {DateTime.Now:yyyy-MM-dd}");
            invoice.AppendLine($"Client: {project.Client.Name}");
            invoice.AppendLine($"Project: {project.Name}\n");

            // Tasks Breakdown
            invoice.AppendLine("TASKS:");
            invoice.AppendLine("--------------------------------------------------");
            foreach (var task in project.Tasks)
            {
                var taskHours = (decimal)(task.TotalTimeSpent?.TotalHours ?? 0);
                var taskAmount = taskHours * hourlyRate;

                invoice.AppendLine(
                    $"{task.Title.PadRight(30)}" +
                    $"{taskHours:0.00} hrs".PadLeft(10) +
                    $" x {hourlyRate:C}".PadLeft(12) +
                    $" = {taskAmount:C}".PadLeft(12)
                );
            }

            // Summary
            invoice.AppendLine("\nSUMMARY:");
            invoice.AppendLine("--------------------------------------------------");
            invoice.AppendLine($"Subtotal: {subtotal:C}");
            invoice.AppendLine($"Tax (15%): {tax:C}");
            invoice.AppendLine($"Total Due: {total:C}");
            invoice.AppendLine("\nPayment Due: 30 days");

            // Save to file
            try
            {
                var fileName = $"Invoice_{project.Client.Name}_{DateTime.Now:yyyyMMdd}.txt";
                var fullPath = Path.Combine(outputPath, fileName);
                Directory.CreateDirectory(outputPath);
                File.WriteAllText(fullPath, invoice.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to save invoice: {ex.Message}");
            }
        }
    }
}