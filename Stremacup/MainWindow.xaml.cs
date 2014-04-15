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
        }

        public void setRecuperationInfos(string infos)
        {
            recuperationStatus.Text = infos;
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            API api = new API(this);
            api.getTeamsHTTP();
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
            catch (FormatException) { }

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
    }
}
