using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stremacup.EF;

namespace Stremacup
{
    class GenereateMatches
    {
        private stremacupEntities em;

        public GenereateMatches(stremacupEntities stremacupEntities)
        {
            this.em = stremacupEntities;
        }

        public void generateMatches(List<team> teams)
        {
            if (teams.Count % 2 == 0)
            {
                work(teams);
            }
            else
            {
                teams.Add(null);
                work(teams);
            }
        }

        /* Rotation */
        void leftRotate(List<team> teams, int d)
        {
            int i;
            for (i = 0; i < d; i++)
            {
                leftRotatebyOne(teams);
            }
        }
 
        void leftRotatebyOne(List<team> teams)
        {
            team temp = null;
            temp = teams[0];
            int i = 0;
            Console.WriteLine("Left rotation " + i);
            for (i = 0; i < teams.Count - 1; i++)
            {
                teams[i] = teams[i + 1];
            }
            teams[i] = temp;
        }

        void rightRotate(List<team> teams, int d)
        {
            int i;
            for (i = 0; i < d; i++)
            {
                rightRotatebyOne(teams);
            }
        }
 
        void rightRotatebyOne(List<team> teams)
        {
            int i = 0;
            team temp = null;
            temp = teams[teams.Count - 1];
            Console.WriteLine("Right rotation " + i);
            for (i = teams.Count - 1; i > 0; i--)
            {
                teams[i] = teams[i - 1];
            }
            teams[0] = temp;
        }
        /* fin rotation */

        public void debug(List<team> teams)
        {
            foreach (team team in teams)
            {
                if (team != null)
                {
                    Console.Write(team.id);
                }
            }
            Console.WriteLine();
        }

        public void work(List<team> teams)
        {
            List<team> firstList = new List<team>();
            List<team> reverseList = new List<team>();

            Console.WriteLine(teams.Count);

            for (int k = 1; k < teams.Count; k++)
            {
                if (k < teams.Count / 2)
                {
                    firstList.Add(teams[k]);
                }
                else
                {
                    reverseList.Add(teams[k]);
                }
            }

            for (int i = 0; i < teams.Count - 1; i++)
            {
                Console.WriteLine("before rotation");
                debug(firstList);
                debug(reverseList);
                Console.WriteLine("after rotation");
                rightRotate(firstList, 1);
                leftRotate(reverseList, 1);
                debug(firstList);
                debug(reverseList);

                // Collections.rotate(firstList, 1);
                // Collections.rotate(reverseList, -1);

                team temp = firstList[0];
                firstList[0] = reverseList[reverseList.Count - 1];
                reverseList[reverseList.Count - 1] = temp;

                if (teams[0] != null && reverseList[0] != null)
                {
                    // créer un nouveau match
                    match match = new match();
                    match.team = teams[0];
                    match.team1 = reverseList[0];

                    this.em.match.Add(match);
                }            

                for (int j = 0; j < teams.Count / 2 - 1; j++)
                {
                    if (firstList[j] == null || reverseList[j + 1] == null)
                    {
                        continue;
                    }

                    // créer un nouveau match
                    match match = new match();
                    match.team = firstList[j];
                    match.team1 = reverseList[j + 1];

                    this.em.match.Add(match);

                }
            }

            this.em.SaveChanges();
        }

        public void roundRobin()
        {
            foreach (category category in em.category)
            {
                int nbTeamsByGroup = 0;
                int nbTeams = category.team.Count;

                List<int> div = new List<int>();

                for (int i = 2; i < nbTeams; i++)
                {
                    if (nbTeams % i == 0 || (nbTeams + 1) % i == 0)
                    {
                        div.Add(i);
                    }
                }

                var dialog = new DialogQuestion();
                dialog.lblQuestion.Text = "Nombre d'équipe totals: " + nbTeams;
                dialog.lblQuestion.Text += "\nNombre d'équipes possibles pour cette catégorie " + category.name + ":\n";
                foreach (int i in div)
                {
                    dialog.lblQuestion.Text += i.ToString() + ", ";
                }

                if (dialog.ShowDialog() == true)
                {
                    nbTeamsByGroup = Convert.ToInt32(dialog.ResponseTextBox.Text);
                    // TODO vérifier la saisi de l'utilisateur
                }

                // former un group
                // int nbGroups = nbTeams / nbTeamsByGroup;
                int counter = 0;
                group group = new group();
                group.name = category.name + "groupe0";
                group.category = category;
                em.group.Add(group);
                int groupNumber = 0;
                foreach (team team in category.team)
                {
                    if (counter >= nbTeamsByGroup)
                    {
                        counter = 0;
                        group = new group();
                        groupNumber++;
                        group.name = category.name + "group" + groupNumber;
                        group.category = category;
                        em.group.Add(group);
                    }
                    group.team.Add(team);
                    counter++;
                }
            }
            em.SaveChanges();

            // round robin
            foreach (group group in em.group)
            {
                List<team> teams = group.team.ToList();
                generateMatches(teams);
            }
        }
    }
}
