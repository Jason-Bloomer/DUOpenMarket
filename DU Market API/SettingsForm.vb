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

    Public Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        Form1.Setting_SaveWindowLoc = CStr(CheckBox1.Checked)
    End Sub

    Private Sub CheckBox2_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox2.CheckedChanged
        Form1.Setting_SaveGridLayout = CStr(CheckBox2.Checked)
    End Sub
End Class