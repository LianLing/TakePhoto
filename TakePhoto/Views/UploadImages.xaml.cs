using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TakePhoto.Models.HtsModels;
using TakePhoto.Services;
using ZXing.Net.Maui;

namespace TakePhoto.Views
{
    internal class UploadImages : ContentPage, INotifyPropertyChanged
    {
        private readonly ApiService _apiService;
        private ProductInfo _currentFilter;
        private readonly HttpClient _httpClient;
        public bool IsLoading { get; private set; }
        public string ErrorMessage { get; private set; }
        // 声明条码扫描器  
        private readonly BarcodeReaderView _barcodeReader;

        public UploadImages(ApiService apiService)
        {
            InitializeComponent();
            _apiService = apiService;

            // 绑定数据上下文  
            BindingContext = this;

            // 初始化条码扫描器  
            _barcodeReader = new BarcodeReaderView
            {
                // 支持的条码格式（二维码和条形码）  
                Options = new BarcodeReaderOptions
                {
                    Formats = BarcodeFormat.QrCode | BarcodeFormat.Code128 | BarcodeFormat.Code39,
                    AutoRotate = true,
                    TryHarder = true
                }
            };
            // 绑定扫描完成事件
            _barcodeReader.BarcodesDetected += BarcodeReader_BarcodesDetected;
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

        // 扫码按钮点击事件
        private async void OnScanButtonClicked(object sender, EventArgs e)
        {
            // 检查相机权限
            var cameraPermission = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (cameraPermission != PermissionStatus.Granted)
            {
                cameraPermission = await Permissions.RequestAsync<Permissions.Camera>();
                if (cameraPermission != PermissionStatus.Granted)
                {
                    await DisplayAlert("权限不足", "请授予相机权限以使用扫码功能", "确定");
                    return;
                }
            }

            // 创建扫码页面并导航
            var scanPage = new ContentPage
            {
                Title = "扫描二维码/条形码",
                Content = new VerticalStackLayout
                {
                    Children = {
                        _barcodeReader,
                        new Label {
                            Text = "将条码对准框内扫描",
                            HorizontalOptions = LayoutOptions.Center,
                            Margin = new Thickness(0, 10)
                        }
                    }
                }
            };

            // 添加返回按钮事件
            scanPage.ToolbarItems.Add(new ToolbarItem("取消", "", () =>
            {
                Navigation.PopModalAsync();
            }));

            await Navigation.PushModalAsync(scanPage);
        }

        // 条码识别完成事件
        private void BarcodeReader_BarcodesDetected(object sender, BarcodeDetectionEventArgs e)
        {
            // 在主线程更新UI
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                // 获取第一个识别结果
                var result = e.Results?.FirstOrDefault()?.Value;
                if (!string.IsNullOrEmpty(result))
                {
                    // 更新文本框内容
                    ScanResultEntry.Text = result;
                    // 关闭扫码页面
                    await Navigation.PopModalAsync();
                    // 显示成功提示
                    await DisplayAlert("扫描成功", $"识别结果: {result}", "确定");
                }
            });
        }

        // 页面消失时释放资源
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _barcodeReader.BarcodesDetected -= BarcodeReader_BarcodesDetected;
        }
    }
}
    
