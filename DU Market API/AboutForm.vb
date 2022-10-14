Public Class AboutForm
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

    Private Sub AboutForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        UpdateThemeState()
    End Sub

    Public Sub UpdateThemeState()
        If Form1.Setting_ThemeState = 0 Then
            Me.BackColor = Color.FromArgb(255, 30, 36, 42)
            Panel1.BackgroundImage = My.Resources.loginbg
            Panel1.BackColor = Color.FromArgb(255, 30, 36, 42)
            Label22.ForeColor = Color.FromArgb(255, 224, 224, 224)
            Button8.BackColor = Color.FromArgb(255, 30, 36, 42)
            Button8.ForeColor = Color.FromArgb(255, 224, 224, 224)
            Label1.ForeColor = Color.Gray
            Label2.ForeColor = Color.FromArgb(255, 224, 224, 224)
            Label3.ForeColor = Color.FromArgb(255, 224, 224, 224)
            Label4.ForeColor = Color.FromArgb(255, 192, 192, 0)
            Label5.ForeColor = Color.FromArgb(255, 224, 224, 224)
            Label6.ForeColor = Color.FromArgb(255, 224, 224, 224)
            Button1.ForeColor = Color.FromArgb(255, 224, 224, 224)
            Button2.ForeColor = Color.FromArgb(255, 224, 224, 224)
        End If
        If Form1.Setting_ThemeState = 1 Then
            Me.BackColor = Color.FromArgb(255, 224, 224, 224)
            Panel1.BackgroundImage = My.Resources.loginbgneg
            Panel1.BackColor = Color.FromArgb(255, 224, 224, 224)
            Label22.ForeColor = Color.FromArgb(255, 30, 36, 42)
            Button8.BackColor = Color.FromArgb(255, 224, 224, 224)
            Button8.ForeColor = Color.FromArgb(255, 30, 36, 42)
            Label1.ForeColor = Color.Gray
            Label2.ForeColor = Color.FromArgb(255, 30, 36, 42)
            Label3.ForeColor = Color.FromArgb(255, 30, 36, 42)
            Label4.ForeColor = Color.FromArgb(255, 0, 50, 192)
            Label5.ForeColor = Color.FromArgb(255, 30, 36, 42)
            Label6.ForeColor = Color.FromArgb(255, 30, 36, 42)
            Button1.ForeColor = Color.FromArgb(255, 30, 36, 42)
            Button2.ForeColor = Color.FromArgb(255, 30, 36, 42)
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Process.Start("https://duopenmarket.com")
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Process.Start("https://github.com/Jason-Bloomer/DUOpenMarket")
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        Me.Close()
    End Sub

    Private Sub TitleBar_MouseUp(ByVal sender As Object, ByVal e As MouseEventArgs) Handles AboutTitleBar.MouseUp
        Go = False
        LeftSet = False
        TopSet = False
    End Sub

    Private Sub TitleBar_MouseDown(ByVal sender As Object, ByVal e As MouseEventArgs) Handles AboutTitleBar.MouseDown
        Go = True
        HoldLeft = (Control.MousePosition.X - Me.Location.X)
        HoldTop = (Control.MousePosition.Y - Me.Location.Y)
    End Sub

    Private Sub TitleBar_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs) Handles AboutTitleBar.MouseMove
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
End Class