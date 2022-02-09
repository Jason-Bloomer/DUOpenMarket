<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SettingsForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.SettingsTitleBar = New System.Windows.Forms.Panel()
        Me.Button8 = New System.Windows.Forms.Button()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.Label22 = New System.Windows.Forms.Label()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.LoadPrefsButton = New System.Windows.Forms.Button()
        Me.SavePrefsButton = New System.Windows.Forms.Button()
        Me.CheckBox2 = New System.Windows.Forms.CheckBox()
        Me.CheckBox1 = New System.Windows.Forms.CheckBox()
        Me.SettingsTitleBar.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'SettingsTitleBar
        '
        Me.SettingsTitleBar.BackColor = System.Drawing.Color.Transparent
        Me.SettingsTitleBar.Controls.Add(Me.Button8)
        Me.SettingsTitleBar.Controls.Add(Me.PictureBox1)
        Me.SettingsTitleBar.Controls.Add(Me.Label22)
        Me.SettingsTitleBar.Location = New System.Drawing.Point(0, 0)
        Me.SettingsTitleBar.Name = "SettingsTitleBar"
        Me.SettingsTitleBar.Size = New System.Drawing.Size(265, 35)
        Me.SettingsTitleBar.TabIndex = 25
        '
        'Button8
        '
        Me.Button8.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Button8.FlatAppearance.BorderSize = 0
        Me.Button8.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.Button8.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button8.ForeColor = System.Drawing.Color.Gray
        Me.Button8.Location = New System.Drawing.Point(220, 2)
        Me.Button8.Name = "Button8"
        Me.Button8.Size = New System.Drawing.Size(45, 30)
        Me.Button8.TabIndex = 3
        Me.Button8.Text = "X"
        Me.Button8.UseVisualStyleBackColor = True
        '
        'PictureBox1
        '
        Me.PictureBox1.BackgroundImage = Global.DU_Market_API.My.Resources.Resources.logo
        Me.PictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.PictureBox1.InitialImage = Global.DU_Market_API.My.Resources.Resources.logo
        Me.PictureBox1.Location = New System.Drawing.Point(3, 2)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(30, 30)
        Me.PictureBox1.TabIndex = 21
        Me.PictureBox1.TabStop = False
        '
        'Label22
        '
        Me.Label22.AutoSize = True
        Me.Label22.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label22.ForeColor = System.Drawing.Color.DarkGray
        Me.Label22.Location = New System.Drawing.Point(39, 10)
        Me.Label22.Name = "Label22"
        Me.Label22.Size = New System.Drawing.Size(160, 16)
        Me.Label22.TabIndex = 20
        Me.Label22.Text = "DUOpenMarket - Settings"
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.FromArgb(CType(CType(26, Byte), Integer), CType(CType(28, Byte), Integer), CType(CType(32, Byte), Integer))
        Me.Panel1.BackgroundImage = Global.DU_Market_API.My.Resources.Resources.loginbg
        Me.Panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.Panel1.Controls.Add(Me.GroupBox1)
        Me.Panel1.Location = New System.Drawing.Point(0, 35)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(267, 167)
        Me.Panel1.TabIndex = 24
        '
        'GroupBox1
        '
        Me.GroupBox1.BackColor = System.Drawing.Color.Transparent
        Me.GroupBox1.Controls.Add(Me.LoadPrefsButton)
        Me.GroupBox1.Controls.Add(Me.SavePrefsButton)
        Me.GroupBox1.Controls.Add(Me.CheckBox2)
        Me.GroupBox1.Controls.Add(Me.CheckBox1)
        Me.GroupBox1.ForeColor = System.Drawing.Color.Silver
        Me.GroupBox1.Location = New System.Drawing.Point(29, 21)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(201, 117)
        Me.GroupBox1.TabIndex = 1
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Preferences"
        '
        'LoadPrefsButton
        '
        Me.LoadPrefsButton.Enabled = False
        Me.LoadPrefsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.LoadPrefsButton.ForeColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.LoadPrefsButton.Location = New System.Drawing.Point(109, 76)
        Me.LoadPrefsButton.Name = "LoadPrefsButton"
        Me.LoadPrefsButton.Size = New System.Drawing.Size(75, 23)
        Me.LoadPrefsButton.TabIndex = 3
        Me.LoadPrefsButton.Text = "Load"
        Me.LoadPrefsButton.UseVisualStyleBackColor = True
        '
        'SavePrefsButton
        '
        Me.SavePrefsButton.Enabled = False
        Me.SavePrefsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.SavePrefsButton.ForeColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.SavePrefsButton.Location = New System.Drawing.Point(16, 76)
        Me.SavePrefsButton.Name = "SavePrefsButton"
        Me.SavePrefsButton.Size = New System.Drawing.Size(75, 23)
        Me.SavePrefsButton.TabIndex = 2
        Me.SavePrefsButton.Text = "Save"
        Me.SavePrefsButton.UseVisualStyleBackColor = True
        '
        'CheckBox2
        '
        Me.CheckBox2.AutoSize = True
        Me.CheckBox2.BackColor = System.Drawing.Color.Transparent
        Me.CheckBox2.ForeColor = System.Drawing.Color.Silver
        Me.CheckBox2.Location = New System.Drawing.Point(16, 42)
        Me.CheckBox2.Name = "CheckBox2"
        Me.CheckBox2.Size = New System.Drawing.Size(168, 17)
        Me.CheckBox2.TabIndex = 1
        Me.CheckBox2.Text = "Save Column Layout on Close"
        Me.CheckBox2.UseVisualStyleBackColor = False
        '
        'CheckBox1
        '
        Me.CheckBox1.AutoSize = True
        Me.CheckBox1.BackColor = System.Drawing.Color.Transparent
        Me.CheckBox1.ForeColor = System.Drawing.Color.Silver
        Me.CheckBox1.Location = New System.Drawing.Point(16, 19)
        Me.CheckBox1.Name = "CheckBox1"
        Me.CheckBox1.Size = New System.Drawing.Size(177, 17)
        Me.CheckBox1.TabIndex = 0
        Me.CheckBox1.Text = "Save Window Position on Close"
        Me.CheckBox1.UseVisualStyleBackColor = False
        '
        'SettingsForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(30, Byte), Integer), CType(CType(36, Byte), Integer), CType(CType(42, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(268, 203)
        Me.Controls.Add(Me.SettingsTitleBar)
        Me.Controls.Add(Me.Panel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "SettingsForm"
        Me.Text = "DUOpenMarket - Settings"
        Me.SettingsTitleBar.ResumeLayout(False)
        Me.SettingsTitleBar.PerformLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents SettingsTitleBar As Panel
    Friend WithEvents Button8 As Button
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents Label22 As Label
    Friend WithEvents Panel1 As Panel
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents CheckBox2 As CheckBox
    Friend WithEvents CheckBox1 As CheckBox
    Friend WithEvents LoadPrefsButton As Button
    Friend WithEvents SavePrefsButton As Button
End Class
