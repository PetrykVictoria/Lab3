using System.IO;
using System.Text.Json;

namespace Lab3;

public partial class MainPage : ContentPage
{
    public string jsonFilePath = "";
    public Dictionary<string, string> filterCadres = new Dictionary<string, string>();
    public List<Cadres> cadres = new List<Cadres>();
    public List<Cadres> filteredCadres = new List<Cadres>();
    public int SelectedIndex { get; set; }

    public class Cadres

    {
        public string Fullname { get; set; }
        public string NameOfFac { get; set; }
        public string Department { get; set; }
        public string Chair { get; set; }
        public string TypeOfeducation { get; set; }
        public string EducInstitution { get; set; }
        public string StartDate { get; set; }
        public string FinalDate { get; set; }
        public Cadres(string fullname, string nameOfFac, string department, string chair, string typeOfeducation, string educInstitution, string startDate, string finalDate)
        {
            Fullname = fullname;
            NameOfFac = nameOfFac;
            Department = department;
            Chair = chair;
            TypeOfeducation = typeOfeducation;
            EducInstitution = educInstitution;
            StartDate = startDate;
            FinalDate = finalDate;
        }
    }
    public MainPage()
    {
        InitializeComponent();
    }
    private void JSONDeserialize()
    {
        FileStream fs = new FileStream(jsonFilePath, FileMode.Open);
        cadres = JsonSerializer.Deserialize<List<Cadres>>(fs);
        fs.Close();
    }
    private void JSONSerialize()
    {
        FileStream fs = new FileStream(jsonFilePath, FileMode.Create);
        JsonSerializer.Serialize(fs, cadres);
        fs.Close();
    }

