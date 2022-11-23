Public Class SettingsForm
    Dim Go As Boolean
    Dim LeftSet As Boolean
    Dim TopSet As Boolean
    Dim deltaX As Integer
    Dim deltaY As Integer
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
        If Form1.Setting_SaveWindowLoc IsNot Nothing And Form1.Setting_SaveWindowLoc IsNot "" Then
            CheckBox1.Checked = CBool(Form1.Setting_SaveWindowLoc)
        End If
        If Form1.Setting_SaveGridLayout IsNot Nothing And Form1.Setting_SaveGridLayout IsNot "" Then
            CheckBox2.Checked = CBool(Form1.Setting_SaveGridLayout)
        End If
        If Form1.Setting_SaveWindowSz IsNot Nothing And Form1.Setting_SaveWindowSz IsNot "" Then
            CheckBox3.Checked = CBool(Form1.Setting_SaveWindowSz)
        End If
        If Form1.Setting_Processinbatch IsNot Nothing And Form1.Setting_Processinbatch IsNot "" Then
            CheckBox4.Checked = CBool(Form1.Setting_Processinbatch)
        End If
        If Form1.Setting_BackgroundWorker IsNot Nothing And Form1.Setting_BackgroundWorker IsNot "" Then
            CheckBox5.Checked = CBool(Form1.Setting_BackgroundWorker)
        End If
        If Form1.Setting_LogCheckTimer IsNot Nothing And Form1.Setting_LogCheckTimer IsNot "" Then
            NumericUpDown1.Value = CInt(Form1.Setting_LogCheckTimer)
        End If
        UpdateThemeState()
    End Sub

    Public Sub UpdateThemeState()
        If Form1.Setting_ThemeState = 0 Then
            Button3.Text = "Theme: Dark"
        End If
        If Form1.Setting_ThemeState = 1 Then
            Button3.Text = "Theme: Light"
        End If
        If Form1.Setting_ThemeState = 2 Then
            Button3.Text = "Theme: Custom"
        End If
        Me.BackColor = Form1.BackgroundColor1
        Panel1.BackgroundImage = Nothing
        Panel1.BackColor = Form1.BackgroundColor1
        Label22.ForeColor = Form1.ForegroundColor1
        Label1.ForeColor = Form1.ForegroundColor1
        Button1.BackColor = Form1.BackgroundColor1
        Button1.ForeColor = Form1.ForegroundColor1
        Button2.BackColor = Form1.BackgroundColor1
        Button2.ForeColor = Form1.ForegroundColor1
        Button3.BackColor = Form1.BackgroundColor1
        Button3.ForeColor = Form1.ForegroundColor1
        Button8.BackColor = Form1.BackgroundColor1
        Button8.ForeColor = Form1.ForegroundColor1
        GroupBox1.ForeColor = Form1.ForegroundColor1
        CheckBox1.ForeColor = Form1.ForegroundColor1
        CheckBox2.ForeColor = Form1.ForegroundColor1
        CheckBox3.ForeColor = Form1.ForegroundColor1
        CheckBox4.ForeColor = Form1.ForegroundColor1
        CheckBox5.ForeColor = Form1.ForegroundColor1
        NumericUpDown1.ForeColor = Form1.ForegroundColor1
        NumericUpDown1.BackColor = Form1.BackgroundColor1
    End Sub

    Private Sub TitleBar_MouseUp(ByVal sender As Object, ByVal e As MouseEventArgs) Handles SettingsTitleBar.MouseUp
        Go = False
        LeftSet = False
        TopSet = False
    End Sub

    Private Sub TitleBar_MouseDown(ByVal sender As Object, ByVal e As MouseEventArgs) Handles SettingsTitleBar.MouseDown
        Go = True
        HoldLeft = (Control.MousePosition.X - Me.Location.X)
        HoldTop = (Control.MousePosition.Y - Me.Location.Y)
    End Sub

    Private Sub TitleBar_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs) Handles SettingsTitleBar.MouseMove
        If Go = True Then
            deltaX = Control.MousePosition.X - HoldLeft
            deltaY = Control.MousePosition.Y - HoldTop

            OffLeft = deltaX
            OffTop = deltaY

            Dim newpoint As New Point
            newpoint.X = OffLeft
            newpoint.Y = OffTop

            Me.Location = newpoint
            Me.Refresh()
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

    Private Sub CheckBox5_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox5.CheckedChanged
        Form1.Setting_BackgroundWorker = CStr(CheckBox5.Checked)
    End Sub

    Private Sub NumericUpDown1_ValueChanged(sender As Object, e As EventArgs) Handles NumericUpDown1.ValueChanged
        Form1.Setting_LogCheckTimer = CStr(NumericUpDown1.Value)
        Form1.LogFileCheckTimer.Interval = NumericUpDown1.Value
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Form1.LoadPrefsFromIni()
        LoadCurrentSettings()
        UpdateThemeState()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Form1.SavePrefsToIni()
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If Form1.Setting_ThemeState = 0 Then
            Form1.Setting_ThemeState = 1
            Form1.SetLightTheme()
        Else
            If Form1.Setting_ThemeState = 1 Then
                Form1.Setting_ThemeState = 2
                Form1.SetCustomTheme()
            Else
                Form1.Setting_ThemeState = 0
                Form1.SetDarkTheme()
            End If
        End If
        AboutForm.UpdateThemeState()
        UpdateThemeState()
    End Sub
End Class