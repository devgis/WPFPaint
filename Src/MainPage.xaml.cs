using App1.DrawingBoard;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

namespace App1
{

    public partial class MainPage : Page
    { 
        Symbol SelectIcon = (Symbol)0xEF20;
        public MainPage()
        {
            this.InitializeComponent();
            DrawingEventControll.setupDrawingEventControll(inkCanvas, canvas);
        }
    }

   


}
