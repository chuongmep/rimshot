﻿using Autodesk.Navisworks.Api.Plugins;
using SpeckleNavisworks.Plugin;
using SpeckleNavisworks.Utilities;
using System.Text;
using NavisworksApp = Autodesk.Navisworks.Api.Application;

namespace SpeckleNavisworks.Ribbon {

  [Plugin( "Rimshot", "Rimshot", DisplayName = "Rimshot" )]
  [Strings( "Ribbon.name" )]
  [RibbonLayout( "Ribbon.xaml" )]
  [RibbonTab( "Rimshot", DisplayName = "Rimshot", LoadForCanExecute = true )]

  [Command( IssueList.Command,
             CallCanExecute = CallCanExecute.DocumentNotClear,
             Icon = "icon_register16.ico",
             LargeIcon = "icon_register32.ico",
             Shortcut = "Ctrl+Shift+R",
             ShortcutWindowTypes = "",
             ToolTip = "Show Rimshot ",
             ExtendedToolTip = "Show Rimshot Issue List",
             DisplayName = "Register"
             )]

  [Command( Plugin.Speckle.Command,
             CallCanExecute = CallCanExecute.DocumentNotClear,
             Icon = "icon_speckle16.ico",
             LargeIcon = "icon_speckle32.ico",
             Shortcut = "Ctrl+Shift+S",
             ShortcutWindowTypes = "",
             ToolTip = "Show Speckle Connector",
             ExtendedToolTip = "Show Speckle Connector",
             DisplayName = "Speckle"
             )]


  class RibbonHandler : CommandHandlerPlugin {
    public override CommandState CanExecuteCommand ( string commandId ) {

      CommandState state = new CommandState( true );

      switch ( commandId ) {
        case ShowOnly.Command: {

            if ( NavisworksApp.ActiveDocument.CurrentSelection.IsEmpty ) {
              state.IsEnabled = false;
            }

            break;
          }

        case ShowAlso.Command: {

            if ( NavisworksApp.ActiveDocument.CurrentSelection.IsEmpty ) {
              state.IsEnabled = false;
            }

            break;
          }

        default: {
            break;
          }
      }



      return state;


    }

    public RibbonHandler () { }
    public bool LoadPlugin ( string plugin, bool notAutomatedCheck = true, string command = "" ) {
      if ( notAutomatedCheck && NavisworksApp.IsAutomated ) {
        return false;
      }

      if ( plugin.Length == 0 || command.Length == 0 ) {
        return false;
      }

      PluginRecord pluginRecord = NavisworksApp.Plugins.FindPlugin( plugin + ".Rimshot" );

      if ( pluginRecord is null ) {
        return false;
      }

      Autodesk.Navisworks.Api.Plugins.Plugin loadedPlugin = pluginRecord.LoadedPlugin ?? pluginRecord.LoadPlugin();

      // Activate the Plugin's pane if it is of the right type
      if ( pluginRecord.IsLoaded && pluginRecord is DockPanePluginRecord && pluginRecord.IsEnabled ) {
        DockPanePlugin dockPanePlugin = ( DockPanePlugin )loadedPlugin;
        dockPanePlugin.ActivatePane();
      } else {
#if DEBUG
        StringBuilder sb = new StringBuilder();

        foreach ( PluginRecord pr in NavisworksApp.Plugins.PluginRecords ) {
          sb.AppendLine( pr.Name + ": " + pr.DisplayName + ", " + pr.Id );
        }

        MessageBox.Show( sb.ToString() );
        MessageBox.Show( command + " Plugin not loaded." );
#endif
      }

      return pluginRecord.IsLoaded;
    }

    public override int ExecuteCommand ( string commandId, params string[] parameters ) {
      string buildVersion = "2020";

#if IS2018
      buildVersion = "2018";
#endif
#if IS2019
      buildVersion = "2019";
#endif
#if IS2020
      buildVersion = "2020";
#endif
#if IS2021
      buildVersion = "2021";
#endif
#if IS2022
      buildVersion = "2022";
#endif

      // Version
      if ( !NavisworksApp.Version.RuntimeProductName.Contains( buildVersion ) ) {
        MessageBox.Show( "This Add-In was built for Navisworks " + buildVersion + ", please contact r+d@Rimshot.com for assistance...",
                        "Cannot Continue!",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error );
        return 0;
      }

      switch ( commandId ) {

        // Show Only Selected Elements
        case ShowOnly.Command: {
            Selections.ShowSelected( showOnlySelected: true );
            break;
          }
        // Show Only Selected Elements
        case ShowAlso.Command: {
            Selections.ShowSelected();
            break;
          }

        // Show Export BCF
        // This is a pane based tool
        case BCFExport.Command: {
            LoadPlugin( plugin: BCFExport.Plugin, command: commandId );
            break;
          }


        // Show Issue List
        // This is a pane based tool
        case IssueList.Command: {
            LoadPlugin( plugin: IssueList.Plugin, command: commandId );
            break;
          }




        default: {
            MessageBox.Show( "You have clicked on the command with ID = '" + commandId + "'" );
            break;
          }
      }

      return 0;
    }

  }
}