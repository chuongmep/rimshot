﻿using Speckle.Core.Logging;
using SpeckleNavisworks.Plugin;

namespace SpeckleNavisworks.Logging {
  public static class Logging {
    public static void ConsoleLog ( string message, ConsoleColor color = ConsoleColor.Magenta ) {
      Console.ForegroundColor = color;
      Console.WriteLine( message );
      Console.ForegroundColor = ConsoleColor.Gray;
    }
    public static void ErrorLog ( Exception err, UIBindings app ) {
      ErrorLog( err.Message );

      if ( err is SpeckleException ) {
        app.NotifyUI( "error", new { message = err.Message } );
        return;
      }
      throw err;
    }

    public static void ErrorLog ( Exception err ) {
      ErrorLog( err.Message );
      throw err;
    }

    public static void ErrorLog ( string errorMessage ) => ConsoleLog( errorMessage, ConsoleColor.Red );
  }
}
