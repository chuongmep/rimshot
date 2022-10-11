using Autodesk.Navisworks.Api;
using NavisworksApp = Autodesk.Navisworks.Api.Application;

namespace SpeckleNavisworks.Utilities {
  class TaskProgress {

    public bool progressIsCancelled = false;

    #region Event Handlers

    private void EndProgress ( Progress p = null ) {
      this.progressIsCancelled = false;

      if ( p != null && !p.IsDisposed ) {
        p.Dispose();
      }
    }

    public void HandleEnd ( object sender, ProgressEndedEventArgs e ) {

      if ( sender != null ) {
        ProgressEnd();
      }

      //Progress p = (Progress)sender;
      ProgressEnd();
    }

    public void HandleSubOpEnd ( object sender, ProgressSubOperationEndedEventArgs e ) {
      Progress progress = e.Progress;
      if ( progress.IsCanceled && !this.progressIsCancelled ) {
        this.progressIsCancelled = true;
        //MessageBox.Show(" Cancelled.");
      }
    }

    public void ProgressEnd ( Progress p = null ) {
      EndProgress( p );
      NavisworksApp.ProgressSubOperationEnded -= HandleSubOpEnd;
      NavisworksApp.ProgressEnded -= HandleEnd;
    }

    #endregion

  }
}
