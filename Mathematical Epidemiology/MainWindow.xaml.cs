using MathematicalEpidemiology.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MathematicalEpidemiology
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CompartmentModel model;
        private State state = new State();
        private Parameters parameters = new Parameters();

        private BackgroundWorker backgroundWorker;

        private double[,] solution;

        CompartmentModelType modelType = 0;

        public MainWindow()
        {
            InitializeComponent();
            lineI.DataPointStyle = GetNewDataPointStyle(255, 0, 0);
            backgroundWorker = (BackgroundWorker)FindResource("backgroundWorker");
        }

        private static Style GetNewDataPointStyle(byte r, byte g, byte b)
        {
            Color background = Color.FromRgb(r, g, b);
            Style style = new Style(typeof(DataPoint));
            Setter st1 = new Setter(DataPoint.BackgroundProperty, new SolidColorBrush(background));
            Setter st2 = new Setter(DataPoint.BorderBrushProperty, new SolidColorBrush(Colors.White));
            Setter st3 = new Setter(DataPoint.BorderThicknessProperty, new Thickness(0.1));
            Setter st4 = new Setter(DataPoint.TemplateProperty, null);
            style.Setters.Add(st1);
            style.Setters.Add(st2);
            style.Setters.Add(st3);
            style.Setters.Add(st4);
            return style;
        }

        private void checkStochastic_Checked_1(object sender, RoutedEventArgs e)
        {
            inputPopulation.IsEnabled = !inputPopulation.IsEnabled;
        }

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool isStochastic = checkStochastic.IsChecked == true;
                if (isStochastic)
                {
                    parameters.Population = double.Parse(inputPopulation.Text);
                }
                parameters.InfectionRate = double.Parse(inputInfectionRate.Text);
                parameters.RecoveryRate = double.Parse(inputRecoveryRate.Text);
                parameters.BirthRate = double.Parse(inputBirthRate.Text);
                parameters.SusceptibleRate = double.Parse(inputSusceptibleRate.Text);
                parameters.ExposedRate = double.Parse(inputExposedRate.Text);

                state.Infected = double.Parse(inputInfected.Text);
                state.Susceptible = double.Parse(inputSusceptible.Text);
                state.Removed = parameters.Population - state.Infected - state.Susceptible;

                model = CompartmentModelFactory.CreateModel((CompartmentModelType)modelType, isStochastic,
                    state, parameters, double.Parse(inputTime.Text), double.Parse(inputTimeStep.Text));
                progressBar.Value = 0;
                lblStatus.Text = "Busy";
                backgroundWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        ObservableCollection<ChartPoint> chartDataI;

        private void BackgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                solution = model.Run();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            chartDataI = new ObservableCollection<ChartPoint>();
            backgroundWorker.ReportProgress(50);
            for (int i = 0; i < solution.Length / (model.CompartmentCount + 1); i++)
            {
                chartDataI.Add(new ChartPoint(solution[i, 0], solution[i, 2]));
            }
            backgroundWorker.ReportProgress(100);
        }

        private void BackgroundWorker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            lineI.ItemsSource = chartDataI;
            lblStatus.Text = "Ready";
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
    }
}
