﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Isomerization.Domain.Data;
using Isomerization.Domain.Models;
using Isomerization.Wpf.OLD.Utils;


namespace Isomerization.Wpf.OLD.VM
{
    public class UnitCreateWindowVM : ViewModelBase
    {
        // public Unit Unit { get; set; } = new Unit();
        private RelayCommand _saveUnit;

        // public RelayCommand SaveUnit
        // {
        //     get
        //     {
        //         return _saveUnit ??= new RelayCommand(o =>
        //         {
        //             DbContextSingleton.GetInstance().Units.Add(Unit);
        //             DbContextSingleton.GetInstance().SaveChanges();
        //             OnClosingRequest();
        //         },_=> !string.IsNullOrEmpty(Unit.UnitName));
        //     }
        // }

        private RelayCommand _exitCommand;

        public RelayCommand ExitCommand
        {
            get
            {
                return _exitCommand ??= new RelayCommand(o =>
                {
                    OnClosingRequest();
                });
            }
        }

    }
}
