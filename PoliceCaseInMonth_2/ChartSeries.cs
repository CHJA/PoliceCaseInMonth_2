using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace HRVPoliceCaseInMonthControl_ZZGA
{
    public class ChartSeries
    {
        private DataTable dataTable;

        public ChartSeries()
        {
        }

        public ChartSeries(DataTable dataTable)
        {
            this.dataTable = dataTable;
        }

        public DataTable DataTable { get => dataTable; set => dataTable = value; }

        public void DrawingSeries()
        {


        }
    }
}
