using System.Text;
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
    partial class Login : UserControl
    {
        public Login()
        {
            InitializeComponent();
            DataContext = ProjectLayerClassLibrary.PresentationLayer.ViewModelLayer.AViewModelLayer.Instance.CreateLoginDataContext(PopUpLoginFailure);
        }
    }
}
