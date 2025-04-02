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
    public partial class CreateTransfer : UserControl
    {
        public CreateTransfer()
        {
            InitializeComponent();
            DataContext = ProjectLayerClassLibrary.PresentationLayer.ViewModelLayer.AViewModelLayer.Instance.CreateCreateTransferDataContext();
        }
    }
}
