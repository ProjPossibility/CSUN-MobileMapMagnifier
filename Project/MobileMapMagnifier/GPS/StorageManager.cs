using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls.Maps;

namespace GPS
{
    public sealed class StorageManager
    {
        public static StorageManager Instance = new StorageManager();

        public Pushpin temp = new Pushpin();

        private StorageManager()
        {

        }

        static StorageManager()
        {

        }
    }
}
