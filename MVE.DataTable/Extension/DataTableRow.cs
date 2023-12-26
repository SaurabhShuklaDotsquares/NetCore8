using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVE.DataTable.Extension
{
    public class DataTableRow : List<string>
    {
        public DataTableRow(string dtRowId = null, string dtRowClass = null)
            : this(dtRowId, dtRowClass, new List<string>())
        {
            this.DT_RowId = dtRowId;
            this.DT_RowClass = dtRowClass;
        }

        public DataTableRow(string dtRowId, string dtRowClass, List<string> collection)
            : base(collection)
        {
            this.DT_RowId = dtRowId;
            this.DT_RowClass = dtRowClass;
        }


        public string DT_RowId
        {
            get;
            set;
        }

        public string DT_RowClass
        {
            get;
            set;
        }
    }
}
