using Freelancer_Task.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using Font = iTextSharp.text.Font;
using Document = iTextSharp.text.Document;
using Chunk = iTextSharp.text.Chunk;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace Freelancer_Task.Services
{
    public class ReportExport
    {
        // CSV Export
        public void ExportToCsv(List<WorkLog> workLogs, string filePath)
        {
            var csv = new StringBuilder();
            // Header
            csv.AppendLine("Date,Client,Project,Task,Time Spent,Status");

            // Data Rows
            foreach (var log in workLogs)
            {
                csv.AppendLine($"\"{log.Date:yyyy-MM-dd}\","
                    + $"\"{log.ClientName}\","
                    + $"\"{log.ProjectName}\","
                    + $"\"{log.TaskTitle}\","
                    + $"\"{log.TimeSpent:hh\\:mm}\","
                    + $"\"{log.Status}\"");
            }

            File.WriteAllText(filePath, csv.ToString());
        }

        // PDF Export
        public void ExportToPdf(List<WorkLog> workLogs, string filePath)
        {
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                var doc = new Document(PageSize.A4.Rotate());
                PdfWriter.GetInstance(doc, stream);

                doc.Open();

                // Title
                doc.Add(new Paragraph("WORK LOG REPORT",
                    new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD)));
                doc.Add(Chunk.NEWLINE);

                // Table
                var table = new PdfPTable(6);
                table.WidthPercentage = 100;

                // Headers
                table.AddCell("Date");
                table.AddCell("Client");
                table.AddCell("Project");
                table.AddCell("Task");
                table.AddCell("Time Spent");
                table.AddCell("Status");

                // Data
                foreach (var log in workLogs)
                {
                    table.AddCell(log.Date.ToString("yyyy-MM-dd"));
                    table.AddCell(log.ClientName);
                    table.AddCell(log.ProjectName);
                    table.AddCell(log.TaskTitle);
                    table.AddCell(log.TimeSpent.ToString(@"hh\:mm"));
                    table.AddCell(log.Status.ToString());
                }

                doc.Add(table);
                doc.Close();
            }
        }
    }
}