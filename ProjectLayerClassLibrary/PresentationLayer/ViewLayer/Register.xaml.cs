﻿using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProjectLayerClassLibrary.PresentationLayer.ViewLayer
{
    partial class Register : UserControl
    {
        public Register()
        {
            InitializeComponent();
            DataContext = ProjectLayerClassLibrary.PresentationLayer.ViewModelLayer.AViewModelLayer.Instance.CreateRegisterDataContext(RegisterFailurePopUp);
        }
    }
}
