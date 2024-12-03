using PasswordManager.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PasswordManager
{

    public class MainViewModel : INotifyPropertyChanged
    {
        public MainWindow MainWindow { get; set; }
        private object _currentView;

        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged(nameof(CurrentView));
            }
        }
        public ProfileInfo CurrentProfile { get; set; } = new ProfileInfo();
        public ICommand NavigateCommand { get; }

        public MainViewModel()
        {
          
            CurrentView = new LoginView();


            NavigateCommand = new RelayCommand(Navigate);
        }

        private void Navigate(object viewName)
        {
            switch (viewName.ToString())
            {
                case "Passwords":
                    CurrentView = new PasswordsView();
                    break;
                case "Options":
                    CurrentView = new OptionsView();
                    break;
                case "Logout":
                    CurrentView = new LoginView();

                    if (MainWindow != null)
                    {
                        MainWindow.btnNavLogout.IsEnabled = false;
                        MainWindow.btnNavOptions.IsEnabled = false;
                        MainWindow.btnNavPasswords.IsEnabled = false;
                    }

                    CurrentProfile = new ProfileInfo();
                    break;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
