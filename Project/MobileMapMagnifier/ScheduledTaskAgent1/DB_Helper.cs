using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace ScheduledTaskAgent1
{
    public class DB_Helper
    {
        // Data context for the local database
        private static Row.GPSDataContext DB;

        // Connect to the database and instantiate data context.
        public static void connect()
        {
            DB = new Row.GPSDataContext(Row.GPSDataContext.DBConnectionString);
        }

        public static ObservableCollection<Row> getAllRows()
        {
            var InDB = from Row todo in DB.Rows select todo;
            return (new ObservableCollection<Row>(InDB));
        }
        public static ObservableCollection<Row> getRowsbyTitle(String title)
        {
            var edit_query = from Row todo in DB.Rows where todo.Title == title select todo;
            return (new ObservableCollection<Row>(edit_query));
        }
        public static void deleteRow(Row temp)
        {
            DB.Rows.DeleteOnSubmit(temp);
            DB.SubmitChanges();
        }
        public static void insertRow(Row temp)
        {
            DB.Rows.InsertOnSubmit(temp);
            DB.SubmitChanges();
        }

    }
}
