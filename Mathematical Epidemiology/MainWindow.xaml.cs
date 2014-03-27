using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.ViewportRestrictions;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Input;

using CompartmentModels;
using CompartmentModels.Analytic;
using System.Collections.Generic;
using System.Windows.Controls;
using CompartmentModels.Imitation;

namespace MathematicalEpidemiology
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IModel model;
        private BackgroundWorker backgroundWorker;
        private CompartmentModelType modelType = 0;
        private int count = 0;
        private IList<State> solution;

        public MainWindow()
        {
            InitializeComponent();
            backgroundWorker = (BackgroundWorker)FindResource("backgroundWorker");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            infectedChart.Description = new PenDescription("Infected");
            susceptibleChart.Description = new PenDescription("Susceptible");
            recoveredChart.Description = new PenDescription("Recovered");

            plotter.Viewport.AutoFitToView = true;
            ViewportAxesRangeRestriction restr = new ViewportAxesRangeRestriction();
            plotter.Viewport.Restrictions.Add(restr);
        }

        private bool IsStochastic()
        {
            return comboBoxType.SelectedIndex != 0;
        }

        private Parameters ParseParameters()
        {
            Parameters parameters = new Parameters();
            parameters.Population = IsStochastic() ? Utils.ParseDoubleInvariantly(inputPopulation.Text) : 1;
            parameters.InfectionRate = Utils.ParseDoubleInvariantly(inputInfectionRate.Text);
            parameters.DiseasePeriod = Utils.ParseDoubleInvariantly(inputRecoveryRate.Text);
            parameters.BirthRate = Utils.ParseDoubleInvariantly(inputBirthRate.Text);
            parameters.SusceptibleRate = Utils.ParseDoubleInvariantly(inputSusceptibleRate.Text);
            parameters.ExposedRate = Utils.ParseDoubleInvariantly(inputExposedRate.Text);
            return parameters;
        }

        private State ParseState(Parameters parameters)
        {
            State state = new State();
            state.Infected = Utils.ParseDoubleInvariantly(inputInfected.Text);
            state.Susceptible = Utils.ParseDoubleInvariantly(inputSusceptible.Text);
            state.Removed = parameters.Population - state.Infected - state.Susceptible;
            return state;
        }

        private int index = 0;
        private double population;

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                index = comboBoxType.SelectedIndex;
                population = Utils.ParseDoubleInvariantly(inputPopulation.Text);
                Parameters parameters = ParseParameters();
                State state = ParseState(parameters);
                count = IsStochastic() ? (int)Utils.ParseDoubleInvariantly(inputCount.Text) : 1;
                double time = Utils.ParseDoubleInvariantly(inputTime.Text);
                double timestep = Utils.ParseDoubleInvariantly(inputTimeStep.Text);
                if (comboBoxType.SelectedIndex == 2)
                {
                    model = new ImitationModel(state, parameters, time);
                }
                else
                {
                    model = AnalyticModelFactory.CreateModel(
                        (CompartmentModelType)modelType,
                        IsStochastic(),
                        state,
                        parameters,
                        time,
                        timestep);
                }
                progressBar.Value = 0;
                lblStatus.Text = "Busy";
                backgroundWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BackgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                IList<State> tmp;
                for (int i = 0; i < count; i++)
                {
                    if (i == 0)
                    {
                        solution = model.Run();
                    }
                    else
                    {
                        tmp = model.Run();
                        for (int j = 0; j < solution.Count; j++)
                        {
                            solution[j] += tmp[j];
                        }
                    }
                }
                for (int j = 0; j < solution.Count; j++)
                {
                    solution[j] /= count;
                }
                if (index == 0)
                {
                    for (int j = 0; j < solution.Count; j++)
                    {
                        solution[j] *= population;
                    }
                }
                backgroundWorker.ReportProgress(100);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BackgroundWorker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            UpdateChart();
            lblStatus.Text = "Ready";
        }

        private void UpdateChart()
        {
            EnumerableDataSource<State> infectedDataSource = new EnumerableDataSource<State>(solution);
            infectedDataSource.SetXYMapping(state => new Point(state.Time, state.Infected));
            infectedChart.DataSource = infectedDataSource;

            EnumerableDataSource<State> susceptibleDataSource = new EnumerableDataSource<State>(solution);
            susceptibleDataSource.SetXYMapping(state => new Point(state.Time, state.Susceptible));
            susceptibleChart.DataSource = susceptibleDataSource;

            EnumerableDataSource<State> removedDataSource = new EnumerableDataSource<State>(solution);
            removedDataSource.SetXYMapping(state => new Point(state.Time, state.Removed));
            recoveredChart.DataSource = removedDataSource;

            plotter.Viewport.FitToView();
        }

        private bool isStochasticOld = false;

        private void ComboBoxType_SelectionChanged(object sender, RoutedEventArgs e)
        {
            //inputPopulation.IsEnabled = IsStochastic();
            inputCount.IsEnabled = IsStochastic();
            try
            {
                if (isStochasticOld != IsStochastic())
                {
                    double infected = Utils.ParseDoubleInvariantly(inputInfected.Text);
                    double susceptible = Utils.ParseDoubleInvariantly(inputSusceptible.Text);
                    double population = Utils.ParseDoubleInvariantly(inputPopulation.Text);
                    inputInfected.Text = IsStochastic() ?
                        Math.Round(infected * population).ToString() :
                        Math.Round(infected / population, 5).ToString();
                    inputSusceptible.Text = IsStochastic() ?
                        Math.Round(susceptible * population).ToString() :
                        Math.Round(susceptible / population, 5).ToString();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Automatic modification failed");
            }
            finally
            {
                isStochasticOld = IsStochastic();
            }
        }

        private void plotter_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            (sender as ChartPlotter).Viewport.FitToView();
        }

        private void CheckBox_Checked_1(object sender, RoutedEventArgs e)
        {
            infectedChart.Visibility = infectedChart.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
            plotter.Viewport.FitToView();
        }

        private void CheckBox_Checked_2(object sender, RoutedEventArgs e)
        {
            susceptibleChart.Visibility = susceptibleChart.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
            plotter.Viewport.FitToView();
        }

        private void CheckBox_Checked_3(object sender, RoutedEventArgs e)
        {
            recoveredChart.Visibility = recoveredChart.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
            plotter.Viewport.FitToView();
        }

        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            modelType = (CompartmentModelType)(sender as ComboBox).SelectedIndex;
        }

        private void btnHide_Click(object sender, RoutedEventArgs e)
        {

        }

        public class ViewportAxesRangeRestriction : IViewportRestriction
        {

            public Rect Apply(Rect oldVisible, Rect newVisible, Viewport2D viewport)
            {
                if (newVisible.X < 0)
                {
                    newVisible.X = 0;
                }

                if (newVisible.Y < 0)
                {
                    newVisible.Y = 0;
                }

                return newVisible;
            }

            public event EventHandler Changed;
        }
    }
}
