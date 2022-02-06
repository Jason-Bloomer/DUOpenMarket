Public Class AboutForm
    Dim Go As Boolean
    Dim LeftSet As Boolean
    Dim TopSet As Boolean
    Dim HoldLeft As Integer
    Dim HoldTop As Integer
    Dim OffLeft As Integer
    Dim OffTop As Integer
    Dim HoldWidth As Integer
    Dim HoldHeight As Integer
    Private Sub DiscordLoginButton_Click(sender As Object, e As EventArgs) Handles DiscordLoginButton.Click
        Process.Start("http://duopenmarket.xyz")
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
    End Sub

    Private Sub TitleBar_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs) Handles AboutTitleBar.MouseMove
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
End Class