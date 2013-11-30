using MathematicalEpidemiology.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public MainWindow()
        {
            InitializeComponent();
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
                //LineSeries lineI = new LineSeries();
                lineI.Title = "Infected";
                lineI.DependentValuePath = "Time";
                lineI.DependentValuePath = "Value";
                lineI.DataPointStyle = GetNewDataPointStyle(255, 0, 0);

                parameters.InfectionRate = double.Parse(inputInfectionRate.Text);
                parameters.RecoveryRate = double.Parse(inputRecoveryRate.Text);
                parameters.BirthRate = double.Parse(inputBirthRate.Text);

                state.Infected = double.Parse(inputInfected.Text);
                state.Susceptible = double.Parse(inputSusceptible.Text);
                state.Removed = parameters.Population - state.Infected - state.Susceptible;
                
                ObservableCollection<ChartPoint> chartDataI = new ObservableCollection<ChartPoint>();
                model = new DeterministicSIR(state, parameters,
                    double.Parse(inputTime.Text), double.Parse(inputTimeStep.Text));
                double[,] solution = model.Run();
                for (int i = 0; i < solution.Length / 4; i++)
                {
                    chartDataI.Add(new ChartPoint(solution[i, 0], solution[i, 2]));
                }
                lineI.ItemsSource = chartDataI;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
