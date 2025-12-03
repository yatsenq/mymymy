using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace StayFit.UI.ViewModels.Base;

public abstract class BaseViewModel : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler? PropertyChanged;

	protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}

	protected virtual bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string? propertyName = null)
	{
		if (EqualityComparer<T>.Default.Equals(backingStore, value))
			return false;

		backingStore = value;
		OnPropertyChanged(propertyName);
		return true;
	}

	private bool _isBusy;
	public bool IsBusy
	{
		get => _isBusy;
		set => SetProperty(ref _isBusy, value);
	}

	private string? _title;
	public string? Title
	{
		get => _title;
		set => SetProperty(ref _title, value);
	}
}

