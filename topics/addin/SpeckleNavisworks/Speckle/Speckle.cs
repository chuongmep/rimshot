using Speckle.Core.Api;
using Speckle.Core.Credentials;
using Speckle.Core.Logging;
using Speckle.Newtonsoft.Json;
using SpeckleNavisworks.Plugin;
using Stream = Speckle.Core.Api.Stream;

namespace SpeckleNavisworks.Speckle {
  internal class SpeckleServer {
    internal string rimshotIssueId;

    public string HostUrl { get; set; }  // TODO: Feedback to the UI when the Client Account does not match the HostUrl. Handle the ability to Commit.
    public string StreamId { get { if ( this.Stream != null ) { return this.Stream.id; } return null; } }
    public string BranchName { get { if ( this.Branch != null ) { return this.Branch.name; } return null; } }
    public string CommitId { get; set; }
    public Account Account { get; set; }
    public Client Client { get; set; }
    public Branch Branch { get; set; }
    public Stream Stream { get; set; }

    public UIBindings RimshotApp { get; set; }

    public SpeckleServer () {
      Account defaultAccount = AccountManager.GetDefaultAccount();

      if ( defaultAccount == null ) {
        Logging.Logging.ErrorLog( new SpeckleException( $"You do not have any accounts active. Please create one or add it to the Speckle Manager." ), this.RimshotApp );
        return;
      }

      this.Account = defaultAccount;

      try {
        this.Client = new Client( this.Account ); // TODO: Feedback to the UI when the Client Account does not match the HostUrl.
      } catch ( Exception e ) {
        Logging.Logging.ErrorLog( e );
      }
    }

    public async Task TryGetStream ( string streamId ) {
      this.Stream = null;
      try {
        this.Stream = await this.Client.StreamGet( streamId );
      } catch {
        Logging.Logging.ErrorLog( new SpeckleException( $"You don't have access to stream {streamId} on server {this.HostUrl}, or the stream does not exist." ), this.RimshotApp );
      }
    }

    private Task<Branch> CreateBranch ( string name, string description ) {

      try {
        Task<string> branchId = this.Client.BranchCreate( new BranchCreateInput() {
          streamId = this.StreamId,
          name = name,
          description = description
        } );
      } catch ( Exception e ) {
        Logging.Logging.ErrorLog( e );
      }

      Task<Branch> branch = this.Client.BranchGet( this.StreamId, name );

      return branch;
    }


    public async Task TryGetBranch ( string name, string description = "" ) {
      this.Branch = null;

      // Get Branch and create if it doesn't exist.
      try {
        this.Branch = await this.Client.BranchGet( this.StreamId, name );
        if ( this.Branch is null ) {
          try {
            this.Branch = await CreateBranch( name, description );
          } catch ( Exception ) {
            Logging.Logging.ErrorLog( new SpeckleException( $"Unable to find an issue branch for {this.BranchName}" ), this.RimshotApp );
          }
        }
      } catch ( Exception ) {
        Logging.Logging.ErrorLog( new SpeckleException( $"Unable to find or create an issue branch for {this.BranchName}" ), this.RimshotApp );
        return;
      }

      if ( this.Branch == null ) {
        try {
          this.Branch = this.Client.BranchGet( this.StreamId, this.BranchName, 1 ).Result;

          if ( this.Branch != null ) {
            this.RimshotApp.NotifyUI( "branch_updated", JsonConvert.SerializeObject( new { branch = this.Branch.name, this.rimshotIssueId } ) );
          }
        } catch ( Exception ) {
          Logging.Logging.ErrorLog( new SpeckleException( $"Still unable to find an issue branch for {this.BranchName}" ), this.RimshotApp );
          return;
        }
      }
    }
  }
}
