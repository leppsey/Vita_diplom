using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;

using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;

using PlenkaAPI;
using PlenkaAPI.Data;
using PlenkaAPI.Models;
using PlenkaAPI.Task1;
using PlenkaAPI.Task2;
using PlenkaWpf.Utils;


namespace PlenkaWpf.VM
{
    /// <summary>
    ///     VM для окна исследователя
    /// </summary>
    internal class ResearcherControlVM : ViewModelBase

    {
        public List<int> ErrorList { get; set; }



    #region Constructors

        public ResearcherControlVM()
        {
           }

    #endregion


    }
}
