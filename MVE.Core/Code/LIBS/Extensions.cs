using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MVE.Core.Code.LIBS
{
    public static class Extensions
    {
        public static void AddOrReplace(this IDictionary<string, object> DICT, string key, object value)
        {
            if (DICT.ContainsKey(key))
                DICT[key] = value;
            else
                DICT.Add(key, value);
        }
        public static Nullable<T> ToNullable<T>(this object input)
           where T : struct
        {
            if (input == null)
                return null;
            if (input is Nullable<T> || input is T)
                return (Nullable<T>)input;
            else if (input is string)
                return (T)Convert.ChangeType(input, typeof(T));

            throw new InvalidCastException();
        }
        public static DateTime? ToDateTime(this string source, string format)
        {
            if (!string.IsNullOrWhiteSpace(source))
            {
                DateTime convertedDateTime;
                if (DateTime.TryParseExact(source, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out convertedDateTime))
                {
                    return convertedDateTime;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        public static bool HasValue(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }
        public static bool IsMenuActive(this IHtmlHelper htmlHelper, string menuItemUrl)
        {
            var viewContext = htmlHelper.ViewContext;
            var currentPageUrl = viewContext.ViewData["ActiveMenu"] as string ?? viewContext.HttpContext.Request.Path;
            var currentSplitUrl = currentPageUrl.Split("/");
            var controllerCurrentUrl = currentSplitUrl[1];
            var menuItemSplitUrl = menuItemUrl.Split("/");
            var controllerMenuItemUrl = menuItemSplitUrl[0];
            var currentUrl = string.Empty;
            if (controllerMenuItemUrl == controllerCurrentUrl)
            {
                currentUrl = controllerMenuItemUrl;
                return currentPageUrl.StartsWith(controllerMenuItemUrl, StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                return currentPageUrl.StartsWith(menuItemUrl, StringComparison.OrdinalIgnoreCase);
            }
            // return currentPageUrl.StartsWith(menuItemUrl, StringComparison.OrdinalIgnoreCase);
            //return currentPageUrl.StartsWith(menuItemUrl, StringComparison.OrdinalIgnoreCase);
        }

        public static string IsActive(this IHtmlHelper html, string menuItemUrl)
        {
            var routeData = html.ViewContext.RouteData;

            var routeAction = (string)routeData.Values["action"];
            var routeControl = (string)routeData.Values["controller"];

            var menuItemSplitUrl = menuItemUrl.Split("/");
            var controllerMenuItemUrl = menuItemSplitUrl[0];
            // both must match
            var returnActive = controllerMenuItemUrl == routeControl.ToLower();
            return returnActive ? "active" : "";
        }

        public static string GetEnumDescription<TEnum>(this TEnum enumerationValue)
         where TEnum : struct
        {
            var type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException($"{nameof(enumerationValue)} must be of Enum type", nameof(enumerationValue));
            }
            var memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo.Length > 0)
            {
                var attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            return enumerationValue.ToString();
        }

        public static string GetDescription(this String value, Type t)
        {
            if (t.IsEnum)
            {
                var fields = t.GetFields(BindingFlags.Static | BindingFlags.Public);
                var values = from f
                    in fields
                             let attribute = Attribute.GetCustomAttribute(f, typeof(DisplayAttribute)) as DisplayAttribute
                             where attribute != null && attribute.ShortName == value
                             select f.GetValue(null);
                return values.First().ToString();
            }
            return "";
        }
        public static string GetDescription<TEnum>(this TEnum value)
        {
            var fi = value.GetType().GetField(value.ToString());

            if (fi != null)
            {
                var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attributes.Length > 0)
                {
                    return attributes[0].Description;
                }
            }

            return value.ToString();
        }

        

        public static string GetEnumDisplayName<TEnum>(this TEnum value)
        {
            var fi = value.GetType().GetField(value.ToString());

            if (fi != null)
            {
                var attributes = (DisplayAttribute[])fi.GetCustomAttributes(typeof(DisplayAttribute), false);

                if (attributes.Length > 0)
                {
                    return attributes[0].Name;
                }
            }

            return value.ToString();
        }

        public static IEnumerable<SelectListItem> GetEnumSelectList<TEnum>(bool getDescription = false, List<TEnum> skipEnums = null)
            where TEnum : struct
        {
            if (!typeof(TEnum).IsEnum) throw new InvalidOperationException();

            var enumList = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();

            if (skipEnums != null && skipEnums.Any())
            {
                enumList = enumList.Except(skipEnums);
            }

            return enumList.Select(c => new SelectListItem()
            {
                Text = getDescription ? c.GetEnumDescription() : c.ToString(),
                Value = Convert.ToInt32(c).ToString()
            });

        }

        public static string GetName(this Enum value)
        {
            return value.GetType().GetFields(BindingFlags.Static | BindingFlags.Public).Select(fi => fi.Name).FirstOrDefault();
        }
        public static string ToUnique(this string fileName)
        {
            return $"{DateTime.Now.Ticks}{Path.GetExtension(fileName.ToLower())}";
        }

        #region enum Method

        // This extension method is broken out so you can use a similar pattern with 
        // other MetaData elements in the future. This is your base method for each.
        //In short this is generic method to get any type of attribute.
        public static T GetAttribute<T>(this Enum value) where T : Attribute
        {
            var type = value.GetType();
            var memberInfo = type.GetMember(value.ToString());
            var attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);
            return (T)attributes.FirstOrDefault();//attributes.Length > 0 ? (T)attributes[0] : null;
        }

        // This method creates a specific call to the above method, requesting the
        // Display MetaData attribute.
        //e.g. [Display(Name = "Sunday")]
        public static string ToDisplayName(this Enum value)
        {
            var attribute = value.GetAttribute<DisplayAttribute>();

            return attribute == null ? value.ToString() : attribute.Name;
        }

        public static string GetDisplayAttrValue(this Enum value, EnumDisplayAttribute enumDisplayAttribute)
        {
            string returnValue = string.Empty;
            var attribute = value.GetAttribute<DisplayAttribute>();
            if (attribute != null)
            {
                switch (enumDisplayAttribute)
                {
                    case EnumDisplayAttribute.GroupName:
                        returnValue = attribute.GroupName;
                        break;
                    case EnumDisplayAttribute.Prompt:
                        returnValue = attribute.Prompt;
                        break;
                    case EnumDisplayAttribute.Description:
                        returnValue = attribute.Description;
                        break;
                    case EnumDisplayAttribute.Title:
                        returnValue = attribute.ShortName;
                        break;
                    default:
                        returnValue = attribute.Name;
                        break;
                }
            }
            else
            {
                returnValue = value.ToString();
            }
            return returnValue;
        }

        // This method creates a specific call to the above method, requesting the
        // Description MetaData attribute.
        //e.g. [Description("Day of week. Sunday")]
        public static string ToDescription(this Enum value)
        {
            var attribute = value.GetAttribute<DescriptionAttribute>();
            return attribute == null ? value.ToString() : attribute.Description;
        }

        #endregion


        public static string Serialize<T>(this T dataToSerialize)
        {
            try
            {
                MemoryStream memoryStream = new MemoryStream();
                XmlSerializer xs = new XmlSerializer(typeof(T));
                XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
                xs.Serialize(xmlTextWriter, dataToSerialize);
                string result = Encoding.UTF8.GetString(memoryStream.ToArray());
                return result;

                //OLD method
                //var stringwriter = new System.IO.StringWriter();
                //var serializer = new XmlSerializer(typeof(T));
                //serializer.Serialize(stringwriter, dataToSerialize);
                //return stringwriter.ToString();
            }
            catch
            {
                throw;
            }
        }

        public static T Deserialize<T>(this string xmlText)
        {
            try
            {
                var stringReader = new System.IO.StringReader(xmlText);
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(stringReader);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

        public static List<T> ConvertDataTable<T>(this System.Data.DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }

        public static List<T> ToListof<T>(this DataTable dt)
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            var columnNames = dt.Columns.Cast<DataColumn>()
                .Select(c => c.ColumnName)
                .ToList();
            var objectProperties = typeof(T).GetProperties(flags);
            var targetList = dt.AsEnumerable().Select(dataRow =>
            {
                var instanceOfT = Activator.CreateInstance<T>();

                foreach (var properties in objectProperties.Where(properties => columnNames.Contains(properties.Name) && dataRow[properties.Name] != DBNull.Value))
                {
                    properties.SetValue(instanceOfT, dataRow[properties.Name], null);
                }
                return instanceOfT;
            }).ToList();

            return targetList;
        }

        public static List<SelectListItem> DatatableToSelectListItem(this DataTable dataTable)
        {
            List<SelectListItem> listObj = new List<SelectListItem>();
            foreach (DataRow mydataRow in dataTable.Rows)
            {
                listObj.Add(new SelectListItem()
                {
                    Value = mydataRow["Value"].ToString().Trim(),
                    Text = mydataRow["Text"].ToString().Trim()
                });
            }
            return listObj;
        }

        public static System.Data.DataTable ToDataTable<T>(this List<T> items)
        {
            System.Data.DataTable dataTable = new System.Data.DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Defining type of data column gives proper data table 
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
    }
}
