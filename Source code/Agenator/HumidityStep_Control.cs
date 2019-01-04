using System;
using System.Drawing;
using System.Windows.Forms;

namespace Agenator
{
    public class HumidityStep_Control : FlowLayoutPanel
    {
        public const String CONTROL_STEP_TYPE_HOLD         = "Hold";
        public const String CONTROL_STEP_TYPE_RAMP         = "Ramp";
        public const String CONTROL_STEP_STATUS_COMPLETED  = "Completed";
        public const String CONTROL_STEP_STATUS_RUNNING    = "Running";
        public const String CONTROL_STEP_STATUS_WAITING    = "Waiting";

        private uint stepID_;
        private String controlPrefix_;

        public Button               order_up;
        public Label                order_number;
        public Button               order_down;
        public ComboBox             stepType;
        public NumericUpDown        humidity;
        public NumericUpDown        duration_day_value;
        private Label               duration_day_label_;
        public NumericUpDown        duration_hour_value;
        private Label               duration_hour_label_;
        public NumericUpDown        duration_minute_value;
        private Label               duration_minute_label_;
        public NumericUpDown        duration_second_value;
        private Label               duration_second_label_;
        public NumericUpDown        fanSpeed;
        public ComboBox             stepStatus;
        public Button               deleteButton;
        public Button               skipButton;

        private delegate void _suspendLayoutControlCallback(Control refControl);
        private delegate void _resumeLayoutControlCallback(Control refControl);
        private delegate void _performLayoutControlCallback(Control refControl);
        private delegate void _changeFlowVisibilityCallback(FlowLayoutPanel flowPanel, bool visibility);
        private delegate void _changeControlEnableCallback(Control butControl, bool state);
        private delegate void _showControlCallback(Control refControl, Control parentControl);
        private delegate void _hideControlCallback(Control refControl, Control parentControl);
        private delegate void _updateTextboxCallback(TextBox boxControl, string data);
        private delegate void _updateControlValueCallback(NumericUpDown refControl, decimal data);
        private delegate string _getComboSelectedItemCallback(ComboBox comboControl);
        private delegate void _changeControlBgCallback(Control refControl, Color refColor);

