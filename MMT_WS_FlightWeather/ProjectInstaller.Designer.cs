namespace MMT_WS_FlightWeather
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.MMT_WS_FlightWeather_serviceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.MMT_WS_FlightWeather_serviceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // MMT_WS_FlightWeather_serviceProcessInstaller
            // 
            this.MMT_WS_FlightWeather_serviceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.NetworkService;
            this.MMT_WS_FlightWeather_serviceProcessInstaller.Password = null;
            this.MMT_WS_FlightWeather_serviceProcessInstaller.Username = null;
            // 
            // MMT_WS_FlightWeather_serviceInstaller
            // 
            this.MMT_WS_FlightWeather_serviceInstaller.ServiceName = "MMT_WS_FlightWeather";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.MMT_WS_FlightWeather_serviceProcessInstaller,
            this.MMT_WS_FlightWeather_serviceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller MMT_WS_FlightWeather_serviceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller MMT_WS_FlightWeather_serviceInstaller;
    }
}