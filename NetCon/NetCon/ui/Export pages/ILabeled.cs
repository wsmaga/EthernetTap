using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCon.ui.Export_pages
{ 

    /// <summary>
    /// Interface for labeled WPF pages to be listed in combo boxes.
    /// </summary>
    interface ILabeled
    {
        String Label { get; }
    }
}
