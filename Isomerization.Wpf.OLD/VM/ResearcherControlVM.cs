using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;

using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;

using Isomerization.Domain;
using Isomerization.Domain.Data;
using Isomerization.Domain.Models;
using Isomerization.Domain.Task1;
using Isomerization.Domain.Task2;
using Isomerization.Wpf.OLD.Utils;


namespace Isomerization.Wpf.OLD.VM
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
