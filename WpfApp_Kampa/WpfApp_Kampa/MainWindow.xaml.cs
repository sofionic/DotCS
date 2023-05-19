using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;

namespace DataGridFilter
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ICollectionView dataGridItems;
        private string filterText;

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();

            //we set our datagrid's bound collection to our entire collection of data
            DataGridItems = CollectionViewSource.GetDefaultView(LoadedDatabaseData);

            //we define our collections Filter property to a method that defines our filter logic
            DataGridItems.Filter = new Predicate<object>(Filter);
        }

        //This is what we bind to our datagrid, an ICollectionView allows custom sorting/filtering.
        public ICollectionView DataGridItems
        {
            get { return dataGridItems; }
            set 
            {
                dataGridItems = value; 
                OnPropertyChanged(); 
            }
        }

        //this will be bound to our textbox, and used to set our filter criteria
        public string FilterText
        {
            get { return filterText; }
            set
            {
                filterText = value;
                OnPropertyChanged();               
                Filter(); //any time this fires, we need to refresh our collection, triggering a filter
            }
        }

        //Convert Filter Text into Rule based filter order.
        private  List<string> TextConverter(string text)
        {            
            int pos = FilterText.IndexOf(":", 0);
            List<string> result = new List<string>();
            if (pos != -1)
            {
                string filterColumn = FilterText.Substring(0, pos);
                string filterValue = FilterText.Substring(pos + 1);
                result.Add(filterColumn);
                result.Add(filterValue);                
            }
            return result;
        }
        

        //this refreshes the collection itself
        private void Filter()
        {
            if (dataGridItems != null)
                dataGridItems.Refresh();
        }

        //this is performed automatically when the collection is refreshed, and contains all logic for filtering
        //this happens on a PER-OBJECT (row) basis
        public bool Filter(object obj)
        {
            var data = obj as ModelClass; //cast the actual type we use
            if (data != null)
            {                                
                if (!string.IsNullOrEmpty(filterText))
                {
                    List<string> filterElements = TextConverter(FilterText);
                    string filterColumn = filterElements[0];
                    string filterValue = filterElements[1]; 
                    //our actual filtering rules, in this case, if either field contains the text
                    //return data.Prop1.Contains(filterText) || data.Prop2.Contains(filterText);
                    switch (filterColumn) 
                    {
                        case "Prop1": return data.Prop1.Contains(filterValue);
                        case "Prop2": return data.Prop2.Contains(filterValue); 
                    }
                    return data.Prop1.Contains(filterText);
                }
                return true; 
            }
            return false;
        }

        //fake database data, this would be your real loaded data
        public IEnumerable<ModelClass> LoadedDatabaseData
        {
            get
            {
                yield return new ModelClass { Prop1 = "Full Mode A", Prop2 = "defgh" };
                yield return new ModelClass { Prop1 = "5", Prop2 = "hijkl" };
                yield return new ModelClass { Prop1 = "5", Prop2 = "mnopq" };
                yield return new ModelClass { Prop1 = "mnopq", Prop2 = "2" };
                yield return new ModelClass { Prop1 = "Diag Mode B Init @ Umin", Prop2 = "5" };
                yield return new ModelClass { Prop1 = "Part Mode A", Prop2 = "2" };
                yield return new ModelClass { Prop1 = "Full Mode A", Prop2 = "5" };
                yield return new ModelClass { Prop1 = "5", Prop2 = "Part Mode B" };
                yield return new ModelClass { Prop1 = "2", Prop2 = "Full Mode A - Init@Unom" };
                yield return new ModelClass { Prop1 = "2", Prop2 = "Part Mode B- Diag Int @ Umin" };
                yield return new ModelClass { Prop1 = "Part Mode prop2", Prop2 = "2" };
                yield return new ModelClass { Prop1 = "Full Mode A - Init@Unom prop2", Prop2 = "5" };
                yield return new ModelClass { Prop1 = "Part Mode B- Diag Int @ Umi prop2n", Prop2 = "5" };
            }
        }

        //mvvm stuff
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string property = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

 
    }

    //your actual model class
    public class ModelClass
    {
        public string Prop1 { get; set; }
        public string Prop2 { get; set; }
    }
}
