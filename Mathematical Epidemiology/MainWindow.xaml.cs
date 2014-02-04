using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.ViewportRestrictions;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Input;

using AnalyticModels;

namespace MathematicalEpidemiology
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CompartmentModel model;
        private BackgroundWorker backgroundWorker;

        private CompartmentModelType modelType = 0;
        bool isStochastic = false;

        public MainWindow()
        {
            InitializeComponent();
            backgroundWorker = (BackgroundWorker)FindResource("backgroundWorker");
        }

        private double ParseDoubleInvariantly(string s)
        {
            return double.Parse(
                s.Replace(",", CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator),
                CultureInfo.InvariantCulture);
        }

        private Parameters ParseParameters()
        {
            Parameters parameters = new Parameters();
            isStochastic = checkStochastic.IsChecked == true;
            if (isStochastic)
            {
                parameters.Population = ParseDoubleInvariantly(inputPopulation.Text);
            }
            parameters.InfectionRate = ParseDoubleInvariantly(inputInfectionRate.Text);
            parameters.RecoveryRate = ParseDoubleInvariantly(inputRecoveryRate.Text);
            parameters.BirthRate = ParseDoubleInvariantly(inputBirthRate.Text);
            parameters.SusceptibleRate = ParseDoubleInvariantly(inputSusceptibleRate.Text);
            parameters.ExposedRate = ParseDoubleInvariantly(inputExposedRate.Text);
            return parameters;
        }

        private State ParseState(Parameters parameters)
        {
            State state = new State();
            state.Infected = ParseDoubleInvariantly(inputInfected.Text);
            state.Susceptible = ParseDoubleInvariantly(inputSusceptible.Text);
            state.Removed = parameters.Population - state.Infected - state.Susceptible;
            return state;
        }

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Parameters parameters = ParseParameters();
                State state = ParseState(parameters);
                model = CompartmentModelFactory.CreateModel(
                    (CompartmentModelType)modelType, 
                    isStochastic,
                    state,
                    parameters,
                    ParseDoubleInvariantly(inputTime.Text),
                    ParseDoubleInvariantly(inputTimeStep.Text));
                progressBar.Value = 0;
                lblStatus.Text = "Busy";
                backgroundWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private double[] time;
        private double[] infected;
        private double[] susceptible;
        private double[] recovered;

        private void BackgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            double[,] solution;
            try
            {
                solution = model.Run();
                CreateChartPoints(solution);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CreateChartPoints(double[,] solution)
        {
            backgroundWorker.ReportProgress(50);
            int n = solution.Length / (model.CompartmentCount + 1);
            infected = new double[n];
            susceptible = new double[n];
            recovered = new double[n];
            time = new double[n];
            for (int i = 0; i < n; i++)
            {
                infected[i] = solution[i, 2];
                susceptible[i] = solution[i, 1];
                recovered[i] = solution[i, 3];
                time[i] = solution[i, 0];
            }
            backgroundWorker.ReportProgress(100);
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
            var daysDataSource = new EnumerableDataSource<double>(time);
            daysDataSource.SetXMapping(x => x);

            var infectedDataSource = new EnumerableDataSource<double>(infected);
            infectedDataSource.SetYMapping(y => y);

            var susceptibleDataSource = new EnumerableDataSource<double>(susceptible);
            susceptibleDataSource.SetYMapping(y => y);

            var recoveredDataSource = new EnumerableDataSource<double>(recovered);
            recoveredDataSource.SetYMapping(y => y);

            CompositeDataSource compositeInfectedDataSource = new
                CompositeDataSource(daysDataSource, infectedDataSource);

             CompositeDataSource compositeSusceptibleDataSource = new
                CompositeDataSource(daysDataSource, susceptibleDataSource);

             CompositeDataSource compositeRecoveredDataSource = new
                CompositeDataSource(daysDataSource, recoveredDataSource);

            infectedChart.DataSource = compositeInfectedDataSource;
            susceptibleChart.DataSource = compositeSusceptibleDataSource;
            recoveredChart.DataSource = compositeRecoveredDataSource;
            plotter.Viewport.FitToView();
        }

        private void ModelTypes_Checked(object sender, RoutedEventArgs e)
        {
            if (rbSIR.IsChecked == true)
            {
                modelType = CompartmentModelType.SIR;
            }
            else if (rbSIS.IsChecked == true)
            {
                modelType = CompartmentModelType.SIS;
            }
            else if (rbSEIR.IsChecked == true)
            {
                modelType = CompartmentModelType.SEIR;
            }
            else if (rbSIRS.IsChecked == true)
            {
                modelType = CompartmentModelType.SIRS;
            }
        }

        private void checkStochastic_Checked(object sender, RoutedEventArgs e)
        {
            inputPopulation.IsEnabled = !inputPopulation.IsEnabled;
            try
            {
                double infected = ParseDoubleInvariantly(inputInfected.Text);
                double susceptible = ParseDoubleInvariantly(inputSusceptible.Text);
                double population = ParseDoubleInvariantly(inputPopulation.Text);
                inputInfected.Text = checkStochastic.IsChecked == true ?
                    Math.Round(infected * population).ToString() :
                    Math.Round(infected / population, 5).ToString();
                inputSusceptible.Text = checkStochastic.IsChecked == true ?
                    Math.Round(susceptible * population).ToString() :
                    Math.Round(susceptible / population, 5).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Automatic modification failed");
            }
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

        private void plotter_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            (sender as ChartPlotter).Viewport.FitToView();
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

        private void btnAllCharts_Click(object sender, RoutedEventArgs e)
        {
            susceptibleChart.Visibility = Visibility.Visible;
            infectedChart.Visibility = Visibility.Visible;
            recoveredChart.Visibility = Visibility.Visible;
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
 
    }
}
