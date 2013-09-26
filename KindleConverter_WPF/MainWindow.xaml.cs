﻿using KindleConverter_WPF.ViewModles;
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

namespace KindleConverter_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindowViewModel VM { get; private set; }

        private MainWindowViewModel vm = new MainWindowViewModel();
        public MainWindow()
        {
            InitializeComponent();
            VM = new MainWindowViewModel();
            VM.View = this;
            DataContext = VM;
        }
    }
}
