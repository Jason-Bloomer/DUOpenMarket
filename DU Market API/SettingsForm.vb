Public Class SettingsForm
    Dim Go As Boolean
    Dim LeftSet As Boolean
    Dim TopSet As Boolean
    Dim HoldLeft As Integer
    Dim HoldTop As Integer
    Dim OffLeft As Integer
    Dim OffTop As Integer
    Dim HoldWidth As Integer
    Dim HoldHeight As Integer

    Private Sub SettingsForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadCurrentSettings()
    End Sub

    Private Sub LoadCurrentSettings()
        CheckBox1.Checked = CBool(Form1.Setting_SaveWindowLoc)
        CheckBox2.Checked = CBool(Form1.Setting_SaveGridLayout)
        CheckBox3.Checked = CBool(Form1.Setting_SaveWindowSz)
        CheckBox4.Checked = CBool(Form1.Setting_Processinbatch)
        NumericUpDown1.Value = CInt(Form1.Setting_LogCheckTimer)
        UpdateThemeState()
    End Sub

    Public Sub UpdateThemeState()
        If Form1.ThemeState = 0 Then
            Me.BackColor = Color.FromArgb(255, 30, 36, 42)
            Panel1.BackgroundImage = My.Resources.loginbg
            Panel1.BackColor = Color.FromArgb(255, 30, 36, 42)
            Label22.ForeColor = Color.FromArgb(255, 224, 224, 224)
            Label1.ForeColor = Color.FromArgb(255, 224, 224, 224)
            Button1.BackColor = Color.FromArgb(255, 30, 36, 42)
            Button1.ForeColor = Color.FromArgb(255, 224, 224, 224)
            Button2.BackColor = Color.FromArgb(255, 30, 36, 42)
            Button2.ForeColor = Color.FromArgb(255, 224, 224, 224)
            Button8.BackColor = Color.FromArgb(255, 30, 36, 42)
            Button8.ForeColor = Color.FromArgb(255, 224, 224, 224)
            GroupBox1.ForeColor = Color.FromArgb(255, 224, 224, 224)
            CheckBox1.ForeColor = Color.FromArgb(255, 224, 224, 224)
            CheckBox2.ForeColor = Color.FromArgb(255, 224, 224, 224)
            CheckBox3.ForeColor = Color.FromArgb(255, 224, 224, 224)
            CheckBox4.ForeColor = Color.FromArgb(255, 224, 224, 224)
            NumericUpDown1.ForeColor = Color.FromArgb(255, 224, 224, 224)
            NumericUpDown1.BackColor = Color.FromArgb(255, 30, 36, 42)
        End If
        If Form1.ThemeState = 1 Then
            Me.BackColor = Color.FromArgb(255, 224, 224, 224)
            Panel1.BackgroundImage = My.Resources.loginbgneg
            Panel1.BackColor = Color.FromArgb(255, 224, 224, 224)
            Label22.ForeColor = Color.FromArgb(255, 30, 36, 42)
            Label1.ForeColor = Color.FromArgb(255, 30, 36, 42)
            Button1.BackColor = Color.FromArgb(255, 224, 224, 224)
            Button1.ForeColor = Color.FromArgb(255, 30, 36, 42)
            Button2.BackColor = Color.FromArgb(255, 224, 224, 224)
            Button2.ForeColor = Color.FromArgb(255, 30, 36, 42)
            Button8.BackColor = Color.FromArgb(255, 224, 224, 224)
            Button8.ForeColor = Color.FromArgb(255, 30, 36, 42)
            GroupBox1.ForeColor = Color.FromArgb(255, 30, 36, 42)
            CheckBox1.ForeColor = Color.FromArgb(255, 30, 36, 42)
            CheckBox2.ForeColor = Color.FromArgb(255, 30, 36, 42)
            CheckBox3.ForeColor = Color.FromArgb(255, 30, 36, 42)
            CheckBox4.ForeColor = Color.FromArgb(255, 30, 36, 42)
            NumericUpDown1.ForeColor = Color.FromArgb(255, 30, 36, 42)
            NumericUpDown1.BackColor = Color.FromArgb(255, 224, 224, 224)
        End If
    End Sub

    Private Sub TitleBar_MouseUp(ByVal sender As Object, ByVal e As MouseEventArgs) Handles SettingsTitleBar.MouseUp
        Go = False
        LeftSet = False
        TopSet = False
    End Sub

    Private Sub TitleBar_MouseDown(ByVal sender As Object, ByVal e As MouseEventArgs) Handles SettingsTitleBar.MouseDown
        Go = True
    End Sub

    Private Sub TitleBar_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs) Handles SettingsTitleBar.MouseMove
        If Go = True Then
            HoldLeft = (Control.MousePosition.X)
            HoldTop = (Control.MousePosition.Y)
            If TopSet = False Then
                OffTop = HoldTop - sender.Parent.Location.Y
                TopSet = True
            End If
            If LeftSet = False Then
                OffLeft = HoldLeft - sender.Parent.Location.X
                LeftSet = True
            End If
            Dim newpoint As New Point
            newpoint.X = HoldLeft - OffLeft
            newpoint.Y = HoldTop - OffTop
            sender.Parent.Location = newpoint
            sender.Parent.Refresh()
        End If
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        Me.Close()
    End Sub

    Public Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        Form1.Setting_SaveWindowLoc = CStr(CheckBox1.Checked)
    End Sub

    Private Sub CheckBox2_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox2.CheckedChanged
        Form1.Setting_SaveGridLayout = CStr(CheckBox2.Checked)
    End Sub

    Private Sub CheckBox3_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox3.CheckedChanged
        Form1.Setting_SaveWindowSz = CStr(CheckBox3.Checked)
    End Sub

    Private Sub CheckBox4_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox4.CheckedChanged
        Form1.Setting_Processinbatch = CStr(CheckBox4.Checked)
    End Sub

    Private Sub NumericUpDown1_ValueChanged(sender As Object, e As EventArgs) Handles NumericUpDown1.ValueChanged
        Form1.Setting_LogCheckTimer = CStr(NumericUpDown1.Value)
        Form1.LogFileCheckTimer.Interval = NumericUpDown1.Value
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Form1.LoadPrefsFromIni()
        LoadCurrentSettings()
        AboutForm.UpdateThemeState()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Form1.SavePrefsToIni()
    End Sub
End Class