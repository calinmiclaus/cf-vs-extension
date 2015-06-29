using CloudFoundry.CloudController.V2.Client;
using CloudFoundry.VisualStudio.ProjectPush;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.VisualStudio.Threading;
using Microsoft.VisualStudio.PlatformUI;
using CloudFoundry.VisualStudio.MSBuild;

namespace CloudFoundry.VisualStudio.Forms
{
    /// <summary>
    /// Interaction logic for PushDialog.xaml
    /// </summary>
    public partial class PushDialog : DialogWindow
    {
        private CancellationToken cancellationToken;
        private PublishProfileEditorResources publishProfileResources;

        public PushDialog(PublishProfile package)
        {
            this.cancellationToken = new CancellationToken();
            this.publishProfileResources = new PublishProfileEditorResources(package, this.cancellationToken);
            this.DataContext = publishProfileResources;

            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.publishProfileResources.Refresh(PublishProfileRefreshTarget.Client);
        }

        private void wizardPush_Finish(object sender, RoutedEventArgs e)
        {

            this.publishProfileResources.CleanManifest();
            this.publishProfileResources.PublishProfile.Save();

            MSBuildProcess process = new MSBuildProcess();
            process.MSBuildProperties = new Dictionary<string, string>();
            process.MSBuildProperties.Add("DeployOnBuild", "true");
            process.MSBuildProperties.Add("PublishProfile", this.publishProfileResources.PublishProfile.Path);

            process.Publish(this.publishProfileResources.PublishProfile.Project);
        }
    }
}
