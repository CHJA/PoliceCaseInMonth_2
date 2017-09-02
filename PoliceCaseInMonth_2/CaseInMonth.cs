using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HRVPoliceCaseInMonthControl_ZZGA
{
    public class CaseInMonth
    {
        private int caseOrder = 1;
        private SortedList<int,int> caseValuePair = new SortedList<int, int>();
        private int average = 0;

        public CaseInMonth()
        {
        }

        public CaseInMonth(int caseOrder, SortedList<int, int> sorList)
        {
            CaseOrder = caseOrder;
            CaseValuePair = sorList ?? throw new ArgumentNullException(nameof(sorList));

            Initial();
        }

        public int CaseOrder { get => caseOrder; set => caseOrder = value; }
        public SortedList<int, int> CaseValuePair { get => caseValuePair; set => caseValuePair = value; }
        public int Average { get => average; set => average = value; }

        private void Initial()
        {
            int sum = 0;
            int n = 0;
            foreach (var s in CaseValuePair)
            {
                if (s.Value >= 0)
                {
                    sum += s.Value;
                    n++;
                }
            }

            Average = n > 0 ? Convert.ToInt32(sum / n) : 0;
        }
    }
}
