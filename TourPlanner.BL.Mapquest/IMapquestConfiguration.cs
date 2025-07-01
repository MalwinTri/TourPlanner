using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.BL.Mapquest
{
    public interface IMapquestConfiguration
    {
        string MapquestApiUrl { get; }
        string ImagePath { get; }
        string MapquestApiKey { get; }
    }
}
