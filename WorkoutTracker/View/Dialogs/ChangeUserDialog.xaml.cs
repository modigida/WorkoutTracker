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
using System.Windows.Shapes;
using WorkoutTracker.ViewModel;

namespace WorkoutTracker.View.Dialogs
{
    /// <summary>
    /// Interaction logic for ChangeUserDialog.xaml
    /// </summary>
    public partial class ChangeUserDialog : Window
    {
        public ChangeUserDialog(Window owner, MainWindowViewModel mainWindowViewModel)
        {
            InitializeComponent();
            Owner = owner;
            DataContext = mainWindowViewModel;
        }
    }
}
