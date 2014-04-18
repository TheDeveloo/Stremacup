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
                string catName = removeSpecialCharacters(category.name);
                if(!Directory.Exists("Categories"))
                    Directory.CreateDirectory("Categories");
                PdfWriter.GetInstance(pdfDoc, new FileStream("Categories/"+ catName + ".pdf", FileMode.Create));
                pdfDoc.Open();

                Paragraph paragraph = new Paragraph(50f, "Matchs " + category.name);
                paragraph.SpacingAfter = 50f;
                pdfDoc.Add(paragraph);

                PdfPTable table = new PdfPTable(5);
                table.WidthPercentage = 100;
                table.AddCell("Heure");
                table.AddCell("Terrain");
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
                            if (match.field != null)
                                table.AddCell(match.field.name);
                            else
                                table.AddCell("");
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
                string groupName = removeSpecialCharacters(group.name);
                string catName = removeSpecialCharacters(group.category.name);
                if (!Directory.Exists("Groups"))
                    Directory.CreateDirectory("Groups");
                PdfWriter.GetInstance(pdfDoc, new FileStream("Groups/" + groupName + "_" + catName + ".pdf", FileMode.Create));
                pdfDoc.Open();

                Paragraph paragraph = new Paragraph(50f, "Matchs "+group.name+" "+group.category.name);
                paragraph.SpacingAfter = 50f;
                pdfDoc.Add(paragraph);

                PdfPTable table = new PdfPTable(3);
                table.WidthPercentage = 100;
                table.AddCell("Heure");
                table.AddCell("Terrain");
                table.AddCell("Équipe 1");
                table.AddCell("Équipe 2");

                foreach (team team in group.team)
                {
                    foreach (match match in team.match)
                    {
                        table.AddCell(match.datetime.ToString());
                        if (match.field != null)
                            table.AddCell(match.field.name);
                        else
                            table.AddCell("");
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
                string fieldName = removeSpecialCharacters(field.name);
                if (!Directory.Exists("Fields"))
                    Directory.CreateDirectory("Fields");
                PdfWriter.GetInstance(pdfDoc, new FileStream("Fields/" + fieldName + ".pdf", FileMode.Create));
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
                string name = removeSpecialCharacters(team.name);
                string catName = removeSpecialCharacters(team.category.name);
                if (!Directory.Exists("Teams"))
                    Directory.CreateDirectory("Teams");
                PdfWriter.GetInstance(pdfDoc, new FileStream("Teams/" + name + "_" + catName + ".pdf", FileMode.Create));
                pdfDoc.Open();

                Paragraph paragraph = new Paragraph(50f, "Matchs " + team.name + " " + team.category.name);
                paragraph.SpacingAfter = 50f;
                pdfDoc.Add(paragraph);

                PdfPTable table = new PdfPTable(4);
                table.WidthPercentage = 100;
                table.AddCell("Heure");
                table.AddCell("Terrain");
                table.AddCell("Équipe 1");
                table.AddCell("Équipe 2");

                foreach (match match in team.match)
                {
                    table.AddCell(match.datetime.ToString());
                    if (match.field != null)
                        table.AddCell(match.field.name);
                    else
                        table.AddCell("");
                    table.AddCell(match.team.name);
                    table.AddCell(match.team1.name);
                }
                foreach (match match in team.match1)
                {
                    table.AddCell(match.datetime.ToString());
                    if (match.field != null)
                        table.AddCell(match.field.name);
                    else
                        table.AddCell("");
                    table.AddCell(match.team.name);
                    table.AddCell(match.team1.name);
                }

                pdfDoc.Add(table);
                pdfDoc.Close();
            }
        }

        public string removeSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_' || c == ' ')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
    }
}
