using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace XtmConnect.XtmConnect.Helpers
{
    internal static class DataSetHelper
    {
        public static DataSet PrepareDataSet<T>(IList<T> list)
        {
            var elementType = typeof(T);
            var dataTable = new DataTable();

            elementType.GetProperties().ToList().ForEach(property =>
            {
                dataTable.Columns.Add(property.Name, property.PropertyType);
            });

            foreach (T element in list)
            {
                var row = dataTable.NewRow();

                elementType.GetProperties().ToList().ForEach(property =>
                {
                    row[property.Name] = property.GetValue(element, null);
                });

                dataTable.Rows.Add(row);
            }

            var dataset = new DataSet();
            dataset.Tables.Add(dataTable);
            return dataset;
        }
    }
}