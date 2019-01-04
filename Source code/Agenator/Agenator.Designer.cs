namespace Agenator
{
    partial class AgenatorForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.CustomLabel customLabel1 = new System.Windows.Forms.DataVisualization.Charting.CustomLabel();
            System.Windows.Forms.DataVisualization.Charting.CustomLabel customLabel2 = new System.Windows.Forms.DataVisualization.Charting.CustomLabel();
            System.Windows.Forms.DataVisualization.Charting.CustomLabel customLabel3 = new System.Windows.Forms.DataVisualization.Charting.CustomLabel();
            System.Windows.Forms.DataVisualization.Charting.CustomLabel customLabel4 = new System.Windows.Forms.DataVisualization.Charting.CustomLabel();
            System.Windows.Forms.DataVisualization.Charting.CustomLabel customLabel5 = new System.Windows.Forms.DataVisualization.Charting.CustomLabel();
            System.Windows.Forms.DataVisualization.Charting.CustomLabel customLabel6 = new System.Windows.Forms.DataVisualization.Charting.CustomLabel();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint1 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0D, 100D);
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint2 = new System.Windows.Forms.DataVisualization.Charting.DataPoint(0D, 20000D);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AgenatorForm));
            this.menu = new System.Windows.Forms.MenuStrip();
            this.menu_config = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_config_openConfig = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_config_saveConfig = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_copyright = new System.Windows.Forms.ToolStripTextBox();
            this.menu_autosave = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_autosave_enableDisable = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_autosave_separator = new System.Windows.Forms.ToolStripSeparator();
            this.menu_autosave_interval_label = new System.Windows.Forms.ToolStripTextBox();
            this.menu_autosave_interval_value = new System.Windows.Forms.ToolStripTextBox();
            this.menu_autosave_separator2 = new System.Windows.Forms.ToolStripSeparator();
            this.menu_autosave_load = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_autosave_status = new System.Windows.Forms.ToolStripTextBox();
            this.openConfigFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveConfigFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.recordFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.HumidityControl_label = new System.Windows.Forms.Label();
            this.HumidityControl_steps = new System.Windows.Forms.FlowLayoutPanel();
            this.HumidityControl_steps_table = new System.Windows.Forms.TableLayoutPanel();
            this.HumidityControl_steps_1 = new System.Windows.Forms.FlowLayoutPanel();
            this.HumidityControl_steps_1_order_up = new System.Windows.Forms.Button();
            this.HumidityControl_steps_1_order_number = new System.Windows.Forms.Label();
            this.HumidityControl_steps_1_order_down = new System.Windows.Forms.Button();
            this.HumidityControl_steps_1_type = new System.Windows.Forms.ComboBox();
            this.HumidityControl_steps_1_humidity = new System.Windows.Forms.NumericUpDown();
            this.HumidityControl_steps_1_duration_day_value = new System.Windows.Forms.NumericUpDown();
            this.HumidityControl_steps_1_duration_day_label = new System.Windows.Forms.Label();
            this.HumidityControl_steps_1_duration_hour_value = new System.Windows.Forms.NumericUpDown();
            this.HumidityControl_steps_1_duration_hour_label = new System.Windows.Forms.Label();
            this.HumidityControl_steps_1_duration_minute_value = new System.Windows.Forms.NumericUpDown();
            this.HumidityControl_steps_1_duration_minute_label = new System.Windows.Forms.Label();
            this.HumidityControl_steps_1_duration_second_value = new System.Windows.Forms.NumericUpDown();
            this.HumidityControl_steps_1_duration_second_label = new System.Windows.Forms.Label();
            this.HumidityControl_steps_1_fanSpeed = new System.Windows.Forms.NumericUpDown();
            this.HumidityControl_steps_1_status = new System.Windows.Forms.ComboBox();
            this.HumidityControl_steps_1_delete = new System.Windows.Forms.Button();
            this.HumidityControl_heading = new System.Windows.Forms.FlowLayoutPanel();
            this.HumidityControl_heading_order = new System.Windows.Forms.Label();
            this.HumidityControl_heading_type = new System.Windows.Forms.Label();
            this.HumidityControl_heading_humidity = new System.Windows.Forms.Label();
            this.HumidityControl_heading_duration = new System.Windows.Forms.Label();
            this.HumidityControl_heading_fanSpeed = new System.Windows.Forms.Label();
            this.HumidityControl_heading_status = new System.Windows.Forms.Label();
            this.HumidityControl = new System.Windows.Forms.FlowLayoutPanel();
            this.HumidityControl_addStep = new System.Windows.Forms.Button();
            this.HumidityControl_start = new System.Windows.Forms.Button();
            this.HumidityControl_pause = new System.Windows.Forms.Button();
            this.Body = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.Body_panel_top = new System.Windows.Forms.TableLayoutPanel();
            this.Chamber_flow = new System.Windows.Forms.FlowLayoutPanel();
            this.Chamber_prev = new System.Windows.Forms.Button();
            this.Chamber_title = new System.Windows.Forms.Label();
            this.Chamber_next = new System.Windows.Forms.Button();
            this.Advanced_button = new System.Windows.Forms.FlowLayoutPanel();
            this.Advanced_button_show = new System.Windows.Forms.Button();
            this.Advanced_button_hide = new System.Windows.Forms.Button();
            this.Advanced_options = new System.Windows.Forms.FlowLayoutPanel();
            this.Advanced_humidity = new System.Windows.Forms.GroupBox();
            this.Advanced_humidity_flow = new System.Windows.Forms.FlowLayoutPanel();
            this.Advanced_humidity_RH1_raw_label = new System.Windows.Forms.Label();
            this.Advanced_humidity_RH1_raw_value = new System.Windows.Forms.NumericUpDown();
            this.Advanced_humidity_RH1_std_label = new System.Windows.Forms.Label();
            this.Advanced_humidity_RH1_std_value = new System.Windows.Forms.NumericUpDown();
            this.Advanced_humidity_RH1_std_apply = new System.Windows.Forms.Button();
            this.Advanced_humidity_RH2_raw_label = new System.Windows.Forms.Label();
            this.Advanced_humidity_RH2_raw_value = new System.Windows.Forms.NumericUpDown();
            this.Advanced_humidity_RH2_std_label = new System.Windows.Forms.Label();
            this.Advanced_humidity_RH2_std_value = new System.Windows.Forms.NumericUpDown();
            this.Advanced_humidity_RH2_std_apply = new System.Windows.Forms.Button();
            this.Advanced_weigh = new System.Windows.Forms.GroupBox();
            this.Advanced_weigh_flow = new System.Windows.Forms.FlowLayoutPanel();
            this.Advanced_weigh_factor_label = new System.Windows.Forms.Label();
            this.Advanced_weigh_factor_value = new System.Windows.Forms.NumericUpDown();
            this.Advanced_weigh_offset_label = new System.Windows.Forms.Label();
            this.Advanced_weigh_offset_value = new System.Windows.Forms.NumericUpDown();
            this.Readings = new System.Windows.Forms.FlowLayoutPanel();
            this.Readings_humidity = new System.Windows.Forms.GroupBox();
            this.Readings_humidity_flow = new System.Windows.Forms.FlowLayoutPanel();
            this.Readings_humidity_flow_value = new System.Windows.Forms.TextBox();
            this.Readings_humidity_flow_label = new System.Windows.Forms.Label();
            this.Readings_temperature = new System.Windows.Forms.GroupBox();
            this.Readings_temperature_flow = new System.Windows.Forms.FlowLayoutPanel();
            this.Readings_temperature_flow_value = new System.Windows.Forms.TextBox();
            this.Readings_temperature_flow_label = new System.Windows.Forms.Label();
            this.Readings_weight = new System.Windows.Forms.GroupBox();
            this.Readings_weight_flow = new System.Windows.Forms.FlowLayoutPanel();
            this.Readings_weight_flow_value = new System.Windows.Forms.TextBox();
            this.Readings_weight_flow_label = new System.Windows.Forms.Label();
            this.Readings_velocity = new System.Windows.Forms.GroupBox();
            this.Readings_velocity_flow = new System.Windows.Forms.FlowLayoutPanel();
            this.Readings_velocity_flow_value = new System.Windows.Forms.TextBox();
            this.Readings_velocity_flow_label = new System.Windows.Forms.Label();
            this.Readings_fan1Speed = new System.Windows.Forms.GroupBox();
            this.Readings_fan1Speed_flow = new System.Windows.Forms.FlowLayoutPanel();
            this.Readings_fan1Speed_flow_value = new System.Windows.Forms.TextBox();
            this.Readings_fan1Speed_flow_label = new System.Windows.Forms.Label();
            this.Readings_fan2Speed = new System.Windows.Forms.GroupBox();
            this.Readings_fan2Speed_flow = new System.Windows.Forms.FlowLayoutPanel();
            this.Readings_fan2Speed_flow_value = new System.Windows.Forms.TextBox();
            this.Readings_fan2Speed_flow_label = new System.Windows.Forms.Label();
            this.Data = new System.Windows.Forms.FlowLayoutPanel();
            this.Data_DAQ = new System.Windows.Forms.GroupBox();
            this.Data_DAQ_flow = new System.Windows.Forms.FlowLayoutPanel();
            this.Data_DAQ_interval_label1 = new System.Windows.Forms.Label();
            this.Data_DAQ_interval_value = new System.Windows.Forms.NumericUpDown();
            this.Data_DAQ_interval_label2 = new System.Windows.Forms.Label();
            this.Data_DAQ_start = new System.Windows.Forms.Button();
            this.Data_DAQ_stop = new System.Windows.Forms.Button();
            this.Data_save = new System.Windows.Forms.GroupBox();
            this.Data_save_flow = new System.Windows.Forms.FlowLayoutPanel();
            this.Data_save_flow_path = new System.Windows.Forms.TextBox();
            this.Data_save_flow_browse = new System.Windows.Forms.Button();
            this.Data_save_flow_start = new System.Windows.Forms.Button();
            this.Data_save_flow_stop = new System.Windows.Forms.Button();
            this.Other = new System.Windows.Forms.FlowLayoutPanel();
            this.Graph = new System.Windows.Forms.GroupBox();
            this.Graph_flow = new System.Windows.Forms.FlowLayoutPanel();
            this.Graph_interval_label1 = new System.Windows.Forms.Label();
            this.Graph_interval_value = new System.Windows.Forms.NumericUpDown();
            this.Graph_interval_label2 = new System.Windows.Forms.Label();
            this.WeighScale = new System.Windows.Forms.GroupBox();
            this.WeighScale_flow = new System.Windows.Forms.FlowLayoutPanel();
            this.WeighScale_flow_tare = new System.Windows.Forms.Button();
            this.WeighScale_flow_cal_label1 = new System.Windows.Forms.Label();
            this.WeighScale_flow_cal_value = new System.Windows.Forms.NumericUpDown();
            this.WeighScale_flow_cal_label2 = new System.Windows.Forms.Label();
            this.WeighScale_flow_cal_button = new System.Windows.Forms.Button();
            this.Status = new System.Windows.Forms.GroupBox();
            this.Status_flow = new System.Windows.Forms.FlowLayoutPanel();
            this.Status_value = new System.Windows.Forms.TextBox();
            this.Displaychart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.Body_panel_btm = new System.Windows.Forms.TableLayoutPanel();
            this.menu.SuspendLayout();
            this.HumidityControl_steps.SuspendLayout();
            this.HumidityControl_steps_table.SuspendLayout();
            this.HumidityControl_steps_1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.HumidityControl_steps_1_humidity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.HumidityControl_steps_1_duration_day_value)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.HumidityControl_steps_1_duration_hour_value)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.HumidityControl_steps_1_duration_minute_value)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.HumidityControl_steps_1_duration_second_value)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.HumidityControl_steps_1_fanSpeed)).BeginInit();
            this.HumidityControl_heading.SuspendLayout();
            this.HumidityControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Body)).BeginInit();
            this.Body.Panel1.SuspendLayout();
            this.Body.Panel2.SuspendLayout();
            this.Body.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.Body_panel_top.SuspendLayout();
            this.Chamber_flow.SuspendLayout();
            this.Advanced_button.SuspendLayout();
            this.Advanced_options.SuspendLayout();
            this.Advanced_humidity.SuspendLayout();
            this.Advanced_humidity_flow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Advanced_humidity_RH1_raw_value)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Advanced_humidity_RH1_std_value)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Advanced_humidity_RH2_raw_value)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Advanced_humidity_RH2_std_value)).BeginInit();
            this.Advanced_weigh.SuspendLayout();
            this.Advanced_weigh_flow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Advanced_weigh_factor_value)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Advanced_weigh_offset_value)).BeginInit();
            this.Readings.SuspendLayout();
            this.Readings_humidity.SuspendLayout();
            this.Readings_humidity_flow.SuspendLayout();
            this.Readings_temperature.SuspendLayout();
            this.Readings_temperature_flow.SuspendLayout();
            this.Readings_weight.SuspendLayout();
            this.Readings_weight_flow.SuspendLayout();
            this.Readings_velocity.SuspendLayout();
            this.Readings_velocity_flow.SuspendLayout();
            this.Readings_fan1Speed.SuspendLayout();
            this.Readings_fan1Speed_flow.SuspendLayout();
            this.Readings_fan2Speed.SuspendLayout();
            this.Readings_fan2Speed_flow.SuspendLayout();
            this.Data.SuspendLayout();
            this.Data_DAQ.SuspendLayout();
            this.Data_DAQ_flow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Data_DAQ_interval_value)).BeginInit();
            this.Data_save.SuspendLayout();
            this.Data_save_flow.SuspendLayout();
            this.Other.SuspendLayout();
            this.Graph.SuspendLayout();
            this.Graph_flow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Graph_interval_value)).BeginInit();
            this.WeighScale.SuspendLayout();
            this.WeighScale_flow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.WeighScale_flow_cal_value)).BeginInit();
            this.Status.SuspendLayout();
            this.Status_flow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Displaychart)).BeginInit();
            this.Body_panel_btm.SuspendLayout();
            this.SuspendLayout();
            // 
            // menu
            // 
            this.menu.BackColor = System.Drawing.Color.White;
            this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menu_config,
            this.menu_copyright,
            this.menu_autosave,
            this.menu_autosave_status});
            this.menu.Location = new System.Drawing.Point(0, 0);
            this.menu.Name = "menu";
            this.menu.Size = new System.Drawing.Size(1164, 27);
            this.menu.TabIndex = 19;
            this.menu.Text = "Menu";
            // 
            // menu_config
            // 
            this.menu_config.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menu_config_openConfig,
            this.menu_config_saveConfig});
            this.menu_config.Name = "menu_config";
            this.menu_config.Size = new System.Drawing.Size(93, 23);
            this.menu_config.Text = "Configuration";
            // 
            // menu_config_openConfig
            // 
            this.menu_config_openConfig.Name = "menu_config_openConfig";
            this.menu_config_openConfig.Size = new System.Drawing.Size(212, 22);
            this.menu_config_openConfig.Text = "Open configuration file";
            this.menu_config_openConfig.Click += new System.EventHandler(this.menu_config_openConfig_Click);
            // 
            // menu_config_saveConfig
            // 
            this.menu_config_saveConfig.Name = "menu_config_saveConfig";
            this.menu_config_saveConfig.Size = new System.Drawing.Size(212, 22);
            this.menu_config_saveConfig.Text = "Save configuration file as..";
            this.menu_config_saveConfig.Click += new System.EventHandler(this.menu_config_saveConfig_Click);
            // 
            // menu_copyright
            // 
            this.menu_copyright.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.menu_copyright.BackColor = System.Drawing.Color.White;
            this.menu_copyright.Enabled = false;
            this.menu_copyright.ForeColor = System.Drawing.Color.White;
            this.menu_copyright.Name = "menu_copyright";
            this.menu_copyright.ReadOnly = true;
            this.menu_copyright.Size = new System.Drawing.Size(140, 23);
            this.menu_copyright.Text = "v 4.0. Soon Kiat Lau, 2018";
            this.menu_copyright.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // menu_autosave
            // 
            this.menu_autosave.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menu_autosave_enableDisable,
            this.menu_autosave_separator,
            this.menu_autosave_interval_label,
            this.menu_autosave_interval_value,
            this.menu_autosave_separator2,
            this.menu_autosave_load});
            this.menu_autosave.Name = "menu_autosave";
            this.menu_autosave.Size = new System.Drawing.Size(68, 23);
            this.menu_autosave.Text = "Autosave";
            // 
            // menu_autosave_enableDisable
            // 
            this.menu_autosave_enableDisable.Name = "menu_autosave_enableDisable";
            this.menu_autosave_enableDisable.Size = new System.Drawing.Size(200, 22);
            this.menu_autosave_enableDisable.Text = "Enable autosave";
            this.menu_autosave_enableDisable.Click += new System.EventHandler(this.menu_autosave_enableDisable_Click);
            // 
            // menu_autosave_separator
            // 
            this.menu_autosave_separator.Name = "menu_autosave_separator";
            this.menu_autosave_separator.Size = new System.Drawing.Size(197, 6);
            // 
            // menu_autosave_interval_label
            // 
            this.menu_autosave_interval_label.BackColor = System.Drawing.Color.White;
            this.menu_autosave_interval_label.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.menu_autosave_interval_label.Enabled = false;
            this.menu_autosave_interval_label.ForeColor = System.Drawing.SystemColors.ControlText;
            this.menu_autosave_interval_label.Name = "menu_autosave_interval_label";
            this.menu_autosave_interval_label.Size = new System.Drawing.Size(140, 16);
            this.menu_autosave_interval_label.Text = "Autosave interval (min):";
            // 
            // menu_autosave_interval_value
            // 
            this.menu_autosave_interval_value.BackColor = System.Drawing.Color.White;
            this.menu_autosave_interval_value.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.menu_autosave_interval_value.Name = "menu_autosave_interval_value";
            this.menu_autosave_interval_value.Size = new System.Drawing.Size(140, 23);
            this.menu_autosave_interval_value.Text = "5";
            this.menu_autosave_interval_value.TextChanged += new System.EventHandler(this.menu_autosave_interval_value_TextChanged);
            // 
            // menu_autosave_separator2
            // 
            this.menu_autosave_separator2.Name = "menu_autosave_separator2";
            this.menu_autosave_separator2.Size = new System.Drawing.Size(197, 6);
            // 
            // menu_autosave_load
            // 
            this.menu_autosave_load.Name = "menu_autosave_load";
            this.menu_autosave_load.Size = new System.Drawing.Size(200, 22);
            this.menu_autosave_load.Text = "Load recovery file";
            this.menu_autosave_load.Click += new System.EventHandler(this.menu_autosave_load_Click);
            // 
            // menu_autosave_status
            // 
            this.menu_autosave_status.AutoSize = false;
            this.menu_autosave_status.BackColor = System.Drawing.Color.DarkGray;
            this.menu_autosave_status.Enabled = false;
            this.menu_autosave_status.Name = "menu_autosave_status";
            this.menu_autosave_status.Size = new System.Drawing.Size(130, 23);
            this.menu_autosave_status.Text = "Autosave disabled";
            this.menu_autosave_status.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // openConfigFileDialog
            // 
            this.openConfigFileDialog.AddExtension = false;
            this.openConfigFileDialog.Filter = "CSV file|*.csv";
            this.openConfigFileDialog.Title = "Open configuration file";
            // 
            // saveConfigFileDialog
            // 
            this.saveConfigFileDialog.DefaultExt = "csv";
            this.saveConfigFileDialog.Filter = "CSV file|*.csv";
            this.saveConfigFileDialog.Title = "Save configuration file";
            // 
            // recordFileDialog
            // 
            this.recordFileDialog.DefaultExt = "csv";
            this.recordFileDialog.Filter = "CSV file|*.csv";
            this.recordFileDialog.Title = "File to save recorded data";
            // 
            // HumidityControl_label
            // 
            this.HumidityControl_label.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.HumidityControl_label.AutoSize = true;
            this.HumidityControl_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HumidityControl_label.Location = new System.Drawing.Point(454, 0);
            this.HumidityControl_label.Margin = new System.Windows.Forms.Padding(0);
            this.HumidityControl_label.Name = "HumidityControl_label";
            this.HumidityControl_label.Size = new System.Drawing.Size(251, 25);
            this.HumidityControl_label.TabIndex = 0;
            this.HumidityControl_label.Text = "Humidity control program";
            // 
            // HumidityControl_steps
            // 
            this.HumidityControl_steps.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.HumidityControl_steps.AutoScroll = true;
            this.HumidityControl_steps.Controls.Add(this.HumidityControl_steps_table);
            this.HumidityControl_steps.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.HumidityControl_steps.Location = new System.Drawing.Point(22, 83);
            this.HumidityControl_steps.Name = "HumidityControl_steps";
            this.HumidityControl_steps.Size = new System.Drawing.Size(1116, 152);
            this.HumidityControl_steps.TabIndex = 8;
            // 
            // HumidityControl_steps_table
            // 
            this.HumidityControl_steps_table.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.HumidityControl_steps_table.AutoScroll = true;
            this.HumidityControl_steps_table.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.HumidityControl_steps_table.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.OutsetDouble;
            this.HumidityControl_steps_table.ColumnCount = 1;
            this.HumidityControl_steps_table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.HumidityControl_steps_table.Controls.Add(this.HumidityControl_steps_1, 0, 0);
            this.HumidityControl_steps_table.Location = new System.Drawing.Point(0, 0);
            this.HumidityControl_steps_table.Margin = new System.Windows.Forms.Padding(0);
            this.HumidityControl_steps_table.Name = "HumidityControl_steps_table";
            this.HumidityControl_steps_table.RowCount = 2;
            this.HumidityControl_steps_table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.HumidityControl_steps_table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.HumidityControl_steps_table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.HumidityControl_steps_table.Size = new System.Drawing.Size(1116, 75);
            this.HumidityControl_steps_table.TabIndex = 4;
            // 
            // HumidityControl_steps_1
            // 
            this.HumidityControl_steps_1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.HumidityControl_steps_1.Controls.Add(this.HumidityControl_steps_1_order_up);
            this.HumidityControl_steps_1.Controls.Add(this.HumidityControl_steps_1_order_number);
            this.HumidityControl_steps_1.Controls.Add(this.HumidityControl_steps_1_order_down);
            this.HumidityControl_steps_1.Controls.Add(this.HumidityControl_steps_1_type);
            this.HumidityControl_steps_1.Controls.Add(this.HumidityControl_steps_1_humidity);
            this.HumidityControl_steps_1.Controls.Add(this.HumidityControl_steps_1_duration_day_value);
            this.HumidityControl_steps_1.Controls.Add(this.HumidityControl_steps_1_duration_day_label);
            this.HumidityControl_steps_1.Controls.Add(this.HumidityControl_steps_1_duration_hour_value);
            this.HumidityControl_steps_1.Controls.Add(this.HumidityControl_steps_1_duration_hour_label);
            this.HumidityControl_steps_1.Controls.Add(this.HumidityControl_steps_1_duration_minute_value);
            this.HumidityControl_steps_1.Controls.Add(this.HumidityControl_steps_1_duration_minute_label);
            this.HumidityControl_steps_1.Controls.Add(this.HumidityControl_steps_1_duration_second_value);
            this.HumidityControl_steps_1.Controls.Add(this.HumidityControl_steps_1_duration_second_label);
            this.HumidityControl_steps_1.Controls.Add(this.HumidityControl_steps_1_fanSpeed);
            this.HumidityControl_steps_1.Controls.Add(this.HumidityControl_steps_1_status);
            this.HumidityControl_steps_1.Controls.Add(this.HumidityControl_steps_1_delete);
            this.HumidityControl_steps_1.Location = new System.Drawing.Point(3, 3);
            this.HumidityControl_steps_1.Margin = new System.Windows.Forms.Padding(0);
            this.HumidityControl_steps_1.Name = "HumidityControl_steps_1";
            this.HumidityControl_steps_1.Size = new System.Drawing.Size(1110, 33);
            this.HumidityControl_steps_1.TabIndex = 0;
            // 
            // HumidityControl_steps_1_order_up
            // 
            this.HumidityControl_steps_1_order_up.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HumidityControl_steps_1_order_up.Location = new System.Drawing.Point(3, 3);
            this.HumidityControl_steps_1_order_up.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.HumidityControl_steps_1_order_up.Name = "HumidityControl_steps_1_order_up";
            this.HumidityControl_steps_1_order_up.Size = new System.Drawing.Size(29, 23);
            this.HumidityControl_steps_1_order_up.TabIndex = 0;
            this.HumidityControl_steps_1_order_up.Text = "▲";
            this.HumidityControl_steps_1_order_up.UseVisualStyleBackColor = true;
            // 
            // HumidityControl_steps_1_order_number
            // 
            this.HumidityControl_steps_1_order_number.AutoSize = true;
            this.HumidityControl_steps_1_order_number.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HumidityControl_steps_1_order_number.Location = new System.Drawing.Point(32, 0);
            this.HumidityControl_steps_1_order_number.Margin = new System.Windows.Forms.Padding(0);
            this.HumidityControl_steps_1_order_number.Name = "HumidityControl_steps_1_order_number";
            this.HumidityControl_steps_1_order_number.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.HumidityControl_steps_1_order_number.Size = new System.Drawing.Size(15, 28);
            this.HumidityControl_steps_1_order_number.TabIndex = 15;
            this.HumidityControl_steps_1_order_number.Text = "1";
            this.HumidityControl_steps_1_order_number.UseCompatibleTextRendering = true;
            // 
            // HumidityControl_steps_1_order_down
            // 
            this.HumidityControl_steps_1_order_down.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HumidityControl_steps_1_order_down.Location = new System.Drawing.Point(47, 3);
            this.HumidityControl_steps_1_order_down.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.HumidityControl_steps_1_order_down.Name = "HumidityControl_steps_1_order_down";
            this.HumidityControl_steps_1_order_down.Size = new System.Drawing.Size(29, 23);
            this.HumidityControl_steps_1_order_down.TabIndex = 1;
            this.HumidityControl_steps_1_order_down.Text = "▼";
            this.HumidityControl_steps_1_order_down.UseVisualStyleBackColor = true;
            // 
            // HumidityControl_steps_1_type
            // 
            this.HumidityControl_steps_1_type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.HumidityControl_steps_1_type.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HumidityControl_steps_1_type.FormattingEnabled = true;
            this.HumidityControl_steps_1_type.Items.AddRange(new object[] {
            "Hold",
            "Ramp"});
            this.HumidityControl_steps_1_type.Location = new System.Drawing.Point(99, 3);
            this.HumidityControl_steps_1_type.Margin = new System.Windows.Forms.Padding(20, 3, 3, 3);
            this.HumidityControl_steps_1_type.Name = "HumidityControl_steps_1_type";
            this.HumidityControl_steps_1_type.Size = new System.Drawing.Size(60, 24);
            this.HumidityControl_steps_1_type.TabIndex = 2;
            // 
            // HumidityControl_steps_1_humidity
            // 
            this.HumidityControl_steps_1_humidity.DecimalPlaces = 1;
            this.HumidityControl_steps_1_humidity.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HumidityControl_steps_1_humidity.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.HumidityControl_steps_1_humidity.Location = new System.Drawing.Point(222, 3);
            this.HumidityControl_steps_1_humidity.Margin = new System.Windows.Forms.Padding(60, 3, 3, 3);
            this.HumidityControl_steps_1_humidity.Name = "HumidityControl_steps_1_humidity";
            this.HumidityControl_steps_1_humidity.Size = new System.Drawing.Size(50, 23);
            this.HumidityControl_steps_1_humidity.TabIndex = 3;
            this.HumidityControl_steps_1_humidity.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // HumidityControl_steps_1_duration_day_value
            // 
            this.HumidityControl_steps_1_duration_day_value.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HumidityControl_steps_1_duration_day_value.Location = new System.Drawing.Point(385, 3);
            this.HumidityControl_steps_1_duration_day_value.Margin = new System.Windows.Forms.Padding(110, 3, 0, 3);
            this.HumidityControl_steps_1_duration_day_value.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.HumidityControl_steps_1_duration_day_value.Name = "HumidityControl_steps_1_duration_day_value";
            this.HumidityControl_steps_1_duration_day_value.Size = new System.Drawing.Size(45, 23);
            this.HumidityControl_steps_1_duration_day_value.TabIndex = 4;
            this.HumidityControl_steps_1_duration_day_value.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // HumidityControl_steps_1_duration_day_label
            // 
            this.HumidityControl_steps_1_duration_day_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HumidityControl_steps_1_duration_day_label.Location = new System.Drawing.Point(430, 0);
            this.HumidityControl_steps_1_duration_day_label.Margin = new System.Windows.Forms.Padding(0);
            this.HumidityControl_steps_1_duration_day_label.Name = "HumidityControl_steps_1_duration_day_label";
            this.HumidityControl_steps_1_duration_day_label.Size = new System.Drawing.Size(25, 25);
            this.HumidityControl_steps_1_duration_day_label.TabIndex = 5;
            this.HumidityControl_steps_1_duration_day_label.Text = "D";
            // 
            // HumidityControl_steps_1_duration_hour_value
            // 
            this.HumidityControl_steps_1_duration_hour_value.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HumidityControl_steps_1_duration_hour_value.Location = new System.Drawing.Point(475, 3);
            this.HumidityControl_steps_1_duration_hour_value.Margin = new System.Windows.Forms.Padding(20, 3, 0, 3);
            this.HumidityControl_steps_1_duration_hour_value.Maximum = new decimal(new int[] {
            23,
            0,
            0,
            0});
            this.HumidityControl_steps_1_duration_hour_value.Name = "HumidityControl_steps_1_duration_hour_value";
            this.HumidityControl_steps_1_duration_hour_value.Size = new System.Drawing.Size(35, 23);
            this.HumidityControl_steps_1_duration_hour_value.TabIndex = 6;
            this.HumidityControl_steps_1_duration_hour_value.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // HumidityControl_steps_1_duration_hour_label
            // 
            this.HumidityControl_steps_1_duration_hour_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HumidityControl_steps_1_duration_hour_label.Location = new System.Drawing.Point(510, 0);
            this.HumidityControl_steps_1_duration_hour_label.Margin = new System.Windows.Forms.Padding(0);
            this.HumidityControl_steps_1_duration_hour_label.Name = "HumidityControl_steps_1_duration_hour_label";
            this.HumidityControl_steps_1_duration_hour_label.Size = new System.Drawing.Size(25, 25);
            this.HumidityControl_steps_1_duration_hour_label.TabIndex = 7;
            this.HumidityControl_steps_1_duration_hour_label.Text = "H";
            // 
            // HumidityControl_steps_1_duration_minute_value
            // 
            this.HumidityControl_steps_1_duration_minute_value.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HumidityControl_steps_1_duration_minute_value.Location = new System.Drawing.Point(555, 3);
            this.HumidityControl_steps_1_duration_minute_value.Margin = new System.Windows.Forms.Padding(20, 3, 0, 3);
            this.HumidityControl_steps_1_duration_minute_value.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.HumidityControl_steps_1_duration_minute_value.Name = "HumidityControl_steps_1_duration_minute_value";
            this.HumidityControl_steps_1_duration_minute_value.Size = new System.Drawing.Size(35, 23);
            this.HumidityControl_steps_1_duration_minute_value.TabIndex = 8;
            this.HumidityControl_steps_1_duration_minute_value.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // HumidityControl_steps_1_duration_minute_label
            // 
            this.HumidityControl_steps_1_duration_minute_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HumidityControl_steps_1_duration_minute_label.Location = new System.Drawing.Point(590, 0);
            this.HumidityControl_steps_1_duration_minute_label.Margin = new System.Windows.Forms.Padding(0);
            this.HumidityControl_steps_1_duration_minute_label.Name = "HumidityControl_steps_1_duration_minute_label";
            this.HumidityControl_steps_1_duration_minute_label.Size = new System.Drawing.Size(25, 25);
            this.HumidityControl_steps_1_duration_minute_label.TabIndex = 9;
            this.HumidityControl_steps_1_duration_minute_label.Text = "M";
            // 
            // HumidityControl_steps_1_duration_second_value
            // 
            this.HumidityControl_steps_1_duration_second_value.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HumidityControl_steps_1_duration_second_value.Location = new System.Drawing.Point(635, 3);
            this.HumidityControl_steps_1_duration_second_value.Margin = new System.Windows.Forms.Padding(20, 3, 0, 3);
            this.HumidityControl_steps_1_duration_second_value.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.HumidityControl_steps_1_duration_second_value.Name = "HumidityControl_steps_1_duration_second_value";
            this.HumidityControl_steps_1_duration_second_value.Size = new System.Drawing.Size(35, 23);
            this.HumidityControl_steps_1_duration_second_value.TabIndex = 10;
            this.HumidityControl_steps_1_duration_second_value.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // HumidityControl_steps_1_duration_second_label
            // 
            this.HumidityControl_steps_1_duration_second_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HumidityControl_steps_1_duration_second_label.Location = new System.Drawing.Point(670, 0);
            this.HumidityControl_steps_1_duration_second_label.Margin = new System.Windows.Forms.Padding(0);
            this.HumidityControl_steps_1_duration_second_label.Name = "HumidityControl_steps_1_duration_second_label";
            this.HumidityControl_steps_1_duration_second_label.Size = new System.Drawing.Size(25, 25);
            this.HumidityControl_steps_1_duration_second_label.TabIndex = 11;
            this.HumidityControl_steps_1_duration_second_label.Text = "S";
            // 
            // HumidityControl_steps_1_fanSpeed
            // 
            this.HumidityControl_steps_1_fanSpeed.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HumidityControl_steps_1_fanSpeed.Location = new System.Drawing.Point(775, 3);
            this.HumidityControl_steps_1_fanSpeed.Margin = new System.Windows.Forms.Padding(80, 3, 3, 3);
            this.HumidityControl_steps_1_fanSpeed.Maximum = new decimal(new int[] {
            2400,
            0,
            0,
            0});
            this.HumidityControl_steps_1_fanSpeed.Name = "HumidityControl_steps_1_fanSpeed";
            this.HumidityControl_steps_1_fanSpeed.Size = new System.Drawing.Size(50, 23);
            this.HumidityControl_steps_1_fanSpeed.TabIndex = 12;
            this.HumidityControl_steps_1_fanSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // HumidityControl_steps_1_status
            // 
            this.HumidityControl_steps_1_status.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.HumidityControl_steps_1_status.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HumidityControl_steps_1_status.FormattingEnabled = true;
            this.HumidityControl_steps_1_status.Items.AddRange(new object[] {
            "Completed",
            "Waiting"});
            this.HumidityControl_steps_1_status.Location = new System.Drawing.Point(883, 3);
            this.HumidityControl_steps_1_status.Margin = new System.Windows.Forms.Padding(55, 3, 3, 3);
            this.HumidityControl_steps_1_status.Name = "HumidityControl_steps_1_status";
            this.HumidityControl_steps_1_status.Size = new System.Drawing.Size(90, 24);
            this.HumidityControl_steps_1_status.TabIndex = 13;
            // 
            // HumidityControl_steps_1_delete
            // 
            this.HumidityControl_steps_1_delete.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HumidityControl_steps_1_delete.Location = new System.Drawing.Point(1011, 3);
            this.HumidityControl_steps_1_delete.Margin = new System.Windows.Forms.Padding(35, 3, 3, 3);
            this.HumidityControl_steps_1_delete.Name = "HumidityControl_steps_1_delete";
            this.HumidityControl_steps_1_delete.Size = new System.Drawing.Size(59, 27);
            this.HumidityControl_steps_1_delete.TabIndex = 14;
            this.HumidityControl_steps_1_delete.Text = "Delete";
            this.HumidityControl_steps_1_delete.UseVisualStyleBackColor = true;
            // 
            // HumidityControl_heading
            // 
            this.HumidityControl_heading.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.HumidityControl_heading.AutoSize = true;
            this.HumidityControl_heading.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.HumidityControl_heading.Controls.Add(this.HumidityControl_heading_order);
            this.HumidityControl_heading.Controls.Add(this.HumidityControl_heading_type);
            this.HumidityControl_heading.Controls.Add(this.HumidityControl_heading_humidity);
            this.HumidityControl_heading.Controls.Add(this.HumidityControl_heading_duration);
            this.HumidityControl_heading.Controls.Add(this.HumidityControl_heading_fanSpeed);
            this.HumidityControl_heading.Controls.Add(this.HumidityControl_heading_status);
            this.HumidityControl_heading.Location = new System.Drawing.Point(30, 60);
            this.HumidityControl_heading.Margin = new System.Windows.Forms.Padding(0);
            this.HumidityControl_heading.Name = "HumidityControl_heading";
            this.HumidityControl_heading.Size = new System.Drawing.Size(1100, 20);
            this.HumidityControl_heading.TabIndex = 7;
            // 
            // HumidityControl_heading_order
            // 
            this.HumidityControl_heading_order.AutoSize = true;
            this.HumidityControl_heading_order.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HumidityControl_heading_order.Location = new System.Drawing.Point(3, 0);
            this.HumidityControl_heading_order.Name = "HumidityControl_heading_order";
            this.HumidityControl_heading_order.Size = new System.Drawing.Size(54, 20);
            this.HumidityControl_heading_order.TabIndex = 0;
            this.HumidityControl_heading_order.Text = "Order";
            // 
            // HumidityControl_heading_type
            // 
            this.HumidityControl_heading_type.AutoSize = true;
            this.HumidityControl_heading_type.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HumidityControl_heading_type.Location = new System.Drawing.Point(90, 0);
            this.HumidityControl_heading_type.Margin = new System.Windows.Forms.Padding(30, 0, 3, 0);
            this.HumidityControl_heading_type.Name = "HumidityControl_heading_type";
            this.HumidityControl_heading_type.Size = new System.Drawing.Size(47, 20);
            this.HumidityControl_heading_type.TabIndex = 1;
            this.HumidityControl_heading_type.Text = "Type";
            // 
            // HumidityControl_heading_humidity
            // 
            this.HumidityControl_heading_humidity.AutoSize = true;
            this.HumidityControl_heading_humidity.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HumidityControl_heading_humidity.Location = new System.Drawing.Point(160, 0);
            this.HumidityControl_heading_humidity.Margin = new System.Windows.Forms.Padding(20, 0, 3, 0);
            this.HumidityControl_heading_humidity.Name = "HumidityControl_heading_humidity";
            this.HumidityControl_heading_humidity.Size = new System.Drawing.Size(164, 20);
            this.HumidityControl_heading_humidity.TabIndex = 2;
            this.HumidityControl_heading_humidity.Text = "Target humidity (%)";
            // 
            // HumidityControl_heading_duration
            // 
            this.HumidityControl_heading_duration.AutoSize = true;
            this.HumidityControl_heading_duration.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HumidityControl_heading_duration.Location = new System.Drawing.Point(347, 0);
            this.HumidityControl_heading_duration.Margin = new System.Windows.Forms.Padding(20, 0, 3, 0);
            this.HumidityControl_heading_duration.Name = "HumidityControl_heading_duration";
            this.HumidityControl_heading_duration.Size = new System.Drawing.Size(364, 20);
            this.HumidityControl_heading_duration.TabIndex = 3;
            this.HumidityControl_heading_duration.Text = "Duration (Days | Hours | Minutes | Seconds)";
            // 
            // HumidityControl_heading_fanSpeed
            // 
            this.HumidityControl_heading_fanSpeed.AutoSize = true;
            this.HumidityControl_heading_fanSpeed.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HumidityControl_heading_fanSpeed.Location = new System.Drawing.Point(734, 0);
            this.HumidityControl_heading_fanSpeed.Margin = new System.Windows.Forms.Padding(20, 0, 3, 0);
            this.HumidityControl_heading_fanSpeed.Name = "HumidityControl_heading_fanSpeed";
            this.HumidityControl_heading_fanSpeed.Size = new System.Drawing.Size(141, 20);
            this.HumidityControl_heading_fanSpeed.TabIndex = 4;
            this.HumidityControl_heading_fanSpeed.Text = "Fan speed (rpm)";
            // 
            // HumidityControl_heading_status
            // 
            this.HumidityControl_heading_status.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HumidityControl_heading_status.Location = new System.Drawing.Point(908, 0);
            this.HumidityControl_heading_status.Margin = new System.Windows.Forms.Padding(30, 0, 130, 0);
            this.HumidityControl_heading_status.Name = "HumidityControl_heading_status";
            this.HumidityControl_heading_status.Size = new System.Drawing.Size(62, 20);
            this.HumidityControl_heading_status.TabIndex = 5;
            this.HumidityControl_heading_status.Text = "Status";
            // 
            // HumidityControl
            // 
            this.HumidityControl.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.HumidityControl.AutoSize = true;
            this.HumidityControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.HumidityControl.Controls.Add(this.HumidityControl_addStep);
            this.HumidityControl.Controls.Add(this.HumidityControl_start);
            this.HumidityControl.Controls.Add(this.HumidityControl_pause);
            this.HumidityControl.Location = new System.Drawing.Point(423, 25);
            this.HumidityControl.Margin = new System.Windows.Forms.Padding(0);
            this.HumidityControl.Name = "HumidityControl";
            this.HumidityControl.Padding = new System.Windows.Forms.Padding(20, 0, 20, 0);
            this.HumidityControl.Size = new System.Drawing.Size(314, 27);
            this.HumidityControl.TabIndex = 6;
            // 
            // HumidityControl_addStep
            // 
            this.HumidityControl_addStep.AutoSize = true;
            this.HumidityControl_addStep.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.HumidityControl_addStep.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HumidityControl_addStep.Location = new System.Drawing.Point(20, 0);
            this.HumidityControl_addStep.Margin = new System.Windows.Forms.Padding(0, 0, 50, 0);
            this.HumidityControl_addStep.Name = "HumidityControl_addStep";
            this.HumidityControl_addStep.Size = new System.Drawing.Size(86, 27);
            this.HumidityControl_addStep.TabIndex = 9;
            this.HumidityControl_addStep.Text = "+ Add step";
            this.HumidityControl_addStep.UseVisualStyleBackColor = true;
            this.HumidityControl_addStep.Click += new System.EventHandler(this.HumidityControl_addStep_Click);
            // 
            // HumidityControl_start
            // 
            this.HumidityControl_start.AutoSize = true;
            this.HumidityControl_start.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.HumidityControl_start.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HumidityControl_start.Location = new System.Drawing.Point(156, 0);
            this.HumidityControl_start.Margin = new System.Windows.Forms.Padding(0);
            this.HumidityControl_start.Name = "HumidityControl_start";
            this.HumidityControl_start.Size = new System.Drawing.Size(58, 27);
            this.HumidityControl_start.TabIndex = 1;
            this.HumidityControl_start.Text = "► Start";
            this.HumidityControl_start.UseVisualStyleBackColor = true;
            this.HumidityControl_start.Click += new System.EventHandler(this.HumidityControl_start_Click);
            // 
            // HumidityControl_pause
            // 
            this.HumidityControl_pause.AutoSize = true;
            this.HumidityControl_pause.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.HumidityControl_pause.Enabled = false;
            this.HumidityControl_pause.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HumidityControl_pause.Location = new System.Drawing.Point(214, 0);
            this.HumidityControl_pause.Margin = new System.Windows.Forms.Padding(0);
            this.HumidityControl_pause.Name = "HumidityControl_pause";
            this.HumidityControl_pause.Size = new System.Drawing.Size(80, 27);
            this.HumidityControl_pause.TabIndex = 2;
            this.HumidityControl_pause.Text = "▐▐ Pause";
            this.HumidityControl_pause.UseVisualStyleBackColor = true;
            this.HumidityControl_pause.Visible = false;
            this.HumidityControl_pause.Click += new System.EventHandler(this.HumidityControl_pause_Click);
            // 
            // Body
            // 
            this.Body.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Body.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Body.Location = new System.Drawing.Point(0, 27);
            this.Body.Margin = new System.Windows.Forms.Padding(0);
            this.Body.Name = "Body";
            this.Body.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // Body.Panel1
            // 
            this.Body.Panel1.Controls.Add(this.tableLayoutPanel1);
            // 
            // Body.Panel2
            // 
            this.Body.Panel2.Controls.Add(this.Body_panel_btm);
            this.Body.Size = new System.Drawing.Size(1164, 835);
            this.Body.SplitterDistance = 585;
            this.Body.SplitterWidth = 8;
            this.Body.TabIndex = 1;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoScroll = true;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.Body_panel_top, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1160, 581);
            this.tableLayoutPanel1.TabIndex = 6;
            // 
            // Body_panel_top
            // 
            this.Body_panel_top.AutoScroll = true;
            this.Body_panel_top.ColumnCount = 1;
            this.Body_panel_top.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.Body_panel_top.Controls.Add(this.Chamber_flow, 0, 0);
            this.Body_panel_top.Controls.Add(this.Advanced_button, 0, 5);
            this.Body_panel_top.Controls.Add(this.Advanced_options, 0, 6);
            this.Body_panel_top.Controls.Add(this.Readings, 0, 2);
            this.Body_panel_top.Controls.Add(this.Data, 0, 3);
            this.Body_panel_top.Controls.Add(this.Other, 0, 4);
            this.Body_panel_top.Controls.Add(this.Displaychart, 0, 1);
            this.Body_panel_top.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Body_panel_top.Location = new System.Drawing.Point(0, 0);
            this.Body_panel_top.Margin = new System.Windows.Forms.Padding(0);
            this.Body_panel_top.MinimumSize = new System.Drawing.Size(0, 446);
            this.Body_panel_top.Name = "Body_panel_top";
            this.Body_panel_top.RowCount = 7;
            this.Body_panel_top.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.Body_panel_top.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.Body_panel_top.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.Body_panel_top.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.Body_panel_top.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.Body_panel_top.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.Body_panel_top.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.Body_panel_top.Size = new System.Drawing.Size(1160, 581);
            this.Body_panel_top.TabIndex = 5;
            // 
            // Chamber_flow
            // 
            this.Chamber_flow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.Chamber_flow.AutoSize = true;
            this.Chamber_flow.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Chamber_flow.Controls.Add(this.Chamber_prev);
            this.Chamber_flow.Controls.Add(this.Chamber_title);
            this.Chamber_flow.Controls.Add(this.Chamber_next);
            this.Chamber_flow.Location = new System.Drawing.Point(472, 0);
            this.Chamber_flow.Margin = new System.Windows.Forms.Padding(0);
            this.Chamber_flow.Name = "Chamber_flow";
            this.Chamber_flow.Size = new System.Drawing.Size(216, 30);
            this.Chamber_flow.TabIndex = 1;
            // 
            // Chamber_prev
            // 
            this.Chamber_prev.AutoSize = true;
            this.Chamber_prev.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Chamber_prev.Location = new System.Drawing.Point(3, 3);
            this.Chamber_prev.Name = "Chamber_prev";
            this.Chamber_prev.Size = new System.Drawing.Size(29, 23);
            this.Chamber_prev.TabIndex = 0;
            this.Chamber_prev.Text = "<<";
            this.Chamber_prev.UseVisualStyleBackColor = true;
            this.Chamber_prev.Click += new System.EventHandler(this.Chamber_prev_Click);
            // 
            // Chamber_title
            // 
            this.Chamber_title.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Chamber_title.AutoSize = true;
            this.Chamber_title.Font = new System.Drawing.Font("Arial", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Chamber_title.Location = new System.Drawing.Point(40, 0);
            this.Chamber_title.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.Chamber_title.Name = "Chamber_title";
            this.Chamber_title.Size = new System.Drawing.Size(136, 29);
            this.Chamber_title.TabIndex = 1;
            this.Chamber_title.Text = "Chamber 1";
            // 
            // Chamber_next
            // 
            this.Chamber_next.AutoSize = true;
            this.Chamber_next.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Chamber_next.Location = new System.Drawing.Point(184, 3);
            this.Chamber_next.Name = "Chamber_next";
            this.Chamber_next.Size = new System.Drawing.Size(29, 23);
            this.Chamber_next.TabIndex = 2;
            this.Chamber_next.Text = ">>";
            this.Chamber_next.UseVisualStyleBackColor = true;
            this.Chamber_next.Click += new System.EventHandler(this.Chamber_next_Click);
            // 
            // Advanced_button
            // 
            this.Advanced_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.Advanced_button.AutoSize = true;
            this.Advanced_button.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Advanced_button.Controls.Add(this.Advanced_button_show);
            this.Advanced_button.Controls.Add(this.Advanced_button_hide);
            this.Advanced_button.Cursor = System.Windows.Forms.Cursors.Default;
            this.Advanced_button.Location = new System.Drawing.Point(424, 494);
            this.Advanced_button.Margin = new System.Windows.Forms.Padding(0);
            this.Advanced_button.Name = "Advanced_button";
            this.Advanced_button.Size = new System.Drawing.Size(312, 29);
            this.Advanced_button.TabIndex = 1;
            // 
            // Advanced_button_show
            // 
            this.Advanced_button_show.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.Advanced_button_show.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Advanced_button_show.Location = new System.Drawing.Point(3, 3);
            this.Advanced_button_show.Name = "Advanced_button_show";
            this.Advanced_button_show.Size = new System.Drawing.Size(150, 23);
            this.Advanced_button_show.TabIndex = 0;
            this.Advanced_button_show.Text = "Show advanced options";
            this.Advanced_button_show.UseVisualStyleBackColor = true;
            this.Advanced_button_show.Click += new System.EventHandler(this.Advanced_button_show_Click);
            // 
            // Advanced_button_hide
            // 
            this.Advanced_button_hide.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.Advanced_button_hide.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Advanced_button_hide.Location = new System.Drawing.Point(159, 3);
            this.Advanced_button_hide.Name = "Advanced_button_hide";
            this.Advanced_button_hide.Size = new System.Drawing.Size(150, 23);
            this.Advanced_button_hide.TabIndex = 1;
            this.Advanced_button_hide.Text = "Hide advanced options";
            this.Advanced_button_hide.UseVisualStyleBackColor = true;
            this.Advanced_button_hide.Visible = false;
            this.Advanced_button_hide.Click += new System.EventHandler(this.Advanced_button_hide_Click);
            // 
            // Advanced_options
            // 
            this.Advanced_options.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.Advanced_options.Controls.Add(this.Advanced_humidity);
            this.Advanced_options.Controls.Add(this.Advanced_weigh);
            this.Advanced_options.Location = new System.Drawing.Point(65, 523);
            this.Advanced_options.Margin = new System.Windows.Forms.Padding(0);
            this.Advanced_options.Name = "Advanced_options";
            this.Advanced_options.Size = new System.Drawing.Size(1030, 58);
            this.Advanced_options.TabIndex = 7;
            this.Advanced_options.Visible = false;
            // 
            // Advanced_humidity
            // 
            this.Advanced_humidity.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.Advanced_humidity.AutoSize = true;
            this.Advanced_humidity.Controls.Add(this.Advanced_humidity_flow);
            this.Advanced_humidity.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Advanced_humidity.Location = new System.Drawing.Point(0, 0);
            this.Advanced_humidity.Margin = new System.Windows.Forms.Padding(0);
            this.Advanced_humidity.Name = "Advanced_humidity";
            this.Advanced_humidity.Padding = new System.Windows.Forms.Padding(10, 3, 10, 3);
            this.Advanced_humidity.Size = new System.Drawing.Size(616, 83);
            this.Advanced_humidity.TabIndex = 1;
            this.Advanced_humidity.TabStop = false;
            this.Advanced_humidity.Text = "Humidity sensor calibration";
            // 
            // Advanced_humidity_flow
            // 
            this.Advanced_humidity_flow.AutoSize = true;
            this.Advanced_humidity_flow.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Advanced_humidity_flow.Controls.Add(this.Advanced_humidity_RH1_raw_label);
            this.Advanced_humidity_flow.Controls.Add(this.Advanced_humidity_RH1_raw_value);
            this.Advanced_humidity_flow.Controls.Add(this.Advanced_humidity_RH1_std_label);
            this.Advanced_humidity_flow.Controls.Add(this.Advanced_humidity_RH1_std_value);
            this.Advanced_humidity_flow.Controls.Add(this.Advanced_humidity_RH1_std_apply);
            this.Advanced_humidity_flow.Controls.Add(this.Advanced_humidity_RH2_raw_label);
            this.Advanced_humidity_flow.Controls.Add(this.Advanced_humidity_RH2_raw_value);
            this.Advanced_humidity_flow.Controls.Add(this.Advanced_humidity_RH2_std_label);
            this.Advanced_humidity_flow.Controls.Add(this.Advanced_humidity_RH2_std_value);
            this.Advanced_humidity_flow.Controls.Add(this.Advanced_humidity_RH2_std_apply);
            this.Advanced_humidity_flow.Location = new System.Drawing.Point(10, 25);
            this.Advanced_humidity_flow.Name = "Advanced_humidity_flow";
            this.Advanced_humidity_flow.Size = new System.Drawing.Size(593, 33);
            this.Advanced_humidity_flow.TabIndex = 0;
            // 
            // Advanced_humidity_RH1_raw_label
            // 
            this.Advanced_humidity_RH1_raw_label.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Advanced_humidity_RH1_raw_label.AutoSize = true;
            this.Advanced_humidity_RH1_raw_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Advanced_humidity_RH1_raw_label.Location = new System.Drawing.Point(5, 8);
            this.Advanced_humidity_RH1_raw_label.Margin = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.Advanced_humidity_RH1_raw_label.Name = "Advanced_humidity_RH1_raw_label";
            this.Advanced_humidity_RH1_raw_label.Size = new System.Drawing.Size(51, 17);
            this.Advanced_humidity_RH1_raw_label.TabIndex = 1;
            this.Advanced_humidity_RH1_raw_label.Text = "Raw 1:";
            this.Advanced_humidity_RH1_raw_label.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Advanced_humidity_RH1_raw_value
            // 
            this.Advanced_humidity_RH1_raw_value.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Advanced_humidity_RH1_raw_value.DecimalPlaces = 1;
            this.Advanced_humidity_RH1_raw_value.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Advanced_humidity_RH1_raw_value.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.Advanced_humidity_RH1_raw_value.Location = new System.Drawing.Point(56, 3);
            this.Advanced_humidity_RH1_raw_value.Margin = new System.Windows.Forms.Padding(0);
            this.Advanced_humidity_RH1_raw_value.Name = "Advanced_humidity_RH1_raw_value";
            this.Advanced_humidity_RH1_raw_value.Size = new System.Drawing.Size(60, 26);
            this.Advanced_humidity_RH1_raw_value.TabIndex = 2;
            this.Advanced_humidity_RH1_raw_value.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.Advanced_humidity_RH1_raw_value.Value = new decimal(new int[] {
            336,
            0,
            0,
            65536});
            // 
            // Advanced_humidity_RH1_std_label
            // 
            this.Advanced_humidity_RH1_std_label.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Advanced_humidity_RH1_std_label.AutoSize = true;
            this.Advanced_humidity_RH1_std_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Advanced_humidity_RH1_std_label.Location = new System.Drawing.Point(121, 8);
            this.Advanced_humidity_RH1_std_label.Margin = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.Advanced_humidity_RH1_std_label.Name = "Advanced_humidity_RH1_std_label";
            this.Advanced_humidity_RH1_std_label.Size = new System.Drawing.Size(49, 17);
            this.Advanced_humidity_RH1_std_label.TabIndex = 3;
            this.Advanced_humidity_RH1_std_label.Text = "Std. 1:";
            this.Advanced_humidity_RH1_std_label.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Advanced_humidity_RH1_std_value
            // 
            this.Advanced_humidity_RH1_std_value.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Advanced_humidity_RH1_std_value.DecimalPlaces = 1;
            this.Advanced_humidity_RH1_std_value.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Advanced_humidity_RH1_std_value.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.Advanced_humidity_RH1_std_value.Location = new System.Drawing.Point(170, 3);
            this.Advanced_humidity_RH1_std_value.Margin = new System.Windows.Forms.Padding(0);
            this.Advanced_humidity_RH1_std_value.Name = "Advanced_humidity_RH1_std_value";
            this.Advanced_humidity_RH1_std_value.Size = new System.Drawing.Size(60, 26);
            this.Advanced_humidity_RH1_std_value.TabIndex = 4;
            this.Advanced_humidity_RH1_std_value.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.Advanced_humidity_RH1_std_value.Value = new decimal(new int[] {
            336,
            0,
            0,
            65536});
            // 
            // Advanced_humidity_RH1_std_apply
            // 
            this.Advanced_humidity_RH1_std_apply.AutoSize = true;
            this.Advanced_humidity_RH1_std_apply.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Advanced_humidity_RH1_std_apply.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Advanced_humidity_RH1_std_apply.Location = new System.Drawing.Point(233, 3);
            this.Advanced_humidity_RH1_std_apply.Name = "Advanced_humidity_RH1_std_apply";
            this.Advanced_humidity_RH1_std_apply.Size = new System.Drawing.Size(53, 27);
            this.Advanced_humidity_RH1_std_apply.TabIndex = 5;
            this.Advanced_humidity_RH1_std_apply.Text = "Apply";
            this.Advanced_humidity_RH1_std_apply.UseVisualStyleBackColor = true;
            this.Advanced_humidity_RH1_std_apply.Click += new System.EventHandler(this.Advanced_humidity_RH1_apply_Click);
            // 
            // Advanced_humidity_RH2_raw_label
            // 
            this.Advanced_humidity_RH2_raw_label.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Advanced_humidity_RH2_raw_label.AutoSize = true;
            this.Advanced_humidity_RH2_raw_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Advanced_humidity_RH2_raw_label.Location = new System.Drawing.Point(309, 8);
            this.Advanced_humidity_RH2_raw_label.Margin = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.Advanced_humidity_RH2_raw_label.Name = "Advanced_humidity_RH2_raw_label";
            this.Advanced_humidity_RH2_raw_label.Size = new System.Drawing.Size(51, 17);
            this.Advanced_humidity_RH2_raw_label.TabIndex = 6;
            this.Advanced_humidity_RH2_raw_label.Text = "Raw 2:";
            this.Advanced_humidity_RH2_raw_label.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Advanced_humidity_RH2_raw_value
            // 
            this.Advanced_humidity_RH2_raw_value.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Advanced_humidity_RH2_raw_value.DecimalPlaces = 1;
            this.Advanced_humidity_RH2_raw_value.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Advanced_humidity_RH2_raw_value.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.Advanced_humidity_RH2_raw_value.Location = new System.Drawing.Point(360, 3);
            this.Advanced_humidity_RH2_raw_value.Margin = new System.Windows.Forms.Padding(0);
            this.Advanced_humidity_RH2_raw_value.Name = "Advanced_humidity_RH2_raw_value";
            this.Advanced_humidity_RH2_raw_value.Size = new System.Drawing.Size(60, 26);
            this.Advanced_humidity_RH2_raw_value.TabIndex = 7;
            this.Advanced_humidity_RH2_raw_value.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.Advanced_humidity_RH2_raw_value.Value = new decimal(new int[] {
            756,
            0,
            0,
            65536});
            // 
            // Advanced_humidity_RH2_std_label
            // 
            this.Advanced_humidity_RH2_std_label.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Advanced_humidity_RH2_std_label.AutoSize = true;
            this.Advanced_humidity_RH2_std_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Advanced_humidity_RH2_std_label.Location = new System.Drawing.Point(425, 8);
            this.Advanced_humidity_RH2_std_label.Margin = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.Advanced_humidity_RH2_std_label.Name = "Advanced_humidity_RH2_std_label";
            this.Advanced_humidity_RH2_std_label.Size = new System.Drawing.Size(49, 17);
            this.Advanced_humidity_RH2_std_label.TabIndex = 8;
            this.Advanced_humidity_RH2_std_label.Text = "Std. 2:";
            this.Advanced_humidity_RH2_std_label.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Advanced_humidity_RH2_std_value
            // 
            this.Advanced_humidity_RH2_std_value.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Advanced_humidity_RH2_std_value.DecimalPlaces = 1;
            this.Advanced_humidity_RH2_std_value.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Advanced_humidity_RH2_std_value.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.Advanced_humidity_RH2_std_value.Location = new System.Drawing.Point(474, 3);
            this.Advanced_humidity_RH2_std_value.Margin = new System.Windows.Forms.Padding(0);
            this.Advanced_humidity_RH2_std_value.Name = "Advanced_humidity_RH2_std_value";
            this.Advanced_humidity_RH2_std_value.Size = new System.Drawing.Size(60, 26);
            this.Advanced_humidity_RH2_std_value.TabIndex = 9;
            this.Advanced_humidity_RH2_std_value.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.Advanced_humidity_RH2_std_value.Value = new decimal(new int[] {
            756,
            0,
            0,
            65536});
            // 
            // Advanced_humidity_RH2_std_apply
            // 
            this.Advanced_humidity_RH2_std_apply.AutoSize = true;
            this.Advanced_humidity_RH2_std_apply.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Advanced_humidity_RH2_std_apply.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Advanced_humidity_RH2_std_apply.Location = new System.Drawing.Point(537, 3);
            this.Advanced_humidity_RH2_std_apply.Name = "Advanced_humidity_RH2_std_apply";
            this.Advanced_humidity_RH2_std_apply.Size = new System.Drawing.Size(53, 27);
            this.Advanced_humidity_RH2_std_apply.TabIndex = 10;
            this.Advanced_humidity_RH2_std_apply.Text = "Apply";
            this.Advanced_humidity_RH2_std_apply.UseVisualStyleBackColor = true;
            this.Advanced_humidity_RH2_std_apply.Click += new System.EventHandler(this.Advanced_humidity_RH2_apply_Click);
            // 
            // Advanced_weigh
            // 
            this.Advanced_weigh.AutoSize = true;
            this.Advanced_weigh.Controls.Add(this.Advanced_weigh_flow);
            this.Advanced_weigh.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Advanced_weigh.Location = new System.Drawing.Point(631, 0);
            this.Advanced_weigh.Margin = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.Advanced_weigh.Name = "Advanced_weigh";
            this.Advanced_weigh.Padding = new System.Windows.Forms.Padding(10, 3, 10, 3);
            this.Advanced_weigh.Size = new System.Drawing.Size(395, 72);
            this.Advanced_weigh.TabIndex = 2;
            this.Advanced_weigh.TabStop = false;
            this.Advanced_weigh.Text = "Weighing scale settings";
            // 
            // Advanced_weigh_flow
            // 
            this.Advanced_weigh_flow.AutoSize = true;
            this.Advanced_weigh_flow.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Advanced_weigh_flow.Controls.Add(this.Advanced_weigh_factor_label);
            this.Advanced_weigh_flow.Controls.Add(this.Advanced_weigh_factor_value);
            this.Advanced_weigh_flow.Controls.Add(this.Advanced_weigh_offset_label);
            this.Advanced_weigh_flow.Controls.Add(this.Advanced_weigh_offset_value);
            this.Advanced_weigh_flow.Location = new System.Drawing.Point(10, 21);
            this.Advanced_weigh_flow.Name = "Advanced_weigh_flow";
            this.Advanced_weigh_flow.Size = new System.Drawing.Size(372, 26);
            this.Advanced_weigh_flow.TabIndex = 0;
            // 
            // Advanced_weigh_factor_label
            // 
            this.Advanced_weigh_factor_label.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Advanced_weigh_factor_label.AutoSize = true;
            this.Advanced_weigh_factor_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Advanced_weigh_factor_label.Location = new System.Drawing.Point(0, 4);
            this.Advanced_weigh_factor_label.Margin = new System.Windows.Forms.Padding(0);
            this.Advanced_weigh_factor_label.Name = "Advanced_weigh_factor_label";
            this.Advanced_weigh_factor_label.Size = new System.Drawing.Size(87, 17);
            this.Advanced_weigh_factor_label.TabIndex = 1;
            this.Advanced_weigh_factor_label.Text = "Scale factor:";
            this.Advanced_weigh_factor_label.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Advanced_weigh_factor_value
            // 
            this.Advanced_weigh_factor_value.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Advanced_weigh_factor_value.DecimalPlaces = 3;
            this.Advanced_weigh_factor_value.Enabled = false;
            this.Advanced_weigh_factor_value.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Advanced_weigh_factor_value.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.Advanced_weigh_factor_value.Location = new System.Drawing.Point(87, 0);
            this.Advanced_weigh_factor_value.Margin = new System.Windows.Forms.Padding(0);
            this.Advanced_weigh_factor_value.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.Advanced_weigh_factor_value.Minimum = new decimal(new int[] {
            1000000,
            0,
            0,
            -2147483648});
            this.Advanced_weigh_factor_value.Name = "Advanced_weigh_factor_value";
            this.Advanced_weigh_factor_value.ReadOnly = true;
            this.Advanced_weigh_factor_value.Size = new System.Drawing.Size(110, 26);
            this.Advanced_weigh_factor_value.TabIndex = 2;
            this.Advanced_weigh_factor_value.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.Advanced_weigh_factor_value.Value = new decimal(new int[] {
            1410056640,
            2,
            0,
            -2147221504});
            // 
            // Advanced_weigh_offset_label
            // 
            this.Advanced_weigh_offset_label.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Advanced_weigh_offset_label.AutoSize = true;
            this.Advanced_weigh_offset_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Advanced_weigh_offset_label.Location = new System.Drawing.Point(212, 4);
            this.Advanced_weigh_offset_label.Margin = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.Advanced_weigh_offset_label.Name = "Advanced_weigh_offset_label";
            this.Advanced_weigh_offset_label.Size = new System.Drawing.Size(50, 17);
            this.Advanced_weigh_offset_label.TabIndex = 3;
            this.Advanced_weigh_offset_label.Text = "Offset:";
            this.Advanced_weigh_offset_label.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Advanced_weigh_offset_value
            // 
            this.Advanced_weigh_offset_value.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Advanced_weigh_offset_value.Enabled = false;
            this.Advanced_weigh_offset_value.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Advanced_weigh_offset_value.Location = new System.Drawing.Point(262, 0);
            this.Advanced_weigh_offset_value.Margin = new System.Windows.Forms.Padding(0);
            this.Advanced_weigh_offset_value.Maximum = new decimal(new int[] {
            -2147483648,
            0,
            0,
            0});
            this.Advanced_weigh_offset_value.Minimum = new decimal(new int[] {
            -2147483648,
            0,
            0,
            -2147483648});
            this.Advanced_weigh_offset_value.Name = "Advanced_weigh_offset_value";
            this.Advanced_weigh_offset_value.ReadOnly = true;
            this.Advanced_weigh_offset_value.Size = new System.Drawing.Size(110, 26);
            this.Advanced_weigh_offset_value.TabIndex = 4;
            this.Advanced_weigh_offset_value.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.Advanced_weigh_offset_value.Value = new decimal(new int[] {
            -2147483648,
            0,
            0,
            0});
            // 
            // Readings
            // 
            this.Readings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.Readings.AutoSize = true;
            this.Readings.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Readings.Controls.Add(this.Readings_humidity);
            this.Readings.Controls.Add(this.Readings_temperature);
            this.Readings.Controls.Add(this.Readings_weight);
            this.Readings.Controls.Add(this.Readings_velocity);
            this.Readings.Controls.Add(this.Readings_fan1Speed);
            this.Readings.Controls.Add(this.Readings_fan2Speed);
            this.Readings.Location = new System.Drawing.Point(122, 284);
            this.Readings.Margin = new System.Windows.Forms.Padding(0);
            this.Readings.Name = "Readings";
            this.Readings.Size = new System.Drawing.Size(915, 70);
            this.Readings.TabIndex = 2;
            // 
            // Readings_humidity
            // 
            this.Readings_humidity.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Readings_humidity.Controls.Add(this.Readings_humidity_flow);
            this.Readings_humidity.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Readings_humidity.Location = new System.Drawing.Point(10, 3);
            this.Readings_humidity.Margin = new System.Windows.Forms.Padding(10, 3, 10, 3);
            this.Readings_humidity.Name = "Readings_humidity";
            this.Readings_humidity.Padding = new System.Windows.Forms.Padding(10, 3, 10, 3);
            this.Readings_humidity.Size = new System.Drawing.Size(133, 60);
            this.Readings_humidity.TabIndex = 1;
            this.Readings_humidity.TabStop = false;
            this.Readings_humidity.Text = "Humidity";
            // 
            // Readings_humidity_flow
            // 
            this.Readings_humidity_flow.AutoSize = true;
            this.Readings_humidity_flow.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Readings_humidity_flow.Controls.Add(this.Readings_humidity_flow_value);
            this.Readings_humidity_flow.Controls.Add(this.Readings_humidity_flow_label);
            this.Readings_humidity_flow.Location = new System.Drawing.Point(10, 22);
            this.Readings_humidity_flow.Name = "Readings_humidity_flow";
            this.Readings_humidity_flow.Size = new System.Drawing.Size(110, 32);
            this.Readings_humidity_flow.TabIndex = 0;
            // 
            // Readings_humidity_flow_value
            // 
            this.Readings_humidity_flow_value.Enabled = false;
            this.Readings_humidity_flow_value.Location = new System.Drawing.Point(3, 3);
            this.Readings_humidity_flow_value.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.Readings_humidity_flow_value.Name = "Readings_humidity_flow_value";
            this.Readings_humidity_flow_value.Size = new System.Drawing.Size(80, 26);
            this.Readings_humidity_flow_value.TabIndex = 0;
            this.Readings_humidity_flow_value.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // Readings_humidity_flow_label
            // 
            this.Readings_humidity_flow_label.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Readings_humidity_flow_label.AutoSize = true;
            this.Readings_humidity_flow_label.Location = new System.Drawing.Point(83, 6);
            this.Readings_humidity_flow_label.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.Readings_humidity_flow_label.Name = "Readings_humidity_flow_label";
            this.Readings_humidity_flow_label.Size = new System.Drawing.Size(24, 20);
            this.Readings_humidity_flow_label.TabIndex = 1;
            this.Readings_humidity_flow_label.Text = "%";
            // 
            // Readings_temperature
            // 
            this.Readings_temperature.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Readings_temperature.Controls.Add(this.Readings_temperature_flow);
            this.Readings_temperature.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Readings_temperature.Location = new System.Drawing.Point(163, 3);
            this.Readings_temperature.Margin = new System.Windows.Forms.Padding(10, 3, 10, 3);
            this.Readings_temperature.Name = "Readings_temperature";
            this.Readings_temperature.Padding = new System.Windows.Forms.Padding(10, 3, 10, 3);
            this.Readings_temperature.Size = new System.Drawing.Size(136, 60);
            this.Readings_temperature.TabIndex = 2;
            this.Readings_temperature.TabStop = false;
            this.Readings_temperature.Text = "Temperature";
            // 
            // Readings_temperature_flow
            // 
            this.Readings_temperature_flow.AutoSize = true;
            this.Readings_temperature_flow.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Readings_temperature_flow.Controls.Add(this.Readings_temperature_flow_value);
            this.Readings_temperature_flow.Controls.Add(this.Readings_temperature_flow_label);
            this.Readings_temperature_flow.Location = new System.Drawing.Point(10, 22);
            this.Readings_temperature_flow.Name = "Readings_temperature_flow";
            this.Readings_temperature_flow.Size = new System.Drawing.Size(113, 32);
            this.Readings_temperature_flow.TabIndex = 0;
            // 
            // Readings_temperature_flow_value
            // 
            this.Readings_temperature_flow_value.Enabled = false;
            this.Readings_temperature_flow_value.Location = new System.Drawing.Point(3, 3);
            this.Readings_temperature_flow_value.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.Readings_temperature_flow_value.Name = "Readings_temperature_flow_value";
            this.Readings_temperature_flow_value.Size = new System.Drawing.Size(80, 26);
            this.Readings_temperature_flow_value.TabIndex = 0;
            this.Readings_temperature_flow_value.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // Readings_temperature_flow_label
            // 
            this.Readings_temperature_flow_label.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Readings_temperature_flow_label.AutoSize = true;
            this.Readings_temperature_flow_label.Location = new System.Drawing.Point(83, 6);
            this.Readings_temperature_flow_label.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.Readings_temperature_flow_label.Name = "Readings_temperature_flow_label";
            this.Readings_temperature_flow_label.Size = new System.Drawing.Size(27, 20);
            this.Readings_temperature_flow_label.TabIndex = 1;
            this.Readings_temperature_flow_label.Text = "°C";
            // 
            // Readings_weight
            // 
            this.Readings_weight.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Readings_weight.Controls.Add(this.Readings_weight_flow);
            this.Readings_weight.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Readings_weight.Location = new System.Drawing.Point(319, 3);
            this.Readings_weight.Margin = new System.Windows.Forms.Padding(10, 3, 10, 3);
            this.Readings_weight.Name = "Readings_weight";
            this.Readings_weight.Padding = new System.Windows.Forms.Padding(10, 3, 10, 3);
            this.Readings_weight.Size = new System.Drawing.Size(118, 60);
            this.Readings_weight.TabIndex = 3;
            this.Readings_weight.TabStop = false;
            this.Readings_weight.Text = "Weight";
            // 
            // Readings_weight_flow
            // 
            this.Readings_weight_flow.AutoSize = true;
            this.Readings_weight_flow.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Readings_weight_flow.Controls.Add(this.Readings_weight_flow_value);
            this.Readings_weight_flow.Controls.Add(this.Readings_weight_flow_label);
            this.Readings_weight_flow.Location = new System.Drawing.Point(10, 22);
            this.Readings_weight_flow.Name = "Readings_weight_flow";
            this.Readings_weight_flow.Size = new System.Drawing.Size(105, 32);
            this.Readings_weight_flow.TabIndex = 0;
            // 
            // Readings_weight_flow_value
            // 
            this.Readings_weight_flow_value.Enabled = false;
            this.Readings_weight_flow_value.Location = new System.Drawing.Point(3, 3);
            this.Readings_weight_flow_value.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.Readings_weight_flow_value.Name = "Readings_weight_flow_value";
            this.Readings_weight_flow_value.Size = new System.Drawing.Size(80, 26);
            this.Readings_weight_flow_value.TabIndex = 0;
            this.Readings_weight_flow_value.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // Readings_weight_flow_label
            // 
            this.Readings_weight_flow_label.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Readings_weight_flow_label.AutoSize = true;
            this.Readings_weight_flow_label.Location = new System.Drawing.Point(83, 6);
            this.Readings_weight_flow_label.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.Readings_weight_flow_label.Name = "Readings_weight_flow_label";
            this.Readings_weight_flow_label.Size = new System.Drawing.Size(19, 20);
            this.Readings_weight_flow_label.TabIndex = 1;
            this.Readings_weight_flow_label.Text = "g";
            // 
            // Readings_velocity
            // 
            this.Readings_velocity.Controls.Add(this.Readings_velocity_flow);
            this.Readings_velocity.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Readings_velocity.Location = new System.Drawing.Point(457, 3);
            this.Readings_velocity.Margin = new System.Windows.Forms.Padding(10, 3, 10, 3);
            this.Readings_velocity.Name = "Readings_velocity";
            this.Readings_velocity.Padding = new System.Windows.Forms.Padding(10, 3, 10, 3);
            this.Readings_velocity.Size = new System.Drawing.Size(136, 60);
            this.Readings_velocity.TabIndex = 4;
            this.Readings_velocity.TabStop = false;
            this.Readings_velocity.Text = "Air velocity";
            // 
            // Readings_velocity_flow
            // 
            this.Readings_velocity_flow.AutoSize = true;
            this.Readings_velocity_flow.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Readings_velocity_flow.Controls.Add(this.Readings_velocity_flow_value);
            this.Readings_velocity_flow.Controls.Add(this.Readings_velocity_flow_label);
            this.Readings_velocity_flow.Location = new System.Drawing.Point(10, 22);
            this.Readings_velocity_flow.Name = "Readings_velocity_flow";
            this.Readings_velocity_flow.Size = new System.Drawing.Size(123, 32);
            this.Readings_velocity_flow.TabIndex = 0;
            // 
            // Readings_velocity_flow_value
            // 
            this.Readings_velocity_flow_value.Enabled = false;
            this.Readings_velocity_flow_value.Location = new System.Drawing.Point(3, 3);
            this.Readings_velocity_flow_value.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.Readings_velocity_flow_value.Name = "Readings_velocity_flow_value";
            this.Readings_velocity_flow_value.Size = new System.Drawing.Size(80, 26);
            this.Readings_velocity_flow_value.TabIndex = 0;
            this.Readings_velocity_flow_value.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // Readings_velocity_flow_label
            // 
            this.Readings_velocity_flow_label.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Readings_velocity_flow_label.AutoSize = true;
            this.Readings_velocity_flow_label.Location = new System.Drawing.Point(83, 6);
            this.Readings_velocity_flow_label.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.Readings_velocity_flow_label.Name = "Readings_velocity_flow_label";
            this.Readings_velocity_flow_label.Size = new System.Drawing.Size(37, 20);
            this.Readings_velocity_flow_label.TabIndex = 1;
            this.Readings_velocity_flow_label.Text = "m/s";
            // 
            // Readings_fan1Speed
            // 
            this.Readings_fan1Speed.Controls.Add(this.Readings_fan1Speed_flow);
            this.Readings_fan1Speed.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Readings_fan1Speed.Location = new System.Drawing.Point(613, 3);
            this.Readings_fan1Speed.Margin = new System.Windows.Forms.Padding(10, 3, 10, 3);
            this.Readings_fan1Speed.Name = "Readings_fan1Speed";
            this.Readings_fan1Speed.Padding = new System.Windows.Forms.Padding(10, 3, 10, 3);
            this.Readings_fan1Speed.Size = new System.Drawing.Size(136, 60);
            this.Readings_fan1Speed.TabIndex = 5;
            this.Readings_fan1Speed.TabStop = false;
            this.Readings_fan1Speed.Text = "Fan 1 speed";
            // 
            // Readings_fan1Speed_flow
            // 
            this.Readings_fan1Speed_flow.AutoSize = true;
            this.Readings_fan1Speed_flow.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Readings_fan1Speed_flow.Controls.Add(this.Readings_fan1Speed_flow_value);
            this.Readings_fan1Speed_flow.Controls.Add(this.Readings_fan1Speed_flow_label);
            this.Readings_fan1Speed_flow.Location = new System.Drawing.Point(10, 22);
            this.Readings_fan1Speed_flow.Name = "Readings_fan1Speed_flow";
            this.Readings_fan1Speed_flow.Size = new System.Drawing.Size(125, 32);
            this.Readings_fan1Speed_flow.TabIndex = 0;
            // 
            // Readings_fan1Speed_flow_value
            // 
            this.Readings_fan1Speed_flow_value.Enabled = false;
            this.Readings_fan1Speed_flow_value.Location = new System.Drawing.Point(3, 3);
            this.Readings_fan1Speed_flow_value.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.Readings_fan1Speed_flow_value.Name = "Readings_fan1Speed_flow_value";
            this.Readings_fan1Speed_flow_value.Size = new System.Drawing.Size(80, 26);
            this.Readings_fan1Speed_flow_value.TabIndex = 0;
            this.Readings_fan1Speed_flow_value.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // Readings_fan1Speed_flow_label
            // 
            this.Readings_fan1Speed_flow_label.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Readings_fan1Speed_flow_label.AutoSize = true;
            this.Readings_fan1Speed_flow_label.Location = new System.Drawing.Point(83, 6);
            this.Readings_fan1Speed_flow_label.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.Readings_fan1Speed_flow_label.Name = "Readings_fan1Speed_flow_label";
            this.Readings_fan1Speed_flow_label.Size = new System.Drawing.Size(39, 20);
            this.Readings_fan1Speed_flow_label.TabIndex = 1;
            this.Readings_fan1Speed_flow_label.Text = "rpm";
            // 
            // Readings_fan2Speed
            // 
            this.Readings_fan2Speed.Controls.Add(this.Readings_fan2Speed_flow);
            this.Readings_fan2Speed.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Readings_fan2Speed.Location = new System.Drawing.Point(769, 3);
            this.Readings_fan2Speed.Margin = new System.Windows.Forms.Padding(10, 3, 10, 3);
            this.Readings_fan2Speed.Name = "Readings_fan2Speed";
            this.Readings_fan2Speed.Padding = new System.Windows.Forms.Padding(10, 3, 10, 3);
            this.Readings_fan2Speed.Size = new System.Drawing.Size(136, 60);
            this.Readings_fan2Speed.TabIndex = 6;
            this.Readings_fan2Speed.TabStop = false;
            this.Readings_fan2Speed.Text = "Fan 2 speed";
            // 
            // Readings_fan2Speed_flow
            // 
            this.Readings_fan2Speed_flow.AutoSize = true;
            this.Readings_fan2Speed_flow.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Readings_fan2Speed_flow.Controls.Add(this.Readings_fan2Speed_flow_value);
            this.Readings_fan2Speed_flow.Controls.Add(this.Readings_fan2Speed_flow_label);
            this.Readings_fan2Speed_flow.Location = new System.Drawing.Point(10, 22);
            this.Readings_fan2Speed_flow.Name = "Readings_fan2Speed_flow";
            this.Readings_fan2Speed_flow.Size = new System.Drawing.Size(125, 32);
            this.Readings_fan2Speed_flow.TabIndex = 0;
            // 
            // Readings_fan2Speed_flow_value
            // 
            this.Readings_fan2Speed_flow_value.Enabled = false;
            this.Readings_fan2Speed_flow_value.Location = new System.Drawing.Point(3, 3);
            this.Readings_fan2Speed_flow_value.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.Readings_fan2Speed_flow_value.Name = "Readings_fan2Speed_flow_value";
            this.Readings_fan2Speed_flow_value.Size = new System.Drawing.Size(80, 26);
            this.Readings_fan2Speed_flow_value.TabIndex = 0;
            this.Readings_fan2Speed_flow_value.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // Readings_fan2Speed_flow_label
            // 
            this.Readings_fan2Speed_flow_label.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Readings_fan2Speed_flow_label.AutoSize = true;
            this.Readings_fan2Speed_flow_label.Location = new System.Drawing.Point(83, 6);
            this.Readings_fan2Speed_flow_label.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.Readings_fan2Speed_flow_label.Name = "Readings_fan2Speed_flow_label";
            this.Readings_fan2Speed_flow_label.Size = new System.Drawing.Size(39, 20);
            this.Readings_fan2Speed_flow_label.TabIndex = 1;
            this.Readings_fan2Speed_flow_label.Text = "rpm";
            // 
            // Data
            // 
            this.Data.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.Data.AutoSize = true;
            this.Data.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Data.Controls.Add(this.Data_DAQ);
            this.Data.Controls.Add(this.Data_save);
            this.Data.Location = new System.Drawing.Point(125, 354);
            this.Data.Margin = new System.Windows.Forms.Padding(0);
            this.Data.Name = "Data";
            this.Data.Size = new System.Drawing.Size(910, 70);
            this.Data.TabIndex = 4;
            // 
            // Data_DAQ
            // 
            this.Data_DAQ.Controls.Add(this.Data_DAQ_flow);
            this.Data_DAQ.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Data_DAQ.Location = new System.Drawing.Point(3, 3);
            this.Data_DAQ.Name = "Data_DAQ";
            this.Data_DAQ.Padding = new System.Windows.Forms.Padding(10, 3, 10, 3);
            this.Data_DAQ.Size = new System.Drawing.Size(348, 60);
            this.Data_DAQ.TabIndex = 0;
            this.Data_DAQ.TabStop = false;
            this.Data_DAQ.Text = "Data acquisition";
            // 
            // Data_DAQ_flow
            // 
            this.Data_DAQ_flow.Controls.Add(this.Data_DAQ_interval_label1);
            this.Data_DAQ_flow.Controls.Add(this.Data_DAQ_interval_value);
            this.Data_DAQ_flow.Controls.Add(this.Data_DAQ_interval_label2);
            this.Data_DAQ_flow.Controls.Add(this.Data_DAQ_start);
            this.Data_DAQ_flow.Controls.Add(this.Data_DAQ_stop);
            this.Data_DAQ_flow.Location = new System.Drawing.Point(10, 22);
            this.Data_DAQ_flow.Name = "Data_DAQ_flow";
            this.Data_DAQ_flow.Size = new System.Drawing.Size(325, 33);
            this.Data_DAQ_flow.TabIndex = 0;
            // 
            // Data_DAQ_interval_label1
            // 
            this.Data_DAQ_interval_label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Data_DAQ_interval_label1.AutoSize = true;
            this.Data_DAQ_interval_label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Data_DAQ_interval_label1.Location = new System.Drawing.Point(0, 8);
            this.Data_DAQ_interval_label1.Margin = new System.Windows.Forms.Padding(0);
            this.Data_DAQ_interval_label1.Name = "Data_DAQ_interval_label1";
            this.Data_DAQ_interval_label1.Size = new System.Drawing.Size(131, 17);
            this.Data_DAQ_interval_label1.TabIndex = 0;
            this.Data_DAQ_interval_label1.Text = "Acquire data every:";
            this.Data_DAQ_interval_label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Data_DAQ_interval_value
            // 
            this.Data_DAQ_interval_value.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Data_DAQ_interval_value.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Data_DAQ_interval_value.Location = new System.Drawing.Point(134, 5);
            this.Data_DAQ_interval_value.Maximum = new decimal(new int[] {
            86399,
            0,
            0,
            0});
            this.Data_DAQ_interval_value.Name = "Data_DAQ_interval_value";
            this.Data_DAQ_interval_value.Size = new System.Drawing.Size(60, 23);
            this.Data_DAQ_interval_value.TabIndex = 1;
            this.Data_DAQ_interval_value.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.Data_DAQ_interval_value.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.Data_DAQ_interval_value.ValueChanged += new System.EventHandler(this.DAQ_interval_value_ValueChanged);
            // 
            // Data_DAQ_interval_label2
            // 
            this.Data_DAQ_interval_label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Data_DAQ_interval_label2.AutoSize = true;
            this.Data_DAQ_interval_label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Data_DAQ_interval_label2.Location = new System.Drawing.Point(197, 8);
            this.Data_DAQ_interval_label2.Margin = new System.Windows.Forms.Padding(0);
            this.Data_DAQ_interval_label2.Name = "Data_DAQ_interval_label2";
            this.Data_DAQ_interval_label2.Size = new System.Drawing.Size(61, 17);
            this.Data_DAQ_interval_label2.TabIndex = 2;
            this.Data_DAQ_interval_label2.Text = "seconds";
            this.Data_DAQ_interval_label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Data_DAQ_start
            // 
            this.Data_DAQ_start.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Data_DAQ_start.Location = new System.Drawing.Point(258, 3);
            this.Data_DAQ_start.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.Data_DAQ_start.Name = "Data_DAQ_start";
            this.Data_DAQ_start.Size = new System.Drawing.Size(63, 27);
            this.Data_DAQ_start.TabIndex = 3;
            this.Data_DAQ_start.Text = "► Start";
            this.Data_DAQ_start.UseVisualStyleBackColor = true;
            this.Data_DAQ_start.Click += new System.EventHandler(this.DAQ_start_Click);
            // 
            // Data_DAQ_stop
            // 
            this.Data_DAQ_stop.Enabled = false;
            this.Data_DAQ_stop.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Data_DAQ_stop.Location = new System.Drawing.Point(0, 36);
            this.Data_DAQ_stop.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.Data_DAQ_stop.Name = "Data_DAQ_stop";
            this.Data_DAQ_stop.Size = new System.Drawing.Size(63, 27);
            this.Data_DAQ_stop.TabIndex = 4;
            this.Data_DAQ_stop.Text = "■ Stop";
            this.Data_DAQ_stop.UseVisualStyleBackColor = true;
            this.Data_DAQ_stop.Click += new System.EventHandler(this.DAQ_stop_Click);
            // 
            // Data_save
            // 
            this.Data_save.Controls.Add(this.Data_save_flow);
            this.Data_save.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Data_save.Location = new System.Drawing.Point(357, 3);
            this.Data_save.Name = "Data_save";
            this.Data_save.Padding = new System.Windows.Forms.Padding(10, 3, 10, 3);
            this.Data_save.Size = new System.Drawing.Size(550, 60);
            this.Data_save.TabIndex = 0;
            this.Data_save.TabStop = false;
            this.Data_save.Text = "Data recording";
            // 
            // Data_save_flow
            // 
            this.Data_save_flow.Controls.Add(this.Data_save_flow_path);
            this.Data_save_flow.Controls.Add(this.Data_save_flow_browse);
            this.Data_save_flow.Controls.Add(this.Data_save_flow_start);
            this.Data_save_flow.Controls.Add(this.Data_save_flow_stop);
            this.Data_save_flow.Location = new System.Drawing.Point(10, 22);
            this.Data_save_flow.Name = "Data_save_flow";
            this.Data_save_flow.Size = new System.Drawing.Size(525, 33);
            this.Data_save_flow.TabIndex = 0;
            // 
            // Data_save_flow_path
            // 
            this.Data_save_flow_path.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Data_save_flow_path.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Data_save_flow_path.Location = new System.Drawing.Point(3, 5);
            this.Data_save_flow_path.Name = "Data_save_flow_path";
            this.Data_save_flow_path.ReadOnly = true;
            this.Data_save_flow_path.Size = new System.Drawing.Size(415, 23);
            this.Data_save_flow_path.TabIndex = 0;
            // 
            // Data_save_flow_browse
            // 
            this.Data_save_flow_browse.AutoSize = true;
            this.Data_save_flow_browse.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Data_save_flow_browse.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Data_save_flow_browse.Location = new System.Drawing.Point(421, 3);
            this.Data_save_flow_browse.Margin = new System.Windows.Forms.Padding(0, 3, 7, 3);
            this.Data_save_flow_browse.Name = "Data_save_flow_browse";
            this.Data_save_flow_browse.Size = new System.Drawing.Size(30, 27);
            this.Data_save_flow_browse.TabIndex = 1;
            this.Data_save_flow_browse.Text = "...";
            this.Data_save_flow_browse.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.Data_save_flow_browse.UseVisualStyleBackColor = true;
            this.Data_save_flow_browse.Click += new System.EventHandler(this.OtherFunc_save_flow_browse_Click);
            // 
            // Data_save_flow_start
            // 
            this.Data_save_flow_start.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Data_save_flow_start.Location = new System.Drawing.Point(458, 3);
            this.Data_save_flow_start.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.Data_save_flow_start.Name = "Data_save_flow_start";
            this.Data_save_flow_start.Size = new System.Drawing.Size(63, 27);
            this.Data_save_flow_start.TabIndex = 2;
            this.Data_save_flow_start.Text = "► Start";
            this.Data_save_flow_start.UseVisualStyleBackColor = true;
            this.Data_save_flow_start.Click += new System.EventHandler(this.OtherFunc_save_flow_start_Click);
            // 
            // Data_save_flow_stop
            // 
            this.Data_save_flow_stop.Enabled = false;
            this.Data_save_flow_stop.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Data_save_flow_stop.Location = new System.Drawing.Point(0, 36);
            this.Data_save_flow_stop.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.Data_save_flow_stop.Name = "Data_save_flow_stop";
            this.Data_save_flow_stop.Size = new System.Drawing.Size(63, 27);
            this.Data_save_flow_stop.TabIndex = 3;
            this.Data_save_flow_stop.Text = "■ Stop";
            this.Data_save_flow_stop.UseVisualStyleBackColor = true;
            this.Data_save_flow_stop.Click += new System.EventHandler(this.OtherFunc_save_flow_stop_Click);
            // 
            // Other
            // 
            this.Other.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.Other.AutoSize = true;
            this.Other.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Other.Controls.Add(this.Graph);
            this.Other.Controls.Add(this.WeighScale);
            this.Other.Controls.Add(this.Status);
            this.Other.Location = new System.Drawing.Point(132, 424);
            this.Other.Margin = new System.Windows.Forms.Padding(0);
            this.Other.Name = "Other";
            this.Other.Size = new System.Drawing.Size(896, 70);
            this.Other.TabIndex = 5;
            // 
            // Graph
            // 
            this.Graph.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Graph.Controls.Add(this.Graph_flow);
            this.Graph.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Graph.Location = new System.Drawing.Point(15, 3);
            this.Graph.Margin = new System.Windows.Forms.Padding(15, 3, 15, 3);
            this.Graph.Name = "Graph";
            this.Graph.Padding = new System.Windows.Forms.Padding(10, 3, 10, 3);
            this.Graph.Size = new System.Drawing.Size(206, 60);
            this.Graph.TabIndex = 1;
            this.Graph.TabStop = false;
            this.Graph.Text = "Graph";
            // 
            // Graph_flow
            // 
            this.Graph_flow.AutoSize = true;
            this.Graph_flow.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Graph_flow.Controls.Add(this.Graph_interval_label1);
            this.Graph_flow.Controls.Add(this.Graph_interval_value);
            this.Graph_flow.Controls.Add(this.Graph_interval_label2);
            this.Graph_flow.Location = new System.Drawing.Point(10, 21);
            this.Graph_flow.Name = "Graph_flow";
            this.Graph_flow.Size = new System.Drawing.Size(183, 26);
            this.Graph_flow.TabIndex = 0;
            // 
            // Graph_interval_label1
            // 
            this.Graph_interval_label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Graph_interval_label1.AutoSize = true;
            this.Graph_interval_label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Graph_interval_label1.Location = new System.Drawing.Point(0, 4);
            this.Graph_interval_label1.Margin = new System.Windows.Forms.Padding(0);
            this.Graph_interval_label1.Name = "Graph_interval_label1";
            this.Graph_interval_label1.Size = new System.Drawing.Size(93, 17);
            this.Graph_interval_label1.TabIndex = 1;
            this.Graph_interval_label1.Text = "Time interval:";
            this.Graph_interval_label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Graph_interval_value
            // 
            this.Graph_interval_value.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Graph_interval_value.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Graph_interval_value.Location = new System.Drawing.Point(93, 0);
            this.Graph_interval_value.Margin = new System.Windows.Forms.Padding(0);
            this.Graph_interval_value.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.Graph_interval_value.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.Graph_interval_value.Name = "Graph_interval_value";
            this.Graph_interval_value.Size = new System.Drawing.Size(50, 26);
            this.Graph_interval_value.TabIndex = 2;
            this.Graph_interval_value.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.Graph_interval_value.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.Graph_interval_value.ValueChanged += new System.EventHandler(this.Other_graph_interval_value_ValueChanged);
            // 
            // Graph_interval_label2
            // 
            this.Graph_interval_label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Graph_interval_label2.AutoSize = true;
            this.Graph_interval_label2.Location = new System.Drawing.Point(143, 3);
            this.Graph_interval_label2.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.Graph_interval_label2.Name = "Graph_interval_label2";
            this.Graph_interval_label2.Size = new System.Drawing.Size(37, 20);
            this.Graph_interval_label2.TabIndex = 3;
            this.Graph_interval_label2.Text = "min";
            // 
            // WeighScale
            // 
            this.WeighScale.Controls.Add(this.WeighScale_flow);
            this.WeighScale.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WeighScale.Location = new System.Drawing.Point(251, 3);
            this.WeighScale.Margin = new System.Windows.Forms.Padding(15, 3, 15, 3);
            this.WeighScale.Name = "WeighScale";
            this.WeighScale.Padding = new System.Windows.Forms.Padding(10, 3, 10, 3);
            this.WeighScale.Size = new System.Drawing.Size(390, 60);
            this.WeighScale.TabIndex = 2;
            this.WeighScale.TabStop = false;
            this.WeighScale.Text = "Weighing scale";
            // 
            // WeighScale_flow
            // 
            this.WeighScale_flow.AutoSize = true;
            this.WeighScale_flow.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.WeighScale_flow.Controls.Add(this.WeighScale_flow_tare);
            this.WeighScale_flow.Controls.Add(this.WeighScale_flow_cal_label1);
            this.WeighScale_flow.Controls.Add(this.WeighScale_flow_cal_value);
            this.WeighScale_flow.Controls.Add(this.WeighScale_flow_cal_label2);
            this.WeighScale_flow.Controls.Add(this.WeighScale_flow_cal_button);
            this.WeighScale_flow.Location = new System.Drawing.Point(10, 21);
            this.WeighScale_flow.Name = "WeighScale_flow";
            this.WeighScale_flow.Size = new System.Drawing.Size(369, 33);
            this.WeighScale_flow.TabIndex = 0;
            // 
            // WeighScale_flow_tare
            // 
            this.WeighScale_flow_tare.AutoSize = true;
            this.WeighScale_flow_tare.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.WeighScale_flow_tare.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WeighScale_flow_tare.Location = new System.Drawing.Point(3, 3);
            this.WeighScale_flow_tare.Name = "WeighScale_flow_tare";
            this.WeighScale_flow_tare.Size = new System.Drawing.Size(48, 27);
            this.WeighScale_flow_tare.TabIndex = 0;
            this.WeighScale_flow_tare.Text = "Tare";
            this.WeighScale_flow_tare.UseVisualStyleBackColor = true;
            this.WeighScale_flow_tare.Click += new System.EventHandler(this.OtherFunc_scale_flow_tare_Click);
            // 
            // WeighScale_flow_cal_label1
            // 
            this.WeighScale_flow_cal_label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.WeighScale_flow_cal_label1.AutoSize = true;
            this.WeighScale_flow_cal_label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WeighScale_flow_cal_label1.Location = new System.Drawing.Point(74, 8);
            this.WeighScale_flow_cal_label1.Margin = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.WeighScale_flow_cal_label1.Name = "WeighScale_flow_cal_label1";
            this.WeighScale_flow_cal_label1.Size = new System.Drawing.Size(123, 17);
            this.WeighScale_flow_cal_label1.TabIndex = 1;
            this.WeighScale_flow_cal_label1.Text = "Calibration weight:";
            this.WeighScale_flow_cal_label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // WeighScale_flow_cal_value
            // 
            this.WeighScale_flow_cal_value.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.WeighScale_flow_cal_value.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WeighScale_flow_cal_value.Location = new System.Drawing.Point(197, 3);
            this.WeighScale_flow_cal_value.Margin = new System.Windows.Forms.Padding(0);
            this.WeighScale_flow_cal_value.Maximum = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            this.WeighScale_flow_cal_value.Name = "WeighScale_flow_cal_value";
            this.WeighScale_flow_cal_value.Size = new System.Drawing.Size(70, 26);
            this.WeighScale_flow_cal_value.TabIndex = 2;
            this.WeighScale_flow_cal_value.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.WeighScale_flow_cal_value.ValueChanged += new System.EventHandler(this.OtherFunc_scale_flow_calValue_ValueChanged);
            // 
            // WeighScale_flow_cal_label2
            // 
            this.WeighScale_flow_cal_label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.WeighScale_flow_cal_label2.AutoSize = true;
            this.WeighScale_flow_cal_label2.Location = new System.Drawing.Point(267, 6);
            this.WeighScale_flow_cal_label2.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.WeighScale_flow_cal_label2.Name = "WeighScale_flow_cal_label2";
            this.WeighScale_flow_cal_label2.Size = new System.Drawing.Size(19, 20);
            this.WeighScale_flow_cal_label2.TabIndex = 3;
            this.WeighScale_flow_cal_label2.Text = "g";
            // 
            // WeighScale_flow_cal_button
            // 
            this.WeighScale_flow_cal_button.AutoSize = true;
            this.WeighScale_flow_cal_button.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.WeighScale_flow_cal_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WeighScale_flow_cal_button.Location = new System.Drawing.Point(292, 3);
            this.WeighScale_flow_cal_button.Name = "WeighScale_flow_cal_button";
            this.WeighScale_flow_cal_button.Size = new System.Drawing.Size(74, 27);
            this.WeighScale_flow_cal_button.TabIndex = 4;
            this.WeighScale_flow_cal_button.Text = "Calibrate";
            this.WeighScale_flow_cal_button.UseVisualStyleBackColor = true;
            this.WeighScale_flow_cal_button.Click += new System.EventHandler(this.OtherFunc_scale_flow_calButton_Click);
            // 
            // Status
            // 
            this.Status.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Status.BackColor = System.Drawing.SystemColors.Control;
            this.Status.Controls.Add(this.Status_flow);
            this.Status.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Status.Location = new System.Drawing.Point(666, 3);
            this.Status.Margin = new System.Windows.Forms.Padding(10, 3, 10, 3);
            this.Status.Name = "Status";
            this.Status.Padding = new System.Windows.Forms.Padding(10, 3, 10, 3);
            this.Status.Size = new System.Drawing.Size(220, 60);
            this.Status.TabIndex = 3;
            this.Status.TabStop = false;
            this.Status.Text = "Connection status";
            // 
            // Status_flow
            // 
            this.Status_flow.AutoSize = true;
            this.Status_flow.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Status_flow.Controls.Add(this.Status_value);
            this.Status_flow.Location = new System.Drawing.Point(10, 22);
            this.Status_flow.Name = "Status_flow";
            this.Status_flow.Size = new System.Drawing.Size(203, 32);
            this.Status_flow.TabIndex = 0;
            // 
            // Status_value
            // 
            this.Status_value.Enabled = false;
            this.Status_value.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Status_value.Location = new System.Drawing.Point(3, 3);
            this.Status_value.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.Status_value.Name = "Status_value";
            this.Status_value.Size = new System.Drawing.Size(200, 26);
            this.Status_value.TabIndex = 0;
            this.Status_value.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // Displaychart
            // 
            customLabel1.ForeColor = System.Drawing.Color.Black;
            customLabel1.FromPosition = 10D;
            customLabel1.GridTicks = System.Windows.Forms.DataVisualization.Charting.GridTickTypes.TickMark;
            customLabel1.Text = "0";
            customLabel1.ToPosition = -10D;
            customLabel2.ForeColor = System.Drawing.Color.Black;
            customLabel2.FromPosition = -110D;
            customLabel2.GridTicks = System.Windows.Forms.DataVisualization.Charting.GridTickTypes.TickMark;
            customLabel2.Text = "-2";
            customLabel2.ToPosition = -130D;
            customLabel3.ForeColor = System.Drawing.Color.Black;
            customLabel3.FromPosition = -230D;
            customLabel3.GridTicks = System.Windows.Forms.DataVisualization.Charting.GridTickTypes.TickMark;
            customLabel3.Text = "-4";
            customLabel3.ToPosition = -250D;
            customLabel4.ForeColor = System.Drawing.Color.Black;
            customLabel4.FromPosition = -350D;
            customLabel4.GridTicks = System.Windows.Forms.DataVisualization.Charting.GridTickTypes.TickMark;
            customLabel4.Text = "-6";
            customLabel4.ToPosition = -370D;
            customLabel5.ForeColor = System.Drawing.Color.Black;
            customLabel5.FromPosition = -470D;
            customLabel5.GridTicks = System.Windows.Forms.DataVisualization.Charting.GridTickTypes.TickMark;
            customLabel5.Text = "-8";
            customLabel5.ToPosition = -490D;
            customLabel6.ForeColor = System.Drawing.Color.Black;
            customLabel6.FromPosition = -590D;
            customLabel6.GridTicks = System.Windows.Forms.DataVisualization.Charting.GridTickTypes.TickMark;
            customLabel6.Text = "-10";
            customLabel6.ToPosition = -610D;
            chartArea1.AxisX.CustomLabels.Add(customLabel1);
            chartArea1.AxisX.CustomLabels.Add(customLabel2);
            chartArea1.AxisX.CustomLabels.Add(customLabel3);
            chartArea1.AxisX.CustomLabels.Add(customLabel4);
            chartArea1.AxisX.CustomLabels.Add(customLabel5);
            chartArea1.AxisX.CustomLabels.Add(customLabel6);
            chartArea1.AxisX.IsLabelAutoFit = false;
            chartArea1.AxisX.LabelStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea1.AxisX.MajorGrid.Enabled = false;
            chartArea1.AxisX.TextOrientation = System.Windows.Forms.DataVisualization.Charting.TextOrientation.Horizontal;
            chartArea1.AxisX.Title = "Time (min)";
            chartArea1.AxisX.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea1.AxisY.LabelStyle.ForeColor = System.Drawing.Color.RoyalBlue;
            chartArea1.AxisY.LineColor = System.Drawing.Color.RoyalBlue;
            chartArea1.AxisY.MajorGrid.LineColor = System.Drawing.Color.Transparent;
            chartArea1.AxisY.MajorTickMark.LineColor = System.Drawing.Color.RoyalBlue;
            chartArea1.AxisY.Maximum = 100D;
            chartArea1.AxisY.Title = "Relative humidity (%)";
            chartArea1.AxisY.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea1.AxisY.TitleForeColor = System.Drawing.Color.RoyalBlue;
            chartArea1.AxisY2.LabelStyle.ForeColor = System.Drawing.Color.MediumVioletRed;
            chartArea1.AxisY2.LineColor = System.Drawing.Color.MediumVioletRed;
            chartArea1.AxisY2.MajorGrid.LineColor = System.Drawing.Color.Transparent;
            chartArea1.AxisY2.MajorTickMark.LineColor = System.Drawing.Color.MediumVioletRed;
            chartArea1.AxisY2.Title = "Weight (g)";
            chartArea1.AxisY2.TitleFont = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea1.AxisY2.TitleForeColor = System.Drawing.Color.MediumVioletRed;
            chartArea1.Name = "ChartArea1";
            this.Displaychart.ChartAreas.Add(chartArea1);
            this.Displaychart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Displaychart.Location = new System.Drawing.Point(20, 33);
            this.Displaychart.Margin = new System.Windows.Forms.Padding(20, 3, 20, 3);
            this.Displaychart.MinimumSize = new System.Drawing.Size(0, 200);
            this.Displaychart.Name = "Displaychart";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Color = System.Drawing.Color.RoyalBlue;
            series1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            series1.Name = "Relative humidity";
            series1.Points.Add(dataPoint1);
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series2.Color = System.Drawing.Color.MediumVioletRed;
            series2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            series2.Name = "Weight";
            series2.Points.Add(dataPoint2);
            series2.YAxisType = System.Windows.Forms.DataVisualization.Charting.AxisType.Secondary;
            this.Displaychart.Series.Add(series1);
            this.Displaychart.Series.Add(series2);
            this.Displaychart.Size = new System.Drawing.Size(1120, 248);
            this.Displaychart.SuppressExceptions = true;
            this.Displaychart.TabIndex = 0;
            this.Displaychart.Text = "chart1";
            // 
            // Body_panel_btm
            // 
            this.Body_panel_btm.AutoSize = true;
            this.Body_panel_btm.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Body_panel_btm.ColumnCount = 1;
            this.Body_panel_btm.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.Body_panel_btm.Controls.Add(this.HumidityControl_label, 0, 0);
            this.Body_panel_btm.Controls.Add(this.HumidityControl, 0, 1);
            this.Body_panel_btm.Controls.Add(this.HumidityControl_heading, 0, 2);
            this.Body_panel_btm.Controls.Add(this.HumidityControl_steps, 0, 3);
            this.Body_panel_btm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Body_panel_btm.Location = new System.Drawing.Point(0, 0);
            this.Body_panel_btm.Name = "Body_panel_btm";
            this.Body_panel_btm.RowCount = 4;
            this.Body_panel_btm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.Body_panel_btm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.Body_panel_btm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.Body_panel_btm.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.Body_panel_btm.Size = new System.Drawing.Size(1160, 238);
            this.Body_panel_btm.TabIndex = 0;
            // 
            // AgenatorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1164, 862);
            this.Controls.Add(this.Body);
            this.Controls.Add(this.menu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menu;
            this.Name = "AgenatorForm";
            this.Text = "Agenator";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AgenatorForm_FormClosing);
            this.menu.ResumeLayout(false);
            this.menu.PerformLayout();
            this.HumidityControl_steps.ResumeLayout(false);
            this.HumidityControl_steps_table.ResumeLayout(false);
            this.HumidityControl_steps_1.ResumeLayout(false);
            this.HumidityControl_steps_1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.HumidityControl_steps_1_humidity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.HumidityControl_steps_1_duration_day_value)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.HumidityControl_steps_1_duration_hour_value)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.HumidityControl_steps_1_duration_minute_value)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.HumidityControl_steps_1_duration_second_value)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.HumidityControl_steps_1_fanSpeed)).EndInit();
            this.HumidityControl_heading.ResumeLayout(false);
            this.HumidityControl_heading.PerformLayout();
            this.HumidityControl.ResumeLayout(false);
            this.HumidityControl.PerformLayout();
            this.Body.Panel1.ResumeLayout(false);
            this.Body.Panel2.ResumeLayout(false);
            this.Body.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Body)).EndInit();
            this.Body.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.Body_panel_top.ResumeLayout(false);
            this.Body_panel_top.PerformLayout();
            this.Chamber_flow.ResumeLayout(false);
            this.Chamber_flow.PerformLayout();
            this.Advanced_button.ResumeLayout(false);
            this.Advanced_options.ResumeLayout(false);
            this.Advanced_options.PerformLayout();
            this.Advanced_humidity.ResumeLayout(false);
            this.Advanced_humidity.PerformLayout();
            this.Advanced_humidity_flow.ResumeLayout(false);
            this.Advanced_humidity_flow.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Advanced_humidity_RH1_raw_value)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Advanced_humidity_RH1_std_value)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Advanced_humidity_RH2_raw_value)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Advanced_humidity_RH2_std_value)).EndInit();
            this.Advanced_weigh.ResumeLayout(false);
            this.Advanced_weigh.PerformLayout();
            this.Advanced_weigh_flow.ResumeLayout(false);
            this.Advanced_weigh_flow.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Advanced_weigh_factor_value)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Advanced_weigh_offset_value)).EndInit();
            this.Readings.ResumeLayout(false);
            this.Readings_humidity.ResumeLayout(false);
            this.Readings_humidity.PerformLayout();
            this.Readings_humidity_flow.ResumeLayout(false);
            this.Readings_humidity_flow.PerformLayout();
            this.Readings_temperature.ResumeLayout(false);
            this.Readings_temperature.PerformLayout();
            this.Readings_temperature_flow.ResumeLayout(false);
            this.Readings_temperature_flow.PerformLayout();
            this.Readings_weight.ResumeLayout(false);
            this.Readings_weight.PerformLayout();
            this.Readings_weight_flow.ResumeLayout(false);
            this.Readings_weight_flow.PerformLayout();
            this.Readings_velocity.ResumeLayout(false);
            this.Readings_velocity.PerformLayout();
            this.Readings_velocity_flow.ResumeLayout(false);
            this.Readings_velocity_flow.PerformLayout();
            this.Readings_fan1Speed.ResumeLayout(false);
            this.Readings_fan1Speed.PerformLayout();
            this.Readings_fan1Speed_flow.ResumeLayout(false);
            this.Readings_fan1Speed_flow.PerformLayout();
            this.Readings_fan2Speed.ResumeLayout(false);
            this.Readings_fan2Speed.PerformLayout();
            this.Readings_fan2Speed_flow.ResumeLayout(false);
            this.Readings_fan2Speed_flow.PerformLayout();
            this.Data.ResumeLayout(false);
            this.Data_DAQ.ResumeLayout(false);
            this.Data_DAQ_flow.ResumeLayout(false);
            this.Data_DAQ_flow.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Data_DAQ_interval_value)).EndInit();
            this.Data_save.ResumeLayout(false);
            this.Data_save_flow.ResumeLayout(false);
            this.Data_save_flow.PerformLayout();
            this.Other.ResumeLayout(false);
            this.Graph.ResumeLayout(false);
            this.Graph.PerformLayout();
            this.Graph_flow.ResumeLayout(false);
            this.Graph_flow.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Graph_interval_value)).EndInit();
            this.WeighScale.ResumeLayout(false);
            this.WeighScale.PerformLayout();
            this.WeighScale_flow.ResumeLayout(false);
            this.WeighScale_flow.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.WeighScale_flow_cal_value)).EndInit();
            this.Status.ResumeLayout(false);
            this.Status.PerformLayout();
            this.Status_flow.ResumeLayout(false);
            this.Status_flow.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Displaychart)).EndInit();
            this.Body_panel_btm.ResumeLayout(false);
            this.Body_panel_btm.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menu;
        private System.Windows.Forms.ToolStripMenuItem menu_config;
        private System.Windows.Forms.ToolStripMenuItem menu_config_openConfig;
        private System.Windows.Forms.ToolStripTextBox menu_copyright;
        private System.Windows.Forms.ToolStripMenuItem menu_config_saveConfig;
        private System.Windows.Forms.OpenFileDialog openConfigFileDialog;
        private System.Windows.Forms.SaveFileDialog saveConfigFileDialog;
        public System.Windows.Forms.SaveFileDialog recordFileDialog;
        private System.Windows.Forms.TableLayoutPanel HumidityControl_steps_table;
        private System.Windows.Forms.FlowLayoutPanel HumidityControl;
        private System.Windows.Forms.Label HumidityControl_label;
        private System.Windows.Forms.Button HumidityControl_start;
        private System.Windows.Forms.Button HumidityControl_pause;
        private System.Windows.Forms.FlowLayoutPanel HumidityControl_steps_1;
        private System.Windows.Forms.Button HumidityControl_steps_1_order_up;
        private System.Windows.Forms.Button HumidityControl_steps_1_order_down;
        private System.Windows.Forms.ComboBox HumidityControl_steps_1_type;
        private System.Windows.Forms.NumericUpDown HumidityControl_steps_1_humidity;
        private System.Windows.Forms.NumericUpDown HumidityControl_steps_1_duration_day_value;
        private System.Windows.Forms.Label HumidityControl_steps_1_duration_day_label;
        private System.Windows.Forms.NumericUpDown HumidityControl_steps_1_duration_hour_value;
        private System.Windows.Forms.Label HumidityControl_steps_1_duration_hour_label;
        private System.Windows.Forms.NumericUpDown HumidityControl_steps_1_duration_minute_value;
        private System.Windows.Forms.Label HumidityControl_steps_1_duration_minute_label;
        private System.Windows.Forms.NumericUpDown HumidityControl_steps_1_duration_second_value;
        private System.Windows.Forms.Label HumidityControl_steps_1_duration_second_label;
        private System.Windows.Forms.FlowLayoutPanel HumidityControl_heading;
        private System.Windows.Forms.Label HumidityControl_heading_order;
        private System.Windows.Forms.Label HumidityControl_heading_type;
        private System.Windows.Forms.Label HumidityControl_heading_humidity;
        private System.Windows.Forms.Label HumidityControl_heading_duration;
        private System.Windows.Forms.Label HumidityControl_heading_fanSpeed;
        private System.Windows.Forms.Label HumidityControl_heading_status;
        private System.Windows.Forms.NumericUpDown HumidityControl_steps_1_fanSpeed;
        private System.Windows.Forms.ComboBox HumidityControl_steps_1_status;
        private System.Windows.Forms.Button HumidityControl_steps_1_delete;
        private System.Windows.Forms.FlowLayoutPanel HumidityControl_steps;
        private System.Windows.Forms.Button HumidityControl_addStep;
        private System.Windows.Forms.SplitContainer Body;
        private System.Windows.Forms.TableLayoutPanel Body_panel_btm;
        private System.Windows.Forms.Label HumidityControl_steps_1_order_number;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel Body_panel_top;
        private System.Windows.Forms.FlowLayoutPanel Chamber_flow;
        private System.Windows.Forms.Button Chamber_prev;
        private System.Windows.Forms.Label Chamber_title;
        private System.Windows.Forms.Button Chamber_next;
        private System.Windows.Forms.FlowLayoutPanel Readings;
        private System.Windows.Forms.GroupBox Readings_humidity;
        private System.Windows.Forms.FlowLayoutPanel Readings_humidity_flow;
        private System.Windows.Forms.TextBox Readings_humidity_flow_value;
        private System.Windows.Forms.Label Readings_humidity_flow_label;
        private System.Windows.Forms.GroupBox Readings_temperature;
        private System.Windows.Forms.FlowLayoutPanel Readings_temperature_flow;
        private System.Windows.Forms.TextBox Readings_temperature_flow_value;
        private System.Windows.Forms.Label Readings_temperature_flow_label;
        private System.Windows.Forms.GroupBox Readings_weight;
        private System.Windows.Forms.FlowLayoutPanel Readings_weight_flow;
        private System.Windows.Forms.TextBox Readings_weight_flow_value;
        private System.Windows.Forms.Label Readings_weight_flow_label;
        private System.Windows.Forms.GroupBox Readings_velocity;
        private System.Windows.Forms.FlowLayoutPanel Readings_velocity_flow;
        private System.Windows.Forms.TextBox Readings_velocity_flow_value;
        private System.Windows.Forms.Label Readings_velocity_flow_label;
        private System.Windows.Forms.GroupBox Readings_fan1Speed;
        private System.Windows.Forms.FlowLayoutPanel Readings_fan1Speed_flow;
        private System.Windows.Forms.TextBox Readings_fan1Speed_flow_value;
        private System.Windows.Forms.Label Readings_fan1Speed_flow_label;
        private System.Windows.Forms.FlowLayoutPanel Data;
        private System.Windows.Forms.GroupBox Data_DAQ;
        private System.Windows.Forms.FlowLayoutPanel Data_DAQ_flow;
        private System.Windows.Forms.Label Data_DAQ_interval_label1;
        private System.Windows.Forms.NumericUpDown Data_DAQ_interval_value;
        private System.Windows.Forms.Label Data_DAQ_interval_label2;
        private System.Windows.Forms.Button Data_DAQ_start;
        private System.Windows.Forms.Button Data_DAQ_stop;
        private System.Windows.Forms.GroupBox Data_save;
        private System.Windows.Forms.FlowLayoutPanel Data_save_flow;
        private System.Windows.Forms.TextBox Data_save_flow_path;
        private System.Windows.Forms.Button Data_save_flow_browse;
        private System.Windows.Forms.Button Data_save_flow_start;
        private System.Windows.Forms.Button Data_save_flow_stop;
        private System.Windows.Forms.FlowLayoutPanel Other;
        private System.Windows.Forms.GroupBox Graph;
        private System.Windows.Forms.FlowLayoutPanel Graph_flow;
        private System.Windows.Forms.Label Graph_interval_label1;
        private System.Windows.Forms.NumericUpDown Graph_interval_value;
        private System.Windows.Forms.Label Graph_interval_label2;
        private System.Windows.Forms.GroupBox WeighScale;
        private System.Windows.Forms.FlowLayoutPanel WeighScale_flow;
        private System.Windows.Forms.Button WeighScale_flow_tare;
        private System.Windows.Forms.Label WeighScale_flow_cal_label1;
        private System.Windows.Forms.NumericUpDown WeighScale_flow_cal_value;
        private System.Windows.Forms.Label WeighScale_flow_cal_label2;
        private System.Windows.Forms.Button WeighScale_flow_cal_button;
        private System.Windows.Forms.DataVisualization.Charting.Chart Displaychart;
        private System.Windows.Forms.Label Advanced_weigh_factor_label;
        private System.Windows.Forms.GroupBox Readings_fan2Speed;
        private System.Windows.Forms.FlowLayoutPanel Readings_fan2Speed_flow;
        private System.Windows.Forms.TextBox Readings_fan2Speed_flow_value;
        private System.Windows.Forms.Label Readings_fan2Speed_flow_label;
        private System.Windows.Forms.GroupBox Status;
        private System.Windows.Forms.FlowLayoutPanel Status_flow;
        private System.Windows.Forms.TextBox Status_value;
        private System.Windows.Forms.ToolStripTextBox menu_autosave_status;
        private System.Windows.Forms.ToolStripMenuItem menu_autosave;
        private System.Windows.Forms.ToolStripMenuItem menu_autosave_enableDisable;
        private System.Windows.Forms.ToolStripSeparator menu_autosave_separator;
        private System.Windows.Forms.ToolStripTextBox menu_autosave_interval_label;
        private System.Windows.Forms.ToolStripTextBox menu_autosave_interval_value;
        private System.Windows.Forms.ToolStripSeparator menu_autosave_separator2;
        private System.Windows.Forms.ToolStripMenuItem menu_autosave_load;
        private System.Windows.Forms.Button Advanced_button_show;
        private System.Windows.Forms.FlowLayoutPanel Advanced_options;
        private System.Windows.Forms.GroupBox Advanced_humidity;
        private System.Windows.Forms.FlowLayoutPanel Advanced_humidity_flow;
        private System.Windows.Forms.Label Advanced_humidity_RH1_std_label;
        private System.Windows.Forms.NumericUpDown Advanced_humidity_RH1_std_value;
        private System.Windows.Forms.Button Advanced_humidity_RH1_std_apply;
        private System.Windows.Forms.Label Advanced_humidity_RH2_std_label;
        private System.Windows.Forms.NumericUpDown Advanced_humidity_RH2_std_value;
        private System.Windows.Forms.Button Advanced_humidity_RH2_std_apply;
        private System.Windows.Forms.GroupBox Advanced_weigh;
        private System.Windows.Forms.FlowLayoutPanel Advanced_weigh_flow;
        private System.Windows.Forms.NumericUpDown Advanced_weigh_factor_value;
        private System.Windows.Forms.Label Advanced_weigh_offset_label;
        private System.Windows.Forms.NumericUpDown Advanced_weigh_offset_value;
        private System.Windows.Forms.FlowLayoutPanel Advanced_button;
        private System.Windows.Forms.Button Advanced_button_hide;
        private System.Windows.Forms.Label Advanced_humidity_RH1_raw_label;
        private System.Windows.Forms.NumericUpDown Advanced_humidity_RH1_raw_value;
        private System.Windows.Forms.Label Advanced_humidity_RH2_raw_label;
        private System.Windows.Forms.NumericUpDown Advanced_humidity_RH2_raw_value;
    }
}