        public HumidityStep_Control(uint stepID, int activeStepID, int stepCount, bool controlRunning, bool controlPaused, bool skipControlStepWaiting, bool nextControlStepWaiting, bool pauseControlWaiting, bool endControlWaiting, HumidityStep.stepType givenStepType, double targetHumidity, int dayRemaining, int hourRemaining, int minuteRemaining, int secondRemaining, double targetFanSpeed, HumidityStep.stepStatus givenStepStatus)
        {
            stepID_ = stepID;
            controlPrefix_ = "HumidityControl_steps_" + Convert.ToString(stepID_) + "_";

            order_up = new System.Windows.Forms.Button();
            order_number = new System.Windows.Forms.Label();
            order_down = new System.Windows.Forms.Button();
            stepType = new System.Windows.Forms.ComboBox();
            humidity = new System.Windows.Forms.NumericUpDown();
            duration_day_value = new System.Windows.Forms.NumericUpDown();
            duration_day_label_ = new System.Windows.Forms.Label();
            duration_hour_value = new System.Windows.Forms.NumericUpDown();
            duration_hour_label_ = new System.Windows.Forms.Label();
            duration_minute_value = new System.Windows.Forms.NumericUpDown();
            duration_minute_label_ = new System.Windows.Forms.Label();
            duration_second_value = new System.Windows.Forms.NumericUpDown();
            duration_second_label_ = new System.Windows.Forms.Label();
            fanSpeed = new System.Windows.Forms.NumericUpDown();
            stepStatus = new System.Windows.Forms.ComboBox();
            deleteButton = new System.Windows.Forms.Button();
            skipButton = new System.Windows.Forms.Button();

            this.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(humidity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(duration_day_value)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(duration_hour_value)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(duration_minute_value)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(duration_second_value)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(fanSpeed)).BeginInit();


            ////////////////////////////////////
            //       this (a FlowLayoutPanel)
            ////////////////////////////////////
            // 
            // Parent container for all the controls
            //
            this.Controls.Add(order_up);
            this.Controls.Add(order_number);
            this.Controls.Add(order_down);
            this.Controls.Add(stepType);
            this.Controls.Add(humidity);
            this.Controls.Add(duration_day_value);
            this.Controls.Add(duration_day_label_);
            this.Controls.Add(duration_hour_value);
            this.Controls.Add(duration_hour_label_);
            this.Controls.Add(duration_minute_value);
            this.Controls.Add(duration_minute_label_);
            this.Controls.Add(duration_second_value);
            this.Controls.Add(duration_second_label_);
            this.Controls.Add(fanSpeed);
            this.Controls.Add(stepStatus);
            this.Controls.Add(deleteButton);
            this.Controls.Add(skipButton);
            this.Location = new System.Drawing.Point(0, 0);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "HumidityControl_steps_" + Convert.ToString(stepID_);
            this.Size = new System.Drawing.Size(1110, 33);
            this.TabIndex = Convert.ToInt16(stepID_);

            if (controlRunning && givenStepStatus == HumidityStep.stepStatus.STATUS_COMPLETED)
            {
                this.BackColor = SystemColors.ControlDark;
            }
            else
            {
                this.BackColor = SystemColors.Control;
            }

            ////////////////////////////////////
            //       Controls
            ////////////////////////////////////
            // 
            // order_up
            // 
            order_up.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            order_up.Location = new System.Drawing.Point(3, 3);
            order_up.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            order_up.Name = controlPrefix_ + "order_up";
            order_up.Size = new System.Drawing.Size(29, 23);
            order_up.TabIndex = 0;
            order_up.Text = "▲";
            order_up.UseVisualStyleBackColor = true;

            if (    stepID == 0
                    ||
                    stepID == activeStepID + 1  // When idle, activeStep is -1. so this becomes the first condition. When control is running, this becomes the step after the running step
                    ||
                    (   controlRunning 
                        &&
                        (   givenStepStatus == HumidityStep.stepStatus.STATUS_RUNNING
                            ||
                            (givenStepStatus == HumidityStep.stepStatus.STATUS_COMPLETED && stepID < activeStepID)
                        )
                    )
                )
            {
                order_up.Enabled = false;
            }
            // 
            // order_number
            // 
            order_number.AutoSize = true;
            order_number.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            order_number.Location = new System.Drawing.Point(32, 0);
            order_number.Margin = new System.Windows.Forms.Padding(0);
            order_number.Name = controlPrefix_ + "order_number";
            order_number.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            order_number.Size = new System.Drawing.Size(15, 28);
            order_number.TabIndex = 1;
            order_number.Text = Convert.ToString(stepID_ + 1); // Step ID is zero-indexed, but display to user as one-indexed
            order_number.UseCompatibleTextRendering = true;
            // 
            // order_down
            // 
            order_down.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            order_down.Location = new System.Drawing.Point(47, 3);
            order_down.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            order_down.Name = controlPrefix_ + "order_down";
            order_down.Size = new System.Drawing.Size(29, 23);
            order_down.TabIndex = 2;
            order_down.Text = "▼";
            order_down.UseVisualStyleBackColor = true;

            if (    stepID == (stepCount - 1)
                    ||
                    (   controlRunning 
                        &&
                        (   givenStepStatus == HumidityStep.stepStatus.STATUS_RUNNING
                            ||
                            (givenStepStatus == HumidityStep.stepStatus.STATUS_COMPLETED && stepID < activeStepID)
                        )
                    )
                )
            {
                order_down.Enabled = false;
            }
            // 
            // type
            // 
            stepType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            stepType.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            stepType.FormattingEnabled = true;
            stepType.Items.AddRange(new object[] {
            CONTROL_STEP_TYPE_HOLD,
            CONTROL_STEP_TYPE_RAMP});
            stepType.Location = new System.Drawing.Point(99, 3);
            stepType.Margin = new System.Windows.Forms.Padding(20, 3, 3, 3);
            stepType.Name = controlPrefix_ + "type";
            stepType.Size = new System.Drawing.Size(60, 24);
            stepType.TabIndex = 3;

            if (givenStepType == HumidityStep.stepType.TYPE_HOLD)
            {
                stepType.Text = CONTROL_STEP_TYPE_HOLD;
                stepType.SelectedValue = CONTROL_STEP_TYPE_HOLD; // This sets the default value of the drop down list...
                stepType.SelectedItem = CONTROL_STEP_TYPE_HOLD; // ...and this makes it display the value
            }
            else if (givenStepType == HumidityStep.stepType.TYPE_RAMP)
            {
                stepType.Text = CONTROL_STEP_TYPE_RAMP;
                stepType.SelectedValue = CONTROL_STEP_TYPE_RAMP; // This sets the default value of the drop down list...
                stepType.SelectedItem = CONTROL_STEP_TYPE_RAMP; // ...and this makes it display the value
            }

            if (    controlRunning 
                    &&
                    (   givenStepStatus == HumidityStep.stepStatus.STATUS_RUNNING
                        ||
                        (givenStepStatus == HumidityStep.stepStatus.STATUS_COMPLETED && stepID < activeStepID)
                    )
                )
            {
                stepType.Enabled = false;
            }
            // 
            // humidity
            // 
            humidity.DecimalPlaces = 1;
            humidity.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            humidity.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            humidity.Location = new System.Drawing.Point(222, 3);
            humidity.Margin = new System.Windows.Forms.Padding(60, 3, 3, 3);
            humidity.Name = controlPrefix_ + "humidity";
            humidity.Size = new System.Drawing.Size(50, 23);
            humidity.TabIndex = 4;
            humidity.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            humidity.Value = Convert.ToDecimal(targetHumidity);

            if (    controlRunning 
                    &&
                    (   givenStepStatus == HumidityStep.stepStatus.STATUS_RUNNING
                        ||
                        (givenStepStatus == HumidityStep.stepStatus.STATUS_COMPLETED && stepID < activeStepID)
                    )
                )
            {
                humidity.Enabled = false;
            }
            // 
            // duration_day_value
            // 
            duration_day_value.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            duration_day_value.Location = new System.Drawing.Point(385, 3);
            duration_day_value.Margin = new System.Windows.Forms.Padding(110, 3, 0, 3);
            duration_day_value.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            duration_day_value.Name = controlPrefix_ + "duration_day_value";
            duration_day_value.Size = new System.Drawing.Size(45, 23);
            duration_day_value.TabIndex = 5;
            duration_day_value.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            duration_day_value.Value = Convert.ToDecimal(dayRemaining);

            if (    controlRunning 
                    &&
                    (   givenStepStatus == HumidityStep.stepStatus.STATUS_RUNNING
                        ||
                        (givenStepStatus == HumidityStep.stepStatus.STATUS_COMPLETED && stepID < activeStepID)
                    )
                )
            {
                duration_day_value.Enabled = false;
            }
            // 
            // duration_day_label
            // 
            duration_day_label_.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            duration_day_label_.Location = new System.Drawing.Point(430, 0);
            duration_day_label_.Margin = new System.Windows.Forms.Padding(0);
            duration_day_label_.Name = controlPrefix_ + "duration_day_label";
            duration_day_label_.Size = new System.Drawing.Size(25, 25);
            duration_day_label_.TabIndex = 6;
            duration_day_label_.Text = "D";
            // 
            // duration_hour_value
            // 
            duration_hour_value.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            duration_hour_value.Location = new System.Drawing.Point(475, 3);
            duration_hour_value.Margin = new System.Windows.Forms.Padding(20, 3, 0, 3);
            duration_hour_value.Maximum = new decimal(new int[] {
            23,
            0,
            0,
            0});
            duration_hour_value.Name = controlPrefix_ + "duration_hour_value";
            duration_hour_value.Size = new System.Drawing.Size(35, 23);
            duration_hour_value.TabIndex = 7;
            duration_hour_value.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            duration_hour_value.Value = Convert.ToDecimal(hourRemaining);

            if (    controlRunning 
                    &&
                    (   givenStepStatus == HumidityStep.stepStatus.STATUS_RUNNING
                        ||
                        (givenStepStatus == HumidityStep.stepStatus.STATUS_COMPLETED && stepID < activeStepID)
                    )
                )
            {
                duration_hour_value.Enabled = false;
            }
            // 
            // duration_hour_label
            // 
            duration_hour_label_.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            duration_hour_label_.Location = new System.Drawing.Point(510, 0);
            duration_hour_label_.Margin = new System.Windows.Forms.Padding(0);
            duration_hour_label_.Name = controlPrefix_ + "duration_hour_label";
            duration_hour_label_.Size = new System.Drawing.Size(25, 25);
            duration_hour_label_.TabIndex = 8;
            duration_hour_label_.Text = "H";
            // 
            // duration_minute_value
            // 
            duration_minute_value.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            duration_minute_value.Location = new System.Drawing.Point(555, 3);
            duration_minute_value.Margin = new System.Windows.Forms.Padding(20, 3, 0, 3);
            duration_minute_value.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            duration_minute_value.Name = controlPrefix_ + "duration_minute_value";
            duration_minute_value.Size = new System.Drawing.Size(35, 23);
            duration_minute_value.TabIndex = 9;
            duration_minute_value.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            duration_minute_value.Value = Convert.ToDecimal(minuteRemaining);

            if (    controlRunning 
                    &&
                    (   givenStepStatus == HumidityStep.stepStatus.STATUS_RUNNING
                        ||
                        (givenStepStatus == HumidityStep.stepStatus.STATUS_COMPLETED && stepID < activeStepID)
                    )
                )
            {
                duration_minute_value.Enabled = false;
            }
            // 
            // duration_minute_label
            // 
            duration_minute_label_.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            duration_minute_label_.Location = new System.Drawing.Point(590, 0);
            duration_minute_label_.Margin = new System.Windows.Forms.Padding(0);
            duration_minute_label_.Name = controlPrefix_ + "duration_minute_label";
            duration_minute_label_.Size = new System.Drawing.Size(25, 25);
            duration_minute_label_.TabIndex = 10;
            duration_minute_label_.Text = "M";
            // 
            // duration_second_value
            // 
            duration_second_value.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            duration_second_value.Location = new System.Drawing.Point(635, 3);
            duration_second_value.Margin = new System.Windows.Forms.Padding(20, 3, 0, 3);
            duration_second_value.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            duration_second_value.Name = controlPrefix_ + "duration_second_value";
            duration_second_value.Size = new System.Drawing.Size(35, 23);
            duration_second_value.TabIndex = 11;
            duration_second_value.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            duration_second_value.Value = Convert.ToDecimal(secondRemaining);

            if (    controlRunning 
                    &&
                    (   givenStepStatus == HumidityStep.stepStatus.STATUS_RUNNING
                        ||
                        (givenStepStatus == HumidityStep.stepStatus.STATUS_COMPLETED && stepID < activeStepID)
                    )
                )
            {
                duration_second_value.Enabled = false;
            }
            // 
            // duration_second_label
            // 
            duration_second_label_.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            duration_second_label_.Location = new System.Drawing.Point(670, 0);
            duration_second_label_.Margin = new System.Windows.Forms.Padding(0);
            duration_second_label_.Name = controlPrefix_ + "duration_second_label";
            duration_second_label_.Size = new System.Drawing.Size(25, 25);
            duration_second_label_.TabIndex = 12;
            duration_second_label_.Text = "S";
            // 
            // fan speed
            // 
            fanSpeed.DecimalPlaces = 0;
            fanSpeed.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            fanSpeed.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            fanSpeed.Maximum = new decimal(new int[] {
            2400,
            0,
            0,
            0});
            fanSpeed.Location = new System.Drawing.Point(775, 3);
            fanSpeed.Margin = new System.Windows.Forms.Padding(80, 3, 3, 3);
            fanSpeed.Name = controlPrefix_ + "fanSpeed";
            fanSpeed.Size = new System.Drawing.Size(50, 23);
            fanSpeed.TabIndex = 13;
            fanSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            fanSpeed.Value = Convert.ToDecimal(targetFanSpeed);

            if (    controlRunning 
                    &&
                    (   givenStepStatus == HumidityStep.stepStatus.STATUS_RUNNING
                        ||
                        (givenStepStatus == HumidityStep.stepStatus.STATUS_COMPLETED && stepID < activeStepID)
                    )
                )
            {
                fanSpeed.Enabled = false;
            }
            // 
            // stepStatus
            // 
            stepStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            stepStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            stepStatus.FormattingEnabled = true;
            stepStatus.Location = new System.Drawing.Point(883, 3);
            stepStatus.Margin = new System.Windows.Forms.Padding(55, 3, 3, 3);
            stepStatus.Name = controlPrefix_ + "status";
            stepStatus.Size = new System.Drawing.Size(90, 24);
            stepStatus.TabIndex = 14;

            if (givenStepStatus != HumidityStep.stepStatus.STATUS_RUNNING)
            {
                stepStatus.Items.AddRange(new object[] {
                CONTROL_STEP_STATUS_COMPLETED,
                CONTROL_STEP_STATUS_WAITING});

                if (givenStepStatus == HumidityStep.stepStatus.STATUS_COMPLETED)
                {
                    stepStatus.Text = CONTROL_STEP_STATUS_COMPLETED;
                    stepStatus.SelectedValue = CONTROL_STEP_STATUS_COMPLETED; // This sets the default value of the drop down list...
                    stepStatus.SelectedItem = CONTROL_STEP_STATUS_COMPLETED; // ...and this makes it display the value

                    if (controlRunning && stepID < activeStepID)
                    {
                        stepStatus.Enabled = false;
                    }
                }
                else if (givenStepStatus == HumidityStep.stepStatus.STATUS_WAITING)
                {
                    stepStatus.Text = CONTROL_STEP_STATUS_WAITING;
                    stepStatus.SelectedValue = CONTROL_STEP_STATUS_WAITING; // This sets the default value of the drop down list...
                    stepStatus.SelectedItem = CONTROL_STEP_STATUS_WAITING; // ...and this makes it display the value
                }
            }
            else
            {
                stepStatus.Items.AddRange(new object[] {    CONTROL_STEP_STATUS_COMPLETED,
                                                            CONTROL_STEP_STATUS_RUNNING,
                                                            CONTROL_STEP_STATUS_WAITING});
                stepStatus.Text = CONTROL_STEP_STATUS_RUNNING;
                stepStatus.SelectedValue = CONTROL_STEP_STATUS_RUNNING; // This sets the default value of the drop down list...
                stepStatus.SelectedItem = CONTROL_STEP_STATUS_RUNNING; // ...and this makes it display the value
                stepStatus.Enabled = false;
            }
            // 
            // delete
            // 
            if (    !controlRunning
                    ||
                    (   controlRunning
                        &&  
                        (   givenStepStatus == HumidityStep.stepStatus.STATUS_WAITING
                            ||
                            (givenStepStatus == HumidityStep.stepStatus.STATUS_COMPLETED && stepID > activeStepID)
                        )
                    )
                )
            {
                deleteButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                deleteButton.Location = new System.Drawing.Point(1011, 3);
                deleteButton.Margin = new System.Windows.Forms.Padding(35, 3, 3, 3);
                deleteButton.Name = controlPrefix_ + "delete";
                deleteButton.Size = new System.Drawing.Size(59, 27);
                deleteButton.TabIndex = 15;
                deleteButton.Text = "Delete";
                deleteButton.UseVisualStyleBackColor = true;
                deleteButton.Enabled = true;
                deleteButton.Visible = true;
            }
            else
            {
                deleteButton.Enabled = false;
                deleteButton.Visible = false;
            }
            // 
            // skip
            // 
            if (controlRunning && givenStepStatus == HumidityStep.stepStatus.STATUS_RUNNING)
            {
                skipButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                skipButton.Location = new System.Drawing.Point(1011, 3);
                skipButton.Margin = new System.Windows.Forms.Padding(35, 3, 3, 3);
                skipButton.Name = controlPrefix_ + "skip";
                skipButton.Size = new System.Drawing.Size(59, 27);
                skipButton.TabIndex = 16;
                skipButton.Text = "Skip";
                skipButton.UseVisualStyleBackColor = true;

                // Disable the skip button under situations where user shouldn't press it
                if (controlPaused || skipControlStepWaiting || nextControlStepWaiting || pauseControlWaiting || endControlWaiting)
                {
                    skipButton.Enabled = false;
                }
            }
            else
            {
                skipButton.Enabled = false;
                skipButton.Visible = false;
            }

            // 
            // Finalize the layout
            // 
            this.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(humidity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(duration_day_value)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(duration_hour_value)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(duration_minute_value)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(duration_second_value)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(fanSpeed)).EndInit();

            //
            // Remove all mouse wheel events from numericUpDown and dropdown boxes. Sometimes, users will attempt to scroll while focused on a numericUpDown and accidentally change its value
            //
            stepType.MouseWheel                     += chill_MouseWheel;
            humidity.MouseWheel                     += chill_MouseWheel;
            duration_day_value.MouseWheel           += chill_MouseWheel;
            duration_hour_value.MouseWheel          += chill_MouseWheel;
            duration_minute_value.MouseWheel        += chill_MouseWheel;
            duration_second_value.MouseWheel        += chill_MouseWheel;
            fanSpeed.MouseWheel                     += chill_MouseWheel;
            stepStatus.MouseWheel                   += chill_MouseWheel;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        // Event handlers
        //
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        private void chill_MouseWheel(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
        }
    }// END HumidityStep_Control class def.
}
