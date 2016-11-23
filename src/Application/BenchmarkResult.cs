using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Szab.Scheduling.MSRCPSP;
using Szab.Scheduling.Representation;

namespace Application
{
    public class BenchmarkResult
    {
        public double? Best
        {
            get
            {
                double? result = this.BestSpecimen != null ? new double?(1 / this.BestSpecimen.RateQuality()) : null;
                return result;
            }
        }

        public List<ScheduleSpecimen> Results { get; set; }
        public double Average { get; set; }
        public double StandardDeviation { get; set; }
        public ScheduleSpecimen BestSpecimen { get; set; }
    }
}
