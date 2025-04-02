﻿using ProjectLayerClassLibrary.PresentationLayer.ViewModelLayer.DataContexts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ProjectLayerClassLibrary.PresentationLayer.ViewModelLayer.Implementations.DataContexts
{
    internal class LoginDataContext : ADataContext, ILoginDataContext
    {
        private string login = "";
        private string password = "";
        private ICommand loginCommand;

        public string Login { get => login; set => login = value; }
        public string Password { get => password; set
            {
                password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public ICommand LoginCommand { get => loginCommand; }

        public LoginDataContext(AViewModelLayer viewModelLayer) : base(viewModelLayer)
        {
            loginCommand = new RelayCommand(ExecuteLogin);
        }

        private void ExecuteLogin(object? parameter)
        {
            viewModelLayer.ModelLayer.Login(Login, Password, parameter as Type);
        }
    }
}
