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

        /*
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
        */

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
                // debug(firstList);
                // debug(reverseList);
                Console.WriteLine("after rotation");
                rightRotate(firstList, 1);
                leftRotate(reverseList, 1);
                // debug(firstList);
                // debug(reverseList);

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

        private int searchField(List<List<group>> fieldToGroup, group currentGroup)
        {
            for (int i = 0; i < fieldToGroup.Count; i++)
            {
                for (int j = 0; j < fieldToGroup[i].Count; j++)
                {
                    if (fieldToGroup[i][j].name == currentGroup.name)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public void roundRobin(int matchTime)
        {
            foreach (match match in em.match)
                em.match.Remove(match);
            foreach (group group in em.group)
                em.group.Remove(group);

            em.SaveChanges();

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
            try
            {
                em.SaveChanges();
            }
            catch (Exception exception)
            {
                MessageBox.Show("Erreur lors de la sauvegarde des matchs");
            }

            // round robin
            foreach (group group in em.group)
            {
                List<team> teams = group.team.ToList();
                generateMatches(teams);
            }


            List<List<group>> fieldToGroup = new List<List<group>>();
            int nbFields = this.em.field.Count();
            for (int i = 0; i < nbFields; i++)
            {
                fieldToGroup.Add(new List<group>());
            }
            int counterGroup = 0;
            int nbGroups = this.em.group.Count();
            List<group> groups = this.em.group.ToList();
            List<field> fields = this.em.field.ToList();
            int stepAllGroups = 0;
            while (counterGroup < nbGroups)
            {
                foreach (field field in this.em.field)
                {
                    fieldToGroup[stepAllGroups].Add(groups[counterGroup]);
                }
                stepAllGroups++;
                if (stepAllGroups >= nbFields) stepAllGroups = 0;
                counterGroup++;
            }

            foreach (match match in this.em.match)
            {
                group currentGroup = match.team.group;

                int index = searchField(fieldToGroup, currentGroup);

                match.field = fields[index];
            }

            this.em.SaveChanges();

            // affecter les jours et heures
            foreach (field field in em.field)
            {
                List<matchday> matchdays = this.em.matchday.ToList();
                List<schedule> matchdaysSchedule = matchdays[0].schedule.ToList();
                DateTime matchDayDate0 = matchdays[0].date;
                // DateTime time0 = matchdaysSchedule[0].beginHour;
                DateTime time0 = new DateTime(matchDayDate0.Year,
                                              matchDayDate0.Month,
                                              matchDayDate0.Day,
                                              matchdaysSchedule[0].beginHour.Hour,
                                              matchdaysSchedule[0].beginHour.Minute,
                                              matchdaysSchedule[0].beginHour.Second);
                // DateTime time1 = matchdaysSchedule[0].endHour;
                DateTime time1 = new DateTime(matchDayDate0.Year,
                                              matchDayDate0.Month,
                                              matchDayDate0.Day,
                                              matchdaysSchedule[0].endHour.Hour,
                                              matchdaysSchedule[0].endHour.Minute,
                                              matchdaysSchedule[0].endHour.Second);
                int counterMatchday = 1;
                int counterSchedule = 1;

                DateTime dateTimeCurrent = new DateTime(matchDayDate0.Year,
                                                        matchDayDate0.Month,
                                                        matchDayDate0.Day,
                                                        time0.Hour,
                                                        time0.Minute,
                                                        time0.Second);

                foreach (match match in field.match)
                {
                    match.datetime = dateTimeCurrent;

                    if (dateTimeCurrent < time1)
                    {
                        dateTimeCurrent = dateTimeCurrent.AddMinutes(matchTime);
                    }
                    else
                    {
                        // changement de jours
                        if (counterSchedule >= matchdaysSchedule.Count)
                        {
                            counterSchedule = 0;

                            matchDayDate0 = matchdays[counterMatchday].date;
                            matchdaysSchedule = matchdays[counterMatchday].schedule.ToList();

                            // time0 = matchdaysSchedule[counterSchedule].beginHour;
                            time0 = new DateTime(matchDayDate0.Year,
                              matchDayDate0.Month,
                              matchDayDate0.Day,
                              matchdaysSchedule[counterSchedule].beginHour.Hour,
                              matchdaysSchedule[counterSchedule].beginHour.Minute,
                              matchdaysSchedule[counterSchedule].beginHour.Second);
                            // time1 = matchdaysSchedule[counterSchedule].endHour;
                            time1 = new DateTime(matchDayDate0.Year,
                              matchDayDate0.Month,
                              matchDayDate0.Day,
                              matchdaysSchedule[counterSchedule].endHour.Hour,
                              matchdaysSchedule[counterSchedule].endHour.Minute,
                              matchdaysSchedule[counterSchedule].endHour.Second);

                            dateTimeCurrent = new DateTime(matchDayDate0.Year,
                                                           matchDayDate0.Month,
                                                           matchDayDate0.Day,
                                                           time0.Hour,
                                                           time0.Minute,
                                                           time0.Second);

                            counterMatchday++;

                            if (counterMatchday == matchdays.Count)
                            {
                                Console.WriteLine("Out date");
                                this.em.SaveChanges();
                                return;
                            }
                        }
                        else
                        {
                            // time0 = matchdaysSchedule[counterSchedule].beginHour;
                            time0 = new DateTime(matchDayDate0.Year,
                              matchDayDate0.Month,
                              matchDayDate0.Day,
                              matchdaysSchedule[counterSchedule].beginHour.Hour,
                              matchdaysSchedule[counterSchedule].beginHour.Minute,
                              matchdaysSchedule[counterSchedule].beginHour.Second);
                            // time1 = matchdaysSchedule[counterSchedule].endHour;
                            time1 = new DateTime(matchDayDate0.Year,
                              matchDayDate0.Month,
                              matchDayDate0.Day,
                              matchdaysSchedule[counterSchedule].endHour.Hour,
                              matchdaysSchedule[counterSchedule].endHour.Minute,
                              matchdaysSchedule[counterSchedule].endHour.Second);

                            dateTimeCurrent = new DateTime(matchDayDate0.Year,
                                                           matchDayDate0.Month,
                                                           matchDayDate0.Day,
                                                           time0.Hour,
                                                           time0.Minute,
                                                           time0.Second);
                            counterSchedule++;
                        }
                    }
                }
            }
            this.em.SaveChanges();
        }
    }
}
