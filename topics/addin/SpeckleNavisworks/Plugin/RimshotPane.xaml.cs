﻿using Autodesk.Navisworks.Api;
using CefSharp;
using CefSharp.Wpf;
using Speckle.Newtonsoft.Json;
using SpeckleNavisworks.Views;
using System.Runtime.ExceptionServices;
using System.Windows;
using MessageBox = System.Windows.MessageBox;
using NavisworksApp = Autodesk.Navisworks.Api.Application;
using Path = System.IO.Path;
using UserControl = System.Windows.Controls.UserControl;

namespace SpeckleNavisworks.Plugin {
  public class Snapshot {
    [JsonProperty]
    internal string name;
    [JsonProperty]
    internal Guid guid;
    [JsonProperty]
    internal string image = "";
    [JsonProperty]
    internal string thumbnail = "";
    [JsonProperty]
    internal ImageViewpoint viewpoint;

    public Snapshot ( SavedViewpoint view ) {
      this.name = view.DisplayName;
      this.guid = view.Guid;

      ImageViewpoint vp = new ImageViewpoint( view );

      this.viewpoint = vp;
    }
  }

  public partial class RimshotPane : UserControl {
    public class Bindings : UIBindings { }

    public Bindings bindings;

    private readonly Document activeDocument = NavisworksApp.ActiveDocument;

    public RimshotPane ( string address = Bindings.Url ) {

      InitializeCef();
      InitializeComponent();

      this.bindings = new Bindings {
        Browser = this.Browser,
        Window = this,
      };

      this.Browser.JavascriptObjectRepository.Settings.LegacyBindingEnabled = true;
      this.Browser.JavascriptObjectRepository.Register( "UIBindings", this.bindings, isAsync: true, options: BindingOptions.DefaultBinder );

      this.Browser.Address = address;
      this.Browser.BrowserSettings.WebGl = CefState.Enabled;
    }
    private void InitializeCef () {
      if ( Cef.IsInitialized ) {
        return;
      }

      Cef.EnableHighDPISupport();

      CefSettings settings = new CefSettings() {
        CachePath = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData ), "CefSharp\\Cache" ),
        RemoteDebuggingPort = 8099,
        PersistUserPreferences = true
      };

      try {
        Cef.Initialize( settings, performDependencyCheck: true, browserProcessHandler: null );
      } catch ( Exception e ) {
        _ = MessageBox.Show( e.ToString() );
      }
    }
    private void SendIssueView ( object sender, RoutedEventArgs e ) => this.bindings.AddImage();

    private void ShowDevTools ( object sender, EventArgs e ) => this.Browser.ShowDevTools();

    private void Refresh ( object sender, EventArgs e ) => this.Browser.Reload( true );


    public void SubscribeCurrentViewEvents () => this.activeDocument.CurrentViewpoint.Changed += CurrentViewpoint_Changed;

    public void UnsubscribeCurrentViewEvents () => this.activeDocument.CurrentViewpoint.Changed -= CurrentViewpoint_Changed;

    [HandleProcessCorruptedStateExceptions]
    private void CurrentViewpoint_Changed ( object sender, EventArgs e ) {

      if ( sender != null ) {

        Document doc = sender as Document;
        Viewpoint p = doc.CurrentViewpoint;

        try {
          string camera = p.GetCamera();
          this.bindings.UpdateView( camera );

        } catch ( AccessViolationException err ) {
          Console.WriteLine( "View camera accessed prematurely" );
          Console.WriteLine( err.Message );
        }
      }
    }
  }
}