    private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e, View view)
    {
        if (e.Value)
        {
            view.IsEnabled = e.Value;
        }
        else
        {
            switch (view)
            {
                case Entry entry:
                    entry.Text = string.Empty;
                    break;
                case Picker picker:
                    picker.SelectedItem = null;
                    break;
            }
            view.IsEnabled = false;
        }
    }

    private void FullNameBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        CheckBox_CheckedChanged(sender, e, FullNameEntry);
    }
    private void NameOfFacBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        CheckBox_CheckedChanged(sender, e, NameOfFacPicker);
    }
    private void DepartmentBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        CheckBox_CheckedChanged(sender, e, DepartmentPicker);
    }
    private void ChairBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        CheckBox_CheckedChanged(sender, e, ChairPicker);
    }
    void LetterEntryTextChanged(object sender, EventArgs e)
    {
        var entry = (Entry)sender;
        entry.Text = new String(entry.Text.Where(c => Char.IsLetter(c) || Char.IsWhiteSpace(c) || c == '`').ToArray());
    }

    private async void ChooseFileButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            var result = await FilePicker.PickAsync();

            if (result != null)
            {
                jsonFilePath = result.FullPath;

                if (System.IO.Path.GetExtension(jsonFilePath = result.FullPath)?.ToLower() == ".json")
                {

                    await DisplayAlert("Успіх", "Файл обрано.", "OK");
                    JSONDeserialize();
                    LoadAllPickers();
                    SearchButton_Enable();
                    AddButton_Enable();
                    EditButton_Enable();
                    DeleteButton_Enable();
                    SaveButton_Enable();
                    SearchResultsCollectionView.ItemsSource = cadres;
                }
                else
                {
                    await DisplayAlert("Помилка", "Оберіть файл типу JSON.", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error picking file: {ex.Message}", "OK");
        }
    }

    private void LoadAllPickers()
    {
        LoadPicker(NameOfFacPicker, "NameOfFac");
        LoadPicker(DepartmentPicker, "Department");
        LoadPicker(ChairPicker, "Chair");
    }
    private void LoadPicker(Picker picker, string Path)
    {
        try
        {
            List<string> pickerItems = new List<string>();
            foreach (var cadre in cadres)
            {
                switch (Path)
                {
                    case "NameOfFac":
                        pickerItems.Add(cadre.NameOfFac);
                        break;
                    case "Department":
                        pickerItems.Add(cadre.Department);
                        break;
                    case "Chair":
                        pickerItems.Add(cadre.Chair);
                        break;
                }

            }
            pickerItems.Sort();
            picker.ItemsSource = pickerItems.Distinct().ToList();
        }
        catch (Exception ex)
        {
            DisplayAlert("Помилка", $"Сталася помилка: {ex.Message}", "OK");
        }
    }
    private void InfoButton_Clicked(object sender, EventArgs e)
    {
        DisplayAlert("Info about program", "This program was created by @VictoriaPetryk, group K-27. There you can add, edit, delete and save data JSON format", "OK");
    }

    private void SearchFilterCadres(string key, string value, bool isEnabled)
    {
        if (!string.IsNullOrEmpty(value) && isEnabled)
        {
            filterCadres[key] = value;
        }
    }
    private async void CloseButton_Clicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Підтвердження виходу", "Ви справді хочете вийти?", "Так", "Ні");
        if (answer)
        {
            MainThread.BeginInvokeOnMainThread(() => System.Environment.Exit(0));
        }
    }

    private void Clear()
    {
        SearchResultsCollectionView.ItemsSource = null;
        FullNameBox.IsChecked = false;
        NameOfFacBox.IsChecked = false;
        DepartmentBox.IsChecked = false;
        ChairBox.IsChecked = false;
    }
    private void ClearButton_Clicked(object sender, EventArgs e)
    {
        Clear();
    }

    private void SearchButton_Clicked(object sender, EventArgs e)
    {
        SearchResultsCollectionView.ItemsSource = null;
        SearchFilterCadres("Fullname", FullNameEntry.Text, FullNameEntry.IsEnabled);
        SearchFilterCadres("NameOfFac", (string)NameOfFacPicker.SelectedItem, NameOfFacPicker.IsEnabled);
        SearchFilterCadres("Department", (string)DepartmentPicker.SelectedItem, DepartmentPicker.IsEnabled);
        SearchFilterCadres("Chair", (string)ChairPicker.SelectedItem, ChairPicker.IsEnabled);
        Search();
        SearchResultsCollectionView.ItemsSource = filteredCadres;
        filterCadres.Clear();
    }
    private void Search()
    {
        filteredCadres = cadres
            .Where(cadre => filterCadres.All(filter =>
                filter.Key == "Fullname" ?
                cadre.GetType().GetProperty(filter.Key).GetValue(cadre, null).ToString().Contains(filter.Value) :
                cadre.GetType().GetProperty(filter.Key).GetValue(cadre, null).ToString() == filter.Value))
            .ToList();
    }

    private void AddCadreButton_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new AddCadre(cadres));
    }

    private async void EditCadreButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            string Fullname;
            if (filteredCadres.Any())
            {
                Fullname = filteredCadres[SelectedIndex].Fullname;
            }
            else
            {
                Fullname = cadres[SelectedIndex].Fullname;
            }
            bool answer = await DisplayAlert("Редагування", $"Ви впевнені, що хочете редагувати інформацію про {Fullname}?", "Так", "Ні");
            if (answer)
            {

                if (filteredCadres.Any())
                {
                    Cadres editcadre = filteredCadres[SelectedIndex];
                    await Navigation.PushAsync(new EditCadre(editcadre));

                }
                else
                {
                    Cadres editcadre = cadres[SelectedIndex];
                    await Navigation.PushAsync(new EditCadre(editcadre));
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Помилка", $"Сталася помилка: {ex.Message}", "OK");
        }
    }
    void OnFrameTapped(object sender, EventArgs e)
    {
        {
            var frame = (Frame)sender;
            System.Collections.IList collection = (System.Collections.IList)SearchResultsCollectionView.ItemsSource;
            var item = frame.BindingContext;
            SelectedIndex = collection.IndexOf(item);
        }
    }

    private async void DeleteCadreButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            string Fullname;
            if (filteredCadres.Any())
            {
                Fullname = filteredCadres[SelectedIndex].Fullname;
            }
            else
            {
                Fullname = cadres[SelectedIndex].Fullname;
            }
            bool answer = await DisplayAlert("Видалення", $"Ви впевнені, що хочете видалити інформацію про {Fullname}?", "Так", "Ні");
            if (answer)
            {

                if (filteredCadres.Any())
                {
                    var itemToRemove = filteredCadres[SelectedIndex];
                    filteredCadres.Remove(itemToRemove);
                    cadres.Remove(itemToRemove);
                    SearchResultsCollectionView.ItemsSource = null;
                    LoadAllPickers();
                    SearchResultsCollectionView.ItemsSource = filteredCadres;
                }
                else
                {
                    cadres.RemoveAt(SelectedIndex);
                    SearchResultsCollectionView.ItemsSource = null;
                    LoadAllPickers();
                    SearchResultsCollectionView.ItemsSource = cadres;
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Помилка", $"Сталася помилка: {ex.Message}", "OK");
        }
    }
    protected override void OnAppearing()
    {
        if (jsonFilePath != string.Empty)
        {
            Clear();
            Search();
            Clear();
            LoadAllPickers();
            SearchResultsCollectionView.ItemsSource = cadres;
        }
        base.OnAppearing();
    }

    private async void SaveButton_Clicked(object sender, EventArgs e)
    {
       try
        {
            JSONSerialize();
            await DisplayAlert("Успіх", "Файл успішно збережено", "ОК");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Помилка", $"Сталася помилка: {ex.Message}", "OK");
        }
    }
    private void SearchButton_Enable()
    {
        if (jsonFilePath != string.Empty)
        {
            SearchButton.IsEnabled = true;
        }
        else
        {
            SearchButton.IsEnabled = false;
        }
    }
    private void AddButton_Enable()
    {
        if (jsonFilePath != string.Empty)
        {
            AddButton.IsEnabled = true;
        }
        else
        {
            AddButton.IsEnabled = false;
        }
    }
    private void EditButton_Enable()
    {
        if (jsonFilePath != string.Empty)
        {
            EditButton.IsEnabled = true;
        }
        else
        {
            EditButton.IsEnabled = false;
        }
    }
    private void DeleteButton_Enable()
    {
        if (jsonFilePath != string.Empty)
        {
            DeleteButton.IsEnabled = true;
        }
        else
        {
            DeleteButton.IsEnabled = false;
        }
    }
    private void SaveButton_Enable()
    {
        if (jsonFilePath != string.Empty)
        {
            SaveButton.IsEnabled = true;
        }
        else
        {
            SaveButton.IsEnabled = false;
        }
    }
}