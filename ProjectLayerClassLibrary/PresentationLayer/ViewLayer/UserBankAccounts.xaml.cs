using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public partial class UserBankAccounts : UserControl
    {
        public UserBankAccounts()
        {
            InitializeComponent();
            DataContext = ProjectLayerClassLibrary.PresentationLayer.ViewModelLayer.AViewModelLayer.Instance.CreateUserBankAccountDataContext();
        }
    }
}
