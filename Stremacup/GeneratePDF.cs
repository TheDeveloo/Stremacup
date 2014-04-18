using iTextSharp.text;
using iTextSharp.text.pdf;
using Stremacup.EF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stremacup
{
    class GeneratePDF
    {
        stremacupEntities em;

        public GeneratePDF()
        {
            this.em = new stremacupEntities();
        }

        public void generateForCategories()
        {
            foreach (category category in em.category)
            {
                var pdfDoc = new Document();
                PdfWriter.GetInstance(pdfDoc, new FileStream(category.name + ".pdf", FileMode.Create));
                pdfDoc.Open();

                Paragraph paragraph = new Paragraph(50f, "Matchs " + category.name);
                paragraph.SpacingAfter = 50f;
                pdfDoc.Add(paragraph);

                PdfPTable table = new PdfPTable(4);
                table.WidthPercentage = 100;
                table.AddCell("Heure");
                table.AddCell("Groupe");
                table.AddCell("Équipe 1");
                table.AddCell("Équipe 2");

                foreach (group group in category.group)
                {
                    foreach (team team in group.team)
                    {
                        foreach (match match in team.match)
                        {
                            table.AddCell(match.datetime.ToString());
                            table.AddCell(match.team.group.name);
                            table.AddCell(match.team.name);
                            table.AddCell(match.team1.name);
                        }
                    }
                }
                pdfDoc.Add(table);

                pdfDoc.Close();
            }
        }

        public void generateForGroups()
        {
            foreach (group group in em.group)
            {
                var pdfDoc = new Document();
                PdfWriter.GetInstance(pdfDoc, new FileStream(group.name+".pdf", FileMode.Create));
                pdfDoc.Open();

                Paragraph paragraph = new Paragraph(50f, "Matchs "+group.name+" "+group.category.name);
                paragraph.SpacingAfter = 50f;
                pdfDoc.Add(paragraph);

                PdfPTable table = new PdfPTable(3);
                table.WidthPercentage = 100;
                table.AddCell("Heure");
                table.AddCell("Équipe 1");
                table.AddCell("Équipe 2");

                foreach (team team in group.team)
                {
                    foreach (match match in team.match)
                    {
                        table.AddCell(match.datetime.ToString());
                        table.AddCell(match.team.name);
                        table.AddCell(match.team1.name);
                    }
                }
                pdfDoc.Add(table);

                pdfDoc.Close();
            }
        }

        public void generateForFields()
        {
            foreach (field field in em.field)
            {
                var pdfDoc = new Document();
                PdfWriter.GetInstance(pdfDoc, new FileStream(field.name + ".pdf", FileMode.Create));
                pdfDoc.Open();

                Paragraph paragraph = new Paragraph(50f, "Matchs " + field.name);
                paragraph.SpacingAfter = 50f;
                pdfDoc.Add(paragraph);

                PdfPTable table = new PdfPTable(4);
                table.WidthPercentage = 100;
                table.AddCell("Heure");
                table.AddCell("Catégorie");
                table.AddCell("Équipe 1");
                table.AddCell("Équipe 2");

                foreach (match match in field.match)
                {
                        table.AddCell(match.datetime.ToString());
                        table.AddCell(match.team.category.name);
                        table.AddCell(match.team.name);
                        table.AddCell(match.team1.name);
                }
                pdfDoc.Add(table);

                pdfDoc.Close();
            }
        }

        public void generateForTeams()
        {
            foreach (team team in em.team)
            {
                var pdfDoc = new Document();
                PdfWriter.GetInstance(pdfDoc, new FileStream(team.name + ".pdf", FileMode.Create));
                pdfDoc.Open();

                Paragraph paragraph = new Paragraph(50f, "Matchs " + team.name + " " + team.category.name);
                paragraph.SpacingAfter = 50f;
                pdfDoc.Add(paragraph);

                PdfPTable table = new PdfPTable(3);
                table.WidthPercentage = 100;
                table.AddCell("Heure");
                table.AddCell("Équipe 1");
                table.AddCell("Équipe 2");

                foreach (match match in team.match)
                {
                    table.AddCell(match.datetime.ToString());
                    table.AddCell(match.team.name);
                    table.AddCell(match.team1.name);
                }
                foreach (match match in team.match1)
                {
                    table.AddCell(match.datetime.ToString());
                    table.AddCell(match.team.name);
                    table.AddCell(match.team1.name);
                }

                pdfDoc.Add(table);
                pdfDoc.Close();
            }
        }
    }
}
