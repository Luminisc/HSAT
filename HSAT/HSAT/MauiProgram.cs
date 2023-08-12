﻿using CommunityToolkit.Maui;
using HSAT.Core.Services;
using HSAT.Menus.CreateProject;
using HSAT.Modules.DatasetViewer;
using HSAT.Services;
using Microsoft.Maui.LifecycleEvents;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using SkiaSharp.Views.Maui.Controls.Hosting;
using Windows.Graphics;

namespace HSAT;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseSkiaSharp()
            .UseMauiCommunityToolkit()
            .AddViewModels()
            .AddServices()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.ConfigureLifecycleEvents(lifecycle =>
        {
            lifecycle.AddWindows(windows => windows.OnWindowCreated((window) =>
            {
                window.ExtendsContentIntoTitleBar = true;
                window.Title = "HSAT";

                IntPtr nativeWindowHandle = WinRT.Interop.WindowNative.GetWindowHandle(window);
                WindowId win32WindowsId = Win32Interop.GetWindowIdFromWindow(nativeWindowHandle);
                AppWindow winuiAppWindow = AppWindow.GetFromWindowId(win32WindowsId);
                if (winuiAppWindow.Presenter is OverlappedPresenter p)
                {
                    // p.Maximize();
                }
                else
                {
                    const int width = 1280;
                    const int height = 720;
                    winuiAppWindow.MoveAndResize(new RectInt32(
                        (int)DeviceDisplay.MainDisplayInfo.Width / 2 - width / 2,
                        (int)DeviceDisplay.MainDisplayInfo.Height / 2 - height / 2, 
                        width, 
                        height));
                }
            }));
        });

        return builder.Build();
    }

    public static MauiAppBuilder AddViewModels(this MauiAppBuilder builder)
    {
        builder.Services.AddTransient<CreateProjectViewModel>();
        return builder;
    }

    public static MauiAppBuilder AddServices(this MauiAppBuilder builder)
    {
        builder.Services.AddTransient<ProjectService>();
        builder.Services.AddScoped<IFileService, FileService>();
        return builder;
    }

    public static MauiAppBuilder AddComponents(this MauiAppBuilder builder)
    {
        builder.Services.AddTransient<CreateProjectPopup>();
        builder.Services.AddTransient<DatasetViewer>();
        return builder;
    }
}
