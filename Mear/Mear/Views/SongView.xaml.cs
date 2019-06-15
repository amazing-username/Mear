﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using MediaManager;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Mear.Constants;
using Mear.Models;
using Mear.Playback;
using Mear.Repositories.Database;
using Mear.Repositories.Remote;
using Mear.ViewModels;

namespace Mear.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SongView : ContentPage
	{
        #region Fields
        private StackLayout _playerIndicatorLayout;
        private Button _controlButton;
        private Label _songTitleLabel;
		private SongViewModel _viewModel;
		#endregion


		#region Properties
		#endregion


		#region Constructors
		public SongView()
		{
			InitializeComponent();

			BindingContext = _viewModel = new SongViewModel();
            AddPlayerIndicator();
            BackgroundControl();
		}
		#endregion


		#region Methods
        private async Task AddPlayerIndicator()
        {
            var mainLayout = SongViewMainLayout;
            if (_playerIndicatorLayout == null)
            {
                _playerIndicatorLayout = await ConfigureSongIndicatorLayout();
            }

            mainLayout.Children.Add(_playerIndicatorLayout);
        }
        private async Task<StackLayout> ConfigureSongIndicatorLayout()
        {
            var indicatorLayout = new StackLayout();
            indicatorLayout.Orientation = StackOrientation.Horizontal;
            indicatorLayout.Padding = 5;
            _controlButton = new Button();
            _controlButton.Clicked += ControlButton_Clicked;
            _songTitleLabel = new Label();
            _songTitleLabel.HorizontalOptions = LayoutOptions.Center;
            _songTitleLabel.VerticalOptions = LayoutOptions.Start;
            var isSongPlayer = MearPlayer.IsPlaying();

            if (isSongPlayer)
            {
                _controlButton.Text = "P";
                _songTitleLabel.Text = await MearPlayer.SongTitle();
            }
            else
            {
                _controlButton.Text = "S";
                _songTitleLabel.Text = string.Empty;
            }

            indicatorLayout.Children.Add(_controlButton);
            indicatorLayout.Children.Add(_songTitleLabel);

            return indicatorLayout;
        }

        #region Events
        private async void ControlButton_Clicked(object sender, EventArgs e)
        {
            if (MearPlayer.IsPlaying())
            {
                MearPlayer.ControlMusic(null, PlayControls.PAUSE);
            }
            else
            {
                MearPlayer.ControlMusic(null, PlayControls.RESUME);
            }
        }

        private async void SongListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			if (SongListView.SelectedItem == null)
			{
				SongListView.SelectedItem = null;
				return;
			}

			try
			{
				var song = (Song)SongListView.SelectedItem;
				if (song.Downloaded)
				{
                    await MearPlayer.ControlMusic(song, PlayControls.PLAYOFFLINE);
				}
				else
				{
                    song = await MearPlayer.ControlMusic(song, PlayControls.STREAM);
				}

			    Navigation.PushModalAsync(new NavigationPage(new MearPlayerView(song)));
			}
			catch (Exception ex)
			{
				var msg = ex.Message;
			}

			SongListView.SelectedItem = null;
		}

        private void SearchSong_TextChanged(object sender, TextChangedEventArgs e)
        {
            var text = e.NewTextValue.ToString();
            _viewModel.FilterSongs(text);
        }

		private async void SongOptions_Clicked(object sender, EventArgs e)
		{
			try
			{
				var btn = (Button)sender;
			}
			catch (Exception ex)
			{
				var msg = ex.Message;
			}
		}
        #endregion

        #region Background
        private async Task BackgroundControl()
        {
            try
            {
                new Thread(async () =>
                {
                    while (true)
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            if (MearPlayer.SongHasBeenChanged(MearPlayer.MusicViews.Song))
                            {
                                _songTitleLabel.Text = MearPlayer.SongTitle().Result;
                                MearPlayer.ResetSongChange(MearPlayer.MusicViews.Song);
                            }
                        });
                        await Task.Delay(800);
                    }
                }).Start();
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
        }
        #endregion

        #region Test
        public void TestRemoteSong()
		{
			var songRepo = new RemoteSongRepository();
			var songs = songRepo.RetrieveSongs();
		}
        #endregion
        #endregion
    }
}