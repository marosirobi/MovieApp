﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MovieApp.MVVM.Model;
using System.Collections.ObjectModel;

namespace MovieApp.MVVM.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private object _currentView;

        [ObservableProperty]
        private HomeViewModel _homeVM;

        [ObservableProperty]
        private DiscoveryViewModel _discoveryVM;

        [ObservableProperty]
        private ObservableCollection<MovieModel> _allMovies;

        public MainViewModel()
        {
            HomeVM = new HomeViewModel();
            DiscoveryVM = new DiscoveryViewModel();
            CurrentView = HomeVM;

            // Start loading movies when MainViewModel is created
            _ = InitializeMovies();
        }

        private async Task InitializeMovies()
        {
            AllMovies = await MovieApi.GetMoviesFromApi();

            HomeVM.SetMovies(AllMovies);
        }

        [RelayCommand]
        private void NavigateToHome()
        {
            CurrentView = HomeVM;
            HomeVM.SetMovies(AllMovies);
        }

        [RelayCommand]
        private void NavigateToDiscovery()
        {
            CurrentView = DiscoveryVM;
        }
    }
}