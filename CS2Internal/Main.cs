﻿using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CS2Internal.Game;
using CS2Internal.ImGuiInterface;
using CS2Internal.Utility;

namespace CS2Internal;

public abstract partial class Main
{
    [UnmanagedCallersOnly(EntryPoint = "DllMain", CallConvs = new[] { typeof(CallConvStdcall) })]
    private static bool DllMain(IntPtr hModule, uint ulReasonForCall, IntPtr lpReserved)
    {
        switch (ulReasonForCall)
        {
            case 1:
                Module = hModule;
                WinApi.AllocConsole();
                Stopwatch = new Stopwatch();
                Stopwatch.Start();
                ImGuiX.Renderer.Init(Renderer);
                Task.Run(MainThread);
                break;
        }

        return true;
    }

    private static void Renderer()
    {
        var deltaTime = Stopwatch!.Elapsed.TotalSeconds;
        FrameCounter++;
        Stopwatch.Restart();

        TimeCounter += deltaTime;
        if (TimeCounter >= RefreshTime)
        {
            Fps = FrameCounter / TimeCounter;
            FrameCounter = 0;
            TimeCounter -= RefreshTime;
        }

        if (ShowMenu) ImGuiUi.RenderMenu();

        if (Config.Overlay) ImGuiUi.RenderOverlay();
    }


    private static void MainThread()
    {
        while (IsRunning)
        {
            Cs2.UpdateEntityList();

            if (WinApi.GetAsyncKeyState(0x2D) != 0)
            {
                ShowMenu = !ShowMenu;
                Thread.Sleep(200);
            }

            if (WinApi.GetAsyncKeyState(0x23) != 0) WinApi.FreeLibrary(Module);
            Thread.Sleep(30);
        }
    }
}