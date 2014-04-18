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

        // round robin
        //public void generateMatchdays(Championship championship) {
        //    List<MatchDay> matchdayList = new ArrayList<>();
        //    List<Team> teamList = mainController.getChampionshipTeams(championship);
        //    String message = "";

        //    if (teamList.size() % 2 == 0) {
        //        work(matchdayList, teamList, championship);
        //    } else {
        //        teamList.add(new Team(-1, message, null, null));
        //        message = work(matchdayList, teamList, championship);
        //    }

        //    FacesMessage msg = new FacesMessage(message, "INFO MSG");
        //    msg.setSeverity(FacesMessage.SEVERITY_INFO);
        //    FacesContext.getCurrentInstance().addMessage(null, msg);
        //}

        //public String work(List<MatchDay> matchdayList, List<Team> teamList, Championship championship) {
        //    String message = " | ";
        //    List<Team> firstList = new ArrayList<>();
        //    List<Team> reverseList = new ArrayList<>();

        //    for (int k = 1; k < teamList.size(); k++) {
        //        if (k < teamList.size() / 2) {
        //            firstList.add(teamList.get(k));
        //        } else {
        //            reverseList.add(teamList.get(k));
        //        }
        //    }

        //    for (int i = 0; i < teamList.size() - 1; i++) {
        //        MatchDay matchDay = new MatchDay();
        //        matchDay.setChampionship(championship);
        //        List<Match> matches = new ArrayList<>();

        //        Collections.rotate(firstList, 1);
        //        Collections.rotate(reverseList, -1);
        //        Team temp = firstList.get(0);
        //        firstList.set(0, reverseList.get(reverseList.size() - 1));
        //        reverseList.set(reverseList.size() - 1, temp);

        //        if (teamList.get(0).getId() != -1 && reverseList.get(0).getId() != -1) {
        //            Match match1 = new Match();
        //            match1.setTeam1(teamList.get(0));
        //            match1.setTeam2(reverseList.get(0));
        //            match1.setMatchDay(matchDay);
        //            matches.add(match1);
        //        }            

        //        if (!("".equals(teamList.get(0).getName())) && !("".equals(reverseList.get(0).getName()))) {
        //            message += teamList.get(0).getName() + " vs ";
        //            message += reverseList.get(0).getName();
        //            message += " | ";
        //        }

        //        for (int j = 0; j < teamList.size() / 2 - 1; j++) {
        //            if (firstList.get(j).getId() == -1 || reverseList.get(j + 1).getId() == -1) {
        //                continue;
        //            }
        //            Match match = new Match();
        //            match.setTeam1(firstList.get(j));
        //            match.setTeam2(reverseList.get(j + 1));
        //            match.setMatchDay(matchDay);

        //            // match.setMatchDay(matchday);
        //            matches.add(match);

        //            if (!("".equals(firstList.get(j).getName())) && !("".equals(reverseList.get(j + 1).getName()))) {
        //                message += firstList.get(j).getName() + " vs ";
        //                message += reverseList.get(j + 1).getName();
        //                message += " | ";
        //            }

        //        }

        //        // save the matchday
        //        matchDay.setMatches(matches);
        //        mainController.persistMatchday(matchDay);
        //    }

        //    return message;
        //}

        public void roundRobin()
        {
            /*
            var dialog = new DialogQuestion();

            if (dialog.ShowDialog() == true)
            {
                MessageBox.Show("You said: " + dialog.ResponseText);
            }
            */

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
                    Console.WriteLine(team.name + " " + group.name);
                    group.team.Add(team);
                    counter++;
                }
            }
            em.SaveChanges();
        }
    }
}
