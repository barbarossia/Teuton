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

namespace SqlText2String
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string sql = txt.Text;
            string[] array = sql.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            var addComma = array.Select(s => string.Format(@"""{0}"",", s));
            string array2Sql = string.Join(Environment.NewLine, addComma);

            txt.Text = array2Sql;
        }
    }
}
