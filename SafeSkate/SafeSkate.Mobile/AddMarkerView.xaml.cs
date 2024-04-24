namespace SafeSkate.Mobile;

public partial class AddMarkerView : ContentView
{
    private AddMarkerViewModel viewModel;

    public AddMarkerView()
	{
		InitializeComponent();
        this.BindingContextChanged += AddMarkerView_BindingContextChanged;
	}

    private void AddMarkerView_BindingContextChanged(object? sender, EventArgs e)
    {
        if (this.BindingContext is MainPageViewModel)
        {
            this.BindingContextChanged -= AddMarkerView_BindingContextChanged;
            this.viewModel = ((MainPageViewModel)this.BindingContext).AddMarkerViewModel;
            this.viewModel.PropertyChanged += ViewModel_PropertyChanged;
            this.BindingContext = viewModel;
        }
    }

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        this.IsVisible = this.viewModel.Visibility;
        this.ZIndex++;
    }

    private void Upload_Clicked(object sender, EventArgs e)
    {
        this.viewModel.SubmitMarker();
    }

    private void Cancel_Clicked(object sender, EventArgs e)
    {
        this.viewModel.HideUI();
    }
}