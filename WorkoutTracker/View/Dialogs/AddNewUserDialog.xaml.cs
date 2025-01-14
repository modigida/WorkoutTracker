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

namespace WorkoutTracker.View.Dialogs
{
    /// <summary>
    /// Interaction logic for AddNewUserDialog.xaml
    /// </summary>
    public partial class AddNewUserDialog : Window
    {
        public AddNewUserDialog(Window owner)
        {
            InitializeComponent();
            Owner = owner;
        }
    }
}
