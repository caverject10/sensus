﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Sensus.UI
{
	public class PauseProtocolsPage : ContentPage
	{
		private const int DEFAULT_SNOOZE = 60;
		private TaskCompletionSource<bool> _responseTaskCompletionSource;
		public Task<bool> Result => _responseTaskCompletionSource.Task;

		private async Task PauseProtocolsAsync(IEnumerable<Protocol> protocols, DateTime? resumeTimestamp)
		{
			foreach (Protocol protocol in protocols)
			{
				await protocol.PauseAsync();

				if (resumeTimestamp != null)
				{
					await protocol.ScheduleResumeAsync(resumeTimestamp.Value);
				}
			}

			_responseTaskCompletionSource.TrySetResult(true);
		}

		public PauseProtocolsPage(IEnumerable<Protocol> protocols)
		{
			_responseTaskCompletionSource = new TaskCompletionSource<bool>();

			Title = "Pause Protocols";

			protocols = protocols.Where(x => x.AllowPause);
			int maxSnoozeTimeAmount = protocols.Min(x => x.MaxSnoozeTime);
			StringBuilder protocolNames = new StringBuilder();

			if(maxSnoozeTimeAmount <= 0)
			{
				maxSnoozeTimeAmount = int.MaxValue;
			}

			Label pageTitle = new Label()
			{
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				FontSize = 20,
				Text = $"Pause or Snooze the following protocols:"
			};
			Label protocolList = new Label()
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Text = protocols.Aggregate(protocolNames, (sb, p) => sb.AppendLine(p.Name)).ToString()
			};
			StackLayout protocolListLayout = new StackLayout()
			{
				Orientation = StackOrientation.Vertical,
				Children = { pageTitle, protocolList }
			};

			Frame protocolListFrame = new Frame()
			{
				Content = protocolListLayout,
				BorderColor = Color.Accent,
				BackgroundColor = Color.Transparent,
				VerticalOptions = LayoutOptions.Start,
				HasShadow = false,
				Padding = new Thickness(10)
			};

			Dictionary<string, int> resumeUnits = new Dictionary<string, int>
			{
				{ "Minute(s)", 1 },
				{ "Hour(s)", 60 },
				{ "Day(s)", 1440 }
			};

			Func<int, (double, int, string)> calculateTimeAmount = t =>
			{
				KeyValuePair<string, int> defaultItem = resumeUnits.OrderBy(x => x.Value).TakeWhile(x => x.Value <= t).Last();
				double timeAmount = Math.Round((double)t / defaultItem.Value, 2);
				int selectedUnit = resumeUnits.ToList().IndexOf(defaultItem);

				return (timeAmount, selectedUnit, defaultItem.Key);
			};

			(double Amount, int UnitIndex, string UnitDescription) initialAmountInUnits = calculateTimeAmount(Math.Min(DEFAULT_SNOOZE, maxSnoozeTimeAmount));

			Label timeAmountLabel = new Label()
			{
				Text = "Snooze for:",
				FontSize = 20,
				VerticalTextAlignment = TextAlignment.Center
			};
			Entry timeAmountEntry = new Entry()
			{
				Keyboard = Keyboard.Numeric,
				Text = initialAmountInUnits.Amount.ToString(),
				HorizontalOptions = LayoutOptions.FillAndExpand
			};
			Picker timeAmountPicker = new Picker()
			{
				ItemsSource = resumeUnits.Keys.ToList(),
				SelectedIndex = initialAmountInUnits.UnitIndex,
				HorizontalOptions = LayoutOptions.FillAndExpand
			};

			StackLayout timeAmountLayout = new StackLayout()
			{
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Children = { timeAmountLabel, timeAmountEntry, timeAmountPicker }
			};

			Switch untilSwitch = new Switch();
			Label dateTimeLabel = new Label() { Text = "or until:", FontSize = 20, VerticalTextAlignment = TextAlignment.Center };
			DatePicker datePicker = new DatePicker()
			{
				Date = DateTime.Now.Date,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				IsEnabled = false
			};
			TimePicker timePicker = new TimePicker()
			{
				Time = DateTime.Now.AddMinutes(Math.Min(DEFAULT_SNOOZE, maxSnoozeTimeAmount)).TimeOfDay,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				IsEnabled = false,
			};

			Func<(DateTime, TimeSpan)> getResumeTime = () =>
			{
				DateTime resumeTime = datePicker.Date.Date.AddMinutes((int)timePicker.Time.TotalMinutes);

				return (resumeTime, resumeTime - DateTime.Now);
			};

			Action syncronize = () =>
			{
				if (untilSwitch.IsToggled)
				{
					(DateTime resumeTime, TimeSpan difference) = getResumeTime();

					(double Amount, int UnitIndex, string UnitDescription) amountInUnits = calculateTimeAmount((int)difference.TotalMinutes);

					timeAmountEntry.Text = amountInUnits.Amount.ToString();
					timeAmountPicker.SelectedIndex = amountInUnits.UnitIndex;
				}
				else
				{
					if (double.TryParse(timeAmountEntry.Text, out double amount))
					{
						DateTime resumeTime = DateTime.Now.AddMinutes(resumeUnits[timeAmountPicker.SelectedItem as string] * amount);

						datePicker.Date = resumeTime.Date;
						timePicker.Time = resumeTime.TimeOfDay;
					}
				}
			};

			timeAmountEntry.TextChanged += (s, e) =>
			{
				if (untilSwitch.IsToggled == false)
				{
					syncronize();
				}
			};

			timeAmountPicker.SelectedIndexChanged += (s, e) =>
			{
				if (untilSwitch.IsToggled == false)
				{
					syncronize();
				}
			};

			datePicker.DateSelected += (s, e) =>
			{
				if (untilSwitch.IsToggled)
				{
					syncronize();
				}
			};

			timePicker.PropertyChanged += (s, e) =>
			{
				if (untilSwitch.IsToggled && e.PropertyName == TimePicker.TimeProperty.PropertyName)
				{
					syncronize();
				}
			};

			untilSwitch.Toggled += (s, e) =>
			{
				timeAmountEntry.IsEnabled = e.Value == false;
				timeAmountPicker.IsEnabled = e.Value == false;
				datePicker.IsEnabled = e.Value;
				timePicker.IsEnabled = e.Value;
			};

			StackLayout dateTimeLayout = new StackLayout()
			{
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Children = { untilSwitch, dateTimeLabel, datePicker, timePicker }
			};

			(double Amount, int UnitIndex, string UnitText) maxSnoozeTime = calculateTimeAmount(maxSnoozeTimeAmount);

			Label maxSnoozeLabel = new Label()
			{
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				Text = $"(Max Snooze: {maxSnoozeTime.Amount} {maxSnoozeTime.UnitText.ToLower()})"
			};

			if(maxSnoozeTimeAmount == int.MaxValue)
			{
				maxSnoozeLabel.IsVisible = false;
			}

			StackLayout resumeTimeLayout = new StackLayout()
			{
				Orientation = StackOrientation.Vertical,
				Children = { timeAmountLayout, dateTimeLayout, maxSnoozeLabel }
			};

			Button pauseButton = new Button()
			{
				Text = "Pause"
			};

			pauseButton.Clicked += async (s, e) =>
			{
				await PauseProtocolsAsync(protocols, null);

				await Navigation.PopAsync();
			};

			Button snoozeButton = new Button()
			{
				Text = "Snooze"
			};

			snoozeButton.Clicked += async (s, e) =>
			{
				(DateTime resumeTime, TimeSpan difference) = getResumeTime();

				if (difference.TotalMinutes <= maxSnoozeTimeAmount)
				{
					await PauseProtocolsAsync(protocols, resumeTime);

					await Navigation.PopAsync();
				}
				else
				{
					await DisplayAlert("Snooze", $"The protocols cannot be Snoozed for more than {maxSnoozeTime.Amount} {maxSnoozeTime.UnitText.ToLower()}", "OK");
				}
			};

			Button cancelButton = new Button()
			{
				Text = "Cancel"
			};

			cancelButton.Clicked += async (s, e) =>
			{
				_responseTaskCompletionSource.TrySetResult(false);

				await Navigation.PopAsync();
			};

			BoxView divider = new BoxView { Color = Color.Gray, HorizontalOptions = LayoutOptions.FillAndExpand, HeightRequest = 0.5 };

			StackLayout layout = new StackLayout()
			{
				Orientation = StackOrientation.Vertical,
				Padding = new Thickness(10, 20, 10, 20),
				Children = { protocolListFrame, pauseButton, resumeTimeLayout, snoozeButton, divider, cancelButton }
			};

			Content = layout;
		}

		public PauseProtocolsPage(Protocol protocol) : this(new Protocol[] { protocol })
		{

		}

		protected override bool OnBackButtonPressed()
		{
			_responseTaskCompletionSource.TrySetResult(false);

			return base.OnBackButtonPressed();
		}
	}
}
