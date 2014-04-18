using Stremacup.EF;
using System.Windows;
﻿using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Threading;

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

namespace Stremacup
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        stremacupEntities em;

        public MainWindow()
        {
            InitializeComponent();

            this.em = new stremacupEntities();
            teamsListView.ItemsSource = em.team.ToList();
        }

        public void setRecuperationInfos(string infos)
        {
            recuperationStatus.Text = infos;
            matchsGrid.ItemsSource = em.match.ToList();
        }


        public void refreshTeamList()
        {
            teamsListView.ItemsSource = em.team.ToList();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            API api = new API(this);
            Thread loadThread = new Thread(new ThreadStart(api.getTeamsHTTP));
            loadThread.Start();
        }

        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.lbxDays.Items.Clear();
            this.lbxSchedules.Items.Clear();
            this.lbxPlaces.Items.Clear();

            foreach (matchday matchday in this.em.matchday)
            {
                this.lbxDays.Items.Add(matchday);
            }

            foreach (place place in this.em.place)
            {
                this.lbxPlaces.Items.Add(place);
            }
        }

        private void lbxDays_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            matchday matchday = (matchday)this.lbxDays.SelectedItem;

            this.lbxSchedules.Items.Clear();

            foreach (schedule schedule in matchday.schedule)
            {
                this.lbxSchedules.Items.Add(schedule);
            }
        }

        private void lbxSchedules_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnAddDay_Click(object sender, RoutedEventArgs e)
        {
            DateTime? dateTime = this.datePicker.SelectedDate;

            if (dateTime == null)
            {
                MessageBox.Show("Date note selected!", "Error");
            }
            else
            {
                matchday matchday = new matchday();
                matchday.date = dateTime.Value;

                this.em.matchday.Add(matchday);
                this.em.SaveChanges();

                this.lbxDays.Items.Add(matchday);
            }
        }

        private void btnAddSchedule_Click(object sender, RoutedEventArgs e)
        {
            matchday selectedMatchday = (matchday)this.lbxDays.SelectedItem;

            int startHour = 0, startMinute = 0, endHour = 0, endMinute = 0;

            try
            {
                startHour = Convert.ToInt32(this.tbxHStart.Text);
                startMinute = Convert.ToInt32(this.tbxMStart.Text);
                endHour = Convert.ToInt32(this.tbxHEnd.Text);
                endMinute = Convert.ToInt32(this.tbxMEnd.Text);
            }
            catch (FormatException) { Console.WriteLine("btnAddSchedule Exception!"); }

            if (selectedMatchday != null && startHour != 0 && startMinute != 0 && endHour != 0 && endMinute != 0)
            {
                DateTime startDateTime = new DateTime(1, 1, 1, startHour, startMinute, 0);
                DateTime endDateTime = new DateTime(1, 1, 1, endHour, endMinute, 0);

                schedule schedule = new schedule();
                schedule.beginHour = startDateTime;
                schedule.endHour = endDateTime;
                schedule.matchday = selectedMatchday;

                this.em.schedule.Add(schedule);
                this.em.SaveChanges();

                this.lbxSchedules.Items.Add(schedule);
            }
            else
            {
                MessageBox.Show("No matchday selected or datetime error!", "Error");
            }
        }

        private void btnAddPlace_Click(object sender, RoutedEventArgs e)
        {
            string placeName = this.tbxPlaceName.Text;
            int numberOfFields = 0;
            try
            {
                numberOfFields = Convert.ToInt32(this.tbxNumberOfFields.Text);
            }
            catch (FormatException) { }

            if (placeName != "" && numberOfFields != 0)
            {
                place place = new place();
                place.name = placeName;

                this.em.place.Add(place);

                for (int i = 0; i < numberOfFields; i++)
                {
                    field field = new field();
                    field.name = placeName + "_field_" + i;

                    field.place = place;

                    this.em.field.Add(field);
                }

                this.em.SaveChanges();

                this.lbxPlaces.Items.Add(place);
            }
            else
            {
                MessageBox.Show("Place name empty or number of fields missed!", "Error");
            }
        }

        private void btnGeneratePDF_Click(object sender, RoutedEventArgs e)
        {
            int matchTime = Convert.ToInt32(this.tbxMatchTime.Text);
            GenereateMatches gen = new GenereateMatches(this.em);
            gen.roundRobin(matchTime);

            /*
            GeneratePDF pdfGenerator = new GeneratePDF();

            if (categoryCheckBox.IsChecked == true)
                // Category pdf
                pdfGenerator.generateForCategories();

            if (groupCheckBox.IsChecked == true)
                // Group pdf
                pdfGenerator.generateForGroups();

            if (fieldCheckBox.IsChecked == true)
                // Field pdf
                pdfGenerator.generateForFields();

            if (teamCheckBox.IsChecked == true)
                // Team pdf
                pdfGenerator.generateForTeams();
            */
        }

        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            matchsGrid.ItemsSource = em.match.ToList();
        }

        private void matchsGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            em.SaveChanges();
        }
    }
}
