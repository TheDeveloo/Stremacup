using Stremacup.EF;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Stremacup
{
    class API
    {
        public static MainWindow mainWindow;

        public API(MainWindow window)
        {
            mainWindow = window;
        }

        public async void getTeamsHTTP()
        {
            Encoding utf8 = Encoding.UTF8;

            string address = "http://streethockeycup.ch/index.php/api.xml";
            System.Net.WebClient webclient = new WebClient();
            webclient.Encoding = utf8;
            String content = webclient.DownloadString(address);

            var em = new stremacupEntities();
            team newTeam = new team();
            int number = 0;
            int i = 0;
            // Create an XmlReader
            using (XmlReader reader = XmlReader.Create(new StringReader(content)))
            {
                // Parse the file and display each of the nodes.
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name == "equipe")
                            {
                                newTeam = new team();
                                reader.MoveToFirstAttribute();
                                newTeam.id = long.Parse(reader.Value);
                                reader.MoveToElement();
                            }
                            else if (reader.Name == "nombre")
                            {
                                reader.Read();
                                number = int.Parse(reader.Value);
                            }
                            else if (reader.Name == "nom")
                            {
                                reader.Read();
                                newTeam.name = reader.Value;
                            }
                            else if (reader.Name == "cat")
                            {
                                category cat = new category();

                                reader.Read();
                                if (!em.category.Any(o => o.name == reader.Value))
                                {
                                    cat.name = reader.Value;
                                    em.category.Add(cat);
                                    try
                                    {
                                        em.SaveChanges();
                                    }
                                    catch (Exception error)
                                    {
                                        Console.WriteLine(error.ToString());
                                    }
                                }
                                else
                                {
                                    cat = em.category.Where(c => c.name == reader.Value).First();
                                }
                                newTeam.cat_fk = cat.id;
                            }
                            break;
                        case XmlNodeType.Text:
                            break;
                        case XmlNodeType.XmlDeclaration:
                        case XmlNodeType.ProcessingInstruction:
                        case XmlNodeType.Comment:
                            break;
                        case XmlNodeType.EndElement:
                            if (reader.Name == "equipe")
                            {
                                em.team.Add(newTeam);
                                i++;
                                mainWindow.setRecuperationInfos("(" + i + "/" + number + ") " + newTeam.name);
                                try
                                {
                                    em.SaveChanges();
                                }
                                catch (DbEntityValidationException error)
                                {
                                    Console.WriteLine("-------- INSERT TEAM -----" + newTeam.name + "------" + error.ToString());
                                    foreach (var eve in error.EntityValidationErrors)
                                    {
                                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                                        foreach (var ve in eve.ValidationErrors)
                                        {
                                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                                ve.PropertyName, ve.ErrorMessage);
                                        }
                                    }
                                }
                            }
                            break;
                    }
                }
            }

            em.SaveChanges();
        }
    }
}
