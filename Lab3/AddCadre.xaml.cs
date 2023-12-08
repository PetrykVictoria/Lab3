using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using static Lab3.MainPage;
using static System.Net.Mime.MediaTypeNames;

namespace Lab3;

public partial class AddCadre : ContentPage
{
    Cadres cadreadd;
    private List<Cadres> cadres1;
    public AddCadre(List<Cadres> cadres)
	{
        this.cadres1 = cadres;
		InitializeComponent();
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
    private async void SaveButton_Clicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Підтвердіть додавання", "Ви впевнені, що хочете додати цей елемент?", "Так", "Ні");
        if (answer)
        {
            cadreadd = new Cadres(
                string.IsNullOrEmpty(FullNameEntry.Text) ? "-" : FullNameEntry.Text,
                string.IsNullOrEmpty(NameOfFacEntry.Text) ? "-" : NameOfFacEntry.Text=="ФКНК" ? "Факультет комп'ютерних наук" : NameOfFacEntry.Text,
                string.IsNullOrEmpty(DepartmentEntry.Text) ? "-" : DepartmentEntry.Text,
                string.IsNullOrEmpty(ChairEntry.Text) ? "-" : ChairEntry.Text,
                TypeOfEducPicker.SelectedItem == null ? "-" : TypeOfEducPicker.SelectedItem.ToString(),
                string.IsNullOrEmpty(EducInstitutionEntry.Text) ?  "-" : EducInstitutionEntry.Text=="КНУ" ? "Київський національний університет імені Тараса Шевченка" : EducInstitutionEntry.Text,
                string.IsNullOrEmpty(StartDateEntry.Text) ? "-" : StartDateEntry.Text,
                string.IsNullOrEmpty(FinalDateEntry.Text) ? "-" : FinalDateEntry.Text
            );
            cadres1.Add(cadreadd);
            await Navigation.PopAsync();
        }
    }
}