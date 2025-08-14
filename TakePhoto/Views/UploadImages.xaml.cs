using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TakePhoto.Models.HtsModels;
using TakePhoto.Services;

namespace TakePhoto.Views
{
    internal class UploadImages : ContentPage, INotifyPropertyChanged
    {
        private readonly ApiService _apiService;
        private ProductInfo _currentFilter;
        private readonly HttpClient _httpClient;
        public bool IsLoading { get; private set; }
        public string ErrorMessage { get; private set; }

        public UploadImages(ApiService apiService)
        {
            InitializeComponent();
            _apiService = apiService;

            // 绑定数据上下文
            BindingContext = this;
        }
        public void SetFilter(ProductInfo filter)
        {
            _currentFilter = filter;
            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                // 1. 更新加载状态
                SetLoadingState(true, null);
            }
            catch (Exception ex)
            {
                SetLoadingState(false, $"发生异常: {ex.Message}");
            }
            finally
            {
                SetLoadingState(false, null);
            }
        }

        // 更新加载状态和错误信息
        private void SetLoadingState(bool isLoading, string errorMessage)
        {
            // 确保在主线程更新UI
            MainThread.BeginInvokeOnMainThread(() =>
            {
                IsLoading = isLoading;
                ErrorMessage = errorMessage;
                OnPropertyChanged(nameof(IsLoading));
                OnPropertyChanged(nameof(ErrorMessage));
            });
        }
    }
}
    
