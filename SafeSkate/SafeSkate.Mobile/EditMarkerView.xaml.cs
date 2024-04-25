namespace SafeSkate.Mobile;

public partial class EditMarkerView : ContentView
{
    private EditMarkerViewModel viewModel;

    public EditMarkerView()
    {
        InitializeComponent();
        this.BindingContextChanged += EditMarkerView_BindingContextChanged;
    }

    private void EditMarkerView_BindingContextChanged(object? sender, EventArgs e)
    {
        if (this.BindingContext is MainPageViewModel)
        {
            this.BindingContextChanged -= EditMarkerView_BindingContextChanged;
            this.viewModel = ((MainPageViewModel)this.BindingContext).EditMarkerViewModel;
            this.viewModel.PropertyChanged += ViewModel_PropertyChanged;
            this.BindingContext = viewModel;
        }
    }

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        this.IsVisible = this.viewModel.Visibility;
        this.ZIndex++;
    }

    private void Save_Clicked(object sender, EventArgs e)
    {
        this.viewModel.SaveMarker();
    }

    private void Delete_Clicked(object sender, EventArgs e)
    {
        this.viewModel.DeleteMarker();
    }

    private void Cancel_Clicked(object sender, EventArgs e)
    {
        this.viewModel.CloseUI();
    }

}