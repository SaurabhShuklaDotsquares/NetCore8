using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVE.DataTable.DataTables;

namespace MVE.DataTable.Extension
{
    /// <summary>
    /// This class represents an MVC Action result for
    /// a jquery.datatables response.
    /// </summary>
    public class DataTableResultExt : ActionResult, IDataTableResult<List<DataTableRow>>
    {

        public Encoding ContentEncoding { get; set; }

        public string ContentType { get; set; }

        // public JsonRequestBehavior JsonRequestBehavior { get; set; }

        public int iTotalDisplayRecords { get; set; }

        public int iTotalRecords { get; set; }

        public string sColumns { get; set; }

        public string sEcho { get; protected set; }

        public List<DataTableRow> aaData { get; set; }


        public DataTableResultExt(MVE.DataTable.DataTables.DataTable dataTable, int iTotalRecords = 0, int iTotalDisplayRecords = 0, List<DataTableRow> aaData = null)
            : this(dataTable.sEcho, iTotalRecords, iTotalDisplayRecords, aaData)
        {
        }

        public DataTableResultExt(string sEcho = "", int iTotalRecords = 0, int iTotalDisplayRecords = 0, List<DataTableRow> aaData = null)
        {
            //  this.JsonRequestBehavior = JsonRequestBehavior.DenyGet;
            this.sEcho = sEcho;
            this.iTotalRecords = iTotalRecords;
            this.iTotalDisplayRecords = iTotalDisplayRecords;
            this.aaData = aaData;
        }

        public override void ExecuteResult(ActionContext context)//(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            //if ((this.JsonRequestBehavior == JsonRequestBehavior.DenyGet) &&
            //    string.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            //{
            //    throw new InvalidOperationException("Get not allowed");
            //}
            // HttpResponseBase response = context.HttpContext.Response;

            HttpResponse response = context.HttpContext.Response;

            if (!string.IsNullOrEmpty(this.ContentType))
            {
                response.ContentType = this.ContentType;
            }
            else
            {
                response.ContentType = "application/json";
            }
            //if (this.ContentEncoding != null)
            //{
            //    response.ContentEncoding = this.ContentEncoding;
            //}

            using (JsonWriter writer = new JsonTextWriter(new StreamWriter(response.Body)))
            {
                writer.WriteStartObject();

                writer.WritePropertyName("sEcho");
                writer.WriteValue(this.sEcho);

                writer.WritePropertyName("iTotalRecords");
                writer.WriteValue(this.iTotalRecords);

                writer.WritePropertyName("iTotalDisplayRecords");
                writer.WriteValue(this.iTotalDisplayRecords);

                writer.WritePropertyName("aaData");
                writer.WriteStartArray();
                for (int i = 0; i < aaData.Count; i++)
                {
                    writer.WriteStartObject();
                    DataTableRow row = aaData[i];
                    if (row.DT_RowId != null)
                    {
                        writer.WritePropertyName("DT_RowId");
                        writer.WriteValue(row.DT_RowId);
                    }

                    if (row.DT_RowClass != null)
                    {
                        writer.WritePropertyName("DT_RowClass");
                        writer.WriteValue(row.DT_RowClass);
                    }

                    for (int j = 0; j < row.Count; j++)
                    {
                        writer.WritePropertyName(j.ToString());
                        writer.WriteValue(row[j]);
                    }
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();
                writer.WriteEndObject();
            }
        }
    }
}
