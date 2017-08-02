using System;
using System.Windows;
using Microsoft.Win32;
using System.Collections.ObjectModel;

namespace Artiva_Profile_Selector
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var ArtivaProfiles = new ObservableCollection<ArtivaProfile>();

            var path = @"Software\VB and VBA Program Settings\Ontario Systems API Manager\Profiles";

            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(path))
                {
                    if (key != null)
                    {
                        foreach (string subkey in key.GetValueNames())
                        {
                            var val = (string)(key.GetValue(subkey));
                            var item = new ArtivaProfile {
                                ProfileName = subkey,
                                Description = val.Split('\x7')[0],
                                HostName = val.Split('\x7')[1],
                                HostPort = val.Split('\x7')[2],
                                User = val.Split('\x7')[3],
                                Password = val.Split('\x7')[4],
                                Namespace = val.Split('\x7')[5],
                                DeskNumber = val.Split('\x7')[6]
                            };
                            ArtivaProfiles.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            Profiles.ItemsSource = ArtivaProfiles;
        }

        private void openWorkstation_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Open Workstation Clicked");
        }

        private void openStudio_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Open Studio Clicked");
        }

        private void addProfile_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Add Profile Clicked");
        }

        private void saveProfile_Click(object sender, RoutedEventArgs e)
        {

        }

        private void saveSetting_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    public class ArtivaProfile
    {
        public string ProfileName { get; set; }
        public string Description { get; set; }
        public string HostName { get; set; }
        public string HostPort { get; set; }
        public string Namespace { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string DeskNumber { get; set; }
    }
}
