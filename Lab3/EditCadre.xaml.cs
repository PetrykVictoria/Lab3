using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using static Lab3.MainPage;
namespace Lab3;

public partial class EditCadre : ContentPage
{
    private Cadres cadreToEdit;
    private List<Cadres> cadreToEditL = new List<Cadres>();
    public EditCadre(Cadres editcadre)
	{
		this.cadreToEdit = editcadre;
        cadreToEditL.Add(cadreToEdit);
        InitializeComponent();
        SearchResultsCollectionView.ItemsSource = cadreToEditL;
    }
    void LetterEntryTextChanged(object sender, EventArgs e)
    {
        var entry = (Entry)sender;
        entry.Text = new String(entry.Text.Where(c => Char.IsLetter(c) || Char.IsWhiteSpace(c) || c == '`').ToArray());
    }
    void NumberEntryTextChanged(object sender, EventArgs e)
    {
        var entry = (Entry)sender;
        entry.Text = new String(entry.Text.Where(c => Char.IsDigit(c) || c == '.').ToArray());
    }

    private async void EditButton_Clicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Підтвердіть зміни", "Ви впевнені, що хочете зберегти зміни?", "Так", "Ні");
        if (answer)
        {
            cadreToEdit.Fullname = string.IsNullOrEmpty(FullNameEntry.Text) ? cadreToEdit.Fullname : FullNameEntry.Text;
            cadreToEdit.NameOfFac = string.IsNullOrEmpty(NameOfFacEntry.Text) ? cadreToEdit.NameOfFac : NameOfFacEntry.Text == "ФКНК" ? "Факультет комп'ютерних наук" : NameOfFacEntry.Text;
            cadreToEdit.Department = string.IsNullOrEmpty(DepartmentEntry.Text) ? cadreToEdit.Department : DepartmentEntry.Text;
            cadreToEdit.Chair = string.IsNullOrEmpty(ChairEntry.Text) ? cadreToEdit.Chair : ChairEntry.Text;
            cadreToEdit.TypeOfeducation = TypeOfEducPicker.SelectedItem == null ? cadreToEdit.TypeOfeducation : TypeOfEducPicker.SelectedItem.ToString();
            cadreToEdit.EducInstitution = string.IsNullOrEmpty(EducInstitutionEntry.Text) ? cadreToEdit.EducInstitution : EducInstitutionEntry.Text == "КНУ" ? "Київський національний університет імені Тараса Шевченка" : EducInstitutionEntry.Text;
            cadreToEdit.StartDate = string.IsNullOrEmpty(StartDateEntry.Text) ? cadreToEdit.StartDate : StartDateEntry.Text;
            cadreToEdit.FinalDate = string.IsNullOrEmpty(FinalDateEntry.Text) ? cadreToEdit.FinalDate : FinalDateEntry.Text;

            await Navigation.PopAsync();
        }
    }
}