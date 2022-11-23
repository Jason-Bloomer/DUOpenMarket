Option Explicit On
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Environment
Imports System.Net
Imports System.Threading

Public Class Form1

    '########## GUI/MISC VARS ##########
    Dim showhlp = False
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
    Private _LastOffset As Long

    Dim WindowMaximizedState As Boolean = False
    Dim WindowNormalBoundsX As Integer = 1280
    Dim WindowNormalBoundsY As Integer = 760
    Dim WindowSavedPosX As Integer = 50
    Dim WindowSavedPosY As Integer = 50
    Dim WindowSavedBoundsX As Integer = 1280
    Dim WindowSavedBoundsY As Integer = 760
    Dim ShowDevPanel As Boolean = False

    Dim API_Client_Version As String = "1.61.3"
    Dim API_Available_Version As String
    Dim API_Connected As Boolean = False
    Dim API_LogfileDirectory As String = GetFolderPath(SpecialFolder.LocalApplicationData) & "\NQ\DualUniverse\log"
    Dim API_LogFile As String = ""
    Dim API_Access_Token As String = ""
    Dim API_Last_Log_Processed As String = ""
    Dim API_Log_Queue As New List(Of LogInfoStructure)
    Dim API_Market_Table As New DataTable
    Dim API_Buy_Orders As New DataTable
    Dim API_Sell_Orders As New DataTable
    Dim API_Buy_Orders_UI As New DataTable
    Dim API_Sell_Orders_UI As New DataTable
    Dim SortedColumnIndex1 As Integer = 0
    Dim SortedColumnDirection1 As System.ComponentModel.ListSortDirection
    Dim SortedColumnIndex2 As Integer = 0
    Dim SortedColumnDirection2 As System.ComponentModel.ListSortDirection

    Dim ListenerStarted As Boolean = False
    Dim API_Discord_Auth_Code As String
    Dim API_Discord_Auth_State As String
    Dim API_Discord_Allow_New_Auth As Boolean = False
    Dim Discord_Login_Window_Handle As Process

    Dim LastFileModified As String = ""
    Dim NumberofModifications As Integer = 0
    Dim LogfileLastOffset As Long
    Dim LastItemId As String = ""
    Dim UniqueItemIds As New List(Of String)
    Dim NumberOfCreates As Integer = 0
    Dim NumberOfReads As Integer = 0
    Dim NumberOfUpdates As Integer = 0
    Dim NumberOfDeletes As Integer = 0
    Dim NumberOfHistories As Integer = 0
    Dim ShowRawData As Boolean = False
    Private Shared OperationTimerLocker As Object = New Object()

    Dim CurrentBatchList As New List(Of String)
    Dim CurrentBatchDeleteList As New List(Of String)
    Dim API_Batch_Orders As New List(Of LogInfoStructure)
    Dim API_Sanitized_Request_Queue As New List(Of APIRequestQueueStructure)

    Dim SearchUserTyping As Boolean = False
    Dim ShowFilters As Boolean = False
    Dim FilterPriceMin As Long
    Dim FilterPriceMax As Long
    Dim FilterQuantityMin As Long
    Dim FilterQuantityMax As Long
    Dim FilterMarketList As New List(Of FilterMarketListStructure)
    Dim FilterItemList As New List(Of FilterItemListStructure)
    Dim CenterUXPanelMode As Integer = 1
    Dim ShowBookmarks As Boolean = False
    Dim Bookmarks As New List(Of String)

    Dim currentdate As Date
    Dim CurrYear As String
    Dim CurrMonth As String
    Dim CurrDay As String
    Dim CurrHour As String
    Dim CurrMin As String
    Dim CurrSec As String

    Dim TempResponse As String

    Public Setting_ThemeState As Integer = 0
    Public Setting_SaveWindowLoc As String = "True"
    Public Setting_SaveWindowSz As String = "True"
    Public Setting_SaveGridLayout As String = "True"
    Public Setting_Processinbatch As String = "True"
    Public Setting_LogCheckTimer As String = "60000"
    Public Setting_BackgroundWorker As String = "True"

    Dim savedSellOrdrGridCol1W As String
    Dim savedSellOrdrGridCol2W As String
    Dim savedSellOrdrGridCol3W As String
    Dim savedSellOrdrGridCol4W As String
    Dim savedSellOrdrGridCol5W As String
    Dim savedBuyOrdrGridCol1W As String
    Dim savedBuyOrdrGridCol2W As String
    Dim savedBuyOrdrGridCol3W As String
    Dim savedBuyOrdrGridCol4W As String
    Dim savedBuyOrdrGridCol5W As String

    Dim HistEntries As Integer = 8

    Dim EconomyStat_Buy_High As Double = 0
    Dim EconomyStat_Buy_Avg As Double = 0
    Dim EconomyStat_Buy_Low As Double = 0
    Dim EconomyStat_Buy_Vol As Double = 0
    Dim EconomyStat_Buy_Total As Double = 0

    Dim EconomyStat_Sell_High As Double = 0
    Dim EconomyStat_Sell_Avg As Double = 0
    Dim EconomyStat_Sell_Low As Double = 0
    Dim EconomyStat_Sell_Vol As Double = 0
    Dim EconomyStat_Sell_Total As Double = 0

    'Theme Default Values
    Public BackgroundColor1 As Color = Color.FromArgb(255, 30, 36, 42)
    Public BackgroundColor2 As Color = Color.FromArgb(255, 50, 56, 62)
    Public BackgroundColor3 As Color = Color.FromArgb(255, 26, 28, 32)

    Public ForegroundColor1 As Color = Color.FromArgb(255, 224, 224, 224)
    Public ForegroundColor2 As Color = Color.FromArgb(255, 165, 165, 165)
    Public ForegroundColor3 As Color = Color.FromArgb(255, 125, 125, 125)

    Public GridColor1 As Color = Color.FromArgb(255, 125, 125, 125)
    Public GridBGColor1 As Color = Color.FromArgb(255, 26, 28, 32)
    Public GridBGColor2 As Color = Color.FromArgb(255, 26, 38, 42)
    Public GridSelectColor1 As Color = Color.FromArgb(255, 28, 74, 92)

    Public HistGridColor As Color = Color.FromArgb(255, 255, 255, 255)
    Public HistBuyColor1 As Color = Color.FromArgb(128, 0, 32, 85)
    Public HistBuyColor2 As Color = Color.FromArgb(128, 0, 128, 188)
    Public HistBuyColor3 As Color = Color.FromArgb(255, 0, 32, 128)
    Public HistSellColor1 As Color = Color.FromArgb(128, 148, 148, 148)
    Public HistSellColor2 As Color = Color.FromArgb(128, 215, 215, 215)
    Public HistSellColor3 As Color = Color.FromArgb(255, 255, 255, 255)


    Public CustomBackgroundColor1 As Color = Color.FromArgb(255, 30, 36, 42)
    Public CustomBackgroundColor2 As Color = Color.FromArgb(255, 50, 56, 62)
    Public CustomBackgroundColor3 As Color = Color.FromArgb(255, 26, 28, 32)

    Public CustomForegroundColor1 As Color = Color.FromArgb(255, 224, 224, 224)
    Public CustomForegroundColor2 As Color = Color.FromArgb(255, 165, 165, 165)
    Public CustomForegroundColor3 As Color = Color.FromArgb(255, 125, 125, 125)

    Public CustomGridColor1 As Color = Color.FromArgb(255, 125, 125, 125)
    Public CustomGridBGColor1 As Color = Color.FromArgb(255, 26, 28, 32)
    Public CustomGridBGColor2 As Color = Color.FromArgb(255, 26, 38, 42)
    Public CustomGridSelectColor1 As Color = Color.FromArgb(255, 28, 74, 92)

    Public CustomHistGridColor As Color = Color.FromArgb(255, 255, 255, 255)
    Public CustomHistBuyColor1 As Color = Color.FromArgb(128, 0, 32, 85)
    Public CustomHistBuyColor2 As Color = Color.FromArgb(128, 0, 128, 188)
    Public CustomHistBuyColor3 As Color = Color.FromArgb(255, 0, 32, 128)
    Public CustomHistSellColor1 As Color = Color.FromArgb(128, 148, 148, 148)
    Public CustomHistSellColor2 As Color = Color.FromArgb(128, 215, 215, 215)
    Public CustomHistSellColor3 As Color = Color.FromArgb(255, 255, 255, 255)

    '############################## - Structure Definitions - ##############################
    Structure LogInfoStructure
        Dim marketid As String
        Dim orderid As String
        Dim itemtype As String
        Dim quantity As String
        Dim expdate As String
        Dim lastupdate As String
        Dim price As String
    End Structure

    Structure FilterMarketListStructure
        Dim Name As String
        Dim Text As String
    End Structure

    Structure FilterItemListStructure
        Dim Name As String
        Dim Text As String
    End Structure

    Structure BookmarkStructure
        Dim Name As String
        Dim Id As String
        Dim Market As String

    End Structure

    Structure APIRequestQueueStructure
        Dim type As String
        Dim data As String
    End Structure

    '############################## - DLL Imports - ##############################
    <DllImport("kernel32")>
    Private Shared Function GetPrivateProfileString(ByVal section As String, ByVal key As String, ByVal def As String, ByVal retVal As StringBuilder, ByVal size As Integer, ByVal filePath As String) As Integer
    End Function

    <DllImport("kernel32")>
    Private Shared Function WritePrivateProfileString(ByVal lpSectionName As String, ByVal lpKeyName As String, ByVal lpString As String, ByVal lpFileName As String) As Long
    End Function

    '############################## - Configuration File Functions - ##############################
    Public Function GetIniValue(section As String, key As String, filename As String, Optional defaultValue As String = "") As String
        Dim sb As New StringBuilder(500)
        If GetPrivateProfileString(section, key, defaultValue, sb, sb.Capacity, filename) > 0 Then
            Return sb.ToString
        Else
            Return defaultValue
        End If
    End Function

    Private Function SetIniValue(section As String, key As String, filename As String, Optional defaultValue As String = "") As String
        Dim sb As New StringBuilder(500)
        Try
            WritePrivateProfileString(section, key, defaultValue, filename)
            Return True
            Exit Try
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Sub LoadPrefsFromIni()
        If File.Exists(My.Application.Info.DirectoryPath & "\DUOMsettings.ini") = True Then
            Dim savedWindowState As String
            Dim savedWindowLocX As String
            Dim savedWindowLocY As String
            Dim savedWindowSizeW As String
            Dim savedWindowSizeH As String
            Dim savedAbtWindowLocX As String
            Dim savedAbtWindowLocY As String
            Dim savedSetWindowLocX As String
            Dim savedSetWindowLocY As String
            Setting_SaveWindowSz = GetIniValue("Application", "SaveWindowSz", My.Application.Info.DirectoryPath & "\DUOMsettings.ini")
            Setting_SaveWindowLoc = GetIniValue("Application", "SaveWindowLoc", My.Application.Info.DirectoryPath & "\DUOMsettings.ini")
            Setting_SaveGridLayout = GetIniValue("Application", "SaveGridLayout", My.Application.Info.DirectoryPath & "\DUOMsettings.ini")
            Setting_Processinbatch = GetIniValue("Application", "BatchProcessing", My.Application.Info.DirectoryPath & "\DUOMsettings.ini")
            If Setting_SaveWindowLoc = "True" Then
                savedWindowLocX = GetIniValue("Application", "WindowLocX", My.Application.Info.DirectoryPath & "\DUOMsettings.ini")
                savedWindowLocY = GetIniValue("Application", "WindowLocY", My.Application.Info.DirectoryPath & "\DUOMsettings.ini")
                savedAbtWindowLocX = GetIniValue("Application", "AbtWindowLocX", My.Application.Info.DirectoryPath & "\DUOMsettings.ini")
                savedAbtWindowLocY = GetIniValue("Application", "AbtWindowLocY", My.Application.Info.DirectoryPath & "\DUOMsettings.ini")
                savedSetWindowLocX = GetIniValue("Application", "SetWindowLocX", My.Application.Info.DirectoryPath & "\DUOMsettings.ini")
                savedSetWindowLocY = GetIniValue("Application", "SetWindowLocY", My.Application.Info.DirectoryPath & "\DUOMsettings.ini")
                If savedWindowLocX IsNot "" And savedWindowLocX IsNot Nothing Then
                    If savedWindowLocY IsNot "" And savedWindowLocY IsNot Nothing Then
                        Dim newpoint As New Point
                        newpoint.X = CInt(savedWindowLocX)
                        newpoint.Y = CInt(savedWindowLocY)
                        Me.Location = newpoint
                    End If
                End If
                If savedAbtWindowLocX IsNot "" And savedSetWindowLocX IsNot Nothing Then
                    If savedAbtWindowLocY IsNot "" And savedAbtWindowLocY IsNot Nothing Then
                        Dim newpoint As New Point
                        newpoint.X = CInt(savedAbtWindowLocX)
                        newpoint.Y = CInt(savedAbtWindowLocY)
                        AboutForm.Location = newpoint
                    End If
                End If
                If savedSetWindowLocX IsNot "" And savedSetWindowLocX IsNot Nothing Then
                    If savedSetWindowLocY IsNot "" And savedAbtWindowLocY IsNot Nothing Then
                        Dim newpoint As New Point
                        newpoint.X = CInt(savedSetWindowLocX)
                        newpoint.Y = CInt(savedSetWindowLocY)
                        SettingsForm.Location = newpoint
                    End If
                End If
            End If
            If Setting_SaveWindowSz = "True" Then
                savedWindowState = GetIniValue("Application", "WindowState", My.Application.Info.DirectoryPath & "\DUOMsettings.ini")
                If savedWindowState IsNot "" And savedWindowState IsNot Nothing Then
                    WindowMaximizedState = CBool(savedWindowState)
                End If
                savedWindowSizeW = GetIniValue("Application", "WindowSizeW", My.Application.Info.DirectoryPath & "\DUOMsettings.ini")
                savedWindowSizeH = GetIniValue("Application", "WindowSizeH", My.Application.Info.DirectoryPath & "\DUOMsettings.ini")
                If savedWindowSizeW IsNot "" And savedWindowSizeW IsNot Nothing Then
                    If savedWindowSizeH IsNot "" And savedWindowSizeH IsNot Nothing Then
                        Dim newbnds As Size = New Size()
                        newbnds.Width = CInt(savedWindowSizeW)
                        newbnds.Height = CInt(savedWindowSizeH)
                        Me.Size = newbnds
                        WindowSavedBoundsX = savedWindowSizeW
                        WindowSavedBoundsY = savedWindowSizeH
                    End If
                End If
            End If
            If Setting_SaveGridLayout = "True" Then
                savedSellOrdrGridCol1W = GetIniValue("Application", "SellOrdrGridCol1W", My.Application.Info.DirectoryPath & "\DUOMsettings.ini")
                savedSellOrdrGridCol2W = GetIniValue("Application", "SellOrdrGridCol2W", My.Application.Info.DirectoryPath & "\DUOMsettings.ini")
                savedSellOrdrGridCol3W = GetIniValue("Application", "SellOrdrGridCol3W", My.Application.Info.DirectoryPath & "\DUOMsettings.ini")
                savedSellOrdrGridCol4W = GetIniValue("Application", "SellOrdrGridCol4W", My.Application.Info.DirectoryPath & "\DUOMsettings.ini")
                savedSellOrdrGridCol5W = GetIniValue("Application", "SellOrdrGridCol5W", My.Application.Info.DirectoryPath & "\DUOMsettings.ini")
                savedBuyOrdrGridCol1W = GetIniValue("Application", "BuyOrdrGridCol1W", My.Application.Info.DirectoryPath & "\DUOMsettings.ini")
                savedBuyOrdrGridCol2W = GetIniValue("Application", "BuyOrdrGridCol2W", My.Application.Info.DirectoryPath & "\DUOMsettings.ini")
                savedBuyOrdrGridCol3W = GetIniValue("Application", "BuyOrdrGridCol3W", My.Application.Info.DirectoryPath & "\DUOMsettings.ini")
                savedBuyOrdrGridCol4W = GetIniValue("Application", "BuyOrdrGridCol4W", My.Application.Info.DirectoryPath & "\DUOMsettings.ini")
                savedBuyOrdrGridCol5W = GetIniValue("Application", "BuyOrdrGridCol5W", My.Application.Info.DirectoryPath & "\DUOMsettings.ini")
            End If
            Dim themetest As String = GetIniValue("Application", "ThemeState", My.Application.Info.DirectoryPath & "\DUOMsettings.ini")
            If themetest IsNot "" And themetest IsNot Nothing Then
                If CInt(themetest) = 0 Then
                    Setting_ThemeState = CInt(themetest)
                    SetDarkTheme()
                ElseIf CInt(themetest) = 1 Then
                    Setting_ThemeState = CInt(themetest)
                    SetLightTheme()
                End If
            End If
            Dim logchecksettingtest As String = GetIniValue("Application", "LogFileCheckTimerInterval", My.Application.Info.DirectoryPath & "\DUOMsettings.ini")
            If logchecksettingtest IsNot "" And logchecksettingtest IsNot Nothing Then
                Setting_LogCheckTimer = logchecksettingtest
                LogFileCheckTimer.Interval = CInt(Setting_LogCheckTimer)
                SettingsForm.NumericUpDown1.Value = CInt(Setting_LogCheckTimer)
            End If
            Dim bgwrkrsettingtest As String = GetIniValue("Application", "BackgroundWorker", My.Application.Info.DirectoryPath & "\DUOMsettings.ini")
            If bgwrkrsettingtest IsNot "" And bgwrkrsettingtest IsNot Nothing Then
                Setting_BackgroundWorker = bgwrkrsettingtest
                'LogFileCheckTimer.Interval = CInt(Setting_LogCheckTimer)
                'SettingsForm.NumericUpDown1.Value = CInt(Setting_LogCheckTimer)
            End If
        Else
            'set default values
            SetIniValue("Application", "SaveWindowSz", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(Setting_SaveWindowLoc))
            SetIniValue("Application", "SaveWindowLoc", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(Setting_SaveWindowLoc))
            SetIniValue("Application", "SaveGridLayout", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(Setting_SaveGridLayout))
            SetIniValue("Application", "BatchProcessing", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(Setting_Processinbatch))
            If Setting_SaveWindowLoc = "True" Then
                SetIniValue("Application", "WindowLocX", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(Me.Location.X))
                SetIniValue("Application", "WindowLocY", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(Me.Location.Y))
                SetIniValue("Application", "AbtWindowLocX", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(AboutForm.Location.X))
                SetIniValue("Application", "AbtWindowLocY", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(AboutForm.Location.Y))
                SetIniValue("Application", "SetWindowLocX", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(SettingsForm.Location.X))
                SetIniValue("Application", "SetWindowLocY", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(SettingsForm.Location.Y))
            End If
            If Setting_SaveWindowSz = "True" Then
                SetIniValue("Application", "WindowState", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(WindowMaximizedState))
                SetIniValue("Application", "WindowSizeW", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(MainPanel.Parent.Size.Width))
                SetIniValue("Application", "WindowSizeH", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(MainPanel.Parent.Size.Height))
            End If
            If Setting_SaveGridLayout = "True" Then
                SetIniValue("Application", "SellOrdrGridCol1W", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(200))
                SetIniValue("Application", "SellOrdrGridCol2W", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(200))
                SetIniValue("Application", "SellOrdrGridCol3W", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(200))
                SetIniValue("Application", "SellOrdrGridCol4W", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(200))
                SetIniValue("Application", "SellOrdrGridCol5W", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(200))
                SetIniValue("Application", "BuyOrdrGridCol1W", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(200))
                SetIniValue("Application", "BuyOrdrGridCol2W", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(200))
                SetIniValue("Application", "BuyOrdrGridCol3W", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(200))
                SetIniValue("Application", "BuyOrdrGridCol4W", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(200))
                SetIniValue("Application", "BuyOrdrGridCol5W", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(200))
            End If
            SetIniValue("Application", "ThemeState", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(Setting_ThemeState))
            SetIniValue("Application", "LogFileCheckTimerInterval", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(Setting_LogCheckTimer))
            SetIniValue("Application", "BackgroundWorker", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(Setting_BackgroundWorker))
        End If
    End Sub

    Public Sub SavePrefsToIni()
        SetIniValue("Application", "SaveWindowSz", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(Setting_SaveWindowLoc))
        SetIniValue("Application", "SaveWindowLoc", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(Setting_SaveWindowLoc))
        SetIniValue("Application", "SaveGridLayout", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(Setting_SaveGridLayout))
        SetIniValue("Application", "BatchProcessing", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(Setting_Processinbatch))
        If Setting_SaveWindowLoc = "True" Then
            SetIniValue("Application", "WindowLocX", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(Me.Location.X))
            SetIniValue("Application", "WindowLocY", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(Me.Location.Y))
            SetIniValue("Application", "AbtWindowLocX", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(AboutForm.Location.X))
            SetIniValue("Application", "AbtWindowLocY", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(AboutForm.Location.Y))
            SetIniValue("Application", "SetWindowLocX", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(SettingsForm.Location.X))
            SetIniValue("Application", "SetWindowLocY", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(SettingsForm.Location.Y))
        End If
        If Setting_SaveWindowSz = "True" Then
            SetIniValue("Application", "WindowState", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(WindowMaximizedState))
            SetIniValue("Application", "WindowSizeW", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(MainPanel.Parent.Size.Width))
            SetIniValue("Application", "WindowSizeH", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(MainPanel.Parent.Size.Height))
        End If
        If Setting_SaveGridLayout = "True" Then
            SetIniValue("Application", "SellOrdrGridCol1W", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(SellOrderGridViewRaw.Columns(0).Width))
            SetIniValue("Application", "SellOrdrGridCol2W", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(SellOrderGridViewRaw.Columns(1).Width))
            SetIniValue("Application", "SellOrdrGridCol3W", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(SellOrderGridViewRaw.Columns(2).Width))
            SetIniValue("Application", "SellOrdrGridCol4W", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(SellOrderGridViewRaw.Columns(3).Width))
            SetIniValue("Application", "SellOrdrGridCol5W", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(SellOrderGridViewRaw.Columns(4).Width))
            SetIniValue("Application", "BuyOrdrGridCol1W", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(BuyOrderGridViewRaw.Columns(0).Width))
            SetIniValue("Application", "BuyOrdrGridCol2W", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(BuyOrderGridViewRaw.Columns(1).Width))
            SetIniValue("Application", "BuyOrdrGridCol3W", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(BuyOrderGridViewRaw.Columns(2).Width))
            SetIniValue("Application", "BuyOrdrGridCol4W", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(BuyOrderGridViewRaw.Columns(3).Width))
            SetIniValue("Application", "BuyOrdrGridCol5W", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(BuyOrderGridViewRaw.Columns(4).Width))
        End If
        SetIniValue("Application", "ThemeState", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(Setting_ThemeState))
        SetIniValue("Application", "LogFileCheckTimerInterval", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(Setting_LogCheckTimer))
        SetIniValue("Application", "BackgroundWorker", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(Setting_BackgroundWorker))
    End Sub

    Public Sub SaveCustomTheme()
        SetIniValue("BackgroundColor1", "R", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini", CStr(CustomBackgroundColor1.R))
        SetIniValue("BackgroundColor1", "G", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini", CStr(CustomBackgroundColor1.G))
        SetIniValue("BackgroundColor1", "B", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini", CStr(CustomBackgroundColor1.B))

        SetIniValue("BackgroundColor2", "R", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini", CStr(CustomBackgroundColor2.R))
        SetIniValue("BackgroundColor2", "G", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini", CStr(CustomBackgroundColor2.G))
        SetIniValue("BackgroundColor2", "B", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini", CStr(CustomBackgroundColor2.B))

        SetIniValue("BackgroundColor3", "R", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini", CStr(CustomBackgroundColor3.R))
        SetIniValue("BackgroundColor3", "G", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini", CStr(CustomBackgroundColor3.G))
        SetIniValue("BackgroundColor3", "B", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini", CStr(CustomBackgroundColor3.B))

        SetIniValue("ForegroundColor1", "R", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini", CStr(CustomForegroundColor1.R))
        SetIniValue("ForegroundColor1", "G", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini", CStr(CustomForegroundColor1.G))
        SetIniValue("ForegroundColor1", "B", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini", CStr(CustomForegroundColor1.B))

        SetIniValue("ForegroundColor2", "R", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini", CStr(CustomForegroundColor2.R))
        SetIniValue("ForegroundColor2", "G", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini", CStr(CustomForegroundColor2.G))
        SetIniValue("ForegroundColor2", "B", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini", CStr(CustomForegroundColor2.B))

        SetIniValue("ForegroundColor3", "R", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini", CStr(CustomForegroundColor3.R))
        SetIniValue("ForegroundColor3", "G", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini", CStr(CustomForegroundColor3.G))
        SetIniValue("ForegroundColor3", "B", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini", CStr(CustomForegroundColor3.B))

        SetIniValue("CustomGridColor1", "R", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini", CStr(CustomGridColor1.R))
        SetIniValue("CustomGridColor1", "G", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini", CStr(CustomGridColor1.G))
        SetIniValue("CustomGridColor1", "B", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini", CStr(CustomGridColor1.B))

        SetIniValue("CustomGridBGColor1", "R", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini", CStr(CustomGridBGColor1.R))
        SetIniValue("CustomGridBGColor1", "G", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini", CStr(CustomGridBGColor1.G))
        SetIniValue("CustomGridBGColor1", "B", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini", CStr(CustomGridBGColor1.B))

        SetIniValue("CustomGridBGColor2", "R", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini", CStr(CustomGridBGColor2.R))
        SetIniValue("CustomGridBGColor2", "G", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini", CStr(CustomGridBGColor2.G))
        SetIniValue("CustomGridBGColor2", "B", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini", CStr(CustomGridBGColor2.B))

        SetIniValue("CustomGridSelectColor1", "R", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini", CStr(CustomGridSelectColor1.R))
        SetIniValue("CustomGridSelectColor1", "G", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini", CStr(CustomGridSelectColor1.G))
        SetIniValue("CustomGridSelectColor1", "B", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini", CStr(CustomGridSelectColor1.B))

        SetIniValue("CustomHistGridColor", "R", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini", CStr(CustomHistGridColor.R))
        SetIniValue("CustomHistGridColor", "G", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini", CStr(CustomHistGridColor.G))
        SetIniValue("CustomHistGridColor", "B", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini", CStr(CustomHistGridColor.B))

        SetIniValue("CustomHistBuyColor1", "R", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini", CStr(CustomHistBuyColor1.R))
        SetIniValue("CustomHistBuyColor1", "G", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini", CStr(CustomHistBuyColor1.G))
        SetIniValue("CustomHistBuyColor1", "B", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini", CStr(CustomHistBuyColor1.B))

        SetIniValue("CustomHistBuyColor2", "R", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini", CStr(CustomHistBuyColor2.R))
        SetIniValue("CustomHistBuyColor2", "G", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini", CStr(CustomHistBuyColor2.G))
        SetIniValue("CustomHistBuyColor2", "B", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini", CStr(CustomHistBuyColor2.B))

        SetIniValue("CustomHistBuyColor3", "R", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini", CStr(CustomHistBuyColor3.R))
        SetIniValue("CustomHistBuyColor3", "G", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini", CStr(CustomHistBuyColor3.G))
        SetIniValue("CustomHistBuyColor3", "B", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini", CStr(CustomHistBuyColor3.B))

        SetIniValue("CustomHistSellColor1", "R", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini", CStr(CustomHistSellColor1.R))
        SetIniValue("CustomHistSellColor1", "G", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini", CStr(CustomHistSellColor1.G))
        SetIniValue("CustomHistSellColor1", "B", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini", CStr(CustomHistSellColor1.B))

        SetIniValue("CustomHistSellColor2", "R", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini", CStr(CustomHistSellColor2.R))
        SetIniValue("CustomHistSellColor2", "G", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini", CStr(CustomHistSellColor2.G))
        SetIniValue("CustomHistSellColor2", "B", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini", CStr(CustomHistSellColor2.B))

        SetIniValue("CustomHistSellColor3", "R", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini", CStr(CustomHistSellColor3.R))
        SetIniValue("CustomHistSellColor3", "G", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini", CStr(CustomHistSellColor3.G))
        SetIniValue("CustomHistSellColor3", "B", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini", CStr(CustomHistSellColor3.B))
    End Sub

    Public Sub LoadCustomTheme()
        If File.Exists(My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini") = True Then
            CustomBackgroundColor1 = Color.FromArgb(255, GetIniValue("BackgroundColor1", "R", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini"), GetIniValue("BackgroundColor1", "G", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini"), GetIniValue("BackgroundColor1", "B", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini"))

            CustomBackgroundColor2 = Color.FromArgb(255, GetIniValue("BackgroundColor2", "R", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini"),
            GetIniValue("BackgroundColor2", "G", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini"),
            GetIniValue("BackgroundColor2", "B", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini"))

            CustomBackgroundColor3 = Color.FromArgb(255, GetIniValue("BackgroundColor3", "R", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini"),
            GetIniValue("BackgroundColor3", "G", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini"),
            GetIniValue("BackgroundColor3", "B", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini"))

            CustomForegroundColor1 = Color.FromArgb(255, GetIniValue("ForegroundColor1", "R", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini"),
            GetIniValue("ForegroundColor1", "G", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini"),
            GetIniValue("ForegroundColor1", "B", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini"))

            CustomForegroundColor2 = Color.FromArgb(255, GetIniValue("ForegroundColor2", "R", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini"),
            GetIniValue("ForegroundColor2", "G", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini"),
            GetIniValue("ForegroundColor2", "B", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini"))

            CustomForegroundColor3 = Color.FromArgb(255, GetIniValue("ForegroundColor3", "R", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini"),
            GetIniValue("ForegroundColor3", "G", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini"),
            GetIniValue("ForegroundColor3", "B", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini"))

            CustomGridColor1 = Color.FromArgb(255, GetIniValue("CustomGridColor1", "R", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini"),
            GetIniValue("CustomGridColor1", "G", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini"),
            GetIniValue("CustomGridColor1", "B", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini"))

            CustomGridBGColor1 = Color.FromArgb(255, GetIniValue("CustomGridBGColor1", "R", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini"),
            GetIniValue("CustomGridBGColor1", "G", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini"),
            GetIniValue("CustomGridBGColor1", "B", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini"))

            CustomGridBGColor2 = Color.FromArgb(255, GetIniValue("CustomGridBGColor2", "R", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini"),
            GetIniValue("CustomGridBGColor2", "G", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini"),
            GetIniValue("CustomGridBGColor2", "B", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini"))

            CustomGridSelectColor1 = Color.FromArgb(255, GetIniValue("CustomGridSelectColor1", "R", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini"),
            GetIniValue("CustomGridSelectColor1", "G", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini"),
            GetIniValue("CustomGridSelectColor1", "B", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini"))

            CustomHistGridColor = Color.FromArgb(255, GetIniValue("CustomHistGridColor", "R", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini"),
            GetIniValue("CustomHistGridColor", "G", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini"),
            GetIniValue("CustomHistGridColor", "B", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini"))

            CustomHistBuyColor1 = Color.FromArgb(255, GetIniValue("CustomHistBuyColor1", "R", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini"),
            GetIniValue("CustomHistBuyColor1", "G", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini"),
            GetIniValue("CustomHistBuyColor1", "B", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini"))

            CustomHistBuyColor2 = Color.FromArgb(255, GetIniValue("CustomHistBuyColor2", "R", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini"),
            GetIniValue("CustomHistBuyColor2", "G", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini"),
            GetIniValue("CustomHistBuyColor2", "B", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini"))

            CustomHistBuyColor3 = Color.FromArgb(255, GetIniValue("CustomHistBuyColor3", "R", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini"),
            GetIniValue("CustomHistBuyColor3", "G", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini"),
            GetIniValue("CustomHistBuyColor3", "B", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini"))

            CustomHistSellColor1 = Color.FromArgb(255, GetIniValue("CustomHistSellColor1", "R", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini"),
            GetIniValue("CustomHistSellColor1", "G", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini"),
            GetIniValue("CustomHistSellColor1", "B", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini"))

            CustomHistSellColor2 = Color.FromArgb(255, GetIniValue("CustomHistSellColor2", "R", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini"),
            GetIniValue("CustomHistSellColor2", "G", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini"),
            GetIniValue("CustomHistSellColor2", "B", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini"))

            CustomHistSellColor3 = Color.FromArgb(255, GetIniValue("CustomHistSellColor3", "R", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini"),
            GetIniValue("CustomHistSellColor3", "G", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini"),
            GetIniValue("CustomHistSellColor3", "B", My.Application.Info.DirectoryPath & "\DUOMcustomTheme.ini"))
        Else
            SaveCustomTheme()
        End If
    End Sub

    '############################## - Registry-Key Constructors - ##############################
    Private Sub CreateEnableStartupKey()
        Dim StartupDirectory As String = GetFolderPath(SpecialFolder.ApplicationData) & "\Microsoft\Windows\Start Menu\Programs\Startup"
        If Directory.Exists(StartupDirectory) = True Then
            Dim oShell As Object
            Dim oLink As Object
            'you don’t need to import anything in the project reference to create the Shell Object
            Try
                oShell = CreateObject("WScript.Shell")
                oLink = oShell.CreateShortcut(StartupDirectory & "\DUOpenMarket.lnk")
                oLink.TargetPath = Application.ExecutablePath
                oLink.WindowStyle = 1
                oLink.Save()
                NewEventMsg("Created Startup entry @: " & StartupDirectory & "\DUOpenMarket.lnk")
                NewEventMsg("Target Path: " & Application.ExecutablePath)
            Catch ex As Exception
                NewEventMsg(ex.Message)
            End Try
        End If
    End Sub

    Private Sub DisableStartupKey()
        Dim StartupDirectory As String = GetFolderPath(SpecialFolder.ApplicationData) & "\Microsoft\Windows\Start Menu\Programs\Startup"
        If Directory.Exists(StartupDirectory) = True Then
            If File.Exists(StartupDirectory & "\DUOpenMarket.lnk") = True Then
                Try
                    File.Delete(StartupDirectory & "\DUOpenMarket.lnk")
                Catch ex As Exception
                    NewEventMsg(ex.Message)
                End Try
            Else
                NewEventMsg("No existing startup entry was found to remove.")
            End If
        End If
    End Sub

    '############################## - Form Load - ##############################
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TimeStampInt()
        LoadCustomTheme()
        LoadPrefsFromIni()
        CenterLoginElements()
        CheckForUpdates()
        InitDataTables()
        InitItemList()
        SetupGridViewStyling()
        InitMarketTable()
        InitAdvMarketTree()
        NewEventMsg("Initialized.")
    End Sub

    '############################## - Custom Sorting - #############################
    Private Sub BuyOrderSortButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BuyOrderGridViewRaw.ColumnHeaderMouseClick
        SortDataTable(API_Buy_Orders_UI, BuyOrderGridViewRaw, e)
    End Sub

    Private Sub SellOrderSortButton_Click(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellMouseEventArgs) Handles SellOrderGridViewRaw.ColumnHeaderMouseClick
        SortDataTable(API_Sell_Orders_UI, SellOrderGridViewRaw, e)
    End Sub

    Private Sub SortDataTable(ByRef dt As DataTable, ByVal gridref As DataGridView, ByVal e As System.Windows.Forms.DataGridViewCellMouseEventArgs)
        Try
            If gridref.SelectedCells.Count > 0 Then
                Dim newDT As DataTable = dt.Clone
                Dim rowCount As Integer = dt.Rows.Count
                Dim sortColumn As DataGridViewColumn = gridref.Columns(e.ColumnIndex)
                Dim oldsortColumn As DataGridViewColumn = Nothing
                Dim sortDirection As System.ComponentModel.ListSortDirection
                If gridref Is BuyOrderGridViewRaw Then
                    oldsortColumn = gridref.Columns(SortedColumnIndex1)
                    sortDirection = SortedColumnDirection1
                End If
                If gridref Is SellOrderGridViewRaw Then
                    oldsortColumn = gridref.Columns(SortedColumnIndex2)
                    sortDirection = SortedColumnDirection2
                End If
                If (Not (oldsortColumn) Is Nothing) Then
                    ' Sort the same column again, reversing the SortOrder.
                    If (oldsortColumn Is sortColumn) Then
                        If sortDirection = System.ComponentModel.ListSortDirection.Ascending Then
                            sortDirection = System.ComponentModel.ListSortDirection.Descending
                        Else
                            sortDirection = System.ComponentModel.ListSortDirection.Ascending
                        End If
                    Else
                        ' Sort a new column and remove the old SortGlyph.
                        sortDirection = System.ComponentModel.ListSortDirection.Ascending
                        oldsortColumn.HeaderCell.SortGlyphDirection = SortOrder.None
                    End If
                Else
                    sortDirection = System.ComponentModel.ListSortDirection.Ascending
                End If
                If gridref Is BuyOrderGridViewRaw Then
                    SortedColumnDirection1 = sortDirection
                End If
                If gridref Is SellOrderGridViewRaw Then
                    SortedColumnDirection2 = sortDirection
                End If
                If (sortColumn Is Nothing) Then
                    NewEventMsg("Select a single column and try again.")
                Else
                    If sortColumn.Index = 0 Or sortColumn.Index = 3 Or sortColumn.Index = 4 Then
                        'string alphabetical sort
                        gridref.Sort(sortColumn, sortDirection)
                    End If
                    If sortColumn.Index = 1 Or sortColumn.Index = 2 Then
                        'reset string sorting, it conflicts with the way we custom-sort doubles
                        If gridref Is BuyOrderGridViewRaw Then
                            If SortedColumnIndex1 < 1 Or SortedColumnIndex1 > 2 Then
                                dt.DefaultView.Sort = Nothing
                            End If
                        End If
                        If gridref Is SellOrderGridViewRaw Then
                            If SortedColumnIndex2 < 1 Or SortedColumnIndex2 > 2 Then
                                dt.DefaultView.Sort = Nothing
                            End If
                        End If
                        'custom numerical sort
                        'For quantity and price, we need to sort numerically. The values are stored as strings, and include decimal places.
                        'we'll need to cast them to Doubles in order to interpret / compare them properly.
                        If sortDirection = System.ComponentModel.ListSortDirection.Ascending Then
                            For sorta As Integer = 1 To rowCount
                                Dim HighestCellValue As Double = 0
                                Dim HighestCellIndex As Integer = 0

                                For sortb As Integer = 1 To dt.Rows.Count
                                    If HighestCellValue = Nothing Then
                                        If dt.Rows(sortb - 1).Item(sortColumn.Index).ToString.Contains(".") = True Then
                                            HighestCellValue = CDbl(dt.Rows(sortb - 1).Item(sortColumn.Index).ToString.Remove(dt.Rows(sortb - 1).Item(sortColumn.Index).ToString.IndexOf("."), 1))
                                        Else
                                            HighestCellValue = CDbl(dt.Rows(sortb - 1).Item(sortColumn.Index))
                                        End If
                                    Else
                                        If dt.Rows(sortb - 1).Item(sortColumn.Index).ToString.Contains(".") = True Then
                                            Dim valuetemp1 As Double = CDbl(dt.Rows(sortb - 1).Item(sortColumn.Index).ToString.Remove(dt.Rows(sortb - 1).Item(sortColumn.Index).ToString.IndexOf("."), 1))
                                            If HighestCellValue >= valuetemp1 Then
                                                'skip
                                            Else
                                                HighestCellValue = valuetemp1
                                                HighestCellIndex = sortb - 1
                                            End If
                                        Else
                                            Dim valuetemp2 As Double = CDbl(dt.Rows(sortb - 1).Item(sortColumn.Index))
                                            If HighestCellValue >= valuetemp2 Then
                                                'skip
                                            Else
                                                HighestCellValue = valuetemp2
                                                HighestCellIndex = sortb - 1
                                            End If
                                        End If
                                    End If
                                Next sortb
                                Dim new_data_row As DataRow = newDT.NewRow
                                new_data_row.ItemArray = dt.Rows(HighestCellIndex).ItemArray
                                newDT.Rows.Add(new_data_row)
                                dt.Rows(HighestCellIndex).Delete()
                            Next sorta
                            For sortc As Integer = 1 To newDT.Rows.Count
                                Dim new_data_row2 As DataRow = dt.NewRow
                                new_data_row2.ItemArray = newDT.Rows(sortc - 1).ItemArray
                                dt.Rows.Add(new_data_row2)
                            Next sortc
                        Else
                            For sorta As Integer = 1 To rowCount
                                Dim lowestCellValue As Double = 0
                                Dim lowestCellIndex As Integer = 0

                                For sortb As Integer = 1 To dt.Rows.Count
                                    If lowestCellValue = Nothing Then
                                        If dt.Rows(sortb - 1).Item(sortColumn.Index).ToString.Contains(".") = True Then
                                            lowestCellValue = CDbl(dt.Rows(sortb - 1).Item(sortColumn.Index).ToString.Remove(dt.Rows(sortb - 1).Item(sortColumn.Index).ToString.IndexOf("."), 1))
                                        Else
                                            lowestCellValue = CDbl(dt.Rows(sortb - 1).Item(sortColumn.Index))
                                        End If
                                    Else
                                        If dt.Rows(sortb - 1).Item(sortColumn.Index).ToString.Contains(".") = True Then
                                            Dim valuetemp1 As Double = CDbl(dt.Rows(sortb - 1).Item(sortColumn.Index).ToString.Remove(dt.Rows(sortb - 1).Item(sortColumn.Index).ToString.IndexOf("."), 1))
                                            If lowestCellValue <= valuetemp1 Then
                                                'skip
                                            Else
                                                lowestCellValue = valuetemp1
                                                lowestCellIndex = sortb - 1
                                            End If
                                        Else
                                            Dim valuetemp2 As Double = CDbl(dt.Rows(sortb - 1).Item(sortColumn.Index))
                                            If lowestCellValue <= valuetemp2 Then
                                                'skip
                                            Else
                                                lowestCellValue = valuetemp2
                                                lowestCellIndex = sortb - 1
                                            End If
                                        End If
                                    End If
                                Next sortb
                                Dim new_data_row As DataRow = newDT.NewRow
                                new_data_row.ItemArray = dt.Rows(lowestCellIndex).ItemArray
                                newDT.Rows.Add(new_data_row)
                                dt.Rows(lowestCellIndex).Delete()
                            Next sorta
                            For sortc As Integer = 1 To newDT.Rows.Count
                                Dim new_data_row2 As DataRow = dt.NewRow
                                new_data_row2.ItemArray = newDT.Rows(sortc - 1).ItemArray
                                dt.Rows.Add(new_data_row2)
                            Next sortc
                        End If
                    End If
                    sortColumn.HeaderCell.SortGlyphDirection = SortOrder.Ascending
                    If sortDirection = System.ComponentModel.ListSortDirection.Ascending Then
                        sortColumn.HeaderCell.SortGlyphDirection = SortOrder.Ascending
                    Else
                        sortColumn.HeaderCell.SortGlyphDirection = SortOrder.Descending
                    End If
                End If
                If gridref Is BuyOrderGridViewRaw Then
                    SortedColumnIndex1 = sortColumn.Index
                End If
                If gridref Is SellOrderGridViewRaw Then
                    SortedColumnIndex2 = sortColumn.Index
                End If
                newDT = Nothing
            Else
                NewEventMsg("Sort Error: No column selected to sort by.")
            End If
        Catch ex As Exception
            NewEventMsg("Sort Error: " & ex.Message)
        End Try
    End Sub

    '############################## - API Request Constructor - ##############################
    Private Function API_Request(ByVal reqtype As String, ByVal addr As String)
        Try
            Dim requestUrl As String = Nothing
            If reqtype = "create" Then
                requestUrl = "https://duopenmarket.com/openmarketapi.php/" & API_Discord_Auth_Code & "/create?" & addr
                NumberOfCreates = NumberOfCreates + 1
            End If
            If reqtype = "read" Then
                requestUrl = "https://duopenmarket.com/openmarketapi.php/" & API_Discord_Auth_Code & "/read?" & addr
                NumberOfReads = NumberOfReads + 1
            End If
            If reqtype = "update" Then
                requestUrl = "https://duopenmarket.com/openmarketapi.php/" & API_Discord_Auth_Code & "/update?" & addr
                NumberOfUpdates = NumberOfUpdates + 1
            End If
            If reqtype = "delete" Then
                requestUrl = "https://duopenmarket.com/openmarketapi.php/" & API_Discord_Auth_Code & "/delete?" & addr
                NumberOfDeletes = NumberOfDeletes + 1
            End If
            If reqtype = "history" Then
                requestUrl = "https://duopenmarket.com/openmarketapi.php/" & API_Discord_Auth_Code & "/history?" & addr
                NumberOfHistories = NumberOfHistories + 1
            End If
            If reqtype = "ver" Then
                requestUrl = "https://duopenmarket.com/openmarketapi.php/version?"
            End If
            If reqtype = "minver" Then
                requestUrl = "https://duopenmarket.com/openmarketapi.php/minversion?"
            End If
            Dim request As WebRequest = WebRequest.Create(requestUrl)
            Dim response As WebResponse = request.GetResponse()
            Dim dataStream As Stream = response.GetResponseStream()
            Dim reader As StreamReader = New StreamReader(dataStream)
            TempResponse = reader.ReadToEnd()
            response.Close()
            Return TempResponse
        Catch ex As Exception
            NewEventMsg(ex.Message)
        End Try
    End Function

    '############################## - User Interface - ##############################
    '########## Order Grid UI ##########
    Private Sub SetupGridViewStyling()
        BuyOrderGridViewRaw.Columns(0).SortMode = DataGridViewColumnSortMode.Programmatic
        BuyOrderGridViewRaw.Columns(1).SortMode = DataGridViewColumnSortMode.Programmatic
        BuyOrderGridViewRaw.Columns(2).SortMode = DataGridViewColumnSortMode.Programmatic
        BuyOrderGridViewRaw.Columns(3).SortMode = DataGridViewColumnSortMode.Programmatic
        BuyOrderGridViewRaw.Columns(4).SortMode = DataGridViewColumnSortMode.Programmatic
        If savedSellOrdrGridCol1W = Nothing And savedSellOrdrGridCol2W = Nothing And savedSellOrdrGridCol3W = Nothing And savedSellOrdrGridCol4W = Nothing And savedSellOrdrGridCol5W = Nothing Then
            Dim col1 As DataGridViewColumn = SellOrderGridViewRaw.Columns(0)
            Dim col2 As DataGridViewColumn = SellOrderGridViewRaw.Columns(1)
            Dim col3 As DataGridViewColumn = SellOrderGridViewRaw.Columns(2)
            Dim col4 As DataGridViewColumn = SellOrderGridViewRaw.Columns(3)
            Dim col5 As DataGridViewColumn = SellOrderGridViewRaw.Columns(4)
            col1.Width = 194
            col2.Width = 194
            col3.Width = 194
            col4.Width = 194
            col5.Width = 194
        Else
            Dim col1 As DataGridViewColumn = SellOrderGridViewRaw.Columns(0)
            Dim col2 As DataGridViewColumn = SellOrderGridViewRaw.Columns(1)
            Dim col3 As DataGridViewColumn = SellOrderGridViewRaw.Columns(2)
            Dim col4 As DataGridViewColumn = SellOrderGridViewRaw.Columns(3)
            Dim col5 As DataGridViewColumn = SellOrderGridViewRaw.Columns(4)
            col1.Width = savedSellOrdrGridCol1W
            col2.Width = savedSellOrdrGridCol2W
            col3.Width = savedSellOrdrGridCol3W
            col4.Width = savedSellOrdrGridCol4W
            col5.Width = savedSellOrdrGridCol5W
        End If
        If savedBuyOrdrGridCol1W = Nothing And savedBuyOrdrGridCol2W = Nothing And savedBuyOrdrGridCol3W = Nothing And savedBuyOrdrGridCol4W = Nothing And savedBuyOrdrGridCol5W = Nothing Then
            Dim col1 As DataGridViewColumn = BuyOrderGridViewRaw.Columns(0)
            Dim col2 As DataGridViewColumn = BuyOrderGridViewRaw.Columns(1)
            Dim col3 As DataGridViewColumn = BuyOrderGridViewRaw.Columns(2)
            Dim col4 As DataGridViewColumn = BuyOrderGridViewRaw.Columns(3)
            Dim col5 As DataGridViewColumn = BuyOrderGridViewRaw.Columns(4)
            col1.Width = 194
            col2.Width = 194
            col3.Width = 194
            col4.Width = 194
            col5.Width = 194
        Else
            Dim col1 As DataGridViewColumn = BuyOrderGridViewRaw.Columns(0)
            Dim col2 As DataGridViewColumn = BuyOrderGridViewRaw.Columns(1)
            Dim col3 As DataGridViewColumn = BuyOrderGridViewRaw.Columns(2)
            Dim col4 As DataGridViewColumn = BuyOrderGridViewRaw.Columns(3)
            Dim col5 As DataGridViewColumn = BuyOrderGridViewRaw.Columns(4)
            col1.Width = savedBuyOrdrGridCol1W
            col2.Width = savedBuyOrdrGridCol2W
            col3.Width = savedBuyOrdrGridCol3W
            col4.Width = savedBuyOrdrGridCol4W
            col5.Width = savedBuyOrdrGridCol5W
        End If
    End Sub

    Private Sub InitDataTables()
        BuyOrderGridViewRaw.Columns.Clear()
        SellOrderGridViewRaw.Columns.Clear()
        'back-end tables
        API_Buy_Orders.Columns.Add("marketID")
        API_Buy_Orders.Columns.Add("orderID")
        API_Buy_Orders.Columns.Add("itemID")
        API_Buy_Orders.Columns.Add("quantity")
        API_Buy_Orders.Columns.Add("price")
        API_Buy_Orders.Columns.Add("expiration")
        API_Buy_Orders.Columns.Add("updated")
        API_Sell_Orders.Columns.Add("marketID")
        API_Sell_Orders.Columns.Add("orderID")
        API_Sell_Orders.Columns.Add("itemID")
        API_Sell_Orders.Columns.Add("quantity")
        API_Sell_Orders.Columns.Add("price")
        API_Sell_Orders.Columns.Add("expiration")
        API_Sell_Orders.Columns.Add("updated")
        'User-Facing tables
        API_Buy_Orders_UI.Columns.Add("market", GetType(String))
        API_Buy_Orders_UI.Columns.Add("quantity", GetType(String))
        API_Buy_Orders_UI.Columns.Add("price", GetType(String))
        API_Buy_Orders_UI.Columns.Add("expiration", GetType(String))
        API_Buy_Orders_UI.Columns.Add("item", GetType(String))
        API_Sell_Orders_UI.Columns.Add("market", GetType(String))
        API_Sell_Orders_UI.Columns.Add("quantity", GetType(String))
        API_Sell_Orders_UI.Columns.Add("price", GetType(String))
        API_Sell_Orders_UI.Columns.Add("expiration", GetType(String))
        API_Sell_Orders_UI.Columns.Add("item", GetType(String))
        BuyOrderGridViewRaw.DataSource = API_Buy_Orders_UI
        SellOrderGridViewRaw.DataSource = API_Sell_Orders_UI
    End Sub

    Private Sub ResetDataTables()
        If ShowRawData = False Then
            BuyOrderGridViewRaw.DataSource = API_Buy_Orders_UI
            SellOrderGridViewRaw.DataSource = API_Sell_Orders_UI
        Else
            BuyOrderGridViewRaw.DataSource = API_Buy_Orders
            SellOrderGridViewRaw.DataSource = API_Sell_Orders
        End If
    End Sub

    Private Sub ClearDataTables()
        API_Buy_Orders.Rows.Clear()
        API_Sell_Orders.Rows.Clear()
        API_Buy_Orders_UI.Rows.Clear()
        API_Sell_Orders_UI.Rows.Clear()
    End Sub

    '########## Market / Item list and treeview init ##########
    Private Sub InitMarketTable()
        API_Market_Table.Columns.Add("id", GetType(Integer))
        API_Market_Table.Columns.Add("name", GetType(String))
        API_Market_Table.Rows.Add(52854, "Tuto Market")
        API_Market_Table.Rows.Add(46130, "Market Haven 01")
        API_Market_Table.Rows.Add(46128, "Market Alioth District 01")
        API_Market_Table.Rows.Add(46129, "Market Alioth District 06")
        API_Market_Table.Rows.Add(46131, "Market Haven 06")
        API_Market_Table.Rows.Add(46132, "Market Alioth 16")
        API_Market_Table.Rows.Add(46133, "Market Sanctuary 01")
        API_Market_Table.Rows.Add(46134, "Market Sanctuary 06")
        API_Market_Table.Rows.Add(46135, "Market Alioth District 10")
        API_Market_Table.Rows.Add(46136, "Aegis Space Market")
        API_Market_Table.Rows.Add(46137, "Market Alioth District 02")
        API_Market_Table.Rows.Add(46138, "Market Alioth District 03")
        API_Market_Table.Rows.Add(46139, "Market Alioth District 04")
        API_Market_Table.Rows.Add(46140, "Market Alioth District 05")
        API_Market_Table.Rows.Add(46141, "Market Alioth District 07")
        API_Market_Table.Rows.Add(46142, "Market Alioth District 08")
        API_Market_Table.Rows.Add(46143, "Market Alioth District 09")
        API_Market_Table.Rows.Add(46144, "Market Haven 10")
        API_Market_Table.Rows.Add(46145, "Market Haven 02")
        API_Market_Table.Rows.Add(46146, "Market Haven 03")
        API_Market_Table.Rows.Add(46147, "Market Haven 04")
        API_Market_Table.Rows.Add(46148, "Market Haven 05")
        API_Market_Table.Rows.Add(46149, "Market Haven 08")
        API_Market_Table.Rows.Add(46150, "Market Haven 09")
        API_Market_Table.Rows.Add(46151, "Market Haven 07")
        API_Market_Table.Rows.Add(46152, "Market Sanctuary 13")
        API_Market_Table.Rows.Add(46153, "Market Madis Moon II 1")
        API_Market_Table.Rows.Add(52846, "Tuto Market")
        API_Market_Table.Rows.Add(46154, "Market Thades 4")
        API_Market_Table.Rows.Add(46155, "Market Madis Moon III 2")
        API_Market_Table.Rows.Add(46156, "Market Teoma 1")
        API_Market_Table.Rows.Add(46157, "Market Teoma 6")
        API_Market_Table.Rows.Add(46158, "Market Alioth 15")
        API_Market_Table.Rows.Add(46159, "Market Alioth 18")
        API_Market_Table.Rows.Add(46160, "Market Alioth 14")
        API_Market_Table.Rows.Add(46161, "Market Madis 5")
        API_Market_Table.Rows.Add(46162, "Market Teoma 5")
        API_Market_Table.Rows.Add(46163, "Market Haven 11")
        API_Market_Table.Rows.Add(46164, "Market Haven 12")
        API_Market_Table.Rows.Add(46165, "Market Teoma 2")
        API_Market_Table.Rows.Add(46166, "Market Madis Moon I 2")
        API_Market_Table.Rows.Add(46167, "Market Madis 4")
        API_Market_Table.Rows.Add(46168, "Market Alioth Moon IV 1")
        API_Market_Table.Rows.Add(46169, "Market Sanctuary 19")
        API_Market_Table.Rows.Add(46170, "Market Thades 1")
        API_Market_Table.Rows.Add(46171, "Market Haven 14")
        API_Market_Table.Rows.Add(46172, "Market Sanctuary 20")
        API_Market_Table.Rows.Add(46173, "Market Madis Moon II 2")
        API_Market_Table.Rows.Add(46174, "Market Alioth 12")
        API_Market_Table.Rows.Add(46175, "Market Madis 6")
        API_Market_Table.Rows.Add(46176, "Market Alioth 17")
        API_Market_Table.Rows.Add(46177, "Market Alioth Moon I 1")
        API_Market_Table.Rows.Add(46178, "Market Alioth 19")
        API_Market_Table.Rows.Add(46179, "Market Haven 16")
        API_Market_Table.Rows.Add(46180, "Market Alioth Moon IV 2")
        API_Market_Table.Rows.Add(46181, "Market Jago 3")
        API_Market_Table.Rows.Add(46182, "Market Madis 3")
        API_Market_Table.Rows.Add(46183, "Market Haven 17")
        API_Market_Table.Rows.Add(46184, "Market Sanctuary 11")
        API_Market_Table.Rows.Add(46185, "Market Sanctuary 14")
        API_Market_Table.Rows.Add(46186, "Market Thades 6")
        API_Market_Table.Rows.Add(46187, "Market Alioth 11")
        API_Market_Table.Rows.Add(46188, "Market Jago 6")
        API_Market_Table.Rows.Add(46189, "Market Sanctuary 18")
        API_Market_Table.Rows.Add(46190, "Market Alioth 13")
        API_Market_Table.Rows.Add(46191, "Market Thades 2")
        API_Market_Table.Rows.Add(46192, "Market Jago 2")
        API_Market_Table.Rows.Add(46193, "Market Sanctuary 16")
        API_Market_Table.Rows.Add(46194, "Market Thades Moon I 2")
        API_Market_Table.Rows.Add(46195, "Market Teoma 3")
        API_Market_Table.Rows.Add(46196, "Market Jago 4")
        API_Market_Table.Rows.Add(46197, "Market Sanctuary 17")
        API_Market_Table.Rows.Add(46198, "Market Haven 13")
        API_Market_Table.Rows.Add(46199, "Market Madis Moon III 1")
        API_Market_Table.Rows.Add(46200, "Market Madis Moon I 1")
        API_Market_Table.Rows.Add(46201, "Market Sanctuary 15")
        API_Market_Table.Rows.Add(46202, "Market Alioth 20")
        API_Market_Table.Rows.Add(46203, "Market Thades Moon I 1")
        API_Market_Table.Rows.Add(46204, "Market Haven 15")
        API_Market_Table.Rows.Add(46205, "Market Thades 5")
        API_Market_Table.Rows.Add(46206, "Market Thades 3")
        API_Market_Table.Rows.Add(46207, "Market Haven 18")
        API_Market_Table.Rows.Add(46208, "Market Madis 1")
        API_Market_Table.Rows.Add(46209, "Market Thades Moon II 1")
        API_Market_Table.Rows.Add(46210, "Market Madis 2")
        API_Market_Table.Rows.Add(46211, "Market Jago 1")
        API_Market_Table.Rows.Add(46212, "Market Jago 5")
        API_Market_Table.Rows.Add(46213, "Market Sanctuary 12")
        API_Market_Table.Rows.Add(46214, "Market Thades Moon II 2")
        API_Market_Table.Rows.Add(46215, "Market Alioth Moon I 2")
        API_Market_Table.Rows.Add(46216, "Market Haven 20")
        API_Market_Table.Rows.Add(46217, "Market Haven 19")
        API_Market_Table.Rows.Add(46218, "Market Teoma 4")
        API_Market_Table.Rows.Add(46219, "Market Sanctuary 10")
        API_Market_Table.Rows.Add(46220, "Market Sanctuary 02")
        API_Market_Table.Rows.Add(46221, "Market Sanctuary 03")
        API_Market_Table.Rows.Add(46222, "Market Sanctuary 04")
        API_Market_Table.Rows.Add(46223, "Market Sanctuary 05")
        API_Market_Table.Rows.Add(46224, "Market Sanctuary 07")
        API_Market_Table.Rows.Add(46225, "Market Sanctuary 08")
        API_Market_Table.Rows.Add(46226, "Market Sanctuary 09")
        API_Market_Table.Rows.Add(52853, "Tuto Market")
    End Sub

    Private Sub InitAdvMarketTree()
        'AdvMarketTreeView.Nodes.Add("52854", "Tuto Market")
        'AdvMarketTreeView.Nodes.Add("52846", "Tuto Market")
        'AdvMarketTreeView.Nodes.Add("52853", "Tuto Market")
        AdvMarketTreeView.Nodes.Add("46136", "Aegis Space Market")
        AdvMarketTreeView.Nodes.Add("46128", "Market Alioth District 01")
        AdvMarketTreeView.Nodes.Add("46137", "Market Alioth District 02")
        AdvMarketTreeView.Nodes.Add("46138", "Market Alioth District 03")
        AdvMarketTreeView.Nodes.Add("46139", "Market Alioth District 04")
        AdvMarketTreeView.Nodes.Add("46140", "Market Alioth District 05")
        AdvMarketTreeView.Nodes.Add("46129", "Market Alioth District 06")
        AdvMarketTreeView.Nodes.Add("46141", "Market Alioth District 07")
        AdvMarketTreeView.Nodes.Add("46142", "Market Alioth District 08")
        AdvMarketTreeView.Nodes.Add("46143", "Market Alioth District 09")
        AdvMarketTreeView.Nodes.Add("46135", "Market Alioth District 10")
        AdvMarketTreeView.Nodes.Add("46187", "Market Alioth 11")
        AdvMarketTreeView.Nodes.Add("46174", "Market Alioth 12")
        AdvMarketTreeView.Nodes.Add("46190", "Market Alioth 13")
        AdvMarketTreeView.Nodes.Add("46160", "Market Alioth 14")
        AdvMarketTreeView.Nodes.Add("46158", "Market Alioth 15")
        AdvMarketTreeView.Nodes.Add("46132", "Market Alioth 16")
        AdvMarketTreeView.Nodes.Add("46176", "Market Alioth 17")
        AdvMarketTreeView.Nodes.Add("46159", "Market Alioth 18")
        AdvMarketTreeView.Nodes.Add("46178", "Market Alioth 19")
        AdvMarketTreeView.Nodes.Add("46202", "Market Alioth 20")
        AdvMarketTreeView.Nodes.Add("46130", "Market Haven 01")
        AdvMarketTreeView.Nodes.Add("46145", "Market Haven 02")
        AdvMarketTreeView.Nodes.Add("46146", "Market Haven 03")
        AdvMarketTreeView.Nodes.Add("46147", "Market Haven 04")
        AdvMarketTreeView.Nodes.Add("46148", "Market Haven 05")
        AdvMarketTreeView.Nodes.Add("46131", "Market Haven 06")
        AdvMarketTreeView.Nodes.Add("46151", "Market Haven 07")
        AdvMarketTreeView.Nodes.Add("46149", "Market Haven 08")
        AdvMarketTreeView.Nodes.Add("46150", "Market Haven 09")
        AdvMarketTreeView.Nodes.Add("46144", "Market Haven 10")
        AdvMarketTreeView.Nodes.Add("46163", "Market Haven 11")
        AdvMarketTreeView.Nodes.Add("46164", "Market Haven 12")
        AdvMarketTreeView.Nodes.Add("46198", "Market Haven 13")
        AdvMarketTreeView.Nodes.Add("46171", "Market Haven 14")
        AdvMarketTreeView.Nodes.Add("46204", "Market Haven 15")
        AdvMarketTreeView.Nodes.Add("46179", "Market Haven 16")
        AdvMarketTreeView.Nodes.Add("46183", "Market Haven 17")
        AdvMarketTreeView.Nodes.Add("46207", "Market Haven 18")
        AdvMarketTreeView.Nodes.Add("46217", "Market Haven 19")
        AdvMarketTreeView.Nodes.Add("46216", "Market Haven 20")
        AdvMarketTreeView.Nodes.Add("46133", "Market Sanctuary 01")
        AdvMarketTreeView.Nodes.Add("46220", "Market Sanctuary 02")
        AdvMarketTreeView.Nodes.Add("46221", "Market Sanctuary 03")
        AdvMarketTreeView.Nodes.Add("46222", "Market Sanctuary 04")
        AdvMarketTreeView.Nodes.Add("46223", "Market Sanctuary 05")
        AdvMarketTreeView.Nodes.Add("46134", "Market Sanctuary 06")
        AdvMarketTreeView.Nodes.Add("46224", "Market Sanctuary 07")
        AdvMarketTreeView.Nodes.Add("46225", "Market Sanctuary 08")
        AdvMarketTreeView.Nodes.Add("46226", "Market Sanctuary 09")
        AdvMarketTreeView.Nodes.Add("46219", "Market Sanctuary 10")
        AdvMarketTreeView.Nodes.Add("46184", "Market Sanctuary 11")
        AdvMarketTreeView.Nodes.Add("46213", "Market Sanctuary 12")
        AdvMarketTreeView.Nodes.Add("46152", "Market Sanctuary 13")
        AdvMarketTreeView.Nodes.Add("46185", "Market Sanctuary 14")
        AdvMarketTreeView.Nodes.Add("46201", "Market Sanctuary 15")
        AdvMarketTreeView.Nodes.Add("46193", "Market Sanctuary 16")
        AdvMarketTreeView.Nodes.Add("46197", "Market Sanctuary 17")
        AdvMarketTreeView.Nodes.Add("46189", "Market Sanctuary 18")
        AdvMarketTreeView.Nodes.Add("46169", "Market Sanctuary 19")
        AdvMarketTreeView.Nodes.Add("46172", "Market Sanctuary 20")
        AdvMarketTreeView.Nodes.Add("46177", "Market Alioth Moon I 1")
        AdvMarketTreeView.Nodes.Add("46215", "Market Alioth Moon I 2")
        AdvMarketTreeView.Nodes.Add("46168", "Market Alioth Moon IV 1")
        AdvMarketTreeView.Nodes.Add("46180", "Market Alioth Moon IV 2")
        AdvMarketTreeView.Nodes.Add("46211", "Market Jago 1")
        AdvMarketTreeView.Nodes.Add("46192", "Market Jago 2")
        AdvMarketTreeView.Nodes.Add("46181", "Market Jago 3")
        AdvMarketTreeView.Nodes.Add("46196", "Market Jago 4")
        AdvMarketTreeView.Nodes.Add("46212", "Market Jago 5")
        AdvMarketTreeView.Nodes.Add("46188", "Market Jago 6")
        AdvMarketTreeView.Nodes.Add("46208", "Market Madis 1")
        AdvMarketTreeView.Nodes.Add("46210", "Market Madis 2")
        AdvMarketTreeView.Nodes.Add("46182", "Market Madis 3")
        AdvMarketTreeView.Nodes.Add("46167", "Market Madis 4")
        AdvMarketTreeView.Nodes.Add("46161", "Market Madis 5")
        AdvMarketTreeView.Nodes.Add("46175", "Market Madis 6")
        AdvMarketTreeView.Nodes.Add("46200", "Market Madis Moon I 1")
        AdvMarketTreeView.Nodes.Add("46166", "Market Madis Moon I 2")
        AdvMarketTreeView.Nodes.Add("46153", "Market Madis Moon II 1")
        AdvMarketTreeView.Nodes.Add("46173", "Market Madis Moon II 2")
        AdvMarketTreeView.Nodes.Add("46199", "Market Madis Moon III 1")
        AdvMarketTreeView.Nodes.Add("46155", "Market Madis Moon III 2")
        AdvMarketTreeView.Nodes.Add("46156", "Market Teoma 1")
        AdvMarketTreeView.Nodes.Add("46165", "Market Teoma 2")
        AdvMarketTreeView.Nodes.Add("46195", "Market Teoma 3")
        AdvMarketTreeView.Nodes.Add("46218", "Market Teoma 4")
        AdvMarketTreeView.Nodes.Add("46162", "Market Teoma 5")
        AdvMarketTreeView.Nodes.Add("46157", "Market Teoma 6")
        AdvMarketTreeView.Nodes.Add("46170", "Market Thades 1")
        AdvMarketTreeView.Nodes.Add("46191", "Market Thades 2")
        AdvMarketTreeView.Nodes.Add("46206", "Market Thades 3")
        AdvMarketTreeView.Nodes.Add("46154", "Market Thades 4")
        AdvMarketTreeView.Nodes.Add("46205", "Market Thades 5")
        AdvMarketTreeView.Nodes.Add("46186", "Market Thades 6")
        AdvMarketTreeView.Nodes.Add("46203", "Market Thades Moon I 1")
        AdvMarketTreeView.Nodes.Add("46194", "Market Thades Moon I 2")
        AdvMarketTreeView.Nodes.Add("46209", "Market Thades Moon II 1")
        AdvMarketTreeView.Nodes.Add("46214", "Market Thades Moon II 2")
    End Sub

    Private Function GetMarketName(ByVal input As Integer)
        Dim result() As DataRow = API_Market_Table.Select("id = " & input)
        For Each row As DataRow In result
            Return row.Item(1)
        Next
    End Function

    Private Sub InitItemList()
        Dim categorynode1 As TreeNode = ItemTree.Nodes.Add("node1", "Aphelia Mission Packages")
        Dim categorynode11 As TreeNode = categorynode1.Nodes.Add("node11", "Very Large Mission Packages")
        Dim categorynode12 As TreeNode = categorynode1.Nodes.Add("node12", "Large Mission Packages")
        Dim categorynode13 As TreeNode = categorynode1.Nodes.Add("node13", "Medium Mission Packages")
        Dim categorynode14 As TreeNode = categorynode1.Nodes.Add("node14", "Small Mission Packages")
        Dim categorynode15 As TreeNode = categorynode1.Nodes.Add("node15", "Very Small Mission Packages")

        Dim categorynode2 As TreeNode = ItemTree.Nodes.Add("node2", "Consumables")
        Dim categorynode21 As TreeNode = categorynode2.Nodes.Add("node21", "Ammunition")
        Dim categorynode211 As TreeNode = categorynode21.Nodes.Add("node211", "Cannon Ammo")
        Dim categorynode2111 As TreeNode = categorynode211.Nodes.Add("node2111", "Cannon Ammo L")
        Dim categorynode21111 As TreeNode = categorynode2111.Nodes.Add("node21111", "Kinetic Ammo")
        categorynode21111.Nodes.Add("item2754186867", "Cannon Agile Kinetic Ammo L")
        categorynode21111.Nodes.Add("item1368644517", "Cannon Defense Kinetic Ammo L")
        categorynode21111.Nodes.Add("item3111934432", "Cannon Heavy Kinetic Ammo L")
        categorynode21111.Nodes.Add("item234876889", "Cannon Kinetic Ammo L")
        categorynode21111.Nodes.Add("item2564171448", "Cannon Precision Kinetic Ammo L")

        Dim categorynode21112 As TreeNode = categorynode2111.Nodes.Add("node21112", "Thermic Ammo")
        categorynode21112.Nodes.Add("item3564640746", "Cannon Agile Thermic Ammo L")
        categorynode21112.Nodes.Add("item99109453", "Cannon Defense Thermic Ammo L")
        categorynode21112.Nodes.Add("item3705351908", "Cannon Heavy Thermic Ammo L")
        categorynode21112.Nodes.Add("item2793941079", "Cannon Precision Thermic Ammo L")
        categorynode21112.Nodes.Add("item2150864517", "Cannon Thermic Ammo L")

        Dim categorynode2112 As TreeNode = categorynode211.Nodes.Add("node2112", "Cannon Ammo M")
        Dim categorynode21121 As TreeNode = categorynode2112.Nodes.Add("node21121", "Kinetic Ammo")
        categorynode21121.Nodes.Add("item3901365200", "Cannon Agile Kinetic Ammo M")
        categorynode21121.Nodes.Add("item3802426170", "Cannon Defense Kinetic Ammo M")
        categorynode21121.Nodes.Add("item2318901128", "Cannon Heavy Kinetic Ammo M")
        categorynode21121.Nodes.Add("item1087392944", "Cannon Kinetic Ammo M")
        categorynode21121.Nodes.Add("item1837088359", "Cannon Precision Kinetic Ammo M")

        Dim categorynode21122 As TreeNode = categorynode2112.Nodes.Add("node21122", "Thermic Ammo")
        categorynode21122.Nodes.Add("item1958427908", "Cannon Agile Thermic Ammo M")
        categorynode21122.Nodes.Add("item3352702648", "Cannon Defense Thermic Ammo M")
        categorynode21122.Nodes.Add("item1627746607", "Cannon Heavy Thermic Ammo M")
        categorynode21122.Nodes.Add("item1445049256", "Cannon Precision Thermic Ammo M")
        categorynode21122.Nodes.Add("item2886559338", "Cannon Thermic Ammo M")

        Dim categorynode2113 As TreeNode = categorynode211.Nodes.Add("node2113", "Cannon Ammo S")
        Dim categorynode21131 As TreeNode = categorynode2113.Nodes.Add("node21131", "Kinetic Ammo")
        categorynode21131.Nodes.Add("item2048035010", "Cannon Agile Kinetic Ammo S")
        categorynode21131.Nodes.Add("item52497197", "Cannon Defense Kinetic Ammo S")
        categorynode21131.Nodes.Add("item2013297395", "Cannon Heavy Kinetic Ammo S")
        categorynode21131.Nodes.Add("item864736227", "Cannon Kinetic Ammo S")
        categorynode21131.Nodes.Add("item1256032552", "Cannon Precision Kinetic Ammo S")

        Dim categorynode21132 As TreeNode = categorynode2113.Nodes.Add("node21132", "Thermic Ammo")
        categorynode21132.Nodes.Add("item2014631386", "Cannon Agile Thermic Ammo S")
        categorynode21132.Nodes.Add("item846420746", "Cannon Defense Thermic Ammo S")
        categorynode21132.Nodes.Add("item1912399735", "Cannon Heavy Thermic Ammo S")
        categorynode21132.Nodes.Add("item1081563239", "Cannon Precision Thermic Ammo S")
        categorynode21132.Nodes.Add("item3253142563", "Cannon Thermic Ammo S")

        Dim categorynode2114 As TreeNode = categorynode211.Nodes.Add("node2114", "Cannon Ammo XS")
        Dim categorynode21141 As TreeNode = categorynode2114.Nodes.Add("node21141", "Kinetic Ammo")
        categorynode21141.Nodes.Add("item2746947552", "Cannon Agile Kinetic Ammo XS")
        categorynode21141.Nodes.Add("item2680492642", "Cannon Defense Kinetic Ammo XS")
        categorynode21141.Nodes.Add("item1980351716", "Cannon Heavy Kinetic Ammo XS")
        categorynode21141.Nodes.Add("item3818049598", "Cannon Kinetic Ammo XS")
        categorynode21141.Nodes.Add("item3238320397", "Cannon Precision Kinetic Ammo XS")

        Dim categorynode21142 As TreeNode = categorynode2114.Nodes.Add("node21142", "Thermic Ammo")
        categorynode21142.Nodes.Add("item370579567", "Cannon Agile Thermic Ammo XS")
        categorynode21142.Nodes.Add("item147467923", "Cannon Defense Thermic Ammo XS")
        categorynode21142.Nodes.Add("item726551231", "Cannon Heavy Thermic Ammo XS")
        categorynode21142.Nodes.Add("item2917884317", "Cannon Precision Thermic Ammo XS")
        categorynode21142.Nodes.Add("item3607061517", "Cannon Thermic Ammo XS")

        Dim categorynode212 As TreeNode = categorynode21.Nodes.Add("node212", "Laser Ammo")
        Dim categorynode2121 As TreeNode = categorynode212.Nodes.Add("node2121", "Laser Ammo L")
        Dim categorynode21211 As TreeNode = categorynode2121.Nodes.Add("node21211", "Electromagnetic Ammo")
        categorynode21211.Nodes.Add("item2170035253", "Laser Agile Electromagnetic Ammo L")
        categorynode21211.Nodes.Add("item2006239134", "Laser Defense Electromagnetic Ammo L")
        categorynode21211.Nodes.Add("item2465107224", "Laser Electromagnetic Ammo L")
        categorynode21211.Nodes.Add("item2281477958", "Laser Heavy Electromagnetic Ammo L")
        categorynode21211.Nodes.Add("item1664787227", "Laser Precision Electromagnetic Ammo L")

        Dim categorynode21212 As TreeNode = categorynode2121.Nodes.Add("node21212", "Thermic Ammo")
        categorynode21212.Nodes.Add("item154196902", "Laser Agile Thermic Ammo L")
        categorynode21212.Nodes.Add("item2619099776", "Laser Defense Thermic Ammo L")
        categorynode21212.Nodes.Add("item518572846", "Laser Heavy Thermic Ammo L")
        categorynode21212.Nodes.Add("item36119774", "Laser Precision Thermic Ammo L")
        categorynode21212.Nodes.Add("item1068250257", "Laser Thermic Ammo L")

        Dim categorynode2122 As TreeNode = categorynode212.Nodes.Add("node2122", "Laser Ammo M")
        Dim categorynode21221 As TreeNode = categorynode2122.Nodes.Add("node21221", "Electromagnetic Ammo")
        categorynode21221.Nodes.Add("item2948970732", "Laser Agile Electromagnetic Ammo M")
        categorynode21221.Nodes.Add("item483699778", "Laser Defense Electromagnetic Ammo M")
        categorynode21221.Nodes.Add("item1693315392", "Laser Electromagnetic Ammo M")
        categorynode21221.Nodes.Add("item220854647", "Laser Heavy Electromagnetic Ammo M")
        categorynode21221.Nodes.Add("item1610308198", "Laser Precision Electromagnetic Ammo M")

        Dim categorynode21222 As TreeNode = categorynode2122.Nodes.Add("node21222", "Thermic Ammo")
        categorynode21222.Nodes.Add("item212874547", "Laser Agile Thermic Ammo M")
        categorynode21222.Nodes.Add("item1230483435", "Laser Defense Thermic Ammo M")
        categorynode21222.Nodes.Add("item984810201", "Laser Heavy Thermic Ammo M")
        categorynode21222.Nodes.Add("item3708417017", "Laser Precision Thermic Ammo M")
        categorynode21222.Nodes.Add("item2843836124", "Laser Thermic Ammo M")

        Dim categorynode2123 As TreeNode = categorynode212.Nodes.Add("node2123", "Laser Ammo S")
        Dim categorynode21231 As TreeNode = categorynode2123.Nodes.Add("node21231", "Electromagnetic Ammo")
        categorynode21231.Nodes.Add("item3098134459", "Laser Agile Electromagnetic Ammo S")
        categorynode21231.Nodes.Add("item2667876309", "Laser Defense Electromagnetic Ammo S")
        categorynode21231.Nodes.Add("item1921694649", "Laser Electromagnetic Ammo S")
        categorynode21231.Nodes.Add("item1929049234", "Laser Heavy Electromagnetic Ammo S")
        categorynode21231.Nodes.Add("item4088065384", "Laser Precision Electromagnetic Ammo S")

        Dim categorynode21232 As TreeNode = categorynode2123.Nodes.Add("node21232", "Thermic Ammo")
        categorynode21232.Nodes.Add("item3423590348", "Laser Agile Thermic Ammo S")
        categorynode21232.Nodes.Add("item1933474332", "Laser Defense Thermic Ammo S")
        categorynode21232.Nodes.Add("item1750052574", "Laser Heavy Thermic Ammo S")
        categorynode21232.Nodes.Add("item3820970963", "Laser Precision Thermic Ammo S")
        categorynode21232.Nodes.Add("item1363871248", "Laser Thermic Ammo S")

        Dim categorynode2124 As TreeNode = categorynode212.Nodes.Add("node2124", "Laser Ammo XS")
        Dim categorynode21241 As TreeNode = categorynode2124.Nodes.Add("node21241", "Electromagnetic Ammo")
        categorynode21241.Nodes.Add("item552630719", "Laser Agile Electromagnetic Ammo XS")
        categorynode21241.Nodes.Add("item1067471403", "Laser Defense Electromagnetic Ammo XS")
        categorynode21241.Nodes.Add("item3637130597", "Laser Electromagnetic Ammo XS")
        categorynode21241.Nodes.Add("item902792933", "Laser Heavy Electromagnetic Ammo XS")
        categorynode21241.Nodes.Add("item3539993652", "Laser Precision Electromagnetic Ammo XS")

        Dim categorynode21242 As TreeNode = categorynode2124.Nodes.Add("node21242", "Thermic Ammo")
        categorynode21242.Nodes.Add("item570530668", "Laser Agile Thermic Ammo XS")
        categorynode21242.Nodes.Add("item839159661", "Laser Defense Thermic Ammo XS")
        categorynode21242.Nodes.Add("item2678465305", "Laser Heavy Thermic Ammo XS")
        categorynode21242.Nodes.Add("item1765328811", "Laser Precision Thermic Ammo XS")
        categorynode21242.Nodes.Add("item4135531540", "Laser Thermic Ammo XS")

        Dim categorynode213 As TreeNode = categorynode21.Nodes.Add("node213", "Missile Pod Ammo")
        Dim categorynode2131 As TreeNode = categorynode213.Nodes.Add("node2131", "Missile Pod Ammo L")
        Dim categorynode21311 As TreeNode = categorynode2131.Nodes.Add("node21311", "Antimatter Ammo")
        categorynode21311.Nodes.Add("item3594012056", "Missile Agile Antimatter Ammo L")
        categorynode21311.Nodes.Add("item995805029", "Missile Antimatter Ammo L")
        categorynode21311.Nodes.Add("item579968086", "Missile Defense Antimatter Ammo L")
        categorynode21311.Nodes.Add("item3376140874", "Missile Heavy Antimatter Ammo L")
        categorynode21311.Nodes.Add("item3164761417", "Missile Precision Antimatter Ammo L")

        Dim categorynode21312 As TreeNode = categorynode2131.Nodes.Add("node21312", "Kinetic Ammo")
        categorynode21312.Nodes.Add("item2529340738", "Missile Agile Kinetic Ammo L")
        categorynode21312.Nodes.Add("item1186613579", "Missile Defense Kinetic Ammo L")
        categorynode21312.Nodes.Add("item3073125595", "Missile Heavy Kinetic Ammo L")
        categorynode21312.Nodes.Add("item934893004", "Missile Kinetic Ammo L")
        categorynode21312.Nodes.Add("item897887498", "Missile Precision Kinetic Ammo L")

        Dim categorynode2132 As TreeNode = categorynode213.Nodes.Add("node2132", "Missile Pod Ammo M")
        Dim categorynode21321 As TreeNode = categorynode2132.Nodes.Add("node21321", "Antimatter Ammo")
        categorynode21321.Nodes.Add("item326385703", "Missile Agile Antimatter Ammo M")
        categorynode21321.Nodes.Add("item403006216", "Missile Antimatter Ammo M")
        categorynode21321.Nodes.Add("item3987182986", "Missile Defense Antimatter Ammo M")
        categorynode21321.Nodes.Add("item291497016", "Missile Heavy Antimatter Ammo M")
        categorynode21321.Nodes.Add("item144252385", "Missile Precision Antimatter Ammo M")

        Dim categorynode21322 As TreeNode = categorynode2132.Nodes.Add("node21322", "Kinetic Ammo")
        categorynode21322.Nodes.Add("item1491281175", "Missile Agile Kinetic Ammo M")
        categorynode21322.Nodes.Add("item397326901", "Missile Defense Kinetic Ammo M")
        categorynode21322.Nodes.Add("item1209270788", "Missile Heavy Kinetic Ammo M")
        categorynode21322.Nodes.Add("item3718373809", "Missile Kinetic Ammo M")
        categorynode21322.Nodes.Add("item871384738", "Missile Precision Kinetic Ammo M")

        Dim categorynode2133 As TreeNode = categorynode213.Nodes.Add("node2133", "Missile Pod Ammo S")
        Dim categorynode21331 As TreeNode = categorynode2133.Nodes.Add("node21331", "Antimatter Ammo")
        categorynode21331.Nodes.Add("item1284945646", "Missile Agile Antimatter Ammo S")
        categorynode21331.Nodes.Add("item2425505244", "Missile Antimatter Ammo S")
        categorynode21331.Nodes.Add("item116711443", "Missile Defense Antimatter Ammo S")
        categorynode21331.Nodes.Add("item1333805710", "Missile Heavy Antimatter Ammo S")
        categorynode21331.Nodes.Add("item2982583326", "Missile Precision Antimatter Ammo S")

        Dim categorynode21332 As TreeNode = categorynode2133.Nodes.Add("node21332", "Kinetic Ammo")
        categorynode21332.Nodes.Add("item2679053199", "Missile Agile Kinetic Ammo S")
        categorynode21332.Nodes.Add("item1256805327", "Missile Defense Kinetic Ammo S")
        categorynode21332.Nodes.Add("item578039658", "Missile Heavy Kinetic Ammo S")
        categorynode21332.Nodes.Add("item108337911", "Missile Kinetic Ammo S")
        categorynode21332.Nodes.Add("item2116379443", "Missile Precision Kinetic Ammo S")

        Dim categorynode2134 As TreeNode = categorynode213.Nodes.Add("node2134", "Missile Pod Ammo XS")
        Dim categorynode21341 As TreeNode = categorynode2134.Nodes.Add("node21341", "Antimatter Ammo")
        categorynode21341.Nodes.Add("item2340151566", "Missile Agile Antimatter Ammo XS")
        categorynode21341.Nodes.Add("item2845912456", "Missile Antimatter Ammo XS")
        categorynode21341.Nodes.Add("item2059964042", "Missile Defense Antimatter Ammo XS")
        categorynode21341.Nodes.Add("item1154972320", "Missile Heavy Antimatter Ammo XS")
        categorynode21341.Nodes.Add("item2239958675", "Missile Precision Antimatter Ammo XS")

        Dim categorynode21342 As TreeNode = categorynode2134.Nodes.Add("node21342", "Kinetic Ammo")
        categorynode21342.Nodes.Add("item2148925933", "Missile Agile Kinetic Ammo XS")
        categorynode21342.Nodes.Add("item3939368391", "Missile Defense Kinetic Ammo XS")
        categorynode21342.Nodes.Add("item2591026571", "Missile Heavy Kinetic Ammo XS")
        categorynode21342.Nodes.Add("item2392386214", "Missile Kinetic Ammo XS")
        categorynode21342.Nodes.Add("item1503181393", "Missile Precision Kinetic Ammo XS")

        Dim categorynode214 As TreeNode = categorynode21.Nodes.Add("node214", "Railgun Ammo")
        Dim categorynode2141 As TreeNode = categorynode214.Nodes.Add("node2141", "Railgun Ammo L")
        Dim categorynode21411 As TreeNode = categorynode2141.Nodes.Add("node21411", "Antimatter Ammo")
        categorynode21411.Nodes.Add("item994404082", "Railgun Agile Antimatter Ammo L")
        categorynode21411.Nodes.Add("item4091052814", "Railgun Antimatter Ammo L")
        categorynode21411.Nodes.Add("item1377917611", "Railgun Defense Antimatter Ammo L")
        categorynode21411.Nodes.Add("item1555786609", "Railgun Heavy Antimatter Ammo L")
        categorynode21411.Nodes.Add("item2009039852", "Railgun Precision Antimatter Ammo L")

        Dim categorynode21412 As TreeNode = categorynode2141.Nodes.Add("node21412", "Electromagnetic Ammo")
        categorynode21412.Nodes.Add("item493646316", "Railgun Agile Electromagnetic Ammo L")
        categorynode21412.Nodes.Add("item2997406270", "Railgun Defense Electromagnetic Ammo L")
        categorynode21412.Nodes.Add("item19332250", "Railgun Electromagnetic Ammo L")
        categorynode21412.Nodes.Add("item985599166", "Railgun Heavy Electromagnetic Ammo L")
        categorynode21412.Nodes.Add("item3711223735", "Railgun Precision Electromagnetic Ammo L")

        Dim categorynode2142 As TreeNode = categorynode214.Nodes.Add("node2142", "Railgun Ammo M")
        Dim categorynode21421 As TreeNode = categorynode2142.Nodes.Add("node21421", "Antimatter Ammo")
        categorynode21421.Nodes.Add("item2753235550", "Railgun Agile Antimatter Ammo M")
        categorynode21421.Nodes.Add("item3025930763", "Railgun Antimatter Ammo M")
        categorynode21421.Nodes.Add("item2519489329", "Railgun Defense Antimatter Ammo M")
        categorynode21421.Nodes.Add("item1129867076", "Railgun Heavy Antimatter Ammo M")
        categorynode21421.Nodes.Add("item1378313789", "Railgun Precision Antimatter Ammo M")

        Dim categorynode21422 As TreeNode = categorynode2142.Nodes.Add("node21422", "Electromagnetic Ammo")
        categorynode21422.Nodes.Add("item2401068335", "Railgun Agile Electromagnetic Ammo M")
        categorynode21422.Nodes.Add("item2547387530", "Railgun Defense Electromagnetic Ammo M")
        categorynode21422.Nodes.Add("item1314738719", "Railgun Electromagnetic Ammo M")
        categorynode21422.Nodes.Add("item711588165", "Railgun Heavy Electromagnetic Ammo M")
        categorynode21422.Nodes.Add("item3778585474", "Railgun Precision Electromagnetic Ammo M")

        Dim categorynode2143 As TreeNode = categorynode214.Nodes.Add("node2143", "Railgun Ammo S")
        Dim categorynode21431 As TreeNode = categorynode2143.Nodes.Add("node21431", "Antimatter Ammo")
        categorynode21431.Nodes.Add("item2765153031", "Railgun Agile Antimatter Ammo S")
        categorynode21431.Nodes.Add("item2665059784", "Railgun Antimatter Ammo S")
        categorynode21431.Nodes.Add("item2454971316", "Railgun Defense Antimatter Ammo S")
        categorynode21431.Nodes.Add("item2944291964", "Railgun Heavy Antimatter Ammo S")
        categorynode21431.Nodes.Add("item2423442023", "Railgun Precision Antimatter Ammo S")

        Dim categorynode21432 As TreeNode = categorynode2143.Nodes.Add("node21432", "Electromagnetic Ammo")
        categorynode21432.Nodes.Add("item1818470694", "Railgun Agile Electromagnetic Ammo S")
        categorynode21432.Nodes.Add("item2890607046", "Railgun Defense Electromagnetic Ammo S")
        categorynode21432.Nodes.Add("item2277755297", "Railgun Electromagnetic Ammo S")
        categorynode21432.Nodes.Add("item3384068103", "Railgun Heavy Electromagnetic Ammo S")
        categorynode21432.Nodes.Add("item3511898141", "Railgun Precision Electromagnetic Ammo S")

        Dim categorynode2144 As TreeNode = categorynode214.Nodes.Add("node2144", "Railgun Ammo XS")
        Dim categorynode21441 As TreeNode = categorynode2144.Nodes.Add("node21441", "Antimatter Ammo")
        categorynode21441.Nodes.Add("item2562077926", "Railgun Agile Antimatter Ammo XS")
        categorynode21441.Nodes.Add("item3669030673", "Railgun Antimatter Ammo XS")
        categorynode21441.Nodes.Add("item1685710165", "Railgun Defense Antimatter Ammo XS")
        categorynode21441.Nodes.Add("item2975180925", "Railgun Heavy Antimatter Ammo XS")
        categorynode21441.Nodes.Add("item2897347844", "Railgun Precision Antimatter Ammo XS")

        Dim categorynode21442 As TreeNode = categorynode2144.Nodes.Add("node21442", "Electromagnetic Ammo")
        categorynode21442.Nodes.Add("item4121476880", "Railgun Agile Electromagnetic Ammo XS")
        categorynode21442.Nodes.Add("item1190298485", "Railgun Defense Electromagnetic Ammo XS")
        categorynode21442.Nodes.Add("item2513950249", "Railgun Electromagnetic Ammo XS")
        categorynode21442.Nodes.Add("item671997275", "Railgun Heavy Electromagnetic Ammo XS")
        categorynode21442.Nodes.Add("item2661753045", "Railgun Precision Electromagnetic Ammo XS")

        Dim categorynode215 As TreeNode = categorynode21.Nodes.Add("node215", "Stasis Ammo")
        categorynode215.Nodes.Add("item2863276263", "Stasis Ammo XS")
        categorynode215.Nodes.Add("item4079234375", "Stasis Ammo S")
        categorynode215.Nodes.Add("item3461950856", "Stasis Ammo M")
        categorynode215.Nodes.Add("item155987084", "Stasis Ammo L")

        Dim categorynode22 As TreeNode = categorynode2.Nodes.Add("node1", "Fireworks")
        Dim categorynode221 As TreeNode = categorynode22.Nodes.Add("node221", "Fireball-shaped Fireworks")
        categorynode221.Nodes.Add("item4235678285", "Firework fireball Gold")
        categorynode221.Nodes.Add("item3495319529", "Firework fireball blue")
        categorynode221.Nodes.Add("item123277277", "Firework fireball green")
        categorynode221.Nodes.Add("item3549742966", "Firework fireball purple")
        categorynode221.Nodes.Add("item1744641587", "Firework fireball red")
        categorynode221.Nodes.Add("item4222908689", "Firework fireball silver")

        Dim categorynode222 As TreeNode = categorynode22.Nodes.Add("node222", "Palm tree-shaped Fireworks")
        categorynode222.Nodes.Add("item284619744", "Firework palm tree Gold")
        categorynode222.Nodes.Add("item283241767", "Firework palm tree blue")
        categorynode222.Nodes.Add("item3456348276", "Firework palm tree green")
        categorynode222.Nodes.Add("item1474187970", "Firework palm tree purple")
        categorynode222.Nodes.Add("item1515628537", "Firework palm tree red")
        categorynode222.Nodes.Add("item2506643916", "Firework palm tree silver")

        Dim categorynode223 As TreeNode = categorynode22.Nodes.Add("node223", "Ring-shaped Fireworks")
        categorynode223.Nodes.Add("item2607498157", "Firework ring Gold")
        categorynode223.Nodes.Add("item2602090525", "Firework ring blue")
        categorynode223.Nodes.Add("item79542399", "Firework ring green")
        categorynode223.Nodes.Add("item992711325", "Firework ring purple")
        categorynode223.Nodes.Add("item197061275", "Firework ring red")
        categorynode223.Nodes.Add("item2258792098", "Firework ring silver")

        Dim categorynode224 As TreeNode = categorynode22.Nodes.Add("node224", "Sparks shaped Fireworks")
        categorynode224.Nodes.Add("item3453793951", "Firework sparks Gold")
        categorynode224.Nodes.Add("item804042991", "Firework sparks blue")
        categorynode224.Nodes.Add("item2232682987", "Firework sparks green")
        categorynode224.Nodes.Add("item928175277", "Firework sparks purple")
        categorynode224.Nodes.Add("item3237575985", "Firework sparks red")
        categorynode224.Nodes.Add("item96831487", "Firework sparks silver")

        Dim categorynode23 As TreeNode = categorynode2.Nodes.Add("node23", "Relic Plasmas")
        categorynode23.Nodes.Add("item1769135512", "Relic Plasma Decem L")
        categorynode23.Nodes.Add("item1831558336", "Relic Plasma Duo L")
        categorynode23.Nodes.Add("item1831557945", "Relic Plasma Novem L")
        categorynode23.Nodes.Add("item1831558342", "Relic Plasma Octo L")
        categorynode23.Nodes.Add("item1831558338", "Relic Plasma Quattuor L")
        categorynode23.Nodes.Add("item1831558341", "Relic Plasma Quinque L")
        categorynode23.Nodes.Add("item1831558343", "Relic Plasma Septem L")
        categorynode23.Nodes.Add("item1831558340", "Relic Plasma Sex L")
        categorynode23.Nodes.Add("item1831558339", "Relic Plasma Tres L")
        categorynode23.Nodes.Add("item1831558337", "Relic Plasma Unus L")

        Dim categorynode24 As TreeNode = categorynode2.Nodes.Add("node24", "Scraps")
        Dim categorynode241 As TreeNode = categorynode24.Nodes.Add("node241", "Advanced Scraps")
        categorynode241.Nodes.Add("item2115439708", "Lithium Scrap")
        categorynode241.Nodes.Add("item409671366", "Nickel Scrap")
        categorynode241.Nodes.Add("item3814734889", "Silver Scrap")
        categorynode241.Nodes.Add("item1423148560", "Sulfur Scrap")

        Dim categorynode242 As TreeNode = categorynode24.Nodes.Add("node242", "Basic Scraps")
        categorynode242.Nodes.Add("item2417840347", "Aluminium Scrap")
        categorynode242.Nodes.Add("item3857279161", "Carbon Scrap")
        categorynode242.Nodes.Add("item2558961706", "Iron Scrap")
        categorynode242.Nodes.Add("item4063983201", "Silicon Scrap")

        Dim categorynode243 As TreeNode = categorynode24.Nodes.Add("node243", "Exotic Scraps")
        categorynode243.Nodes.Add("item1182663952", "Manganese Scrap")
        categorynode243.Nodes.Add("item877202037", "Niobium Scrap")
        categorynode243.Nodes.Add("item2165650011", "Titanium Scrap")
        categorynode243.Nodes.Add("item3307634000", "Vanadium Scrap")

        Dim categorynode244 As TreeNode = categorynode24.Nodes.Add("node244", "Rare Scraps")
        categorynode244.Nodes.Add("item1370993297", "Cobalt Scrap")
        categorynode244.Nodes.Add("item3150580281", "Fluorine Scrap")
        categorynode244.Nodes.Add("item1032380176", "Gold Scrap")
        categorynode244.Nodes.Add("item270611770", "Scandium Scrap")

        Dim categorynode245 As TreeNode = categorynode24.Nodes.Add("node245", "Uncommon Scraps")
        categorynode245.Nodes.Add("item1251531294", "Calcium Scrap")
        categorynode245.Nodes.Add("item409040753", "Chromium Scrap")
        categorynode245.Nodes.Add("item3630798120", "Copper Scrap")
        categorynode245.Nodes.Add("item1831205658", "Sodium Scrap")

        Dim categorynode25 As TreeNode = categorynode2.Nodes.Add("node24", "Warp Cell")
        categorynode25.Nodes.Add("item1339253011", "Warp Cell")

        Dim categorynode3 As TreeNode = ItemTree.Nodes.Add("node3", "Data Item")
        Dim categorynode31 As TreeNode = categorynode3.Nodes.Add("node31", "Schematic Copies")
        categorynode31.Nodes.Add("3077761447","Atmospheric Fuel Schematic Copy")
        categorynode31.Nodes.Add("674258992", "Bonsai Schematic Copy")
        categorynode31.Nodes.Add("1477134528", "Construct Support  XS Schematic Copy")
        categorynode31.Nodes.Add("784932973", "Construct Support L Schematic Copy")
        categorynode31.Nodes.Add("1861676811", "Construct Support M Schematic Copy")
        categorynode31.Nodes.Add("1224468838", "Construct Support S Schematic Copy")
        categorynode31.Nodes.Add("1202149588", "Core Unit L Schematic Copy")
        categorynode31.Nodes.Add("1417495315", "Core Unit M Schematic Copy")
        categorynode31.Nodes.Add("1213081642", "Core Unit S Schematic Copy")
        categorynode31.Nodes.Add("120427296", "Core Unit XS Schematic Copy")
        categorynode31.Nodes.Add("3992802706", "Rocket Fuels Schematic Copy")
        categorynode31.Nodes.Add("1917988879", "Space Fuels Schematic Copy")
        categorynode31.Nodes.Add("318308564", "Territory Unit Schematic Copy")
        categorynode31.Nodes.Add("2068774589", "Tier 1 L Element Schematic Copy")
        categorynode31.Nodes.Add("2066101218", "Tier 1 M Element Schematic Copy")
        categorynode31.Nodes.Add("2479827059", "Tier 1 Product Honeycomb Schematic Copy")
        categorynode31.Nodes.Add("690638651", "Tier 1 Product Material Schematic Copy")
        categorynode31.Nodes.Add("4148773283", "Tier 1 S Element Schematic Copy")
        categorynode31.Nodes.Add("304578197", "Tier 1 XL Element Schematic Copy")
        categorynode31.Nodes.Add("1910482623", "Tier 1 XS Element Schematic Copy")
        categorynode31.Nodes.Add("512435856", "Tier 2 Ammo L Schematic Copy")
        categorynode31.Nodes.Add("399761377", "Tier 2 Ammo M Schematic Copy")
        categorynode31.Nodes.Add("3336558558", "Tier 2 Ammo S Schematic Copy")
        categorynode31.Nodes.Add("326757369", "Tier 2 Ammo XS Schematic Copy")
        categorynode31.Nodes.Add("616601802", "Tier 2 L Element Schematic Copy")
        categorynode31.Nodes.Add("2726927301", "Tier 2 M Element Schematic Copy")
        categorynode31.Nodes.Add("632722426", "Tier 2 Product Honeycomb Schematic Copy")
        categorynode31.Nodes.Add("4073976374", "Tier 2 Product Material Schematic Copy")
        categorynode31.Nodes.Add("625377458", "Tier 2 Pure Honeycomb Schematic Copy")
        categorynode31.Nodes.Add("3332597852", "Tier 2 Pure Material Schematic Copy")
        categorynode31.Nodes.Add("1752968727", "Tier 2 S Element Schematic Copy")
        categorynode31.Nodes.Add("1952035274", "Tier 2 Scrap Schematic Copy")
        categorynode31.Nodes.Add("3677281424", "Tier 2 XL Element Schematic Copy")
        categorynode31.Nodes.Add("2096799848", "Tier 2 XS Element Schematic Copy")
        categorynode31.Nodes.Add("2913149958", "Tier 3 Ammo L Schematic Copy")
        categorynode31.Nodes.Add("3125069948", "Tier 3 Ammo M Schematic Copy")
        categorynode31.Nodes.Add("1705420479", "Tier 3 Ammo S Schematic Copy")
        categorynode31.Nodes.Add("2413250793", "Tier 3 Ammo XS Schematic Copy")
        categorynode31.Nodes.Add("1427639881", "Tier 3 L Element Schematic Copy")
        categorynode31.Nodes.Add("3713463144", "Tier 3 M Element Schematic Copy")
        categorynode31.Nodes.Add("2343247971", "Tier 3 Product Honeycomb Schematic Copy")
        categorynode31.Nodes.Add("3707339625", "Tier 3 Product Material Schematic Copy")
        categorynode31.Nodes.Add("4221430495", "Tier 3 Pure Honeycomb Schematic Copy")
        categorynode31.Nodes.Add("2003602752", "Tier 3 Pure Material Schematic Copy")
        categorynode31.Nodes.Add("425872842", "Tier 3 S Element Schematic Copy")
        categorynode31.Nodes.Add("2566982373", "Tier 3 Scrap Schematic Copy")
        categorynode31.Nodes.Add("109515712", "Tier 3 XL Element Schematic Copy")
        categorynode31.Nodes.Add("787727253", "Tier 3 XS Element Schematic Copy")
        categorynode31.Nodes.Add("2557110259", "Tier 4 Ammo L Schematic Copy")
        categorynode31.Nodes.Add("3847207511", "Tier 4 Ammo M Schematic Copy")
        categorynode31.Nodes.Add("3636126848", "Tier 4 Ammo S Schematic Copy")
        categorynode31.Nodes.Add("2293088862", "Tier 4 Ammo XS Schematic Copy")
        categorynode31.Nodes.Add("1614573474", "Tier 4 L Element Schematic Copy")
        categorynode31.Nodes.Add("3881438643", "Tier 4 M Element Schematic Copy")
        categorynode31.Nodes.Add("3743434922", "Tier 4 Product Honeycomb Schematic Copy")
        categorynode31.Nodes.Add("2485530515", "Tier 4 Product Material Schematic Copy")
        categorynode31.Nodes.Add("99491659", "Tier 4 Pure Honeycomb Schematic Copy")
        categorynode31.Nodes.Add("2326433413", "Tier 4 Pure Material Schematic Copy")
        categorynode31.Nodes.Add("3890840920", "Tier 4 S Element Schematic Copy")
        categorynode31.Nodes.Add("1045229911", "Tier 4 Scrap Schematic Copy")
        categorynode31.Nodes.Add("1974208697", "Tier 4 XL Element Schematic Copy")
        categorynode31.Nodes.Add("210052275", "Tier 4 XS Element Schematic Copy")
        categorynode31.Nodes.Add("86717297", "Tier 5 L Element Schematic Copy")
        categorynode31.Nodes.Add("3672319913", "Tier 5 M Element Schematic Copy")
        categorynode31.Nodes.Add("1885016266", "Tier 5 Product Honeycomb Schematic Copy")
        categorynode31.Nodes.Add("2752973532", "Tier 5 Product Material Schematic Copy")
        categorynode31.Nodes.Add("3303272691", "Tier 5 Pure Honeycomb Schematic Copy")
        categorynode31.Nodes.Add("1681671893", "Tier 5 Pure Material Schematic Copy")
        categorynode31.Nodes.Add("880043901", "Tier 5 S Element Schematic Copy")
        categorynode31.Nodes.Add("2702634486", "Tier 5 Scrap Schematic Copy")
        categorynode31.Nodes.Add("1320378000", "Tier 5 XL Element Schematic Copy")
        categorynode31.Nodes.Add("1513927457", "Tier 5 XS Element Schematic Copy")
        categorynode31.Nodes.Add("3437488324", "Warp Beacon Schematic Copy")
        categorynode31.Nodes.Add("363077945", "Warp Cell Schematic Copy")


        Dim categorynode4 As TreeNode = ItemTree.Nodes.Add("node4", "Elements")
        Dim categorynode41 As TreeNode = categorynode4.Nodes.Add("node41", "Combat & Defense Elements")

        Dim categorynode410 As TreeNode = categorynode41.Nodes.Add("node410", "Base Shield Generators")
        categorynode410.Nodes.Add("item1430252067", "Base Shield Generator XL")

        Dim categorynode411 As TreeNode = categorynode41.Nodes.Add("node411", "Radar")
        Dim categorynode4111 As TreeNode = categorynode411.Nodes.Add("node4111", "Atmospheric Radar")
        categorynode4111.Nodes.Add("item4213791403", "Atmospheric Radar S")
        categorynode4111.Nodes.Add("item612626034", "Atmospheric Radar M")
        categorynode4111.Nodes.Add("item3094514782", "Atmospheric Radar L")

        Dim categorynode4112 As TreeNode = categorynode411.Nodes.Add("node4112", "Space Radars")
        Dim categorynode41121 As TreeNode = categorynode4112.Nodes.Add("node41121", "Space Radars L")
        categorynode41121.Nodes.Add("item2075264944", "Advanced Phased-Array Space Radar L")
        categorynode41121.Nodes.Add("item3250064332", "Advanced Protected Space Radar L")
        categorynode41121.Nodes.Add("item3612800224", "Advanced Quick-Wired Space Radar L")
        categorynode41121.Nodes.Add("item2075264590", "Exotic Phased-Array Space Radar L")
        categorynode41121.Nodes.Add("item3250064334", "Exotic Protected Space Radar L")
        categorynode41121.Nodes.Add("item3612800254", "Exotic Quick-Wired Space Radar L")
        categorynode41121.Nodes.Add("item2075264591", "Rare Phased-Array Space Radar L")
        categorynode41121.Nodes.Add("item3250064333", "Rare Protected Space Radar L")
        categorynode41121.Nodes.Add("item3612800255", "Rare Quick-Wired Space Radar L")
        categorynode41121.Nodes.Add("item2802863920", "Space Radar L")

        Dim categorynode41122 As TreeNode = categorynode4112.Nodes.Add("node41122", "Space Radars M")
        categorynode41122.Nodes.Add("item1707018154", "Advanced Phased-Array Space Radar M")
        categorynode41122.Nodes.Add("item3060580950", "Advanced Protected Space Radar M")
        categorynode41122.Nodes.Add("item2608116212", "Advanced Quick-Wired Space Radar M")
        categorynode41122.Nodes.Add("item1707018148", "Exotic Phased-Array Space Radar M")
        categorynode41122.Nodes.Add("item3060580944", "Exotic Protected Space Radar M")
        categorynode41122.Nodes.Add("item2608116214", "Exotic Quick-Wired Space Radar M")
        categorynode41122.Nodes.Add("item1707018149", "Rare Phased-Array Space Radar M")
        categorynode41122.Nodes.Add("item3060580945", "Rare Protected Space Radar M")
        categorynode41122.Nodes.Add("item2608116213", "Rare Quick-Wired Space Radar M")
        categorynode41122.Nodes.Add("item3831485995", "Space Radar M")

        Dim categorynode41123 As TreeNode = categorynode4112.Nodes.Add("node41123", "Space Radars S")
        categorynode41123.Nodes.Add("item809783408", "Advanced Phased-Array Space Radar S")
        categorynode41123.Nodes.Add("item2375197139", "Advanced Protected Space Radar S")
        categorynode41123.Nodes.Add("item838245688", "Advanced Quick-Wired Space Radar S")
        categorynode41123.Nodes.Add("item809783310", "Exotic Phased-Array Space Radar S")
        categorynode41123.Nodes.Add("item2375197137", "Exotic Protected Space Radar S")
        categorynode41123.Nodes.Add("item838245690", "Exotic Quick-Wired Space Radar S")
        categorynode41123.Nodes.Add("item809783311", "Rare Phased-Array Space Radar S")
        categorynode41123.Nodes.Add("item2375197136", "Rare Protected Space Radar S")
        categorynode41123.Nodes.Add("item838245691", "Rare Quick-Wired Space Radar S")
        categorynode41123.Nodes.Add("item4118496992", "Space Radar S")

        categorynode41.Nodes.Add("item774130122", "Repair Unit")

        Dim categorynode412 As TreeNode = categorynode41.Nodes.Add("node412", "Shield Generator")
        Dim categorynode4121 As TreeNode = categorynode412.Nodes.Add("node4121", "Shield Generators L")
        categorynode4121.Nodes.Add("item2034818941", "Advanced Shield Generator L")
        categorynode4121.Nodes.Add("item1747277189", "Exotic Active Shield Generator L")
        categorynode4121.Nodes.Add("item3840257886", "Exotic Capacitor Shield Generator L")
        categorynode4121.Nodes.Add("item2209766327", "Exotic Variable Shield Generator L")
        categorynode4121.Nodes.Add("item982995683", "Rare Active Shield Generator L")
        categorynode4121.Nodes.Add("item1478631104", "Rare Capacitor Shield Generator L")
        categorynode4121.Nodes.Add("item1486568571", "Rare Variable Shield Generator L")

        Dim categorynode4122 As TreeNode = categorynode412.Nodes.Add("node4122", "Shield Generators M")
        categorynode4122.Nodes.Add("item254923774", "Advanced Shield Generator M")
        categorynode4122.Nodes.Add("item484538921", "Exotic Active Shield Generator M")
        categorynode4122.Nodes.Add("item2037602890", "Exotic Capacitor Shield Generator M")
        categorynode4122.Nodes.Add("item3343633564", "Exotic Variable Shield Generator M")
        categorynode4122.Nodes.Add("item1514631881", "Rare Active Shield Generator M")
        categorynode4122.Nodes.Add("item2098965040", "Rare Capacitor Shield Generator M")
        categorynode4122.Nodes.Add("item1401343832", "Rare Variable Shield Generator M")

        Dim categorynode4123 As TreeNode = categorynode412.Nodes.Add("node4123", "Shield Generators S")
        categorynode4123.Nodes.Add("item3696387320", "Advanced Shield Generator S")
        categorynode4123.Nodes.Add("item2842824007", "Exotic Active Shield Generator S")
        categorynode4123.Nodes.Add("item2602781071", "Exotic Capacitor Shield Generator S")
        categorynode4123.Nodes.Add("item4081549548", "Exotic Variable Shield Generator S")
        categorynode4123.Nodes.Add("item4128180027", "Rare Active Shield Generator S")
        categorynode4123.Nodes.Add("item2846330267", "Rare Capacitor Shield Generator S")
        categorynode4123.Nodes.Add("item3052136397", "Rare Variable Shield Generator S")

        Dim categorynode4124 As TreeNode = categorynode412.Nodes.Add("node4124", "Shield Generators XS")
        categorynode4124.Nodes.Add("item2882830295", "Advanced Shield Generator XS")
        categorynode4124.Nodes.Add("item4078736566", "Exotic Active Shield Generator XS")
        categorynode4124.Nodes.Add("item311842630", "Exotic Capacitor Shield Generator XS")
        categorynode4124.Nodes.Add("item2343432065", "Exotic Variable Shield Generator XS")
        categorynode4124.Nodes.Add("item1735485600", "Rare Active Shield Generator XS")
        categorynode4124.Nodes.Add("item2533776367", "Rare Capacitor Shield Generator XS")
        categorynode4124.Nodes.Add("item3864567612", "Rare Variable Shield Generator XS")



        categorynode41.Nodes.Add("item63667997", "Transponder")

        Dim categorynode413 As TreeNode = categorynode41.Nodes.Add("node413", "Weapon Unit")
        Dim categorynode4131 As TreeNode = categorynode413.Nodes.Add("node4131", "Cannons")
        Dim categorynode41311 As TreeNode = categorynode4131.Nodes.Add("node41311", "Cannons L")
        categorynode41311.Nodes.Add("item3152865672", "Advanced Agile Cannon L")
        categorynode41311.Nodes.Add("item418164306", "Advanced Defense Cannon L")
        categorynode41311.Nodes.Add("item3960316615", "Advanced Heavy Cannon L")
        categorynode41311.Nodes.Add("item845167470", "Advanced Precision Cannon L")
        categorynode41311.Nodes.Add("item3289044684", "Cannon L")
        categorynode41311.Nodes.Add("item3152865678", "Exotic Agile Cannon L")
        categorynode41311.Nodes.Add("item418164308", "Exotic Defense Cannon L")
        categorynode41311.Nodes.Add("item3960316609", "Exotic Heavy Cannon L")
        categorynode41311.Nodes.Add("item845167468", "Exotic Precision Cannon L")
        categorynode41311.Nodes.Add("item3152865673", "Rare Agile Cannon L")
        categorynode41311.Nodes.Add("item418164307", "Rare Defense Cannon L")
        categorynode41311.Nodes.Add("item3960316608", "Rare Heavy Cannon L")
        categorynode41311.Nodes.Add("item845167469", "Rare Precision Cannon L")

        Dim categorynode41312 As TreeNode = categorynode4131.Nodes.Add("node41312", "Cannons M")
        categorynode41312.Nodes.Add("item2672575276", "Advanced Agile Cannon M")
        categorynode41312.Nodes.Add("item2383624964", "Advanced Defense Cannon M")
        categorynode41312.Nodes.Add("item2188788020", "Advanced Heavy Cannon M")
        categorynode41312.Nodes.Add("item2457342404", "Advanced Precision Cannon M")
        categorynode41312.Nodes.Add("item1699425404", "Cannon M")
        categorynode41312.Nodes.Add("item2672575278", "Exotic Agile Cannon M")
        categorynode41312.Nodes.Add("item2383624966", "Exotic Defense Cannon M")
        categorynode41312.Nodes.Add("item2188788022", "Exotic Heavy Cannon M")
        categorynode41312.Nodes.Add("item2457342402", "Exotic Precision Cannon M")
        categorynode41312.Nodes.Add("item2672575279", "Rare Agile Cannon M")
        categorynode41312.Nodes.Add("item2383624965", "Rare Defense Cannon M")
        categorynode41312.Nodes.Add("item2188788021", "Rare Heavy Cannon M")
        categorynode41312.Nodes.Add("item2457342403", "Rare Precision Cannon M")

        Dim categorynode41313 As TreeNode = categorynode4131.Nodes.Add("node41313", "Cannons S")
        categorynode41313.Nodes.Add("item429894438", "Advanced Agile Cannon S")
        categorynode41313.Nodes.Add("item1073121333", "Advanced Defense Cannon S")
        categorynode41313.Nodes.Add("item2058706007", "Advanced Heavy Cannon S")
        categorynode41313.Nodes.Add("item3567179843", "Advanced Precision Cannon S")
        categorynode41313.Nodes.Add("item1901919706", "Cannon S")
        categorynode41313.Nodes.Add("item429894436", "Exotic Agile Cannon S")
        categorynode41313.Nodes.Add("item1073121335", "Exotic Defense Cannon S")
        categorynode41313.Nodes.Add("item2058706005", "Exotic Heavy Cannon S")
        categorynode41313.Nodes.Add("item3567179845", "Exotic Precision Cannon S")
        categorynode41313.Nodes.Add("item429894437", "Rare Agile Cannon S")
        categorynode41313.Nodes.Add("item1073121334", "Rare Defense Cannon S")
        categorynode41313.Nodes.Add("item2058706004", "Rare Heavy Cannon S")
        categorynode41313.Nodes.Add("item3567179842", "Rare Precision Cannon S")

        Dim categorynode41314 As TreeNode = categorynode4131.Nodes.Add("node41314", "Cannons XS")
        categorynode41314.Nodes.Add("item684853120", "Advanced Agile Cannon XS")
        categorynode41314.Nodes.Add("item3467785559", "Advanced Defense Cannon XS")
        categorynode41314.Nodes.Add("item3384934781", "Advanced Heavy Cannon XS")
        categorynode41314.Nodes.Add("item3455226645", "Advanced Precision Cannon XS")
        categorynode41314.Nodes.Add("item3741742452", "Cannon XS")
        categorynode41314.Nodes.Add("item684853150", "Exotic Agile Cannon XS")
        categorynode41314.Nodes.Add("item3467785553", "Exotic Defense Cannon XS")
        categorynode41314.Nodes.Add("item3384934783", "Exotic Heavy Cannon XS")
        categorynode41314.Nodes.Add("item3455226647", "Exotic Precision Cannon XS")
        categorynode41314.Nodes.Add("item684853151", "Rare Agile Cannon XS")
        categorynode41314.Nodes.Add("item3467785552", "Rare Defense Cannon XS")
        categorynode41314.Nodes.Add("item3384934780", "Rare Heavy Cannon XS")
        categorynode41314.Nodes.Add("item3455226644", "Rare Precision Cannon XS")

        Dim categorynode4132 As TreeNode = categorynode413.Nodes.Add("node4132", "Lasers")
        Dim categorynode41321 As TreeNode = categorynode4132.Nodes.Add("node41321", "Lasers L")
        categorynode41321.Nodes.Add("item679378436", "Advanced Agile Laser L")
        categorynode41321.Nodes.Add("item3991674478", "Advanced Defense Laser L")
        categorynode41321.Nodes.Add("item4270062446", "Advanced Heavy Laser L")
        categorynode41321.Nodes.Add("item2356629408", "Advanced Precision Laser L")
        categorynode41321.Nodes.Add("item3516228574", "Laser L")
        categorynode41321.Nodes.Add("item679378438", "Exotic Agile Laser L")
        categorynode41321.Nodes.Add("item3991674464", "Exotic Defense Laser L")
        categorynode41321.Nodes.Add("item4270062440", "Exotic Heavy Laser L")
        categorynode41321.Nodes.Add("item2356629410", "Exotic Precision Laser L")
        categorynode41321.Nodes.Add("item679378437", "Rare Agile Laser L")
        categorynode41321.Nodes.Add("item3991674479", "Rare Defense Laser L")
        categorynode41321.Nodes.Add("item4270062441", "Rare Heavy Laser L")
        categorynode41321.Nodes.Add("item2356629409", "Rare Precision Laser L")

        Dim categorynode41322 As TreeNode = categorynode4132.Nodes.Add("node41322", "Lasers M")
        categorynode41322.Nodes.Add("item360504284", "Advanced Agile Laser M")
        categorynode41322.Nodes.Add("item3805044393", "Advanced Defense Laser M")
        categorynode41322.Nodes.Add("item3588765876", "Advanced Heavy Laser M")
        categorynode41322.Nodes.Add("item3840109424", "Advanced Precision Laser M")
        categorynode41322.Nodes.Add("item1117413121", "Laser M")
        categorynode41322.Nodes.Add("item360504286", "Exotic Agile Laser M")
        categorynode41322.Nodes.Add("item3805044395", "Exotic Defense Laser M")
        categorynode41322.Nodes.Add("item3588766026", "Exotic Heavy Laser M")
        categorynode41322.Nodes.Add("item3840109426", "Exotic Precision Laser M")
        categorynode41322.Nodes.Add("item360504287", "Rare Agile Laser M")
        categorynode41322.Nodes.Add("item3805044394", "Rare Defense Laser M")
        categorynode41322.Nodes.Add("item3588765877", "Rare Heavy Laser M")
        categorynode41322.Nodes.Add("item3840109425", "Rare Precision Laser M")

        Dim categorynode41323 As TreeNode = categorynode4132.Nodes.Add("node41323", "Lasers S")
        categorynode41323.Nodes.Add("item4124398193", "Advanced Agile Laser S")
        categorynode41323.Nodes.Add("item1737118473", "Advanced Defense Laser S")
        categorynode41323.Nodes.Add("item338218847", "Advanced Heavy Laser S")
        categorynode41323.Nodes.Add("item3730148334", "Advanced Precision Laser S")
        categorynode41323.Nodes.Add("item32593579", "Laser S")
        categorynode41323.Nodes.Add("item4124398199", "Exotic Agile Laser S")
        categorynode41323.Nodes.Add("item1737118475", "Exotic Defense Laser S")
        categorynode41323.Nodes.Add("item338218841", "Exotic Heavy Laser S")
        categorynode41323.Nodes.Add("item3730148320", "Exotic Precision Laser S")
        categorynode41323.Nodes.Add("item4124398192", "Rare Agile Laser S")
        categorynode41323.Nodes.Add("item1737118474", "Rare Defense Laser S")
        categorynode41323.Nodes.Add("item3730148335", "Rare Precision Laser S")
        categorynode41323.Nodes.Add("item338218840", "Rare Heavy Laser S")

        Dim categorynode41324 As TreeNode = categorynode4132.Nodes.Add("node41324", "Lasers XS")
        categorynode41324.Nodes.Add("item3972697532", "Advanced Agile Laser XS")
        categorynode41324.Nodes.Add("item3698237865", "Advanced Heavy Laser XS")
        categorynode41324.Nodes.Add("item796456749", "Advanced Defense Laser XS")
        categorynode41324.Nodes.Add("item1604660449", "Advanced Precision Laser XS")
        categorynode41324.Nodes.Add("item11309408", "laser XS")
        categorynode41324.Nodes.Add("item3972697534", "Exotic Agile Laser XS")
        categorynode41324.Nodes.Add("item796456747", "Exotic Defense Laser XS")
        categorynode41324.Nodes.Add("item3698237863", "Exotic Heavy Laser XS")
        categorynode41324.Nodes.Add("item1604660455", "Exotic Precision Laser XS")
        categorynode41324.Nodes.Add("item3972697533", "Rare Agile Laser XS")
        categorynode41324.Nodes.Add("item796456746", "Rare Defense Laser XS")
        categorynode41324.Nodes.Add("item3698237862", "Rare Heavy Laser XS")
        categorynode41324.Nodes.Add("item1604660448", "Rare Precision Laser XS")

        Dim categorynode4133 As TreeNode = categorynode413.Nodes.Add("node4133", "Missile Pods")
        Dim categorynode41331 As TreeNode = categorynode4133.Nodes.Add("node41331", "Missiles L")
        categorynode41331.Nodes.Add("item3650288374", "Advanced Agile Missile L")
        categorynode41331.Nodes.Add("item3453451048", "Advanced Defense Missile L")
        categorynode41331.Nodes.Add("item708864069", "Advanced Heavy Missile L")
        categorynode41331.Nodes.Add("item1205879485", "Advanced Precision Missile L")
        categorynode41331.Nodes.Add("item3873532190", "Missile L")
        categorynode41331.Nodes.Add("item3650288368", "Exotic Agile Missile L")
        categorynode41331.Nodes.Add("item3453451050", "Exotic Defense Missile L")
        categorynode41331.Nodes.Add("item708864067", "Exotic Heavy Missile L")
        categorynode41331.Nodes.Add("item1205879483", "Exotic Precision Missile L")
        categorynode41331.Nodes.Add("item3650288369", "Rare Agile Missile L")
        categorynode41331.Nodes.Add("item3453451051", "Rare Defense Missile L")
        categorynode41331.Nodes.Add("item708864066", "Rare Heavy Missile L")
        categorynode41331.Nodes.Add("item1205879482", "Rare Precision Missile L")

        Dim categorynode41332 As TreeNode = categorynode4133.Nodes.Add("node41332", "Missiles M")
        categorynode41332.Nodes.Add("item598736203", "Advanced Agile Missile M")
        categorynode41332.Nodes.Add("item1068910670", "Advanced Defense Missile M")
        categorynode41332.Nodes.Add("item1102564708", "Advanced Heavy Missile M")
        categorynode41332.Nodes.Add("item1217643701", "Advanced Precision Missile M")
        categorynode41332.Nodes.Add("item1557865377", "Missile M")
        categorynode41332.Nodes.Add("item598736197", "Exotic Agile Missile M")
        categorynode41332.Nodes.Add("item1068910656", "Exotic Defense Missile M")
        categorynode41332.Nodes.Add("item1102564706", "Exotic Heavy Missile M")
        categorynode41332.Nodes.Add("item1217644363", "Exotic Precision Missile M")
        categorynode41332.Nodes.Add("item598736196", "Rare Agile Missile M")
        categorynode41332.Nodes.Add("item1068910671", "Rare Defense Missile M")
        categorynode41332.Nodes.Add("item1102564707", "Rare Heavy Missile M")
        categorynode41332.Nodes.Add("item1217643700", "Rare Precision Missile M")

        Dim categorynode41333 As TreeNode = categorynode4133.Nodes.Add("node41333", "Missiles S")
        categorynode41333.Nodes.Add("item1843877007", "Advanced Agile Missile S")
        categorynode41333.Nodes.Add("item136359050", "Advanced Defense Missile S")
        categorynode41333.Nodes.Add("item1100091709", "Advanced Heavy Missile S")
        categorynode41333.Nodes.Add("item2668363439", "Advanced Precision Missile S")
        categorynode41333.Nodes.Add("item1109891544", "Missile S")
        categorynode41333.Nodes.Add("item1843877005", "Exotic Agile Missile S")
        categorynode41333.Nodes.Add("item136359048", "Exotic Defense Missile S")
        categorynode41333.Nodes.Add("item1100091711", "Exotic Heavy Missile S")
        categorynode41333.Nodes.Add("item2668363433", "Exotic Precision Missile S")
        categorynode41333.Nodes.Add("item1843877006", "Rare Agile Missile S")
        categorynode41333.Nodes.Add("item136359051", "Rare Defense Missile S")
        categorynode41333.Nodes.Add("item1100091708", "Rare Heavy Missile S")
        categorynode41333.Nodes.Add("item2668363432", "Rare Precision Missile S")

        Dim categorynode41334 As TreeNode = categorynode4133.Nodes.Add("node41334", "Missiles XS")
        categorynode41334.Nodes.Add("item1780076560", "Advanced Agile Missile XS")
        categorynode41334.Nodes.Add("item134390791", "Advanced Defense Missile XS")
        categorynode41334.Nodes.Add("item3611570511", "Advanced Heavy Missile XS")
        categorynode41334.Nodes.Add("item2239993844", "Advanced Precision Missile XS")
        categorynode41334.Nodes.Add("item1260582276", "missile XS")
        categorynode41334.Nodes.Add("item1780076562", "Exotic Agile Missile XS")
        categorynode41334.Nodes.Add("item134390789", "Exotic Defense Missile XS")
        categorynode41334.Nodes.Add("item3611570509", "Exotic Heavy Missile XS")
        categorynode41334.Nodes.Add("item2239993846", "Exotic Precision Missile XS")
        categorynode41334.Nodes.Add("item1780076561", "Rare Agile Missile XS")
        categorynode41334.Nodes.Add("item134390788", "Rare Defense Missile XS")
        categorynode41334.Nodes.Add("item3611570508", "Rare Heavy Missile XS")
        categorynode41334.Nodes.Add("item2239993845", "Rare Precision Missile XS")

        Dim categorynode4134 As TreeNode = categorynode413.Nodes.Add("node4134", "Railguns")
        Dim categorynode41341 As TreeNode = categorynode4134.Nodes.Add("node41341", "Railguns L")
        categorynode41341.Nodes.Add("item4062760160", "Advanced Agile Railgun L")
        categorynode41341.Nodes.Add("item3670363955", "Advanced Defense Railgun L")
        categorynode41341.Nodes.Add("item30018129", "Advanced Heavy Railgun L")
        categorynode41341.Nodes.Add("item2916726762", "Advanced Precision Railgun L")
        categorynode41341.Nodes.Add("item430145504", "Railgun L")
        categorynode41341.Nodes.Add("item4062760162", "Exotic Agile Railgun L")
        categorynode41341.Nodes.Add("item3670363953", "Exotic Defense Railgun L")
        categorynode41341.Nodes.Add("item30018135", "Exotic Heavy Railgun L")
        categorynode41341.Nodes.Add("item2916726760", "Exotic Precision Railgun L")
        categorynode41341.Nodes.Add("item4062760163", "Rare Agile Railgun L")
        categorynode41341.Nodes.Add("item3670363952", "Rare Defense Railgun L")
        categorynode41341.Nodes.Add("item30018128", "Rare Heavy Railgun L")
        categorynode41341.Nodes.Add("item2916726763", "Rare Precision Railgun L")

        Dim categorynode41342 As TreeNode = categorynode4134.Nodes.Add("node41342", "Railguns M")
        categorynode41342.Nodes.Add("item3057550275", "Advanced Agile Railgun M")
        categorynode41342.Nodes.Add("item3396072211", "Advanced Defense Railgun M")
        categorynode41342.Nodes.Add("item1641776330", "Advanced Heavy Railgun M")
        categorynode41342.Nodes.Add("item111253038", "Advanced Precision Railgun M")
        categorynode41342.Nodes.Add("item2733257194", "Railgun M")
        categorynode41342.Nodes.Add("item3057550301", "Exotic Agile Railgun M")
        categorynode41342.Nodes.Add("item3396072237", "Exotic Defense Railgun M")
        categorynode41342.Nodes.Add("item1641776328", "Exotic Heavy Railgun M")
        categorynode41342.Nodes.Add("item111253024", "Exotic Precision Railgun M")
        categorynode41342.Nodes.Add("item3057550300", "Rare Agile Railgun M")
        categorynode41342.Nodes.Add("item3396072236", "Rare Defense Railgun M")
        categorynode41342.Nodes.Add("item1641776331", "Rare Heavy Railgun M")
        categorynode41342.Nodes.Add("item111253039", "Rare Precision Railgun M")

        Dim categorynode41343 As TreeNode = categorynode4134.Nodes.Add("node41343", "Railguns S")
        categorynode41343.Nodes.Add("item1767704175", "Advanced Agile Railgun S")
        categorynode41343.Nodes.Add("item223437801", "Advanced Defense Railgun S")
        categorynode41343.Nodes.Add("item2991505111", "Advanced Heavy Railgun S")
        categorynode41343.Nodes.Add("item831043069", "Advanced Precision Railgun S")
        categorynode41343.Nodes.Add("item853107412", "Railgun S")
        categorynode41343.Nodes.Add("item1767704161", "Exotic Agile Railgun S")
        categorynode41343.Nodes.Add("item223437807", "Exotic Defense Railgun S")
        categorynode41343.Nodes.Add("item2991505105", "Exotic Heavy Railgun S")
        categorynode41343.Nodes.Add("item831043071", "Exotic Precision Railgun S")
        categorynode41343.Nodes.Add("item1767704174", "Rare Agile Railgun S")
        categorynode41343.Nodes.Add("item223437800", "Rare Defense Railgun S")
        categorynode41343.Nodes.Add("item2991505104", "Rare Heavy Railgun S")
        categorynode41343.Nodes.Add("item831043070", "Rare Precision Railgun S")

        Dim categorynode41344 As TreeNode = categorynode4134.Nodes.Add("node41344", "Railguns XS")
        categorynode41344.Nodes.Add("item549955075", "Advanced Agile Railgun XS")
        categorynode41344.Nodes.Add("item2108818543", "Advanced Defense Railgun XS")
        categorynode41344.Nodes.Add("item1816732415", "Advanced Heavy Railgun XS")
        categorynode41344.Nodes.Add("item690643397", "Advanced Precision Railgun XS")
        categorynode41344.Nodes.Add("item31327772", "railgun XS")
        categorynode41344.Nodes.Add("item549955101", "Exotic Agile Railgun XS")
        categorynode41344.Nodes.Add("item2108818541", "Exotic Defense Railgun XS")
        categorynode41344.Nodes.Add("item1816732409", "Exotic Heavy Railgun XS")
        categorynode41344.Nodes.Add("item690643419", "Exotic Precision Railgun XS")
        categorynode41344.Nodes.Add("item549955100", "Rare Agile Railgun XS")
        categorynode41344.Nodes.Add("item2108818540", "Rare Defense Railgun XS")
        categorynode41344.Nodes.Add("item1816732408", "Rare Heavy Railgun XS")
        categorynode41344.Nodes.Add("item690643396", "Rare Precision Railgun XS")

        Dim categorynode4135 As TreeNode = categorynode413.Nodes.Add("node4135", "Stasis Weapon Unit")
        categorynode4135.Nodes.Add("item3748127459", "Stasis Weapon XS")
        categorynode4135.Nodes.Add("item3512491987", "Stasis Weapon S")
        categorynode4135.Nodes.Add("item298719906", "Stasis Weapon M")
        categorynode4135.Nodes.Add("item2644314167", "Stasis Weapon L")

        Dim categorynode42 As TreeNode = categorynode4.Nodes.Add("node42", "Furniture & Appliances")
        Dim categorynode421 As TreeNode = categorynode42.Nodes.Add("node421", "Chairs")
        categorynode421.Nodes.Add("item4216497731", "Bed")
        categorynode421.Nodes.Add("item542122758", "Bench")
        categorynode421.Nodes.Add("item3555832402", "Chair Surface")
        categorynode421.Nodes.Add("item2169816178", "Encampment Chair")
        categorynode421.Nodes.Add("item392866463", "Golden Throne")
        categorynode421.Nodes.Add("item2542033786", "Metal Throne")
        categorynode421.Nodes.Add("item1261703398", "Navigator Chair")
        categorynode421.Nodes.Add("item536277576", "Obsidian Throne")
        categorynode421.Nodes.Add("item554266799", "Office Chair")
        categorynode421.Nodes.Add("item2846288811", "Shower Unit")
        categorynode421.Nodes.Add("item1235633417", "Sofa")
        categorynode421.Nodes.Add("item4186859262", "Toilet unit A")
        categorynode421.Nodes.Add("item3929116491", "Toilet unit B")
        categorynode421.Nodes.Add("item3517217013", "Urinal unit")
        categorynode421.Nodes.Add("item2453312794", "Wooden Chair")
        categorynode421.Nodes.Add("item2018455538", "Wooden Sofa")
        categorynode421.Nodes.Add("item3736537839", "Wooden armchair")

        Dim categorynode422 As TreeNode = categorynode42.Nodes.Add("node422", "Decorative Elements")
        Dim categorynode4221 As TreeNode = categorynode422.Nodes.Add("node4221", "Adjuncts")
        categorynode4221.Nodes.Add("item1894947006", "Vertical wing")
        categorynode4221.Nodes.Add("item2429336341", "Wingtip S")
        categorynode4221.Nodes.Add("item3695530525", "Wingtip M")
        categorynode4221.Nodes.Add("item3292462663", "Wingtip L")

        Dim categorynode4222 As TreeNode = categorynode422.Nodes.Add("node4222", "Antennas")
        categorynode4222.Nodes.Add("item1951235468", "Antenna S")
        categorynode4222.Nodes.Add("item206489025", "Antenna M")
        categorynode4222.Nodes.Add("item413322747", "Antenna L")

        Dim categorynode4223 As TreeNode = categorynode422.Nodes.Add("node4223", "Barriers")
        categorynode4223.Nodes.Add("item3261824822", "Barrier S")
        categorynode4223.Nodes.Add("item3261824887", "Barrier M")
        categorynode4223.Nodes.Add("item1377211067", "Barrier corner")

        Dim categorynode4224 As TreeNode = categorynode422.Nodes.Add("node4224", "Bathroom Elements")
        categorynode4224.Nodes.Add("item400937499", "Sink unit")

        Dim categorynode4225 As TreeNode = categorynode422.Nodes.Add("node4225", "Board")
        categorynode4225.Nodes.Add("item542805258", "Keyboard unit")

        Dim categorynode4226 As TreeNode = categorynode422.Nodes.Add("node4226", "Decorative Cables")
        categorynode4226.Nodes.Add("item1542390551", "Cable Model A S")
        categorynode4226.Nodes.Add("item1542146744", "Cable Model A M")
        categorynode4226.Nodes.Add("item1542390550", "Cable Model B S")
        categorynode4226.Nodes.Add("item1542146745", "Cable Model B M")
        categorynode4226.Nodes.Add("item1542390549", "Cable Model C S")
        categorynode4226.Nodes.Add("item1542146746", "Cable Model C M")
        categorynode4226.Nodes.Add("item1700326384", "Corner Cable Model A")
        categorynode4226.Nodes.Add("item1700326385", "Corner Cable Model B")
        categorynode4226.Nodes.Add("item1700326390", "Corner Cable Model C")

        Dim categorynode4227 As TreeNode = categorynode422.Nodes.Add("node4227", "Furniture")
        categorynode4227.Nodes.Add("item3193900802", "Eye Dolls Workshop - Artist Unknown")
        categorynode4227.Nodes.Add("item3193900800", "HMS Ajax33 - Artist Unknown")
        categorynode4227.Nodes.Add("item3193900801", "Parrotos Sanctuary - Artist Unknown")
        categorynode4227.Nodes.Add("item283549593", "Dresser")
        categorynode4227.Nodes.Add("item2358357442", "Model of the Helios System")
        categorynode4227.Nodes.Add("item2216112746", "Nightstand")
        categorynode4227.Nodes.Add("item82404567", "Obsidian Table M")
        categorynode4227.Nodes.Add("item3813093434", "Round carpet")
        categorynode4227.Nodes.Add("item4083139459", "Shelf empty")
        categorynode4227.Nodes.Add("item4083139485", "Shelf full")
        categorynode4227.Nodes.Add("item4083139484", "Shelf half full")
        categorynode4227.Nodes.Add("item3813093435", "Square carpet")
        categorynode4227.Nodes.Add("item1395483977", "Table")
        categorynode4227.Nodes.Add("item1407324391", "Trash can")
        categorynode4227.Nodes.Add("item2428627426", "Wardrobe")
        categorynode4227.Nodes.Add("item3824401006", "Wooden dresser")
        categorynode4227.Nodes.Add("item1082668972", "Wooden low table")
        categorynode4227.Nodes.Add("item3845900543", "Wooden table M")
        categorynode4227.Nodes.Add("item3893102542", "Wooden table L")
        categorynode4227.Nodes.Add("item1268259677", "Wooden wardrobe")

        Dim categorynode4228 As TreeNode = categorynode422.Nodes.Add("node4228", "Holograms")
        categorynode4228.Nodes.Add("item133985418", "Helios System Hologram L")
        categorynode4228.Nodes.Add("item1147784546", "Novean Arkship Hologram L")
        categorynode4228.Nodes.Add("item1541106442", "Planet Hologram L")
        categorynode4228.Nodes.Add("item4090740447", "Planet Hologram")
        categorynode4228.Nodes.Add("item124823209", "Spaceship Hologram S")
        categorynode4228.Nodes.Add("item85154060", "Spaceship Hologram M")
        categorynode4228.Nodes.Add("item2137895179", "Spaceship Hologram L")

        Dim categorynode4229 As TreeNode = categorynode422.Nodes.Add("node4229", "Hull decoration")
        categorynode4229.Nodes.Add("item767916091", "Canopy Metal corner S")
        categorynode4229.Nodes.Add("item283728321", "Canopy Metal corner M")
        categorynode4229.Nodes.Add("item2118024887", "Canopy Metal corner L")
        categorynode4229.Nodes.Add("item2266058296", "Canopy Metal flat S")
        categorynode4229.Nodes.Add("item3729727572", "Canopy Metal flat M")
        categorynode4229.Nodes.Add("item2635025376", "Canopy Metal flat L")
        categorynode4229.Nodes.Add("item3294726704", "Canopy Metal tilted S")
        categorynode4229.Nodes.Add("item4015784029", "Canopy Metal tilted M")
        categorynode4229.Nodes.Add("item798367766", "Canopy Metal tilted L")
        categorynode4229.Nodes.Add("item1339058404", "Canopy Metal triangle S")
        categorynode4229.Nodes.Add("item265675573", "Canopy Metal triangle M")
        categorynode4229.Nodes.Add("item3943842048", "Canopy Metal triangle L")
        categorynode4229.Nodes.Add("item3337817677", "Hull decorative Element A")
        categorynode4229.Nodes.Add("item3337817674", "Hull decorative Element B")
        categorynode4229.Nodes.Add("item3337817675", "Hull decorative Element C")
        categorynode4229.Nodes.Add("item1220701936", "Steel column")
        categorynode4229.Nodes.Add("item4145570204", "Steel panel")

        Dim categorynode42210 As TreeNode = categorynode422.Nodes.Add("node42210", "Pipes")
        categorynode42210.Nodes.Add("item2824951359", "Pipe A M")
        categorynode42210.Nodes.Add("item2709793409", "Pipe B M")
        categorynode42210.Nodes.Add("item2937058341", "Pipe C M")
        categorynode42210.Nodes.Add("item2917319456", "Pipe Connector M")
        categorynode42210.Nodes.Add("item543225023", "Pipe D M")
        categorynode42210.Nodes.Add("item2123842216", "Pipe corner M")

        Dim categorynode42211 As TreeNode = categorynode422.Nodes.Add("node42211", "Plants")
        categorynode42211.Nodes.Add("item630574506", "Bagged Plant A")
        categorynode42211.Nodes.Add("item630574505", "Bagged Plant B")
        categorynode42211.Nodes.Add("item2648123924", "Bonsai")
        categorynode42211.Nodes.Add("item3106061128", "Eggplant Plant Case")
        categorynode42211.Nodes.Add("item3106061140", "Ficus Plant A")
        categorynode42211.Nodes.Add("item3106061141", "Ficus Plant B")
        categorynode42211.Nodes.Add("item3106061142", "Foliage Plant Case A")
        categorynode42211.Nodes.Add("item3106061143", "Foliage Plant Case B")
        categorynode42211.Nodes.Add("item1797415729", "Plant")
        categorynode42211.Nodes.Add("item3106061133", "Plant Case S")
        categorynode42211.Nodes.Add("item3106061130", "Plant Case M")
        categorynode42211.Nodes.Add("item195870295", "Plant Case A")
        categorynode42211.Nodes.Add("item195870294", "Plant Case B")
        categorynode42211.Nodes.Add("item195870297", "Plant Case C")
        categorynode42211.Nodes.Add("item195870296", "Plant Case D")
        categorynode42211.Nodes.Add("item195870299", "Plant Case E")
        categorynode42211.Nodes.Add("item3106061129", "Salad Plant Case")
        categorynode42211.Nodes.Add("item3106061131", "Squash Plant Case")
        categorynode42211.Nodes.Add("item630574502", "Suspended Fruit Plant")
        categorynode42211.Nodes.Add("item630574504", "Suspended Plant A")
        categorynode42211.Nodes.Add("item630574503", "Suspended Plant B")

        Dim categorynode42212 As TreeNode = categorynode422.Nodes.Add("node42212", "Windows")
        categorynode42212.Nodes.Add("item515378511", "Armored window XS")
        categorynode42212.Nodes.Add("item3014939922", "Armored window S")
        categorynode42212.Nodes.Add("item2158665549", "Armored window M")
        categorynode42212.Nodes.Add("item1804139232", "Armored window L")
        categorynode42212.Nodes.Add("item1952409967", "Bay window XL")
        categorynode42212.Nodes.Add("item695039310", "Canopy Windshield corner S")
        categorynode42212.Nodes.Add("item1484667376", "Canopy Windshield corner M")
        categorynode42212.Nodes.Add("item4226053198", "Canopy Windshield corner L")
        categorynode42212.Nodes.Add("item2433054263", "Canopy Windshield flat S")
        categorynode42212.Nodes.Add("item1900076171", "Canopy Windshield flat M")
        categorynode42212.Nodes.Add("item1001848134", "Canopy Windshield flat L")
        categorynode42212.Nodes.Add("item1326565833", "Canopy Windshield tilted S")
        categorynode42212.Nodes.Add("item4167375414", "Canopy Windshield tilted M")
        categorynode42212.Nodes.Add("item2086563919", "Canopy Windshield tilted L")
        categorynode42212.Nodes.Add("item2792485016", "Canopy Windshield triangle S")
        categorynode42212.Nodes.Add("item3521312761", "Canopy Windshield triangle M")
        categorynode42212.Nodes.Add("item2236273961", "Canopy Windshield triangle L")
        categorynode42212.Nodes.Add("item561162197", "Glass Panel S")
        categorynode42212.Nodes.Add("item2266946860", "Glass Panel M")
        categorynode42212.Nodes.Add("item1165506034", "Glass Panel L")
        categorynode42212.Nodes.Add("item3268459843", "Window XS")
        categorynode42212.Nodes.Add("item242448402", "Window S")
        categorynode42212.Nodes.Add("item3924941627", "Window M")
        categorynode42212.Nodes.Add("item894516284", "Window L")

        Dim categorynode423 As TreeNode = categorynode42.Nodes.Add("node423", "Displays")
        Dim categorynode4231 As TreeNode = categorynode423.Nodes.Add("node4231", "Info Buttons")
        categorynode4231.Nodes.Add("item3996923355", "Info Button S")

        Dim categorynode4232 As TreeNode = categorynode423.Nodes.Add("node4232", "Screens")
        categorynode4232.Nodes.Add("item184261427", "Screen XS")
        categorynode4232.Nodes.Add("item184261490", "Screen S")
        categorynode4232.Nodes.Add("item184261558", "Screen M")
        categorynode4232.Nodes.Add("item879675317", "Screen XL")
        Dim categorynode42321 As TreeNode = categorynode4232.Nodes.Add("node42321", "Signs")
        categorynode42321.Nodes.Add("item166656023", "Sign XS")
        categorynode42321.Nodes.Add("item362159734", "Sign S")
        categorynode42321.Nodes.Add("item3068429457", "Sign M")
        categorynode42321.Nodes.Add("item166549741", "Sign L")
        categorynode42321.Nodes.Add("item3919696834", "Vertical Sign XS")
        categorynode42321.Nodes.Add("item2610895147", "Vertical Sign M")
        categorynode42321.Nodes.Add("item1533790308", "Vertical Sign L")
        categorynode4232.Nodes.Add("item3988665660", "Transparent Screen XS")
        categorynode4232.Nodes.Add("item3988663014", "Transparent Screen S")
        categorynode4232.Nodes.Add("item3988662951", "Transparent Screen M")
        categorynode4232.Nodes.Add("item3988662884", "Transparent Screen L")

        Dim categorynode424 As TreeNode = categorynode42.Nodes.Add("node424", "Doors")
        categorynode424.Nodes.Add("item4249659729", "Airlock")
        categorynode424.Nodes.Add("item581667413", "Expanded gate S")
        categorynode424.Nodes.Add("item1289884535", "Expanded gate L")
        categorynode424.Nodes.Add("item764397251", "Fuel Intake XS")
        categorynode424.Nodes.Add("item1097676949", "Gate XS")
        categorynode424.Nodes.Add("item2858887382", "Gate M")
        categorynode424.Nodes.Add("item1256519882", "Gate XL")
        categorynode424.Nodes.Add("item297147615", "Hatch S")
        categorynode424.Nodes.Add("item3709017308", "Interior door")
        categorynode424.Nodes.Add("item1139773633", "Reinforced Sliding Door")
        categorynode424.Nodes.Add("item201196316", "Sliding Door S")
        categorynode424.Nodes.Add("item741980535", "Sliding Door M")

        Dim categorynode425 As TreeNode = categorynode42.Nodes.Add("node425", "Electronic Elements")
        Dim categorynode4251 As TreeNode = categorynode425.Nodes.Add("node4251", "Counters")
        categorynode4251.Nodes.Add("item888063487", "10 counter")
        categorynode4251.Nodes.Add("item888062905", "2 counter")
        categorynode4251.Nodes.Add("item888062906", "3 counter")
        categorynode4251.Nodes.Add("item888062908", "5 counter")
        categorynode4251.Nodes.Add("item888062910", "7 counter")

        Dim categorynode4252 As TreeNode = categorynode425.Nodes.Add("node4252", "Data Emitters")
        categorynode4252.Nodes.Add("item1279651501", "Emitter XS")
        categorynode4252.Nodes.Add("item3287187256", "Emitter S")
        categorynode4252.Nodes.Add("item2809213930", "Emitter M")

        Dim categorynode4253 As TreeNode = categorynode425.Nodes.Add("node4253", "Databanks")
        categorynode4253.Nodes.Add("item812400865", "Databank")

        Dim categorynode4254 As TreeNode = categorynode425.Nodes.Add("node4254", "Delay Lines")
        categorynode4254.Nodes.Add("item1474604499", "Delay Line")

        Dim categorynode4255 As TreeNode = categorynode425.Nodes.Add("node4255", "Laser Emitters")
        categorynode4255.Nodes.Add("item609676854", "Infrared Laser Emitter")
        categorynode4255.Nodes.Add("item1784722190", "Laser Emitter")

        Dim categorynode4256 As TreeNode = categorynode425.Nodes.Add("node4256", "Logic Operators")
        categorynode4256.Nodes.Add("item2569152632", "And operator")
        categorynode4256.Nodes.Add("item3600874516", "NAND Operator")
        categorynode4256.Nodes.Add("item1839029088", "NOR Operator")
        categorynode4256.Nodes.Add("item2629309308", "Not operator")
        categorynode4256.Nodes.Add("item1707712023", "Or operator")
        categorynode4256.Nodes.Add("item3437395596", "Xor Operator")

        Dim categorynode4257 As TreeNode = categorynode425.Nodes.Add("node4257", "Receivers")
        categorynode4257.Nodes.Add("item3732634076", "Receiver XS")
        categorynode4257.Nodes.Add("item2082095499", "Receiver S")
        categorynode4257.Nodes.Add("item736740615", "Receiver M")

        Dim categorynode4258 As TreeNode = categorynode425.Nodes.Add("node4258", "Relays")
        categorynode4258.Nodes.Add("item1694177571", "Relay")

        Dim categorynode4259 As TreeNode = categorynode425.Nodes.Add("node4259", "Sensors")
        Dim categorynode42591 As TreeNode = categorynode4259.Nodes.Add("node42591", "Laser Detectors")
        categorynode42591.Nodes.Add("item2153998731", "Infrared Laser Receiver")
        categorynode42591.Nodes.Add("item783555860", "Laser Receiver")

        Dim categorynode42592 As TreeNode = categorynode4259.Nodes.Add("node42592", "Telemeters")
        categorynode42592.Nodes.Add("item1722901246", "Telemeter")

        Dim categorynode42593 As TreeNode = categorynode4259.Nodes.Add("node42593", "Zone detectors")
        categorynode42593.Nodes.Add("item485151209", "Detection Zone XS")
        categorynode42593.Nodes.Add("item485149228", "Detection Zone S")
        categorynode42593.Nodes.Add("item485149481", "Detection Zone M")
        categorynode42593.Nodes.Add("item4241228057", "Detection Zone L")

        Dim categorynode42510 As TreeNode = categorynode425.Nodes.Add("node42510", "Triggers")
        Dim categorynode425101 As TreeNode = categorynode42510.Nodes.Add("node425101", "Manual Buttons")
        categorynode425101.Nodes.Add("item1550904282", "Manual Button XS")
        categorynode425101.Nodes.Add("item2896791363", "Manual Button S")

        Dim categorynode425102 As TreeNode = categorynode42510.Nodes.Add("node425102", "Manual Switches")
        categorynode425102.Nodes.Add("item4181147843", "Manual Switch")

        Dim categorynode425103 As TreeNode = categorynode42510.Nodes.Add("node425103", "Pressure Tiles")
        categorynode425103.Nodes.Add("item2012928469", "Pressure tile")

        Dim categorynode426 As TreeNode = categorynode42.Nodes.Add("node426", "Elevators")
        categorynode426.Nodes.Add("item3663249627", "Elevator XS")

        Dim categorynode427 As TreeNode = categorynode42.Nodes.Add("node427", "Fireworks Launcher")
        categorynode427.Nodes.Add("item3882559017", "Fireworks Launcher")

        Dim categorynode428 As TreeNode = categorynode42.Nodes.Add("node428", "High-Tech Furniture")
        Dim categorynode4281 As TreeNode = categorynode428.Nodes.Add("node4281", "Force Fields")
        categorynode4281.Nodes.Add("item3686074288", "Force Field XS")
        categorynode4281.Nodes.Add("item3685998465", "Force Field S")
        categorynode4281.Nodes.Add("item3686006062", "Force Field M")
        categorynode4281.Nodes.Add("item3685982092", "Force Field L")

        Dim categorynode4282 As TreeNode = categorynode428.Nodes.Add("node4282", "Virtual projectors")
        categorynode4282.Nodes.Add("item3929462194", "Virtual scaffolding projector")

        Dim categorynode429 As TreeNode = categorynode42.Nodes.Add("node429", "Lights")
        categorynode429.Nodes.Add("item787207321", "Headlight")
        categorynode429.Nodes.Add("item25682791", "Long Light XS")
        categorynode429.Nodes.Add("item3180371725", "Long Light S")
        categorynode429.Nodes.Add("item677591159", "Long Light M")
        categorynode429.Nodes.Add("item3524314552", "Long Light L")
        categorynode429.Nodes.Add("item177821174", "Square Light XS")
        categorynode429.Nodes.Add("item3981684520", "Square Light S")
        categorynode429.Nodes.Add("item632353355", "Square Light M")
        categorynode429.Nodes.Add("item823697268", "Square Light L")
        categorynode429.Nodes.Add("item3923388834", "Vertical Light XS")
        categorynode429.Nodes.Add("item3231255047", "Vertical Light S")
        categorynode429.Nodes.Add("item1603266808", "Vertical Light M")
        categorynode429.Nodes.Add("item2027152926", "Vertical Light L")

        Dim categorynode43 As TreeNode = categorynode4.Nodes.Add("node43", "Industry & Infrastructure Elements")
        Dim categorynode431 As TreeNode = categorynode43.Nodes.Add("node431", "Containers")
        Dim categorynode4311 As TreeNode = categorynode431.Nodes.Add("node4311", "Ammo Containers")
        categorynode4311.Nodes.Add("item50309297", "Ammo Container XL L")
        categorynode4311.Nodes.Add("item300986010", "Ammo Container S XS")
        categorynode4311.Nodes.Add("item2300179701", "Ammo Container M S")
        categorynode4311.Nodes.Add("item923167511", "Ammo Container L M")

        Dim categorynode4312 As TreeNode = categorynode431.Nodes.Add("node4312", "Dispensers")
        categorynode4312.Nodes.Add("item16651125", "Deprecated Dispenser")
        categorynode4312.Nodes.Add("item333062081", "Dispenser")

        Dim categorynode4313 As TreeNode = categorynode431.Nodes.Add("node4313", "Fuel Tanks")
        Dim categorynode43131 As TreeNode = categorynode4313.Nodes.Add("node43131", "Atmospheric Fuel Containers")
        categorynode43131.Nodes.Add("item3273319200", "Atmospheric Fuel Tank XS")
        categorynode43131.Nodes.Add("item2183619036", "Atmospheric Fuel Tank S")
        categorynode43131.Nodes.Add("item3464628964", "Atmospheric Fuel Tank M")
        categorynode43131.Nodes.Add("item3039582547", "Atmospheric Fuel Tank L")

        Dim categorynode43132 As TreeNode = categorynode4313.Nodes.Add("node43132", "Rocket Fuel Containers")
        categorynode43132.Nodes.Add("item1663412227", "Rocket Fuel Tank XS")
        categorynode43132.Nodes.Add("item3126840739", "Rocket Fuel Tank S")
        categorynode43132.Nodes.Add("item2477859329", "Rocket Fuel Tank M")
        categorynode43132.Nodes.Add("item4180073139", "Rocket Fuel Tank L")

        Dim categorynode43133 As TreeNode = categorynode4313.Nodes.Add("node43133", "Space Fuel Containers")
        categorynode43133.Nodes.Add("item2421673145", "Space Fuel Tank XS")
        categorynode43133.Nodes.Add("item1790622152", "Space Fuel Tank S")
        categorynode43133.Nodes.Add("item773467906", "Space Fuel Tank M")
        categorynode43133.Nodes.Add("item2212207656", "Space Fuel Tank L")

        Dim categorynode4314 As TreeNode = categorynode431.Nodes.Add("node4314", "Item containers")
        categorynode4314.Nodes.Add("item373359444", "Container Hub")

        Dim categorynode43141 As TreeNode = categorynode4314.Nodes.Add("node43141", "Expanded Extra Large Containers")
        categorynode43141.Nodes.Add("item1832899707", "Expanded Advanced Gravity-Inverted Container XL")
        categorynode43141.Nodes.Add("item1604594466", "Expanded Advanced Optimised Container XL")
        categorynode43141.Nodes.Add("item572613525", "Expanded Container XL")
        categorynode43141.Nodes.Add("item1832899705", "Expanded Exotic Gravity-Inverted Container XL")
        categorynode43141.Nodes.Add("item1604594468", "Expanded Exotic Optimised Container XL")
        categorynode43141.Nodes.Add("item1832899704", "Expanded Rare Gravity-Inverted Container XL")
        categorynode43141.Nodes.Add("item1604594467", "Expanded Rare Optimised Container XL")
        categorynode43141.Nodes.Add("item2431483718", "Expanded Uncommon Gravity-Inverted Container XL")
        categorynode43141.Nodes.Add("item987846328", "Expanded Uncommon Optimised Container XL")

        Dim categorynode43142 As TreeNode = categorynode4314.Nodes.Add("node43142", "Extra Large Containers")
        categorynode43142.Nodes.Add("item3514648917", "Advanced Gravity-Inverted Container XL")
        categorynode43142.Nodes.Add("item2697077621", "Advanced Optimised Container XL")
        categorynode43142.Nodes.Add("item373451737", "Container XL")
        categorynode43142.Nodes.Add("item3514648919", "Exotic Gravity-Inverted Container XL")
        categorynode43142.Nodes.Add("item2697077515", "Exotic Optimised Container XL")
        categorynode43142.Nodes.Add("item3514648916", "Rare Gravity-Inverted Container XL")
        categorynode43142.Nodes.Add("item2697077620", "Rare Optimised Container XL")
        categorynode43142.Nodes.Add("item3705714977", "Uncommon Gravity-Inverted Container XL")
        categorynode43142.Nodes.Add("item1154650699", "Uncommon Optimised Container XL")

        Dim categorynode43143 As TreeNode = categorynode4314.Nodes.Add("node43143", "Extra Small Containers")
        categorynode43143.Nodes.Add("item343666429", "Advanced Gravity-Inverted Container XS")
        categorynode43143.Nodes.Add("item4257269383", "Advanced Optimised Container XS")
        categorynode43143.Nodes.Add("item1689381593", "Container XS")
        categorynode43143.Nodes.Add("item343666431", "Exotic Gravity-Inverted Container XS")
        categorynode43143.Nodes.Add("item4257269381", "Exotic Optimised Container XS")
        categorynode43143.Nodes.Add("item343666430", "Rare Gravity-Inverted Container XS")
        categorynode43143.Nodes.Add("item4257269380", "Rare Optimised Container XS")
        categorynode43143.Nodes.Add("item2747418228", "Uncommon Gravity-Inverted Container XS")
        categorynode43143.Nodes.Add("item2000409238", "Uncommon Optimised Container XS")

        Dim categorynode43144 As TreeNode = categorynode4314.Nodes.Add("node43144", "Large Containers")
        categorynode43144.Nodes.Add("item3943113245", "Advanced Gravity-Inverted Container L")
        categorynode43144.Nodes.Add("item2504111556", "Advanced Optimised Container L")
        categorynode43144.Nodes.Add("item2125213321", "Container L")
        categorynode43144.Nodes.Add("item3943113247", "Exotic Gravity-Inverted Container L")
        categorynode43144.Nodes.Add("item2504111554", "Exotic Optimised Container L")
        categorynode43144.Nodes.Add("item3943113244", "Rare Gravity-Inverted Container L")
        categorynode43144.Nodes.Add("item2504111555", "Rare Optimised Container L")
        categorynode43144.Nodes.Add("item2004990657", "Uncommon Gravity-Inverted Container L")
        categorynode43144.Nodes.Add("item200670527", "Uncommon Optimised Container L")

        Dim categorynode43145 As TreeNode = categorynode4314.Nodes.Add("node43145", "Medium Containers")
        categorynode43145.Nodes.Add("item311555253", "Advanced Gravity-Inverted Container M")
        categorynode43145.Nodes.Add("item3983850220", "Advanced Optimised Container M")
        categorynode43145.Nodes.Add("item521274609", "Container M")
        categorynode43145.Nodes.Add("item311555255", "Exotic Gravity-Inverted Container M")
        categorynode43145.Nodes.Add("item3983850218", "Exotic Optimised Container M")
        categorynode43145.Nodes.Add("item311555254", "Rare Gravity-Inverted Container M")
        categorynode43145.Nodes.Add("item3983850219", "Rare Optimised Container M")
        categorynode43145.Nodes.Add("item2533784020", "Uncommon Gravity-Inverted Container M")
        categorynode43145.Nodes.Add("item678611231", "Uncommon Optimised Container M")

        Dim categorynode43146 As TreeNode = categorynode4314.Nodes.Add("node43146", "Small Containers")
        categorynode43146.Nodes.Add("item1123475702", "Advanced Gravity-Inverted Container S")
        categorynode43146.Nodes.Add("item2557270547", "Advanced Optimised Container S")
        categorynode43146.Nodes.Add("item1594689569", "Container S")
        categorynode43146.Nodes.Add("item1123475696", "Exotic Gravity-Inverted Container S")
        categorynode43146.Nodes.Add("item2557270549", "Exotic Optimised Container S")
        categorynode43146.Nodes.Add("item1123475697", "Rare Gravity-Inverted Container S")
        categorynode43146.Nodes.Add("item2557270546", "Rare Optimised Container S")
        categorynode43146.Nodes.Add("item1978507645", "Uncommon Gravity-Inverted Container S")
        categorynode43146.Nodes.Add("item3801121529", "Uncommon Optimised Container S")

        Dim categorynode4315 As TreeNode = categorynode431.Nodes.Add("node4315", "Parcel Containers")
        categorynode4315.Nodes.Add("item1920590006", "Expanded Parcel Container XL")
        categorynode4315.Nodes.Add("item386276308", "Parcel Container XS")
        categorynode4315.Nodes.Add("item4029924807", "Parcel Container S")
        categorynode4315.Nodes.Add("item4029924861", "Parcel Container M")
        categorynode4315.Nodes.Add("item4029924862", "Parcel Container L")
        categorynode4315.Nodes.Add("item386276317", "Parcel Container XL")

        Dim categorynode432 As TreeNode = categorynode43.Nodes.Add("node432", "Industry")
        Dim categorynode4321 As TreeNode = categorynode432.Nodes.Add("node4321", "Advanced Industry")
        categorynode4321.Nodes.Add("item2793358079", "Advanced 3D Printer M")
        categorynode4321.Nodes.Add("item2480928551", "Advanced Assembly Line XS")
        categorynode4321.Nodes.Add("item1762226232", "Advanced Assembly Line S")
        categorynode4321.Nodes.Add("item1762227888", "Advanced Assembly Line M")
        categorynode4321.Nodes.Add("item1762226675", "Advanced Assembly Line L")
        categorynode4321.Nodes.Add("item2480866767", "Advanced Assembly Line XL")
        categorynode4321.Nodes.Add("item648743080", "Advanced Chemical Industry M")
        categorynode4321.Nodes.Add("item2861848557", "Advanced Electronics Industry M")
        categorynode4321.Nodes.Add("item2200747731", "Advanced Glass Furnace M")
        categorynode4321.Nodes.Add("item3026799988", "Advanced Honeycomb Refinery M")
        categorynode4321.Nodes.Add("item2808015397", "Advanced Metalwork Industry M")
        categorynode4321.Nodes.Add("item3264314259", "Advanced Recycler M")
        categorynode4321.Nodes.Add("item584577124", "Advanced Refiner M")
        categorynode4321.Nodes.Add("item1132446361", "Advanced Smelter M")

        Dim categorynode4322 As TreeNode = categorynode43.Nodes.Add("node4322", "Basic Industry")
        categorynode4322.Nodes.Add("item409410678", "basic 3D Printer M")
        categorynode4322.Nodes.Add("item1762226876", "basic Assembly Line XS")
        categorynode4322.Nodes.Add("item983225818", "basic Assembly Line S")
        categorynode4322.Nodes.Add("item983225808", "basic Assembly Line M")
        categorynode4322.Nodes.Add("item983225811", "basic Assembly Line L")
        categorynode4322.Nodes.Add("item1762226819", "basic Assembly Line XL")
        categorynode4322.Nodes.Add("item2681009434", "basic Chemical industry M")
        categorynode4322.Nodes.Add("item2702446443", "basic Electronics industry M")
        categorynode4322.Nodes.Add("item1215026169", "basic Glass Furnace M")
        categorynode4322.Nodes.Add("item3857150880", "basic Honeycomb Refinery M")
        categorynode4322.Nodes.Add("item2022563937", "basic Metalwork Industry M")
        categorynode4322.Nodes.Add("item3914155468", "basic Recycler M")
        categorynode4322.Nodes.Add("item3701755071", "basic Refiner M")
        categorynode4322.Nodes.Add("item2556123438", "basic Smelter M")

        Dim categorynode4323 As TreeNode = categorynode43.Nodes.Add("node4323", "Rare Industry")
        categorynode4323.Nodes.Add("item2793358076", "Rare 3D Printer M")
        categorynode4323.Nodes.Add("item2480928544", "Rare Assembly Line XS")
        categorynode4323.Nodes.Add("item1762226233", "Rare Assembly Line S")
        categorynode4323.Nodes.Add("item1762227889", "Rare Assembly Line M")
        categorynode4323.Nodes.Add("item1762226674", "Rare Assembly Line L")
        categorynode4323.Nodes.Add("item2480866766", "Rare Assembly Line XL")
        categorynode4323.Nodes.Add("item648743081", "Rare Chemical Industry M")
        categorynode4323.Nodes.Add("item2861848556", "Rare Electronics Industry M")
        categorynode4323.Nodes.Add("item2200747730", "Rare Glass Furnace M")
        categorynode4323.Nodes.Add("item3026799989", "Rare Honeycomb Refinery M")
        categorynode4323.Nodes.Add("item2808015396", "Rare Metalwork Industry M")
        categorynode4323.Nodes.Add("item3264314284", "Rare Recycler M")
        categorynode4323.Nodes.Add("item584577123", "Rare Refiner M")
        categorynode4323.Nodes.Add("item1132446358", "Rare Smelter M")

        categorynode432.Nodes.Add("item4139262245", "Transfer Unit")

        Dim categorynode4324 As TreeNode = categorynode43.Nodes.Add("node4324", "Uncommon Industry")
        categorynode4324.Nodes.Add("item2793358078", "Uncommon 3D Printer M")
        categorynode4324.Nodes.Add("item2480928550", "Uncommon Assembly Line XS")
        categorynode4324.Nodes.Add("item1762226235", "Uncommon Assembly Line S")
        categorynode4324.Nodes.Add("item1762227855", "Uncommon Assembly Line M")
        categorynode4324.Nodes.Add("item1762226636", "Uncommon Assembly Line L")
        categorynode4324.Nodes.Add("item2480866760", "Uncommon Assembly Line XL")
        categorynode4324.Nodes.Add("item648743083", "Uncommon Chemical Industry M")
        categorynode4324.Nodes.Add("item2861848558", "Uncommon Electronics Industry M")
        categorynode4324.Nodes.Add("item2200747728", "Uncommon Glass Furnace M")
        categorynode4324.Nodes.Add("item3026799987", "Uncommon Honeycomb Refinery M")
        categorynode4324.Nodes.Add("item2808015394", "Uncommon Metalwork Industry M")
        categorynode4324.Nodes.Add("item3264314258", "Uncommon Recycler M")
        categorynode4324.Nodes.Add("item584577125", "Uncommon Refiner M")
        categorynode4324.Nodes.Add("item1132446360", "Uncommon Smelter M")

        Dim categorynode433 As TreeNode = categorynode43.Nodes.Add("node433", "Mining Units")
        categorynode433.Nodes.Add("item3204140766", "Advanced Mining Unit L")
        categorynode433.Nodes.Add("item1949562989", "Basic Mining Unit S")
        categorynode433.Nodes.Add("item3204140760", "Basic Mining Unit L")
        categorynode433.Nodes.Add("item3204140764", "Exotic Mining Unit L")
        categorynode433.Nodes.Add("item3204140767", "Rare Mining Unit L")
        categorynode433.Nodes.Add("item3204140761", "Uncommon Mining Unit L")

        Dim categorynode434 As TreeNode = categorynode43.Nodes.Add("node433", "Plasma Extractors")
        categorynode434.Nodes.Add("item4024529716", "Relic Plasma Extractor L")

        Dim categorynode44 As TreeNode = categorynode4.Nodes.Add("node44", "Planet Elements")
        Dim categorynode441 As TreeNode = categorynode44.Nodes.Add("node441", "Core Units")
        categorynode441.Nodes.Add("item3511907544", "Alien Space Core Unit L")

        Dim categorynode4411 As TreeNode = categorynode441.Nodes.Add("node4411", "Dynamic Core Units")
        categorynode4411.Nodes.Add("item183890713", "Dynamic Core Unit XS")
        categorynode4411.Nodes.Add("item183890525", "Dynamic Core Unit S")
        categorynode4411.Nodes.Add("item1418170469", "Dynamic Core Unit M")
        categorynode4411.Nodes.Add("item1417952990", "Dynamic Core Unit L")

        Dim categorynode4412 As TreeNode = categorynode441.Nodes.Add("node4412", "Space Core Units")
        categorynode4412.Nodes.Add("item3624942103", "Space Core Unit XS")
        categorynode4412.Nodes.Add("item3624940909", "Space Core Unit S")
        categorynode4412.Nodes.Add("item5904195", "Space Core Unit M")
        categorynode4412.Nodes.Add("item5904544", "Space Core Unit L")

        Dim categorynode4413 As TreeNode = categorynode441.Nodes.Add("node4413", "Static Core Units")
        categorynode4413.Nodes.Add("item2738359963", "Static Core Unit XS")
        categorynode4413.Nodes.Add("item2738359893", "Static Core Unit S")
        categorynode4413.Nodes.Add("item909184430", "Static Core Unit M")
        categorynode4413.Nodes.Add("item910155097", "Static Core Unit L")

        Dim categorynode442 As TreeNode = categorynode44.Nodes.Add("node442", "Territory Units")
        categorynode442.Nodes.Add("item1358842892", "Territory Unit")

        Dim categorynode45 As TreeNode = categorynode4.Nodes.Add("node45", "Systems")
        Dim categorynode451 As TreeNode = categorynode45.Nodes.Add("node451", "Control Units")
        Dim categorynode4511 As TreeNode = categorynode451.Nodes.Add("node4511", "Emergency Controllers")
        categorynode4511.Nodes.Add("item286542481", "Emergency controller")

        Dim categorynode4512 As TreeNode = categorynode451.Nodes.Add("node4512", "Generic Control Units")
        categorynode4512.Nodes.Add("item3415128439", "Programming board")

        Dim categorynode4513 As TreeNode = categorynode451.Nodes.Add("node4513", "Gunner Module")
        categorynode4513.Nodes.Add("item1373443625", "Gunner Module S")
        categorynode4513.Nodes.Add("item564736657", "Gunner Module M")
        categorynode4513.Nodes.Add("item3327293642", "Gunner Module L")

        Dim categorynode4514 As TreeNode = categorynode451.Nodes.Add("node4514", "Piloting Control Units")
        Dim categorynode45141 As TreeNode = categorynode4514.Nodes.Add("node45141", "Closed Cockpits")
        categorynode45141.Nodes.Add("item3640291983", "Cockpit controller")

        Dim categorynode45142 As TreeNode = categorynode4514.Nodes.Add("node45142", "Command Seat")
        categorynode45142.Nodes.Add("item3655856020", "Command seat controller")

        Dim categorynode45143 As TreeNode = categorynode4514.Nodes.Add("node45143", "Hovercraft cockpits")
        categorynode45143.Nodes.Add("item1744160618", "Hovercraft seat controller")

        Dim categorynode45144 As TreeNode = categorynode4514.Nodes.Add("node45144", "Remote Controllers")
        categorynode45144.Nodes.Add("item1866437084", "Remote Controller")

        Dim categorynode452 As TreeNode = categorynode45.Nodes.Add("node452", "Deep Space Asteroid Tracker")
        categorynode452.Nodes.Add("item2413564665", "Deep Space Asteroid Tracker")

        Dim categorynode453 As TreeNode = categorynode45.Nodes.Add("node453", "Resurrection Nodes")
        categorynode453.Nodes.Add("item1109114394", "Resurrection Node")

        Dim categorynode454 As TreeNode = categorynode45.Nodes.Add("node454", "Surrogate Station Equipment")
        categorynode454.Nodes.Add("item3667785070", "Surrogate Pod Station")
        categorynode454.Nodes.Add("item2093838343", "Surrogate VR Station")

        Dim categorynode455 As TreeNode = categorynode45.Nodes.Add("node455", "Territory Scanners")
        categorynode455.Nodes.Add("item3858829819", "Territory Scanner")

        Dim categorynode46 As TreeNode = categorynode4.Nodes.Add("node46", "Transportation Elements")
        Dim categorynode461 As TreeNode = categorynode46.Nodes.Add("node461", "Airfoil")
        Dim categorynode4611 As TreeNode = categorynode461.Nodes.Add("node4611", "Aileron")
        categorynode4611.Nodes.Add("item2292270972", "Aileron XS")
        categorynode4611.Nodes.Add("item2737703104", "Aileron S")
        categorynode4611.Nodes.Add("item1856288931", "Aileron M")
        categorynode4611.Nodes.Add("item2334843027", "Compact Aileron XS")
        categorynode4611.Nodes.Add("item1923840124", "Compact Aileron S")
        categorynode4611.Nodes.Add("item4017253256", "Compact Aileron M")

        Dim categorynode4612 As TreeNode = categorynode461.Nodes.Add("node4612", "Stabilizer")
        categorynode4612.Nodes.Add("item1455311973", "Stabilizer XS")
        categorynode4612.Nodes.Add("item1234961120", "Stabilizer S")
        categorynode4612.Nodes.Add("item3474622996", "Stabilizer M")
        categorynode4612.Nodes.Add("item1090402453", "Stabilizer L")

        Dim categorynode4613 As TreeNode = categorynode461.Nodes.Add("node4613", "Wing")
        categorynode4613.Nodes.Add("item1727614690", "Wing XS")
        categorynode4613.Nodes.Add("item2532454166", "Wing S")
        categorynode4613.Nodes.Add("item404188468", "Wing M")
        categorynode4613.Nodes.Add("item4179758576", "Wing variant M")

        Dim categorynode462 As TreeNode = categorynode46.Nodes.Add("node462", "Atmospheric Brakes") 'Really, NQ?
        Dim categorynode4621 As TreeNode = categorynode462.Nodes.Add("node4621", "Airbrakes")
        categorynode4621.Nodes.Add("item65048663", "Atmospheric Airbrake S")
        categorynode4621.Nodes.Add("item2198271703", "Atmospheric Airbrake M")
        categorynode4621.Nodes.Add("item104971834", "Atmospheric Airbrake L")

        Dim categorynode4622 As TreeNode = categorynode462.Nodes.Add("node4622", "Space Brake")
        categorynode4622.Nodes.Add("item3039211660", "Retro-rocket Brake S")
        categorynode4622.Nodes.Add("item3243532126", "Retro-rocket Brake M")
        categorynode4622.Nodes.Add("item1452351552", "Retro-rocket Brake L")

        Dim categorynode463 As TreeNode = categorynode46.Nodes.Add("node463", "Engines")
        Dim categorynode4631 As TreeNode = categorynode463.Nodes.Add("node4631", "Adjustors")
        categorynode4631.Nodes.Add("item2648523849", "Adjustor XS")
        categorynode4631.Nodes.Add("item47474508", "Adjustor S")
        categorynode4631.Nodes.Add("item3790013467", "Adjustor M")
        categorynode4631.Nodes.Add("item2818864930", "Adjustor L")

        Dim categorynode4632 As TreeNode = categorynode463.Nodes.Add("node4632", "Atmospheric Engines")
        Dim categorynode46321 As TreeNode = categorynode4632.Nodes.Add("node46321", "Atmospheric Engines L")
        categorynode46321.Nodes.Add("item1638517115", "Advanced Freight Atmospheric Engine L")
        categorynode46321.Nodes.Add("item1397818124", "Advanced Maneuver Atmospheric Engine L")
        categorynode46321.Nodes.Add("item2559369183", "Advanced Military Atmospheric Engine L")
        categorynode46321.Nodes.Add("item3211645339", "Advanced Safe Atmospheric Engine L")
        categorynode46321.Nodes.Add("item2375915630", "Basic Atmospheric Engine L")
        categorynode46321.Nodes.Add("item1638517113", "Exotic Freight Atmospheric Engine L")
        categorynode46321.Nodes.Add("item1397818122", "Exotic Maneuver Atmospheric Engine L")
        categorynode46321.Nodes.Add("item2559369177", "Exotic Military Atmospheric Engine L")
        categorynode46321.Nodes.Add("item3211645333", "Exotic Safe Atmospheric Engine L")
        categorynode46321.Nodes.Add("item1638517112", "Rare Freight Atmospheric Engine L")
        categorynode46321.Nodes.Add("item1397818123", "Rare Maneuver Atmospheric Engine L")
        categorynode46321.Nodes.Add("item2559369176", "Rare Military Atmospheric Engine L")
        categorynode46321.Nodes.Add("item3211645332", "Rare Safe Atmospheric Engine L")
        categorynode46321.Nodes.Add("item1053170502", "Uncommon Freight Atmospheric Engine L")
        categorynode46321.Nodes.Add("item3475626911", "Uncommon Maneuver Atmospheric Engine L")
        categorynode46321.Nodes.Add("item2714399324", "Uncommon Military Atmospheric Engine L")
        categorynode46321.Nodes.Add("item2510112556", "Uncommon Safe Atmospheric Engine L")

        Dim categorynode46322 As TreeNode = categorynode4632.Nodes.Add("node46322", "Atmospheric Engines M")
        categorynode46322.Nodes.Add("item488092470", "Advanced Freight Atmospheric Engine M")
        categorynode46322.Nodes.Add("item3377917825", "Advanced Maneuver Atmospheric Engine M")
        categorynode46322.Nodes.Add("item790956353", "Advanced Military Atmospheric Engine M")
        categorynode46322.Nodes.Add("item2370891601", "Advanced Safe Atmospheric Engine M")
        categorynode46322.Nodes.Add("item4072611011", "Basic Atmospheric Engine M")
        categorynode46322.Nodes.Add("item488092468", "Exotic Freight Atmospheric Engine M")
        categorynode46322.Nodes.Add("item3377917831", "Exotic Maneuver Atmospheric Engine M")
        categorynode46322.Nodes.Add("item790956383", "Exotic Military Atmospheric Engine M")
        categorynode46322.Nodes.Add("item2370891603", "Exotic Safe Atmospheric Engine M")
        categorynode46322.Nodes.Add("item488092471", "Rare Freight Atmospheric Engine M")
        categorynode46322.Nodes.Add("item3377917824", "Rare Maneuver Atmospheric Engine M")
        categorynode46322.Nodes.Add("item790956382", "Rare Military Atmospheric Engine M")
        categorynode46322.Nodes.Add("item2370891600", "Rare Safe Atmospheric Engine M")
        categorynode46322.Nodes.Add("item230429858", "Uncommon Freight Atmospheric Engine M")
        categorynode46322.Nodes.Add("item3847351355", "Uncommon Maneuver Atmospheric Engine M")
        categorynode46322.Nodes.Add("item3295665550", "Uncommon Military Atmospheric Engine M")
        categorynode46322.Nodes.Add("item260237137", "Uncommon Safe Atmospheric Engine M")

        Dim categorynode46323 As TreeNode = categorynode4632.Nodes.Add("node46323", "Atmospheric Engines S")
        categorynode46323.Nodes.Add("item1152783534", "Advanced Freight Atmospheric Engine S")
        categorynode46323.Nodes.Add("item1301142496", "Advanced Maneuver Atmospheric Engine S")
        categorynode46323.Nodes.Add("item385121456", "Advanced Military Atmospheric Engine S")
        categorynode46323.Nodes.Add("item3689697794", "Advanced Safe Atmospheric Engine S")
        categorynode46323.Nodes.Add("item2043566501", "Basic Atmospheric Engine S")
        categorynode46323.Nodes.Add("item1152783520", "Exotic Freight Atmospheric Engine S")
        categorynode46323.Nodes.Add("item1301142502", "Exotic Maneuver Atmospheric Engine S")
        categorynode46323.Nodes.Add("item385121458", "Exotic Military Atmospheric Engine S")
        categorynode46323.Nodes.Add("item3689697820", "Exotic Safe Atmospheric Engine S")
        categorynode46323.Nodes.Add("item1152783535", "Rare Freight Atmospheric Engine S")
        categorynode46323.Nodes.Add("item1301142497", "Rare Maneuver Atmospheric Engine S")
        categorynode46323.Nodes.Add("item385121459", "Rare Military Atmospheric Engine S")
        categorynode46323.Nodes.Add("item3689697821", "Rare Safe Atmospheric Engine S")
        categorynode46323.Nodes.Add("item1503780712", "Uncommon Freight Atmospheric Engine S")
        categorynode46323.Nodes.Add("item317861818", "Uncommon Maneuver Atmospheric Engine S")
        categorynode46323.Nodes.Add("item2203746213", "Uncommon Military Atmospheric Engine S")
        categorynode46323.Nodes.Add("item1679964557", "Uncommon Safe Atmospheric Engine S")

        Dim categorynode46324 As TreeNode = categorynode4632.Nodes.Add("node46324", "Atmospheric Engines XS")
        categorynode46324.Nodes.Add("item2711764151", "Advanced Freight Atmospheric Engine XS")
        categorynode46324.Nodes.Add("item4201522399", "Advanced Maneuver Atmospheric Engine XS")
        categorynode46324.Nodes.Add("item2472120802", "Advanced Military Atmospheric Engine XS")
        categorynode46324.Nodes.Add("item3612851279", "Advanced Safe Atmospheric Engine XS")
        categorynode46324.Nodes.Add("item710193240", "Basic Atmospheric Engine XS")
        categorynode46324.Nodes.Add("item2711763785", "Exotic Freight Atmospheric Engine XS")
        categorynode46324.Nodes.Add("item4201522393", "Exotic Maneuver Atmospheric Engine XS")
        categorynode46324.Nodes.Add("item2472120804", "Exotic Military Atmospheric Engine XS")
        categorynode46324.Nodes.Add("item3612851273", "Exotic Safe Atmospheric Engine XS")
        categorynode46324.Nodes.Add("item2711764150", "Rare Freight Atmospheric Engine XS")
        categorynode46324.Nodes.Add("item4201522392", "Rare Maneuver Atmospheric Engine XS")
        categorynode46324.Nodes.Add("item2472120803", "Rare Military Atmospheric Engine XS")
        categorynode46324.Nodes.Add("item3612851272", "Rare Safe Atmospheric Engine XS")
        categorynode46324.Nodes.Add("item3174850377", "Uncommon Freight Atmospheric Engine XS")
        categorynode46324.Nodes.Add("item1933133404", "Uncommon Maneuver Atmospheric Engine XS")
        categorynode46324.Nodes.Add("item676012472", "Uncommon Military Atmospheric Engine XS")
        categorynode46324.Nodes.Add("item887167900", "Uncommon Safe Atmospheric Engine XS")

        Dim categorynode4633 As TreeNode = categorynode463.Nodes.Add("node4633", "Ground Engines")
        Dim categorynode46331 As TreeNode = categorynode4633.Nodes.Add("node46331", "Hovercraft Engines")
        Dim categorynode463311 As TreeNode = categorynode46331.Nodes.Add("node463311", "Large Hovercraft Engines")
        categorynode463311.Nodes.Add("item650556742", "Advanced Freight Flat Hover Engine L")
        categorynode463311.Nodes.Add("item2776777570", "Advanced Freight Hover Engine L")
        categorynode463311.Nodes.Add("item2943949391", "Advanced Maneuver Flat Hover Engine L")
        categorynode463311.Nodes.Add("item2642637995", "Advanced Maneuver Hover Engine L")
        categorynode463311.Nodes.Add("item1559669873", "Advanced Military Flat Hover Engine L")
        categorynode463311.Nodes.Add("item1943212688", "Advanced Military Hover Engine L")
        categorynode463311.Nodes.Add("item650556760", "Exotic Freight Flat Hover Engine L")
        categorynode463311.Nodes.Add("item2776777596", "Exotic Freight Hover Engine L")
        categorynode463311.Nodes.Add("item2943949389", "Exotic Maneuver Flat Hover Engine L")
        categorynode463311.Nodes.Add("item2642637993", "Exotic Maneuver Hover Engine L")
        categorynode463311.Nodes.Add("item1559669875", "Exotic Military Flat Hover Engine L")
        categorynode463311.Nodes.Add("item1943212690", "Exotic Military Hover Engine L")
        categorynode463311.Nodes.Add("item1105322870", "Flat Hover Engine L")
        categorynode463311.Nodes.Add("item2494203891", "Hover Engine L")
        categorynode463311.Nodes.Add("item650556743", "Rare Freight Flat Hover Engine L")
        categorynode463311.Nodes.Add("item2776777597", "Rare Freight Hover Engine L")
        categorynode463311.Nodes.Add("item2943949390", "Rare Maneuver Flat Hover Engine L")
        categorynode463311.Nodes.Add("item2642637992", "Rare Maneuver Hover Engine L")
        categorynode463311.Nodes.Add("item1559669874", "Rare Military Flat Hover Engine L")
        categorynode463311.Nodes.Add("item1943212689", "Rare Military Hover Engine L")
        categorynode463311.Nodes.Add("item2680916872", "Uncommon Freight Flat Hover Engine L")
        categorynode463311.Nodes.Add("item2972230925", "Uncommon Freight Hover Engine L")
        categorynode463311.Nodes.Add("item3460169859", "Uncommon Maneuver Flat Hover Engine L")
        categorynode463311.Nodes.Add("item3455781126", "Uncommon Maneuver Hover Engine L")
        categorynode463311.Nodes.Add("item50590628", "Uncommon Military Flat Hover Engine L")
        categorynode463311.Nodes.Add("item1311801168", "Uncommon Military Hover Engine L")

        Dim categorynode463312 As TreeNode = categorynode46331.Nodes.Add("node463312", "Medium Hovercraft Engines")
        categorynode463312.Nodes.Add("item3919255960", "Advanced Freight Hover Engine M")
        categorynode463312.Nodes.Add("item3750012040", "Advanced Maneuver Hover Engine M")
        categorynode463312.Nodes.Add("item3087709976", "Advanced Military Hover Engine M")
        categorynode463312.Nodes.Add("item3919255966", "Exotic Freight Hover Engine M")
        categorynode463312.Nodes.Add("item3750012042", "Exotic Maneuver Hover Engine M")
        categorynode463312.Nodes.Add("item3087709978", "Exotic Military Hover Engine M")
        categorynode463312.Nodes.Add("item2991279664", "Hover Engine M")
        categorynode463312.Nodes.Add("item3919255961", "Rare Freight Hover Engine M")
        categorynode463312.Nodes.Add("item3750012043", "Rare Maneuver Hover Engine M")
        categorynode463312.Nodes.Add("item3087709977", "Rare Military Hover Engine M")
        categorynode463312.Nodes.Add("item2137235802", "Uncommon Freight Hover Engine M")
        categorynode463312.Nodes.Add("item3192913797", "Uncommon Maneuver Hover Engine M")
        categorynode463312.Nodes.Add("item206298096", "Uncommon Military Hover Engine M")

        Dim categorynode463313 As TreeNode = categorynode46331.Nodes.Add("node463313", "Small Hovercraft Engines")
        categorynode463313.Nodes.Add("item1468805963", "Advanced Freight Hover Engine S")
        categorynode463313.Nodes.Add("item3493967061", "Advanced Maneuver Hover Engine S")
        categorynode463313.Nodes.Add("item3032489975", "Advanced Military Hover Engine S")
        categorynode463313.Nodes.Add("item1468805961", "Exotic Freight Hover Engine S")
        categorynode463313.Nodes.Add("item3493967063", "Exotic Maneuver Hover Engine S")
        categorynode463313.Nodes.Add("item3032489865", "Exotic Military Hover Engine S")
        categorynode463313.Nodes.Add("item2333052331", "Hover Engine S")
        categorynode463313.Nodes.Add("item1468805960", "Rare Freight Hover Engine S")
        categorynode463313.Nodes.Add("item3493967060", "Rare Maneuver Hover Engine S")
        categorynode463313.Nodes.Add("item3032489974", "Rare Military Hover Engine S")
        categorynode463313.Nodes.Add("item1779666310", "Uncommon Freight Hover Engine S")
        categorynode463313.Nodes.Add("item148948769", "Uncommon Maneuver Hover Engine S")
        categorynode463313.Nodes.Add("item141653577", "Uncommon Military Hover Engine S")

        Dim categorynode46332 As TreeNode = categorynode4633.Nodes.Add("node46332", "Vertical Boosters")
        Dim categorynode463321 As TreeNode = categorynode46332.Nodes.Add("node463321", "Extra Small Vertical Boosters")
        categorynode463321.Nodes.Add("item3536863443", "Advanced Freight Vertical Booster XS")
        categorynode463321.Nodes.Add("item2978750919", "Advanced Maneuver Vertical Booster XS")
        categorynode463321.Nodes.Add("item2867114992", "Advanced Military Vertical Booster XS")
        categorynode463321.Nodes.Add("item3536863441", "Exotic Freight Vertical Booster XS")
        categorynode463321.Nodes.Add("item2978750937", "Exotic Maneuver Vertical Booster XS")
        categorynode463321.Nodes.Add("item2867114998", "Exotic Military Vertical Booster XS")
        categorynode463321.Nodes.Add("item3536863442", "Rare Freight Vertical Booster XS")
        categorynode463321.Nodes.Add("item2978750918", "Rare Maneuver Hover Engine XS")
        categorynode463321.Nodes.Add("item2867114993", "Rare Military Hover Engine XS")
        categorynode463321.Nodes.Add("item2584744787", "Uncommon Freight Vertical Booster XS")
        categorynode463321.Nodes.Add("item2967184521", "Uncommon Maneuver Vertical Booster XS")
        categorynode463321.Nodes.Add("item2515360890", "Uncommon Military Vertical Booster XS")
        categorynode463321.Nodes.Add("item3775402879", "Vertical Booster XS")

        Dim categorynode463322 As TreeNode = categorynode46332.Nodes.Add("node463322", "Large Vertical Boosters")
        categorynode463322.Nodes.Add("item1203066130", "Advanced Freight Vertical Booster L")
        categorynode463322.Nodes.Add("item1084121317", "Advanced Maneuver Vertical Booster L")
        categorynode463322.Nodes.Add("item2210390722", "Advanced Military Vertical Booster L")
        categorynode463322.Nodes.Add("item1203066128", "Exotic Freight Vertical Booster L")
        categorynode463322.Nodes.Add("item1084121319", "Exotic Maneuver Vertical Booster L")
        categorynode463322.Nodes.Add("item2210390724", "Exotic Military Vertical Booster L")
        categorynode463322.Nodes.Add("item1203066131", "Rare Freight Vertical Booster L")
        categorynode463322.Nodes.Add("item1084121316", "Rare Maneuver Hover Engine L")
        categorynode463322.Nodes.Add("item2210390723", "Rare Military Hover Engine L")
        categorynode463322.Nodes.Add("item923971410", "Uncommon Freight Vertical Booster L")
        categorynode463322.Nodes.Add("item4015451489", "Uncommon Maneuver Vertical Booster L")
        categorynode463322.Nodes.Add("item3319454776", "Uncommon Military Vertical Booster L")
        categorynode463322.Nodes.Add("item2216363013", "Vertical Booster L")

        Dim categorynode463323 As TreeNode = categorynode46332.Nodes.Add("node463323", "Medium Vertical Boosters")
        categorynode463323.Nodes.Add("item2239456928", "Advanced Freight Vertical Booster M")
        categorynode463323.Nodes.Add("item4049794475", "Advanced Maneuver Vertical Booster M")
        categorynode463323.Nodes.Add("item650753695", "Advanced Military Vertical Booster M")
        categorynode463323.Nodes.Add("item2239456958", "Exotic Freight Vertical Booster M")
        categorynode463323.Nodes.Add("item4049794473", "Exotic Maneuver Vertical Booster M")
        categorynode463323.Nodes.Add("item650753693", "Exotic Military Vertical Booster M")
        categorynode463323.Nodes.Add("item2239456959", "Rare Freight Vertical Booster M")
        categorynode463323.Nodes.Add("item4049794474", "Rare Maneuver Hover Engine M")
        categorynode463323.Nodes.Add("item650753694", "Rare Military Hover Engine M")
        categorynode463323.Nodes.Add("item3182220303", "Uncommon Freight Vertical Booster M")
        categorynode463323.Nodes.Add("item4104758707", "Uncommon Maneuver Vertical Booster M")
        categorynode463323.Nodes.Add("item170972867", "Uncommon Military Vertical Booster M")
        categorynode463323.Nodes.Add("item913372512", "Vertical Booster M")

        Dim categorynode463324 As TreeNode = categorynode46332.Nodes.Add("node463324", "Small Vertical Boosters")
        categorynode463324.Nodes.Add("item3885920509", "Advanced Freight Vertical Booster S")
        categorynode463324.Nodes.Add("item2514351854", "Advanced Maneuver Vertical Booster S")
        categorynode463324.Nodes.Add("item1196801714", "Advanced Military Vertical Booster S")
        categorynode463324.Nodes.Add("item3885920511", "Exotic Freight Vertical Booster S")
        categorynode463324.Nodes.Add("item2514351848", "Exotic Maneuver Vertical Booster S")
        categorynode463324.Nodes.Add("item1196801356", "Exotic Military Vertical Booster S")
        categorynode463324.Nodes.Add("item3885920508", "Rare Freight Vertical Booster S")
        categorynode463324.Nodes.Add("item2514351849", "Rare Maneuver Hover Engine S")
        categorynode463324.Nodes.Add("item1196801357", "Rare Military Hover Engine S")
        categorynode463324.Nodes.Add("item3310166593", "Uncommon Freight Vertical Booster S")
        categorynode463324.Nodes.Add("item4063531805", "Uncommon Maneuver Vertical Booster S")
        categorynode463324.Nodes.Add("item2149892941", "Uncommon Military Vertical Booster S")
        categorynode463324.Nodes.Add("item3556600005", "Vertical Booster S")

        Dim categorynode4634 As TreeNode = categorynode463.Nodes.Add("node4634", "Rocket Engines")
        categorynode4634.Nodes.Add("item2112772336", "Rocket Engine S")
        categorynode4634.Nodes.Add("item3623903713", "Rocket Engine M")
        categorynode4634.Nodes.Add("item359938916", "Rocket Engine L")

        Dim categorynode4635 As TreeNode = categorynode463.Nodes.Add("node4635", "Space Engines")
        Dim categorynode46351 As TreeNode = categorynode4635.Nodes.Add("node46351", "Space Engines L")
        categorynode46351.Nodes.Add("item2809629801", "Advanced Freight Space Engine L")
        categorynode46351.Nodes.Add("item4025377657", "Advanced Maneuver Space Engine L")
        categorynode46351.Nodes.Add("item2379018394", "Advanced Military Space Engine L")
        categorynode46351.Nodes.Add("item3432389652", "Advanced Safe Space Engine L")
        categorynode46351.Nodes.Add("item2495558023", "Basic Space Engine L")
        categorynode46351.Nodes.Add("item2809629799", "Exotic Freight Space Engine L")
        categorynode46351.Nodes.Add("item4025377659", "Exotic Maneuver Space Engine L")
        categorynode46351.Nodes.Add("item2379018392", "Exotic Military Space Engine L")
        categorynode46351.Nodes.Add("item3432389654", "Exotic Safe Space Engine L")
        categorynode46351.Nodes.Add("item2809629798", "Rare Freight Space Engine L")
        categorynode46351.Nodes.Add("item4025377658", "Rare Maneuver Space Engine L")
        categorynode46351.Nodes.Add("item2379018393", "Rare Military Space Engine L")
        categorynode46351.Nodes.Add("item3432389655", "Rare Safe Space Engine L")
        categorynode46351.Nodes.Add("item273900142", "Uncommon Freight Space Engine L")
        categorynode46351.Nodes.Add("item613453124", "Uncommon Maneuver Space Engine L")
        categorynode46351.Nodes.Add("item2637003463", "Uncommon Military Space Engine L")
        categorynode46351.Nodes.Add("item892904533", "Uncommon Safe Space Engine L")

        Dim categorynode46352 As TreeNode = categorynode4635.Nodes.Add("node46352", "Space Engines M")
        categorynode46352.Nodes.Add("item516669710", "Advanced Freight Space Engine M")
        categorynode46352.Nodes.Add("item1757019469", "Advanced Maneuver Space Engine M")
        categorynode46352.Nodes.Add("item37629188", "Advanced Military Space Engine M")
        categorynode46352.Nodes.Add("item1326315524", "Advanced Safe Space Engine M")
        categorynode46352.Nodes.Add("item85796763", "Basic Space Engine M")
        categorynode46352.Nodes.Add("item516669708", "Exotic Freight Space Engine M")
        categorynode46352.Nodes.Add("item1757019459", "Exotic Maneuver Space Engine M")
        categorynode46352.Nodes.Add("item37629190", "Exotic Military Space Engine M")
        categorynode46352.Nodes.Add("item1326315526", "Exotic Safe Space Engine M")
        categorynode46352.Nodes.Add("item516669711", "Rare Freight Space Engine M")
        categorynode46352.Nodes.Add("item1757019468", "Rare Maneuver Space Engine M")
        categorynode46352.Nodes.Add("item37629189", "Rare Military Space Engine M")
        categorynode46352.Nodes.Add("item1326315525", "Rare Safe Space Engine M")
        categorynode46352.Nodes.Add("item99470466", "Uncommon Freight Space Engine M")
        categorynode46352.Nodes.Add("item3024541675", "Uncommon Maneuver Space Engine M")
        categorynode46352.Nodes.Add("item3897078752", "Uncommon Military Space Engine M")
        categorynode46352.Nodes.Add("item2489350112", "Uncommon Safe Space Engine M")

        Dim categorynode46353 As TreeNode = categorynode4635.Nodes.Add("node46353", "Space Engines S")
        categorynode46353.Nodes.Add("item270403386", "Advanced Freight Space Engine S")
        categorynode46353.Nodes.Add("item1624640879", "Advanced Maneuver Space Engine S")
        categorynode46353.Nodes.Add("item2510194716", "Advanced Military Space Engine S")
        categorynode46353.Nodes.Add("item2682344779", "Advanced Safe Space Engine S")
        categorynode46353.Nodes.Add("item1326357437", "Basic Space Engine S")
        categorynode46353.Nodes.Add("item270403388", "Exotic Freight Space Engine S")
        categorynode46353.Nodes.Add("item1624640873", "Exotic Maneuver Space Engine S")
        categorynode46353.Nodes.Add("item2510194718", "Exotic Military Space Engine S")
        categorynode46353.Nodes.Add("item2682344781", "Exotic Safe Space Engine S")
        categorynode46353.Nodes.Add("item270403387", "Rare Freight Space Engine S")
        categorynode46353.Nodes.Add("item1624640872", "Rare Maneuver Space Engine S")
        categorynode46353.Nodes.Add("item2510194717", "Rare Military Space Engine S")
        categorynode46353.Nodes.Add("item2682344778", "Rare Safe Space Engine S")
        categorynode46353.Nodes.Add("item3764949976", "Uncommon Freight Space Engine S")
        categorynode46353.Nodes.Add("item1171610140", "Uncommon Maneuver Space Engine S")
        categorynode46353.Nodes.Add("item529520576", "Uncommon Military Space Engine S")
        categorynode46353.Nodes.Add("item2090364569", "Uncommon Safe Space Engine S")


        Dim categorynode46354 As TreeNode = categorynode4635.Nodes.Add("node46354", "Space Engines XL")
        categorynode46354.Nodes.Add("item2497069958", "Advanced Freight Space Engine XL")
        categorynode46354.Nodes.Add("item1773467599", "Advanced Maneuver Space Engine XL")
        categorynode46354.Nodes.Add("item934426297", "Advanced Military Space Engine XL")
        categorynode46354.Nodes.Add("item3478227881", "Advanced Safe Space Engine XL")
        categorynode46354.Nodes.Add("item2200254788", "Basic Space Engine XL")
        categorynode46354.Nodes.Add("item2497069976", "Exotic Freight Space Engine XL")
        categorynode46354.Nodes.Add("item1773467597", "Exotic Maneuver Space Engine XL")
        categorynode46354.Nodes.Add("item934426303", "Exotic Military Space Engine XL")
        categorynode46354.Nodes.Add("item3478227883", "Exotic Safe Space Engine XL")
        categorynode46354.Nodes.Add("item2497069959", "Rare Freight Space Engine XL")
        categorynode46354.Nodes.Add("item1773467598", "Rare Maneuver Space Engine XL")
        categorynode46354.Nodes.Add("item934426296", "Rare Military Space Engine XL")
        categorynode46354.Nodes.Add("item3478227882", "Rare Safe Space Engine XL")
        categorynode46354.Nodes.Add("item130796680", "Uncommon Freight Space Engine XL")
        categorynode46354.Nodes.Add("item1237158531", "Uncommon Maneuver Space Engine XL")
        categorynode46354.Nodes.Add("item701947611", "Uncommon Military Space Engine XL")
        categorynode46354.Nodes.Add("item3846850308", "Uncommon Safe Space Engine XL")

        Dim categorynode46355 As TreeNode = categorynode4635.Nodes.Add("node46355", "Space Engines XS")
        categorynode46355.Nodes.Add("item3719125853", "Advanced Freight Space Engine XS")
        categorynode46355.Nodes.Add("item2368501172", "Advanced Maneuver Space Engine XS")
        categorynode46355.Nodes.Add("item1754053134", "Advanced Military Space Engine XS")
        categorynode46355.Nodes.Add("item175947629", "Advanced Safe Space Engine XS")
        categorynode46355.Nodes.Add("item2243775376", "Basic Space Engine XS")
        categorynode46355.Nodes.Add("item3719125843", "Exotic Freight Space Engine XS")
        categorynode46355.Nodes.Add("item2368501170", "Exotic Maneuver Space Engine XS")
        categorynode46355.Nodes.Add("item1754053132", "Exotic Military Space Engine XS")
        categorynode46355.Nodes.Add("item175947631", "Exotic Safe Space Engine XS")
        categorynode46355.Nodes.Add("item3719125852", "Rare Freight Space Engine XS")
        categorynode46355.Nodes.Add("item2368501171", "Rare Maneuver Space Engine XS")
        categorynode46355.Nodes.Add("item1754053133", "Rare Military Space Engine XS")
        categorynode46355.Nodes.Add("item175947630", "Rare Safe Space Engine XS")
        categorynode46355.Nodes.Add("item16482091", "Uncommon Freight Space Engine XS")
        categorynode46355.Nodes.Add("item1213509759", "Uncommon Maneuver Space Engine XS")
        categorynode46355.Nodes.Add("item1971700279", "Uncommon Military Space Engine XS")
        categorynode46355.Nodes.Add("item3083225012", "Uncommon Safe Space Engine XS")

        Dim categorynode464 As TreeNode = categorynode46.Nodes.Add("node464", "High-Tech Transportation")
        Dim categorynode4641 As TreeNode = categorynode464.Nodes.Add("node4641", "Anti-Gravity Generator")
        categorynode4641.Nodes.Add("item3997343699", "Anti-Gravity Generator S")
        categorynode4641.Nodes.Add("item233079829", "Anti-Gravity Generator M")
        categorynode4641.Nodes.Add("item294414265", "Anti-Gravity Generator L")

        Dim categorynode4642 As TreeNode = categorynode464.Nodes.Add("node4642", "Anti-Gravity Pulsor")
        categorynode4642.Nodes.Add("item966816758", "Anti-Gravity Pulsor")

        Dim categorynode4643 As TreeNode = categorynode464.Nodes.Add("node4643", "Warp Beacon Units")
        categorynode4643.Nodes.Add("item2468029849", "Warp Beacon XL")

        Dim categorynode4644 As TreeNode = categorynode464.Nodes.Add("node4644", "Warp Drive Units")
        categorynode4644.Nodes.Add("item2643443936", "Warp Drive L")

        Dim categorynode465 As TreeNode = categorynode46.Nodes.Add("node465", "Support Tech")
        Dim categorynode4651 As TreeNode = categorynode465.Nodes.Add("node4651", "Gyroscopes")
        categorynode4651.Nodes.Add("item2585415184", "Gyroscope")

        Dim categorynode4652 As TreeNode = categorynode465.Nodes.Add("node4652", "Landing Gears")
        categorynode4652.Nodes.Add("item4078067869", "Landing Gear XS")
        categorynode4652.Nodes.Add("item1884031929", "Landing Gear S")
        categorynode4652.Nodes.Add("item1899560165", "Landing Gear M")
        categorynode4652.Nodes.Add("item2667697870", "Landing Gear L")

        Dim categorynode5 As TreeNode = ItemTree.Nodes.Add("node5", "Materials")
        Dim categorynode51 As TreeNode = categorynode5.Nodes.Add("node51", "Fuels")
        Dim categorynode511 As TreeNode = categorynode51.Nodes.Add("node511", "Atmospheric Fuels")
        categorynode511.Nodes.Add("item2579672037", "Nitron Fuel")

        Dim categorynode512 As TreeNode = categorynode51.Nodes.Add("node512", "Rocket Fuels")
        categorynode512.Nodes.Add("item106455050", "Xeron Fuel")

        Dim categorynode513 As TreeNode = categorynode51.Nodes.Add("node513", "Space Fuels")
        categorynode513.Nodes.Add("item840202980", "Kergon-X1 fuel")
        categorynode513.Nodes.Add("item840202981", "Kergon-X2 fuel")
        categorynode513.Nodes.Add("item840202986", "Kergon-X3 fuel")
        categorynode513.Nodes.Add("item840202987", "Kergon-X4 fuel")

        Dim categorynode52 As TreeNode = categorynode5.Nodes.Add("node52", "Honeycomb materials")
        Dim categorynode521 As TreeNode = categorynode52.Nodes.Add("node521", "Product Honeycomb Materials")
        Dim categorynode5211 As TreeNode = categorynode521.Nodes.Add("node5211", "Al-Li Honeycomb")
        categorynode5211.Nodes.Add("item2906228118", "Al-Li Painted Military")
        categorynode5211.Nodes.Add("item2906228118", "Al-Li Painted Orange")
        categorynode5211.Nodes.Add("item2906228118", "Al-Li Painted Purple")
        categorynode5211.Nodes.Add("item2906228118", "Al-Li Painted Red")
        categorynode5211.Nodes.Add("item2906228118", "Al-Li Painted Sky")
        categorynode5211.Nodes.Add("item2906228118", "Al-Li Painted White")
        categorynode5211.Nodes.Add("item2906228118", "Al-Li Painted Yellow")
        categorynode5211.Nodes.Add("item2906228118", "Black Al-Li Panel")
        categorynode5211.Nodes.Add("item2906228118", "Dark Gray Al-Li Panel")
        categorynode5211.Nodes.Add("item2906228118", "Gray Al-Li Panel")
        categorynode5211.Nodes.Add("item2906228118", "Al-Li Painted Light Gray")
        categorynode5211.Nodes.Add("item2906228118", "Green Al-Li Panel")
        categorynode5211.Nodes.Add("item2906228118", "Light Gray Al-Li Panel")
        categorynode5211.Nodes.Add("item2906228118", "Military Al-Li Panel")
        categorynode5211.Nodes.Add("item2906228118", "Orange Al-Li Panel")
        categorynode5211.Nodes.Add("item2906228118", "Purple Al-Li Panel")
        categorynode5211.Nodes.Add("item2906228118", "Red Al-Li Panel")
        categorynode5211.Nodes.Add("item2906228118", "Sky Al-Li Panel")
        categorynode5211.Nodes.Add("item2906228118", "White Al-Li Panel")
        categorynode5211.Nodes.Add("item2906228118", "Yellow Al-Li Panel")
        categorynode5211.Nodes.Add("item2906228118", "Ice Al-Li Panel")
        categorynode5211.Nodes.Add("item2906228118", "Al-Li Painted Ice")
        categorynode5211.Nodes.Add("item2906228118", "Al-Li Painted Gray")
        categorynode5211.Nodes.Add("item2906228118", "Al-Li Painted Green")
        categorynode5211.Nodes.Add("item2906228118", "Aged Al-Li")
        categorynode5211.Nodes.Add("item2906228118", "Glossy Al-Li")
        categorynode5211.Nodes.Add("item2906228118", "Matte Al-Li")
        categorynode5211.Nodes.Add("item2906228118", "Al-Li Painted Black")
        categorynode5211.Nodes.Add("item2906228118", "Al-Li Painted Dark Gray")

        Dim categorynode5212 As TreeNode = categorynode521.Nodes.Add("node5212", "Brick Honeycomb")
        categorynode5212.Nodes.Add("item2698580432", "White pattern brick")
        categorynode5212.Nodes.Add("item2698580432", "Stained gray pattern brick")
        categorynode5212.Nodes.Add("item2698580432", "Aged gray pattern brick")
        categorynode5212.Nodes.Add("item2698580432", "Gray pattern brick")
        categorynode5212.Nodes.Add("item2698580432", "Stained pattern brick 4")
        categorynode5212.Nodes.Add("item2698580432", "Aged pattern brick 4")
        categorynode5212.Nodes.Add("item2698580432", "Brick pattern 4")
        categorynode5212.Nodes.Add("item2698580432", "Stained pattern brick 3")
        categorynode5212.Nodes.Add("item2698580432", "Aged pattern brick 3")
        categorynode5212.Nodes.Add("item2698580432", "Brick pattern 3")
        categorynode5212.Nodes.Add("item2698580432", "Stained pattern brick 2")
        categorynode5212.Nodes.Add("item2698580432", "Aged pattern brick 2")
        categorynode5212.Nodes.Add("item2698580432", "Brick pattern 2")
        categorynode5212.Nodes.Add("item2698580432", "Stained pattern brick 1")
        categorynode5212.Nodes.Add("item2698580432", "Aged pattern brick 1")
        categorynode5212.Nodes.Add("item2698580432", "Brick pattern 1")
        categorynode5212.Nodes.Add("item2698580432", "Black pattern brick")
        categorynode5212.Nodes.Add("item2698580432", "Matte white brick")
        categorynode5212.Nodes.Add("item2698580432", "Matte gray brick")
        categorynode5212.Nodes.Add("item2698580432", "Matte light brick 4")
        categorynode5212.Nodes.Add("item2698580432", "Matte dark brick 4")
        categorynode5212.Nodes.Add("item2698580432", "Matte black brick 4")
        categorynode5212.Nodes.Add("item2698580432", "Matte light brick 3")
        categorynode5212.Nodes.Add("item2698580432", "Matte dark brick 3")
        categorynode5212.Nodes.Add("item2698580432", "Matte black brick 3")
        categorynode5212.Nodes.Add("item2698580432", "Matte light brick 2")
        categorynode5212.Nodes.Add("item2698580432", "Matte dark brick 2")
        categorynode5212.Nodes.Add("item2698580432", "Matte black brick 2")
        categorynode5212.Nodes.Add("item2698580432", "Matte light brick 1")
        categorynode5212.Nodes.Add("item2698580432", "Matte dark brick 1")
        categorynode5212.Nodes.Add("item2698580432", "Matte black brick 1")
        categorynode5212.Nodes.Add("item2698580432", "Matte black brick")
        categorynode5212.Nodes.Add("item2698580432", "Waxed white brick")
        categorynode5212.Nodes.Add("item2698580432", "Waxed light gray brick")
        categorynode5212.Nodes.Add("item2698580432", "Waxed dark gray brick")
        categorynode5212.Nodes.Add("item2698580432", "Waxed gray brick")
        categorynode5212.Nodes.Add("item2698580432", "Waxed stained brick 4")
        categorynode5212.Nodes.Add("item2698580432", "Waxed aged brick 4")
        categorynode5212.Nodes.Add("item2698580432", "Waxed brick 4")
        categorynode5212.Nodes.Add("item2698580432", "Waxed stained brick 3")
        categorynode5212.Nodes.Add("item2698580432", "Waxed aged brick 3")
        categorynode5212.Nodes.Add("item2698580432", "Waxed brick 3")
        categorynode5212.Nodes.Add("item2698580432", "Waxed stained brick 2")
        categorynode5212.Nodes.Add("item2698580432", "Waxed aged brick 2")
        categorynode5212.Nodes.Add("item2698580432", "Waxed brick 2")
        categorynode5212.Nodes.Add("item2698580432", "Waxed stained brick 1")
        categorynode5212.Nodes.Add("item2698580432", "Waxed aged brick 1")
        categorynode5212.Nodes.Add("item2698580432", "Waxed brick 1")
        categorynode5212.Nodes.Add("item2698580432", "Waxed black brick")
        categorynode5212.Nodes.Add("item2698580432", "Matte dark gray brick")
        categorynode5212.Nodes.Add("item2698580432", "Matte light gray brick")

        Dim categorynode5213 As TreeNode = categorynode521.Nodes.Add("node5213", "Carbon Fiber Honeycomb")
        categorynode5213.Nodes.Add("item2647328640", "White pattern carbon fiber")
        categorynode5213.Nodes.Add("item2647328640", "Stained gray pattern carbon fiber")
        categorynode5213.Nodes.Add("item2647328640", "Aged gray pattern carbon fiber")
        categorynode5213.Nodes.Add("item2647328640", "Gray pattern carbon fiber")
        categorynode5213.Nodes.Add("item2647328640", "Black pattern carbon fiber")
        categorynode5213.Nodes.Add("item2647328640", "Matte white carbon fiber")
        categorynode5213.Nodes.Add("item2647328640", "Matte light gray carbon fiber")
        categorynode5213.Nodes.Add("item2647328640", "Matte dark gray carbon fiber")
        categorynode5213.Nodes.Add("item2647328640", "Matte gray carbon fiber")
        categorynode5213.Nodes.Add("item2647328640", "Matte black carbon fiber")
        categorynode5213.Nodes.Add("item2647328640", "Glossy white carbon fiber")
        categorynode5213.Nodes.Add("item2647328640", "Glossy light gray carbon fiber")
        categorynode5213.Nodes.Add("item2647328640", "Glossy dark gray carbon fiber")
        categorynode5213.Nodes.Add("item2647328640", "Glossy gray carbon fiber")
        categorynode5213.Nodes.Add("item2647328640", "Glossy black carbon fiber")

        Dim categorynode5214 As TreeNode = categorynode521.Nodes.Add("node5214", "Concrete Honeycomb")
        categorynode5214.Nodes.Add("item38264863", "White pattern concrete")
        categorynode5214.Nodes.Add("item38264863", "Stained gray pattern concrete")
        categorynode5214.Nodes.Add("item38264863", "Aged gray pattern concrete")
        categorynode5214.Nodes.Add("item38264863", "Gray pattern concrete")
        categorynode5214.Nodes.Add("item38264863", "Black pattern concrete")
        categorynode5214.Nodes.Add("item38264863", "Matte white concrete")
        categorynode5214.Nodes.Add("item38264863", "Matte light gray concrete")
        categorynode5214.Nodes.Add("item38264863", "Matte dark gray concrete")
        categorynode5214.Nodes.Add("item38264863", "Matte gray concrete")
        categorynode5214.Nodes.Add("item38264863", "Matte black concrete")
        categorynode5214.Nodes.Add("item38264863", "Waxed white concrete")
        categorynode5214.Nodes.Add("item38264863", "Waxed light gray concrete")
        categorynode5214.Nodes.Add("item38264863", "Waxed dark gray concrete")
        categorynode5214.Nodes.Add("item38264863", "Waxed gray concrete")
        categorynode5214.Nodes.Add("item38264863", "Waxed black concrete")

        Dim categorynode5215 As TreeNode = categorynode521.Nodes.Add("node5215", "Duralumin Honeycomb")
        categorynode5215.Nodes.Add("item1993502154", "Yellow Duralumin Panel")
        categorynode5215.Nodes.Add("item1993502154", "White Duralumin Panel")
        categorynode5215.Nodes.Add("item1993502154", "Sky Duralumin Panel")
        categorynode5215.Nodes.Add("item1993502154", "Red Duralumin Panel")
        categorynode5215.Nodes.Add("item1993502154", "Painted Black Duralumin")
        categorynode5215.Nodes.Add("item1993502154", "Painted Dark Gray Duralumin")
        categorynode5215.Nodes.Add("item1993502154", "Painted Gray Duralumin")
        categorynode5215.Nodes.Add("item1993502154", "Painted Green Duralumin")
        categorynode5215.Nodes.Add("item1993502154", "Painted Ice Duralumin")
        categorynode5215.Nodes.Add("item1993502154", "Painted Light Gray Duralumin")
        categorynode5215.Nodes.Add("item1993502154", "Painted Military Duralumin")
        categorynode5215.Nodes.Add("item1993502154", "Painted Orange Duralumin")
        categorynode5215.Nodes.Add("item1993502154", "Painted Purple Duralumin")
        categorynode5215.Nodes.Add("item1993502154", "Painted Red Duralumin")
        categorynode5215.Nodes.Add("item1993502154", "Matte Duralumin")
        categorynode5215.Nodes.Add("item1993502154", "Painted Sky Duralumin")
        categorynode5215.Nodes.Add("item1993502154", "Painted Yellow Duralumin")
        categorynode5215.Nodes.Add("item1993502154", "Black Duralumin Panel")
        categorynode5215.Nodes.Add("item1993502154", "Dark Gray Duralumin Panel")
        categorynode5215.Nodes.Add("item1993502154", "Gray Duralumin Panel")
        categorynode5215.Nodes.Add("item1993502154", "Green Duralumin Panel")
        categorynode5215.Nodes.Add("item1993502154", "Ice Duralumin Panel")
        categorynode5215.Nodes.Add("item1993502154", "Light Gray Duralumin Panel")
        categorynode5215.Nodes.Add("item1993502154", "Military Duralumin Panel")
        categorynode5215.Nodes.Add("item1993502154", "Orange Duralumin Panel")
        categorynode5215.Nodes.Add("item1993502154", "Purple Duralumin Panel")
        categorynode5215.Nodes.Add("item1993502154", "Painted White Duralumin")
        categorynode5215.Nodes.Add("item1993502154", "Glossy Duralumin")
        categorynode5215.Nodes.Add("item1993502154", "Aged Duralumin")

        Dim categorynode5216 As TreeNode = categorynode521.Nodes.Add("node5216", "Grade 5 Titanium Alloy")
        categorynode5216.Nodes.Add("item483425306", "Painted Yellow Grade 5 Titanium Alloy")
        categorynode5216.Nodes.Add("item483425306", "Black Grade 5 Titanium Alloy Panel")
        categorynode5216.Nodes.Add("item483425306", "Dark Gray Grade 5 Titanium Alloy Panel")
        categorynode5216.Nodes.Add("item483425306", "Gray Grade 5 Titanium Alloy Panel")
        categorynode5216.Nodes.Add("item483425306", "Green Grade 5 Titanium Alloy Panel")
        categorynode5216.Nodes.Add("item483425306", "Painted White Grade 5 Titanium Alloy")
        categorynode5216.Nodes.Add("item483425306", "Painted Sky Grade 5 Titanium Alloy")
        categorynode5216.Nodes.Add("item483425306", "Painted Purple Grade 5 Titanium Alloy")
        categorynode5216.Nodes.Add("item483425306", "Ice Grade 5 Titanium Alloy Panel")
        categorynode5216.Nodes.Add("item483425306", "Light Gray Grade 5 Titanium Alloy Panel")
        categorynode5216.Nodes.Add("item483425306", "Military Grade 5 Titanium Alloy Panel")
        categorynode5216.Nodes.Add("item483425306", "Orange Grade 5 Titanium Alloy Panel")
        categorynode5216.Nodes.Add("item483425306", "Purple Grade 5 Titanium Alloy Panel")
        categorynode5216.Nodes.Add("item483425306", "Red Grade 5 Titanium Alloy Panel")
        categorynode5216.Nodes.Add("item483425306", "Sky Grade 5 Titanium Alloy Panel")
        categorynode5216.Nodes.Add("item483425306", "White Grade 5 Titanium Alloy Panel")
        categorynode5216.Nodes.Add("item483425306", "Painted Red Grade 5 Titanium Alloy")
        categorynode5216.Nodes.Add("item483425306", "Yellow Grade 5 Titanium Alloy Panel")
        categorynode5216.Nodes.Add("item483425306", "Glossy Grade 5 Titanium Alloy")
        categorynode5216.Nodes.Add("item483425306", "Matte Grade 5 Titanium Alloy")
        categorynode5216.Nodes.Add("item483425306", "Painted Black Grade 5 Titanium Alloy")
        categorynode5216.Nodes.Add("item483425306", "Painted Dark Gray Grade 5 Titanium Alloy")
        categorynode5216.Nodes.Add("item483425306", "Painted Gray Grade 5 Titanium Alloy")
        categorynode5216.Nodes.Add("item483425306", "Painted Green Grade 5 Titanium Alloy")
        categorynode5216.Nodes.Add("item483425306", "Painted Ice Grade 5 Titanium Alloy")
        categorynode5216.Nodes.Add("item483425306", "Painted Light Gray Grade 5 Titanium Alloy")
        categorynode5216.Nodes.Add("item483425306", "Painted Military Grade 5 Titanium Alloy")
        categorynode5216.Nodes.Add("item483425306", "Painted Orange Grade 5 Titanium Alloy")
        categorynode5216.Nodes.Add("item483425306", "Aged Grade 5 Titanium Alloy")

        Dim categorynode5217 As TreeNode = categorynode521.Nodes.Add("node5217", "Inconel")
        categorynode5217.Nodes.Add("item1972837708", "Aged Inconel")
        categorynode5217.Nodes.Add("item1972837708", "Glossy Inconel")
        categorynode5217.Nodes.Add("item1972837708", "Matte Inconel")
        categorynode5217.Nodes.Add("item1972837708", "Painted Black Inconel")
        categorynode5217.Nodes.Add("item1972837708", "Painted Dark Gray Inconel")
        categorynode5217.Nodes.Add("item1972837708", "Painted Gray Inconel")
        categorynode5217.Nodes.Add("item1972837708", "Painted Ice Inconel")
        categorynode5217.Nodes.Add("item1972837708", "Painted Light Gray Inconel")
        categorynode5217.Nodes.Add("item1972837708", "Painted Military Inconel")
        categorynode5217.Nodes.Add("item1972837708", "Painted Orange Inconel")
        categorynode5217.Nodes.Add("item1972837708", "Painted Purple Inconel")
        categorynode5217.Nodes.Add("item1972837708", "Painted Red Inconel")
        categorynode5217.Nodes.Add("item1972837708", "Painted Sky Inconel")
        categorynode5217.Nodes.Add("item1972837708", "Painted White Inconel")
        categorynode5217.Nodes.Add("item1972837708", "Painted Yellow Inconel")
        categorynode5217.Nodes.Add("item1972837708", "Black Inconel Panel")
        categorynode5217.Nodes.Add("item1972837708", "Painted Green Inconel")
        categorynode5217.Nodes.Add("item1972837708", "Dark Gray Inconel Panel ")
        categorynode5217.Nodes.Add("item1972837708", "Gray Inconel Panel")
        categorynode5217.Nodes.Add("item1972837708", "Ice Inconel Panel")
        categorynode5217.Nodes.Add("item1972837708", "Green Inconel Panel")
        categorynode5217.Nodes.Add("item1972837708", "Light Gray Inconel Panel")
        categorynode5217.Nodes.Add("item1972837708", "Military Inconel Panel")
        categorynode5217.Nodes.Add("item1972837708", "Orange Inconel Panel")
        categorynode5217.Nodes.Add("item1972837708", "Purple Inconel Panel")
        categorynode5217.Nodes.Add("item1972837708", "Red Inconel Panel")
        categorynode5217.Nodes.Add("item1972837708", "Sky Inconel Panel")
        categorynode5217.Nodes.Add("item1972837708", "White Inconel Panel")
        categorynode5217.Nodes.Add("item1972837708", "Yellow Inconel Panel")

        Dim categorynode5218 As TreeNode = categorynode521.Nodes.Add("node5218", "Luminescent Glass Honeycomb")
        categorynode5218.Nodes.Add("item1268122879", "Luminescent White Glass")

        Dim categorynode5219 As TreeNode = categorynode521.Nodes.Add("node5219", "Mangalloy")
        categorynode5219.Nodes.Add("item3573936284", "Gray Mangalloy Panel")
        categorynode5219.Nodes.Add("item3573936284", "Green Mangalloy Panel")
        categorynode5219.Nodes.Add("item3573936284", "Ice Mangalloy Panel")
        categorynode5219.Nodes.Add("item3573936284", "Light Gray Mangalloy Panel")
        categorynode5219.Nodes.Add("item3573936284", "Military Mangalloy Panel")
        categorynode5219.Nodes.Add("item3573936284", "Orange Mangalloy Panel")
        categorynode5219.Nodes.Add("item3573936284", "Purple Mangalloy Panel")
        categorynode5219.Nodes.Add("item3573936284", "Red Mangalloy Panel")
        categorynode5219.Nodes.Add("item3573936284", "Sky Mangalloy Panel")
        categorynode5219.Nodes.Add("item3573936284", "White Mangalloy Panel")
        categorynode5219.Nodes.Add("item3573936284", "Dark Gray Mangalloy Panel")
        categorynode5219.Nodes.Add("item3573936284", "Yellow Mangalloy Panel")
        categorynode5219.Nodes.Add("item3573936284", "Black Mangalloy Panel")
        categorynode5219.Nodes.Add("item3573936284", "Painted White Mangalloy")
        categorynode5219.Nodes.Add("item3573936284", "Aged Mangalloy")
        categorynode5219.Nodes.Add("item3573936284", "Glossy Mangalloy")
        categorynode5219.Nodes.Add("item3573936284", "Painted Yellow Mangalloy")
        categorynode5219.Nodes.Add("item3573936284", "Matte Mangalloy")
        categorynode5219.Nodes.Add("item3573936284", "Painted Dark Gray Mangalloy")
        categorynode5219.Nodes.Add("item3573936284", "Painted Gray Mangalloy")
        categorynode5219.Nodes.Add("item3573936284", "Painted Green Mangalloy")
        categorynode5219.Nodes.Add("item3573936284", "Painted Ice Mangalloy")
        categorynode5219.Nodes.Add("item3573936284", "Painted Light Gray Mangalloy")
        categorynode5219.Nodes.Add("item3573936284", "Painted Military Mangalloy")
        categorynode5219.Nodes.Add("item3573936284", "Painted Orange Mangalloy")
        categorynode5219.Nodes.Add("item3573936284", "Painted Purple Mangalloy")
        categorynode5219.Nodes.Add("item3573936284", "Painted Red Mangalloy")
        categorynode5219.Nodes.Add("item3573936284", "Painted Sky Mangalloy")
        categorynode5219.Nodes.Add("item3573936284", "Painted Black Mangalloy")

        Dim categorynode52110 As TreeNode = categorynode521.Nodes.Add("node52110", "Maraging Steel")
        categorynode52110.Nodes.Add("item2814304696", "Painted Sky Steel")
        categorynode52110.Nodes.Add("item734351314", "Painted Yellow Maraging Steel")
        categorynode52110.Nodes.Add("item734351314", "Black Maraging Steel Panel")
        categorynode52110.Nodes.Add("item734351314", "Dark Gray Maraging Steel Panel")
        categorynode52110.Nodes.Add("item734351314", "Gray Maraging Steel Panel")
        categorynode52110.Nodes.Add("item734351314", "Green Maraging Steel Panel")
        categorynode52110.Nodes.Add("item734351314", "Ice Maraging Steel Panel")
        categorynode52110.Nodes.Add("item734351314", "Light Gray Maraging Steel Panel")
        categorynode52110.Nodes.Add("item734351314", "Military Maraging Steel Panel")
        categorynode52110.Nodes.Add("item734351314", "Orange Maraging Steel Panel")
        categorynode52110.Nodes.Add("item734351314", "Purple Maraging Steel Panel")
        categorynode52110.Nodes.Add("item734351314", "Red Maraging Steel Panel")
        categorynode52110.Nodes.Add("item734351314", "White Maraging Steel Panel")
        categorynode52110.Nodes.Add("item734351314", "Yellow Maraging Steel Panel")
        categorynode52110.Nodes.Add("item734351314", "Sky Maraging Steel Panel")
        categorynode52110.Nodes.Add("item734351314", "Painted White Maraging Steel")
        categorynode52110.Nodes.Add("item734351314", "Painted Sky Maraging Steel")
        categorynode52110.Nodes.Add("item734351314", "Painted Red Maraging Steel")
        categorynode52110.Nodes.Add("item734351314", "Painted Purple Maraging Steel")
        categorynode52110.Nodes.Add("item734351314", "Glossy Maraging Steel")
        categorynode52110.Nodes.Add("item734351314", "Matte Maraging Steel")
        categorynode52110.Nodes.Add("item734351314", "Painted Black Maraging Steel")
        categorynode52110.Nodes.Add("item734351314", "Painted Dark Gray Maraging Steel")
        categorynode52110.Nodes.Add("item734351314", "Painted Gray Maraging Steel")
        categorynode52110.Nodes.Add("item734351314", "Painted Green Maraging Steel")
        categorynode52110.Nodes.Add("item734351314", "Painted Ice Maraging Steel")
        categorynode52110.Nodes.Add("item734351314", "Painted Light Gray Maraging Steel")
        categorynode52110.Nodes.Add("item734351314", "Painted Military Maraging Steel")
        categorynode52110.Nodes.Add("item734351314", "Painted Orange Maraging Steel")
        categorynode52110.Nodes.Add("item734351314", "Aged Maraging Steel")

        Dim categorynode52111 As TreeNode = categorynode521.Nodes.Add("node52111", "Marble")
        categorynode52111.Nodes.Add("item2003621933", "Stained yellow pattern marble (cold)")
        categorynode52111.Nodes.Add("item2003621933", "Aged yellow pattern marble (cold)")
        categorynode52111.Nodes.Add("item2003621933", "Yellow pattern marble (cold)")
        categorynode52111.Nodes.Add("item2003621933", "White pattern marble")
        categorynode52111.Nodes.Add("item2003621933", "Stained red pattern marble (cold)")
        categorynode52111.Nodes.Add("item2003621933", "Aged red pattern marble (cold)")
        categorynode52111.Nodes.Add("item2003621933", "Red pattern marble (cold)")
        categorynode52111.Nodes.Add("item2003621933", "Stained orange pattern marble (cold)")
        categorynode52111.Nodes.Add("item2003621933", "Aged orange pattern marble (cold)")
        categorynode52111.Nodes.Add("item2003621933", "Orange pattern marble (cold)")
        categorynode52111.Nodes.Add("item2003621933", "Stained green pattern marble (cold)")
        categorynode52111.Nodes.Add("item2003621933", "Aged green pattern marble (cold)")
        categorynode52111.Nodes.Add("item2003621933", "Green pattern marble (cold)")
        categorynode52111.Nodes.Add("item2003621933", "Stained gray pattern marble")
        categorynode52111.Nodes.Add("item2003621933", "Aged gray pattern marble")
        categorynode52111.Nodes.Add("item2003621933", "Gray pattern marble")
        categorynode52111.Nodes.Add("item2003621933", "Stained blue pattern marble(cold)")
        categorynode52111.Nodes.Add("item2003621933", "Aged blue pattern marble (cold)")
        categorynode52111.Nodes.Add("item2003621933", "Blue pattern marble (cold)")
        categorynode52111.Nodes.Add("item2003621933", "Black pattern marble")
        categorynode52111.Nodes.Add("item2003621933", "Stained beige pattern marble (cold)")
        categorynode52111.Nodes.Add("item2003621933", "Aged beige pattern marble (cold)")
        categorynode52111.Nodes.Add("item2003621933", "Beige pattern marble (cold)")
        categorynode52111.Nodes.Add("item2003621933", "Matte light yellow marble (cold)")
        categorynode52111.Nodes.Add("item2003621933", "Matte dark yellow marble (cold)")
        categorynode52111.Nodes.Add("item2003621933", "Matte yellow marble (cold)")
        categorynode52111.Nodes.Add("item2003621933", "Matte white marble")
        categorynode52111.Nodes.Add("item2003621933", "Matte light red marble (cold)")
        categorynode52111.Nodes.Add("item2003621933", "Matte dark red marble (cold)")
        categorynode52111.Nodes.Add("item2003621933", "Matte red marble (cold)")
        categorynode52111.Nodes.Add("item2003621933", "Matte light orange marble (cold)")
        categorynode52111.Nodes.Add("item2003621933", "Matte dark orange marble (cold)")
        categorynode52111.Nodes.Add("item2003621933", "Matte orange marble (cold)")
        categorynode52111.Nodes.Add("item2003621933", "Matte light green marble (cold)")
        categorynode52111.Nodes.Add("item2003621933", "Matte dark green marble (cold)")
        categorynode52111.Nodes.Add("item2003621933", "Matte green marble (cold)")
        categorynode52111.Nodes.Add("item2003621933", "Matte light gray marble")
        categorynode52111.Nodes.Add("item2003621933", "Matte dark gray marble")
        categorynode52111.Nodes.Add("item2003621933", "Matte gray marble")
        categorynode52111.Nodes.Add("item2003621933", "Matte light blue marble (cold)")
        categorynode52111.Nodes.Add("item2003621933", "Matte dark blue marble (cold)")
        categorynode52111.Nodes.Add("item2003621933", "Matte blue marble (cold)")
        categorynode52111.Nodes.Add("item2003621933", "Matte black marble")
        categorynode52111.Nodes.Add("item2003621933", "Matte light beige marble (cold)")
        categorynode52111.Nodes.Add("item2003621933", "Matte dark beige marble (cold)")
        categorynode52111.Nodes.Add("item2003621933", "Matte beige marble (cold)")
        categorynode52111.Nodes.Add("item2003621933", "Polished light yellow marble (cold)")
        categorynode52111.Nodes.Add("item2003621933", "Polished dark yellow marble (cold)")
        categorynode52111.Nodes.Add("item2003621933", "Polished yellow marble (cold)")
        categorynode52111.Nodes.Add("item2003621933", "Polished white marble")
        categorynode52111.Nodes.Add("item2003621933", "Polished light red marble (cold)")
        categorynode52111.Nodes.Add("item2003621933", "Polished dark red marble (cold)")
        categorynode52111.Nodes.Add("item2003621933", "Polished red marble (cold)")
        categorynode52111.Nodes.Add("item2003621933", "Polished orange marble (cold)")
        categorynode52111.Nodes.Add("item2003621933", "Polished light green marble (cold)")
        categorynode52111.Nodes.Add("item2003621933", "Polished dark green marble (cold)")
        categorynode52111.Nodes.Add("item2003621933", "Polished green marble (cold)")
        categorynode52111.Nodes.Add("item2003621933", "Polished light gray marble")
        categorynode52111.Nodes.Add("item2003621933", "Polished dark gray marble")
        categorynode52111.Nodes.Add("item2003621933", "Polished gray marble")
        categorynode52111.Nodes.Add("item2003621933", "Polished light blue marble (cold)")
        categorynode52111.Nodes.Add("item2003621933", "Polished dark blue marble (cold)")
        categorynode52111.Nodes.Add("item2003621933", "Polished blue marble (cold)")
        categorynode52111.Nodes.Add("item2003621933", "Polished black marble")
        categorynode52111.Nodes.Add("item2003621933", "Polished light beige marble (cold)")
        categorynode52111.Nodes.Add("item2003621933", "Polished dark beige marble (cold)")
        categorynode52111.Nodes.Add("item2003621933", "Polished beige marble (cold)")
        categorynode52111.Nodes.Add("item2003621933", "Polished light orange marble (cold)")
        categorynode52111.Nodes.Add("item2003621933", "Polished dark orange marble (cold)")

        Dim categorynode52112 As TreeNode = categorynode521.Nodes.Add("node52112", "Plastic")
        categorynode52112.Nodes.Add("item1269767483", "Stained yellow pattern plastic")
        categorynode52112.Nodes.Add("item1269767483", "Aged yellow pattern plastic")
        categorynode52112.Nodes.Add("item1269767483", "Stained yellow pattern plastic (cold)")
        categorynode52112.Nodes.Add("item1269767483", "Aged yellow pattern plastic (cold)")
        categorynode52112.Nodes.Add("item1269767483", "Yellow pattern plastic (cold)")
        categorynode52112.Nodes.Add("item1269767483", "Yellow pattern plastic")
        categorynode52112.Nodes.Add("item1269767483", "White pattern plastic")
        categorynode52112.Nodes.Add("item1269767483", "Stained red pattern plastic")
        categorynode52112.Nodes.Add("item1269767483", "Aged red pattern plastic")
        categorynode52112.Nodes.Add("item1269767483", "Stained red pattern plastic (cold)")
        categorynode52112.Nodes.Add("item1269767483", "Aged red pattern plastic (cold)")
        categorynode52112.Nodes.Add("item1269767483", "Red pattern plastic (cold)")
        categorynode52112.Nodes.Add("item1269767483", "Red pattern plastic")
        categorynode52112.Nodes.Add("item1269767483", "Stained orange pattern plastic")
        categorynode52112.Nodes.Add("item1269767483", "Aged orange pattern plastic")
        categorynode52112.Nodes.Add("item1269767483", "Stained orange pattern plastic (cold)")
        categorynode52112.Nodes.Add("item1269767483", "Aged orange pattern plastic (cold)")
        categorynode52112.Nodes.Add("item1269767483", "Orange pattern plastic (cold)")
        categorynode52112.Nodes.Add("item1269767483", "Orange pattern plastic")
        categorynode52112.Nodes.Add("item1269767483", "Stained green pattern plastic")
        categorynode52112.Nodes.Add("item1269767483", "Aged green pattern plastic")
        categorynode52112.Nodes.Add("item1269767483", "Stained green pattern plastic (cold)")
        categorynode52112.Nodes.Add("item1269767483", "Aged green pattern plastic (cold)")
        categorynode52112.Nodes.Add("item1269767483", "Green pattern plastic (cold)")
        categorynode52112.Nodes.Add("item1269767483", "Green pattern plastic")
        categorynode52112.Nodes.Add("item1269767483", "Stained gray pattern plastic")
        categorynode52112.Nodes.Add("item1269767483", "Aged gray pattern plastic")
        categorynode52112.Nodes.Add("item1269767483", "Gray pattern plastic")
        categorynode52112.Nodes.Add("item1269767483", "Stained blue pattern plastic")
        categorynode52112.Nodes.Add("item1269767483", "Aged blue pattern plastic")
        categorynode52112.Nodes.Add("item1269767483", "Stained blue pattern plastic (cold)")
        categorynode52112.Nodes.Add("item1269767483", "Aged blue pattern plastic (cold)")
        categorynode52112.Nodes.Add("item1269767483", "Blue pattern plastic (cold)")
        categorynode52112.Nodes.Add("item1269767483", "Blue pattern plastic")
        categorynode52112.Nodes.Add("item1269767483", "Black pattern plastic")
        categorynode52112.Nodes.Add("item1269767483", "Stained beige pattern plastic")
        categorynode52112.Nodes.Add("item1269767483", "Aged beige pattern plastic")
        categorynode52112.Nodes.Add("item1269767483", "Stained beige pattern plastic (cold)")
        categorynode52112.Nodes.Add("item1269767483", "Aged beige pattern plastic (cold)")
        categorynode52112.Nodes.Add("item1269767483", "Beige pattern plastic(cold)")
        categorynode52112.Nodes.Add("item1269767483", "Beige pattern plastic")
        categorynode52112.Nodes.Add("item1269767483", "Matte light yellow plastic")
        categorynode52112.Nodes.Add("item1269767483", "Matte dark yellow plastic")
        categorynode52112.Nodes.Add("item1269767483", "Matte light yellow plastic (cold)")
        categorynode52112.Nodes.Add("item1269767483", "Matte dark yellow plastic (cold)")
        categorynode52112.Nodes.Add("item1269767483", "Matte yellow plastic (cold)")
        categorynode52112.Nodes.Add("item1269767483", "Matte yellow plastic")
        categorynode52112.Nodes.Add("item1269767483", "Matte white plastic")
        categorynode52112.Nodes.Add("item1269767483", "Matte light red plastic")
        categorynode52112.Nodes.Add("item1269767483", "Matte dark red plastic")
        categorynode52112.Nodes.Add("item1269767483", "Matte light red plastic (cold)")
        categorynode52112.Nodes.Add("item1269767483", "Matte dark red plastic (cold)")
        categorynode52112.Nodes.Add("item1269767483", "Matte red plastic (cold)")
        categorynode52112.Nodes.Add("item1269767483", "Matte red plastic")
        categorynode52112.Nodes.Add("item1269767483", "Matte light orange plastic")
        categorynode52112.Nodes.Add("item1269767483", "Matte dark orange plastic")
        categorynode52112.Nodes.Add("item1269767483", "Matte light orange plastic (cold)")
        categorynode52112.Nodes.Add("item1269767483", "Matte dark orange plastic (cold)")
        categorynode52112.Nodes.Add("item1269767483", "Matte orange plastic (cold)")
        categorynode52112.Nodes.Add("item1269767483", "Matte orange plastic")
        categorynode52112.Nodes.Add("item1269767483", "Matte light green plastic")
        categorynode52112.Nodes.Add("item1269767483", "Matte dark green plastic")
        categorynode52112.Nodes.Add("item1269767483", "Matte light green plastic (cold)")
        categorynode52112.Nodes.Add("item1269767483", "Matte dark green plastic (cold)")
        categorynode52112.Nodes.Add("item1269767483", "Matte green plastic (cold)")
        categorynode52112.Nodes.Add("item1269767483", "Matte green plastic")
        categorynode52112.Nodes.Add("item1269767483", "Matte light gray plastic")
        categorynode52112.Nodes.Add("item1269767483", "Matte dark gray plastic")
        categorynode52112.Nodes.Add("item1269767483", "Matte gray plastic")
        categorynode52112.Nodes.Add("item1269767483", "Matte light blue plastic")
        categorynode52112.Nodes.Add("item1269767483", "Matte dark blue plastic")
        categorynode52112.Nodes.Add("item1269767483", "Matte light blue plastic (cold)")
        categorynode52112.Nodes.Add("item1269767483", "Matte dark blue plastic (cold)")
        categorynode52112.Nodes.Add("item1269767483", "Matte blue (cold) plastic")
        categorynode52112.Nodes.Add("item1269767483", "Matte blue plastic")
        categorynode52112.Nodes.Add("item1269767483", "Matte black plastic")
        categorynode52112.Nodes.Add("item1269767483", "Matte light beige plastic")
        categorynode52112.Nodes.Add("item1269767483", "Matte dark beige plastic")
        categorynode52112.Nodes.Add("item1269767483", "Matte light beige plastic(cold)")
        categorynode52112.Nodes.Add("item1269767483", "Matte dark beige plastic (cold)")
        categorynode52112.Nodes.Add("item1269767483", "Matte beige plastic (cold)")
        categorynode52112.Nodes.Add("item1269767483", "Matte beige plastic")
        categorynode52112.Nodes.Add("item1269767483", "Glossy light yellow plastic")
        categorynode52112.Nodes.Add("item1269767483", "Glossy dark yellow plastic")
        categorynode52112.Nodes.Add("item1269767483", "Glossy light yellow plastic (cold)")
        categorynode52112.Nodes.Add("item1269767483", "Glossy dark yellow plastic (cold)")
        categorynode52112.Nodes.Add("item1269767483", "Glossy yellow plastic")
        categorynode52112.Nodes.Add("item1269767483", "Glossy yellow plastic")
        categorynode52112.Nodes.Add("item1269767483", "Glossy light red plastic")
        categorynode52112.Nodes.Add("item1269767483", "Glossy dark red plastic")
        categorynode52112.Nodes.Add("item1269767483", "Glossy light red plastic (cold)")
        categorynode52112.Nodes.Add("item1269767483", "Glossy dark red plastic (cold)")
        categorynode52112.Nodes.Add("item1269767483", "Glossy red plastic (cold)")
        categorynode52112.Nodes.Add("item1269767483", "Glossy red plastic")
        categorynode52112.Nodes.Add("item1269767483", "Glossy light orange plastic")
        categorynode52112.Nodes.Add("item1269767483", "Glossy dark orange plastic")
        categorynode52112.Nodes.Add("item1269767483", "Glossy light orange plastic (cold)")
        categorynode52112.Nodes.Add("item1269767483", "Glossy dark orange plastic (cold)")
        categorynode52112.Nodes.Add("item1269767483", "Glossy orange plastic (cold)")
        categorynode52112.Nodes.Add("item1269767483", "Glossy orange plastic")
        categorynode52112.Nodes.Add("item1269767483", "Glossy light green plastic")
        categorynode52112.Nodes.Add("item1269767483", "Glossy dark green plastic")
        categorynode52112.Nodes.Add("item1269767483", "Glossy light green plastic (cold)")
        categorynode52112.Nodes.Add("item1269767483", "Glossy dark green plastic (cold)")
        categorynode52112.Nodes.Add("item1269767483", "Glossy green plastic (cold)")
        categorynode52112.Nodes.Add("item1269767483", "Glossy green plastic")
        categorynode52112.Nodes.Add("item1269767483", "Glossy light gray plastic")
        categorynode52112.Nodes.Add("item1269767483", "Glossy dark gray plastic")
        categorynode52112.Nodes.Add("item1269767483", "Glossy gray plastic")
        categorynode52112.Nodes.Add("item1269767483", "Glossy light blue plastic")
        categorynode52112.Nodes.Add("item1269767483", "Glossy dark blue plastic")
        categorynode52112.Nodes.Add("item1269767483", "Glossy light blue plastic (cold)")
        categorynode52112.Nodes.Add("item1269767483", "Glossy dark blue plastic (cold)")
        categorynode52112.Nodes.Add("item1269767483", "Glossy blue plastic (cold)")
        categorynode52112.Nodes.Add("item1269767483", "Glossy blue plastic")
        categorynode52112.Nodes.Add("item1269767483", "Glossy black plastic")
        categorynode52112.Nodes.Add("item1269767483", "Glossy light beige plastic")
        categorynode52112.Nodes.Add("item1269767483", "Glossy dark beige plastic")
        categorynode52112.Nodes.Add("item1269767483", "Glossy light beige plastic (cold)")
        categorynode52112.Nodes.Add("item1269767483", "Glossy white plastic")
        categorynode52112.Nodes.Add("item1269767483", "Glossy dark beige plastic (cold)")
        categorynode52112.Nodes.Add("item1269767483", "Glossy beige plastic (cold)")
        categorynode52112.Nodes.Add("item1269767483", "Glossy beige plastic")

        Dim categorynode52113 As TreeNode = categorynode521.Nodes.Add("node52113", "Sc-Al")
        categorynode52113.Nodes.Add("item1160705623", "Red Sc-Al Panel")
        categorynode52113.Nodes.Add("item1160705623", "Sky Sc-Al Panel")
        categorynode52113.Nodes.Add("item1160705623", "White Sc-Al Panel")
        categorynode52113.Nodes.Add("item1160705623", "Yellow Sc-Al Panel")
        categorynode52113.Nodes.Add("item1160705623", "Purple Sc-Al Panel")
        categorynode52113.Nodes.Add("item1160705623", "Orange Sc-Al Panel")
        categorynode52113.Nodes.Add("item1160705623", "Light Gray Sc-Al Panel")
        categorynode52113.Nodes.Add("item1160705623", "Aged Sc-Al")
        categorynode52113.Nodes.Add("item1160705623", "Glossy Sc-Al")
        categorynode52113.Nodes.Add("item1160705623", "Matte Sc-Al")
        categorynode52113.Nodes.Add("item1160705623", "Painted Black Sc-Al")
        categorynode52113.Nodes.Add("item1160705623", "Painted Dark Gray Sc-Al")
        categorynode52113.Nodes.Add("item1160705623", "Painted Gray Sc-Al")
        categorynode52113.Nodes.Add("item1160705623", "Painted Green Sc-Al")
        categorynode52113.Nodes.Add("item1160705623", "Painted Ice Sc-Al")
        categorynode52113.Nodes.Add("item1160705623", "Painted Light Gray Sc-Al")
        categorynode52113.Nodes.Add("item1160705623", "Military Sc-Al Panel")
        categorynode52113.Nodes.Add("item1160705623", "Painted Military Sc-Al")
        categorynode52113.Nodes.Add("item1160705623", "Painted Purple Sc-Al")
        categorynode52113.Nodes.Add("item1160705623", "Painted Red Sc-Al")
        categorynode52113.Nodes.Add("item1160705623", "Painted Sky Sc-Al")
        categorynode52113.Nodes.Add("item1160705623", "Painted White Sc-Al")
        categorynode52113.Nodes.Add("item1160705623", "Painted Yellow Sc-Al")
        categorynode52113.Nodes.Add("item1160705623", "Black Sc-Al Panel")
        categorynode52113.Nodes.Add("item1160705623", "Dark Gray Sc-Al Panel")
        categorynode52113.Nodes.Add("item1160705623", "Gray Sc-Al Panel")
        categorynode52113.Nodes.Add("item1160705623", "Green Sc-Al Panel")
        categorynode52113.Nodes.Add("item1160705623", "Ice Sc-Al Panel")
        categorynode52113.Nodes.Add("item1160705623", "Painted Orange Sc-Al")

        Dim categorynode52114 As TreeNode = categorynode521.Nodes.Add("node52114", "Silumin")
        categorynode52114.Nodes.Add("item3134890135", "Aged Silumin")
        categorynode52114.Nodes.Add("item3134890135", "Glossy Silumin")
        categorynode52114.Nodes.Add("item3134890135", "Matte Silumin")
        categorynode52114.Nodes.Add("item3134890135", "Painted Black Silumin")
        categorynode52114.Nodes.Add("item3134890135", "Painted Dark Gray Silumin")
        categorynode52114.Nodes.Add("item3134890135", "Painted Gray Silumin")
        categorynode52114.Nodes.Add("item3134890135", "Painted Green Silumin")
        categorynode52114.Nodes.Add("item3134890135", "Painted Light Gray Silumin")
        categorynode52114.Nodes.Add("item3134890135", "Painted Military Silumin")
        categorynode52114.Nodes.Add("item3134890135", "Painted Orange Silumin")
        categorynode52114.Nodes.Add("item3134890135", "Painted Purple Silumin")
        categorynode52114.Nodes.Add("item3134890135", "Painted Red Silumin")
        categorynode52114.Nodes.Add("item3134890135", "Painted Sky Silumin")
        categorynode52114.Nodes.Add("item3134890135", "Painted White Silumin")
        categorynode52114.Nodes.Add("item3134890135", "Painted Yellow Silumin")
        categorynode52114.Nodes.Add("item3134890135", "Black Silumin Panel")
        categorynode52114.Nodes.Add("item3134890135", "Dark Gray Silumin Panel")
        categorynode52114.Nodes.Add("item3134890135", "Painted Ice Silumin")
        categorynode52114.Nodes.Add("item3134890135", "Gray Silumin Panel")
        categorynode52114.Nodes.Add("item3134890135", "Green Silumin Panel")
        categorynode52114.Nodes.Add("item3134890135", "Light Gray Silumin Panel")
        categorynode52114.Nodes.Add("item3134890135", "Ice Silumin Panel")
        categorynode52114.Nodes.Add("item3134890135", "Military Silumin Panel")
        categorynode52114.Nodes.Add("item3134890135", "Orange Silumin Panel")
        categorynode52114.Nodes.Add("item3134890135", "Purple Silumin Panel")
        categorynode52114.Nodes.Add("item3134890135", "Red Silumin Panel")
        categorynode52114.Nodes.Add("item3134890135", "Sky Silumin Panel")
        categorynode52114.Nodes.Add("item3134890135", "White Silumin Panel")
        categorynode52114.Nodes.Add("item3134890135", "Yellow Silumin Panel")

        Dim categorynode52115 As TreeNode = categorynode521.Nodes.Add("node52115", "Stainless Steel")
        categorynode52115.Nodes.Add("item3200326100", "Green Stainless Steel Panel")
        categorynode52115.Nodes.Add("item3200326100", "Ice Stainless Steel Panel")
        categorynode52115.Nodes.Add("item3200326100", "Light Gray Stainless Steel Panel")
        categorynode52115.Nodes.Add("item3200326100", "Military Stainless Steel Panel")
        categorynode52115.Nodes.Add("item3200326100", "Orange Stainless Steel Panel")
        categorynode52115.Nodes.Add("item3200326100", "Purple Stainless Steel Panel")
        categorynode52115.Nodes.Add("item3200326100", "Red Stainless Steel Panel")
        categorynode52115.Nodes.Add("item3200326100", "Sky Stainless Steel Panel")
        categorynode52115.Nodes.Add("item3200326100", "White Stainless Steel Panel")
        categorynode52115.Nodes.Add("item3200326100", "Yellow Stainless Steel Panel")
        categorynode52115.Nodes.Add("item3200326100", "Gray Stainless Steel Panel")
        categorynode52115.Nodes.Add("item3200326100", "Dark Gray Stainless Steel Panel")
        categorynode52115.Nodes.Add("item3200326100", "Painted Yellow Stainless Steel")
        categorynode52115.Nodes.Add("item3200326100", "Aged Stainless Steel")
        categorynode52115.Nodes.Add("item3200326100", "Glossy Stainless Steel")
        categorynode52115.Nodes.Add("item3200326100", "Matte Stainless Steel")
        categorynode52115.Nodes.Add("item3200326100", "Black Stainless Steel Panel")
        categorynode52115.Nodes.Add("item3200326100", "Painted Black Stainless Steel")
        categorynode52115.Nodes.Add("item3200326100", "Painted Gray Stainless Steel")
        categorynode52115.Nodes.Add("item3200326100", "Painted Green Stainless Steel")
        categorynode52115.Nodes.Add("item3200326100", "Painted Ice Stainless Steel")
        categorynode52115.Nodes.Add("item3200326100", "Painted Light Gray Stainless Steel")
        categorynode52115.Nodes.Add("item3200326100", "Painted Military Stainless Steel")
        categorynode52115.Nodes.Add("item3200326100", "Painted Orange Stainless Steel")
        categorynode52115.Nodes.Add("item3200326100", "Painted Purple Stainless Steel")
        categorynode52115.Nodes.Add("item3200326100", "Painted Red Stainless Steel")
        categorynode52115.Nodes.Add("item3200326100", "Painted Sky Stainless Steel")
        categorynode52115.Nodes.Add("item3200326100", "Painted White Stainless Steel")
        categorynode52115.Nodes.Add("item3200326100", "Painted Dark Gray Stainless Steel")

        Dim categorynode52116 As TreeNode = categorynode521.Nodes.Add("node52116", "Steel")
        categorynode52116.Nodes.Add("item2814304696", "Black Steel Panel")
        categorynode52116.Nodes.Add("item2814304696", "Dark Gray Steel Panel")
        categorynode52116.Nodes.Add("item2814304696", "Gray Steel Panel")
        categorynode52116.Nodes.Add("item2814304696", "Green Steel Panel")
        categorynode52116.Nodes.Add("item2814304696", "Ice Steel Panel")
        categorynode52116.Nodes.Add("item2814304696", "Light Gray Steel Panel")
        categorynode52116.Nodes.Add("item2814304696", "Military Steel Panel")
        categorynode52116.Nodes.Add("item2814304696", "Orange Steel Panel")
        categorynode52116.Nodes.Add("item2814304696", "Purple Steel Panel")
        categorynode52116.Nodes.Add("item2814304696", "Red Steel Panel")
        categorynode52116.Nodes.Add("item2814304696", "Sky Steel Panel")
        categorynode52116.Nodes.Add("item2814304696", "Yellow Steel Panel")
        categorynode52116.Nodes.Add("item2814304696", "White Steel Panel")
        categorynode52116.Nodes.Add("item2814304696", "Painted Yellow Steel")
        categorynode52116.Nodes.Add("item2814304696", "Painted White Steel")
        categorynode52116.Nodes.Add("item2814304696", "Painted Red Steel")
        categorynode52116.Nodes.Add("item2814304696", "Painted Ice Steel")
        categorynode52116.Nodes.Add("item2814304696", "Glossy Steel")
        categorynode52116.Nodes.Add("item2814304696", "Matte Steel")
        categorynode52116.Nodes.Add("item2814304696", "Painted Black Steel")
        categorynode52116.Nodes.Add("item2814304696", "Painted Dark Gray Steel")
        categorynode52116.Nodes.Add("item2814304696", "Painted Gray Steel")
        categorynode52116.Nodes.Add("item2814304696", "Painted Green Steel")
        categorynode52116.Nodes.Add("item2814304696", "Painted Light Gray Steel")
        categorynode52116.Nodes.Add("item2814304696", "Painted Military Steel")
        categorynode52116.Nodes.Add("item2814304696", "Painted Orange Steel")
        categorynode52116.Nodes.Add("item2814304696", "Painted Purple Steel")
        categorynode52116.Nodes.Add("item2814304696", "Aged Steel")
        categorynode52116.Nodes.Add("item2814304696", "Stained yellow pattern steel")
        categorynode52116.Nodes.Add("item2814304696", "Aged yellow pattern steel")
        categorynode52116.Nodes.Add("item2814304696", "Blue pattern steel")
        categorynode52116.Nodes.Add("item2814304696", "Black pattern steel")
        categorynode52116.Nodes.Add("item2814304696", "Stained beige pattern steel")
        categorynode52116.Nodes.Add("item2814304696", "Aged beige pattern steel")
        categorynode52116.Nodes.Add("item2814304696", "Beige pattern steel")
        categorynode52116.Nodes.Add("item2814304696", "Galvanized light yellow steel")
        categorynode52116.Nodes.Add("item2814304696", "Galvanized dark yellow steel")
        categorynode52116.Nodes.Add("item2814304696", "Galvanized yellow steel")
        categorynode52116.Nodes.Add("item2814304696", "Galvanized white steel")
        categorynode52116.Nodes.Add("item2814304696", "Galvanized light red steel")
        categorynode52116.Nodes.Add("item2814304696", "Galvanized red steel (cold)")
        categorynode52116.Nodes.Add("item2814304696", "Galvanized red steel")
        categorynode52116.Nodes.Add("item2814304696", "Galvanized light orange steel")
        categorynode52116.Nodes.Add("item2814304696", "Galvanized dark orange steel")
        categorynode52116.Nodes.Add("item2814304696", "Galvanized orange steel")
        categorynode52116.Nodes.Add("item2814304696", "Galvanized light green steel")
        categorynode52116.Nodes.Add("item2814304696", "Galvanized dark green steel")
        categorynode52116.Nodes.Add("item2814304696", "Galvanized green steel")
        categorynode52116.Nodes.Add("item2814304696", "Galvanized light gray steel")
        categorynode52116.Nodes.Add("item2814304696", "Galvanized dark gray steel")
        categorynode52116.Nodes.Add("item2814304696", "Galvanized gray steel")
        categorynode52116.Nodes.Add("item2814304696", "Galvanized light blue steel")
        categorynode52116.Nodes.Add("item2814304696", "Galvanized dark blue steel")
        categorynode52116.Nodes.Add("item2814304696", "Galvanized blue steel")
        categorynode52116.Nodes.Add("item2814304696", "Galvanized black steel")
        categorynode52116.Nodes.Add("item2814304696", "Galvanized light beige steel")
        categorynode52116.Nodes.Add("item2814304696", "Galvanized dark beige steel")
        categorynode52116.Nodes.Add("item2814304696", "Galvanized beige steel")
        categorynode52116.Nodes.Add("item2814304696", "Polished light yellow steel")
        categorynode52116.Nodes.Add("item2814304696", "Polished dark yellow steel")
        categorynode52116.Nodes.Add("item2814304696", "Polished dark red steel")
        categorynode52116.Nodes.Add("item2814304696", "Polished red steel")
        categorynode52116.Nodes.Add("item2814304696", "Polished light orange steel")
        categorynode52116.Nodes.Add("item2814304696", "Polished dark orange steel")
        categorynode52116.Nodes.Add("item2814304696", "Polished orange steel")
        categorynode52116.Nodes.Add("item2814304696", "Polished light green steel")
        categorynode52116.Nodes.Add("item2814304696", "Polished dark green steel")
        categorynode52116.Nodes.Add("item2814304696", "Polished green steel")
        categorynode52116.Nodes.Add("item2814304696", "Polished light gray steel")
        categorynode52116.Nodes.Add("item2814304696", "Polished dark gray steel")
        categorynode52116.Nodes.Add("item2814304696", "Polished gray steel")
        categorynode52116.Nodes.Add("item2814304696", "Polished light blue steel")
        categorynode52116.Nodes.Add("item2814304696", "Polished dark blue steel")
        categorynode52116.Nodes.Add("item2814304696", "Polished black steel")
        categorynode52116.Nodes.Add("item2814304696", "Polished light beige steel")
        categorynode52116.Nodes.Add("item2814304696", "Polished dark beige steel")
        categorynode52116.Nodes.Add("item2814304696", "Polished beige steel")
        categorynode52116.Nodes.Add("item2814304696", "Polished yellow steel")
        categorynode52116.Nodes.Add("item2814304696", "Polished white steel")
        categorynode52116.Nodes.Add("item2814304696", "Galvanized dark red steel")
        categorynode52116.Nodes.Add("item2814304696", "Polished blue steel")
        categorynode52116.Nodes.Add("item2814304696", "Yellow pattern steel")
        categorynode52116.Nodes.Add("item2814304696", "White pattern steel")
        categorynode52116.Nodes.Add("item2814304696", "Stained red pattern steel")
        categorynode52116.Nodes.Add("item2814304696", "Red pattern steel")
        categorynode52116.Nodes.Add("item2814304696", "Stained orange pattern steel")
        categorynode52116.Nodes.Add("item2814304696", "Aged orange pattern steel")
        categorynode52116.Nodes.Add("item2814304696", "Orange pattern steel")
        categorynode52116.Nodes.Add("item2814304696", "Stained green pattern steel")
        categorynode52116.Nodes.Add("item2814304696", "Aged green pattern steel")
        categorynode52116.Nodes.Add("item2814304696", "Green pattern steel")
        categorynode52116.Nodes.Add("item2814304696", "Stained gray pattern steel")
        categorynode52116.Nodes.Add("item2814304696", "Aged gray pattern steel")
        categorynode52116.Nodes.Add("item2814304696", "Gray pattern steel")
        categorynode52116.Nodes.Add("item2814304696", "Stained blue pattern steel")
        categorynode52116.Nodes.Add("item2814304696", "Aged blue pattern steel")
        categorynode52116.Nodes.Add("item2814304696", "Aged red pattern steel")
        categorynode52116.Nodes.Add("item2814304696", "Polished light red steel")

        Dim categorynode52117 As TreeNode = categorynode521.Nodes.Add("node52117", "Wood")
        categorynode52117.Nodes.Add("item2497146600", "White pattern wood")
        categorynode52117.Nodes.Add("item2497146600", "Stained gray pattern wood")
        categorynode52117.Nodes.Add("item2497146600", "Aged gray pattern wood")
        categorynode52117.Nodes.Add("item2497146600", "Gray pattern wood")
        categorynode52117.Nodes.Add("item2497146600", "Stained brown pattern wood 4")
        categorynode52117.Nodes.Add("item2497146600", "Aged brown pattern wood 4")
        categorynode52117.Nodes.Add("item2497146600", "Brown pattern wood 4")
        categorynode52117.Nodes.Add("item2497146600", "Stained brown pattern wood 3")
        categorynode52117.Nodes.Add("item2497146600", "Aged brown pattern wood 3")
        categorynode52117.Nodes.Add("item2497146600", "Brown pattern wood 3")
        categorynode52117.Nodes.Add("item2497146600", "Stained brown pattern wood 2")
        categorynode52117.Nodes.Add("item2497146600", "Aged brown pattern wood 2")
        categorynode52117.Nodes.Add("item2497146600", "Brown pattern wood 2")
        categorynode52117.Nodes.Add("item2497146600", "Stained brown pattern wood 1")
        categorynode52117.Nodes.Add("item2497146600", "Aged brown pattern wood 1")
        categorynode52117.Nodes.Add("item2497146600", "Brown pattern wood 1")
        categorynode52117.Nodes.Add("item2497146600", "Black pattern wood")
        categorynode52117.Nodes.Add("item2497146600", "Matte light gray wood")
        categorynode52117.Nodes.Add("item2497146600", "Matte dark gray wood")
        categorynode52117.Nodes.Add("item2497146600", "Matte gray wood")
        categorynode52117.Nodes.Add("item2497146600", "Matte light brown wood 4")
        categorynode52117.Nodes.Add("item2497146600", "Matte dark brown wood 4")
        categorynode52117.Nodes.Add("item2497146600", "Matte brown wood 4")
        categorynode52117.Nodes.Add("item2497146600", "Matte light brown wood 3")
        categorynode52117.Nodes.Add("item2497146600", "Matte dark brown wood 3")
        categorynode52117.Nodes.Add("item2497146600", "Matte brown wood 3")
        categorynode52117.Nodes.Add("item2497146600", "Matte light brown wood 2")
        categorynode52117.Nodes.Add("item2497146600", "Matte dark brown wood 2")
        categorynode52117.Nodes.Add("item2497146600", "Matte brown wood 2")
        categorynode52117.Nodes.Add("item2497146600", "Matte light brown wood 1")
        categorynode52117.Nodes.Add("item2497146600", "Matte dark brown wood 1")
        categorynode52117.Nodes.Add("item2497146600", "Matte brown wood 1")
        categorynode52117.Nodes.Add("item2497146600", "Matte black wood")
        categorynode52117.Nodes.Add("item2497146600", "Polished white wood")
        categorynode52117.Nodes.Add("item2497146600", "Polished light gray wood")
        categorynode52117.Nodes.Add("item2497146600", "Polished dark gray wood")
        categorynode52117.Nodes.Add("item2497146600", "Polished gray wood")
        categorynode52117.Nodes.Add("item2497146600", "Polished light brown wood 4")
        categorynode52117.Nodes.Add("item2497146600", "Polished dark brown wood 4")
        categorynode52117.Nodes.Add("item2497146600", "Polished brown wood 4")
        categorynode52117.Nodes.Add("item2497146600", "Polished light brown wood 3")
        categorynode52117.Nodes.Add("item2497146600", "Polished dark brown wood 3")
        categorynode52117.Nodes.Add("item2497146600", "Polished brown wood 3")
        categorynode52117.Nodes.Add("item2497146600", "Polished light brown wood 2")
        categorynode52117.Nodes.Add("item2497146600", "Polished dark brown wood 2")
        categorynode52117.Nodes.Add("item2497146600", "Polished brown wood 2")
        categorynode52117.Nodes.Add("item2497146600", "Polished light brown wood 1")
        categorynode52117.Nodes.Add("item2497146600", "Polished dark brown wood 1")
        categorynode52117.Nodes.Add("item2497146600", "Polished brown wood 1")
        categorynode52117.Nodes.Add("item2497146600", "Polished black wood")
        categorynode52117.Nodes.Add("item2497146600", "Matte white wood")

        Dim categorynode522 As TreeNode = categorynode52.Nodes.Add("node522", "Pure Honeycomb Materials")
        Dim categorynode5221 As TreeNode = categorynode522.Nodes.Add("node5221", "Aluminium Honeycomb")
        categorynode5221.Nodes.Add("item123493466", "Aged Aluminium")
        categorynode5221.Nodes.Add("item123493466", "Glossy Aluminium")
        categorynode5221.Nodes.Add("item123493466", "Matte Aluminium")
        categorynode5221.Nodes.Add("item123493466", "Painted Black Aluminium")
        categorynode5221.Nodes.Add("item123493466", "Painted Dark Gray Aluminium")
        categorynode5221.Nodes.Add("item123493466", "Painted Gray Aluminium")
        categorynode5221.Nodes.Add("item123493466", "Painted Green Aluminium")
        categorynode5221.Nodes.Add("item123493466", "Painted Ice Aluminium")
        categorynode5221.Nodes.Add("item123493466", "Painted Light Gray Aluminium")
        categorynode5221.Nodes.Add("item123493466", "Painted Military Aluminium")
        categorynode5221.Nodes.Add("item123493466", "Painted Orange Aluminium")
        categorynode5221.Nodes.Add("item123493466", "Painted Purple Aluminium")
        categorynode5221.Nodes.Add("item123493466", "Painted Red Aluminium")
        categorynode5221.Nodes.Add("item123493466", "Painted Sky Aluminium")
        categorynode5221.Nodes.Add("item123493466", "Painted White Aluminium")
        categorynode5221.Nodes.Add("item123493466", "Painted Yellow Aluminium")
        categorynode5221.Nodes.Add("item123493466", "Black Aluminium Panel")
        categorynode5221.Nodes.Add("item123493466", "Dark Gray Aluminium Panel")
        categorynode5221.Nodes.Add("item123493466", "Gray Aluminium Panel")
        categorynode5221.Nodes.Add("item123493466", "Green Aluminium Panel")
        categorynode5221.Nodes.Add("item123493466", "Ice Aluminium Panel")
        categorynode5221.Nodes.Add("item123493466", "Light Gray Aluminium Panel")
        categorynode5221.Nodes.Add("item123493466", "Military Aluminium Panel")
        categorynode5221.Nodes.Add("item123493466", "Orange Aluminium Panel")
        categorynode5221.Nodes.Add("item123493466", "Purple Aluminium Panel")
        categorynode5221.Nodes.Add("item123493466", "Red Aluminium Panel")
        categorynode5221.Nodes.Add("item123493466", "Yellow Aluminium Panel")
        categorynode5221.Nodes.Add("item123493466", "White Aluminium Panel")
        categorynode5221.Nodes.Add("item123493466", "Sky Aluminium Panel")
        categorynode5221.Nodes.Add("item123493466", "White aluminium pattern")
        categorynode5221.Nodes.Add("item123493466", "Stained gray pattern aluminium")
        categorynode5221.Nodes.Add("item123493466", "Aged gray pattern aluminium")
        categorynode5221.Nodes.Add("item123493466", "Y aluminium pattern")
        categorynode5221.Nodes.Add("item123493466", "Black pattern aluminium")
        categorynode5221.Nodes.Add("item123493466", "Galvanized white aluminium")
        categorynode5221.Nodes.Add("item123493466", "Galvanized light gray aluminium")
        categorynode5221.Nodes.Add("item123493466", "Galvanized dark gray aluminium")
        categorynode5221.Nodes.Add("item123493466", "Galvanized gray aluminium")
        categorynode5221.Nodes.Add("item123493466", "Galvanized black aluminium")
        categorynode5221.Nodes.Add("item123493466", "Polished white aluminium")
        categorynode5221.Nodes.Add("item123493466", "Polished light gray aluminium")
        categorynode5221.Nodes.Add("item123493466", "Polished dark gray aluminium")
        categorynode5221.Nodes.Add("item123493466", "Polished gray aluminium")
        categorynode5221.Nodes.Add("item123493466", "Polished black aluminium")

        Dim categorynode5222 As TreeNode = categorynode522.Nodes.Add("node5222", "Calcium Honeycomb")
        categorynode5222.Nodes.Add("item3628423708", "Light Gray Calcium Panel")
        categorynode5222.Nodes.Add("item3628423708", "Military Calcium Panel")
        categorynode5222.Nodes.Add("item3628423708", "Orange Calcium Panel")
        categorynode5222.Nodes.Add("item3628423708", "Purple Calcium Panel")
        categorynode5222.Nodes.Add("item3628423708", "Red Calcium Panel")
        categorynode5222.Nodes.Add("item3628423708", "Sky Calcium Panel")
        categorynode5222.Nodes.Add("item3628423708", "White Calcium Panel")
        categorynode5222.Nodes.Add("item3628423708", "Yellow Calcium Panel")
        categorynode5222.Nodes.Add("item3628423708", "Painted Light Gray Calcium")
        categorynode5222.Nodes.Add("item3628423708", "Painted Green Calcium")
        categorynode5222.Nodes.Add("item3628423708", "Painted Gray Calcium")
        categorynode5222.Nodes.Add("item3628423708", "Painted Dark Gray Calcium")
        categorynode5222.Nodes.Add("item3628423708", "Matte Calcium")
        categorynode5222.Nodes.Add("item3628423708", "Glossy Calcium")
        categorynode5222.Nodes.Add("item3628423708", "Aged Calcium")
        categorynode5222.Nodes.Add("item3628423708", "Painted Black Calcium")
        categorynode5222.Nodes.Add("item3628423708", "Ice Calcium Panel")
        categorynode5222.Nodes.Add("item3628423708", "Green Calcium Panel")
        categorynode5222.Nodes.Add("item3628423708", "Gray Calcium Panel")
        categorynode5222.Nodes.Add("item3628423708", "Dark Gray Calcium Panel")
        categorynode5222.Nodes.Add("item3628423708", "Painted Ice Calcium")
        categorynode5222.Nodes.Add("item3628423708", "Black Calcium Panel")
        categorynode5222.Nodes.Add("item3628423708", "Painted White Calcium")
        categorynode5222.Nodes.Add("item3628423708", "Painted Sky Calcium")
        categorynode5222.Nodes.Add("item3628423708", "Painted Red Calcium")
        categorynode5222.Nodes.Add("item3628423708", "Painted Purple Calcium")
        categorynode5222.Nodes.Add("item3628423708", "Painted Orange Calcium")
        categorynode5222.Nodes.Add("item3628423708", "Painted Military Calcium")
        categorynode5222.Nodes.Add("item3628423708", "Painted Yellow Calcium")

        Dim categorynode5223 As TreeNode = categorynode522.Nodes.Add("node5223", "Carbon Honeycomb")
        categorynode5223.Nodes.Add("item1063775897", "Aged Carbon")
        categorynode5223.Nodes.Add("item1063775897", "Matte Carbon")
        categorynode5223.Nodes.Add("item1063775897", "Painted Black Carbon")
        categorynode5223.Nodes.Add("item1063775897", "Painted Dark Gray Carbon")
        categorynode5223.Nodes.Add("item1063775897", "Painted Gray Carbon")
        categorynode5223.Nodes.Add("item1063775897", "Painted Green Carbon")
        categorynode5223.Nodes.Add("item1063775897", "Painted Ice Carbon")
        categorynode5223.Nodes.Add("item1063775897", "Painted Light Gray Carbon")
        categorynode5223.Nodes.Add("item1063775897", "Painted Military Carbon")
        categorynode5223.Nodes.Add("item1063775897", "Painted Orange Carbon")
        categorynode5223.Nodes.Add("item1063775897", "Painted Purple Carbon")
        categorynode5223.Nodes.Add("item1063775897", "Painted Red Carbon")
        categorynode5223.Nodes.Add("item1063775897", "Painted Sky Carbon")
        categorynode5223.Nodes.Add("item1063775897", "Painted White Carbon")
        categorynode5223.Nodes.Add("item1063775897", "Painted Yellow Carbon")
        categorynode5223.Nodes.Add("item1063775897", "Glossy Carbon")
        categorynode5223.Nodes.Add("item1063775897", "Black Carbon Panel")
        categorynode5223.Nodes.Add("item1063775897", "Dark Gray Carbon Panel")
        categorynode5223.Nodes.Add("item1063775897", "Gray Carbon Panel")
        categorynode5223.Nodes.Add("item1063775897", "Green Carbon Panel")
        categorynode5223.Nodes.Add("item1063775897", "Ice Carbon Panel")
        categorynode5223.Nodes.Add("item1063775897", "Light Gray Carbon Panel")
        categorynode5223.Nodes.Add("item1063775897", "Military Carbon Panel")
        categorynode5223.Nodes.Add("item1063775897", "Orange Carbon Panel")
        categorynode5223.Nodes.Add("item1063775897", "Purple Carbon Panel")
        categorynode5223.Nodes.Add("item1063775897", "Red Carbon Panel")
        categorynode5223.Nodes.Add("item1063775897", "Sky Carbon Panel")
        categorynode5223.Nodes.Add("item1063775897", "White Carbon Panel")
        categorynode5223.Nodes.Add("item1063775897", "Yellow Carbon Panel")

        Dim categorynode5224 As TreeNode = categorynode522.Nodes.Add("node5224", "Chromium Honeycomb")
        categorynode5224.Nodes.Add("item1406093224", "Yellow Chromium Panel")
        categorynode5224.Nodes.Add("item1406093224", "White Chromium Panel")
        categorynode5224.Nodes.Add("item1406093224", "Sky Chromium Panel")
        categorynode5224.Nodes.Add("item1406093224", "Red Chromium Panel")
        categorynode5224.Nodes.Add("item1406093224", "Purple Chromium Panel")
        categorynode5224.Nodes.Add("item1406093224", "Military Chromium Panel")
        categorynode5224.Nodes.Add("item1406093224", "Painted Black Chromium")
        categorynode5224.Nodes.Add("item1406093224", "Matte Chromium")
        categorynode5224.Nodes.Add("item1406093224", "Glossy Chromium")
        categorynode5224.Nodes.Add("item1406093224", "Aged Chromium")
        categorynode5224.Nodes.Add("item1406093224", "Painted Dark Gray Chromium")
        categorynode5224.Nodes.Add("item1406093224", "Orange Chromium Panel")
        categorynode5224.Nodes.Add("item1406093224", "Painted Gray Chromium")
        categorynode5224.Nodes.Add("item1406093224", "Painted Ice Chromium")
        categorynode5224.Nodes.Add("item1406093224", "Light Gray Chromium Panel")
        categorynode5224.Nodes.Add("item1406093224", "Ice Chromium Panel")
        categorynode5224.Nodes.Add("item1406093224", "Green Chromium Panel")
        categorynode5224.Nodes.Add("item1406093224", "Gray Chromium Panel")
        categorynode5224.Nodes.Add("item1406093224", "Dark Gray Chromium Panel")
        categorynode5224.Nodes.Add("item1406093224", "Black Chromium Panel")
        categorynode5224.Nodes.Add("item1406093224", "Painted Green Chromium")
        categorynode5224.Nodes.Add("item1406093224", "Painted Yellow Chromium")
        categorynode5224.Nodes.Add("item1406093224", "Painted Sky Chromium")
        categorynode5224.Nodes.Add("item1406093224", "Painted Red Chromium")
        categorynode5224.Nodes.Add("item1406093224", "Painted Purple Chromium")
        categorynode5224.Nodes.Add("item1406093224", "Painted Orange Chromium")
        categorynode5224.Nodes.Add("item1406093224", "Painted Military Chromium")
        categorynode5224.Nodes.Add("item1406093224", "Painted Light Gray Chromium")
        categorynode5224.Nodes.Add("item1406093224", "Painted White Chromium")
        categorynode5224.Nodes.Add("item1406093224", "White pattern Chromium")
        categorynode5224.Nodes.Add("item1406093224", "Stained gray pattern Chromium")
        categorynode5224.Nodes.Add("item1406093224", "Aged gray pattern Chromium")
        categorynode5224.Nodes.Add("item1406093224", "Gray pattern Chromium")
        categorynode5224.Nodes.Add("item1406093224", "Black pattern Chromium")
        categorynode5224.Nodes.Add("item1406093224", "Galvanized white Chromium")
        categorynode5224.Nodes.Add("item1406093224", "Galvanized light gray Chromium")
        categorynode5224.Nodes.Add("item1406093224", "Galvanized dark gray Chromium")
        categorynode5224.Nodes.Add("item1406093224", "Galvanized gray Chromium")
        categorynode5224.Nodes.Add("item1406093224", "Galvanized black Chromium")
        categorynode5224.Nodes.Add("item1406093224", "Polished white Chromium")
        categorynode5224.Nodes.Add("item1406093224", "Polished light gray Chromium")
        categorynode5224.Nodes.Add("item1406093224", "Polished dark gray Chromium")
        categorynode5224.Nodes.Add("item1406093224", "Polished gray Chromium")
        categorynode5224.Nodes.Add("item1406093224", "Polished black Chromium")

        Dim categorynode5225 As TreeNode = categorynode522.Nodes.Add("node5225", "Cobalt Honeycomb")
        categorynode5225.Nodes.Add("item3292873120", "Painted Orange Cobalt")
        categorynode5225.Nodes.Add("item3292873120", "Painted Military Cobalt")
        categorynode5225.Nodes.Add("item3292873120", "Painted Light Gray Cobalt")
        categorynode5225.Nodes.Add("item3292873120", "Painted Ice Cobalt")
        categorynode5225.Nodes.Add("item3292873120", "Painted Green Cobalt")
        categorynode5225.Nodes.Add("item3292873120", "Painted Gray Cobalt")
        categorynode5225.Nodes.Add("item3292873120", "Painted Purple Cobalt")
        categorynode5225.Nodes.Add("item3292873120", "Painted Dark Gray Cobalt")
        categorynode5225.Nodes.Add("item3292873120", "Matte Cobalt")
        categorynode5225.Nodes.Add("item3292873120", "Glossy Cobalt")
        categorynode5225.Nodes.Add("item3292873120", "Aged Cobalt")
        categorynode5225.Nodes.Add("item3292873120", "Painted Black Cobalt")
        categorynode5225.Nodes.Add("item3292873120", "Painted Red Cobalt")
        categorynode5225.Nodes.Add("item3292873120", "Painted White Cobalt")
        categorynode5225.Nodes.Add("item3292873120", "Yellow Cobalt Panel")
        categorynode5225.Nodes.Add("item3292873120", "White Cobalt Panel")
        categorynode5225.Nodes.Add("item3292873120", "Sky Cobalt Panel")
        categorynode5225.Nodes.Add("item3292873120", "Red Cobalt Panel")
        categorynode5225.Nodes.Add("item3292873120", "Purple Cobalt Panel")
        categorynode5225.Nodes.Add("item3292873120", "Orange Cobalt Panel")
        categorynode5225.Nodes.Add("item3292873120", "Painted Sky Cobalt")
        categorynode5225.Nodes.Add("item3292873120", "Military Cobalt Panel")
        categorynode5225.Nodes.Add("item3292873120", "Ice Cobalt Panel")
        categorynode5225.Nodes.Add("item3292873120", "Green Cobalt Panel")
        categorynode5225.Nodes.Add("item3292873120", "Gray Cobalt Panel")
        categorynode5225.Nodes.Add("item3292873120", "Dark Gray Cobalt Panel")
        categorynode5225.Nodes.Add("item3292873120", "Black Cobalt Panel")
        categorynode5225.Nodes.Add("item3292873120", "Painted Yellow Cobalt")
        categorynode5225.Nodes.Add("item3292873120", "Light Gray Cobalt Panel")

        Dim categorynode5226 As TreeNode = categorynode522.Nodes.Add("node5226", "Copper Honeycomb")
        categorynode5226.Nodes.Add("item1374916603", "Black Copper Panel")
        categorynode5226.Nodes.Add("item1374916603", "Dark Gray Copper Panel")
        categorynode5226.Nodes.Add("item1374916603", "Gray Copper Panel")
        categorynode5226.Nodes.Add("item1374916603", "Green Copper Panel")
        categorynode5226.Nodes.Add("item1374916603", "Ice Copper Panel")
        categorynode5226.Nodes.Add("item1374916603", "Light Gray Copper Panel")
        categorynode5226.Nodes.Add("item1374916603", "Military Copper Panel")
        categorynode5226.Nodes.Add("item1374916603", "Orange Copper Panel")
        categorynode5226.Nodes.Add("item1374916603", "Purple Copper Panel")
        categorynode5226.Nodes.Add("item1374916603", "Red Copper Panel")
        categorynode5226.Nodes.Add("item1374916603", "Sky Copper Panel")
        categorynode5226.Nodes.Add("item1374916603", "White Copper Panel")
        categorynode5226.Nodes.Add("item1374916603", "Yellow Copper Panel")
        categorynode5226.Nodes.Add("item1374916603", "Painted Yellow Copper")
        categorynode5226.Nodes.Add("item1374916603", "Painted White Copper")
        categorynode5226.Nodes.Add("item1374916603", "Painted Sky Copper")
        categorynode5226.Nodes.Add("item1374916603", "Painted Red Copper")
        categorynode5226.Nodes.Add("item1374916603", "Painted Purple Copper")
        categorynode5226.Nodes.Add("item1374916603", "Painted Orange Copper")
        categorynode5226.Nodes.Add("item1374916603", "Painted Military Copper")
        categorynode5226.Nodes.Add("item1374916603", "Painted Light Gray Copper")
        categorynode5226.Nodes.Add("item1374916603", "Painted Ice Copper")
        categorynode5226.Nodes.Add("item1374916603", "Painted Green Copper")
        categorynode5226.Nodes.Add("item1374916603", "Painted Gray Copper")
        categorynode5226.Nodes.Add("item1374916603", "Painted Black Copper")
        categorynode5226.Nodes.Add("item1374916603", "Matte Copper")
        categorynode5226.Nodes.Add("item1374916603", "Glossy Copper")
        categorynode5226.Nodes.Add("item1374916603", "Aged Copper")
        categorynode5226.Nodes.Add("item1374916603", "Painted Dark Gray Copper")
        categorynode5226.Nodes.Add("item1374916603", "Polished gray Copper")
        categorynode5226.Nodes.Add("item1374916603", "Polished black Copper")
        categorynode5226.Nodes.Add("item1374916603", "White pattern Copper")
        categorynode5226.Nodes.Add("item1374916603", "Stained gray pattern Copper")
        categorynode5226.Nodes.Add("item1374916603", "Aged gray pattern Copper")
        categorynode5226.Nodes.Add("item1374916603", "Gray pattern Copper")
        categorynode5226.Nodes.Add("item1374916603", "Black pattern Copper")
        categorynode5226.Nodes.Add("item1374916603", "Galvanized white Copper")
        categorynode5226.Nodes.Add("item1374916603", "Galvanized light gray Copper")
        categorynode5226.Nodes.Add("item1374916603", "Aged gray Copper")
        categorynode5226.Nodes.Add("item1374916603", "Galvanized gray Copper")
        categorynode5226.Nodes.Add("item1374916603", "Galvanized black Copper")
        categorynode5226.Nodes.Add("item1374916603", "Polished white Copper")
        categorynode5226.Nodes.Add("item1374916603", "Polished light gray Copper")
        categorynode5226.Nodes.Add("item1374916603", "Polished dark gray Copper")

        Dim categorynode5227 As TreeNode = categorynode522.Nodes.Add("node5227", "Fluorine Honeycomb")
        categorynode5227.Nodes.Add("item1440099000", "Military Fluorine Panel")
        categorynode5227.Nodes.Add("item1440099000", "Orange Fluorine Panel")
        categorynode5227.Nodes.Add("item1440099000", "Purple Fluorine Panel")
        categorynode5227.Nodes.Add("item1440099000", "Red Fluorine Panel")
        categorynode5227.Nodes.Add("item1440099000", "Sky Fluorine Panel")
        categorynode5227.Nodes.Add("item1440099000", "White Fluorine Panel")
        categorynode5227.Nodes.Add("item1440099000", "Yellow Fluorine Panel")
        categorynode5227.Nodes.Add("item1440099000", "Light Gray Fluorine Panel")
        categorynode5227.Nodes.Add("item1440099000", "Ice Fluorine Panel")
        categorynode5227.Nodes.Add("item1440099000", "Green Fluorine Panel")
        categorynode5227.Nodes.Add("item1440099000", "Gray Fluorine Panel")
        categorynode5227.Nodes.Add("item1440099000", "Aged Fluorine")
        categorynode5227.Nodes.Add("item1440099000", "Glossy Fluorine")
        categorynode5227.Nodes.Add("item1440099000", "Matte Fluorine")
        categorynode5227.Nodes.Add("item1440099000", "Painted Black Fluorine")
        categorynode5227.Nodes.Add("item1440099000", "Dark Gray Fluorine Panel")
        categorynode5227.Nodes.Add("item1440099000", "Black Fluorine Panel")
        categorynode5227.Nodes.Add("item1440099000", "Painted Yellow Fluorine")
        categorynode5227.Nodes.Add("item1440099000", "Painted White Fluorine")
        categorynode5227.Nodes.Add("item1440099000", "Painted Sky Fluorine")
        categorynode5227.Nodes.Add("item1440099000", "Painted Red Fluorine")
        categorynode5227.Nodes.Add("item1440099000", "Painted Purple Fluorine")
        categorynode5227.Nodes.Add("item1440099000", "Painted Military Fluorine")
        categorynode5227.Nodes.Add("item1440099000", "Painted Light Gray Fluorine")
        categorynode5227.Nodes.Add("item1440099000", "Painted Ice Fluorine")
        categorynode5227.Nodes.Add("item1440099000", "Painted Green Fluorine")
        categorynode5227.Nodes.Add("item1440099000", "Painted Gray Fluorine")
        categorynode5227.Nodes.Add("item1440099000", "Painted Dark Gray Fluorine")
        categorynode5227.Nodes.Add("item1440099000", "Painted Orange Fluorine")

        Dim categorynode5228 As TreeNode = categorynode522.Nodes.Add("node5228", "Gold Honeycomb")
        categorynode5228.Nodes.Add("item2892111312", "Aged Gold")
        categorynode5228.Nodes.Add("item2892111312", "Glossy Gold")
        categorynode5228.Nodes.Add("item2892111312", "Matte Gold")
        categorynode5228.Nodes.Add("item2892111312", "Painted Black Gold")
        categorynode5228.Nodes.Add("item2892111312", "Painted Dark Gray Gold")
        categorynode5228.Nodes.Add("item2892111312", "Painted Gray Gold")
        categorynode5228.Nodes.Add("item2892111312", "Painted Green Gold")
        categorynode5228.Nodes.Add("item2892111312", "Painted Ice Gold")
        categorynode5228.Nodes.Add("item2892111312", "Painted Light Gray Gold")
        categorynode5228.Nodes.Add("item2892111312", "Painted Military Gold")
        categorynode5228.Nodes.Add("item2892111312", "Painted Purple Gold")
        categorynode5228.Nodes.Add("item2892111312", "Painted Red Gold")
        categorynode5228.Nodes.Add("item2892111312", "Painted Sky Gold")
        categorynode5228.Nodes.Add("item2892111312", "Painted White Gold")
        categorynode5228.Nodes.Add("item2892111312", "Painted Yellow Gold")
        categorynode5228.Nodes.Add("item2892111312", "Black Gold Panel")
        categorynode5228.Nodes.Add("item2892111312", "Dark Gray Gold Panel")
        categorynode5228.Nodes.Add("item2892111312", "Gray Gold Panel")
        categorynode5228.Nodes.Add("item2892111312", "Green Gold Panel")
        categorynode5228.Nodes.Add("item2892111312", "Ice Gold Panel")
        categorynode5228.Nodes.Add("item2892111312", "Light Gray Gold Panel")
        categorynode5228.Nodes.Add("item2892111312", "Military Gold Panel")
        categorynode5228.Nodes.Add("item2892111312", "Orange Gold Panel")
        categorynode5228.Nodes.Add("item2892111312", "Purple Gold Panel")
        categorynode5228.Nodes.Add("item2892111312", "Red Gold Panel")
        categorynode5228.Nodes.Add("item2892111312", "Sky Gold Panel")
        categorynode5228.Nodes.Add("item2892111312", "White Gold Panel")
        categorynode5228.Nodes.Add("item2892111312", "Yellow Gold Panel")
        categorynode5228.Nodes.Add("item2892111312", "Painted Orange Gold")
        categorynode5228.Nodes.Add("item2892111312", "White pattern Gold")
        categorynode5228.Nodes.Add("item2892111312", "Stained gray pattern Gold")
        categorynode5228.Nodes.Add("item2892111312", "Aged gray pattern Gold")
        categorynode5228.Nodes.Add("item2892111312", "Gray pattern Gold")
        categorynode5228.Nodes.Add("item2892111312", "Black pattern Gold")
        categorynode5228.Nodes.Add("item2892111312", "Galvanized white Gold")
        categorynode5228.Nodes.Add("item2892111312", "Galvanized light gray Gold")
        categorynode5228.Nodes.Add("item2892111312", "Galvanized dark gray Gold")
        categorynode5228.Nodes.Add("item2892111312", "Galvanized gray Gold")
        categorynode5228.Nodes.Add("item2892111312", "Galvanized black Gold")
        categorynode5228.Nodes.Add("item2892111312", "Polished white Gold")
        categorynode5228.Nodes.Add("item2892111312", "Polished light gray Gold")
        categorynode5228.Nodes.Add("item2892111312", "Polished dark gray Gold")
        categorynode5228.Nodes.Add("item2892111312", "Polished gray Gold")
        categorynode5228.Nodes.Add("item2892111312", "Polished black Gold")

        Dim categorynode5229 As TreeNode = categorynode522.Nodes.Add("node5229", "Iron Honeycomb")
        categorynode5229.Nodes.Add("item2085561075", "Dark Gray Iron Panel")
        categorynode5229.Nodes.Add("item2085561075", "Gray Iron Panel")
        categorynode5229.Nodes.Add("item2085561075", "Green Iron Panel")
        categorynode5229.Nodes.Add("item2085561075", "Ice Iron Panel")
        categorynode5229.Nodes.Add("item2085561075", "Light Gray Iron Panel")
        categorynode5229.Nodes.Add("item2085561075", "Military Iron Panel")
        categorynode5229.Nodes.Add("item2085561075", "Orange Iron Panel")
        categorynode5229.Nodes.Add("item2085561075", "Purple Iron Panel")
        categorynode5229.Nodes.Add("item2085561075", "Red Iron Panel")
        categorynode5229.Nodes.Add("item2085561075", "Sky Iron Panel")
        categorynode5229.Nodes.Add("item2085561075", "White Iron Panel")
        categorynode5229.Nodes.Add("item2085561075", "Yellow Iron Panel")
        categorynode5229.Nodes.Add("item2085561075", "Black Iron Panel")
        categorynode5229.Nodes.Add("item2085561075", "Painted Yellow Iron")
        categorynode5229.Nodes.Add("item2085561075", "Painted White Iron")
        categorynode5229.Nodes.Add("item2085561075", "Painted Sky Iron")
        categorynode5229.Nodes.Add("item2085561075", "Painted Red Iron")
        categorynode5229.Nodes.Add("item2085561075", "Painted Purple Iron")
        categorynode5229.Nodes.Add("item2085561075", "Painted Orange Iron")
        categorynode5229.Nodes.Add("item2085561075", "Painted Military Iron")
        categorynode5229.Nodes.Add("item2085561075", "Painted Light Gray Iron")
        categorynode5229.Nodes.Add("item2085561075", "Painted Ice Iron")
        categorynode5229.Nodes.Add("item2085561075", "Painted Green Iron")
        categorynode5229.Nodes.Add("item2085561075", "Painted Dark Gray Iron")
        categorynode5229.Nodes.Add("item2085561075", "Painted Black Iron")
        categorynode5229.Nodes.Add("item2085561075", "Matte Iron")
        categorynode5229.Nodes.Add("item2085561075", "Glossy Iron")
        categorynode5229.Nodes.Add("item2085561075", "Aged Iron")
        categorynode5229.Nodes.Add("item2085561075", "Painted Gray Iron")
        categorynode5229.Nodes.Add("item2085561075", "White pattern iron")
        categorynode5229.Nodes.Add("item2085561075", "Stained gray pattern iron")
        categorynode5229.Nodes.Add("item2085561075", "Aged gray pattern iron")
        categorynode5229.Nodes.Add("item2085561075", "Gray pattern iron")
        categorynode5229.Nodes.Add("item2085561075", "Black pattern iron")
        categorynode5229.Nodes.Add("item2085561075", "Galvanized white iron")
        categorynode5229.Nodes.Add("item2085561075", "Galvanized light gray iron")
        categorynode5229.Nodes.Add("item2085561075", "Galvanized dark gray iron")
        categorynode5229.Nodes.Add("item2085561075", "Galvanized gray iron")
        categorynode5229.Nodes.Add("item2085561075", "Galvanized black iron")
        categorynode5229.Nodes.Add("item2085561075", "Polished white iron")
        categorynode5229.Nodes.Add("item2085561075", "Polished light gray iron")
        categorynode5229.Nodes.Add("item2085561075", "Polished dark gray iron")
        categorynode5229.Nodes.Add("item2085561075", "Polished gray iron")
        categorynode5229.Nodes.Add("item2085561075", "Polished black iron")


        Dim categorynode52210 As TreeNode = categorynode522.Nodes.Add("node52210", "Lithium Honeycomb")
        categorynode52210.Nodes.Add("item1987555115", "Painted Light Gray Lithium")
        categorynode52210.Nodes.Add("item1987555115", "Painted Military Lithium")
        categorynode52210.Nodes.Add("item1987555115", "Painted Orange Lithium")
        categorynode52210.Nodes.Add("item1987555115", "Painted Purple Lithium")
        categorynode52210.Nodes.Add("item1987555115", "Painted Red Lithium")
        categorynode52210.Nodes.Add("item1987555115", "Painted Sky Lithium")
        categorynode52210.Nodes.Add("item1987555115", "Painted White Lithium")
        categorynode52210.Nodes.Add("item1987555115", "Painted Yellow Lithium")
        categorynode52210.Nodes.Add("item1987555115", "Black Lithium Panel")
        categorynode52210.Nodes.Add("item1987555115", "Dark Gray Lithium Panel")
        categorynode52210.Nodes.Add("item1987555115", "Gray Lithium Panel")
        categorynode52210.Nodes.Add("item1987555115", "Green Lithium Panel")
        categorynode52210.Nodes.Add("item1987555115", "Ice Lithium Panel")
        categorynode52210.Nodes.Add("item1987555115", "Light Gray Lithium Panel")
        categorynode52210.Nodes.Add("item1987555115", "Military Lithium Panel")
        categorynode52210.Nodes.Add("item1987555115", "Orange Lithium Panel")
        categorynode52210.Nodes.Add("item1987555115", "Purple Lithium Panel")
        categorynode52210.Nodes.Add("item1987555115", "Yellow Lithium Panel")
        categorynode52210.Nodes.Add("item1987555115", "White Lithium Panel")
        categorynode52210.Nodes.Add("item1987555115", "Sky Lithium Panel")
        categorynode52210.Nodes.Add("item1987555115", "Red Lithium Panel")
        categorynode52210.Nodes.Add("item1987555115", "Painted Ice Lithium")
        categorynode52210.Nodes.Add("item1987555115", "Painted Green Lithium")
        categorynode52210.Nodes.Add("item1987555115", "Painted Gray Lithium")
        categorynode52210.Nodes.Add("item1987555115", "Painted Dark Gray Lithium")
        categorynode52210.Nodes.Add("item1987555115", "Aged Lithium")
        categorynode52210.Nodes.Add("item1987555115", "Glossy Lithium")
        categorynode52210.Nodes.Add("item1987555115", "Matte Lithium")
        categorynode52210.Nodes.Add("item1987555115", "Painted Black Lithium")

        Dim categorynode52211 As TreeNode = categorynode522.Nodes.Add("node52211", "Manganese Honeycomb")
        categorynode52211.Nodes.Add("item3522164802", "Painted White Manganese")
        categorynode52211.Nodes.Add("item3522164802", "Painted Yellow Manganese")
        categorynode52211.Nodes.Add("item3522164802", "Black Manganese Panel")
        categorynode52211.Nodes.Add("item3522164802", "Dark Gray Manganese Panel")
        categorynode52211.Nodes.Add("item3522164802", "Gray Manganese Panel")
        categorynode52211.Nodes.Add("item3522164802", "Green Manganese Panel")
        categorynode52211.Nodes.Add("item3522164802", "Ice Manganese Panel")
        categorynode52211.Nodes.Add("item3522164802", "Light Gray Manganese Panel")
        categorynode52211.Nodes.Add("item3522164802", "Military Manganese Panel")
        categorynode52211.Nodes.Add("item3522164802", "Orange Manganese Panel")
        categorynode52211.Nodes.Add("item3522164802", "Purple Manganese Panel")
        categorynode52211.Nodes.Add("item3522164802", "Red Manganese Panel")
        categorynode52211.Nodes.Add("item3522164802", "Sky Manganese Panel")
        categorynode52211.Nodes.Add("item3522164802", "White Manganese Panel")
        categorynode52211.Nodes.Add("item3522164802", "Painted Sky Manganese")
        categorynode52211.Nodes.Add("item3522164802", "Yellow Manganese Panel")
        categorynode52211.Nodes.Add("item3522164802", "Painted Red Manganese")
        categorynode52211.Nodes.Add("item3522164802", "Painted Purple Manganese")
        categorynode52211.Nodes.Add("item3522164802", "Painted Orange Manganese")
        categorynode52211.Nodes.Add("item3522164802", "Painted Military Manganese")
        categorynode52211.Nodes.Add("item3522164802", "Painted Light Gray Manganese")
        categorynode52211.Nodes.Add("item3522164802", "Painted Ice Manganese")
        categorynode52211.Nodes.Add("item3522164802", "Painted Green Manganese")
        categorynode52211.Nodes.Add("item3522164802", "Painted Gray Manganese")
        categorynode52211.Nodes.Add("item3522164802", "Painted Dark Gray Manganese")
        categorynode52211.Nodes.Add("item3522164802", "Painted Black Manganese")
        categorynode52211.Nodes.Add("item3522164802", "Glossy Manganese")
        categorynode52211.Nodes.Add("item3522164802", "Aged Manganese")
        categorynode52211.Nodes.Add("item3522164802", "Matte Manganese")

        Dim categorynode52212 As TreeNode = categorynode522.Nodes.Add("node52212", "Nickel Honeycomb")
        categorynode52212.Nodes.Add("item1194276464", "Glossy Nickel")
        categorynode52212.Nodes.Add("item1194276464", "Matte Nickel")
        categorynode52212.Nodes.Add("item1194276464", "Painted Black Nickel")
        categorynode52212.Nodes.Add("item1194276464", "Painted Dark Gray Nickel")
        categorynode52212.Nodes.Add("item1194276464", "Painted Gray Nickel")
        categorynode52212.Nodes.Add("item1194276464", "Painted Green Nickel")
        categorynode52212.Nodes.Add("item1194276464", "Painted Ice Nickel")
        categorynode52212.Nodes.Add("item1194276464", "Painted Light Gray Nickel")
        categorynode52212.Nodes.Add("item1194276464", "Painted Military Nickel")
        categorynode52212.Nodes.Add("item1194276464", "Painted Orange Nickel")
        categorynode52212.Nodes.Add("item1194276464", "Painted Purple Nickel")
        categorynode52212.Nodes.Add("item1194276464", "Painted Red Nickel")
        categorynode52212.Nodes.Add("item1194276464", "Painted Sky Nickel")
        categorynode52212.Nodes.Add("item1194276464", "Painted White Nickel")
        categorynode52212.Nodes.Add("item1194276464", "Aged Nickel")
        categorynode52212.Nodes.Add("item1194276464", "Painted Yellow Nickel")
        categorynode52212.Nodes.Add("item1194276464", "Black Nickel Panel")
        categorynode52212.Nodes.Add("item1194276464", "Gray Nickel Panel")
        categorynode52212.Nodes.Add("item1194276464", "Green Nickel Panel")
        categorynode52212.Nodes.Add("item1194276464", "Ice Nickel Panel")
        categorynode52212.Nodes.Add("item1194276464", "Light Gray Nickel Panel")
        categorynode52212.Nodes.Add("item1194276464", "Military Nickel Panel")
        categorynode52212.Nodes.Add("item1194276464", "Orange Nickel Panel")
        categorynode52212.Nodes.Add("item1194276464", "Purple Nickel Panel")
        categorynode52212.Nodes.Add("item1194276464", "Red Nickel Panel")
        categorynode52212.Nodes.Add("item1194276464", "Sky Nickel Panel")
        categorynode52212.Nodes.Add("item1194276464", "White Nickel Panel")
        categorynode52212.Nodes.Add("item1194276464", "Yellow Nickel Panel")
        categorynode52212.Nodes.Add("item1194276464", "Dark Gray Nickel Panel")

        Dim categorynode52213 As TreeNode = categorynode522.Nodes.Add("node52213", "Niobium Honeycomb")
        categorynode52213.Nodes.Add("item30546913", "Red Niobium Panel")
        categorynode52213.Nodes.Add("item30546913", "Sky Niobium Panel")
        categorynode52213.Nodes.Add("item30546913", "White Niobium Panel")
        categorynode52213.Nodes.Add("item30546913", "Yellow Niobium Panel")
        categorynode52213.Nodes.Add("item30546913", "Purple Niobium Panel")
        categorynode52213.Nodes.Add("item30546913", "Orange Niobium Panel")
        categorynode52213.Nodes.Add("item30546913", "Military Niobium Panel")
        categorynode52213.Nodes.Add("item30546913", "Light Gray Niobium Panel")
        categorynode52213.Nodes.Add("item30546913", "Aged Niobium")
        categorynode52213.Nodes.Add("item30546913", "Glossy Niobium")
        categorynode52213.Nodes.Add("item30546913", "Matte Niobium")
        categorynode52213.Nodes.Add("item30546913", "Painted Black Niobium")
        categorynode52213.Nodes.Add("item30546913", "Painted Dark Gray Niobium")
        categorynode52213.Nodes.Add("item30546913", "Painted Gray Niobium")
        categorynode52213.Nodes.Add("item30546913", "Painted Green Niobium")
        categorynode52213.Nodes.Add("item30546913", "Ice Niobium Panel")
        categorynode52213.Nodes.Add("item30546913", "Green Niobium Panel")
        categorynode52213.Nodes.Add("item30546913", "Gray Niobium Panel")
        categorynode52213.Nodes.Add("item30546913", "Dark Gray Niobium Panel")
        categorynode52213.Nodes.Add("item30546913", "Black Niobium Panel")
        categorynode52213.Nodes.Add("item30546913", "Painted Yellow Niobium")
        categorynode52213.Nodes.Add("item30546913", "Painted White Niobium")
        categorynode52213.Nodes.Add("item30546913", "Painted Red Niobium")
        categorynode52213.Nodes.Add("item30546913", "Painted Purple Niobium")
        categorynode52213.Nodes.Add("item30546913", "Painted Orange Niobium")
        categorynode52213.Nodes.Add("item30546913", "Painted Military Niobium")
        categorynode52213.Nodes.Add("item30546913", "Painted Light Gray Niobium")
        categorynode52213.Nodes.Add("item30546913", "Painted Ice Niobium")
        categorynode52213.Nodes.Add("item30546913", "Painted Sky Niobium")

        Dim categorynode52214 As TreeNode = categorynode522.Nodes.Add("node52214", "Scandium Honeycomb")
        categorynode52214.Nodes.Add("item2980173742", "Yellow Scandium Panel")
        categorynode52214.Nodes.Add("item2980173742", "Sky Scandium Panel")
        categorynode52214.Nodes.Add("item2980173742", "White Scandium Panel")
        categorynode52214.Nodes.Add("item2980173742", "Aged Scandium")
        categorynode52214.Nodes.Add("item2980173742", "Glossy Scandium")
        categorynode52214.Nodes.Add("item2980173742", "Matte Scandium")
        categorynode52214.Nodes.Add("item2980173742", "Painted Black Scandium")
        categorynode52214.Nodes.Add("item2980173742", "Painted Dark Gray Scandium")
        categorynode52214.Nodes.Add("item2980173742", "Painted Gray Scandium")
        categorynode52214.Nodes.Add("item2980173742", "Painted Green Scandium")
        categorynode52214.Nodes.Add("item2980173742", "Painted Ice Scandium")
        categorynode52214.Nodes.Add("item2980173742", "Painted Light Gray Scandium")
        categorynode52214.Nodes.Add("item2980173742", "Painted Military Scandium")
        categorynode52214.Nodes.Add("item2980173742", "Painted Orange Scandium")
        categorynode52214.Nodes.Add("item2980173742", "Painted Red Scandium")
        categorynode52214.Nodes.Add("item2980173742", "Painted Sky Scandium")
        categorynode52214.Nodes.Add("item2980173742", "Painted White Scandium")
        categorynode52214.Nodes.Add("item2980173742", "Painted Yellow Scandium")
        categorynode52214.Nodes.Add("item2980173742", "Black Scandium Panel")
        categorynode52214.Nodes.Add("item2980173742", "Dark Gray Scandium Panel")
        categorynode52214.Nodes.Add("item2980173742", "Gray Scandium Panel")
        categorynode52214.Nodes.Add("item2980173742", "Green Scandium Panel")
        categorynode52214.Nodes.Add("item2980173742", "Ice Scandium Panel")
        categorynode52214.Nodes.Add("item2980173742", "Light Gray Scandium Panel")
        categorynode52214.Nodes.Add("item2980173742", "Military Scandium Panel")
        categorynode52214.Nodes.Add("item2980173742", "Orange Scandium Panel")
        categorynode52214.Nodes.Add("item2980173742", "Purple Scandium Panel")
        categorynode52214.Nodes.Add("item2980173742", "Red Scandium Panel")
        categorynode52214.Nodes.Add("item2980173742", "Painted Purple Scandium")

        Dim categorynode52215 As TreeNode = categorynode522.Nodes.Add("node52215", "Silicon Honeycomb")
        categorynode52215.Nodes.Add("item4079996329", "Purple Silicon Panel")
        categorynode52215.Nodes.Add("item4079996329", "Red Silicon Panel")
        categorynode52215.Nodes.Add("item4079996329", "Sky Silicon Panel")
        categorynode52215.Nodes.Add("item4079996329", "White Silicon Panel")
        categorynode52215.Nodes.Add("item4079996329", "Yellow Silicon Panel")
        categorynode52215.Nodes.Add("item4079996329", "Aged Silicon")
        categorynode52215.Nodes.Add("item4079996329", "Glossy Silicon")
        categorynode52215.Nodes.Add("item4079996329", "Matte Silicon")
        categorynode52215.Nodes.Add("item4079996329", "Painted Black Silicon")
        categorynode52215.Nodes.Add("item4079996329", "Painted Dark Gray Silicon")
        categorynode52215.Nodes.Add("item4079996329", "Painted Gray Silicon")
        categorynode52215.Nodes.Add("item4079996329", "Painted Green Silicon")
        categorynode52215.Nodes.Add("item4079996329", "Painted Ice Silicon")
        categorynode52215.Nodes.Add("item4079996329", "Painted Light Gray Silicon")
        categorynode52215.Nodes.Add("item4079996329", "Painted Military Silicon")
        categorynode52215.Nodes.Add("item4079996329", "Painted Orange Silicon")
        categorynode52215.Nodes.Add("item4079996329", "Painted Purple Silicon")
        categorynode52215.Nodes.Add("item4079996329", "Military Silicon Panel")
        categorynode52215.Nodes.Add("item4079996329", "Light Gray Silicon Panel")
        categorynode52215.Nodes.Add("item4079996329", "Ice Silicon Panel")
        categorynode52215.Nodes.Add("item2013004922", "Painted Light Gray Sodium")
        categorynode52215.Nodes.Add("item4079996329", "Green Silicon Panel")
        categorynode52215.Nodes.Add("item4079996329", "Dark Gray Silicon Panel")
        categorynode52215.Nodes.Add("item4079996329", "Black Silicon Panel")
        categorynode52215.Nodes.Add("item4079996329", "Painted Yellow Silicon")
        categorynode52215.Nodes.Add("item4079996329", "Painted White Silicon")
        categorynode52215.Nodes.Add("item4079996329", "Painted Sky Silicon")
        categorynode52215.Nodes.Add("item4079996329", "Painted Red Silicon")
        categorynode52215.Nodes.Add("item4079996329", "Gray Silicon Panel")
        categorynode52215.Nodes.Add("item4079996329", "Orange Silicon Panel")

        Dim categorynode52216 As TreeNode = categorynode522.Nodes.Add("node52216", "Silver Honeycomb")
        categorynode52216.Nodes.Add("item3760652609", "Painted Ice Silver")
        categorynode52216.Nodes.Add("item3760652609", "Painted Light Gray Silver")
        categorynode52216.Nodes.Add("item3760652609", "Painted Military Silver")
        categorynode52216.Nodes.Add("item3760652609", "Painted Orange Silver")
        categorynode52216.Nodes.Add("item3760652609", "Painted Purple Silver")
        categorynode52216.Nodes.Add("item3760652609", "Painted Red Silver")
        categorynode52216.Nodes.Add("item3760652609", "Painted Sky Silver")
        categorynode52216.Nodes.Add("item3760652609", "Painted White Silver")
        categorynode52216.Nodes.Add("item3760652609", "Painted Yellow Silver")
        categorynode52216.Nodes.Add("item3760652609", "Black Silver Panel")
        categorynode52216.Nodes.Add("item3760652609", "Dark Gray Silver Panel")
        categorynode52216.Nodes.Add("item3760652609", "Gray Silver Panel")
        categorynode52216.Nodes.Add("item3760652609", "Green Silver Panel")
        categorynode52216.Nodes.Add("item3760652609", "Ice Silver Panel")
        categorynode52216.Nodes.Add("item3760652609", "Painted Green Silver")
        categorynode52216.Nodes.Add("item3760652609", "Light Gray Silver Panel")
        categorynode52216.Nodes.Add("item3760652609", "Orange Silver Panel")
        categorynode52216.Nodes.Add("item3760652609", "Purple Silver Panel")
        categorynode52216.Nodes.Add("item3760652609", "Red Silver Panel")
        categorynode52216.Nodes.Add("item3760652609", "Sky Silver Panel")
        categorynode52216.Nodes.Add("item3760652609", "White Silver Panel")
        categorynode52216.Nodes.Add("item3760652609", "Yellow Silver Panel")
        categorynode52216.Nodes.Add("item3760652609", "Military Silver Panel")
        categorynode52216.Nodes.Add("item3760652609", "Painted Gray Silver")
        categorynode52216.Nodes.Add("item3760652609", "Painted Dark Gray Silver")
        categorynode52216.Nodes.Add("item3760652609", "Painted Black Silver")
        categorynode52216.Nodes.Add("item3760652609", "Matte Silver")
        categorynode52216.Nodes.Add("item3760652609", "Glossy Silver")
        categorynode52216.Nodes.Add("item3760652609", "Aged Silver")

        Dim categorynode52217 As TreeNode = categorynode522.Nodes.Add("node52217", "Sodium Honeycomb")
        categorynode52217.Nodes.Add("item2013004922", "Aged Sodium")
        categorynode52217.Nodes.Add("item2013004922", "Glossy Sodium")
        categorynode52217.Nodes.Add("item2013004922", "Matte Sodium")
        categorynode52217.Nodes.Add("item2013004922", "Painted Black Sodium")
        categorynode52217.Nodes.Add("item2013004922", "Painted Dark Gray Sodium")
        categorynode52217.Nodes.Add("item2013004922", "Painted Gray Sodium")
        categorynode52217.Nodes.Add("item2013004922", "Painted Green Sodium")
        categorynode52217.Nodes.Add("item2013004922", "Painted Ice Sodium")
        categorynode52217.Nodes.Add("item2013004922", "Painted Military Sodium")
        categorynode52217.Nodes.Add("item2013004922", "Painted Purple Sodium")
        categorynode52217.Nodes.Add("item2013004922", "Painted Red Sodium")
        categorynode52217.Nodes.Add("item2013004922", "Painted Sky Sodium")
        categorynode52217.Nodes.Add("item2013004922", "Painted White Sodium")
        categorynode52217.Nodes.Add("item2013004922", "Painted Yellow Sodium")
        categorynode52217.Nodes.Add("item2013004922", "Black Sodium Panel")
        categorynode52217.Nodes.Add("item2013004922", "Dark Gray Sodium Panel")
        categorynode52217.Nodes.Add("item2013004922", "Gray Sodium Panel")
        categorynode52217.Nodes.Add("item2013004922", "Green Sodium Panel")
        categorynode52217.Nodes.Add("item2013004922", "Ice Sodium Panel")
        categorynode52217.Nodes.Add("item2013004922", "Light Gray Sodium Panel")
        categorynode52217.Nodes.Add("item2013004922", "Military Sodium Panel")
        categorynode52217.Nodes.Add("item2013004922", "Orange Sodium Panel")
        categorynode52217.Nodes.Add("item2013004922", "Purple Sodium Panel")
        categorynode52217.Nodes.Add("item2013004922", "Red Sodium Panel")
        categorynode52217.Nodes.Add("item2013004922", "Sky Sodium Panel")
        categorynode52217.Nodes.Add("item2013004922", "White Sodium Panel")
        categorynode52217.Nodes.Add("item2013004922", "Yellow Sodium Panel")
        categorynode52217.Nodes.Add("item2013004922", "Painted Orange Sodium")

        Dim categorynode52218 As TreeNode = categorynode522.Nodes.Add("node52218", "Sulfur Honeycomb")
        categorynode52218.Nodes.Add("item1519873395", "Gray Sulfur Panel")
        categorynode52218.Nodes.Add("item1519873395", "Green Sulfur Panel")
        categorynode52218.Nodes.Add("item1519873395", "Ice Sulfur Panel")
        categorynode52218.Nodes.Add("item1519873395", "Light Gray Sulfur Panel")
        categorynode52218.Nodes.Add("item1519873395", "Military Sulfur Panel")
        categorynode52218.Nodes.Add("item1519873395", "Orange Sulfur Panel")
        categorynode52218.Nodes.Add("item1519873395", "Purple Sulfur Panel")
        categorynode52218.Nodes.Add("item1519873395", "Red Sulfur Panel")
        categorynode52218.Nodes.Add("item1519873395", "Sky Sulfur Panel")
        categorynode52218.Nodes.Add("item1519873395", "White Sulfur Panel")
        categorynode52218.Nodes.Add("item1519873395", "Yellow Sulfur Panel")
        categorynode52218.Nodes.Add("item1519873395", "Dark Gray Sulfur Panel")
        categorynode52218.Nodes.Add("item1519873395", "Black Sulfur Panel")
        categorynode52218.Nodes.Add("item1519873395", "Painted Yellow Sulfur")
        categorynode52218.Nodes.Add("item1519873395", "Painted White Sulfur")
        categorynode52218.Nodes.Add("item1519873395", "Painted Sky Sulfur")
        categorynode52218.Nodes.Add("item1519873395", "Painted Red Sulfur")
        categorynode52218.Nodes.Add("item1519873395", "Painted Purple Sulfur")
        categorynode52218.Nodes.Add("item1519873395", "Painted Orange Sulfur")
        categorynode52218.Nodes.Add("item1519873395", "Painted Military Sulfur")
        categorynode52218.Nodes.Add("item1519873395", "Painted Light Gray Sulfur")
        categorynode52218.Nodes.Add("item1519873395", "Painted Ice Sulfur")
        categorynode52218.Nodes.Add("item1519873395", "Painted Gray Sulfur")
        categorynode52218.Nodes.Add("item1519873395", "Painted Dark Gray Sulfur")
        categorynode52218.Nodes.Add("item1519873395", "Painted Black Sulfur")
        categorynode52218.Nodes.Add("item1519873395", "Matte Sulfur")
        categorynode52218.Nodes.Add("item1519873395", "Glossy Sulfur")
        categorynode52218.Nodes.Add("item1519873395", "Aged Sulfur")
        categorynode52218.Nodes.Add("item1519873395", "Painted Green Sulfur")

        Dim categorynode52219 As TreeNode = categorynode522.Nodes.Add("node52219", "Titanium Honeycomb")
        categorynode52219.Nodes.Add("item402511494", "Aged Titanium")
        categorynode52219.Nodes.Add("item402511494", "Glossy Titanium")
        categorynode52219.Nodes.Add("item402511494", "Matte Titanium")
        categorynode52219.Nodes.Add("item402511494", "Painted Black Titanium")
        categorynode52219.Nodes.Add("item402511494", "Painted Gray Titanium")
        categorynode52219.Nodes.Add("item402511494", "Painted Green Titanium")
        categorynode52219.Nodes.Add("item402511494", "Painted Ice Titanium")
        categorynode52219.Nodes.Add("item402511494", "Painted Light Gray Titanium")
        categorynode52219.Nodes.Add("item402511494", "Painted Military Titanium")
        categorynode52219.Nodes.Add("item402511494", "Painted Orange Titanium")
        categorynode52219.Nodes.Add("item402511494", "Painted Purple Titanium")
        categorynode52219.Nodes.Add("item402511494", "Painted Red Titanium")
        categorynode52219.Nodes.Add("item402511494", "Painted Sky Titanium")
        categorynode52219.Nodes.Add("item402511494", "Painted White Titanium")
        categorynode52219.Nodes.Add("item402511494", "Painted Yellow Titanium")
        categorynode52219.Nodes.Add("item402511494", "Black Titanium Panel")
        categorynode52219.Nodes.Add("item402511494", "Dark Gray Titanium Panel")
        categorynode52219.Nodes.Add("item402511494", "Gray Titanium Panel")
        categorynode52219.Nodes.Add("item402511494", "Painted Dark Gray Titanium")
        categorynode52219.Nodes.Add("item402511494", "Green Titanium Panel")
        categorynode52219.Nodes.Add("item402511494", "Ice Titanium Panel")
        categorynode52219.Nodes.Add("item402511494", "Military Titanium Panel")
        categorynode52219.Nodes.Add("item402511494", "Light Gray Titanium Panel")
        categorynode52219.Nodes.Add("item402511494", "Orange Titanium Panel")
        categorynode52219.Nodes.Add("item402511494", "Purple Titanium Panel")
        categorynode52219.Nodes.Add("item402511494", "Red Titanium Panel")
        categorynode52219.Nodes.Add("item402511494", "Sky Titanium Panel")
        categorynode52219.Nodes.Add("item402511494", "White Titanium Panel")
        categorynode52219.Nodes.Add("item402511494", "Yellow Titanium Panel")
        categorynode52219.Nodes.Add("item402511494", "White pattern Titanium")
        categorynode52219.Nodes.Add("item402511494", "Stained gray pattern Titanium")
        categorynode52219.Nodes.Add("item402511494", "Aged gray pattern Titanium")
        categorynode52219.Nodes.Add("item402511494", "Gray pattern Titanium")
        categorynode52219.Nodes.Add("item402511494", "Black pattern Titanium")
        categorynode52219.Nodes.Add("item402511494", "Galvanized white Titanium")
        categorynode52219.Nodes.Add("item402511494", "Galvanized light gray Titanium")
        categorynode52219.Nodes.Add("item402511494", "Galvanized dark gray Titanium")
        categorynode52219.Nodes.Add("item402511494", "Galvanized gray Titanium")
        categorynode52219.Nodes.Add("item402511494", "Galvanized black Titanium")
        categorynode52219.Nodes.Add("item402511494", "Polished white Titanium")
        categorynode52219.Nodes.Add("item402511494", "Polished light gray Titanium")
        categorynode52219.Nodes.Add("item402511494", "Polished dark gray Titanium")
        categorynode52219.Nodes.Add("item402511494", "Polished gray Titanium")
        categorynode52219.Nodes.Add("item402511494", "Polished black Titanium")

        Dim categorynode52220 As TreeNode = categorynode522.Nodes.Add("node52220", "Vanadium Honeycomb")
        categorynode52220.Nodes.Add("item1605580774", "Painted White Vanadium")
        categorynode52220.Nodes.Add("item1605580774", "Painted Yellow Vanadium")
        categorynode52220.Nodes.Add("item1605580774", "Black Vanadium Panel")
        categorynode52220.Nodes.Add("item1605580774", "Dark Gray Vanadium Panel")
        categorynode52220.Nodes.Add("item1605580774", "Gray Vanadium Panel")
        categorynode52220.Nodes.Add("item1605580774", "Green Vanadium Panel")
        categorynode52220.Nodes.Add("item1605580774", "Ice Vanadium Panel")
        categorynode52220.Nodes.Add("item1605580774", "Light Gray Vanadium Panel")
        categorynode52220.Nodes.Add("item1605580774", "Military Vanadium Panel")
        categorynode52220.Nodes.Add("item1605580774", "Orange Vanadium Panel")
        categorynode52220.Nodes.Add("item1605580774", "Purple Vanadium Panel")
        categorynode52220.Nodes.Add("item1605580774", "Red Vanadium Panel")
        categorynode52220.Nodes.Add("item1605580774", "Sky Vanadium Panel")
        categorynode52220.Nodes.Add("item1605580774", "White Vanadium Panel")
        categorynode52220.Nodes.Add("item1605580774", "Painted Sky Vanadium")
        categorynode52220.Nodes.Add("item1605580774", "Yellow Vanadium Panel")
        categorynode52220.Nodes.Add("item1605580774", "Painted Red Vanadium")
        categorynode52220.Nodes.Add("item1605580774", "Painted Purple Vanadium")
        categorynode52220.Nodes.Add("item1605580774", "Painted Orange Vanadium")
        categorynode52220.Nodes.Add("item1605580774", "Painted Military Vanadium")
        categorynode52220.Nodes.Add("item1605580774", "Painted Light Gray Vanadium")
        categorynode52220.Nodes.Add("item1605580774", "Painted Ice Vanadium")
        categorynode52220.Nodes.Add("item1605580774", "Painted Green Vanadium")
        categorynode52220.Nodes.Add("item1605580774", "Painted Gray Vanadium")
        categorynode52220.Nodes.Add("item1605580774", "Painted Dark Gray Vanadium")
        categorynode52220.Nodes.Add("item1605580774", "Painted Black Vanadium")
        categorynode52220.Nodes.Add("item1605580774", "Glossy Vanadium")
        categorynode52220.Nodes.Add("item1605580774", "Aged Vanadium")
        categorynode52220.Nodes.Add("item1605580774", "Matte Vanadium")

        Dim categorynode53 As TreeNode = categorynode5.Nodes.Add("node53", "Minable  materials")
        Dim categorynode531 As TreeNode = categorynode53.Nodes.Add("node531", "Ore")
        Dim categorynode5311 As TreeNode = categorynode531.Nodes.Add("node5311", "Tier 1 Ores")
        categorynode5311.Nodes.Add("item262147665", "Bauxite")
        categorynode5311.Nodes.Add("item299255727", "Coal")
        categorynode5311.Nodes.Add("item4234772167", "Hematite")
        categorynode5311.Nodes.Add("item3724036288", "Quartz")

        Dim categorynode5312 As TreeNode = categorynode531.Nodes.Add("node5312", "Tier 2 Ores")
        categorynode5312.Nodes.Add("item2029139010", "Chromite")
        categorynode5312.Nodes.Add("item3086347393", "Limestone")
        categorynode5312.Nodes.Add("item2289641763", "Malachite")
        categorynode5312.Nodes.Add("item343766315", "Natron")

        Dim categorynode5313 As TreeNode = categorynode531.Nodes.Add("node5313", "Tier 3 Ores")
        categorynode5313.Nodes.Add("item1050500112", "Acanthite")
        categorynode5313.Nodes.Add("item1065079614", "Garnierite")
        categorynode5313.Nodes.Add("item3837858336", "Petalite")
        categorynode5313.Nodes.Add("item4041459743", "Pyrite")

        Dim categorynode5314 As TreeNode = categorynode531.Nodes.Add("node5314", "Tier 4 Ores")
        categorynode5314.Nodes.Add("item3546085401", "Cobaltite")
        categorynode5314.Nodes.Add("item1467310917", "Cryolite")
        categorynode5314.Nodes.Add("item1866812055", "Gold nuggets")
        categorynode5314.Nodes.Add("item271971371", "Kolbeckite")

        Dim categorynode5315 As TreeNode = categorynode531.Nodes.Add("node5315", "Tier 5 Ores")
        categorynode5315.Nodes.Add("item789110817", "Columbite")
        categorynode5315.Nodes.Add("item629636034", "Illmenite")
        categorynode5315.Nodes.Add("item3934774987", "Rhodonite")
        categorynode5315.Nodes.Add("item1652978615", "Thoramine")
        categorynode5315.Nodes.Add("item2162350405", "Vanadinite")

        Dim categorynode532 As TreeNode = categorynode53.Nodes.Add("node532", "Soils")
        ' Soils are missing
        ' Thanks

        Dim categorynode54 As TreeNode = categorynode5.Nodes.Add("node54", "Refined Materials")
        Dim categorynode541 As TreeNode = categorynode54.Nodes.Add("node541", "Catalyst")
        categorynode541.Nodes.Add("item3729464848", "Catalyst 3")
        categorynode541.Nodes.Add("item3729464849", "Catalyst 4")
        categorynode541.Nodes.Add("item3729464850", "Catalyst 5")

        Dim categorynode542 As TreeNode = categorynode54.Nodes.Add("node542", "Product")
        categorynode542.Nodes.Add("item2646210914", "Biological matter product")
        categorynode542.Nodes.Add("item2679709617", "Brick Honeycomb")
        categorynode542.Nodes.Add("item1622880428", "Carbon Fiber product")
        categorynode542.Nodes.Add("item645870905", "Concrete product")

        Dim categorynode5421 As TreeNode = categorynode542.Nodes.Add("node5421", "Conductor Metals")
        categorynode5421.Nodes.Add("item18262914", "Al-Fe Alloy product")
        categorynode5421.Nodes.Add("item1034957327", "Calcium Reinforced Copper product")
        categorynode5421.Nodes.Add("item1673011820", "Cu-Ag Alloy product")
        categorynode5421.Nodes.Add("item2550840787", "Red Gold Product")
        categorynode5421.Nodes.Add("item1734893264", "Ti-Nb Supraconductor product")

        Dim categorynode5422 As TreeNode = categorynode542.Nodes.Add("node5422", "Glass materials")
        categorynode5422.Nodes.Add("item1942154251", "Advanced glass product")
        categorynode5422.Nodes.Add("item2301749833", "Ag-Li Reinforced glass product")
        categorynode5422.Nodes.Add("item3308209457", "Glass product")
        categorynode5422.Nodes.Add("item606249095", "Gold-Coated glass product")
        categorynode5422.Nodes.Add("item4150961531", "Manganese Reinforced glass product")

        Dim categorynode5423 As TreeNode = categorynode542.Nodes.Add("node5423", "Heavy Metals")
        categorynode5423.Nodes.Add("item167908167", "Inconel product")
        categorynode5423.Nodes.Add("item3987872305", "Mangalloy product")
        categorynode5423.Nodes.Add("item3518490274", "Maraging Steel product")
        categorynode5423.Nodes.Add("item2984358477", "Stainless Steel product")
        categorynode5423.Nodes.Add("item511774178", "Steel product")

        Dim categorynode5424 As TreeNode = categorynode542.Nodes.Add("node5424", "Light Metals")
        categorynode5424.Nodes.Add("item2021406770", "Al-Li Alloy product")
        categorynode5424.Nodes.Add("item231758472", "Duralumin product")
        categorynode5424.Nodes.Add("item3292291904", "Grade 5 Titanium Alloy product")
        categorynode5424.Nodes.Add("item2929462635", "Sc-Al Alloy Product")
        categorynode5424.Nodes.Add("item2565702107", "Silumin Product")

        categorynode542.Nodes.Add("item331532952", "Marble product")

        Dim categorynode5425 As TreeNode = categorynode542.Nodes.Add("node5425", "Polymers")
        categorynode5425.Nodes.Add("item918590356", "Fluoropolymer product")
        categorynode5425.Nodes.Add("item4103265826", "Polycalcite plastic product")
        categorynode5425.Nodes.Add("item2014531313", "Polycarbonate plastic product")
        categorynode5425.Nodes.Add("item2097691217", "Polysulfide plastic product")
        categorynode5425.Nodes.Add("item255776324", "Vanamer product")

        categorynode542.Nodes.Add("item770773323", "Wood product")

        Dim categorynode543 As TreeNode = categorynode54.Nodes.Add("node543", "Pure")
        Dim categorynode5431 As TreeNode = categorynode543.Nodes.Add("node5431", "Advanced Pure Material")
        categorynode5431.Nodes.Add("item3810111622", "Pure Lithium")
        categorynode5431.Nodes.Add("item3012303017", "Pure Nickel")
        categorynode5431.Nodes.Add("item1807690770", "Pure Silver")
        categorynode5431.Nodes.Add("item3822811562", "Pure Sulfur")

        Dim categorynode5432 As TreeNode = categorynode543.Nodes.Add("node5432", "Basic Pure Material")
        categorynode5432.Nodes.Add("item2240749601", "Pure Aluminium")
        categorynode5432.Nodes.Add("item159858782", "Pure Carbon")
        categorynode5432.Nodes.Add("item198782496", "Pure Iron")
        categorynode5432.Nodes.Add("item2589986891", "Pure Silicon")

        Dim categorynode5433 As TreeNode = categorynode543.Nodes.Add("node5433", "Exotic Pure Material")
        categorynode5433.Nodes.Add("item2421303625", "Pure manganese")
        categorynode5433.Nodes.Add("item1126600143", "Pure Niobium")
        categorynode5433.Nodes.Add("item752542080", "Pure Titanium")
        categorynode5433.Nodes.Add("item2007627267", "Pure Vanadium")

        categorynode543.Nodes.Add("item1010524904", "Pure Hydrogen")
        categorynode543.Nodes.Add("item947806142", "Pure Oxygen")
        Dim categorynode5434 As TreeNode = categorynode543.Nodes.Add("node5434", "Rare Pure Material")
        categorynode5434.Nodes.Add("item2031444137", "Pure Cobalt")
        categorynode5434.Nodes.Add("item3323724376", "Pure Fluorine")
        categorynode5434.Nodes.Add("item3837955371", "Pure Gold")
        categorynode5434.Nodes.Add("item3211418846", "Pure Scandium")

        Dim categorynode5435 As TreeNode = categorynode543.Nodes.Add("node5435", "Uncommon Pure Material")
        categorynode5435.Nodes.Add("item2112763718", "Pure Calcium")
        categorynode5435.Nodes.Add("item2147954574", "Pure Chromium")
        categorynode5435.Nodes.Add("item1466453887", "Pure Copper")
        categorynode5435.Nodes.Add("item3603734543", "Pure Sodium")

        Dim categorynode6 As TreeNode = ItemTree.Nodes.Add("node6", "Parts")
        Dim categorynode61 As TreeNode = categorynode6.Nodes.Add("node61", "Complex parts")
        Dim categorynode611 As TreeNode = categorynode61.Nodes.Add("node611", "Antimatter capsules")
        categorynode611.Nodes.Add("item3661595540", "Advanced Antimatter Capsule")
        categorynode611.Nodes.Add("item3661595538", "Basic Antimatter Capsule")
        categorynode611.Nodes.Add("item3661595539", "Uncommon Antimatter Capsule")

        Dim categorynode612 As TreeNode = categorynode61.Nodes.Add("node612", "Burners")
        categorynode612.Nodes.Add("item2660328734", "Advanced Burner")
        categorynode612.Nodes.Add("item2660328728", "Basic Burner")
        categorynode612.Nodes.Add("item2660328732", "Exotic Burner")
        categorynode612.Nodes.Add("item2660328735", "Rare Burner")
        categorynode612.Nodes.Add("item2660328729", "Uncommon Burner")

        Dim categorynode613 As TreeNode = categorynode61.Nodes.Add("node613", "Electronics")
        categorynode613.Nodes.Add("item1297540452", "Advanced Electronics")
        categorynode613.Nodes.Add("item1297540454", "Basic Electronics")
        categorynode613.Nodes.Add("item1297540450", "Exotic Electronics")
        categorynode613.Nodes.Add("item1297540451", "Rare Electronics")
        categorynode613.Nodes.Add("item1297540453", "Uncommon Electronics")

        Dim categorynode614 As TreeNode = categorynode61.Nodes.Add("node614", "Explosive Modules")
        categorynode614.Nodes.Add("item2541811484", "Advanced Explosive Module")
        categorynode614.Nodes.Add("item2541811486", "Basic Explosive Module")
        categorynode614.Nodes.Add("item2541811485", "Uncommon Explosive Module")

        Dim categorynode615 As TreeNode = categorynode61.Nodes.Add("node615", "Hydraulics")
        categorynode615.Nodes.Add("item1331181089", "Advanced hydraulics")
        categorynode615.Nodes.Add("item1331181119", "Basic hydraulics")
        categorynode615.Nodes.Add("item1331181091", "Exotic hydraulics")
        categorynode615.Nodes.Add("item1331181088", "Rare hydraulics")
        categorynode615.Nodes.Add("item1331181118", "Uncommon hydraulics")

        Dim categorynode616 As TreeNode = categorynode61.Nodes.Add("node616", "Injectors")
        categorynode616.Nodes.Add("item1971447078", "Advanced Injector")
        categorynode616.Nodes.Add("item1971447072", "Basic Injector")
        categorynode616.Nodes.Add("item1971447079", "Rare Injector")
        categorynode616.Nodes.Add("item1971447073", "Uncommon Injector")

        Dim categorynode617 As TreeNode = categorynode61.Nodes.Add("node617", "Magnets")
        categorynode617.Nodes.Add("item1246524876", "Advanced Magnet")
        categorynode617.Nodes.Add("item1246524878", "Basic Magnet")
        categorynode617.Nodes.Add("item1246524866", "Exotic Magnet")
        categorynode617.Nodes.Add("item1246524877", "Rare Magnet")
        categorynode617.Nodes.Add("item1246524879", "Uncommon Magnet")

        Dim categorynode618 As TreeNode = categorynode61.Nodes.Add("node618", "Optics")
        categorynode618.Nodes.Add("item3739200049", "Advanced Optics")
        categorynode618.Nodes.Add("item3739200051", "Basic Optics")
        categorynode618.Nodes.Add("item3739200055", "Exotic Optics")
        categorynode618.Nodes.Add("item3739200048", "Rare Optics")
        categorynode618.Nodes.Add("item3739200050", "Uncommon Optics")

        Dim categorynode619 As TreeNode = categorynode61.Nodes.Add("node619", "Power Systems")
        categorynode619.Nodes.Add("item527681753", "Advanced Power System")
        categorynode619.Nodes.Add("item527681755", "Basic Power System")
        categorynode619.Nodes.Add("item527681751", "Exotic Power System")
        categorynode619.Nodes.Add("item527681750", "Rare Power System")
        categorynode619.Nodes.Add("item527681752", "Uncommon Power System")

        Dim categorynode6110 As TreeNode = categorynode61.Nodes.Add("node6110", "Processors")
        categorynode6110.Nodes.Add("item3808417020", "Advanced Processor")
        categorynode6110.Nodes.Add("item3808417022", "Basic Processor")
        categorynode6110.Nodes.Add("item3808417021", "Uncommon Processor")

        Dim categorynode6111 As TreeNode = categorynode61.Nodes.Add("node6111", "Quantum Cores")
        categorynode6111.Nodes.Add("item850241764", "Advanced Quantum Core")
        categorynode6111.Nodes.Add("item850241766", "Basic Quantum Core")
        categorynode6111.Nodes.Add("item850241762", "Exotic Quantum Core")
        categorynode6111.Nodes.Add("item850241763", "Rare Quantum Core")
        categorynode6111.Nodes.Add("item850241765", "Uncommon Quantum Core")

        Dim categorynode6112 As TreeNode = categorynode61.Nodes.Add("node6112", "Singularity Containers")
        categorynode6112.Nodes.Add("item3640212312", "Advanced Singularity Container")
        categorynode6112.Nodes.Add("item3640212318", "Basic Singularity Container")
        categorynode6112.Nodes.Add("item3640212314", "Exotic Singularity Container")
        categorynode6112.Nodes.Add("item3640212315", "Rare Singularity Container")
        categorynode6112.Nodes.Add("item3640212313", "Uncommon Singularity Container")

        Dim categorynode6113 As TreeNode = categorynode61.Nodes.Add("node6113", "Solid Warheads")
        categorynode6113.Nodes.Add("item2599686739", "Advanced Solid Warhead")
        categorynode6113.Nodes.Add("item2599686738", "Uncommon Solid Warhead")

        Dim categorynode62 As TreeNode = categorynode6.Nodes.Add("node62", "Exceptional parts")
        Dim categorynode621 As TreeNode = categorynode62.Nodes.Add("node621", "Anti-gravity cores")
        categorynode621.Nodes.Add("item2999509666", "Advanced Anti-Gravity Core")
        categorynode621.Nodes.Add("item2999509692", "Exotic Anti-Gravity Core")
        categorynode621.Nodes.Add("item2999509693", "Rare Anti-Gravity Core")

        Dim categorynode622 As TreeNode = categorynode62.Nodes.Add("node622", "Antimatter cores")
        categorynode622.Nodes.Add("item375744325", "Advanced Antimatter Core")

        Dim categorynode623 As TreeNode = categorynode62.Nodes.Add("node623", "Quantum Alignment Units")
        categorynode623.Nodes.Add("item2601646636", "Advanced Quantum Alignment Unit")
        categorynode623.Nodes.Add("item2601646634", "Exotic Quantum Alignment Unit")
        categorynode623.Nodes.Add("item2601646635", "Rare Quantum Alignment Unit")

        Dim categorynode624 As TreeNode = categorynode62.Nodes.Add("node624", "Quantum Barriers")
        categorynode624.Nodes.Add("item984088007", "Advanced Quantum Barrier")

        Dim categorynode63 As TreeNode = categorynode6.Nodes.Add("node63", "Functional parts")
        categorynode63.Nodes.Add("item2796831846", "Advanced Structural Parts")

        Dim categorynode631 As TreeNode = categorynode63.Nodes.Add("node631", "Antennas")
        categorynode631.Nodes.Add("item1080827550", "Advanced Antenna S")
        categorynode631.Nodes.Add("item1080827544", "Advanced Antenna M")
        categorynode631.Nodes.Add("item1080827527", "Advanced Antenna L")
        categorynode631.Nodes.Add("item2302031898", "Advanced Antenna XL")
        categorynode631.Nodes.Add("item2301991330", "Basic Antenna XS")
        categorynode631.Nodes.Add("item1080826905", "Basic Antenna S")
        categorynode631.Nodes.Add("item2301991355", "Basic Antenna XL")
        categorynode631.Nodes.Add("item1080827676", "Exotic Antenna S")
        categorynode631.Nodes.Add("item1080827674", "Exotic Antenna M")
        categorynode631.Nodes.Add("item1080827653", "Exotic Antenna L")
        categorynode631.Nodes.Add("item2302040376", "Exotic Antenna XL")
        categorynode631.Nodes.Add("item1080827741", "Rare Antenna S")
        categorynode631.Nodes.Add("item1080827739", "Rare Antenna M")
        categorynode631.Nodes.Add("item1080827716", "Rare Antenna L")
        categorynode631.Nodes.Add("item2302027954", "Uncommon Antenna XS")
        categorynode631.Nodes.Add("item1080827615", "Uncommon Antenna S")
        categorynode631.Nodes.Add("item1080827609", "Uncommon Antenna M")
        categorynode631.Nodes.Add("item1080827590", "Uncommon Antenna L")

        categorynode63.Nodes.Add("item2796831840", "Basic Structural Parts")

        Dim categorynode632 As TreeNode = categorynode63.Nodes.Add("node632", "Chemical containers")
        categorynode632.Nodes.Add("item625115345", "Advanced Chemical Container S")
        categorynode632.Nodes.Add("item625115179", "Advanced Chemical Container M")
        categorynode632.Nodes.Add("item625115176", "Advanced Chemical Container L")
        categorynode632.Nodes.Add("item3714764686", "Advanced Chemical Container XL")
        categorynode632.Nodes.Add("item3717621915", "Basic Chemical Container XS")
        categorynode632.Nodes.Add("item625289720", "Basic Chemical Container S")
        categorynode632.Nodes.Add("item625289726", "Basic Chemical Container M")
        categorynode632.Nodes.Add("item625289727", "Basic Chemical Container L")
        categorynode632.Nodes.Add("item3717621906", "Basic Chemical Container XL")
        categorynode632.Nodes.Add("item625115242", "Rare Chemical Container M")
        categorynode632.Nodes.Add("item625289663", "Uncommon Chemical Container M")

        Dim categorynode633 As TreeNode = categorynode63.Nodes.Add("node633", "Combustion Chambers")
        categorynode633.Nodes.Add("item4016322616", "Advanced Combustion Chamber XS")
        categorynode633.Nodes.Add("item2662310081", "Advanced Combustion Chamber S")
        categorynode633.Nodes.Add("item2662310087", "Advanced Combustion Chamber M")
        categorynode633.Nodes.Add("item2662310086", "Advanced Combustion Chamber L")
        categorynode633.Nodes.Add("item4017996241", "Basic Combustion Chamber XS")
        categorynode633.Nodes.Add("item2662317132", "Basic Combustion Chamber S")
        categorynode633.Nodes.Add("item2662317126", "Basic Combustion Chamber M")
        categorynode633.Nodes.Add("item2662317125", "Basic Combustion Chamber L")
        categorynode633.Nodes.Add("item4016318475", "Rare Combustion Chamber XS")
        categorynode633.Nodes.Add("item2662310018", "Rare Combustion Chamber S")
        categorynode633.Nodes.Add("item2662310020", "Rare Combustion Chamber M")
        categorynode633.Nodes.Add("item2662310021", "Rare Combustion Chamber L")
        categorynode633.Nodes.Add("item4016359657", "Uncommon Combustion Chamber XS")
        categorynode633.Nodes.Add("item2662309888", "Uncommon Combustion Chamber S")
        categorynode633.Nodes.Add("item2662309894", "Uncommon Combustion Chamber M")
        categorynode633.Nodes.Add("item2662309895", "Uncommon Combustion Chamber L")

        Dim categorynode634 As TreeNode = categorynode63.Nodes.Add("node634", "Control systems")
        categorynode634.Nodes.Add("item3431996625", "Advanced Control System S")
        categorynode634.Nodes.Add("item3431996639", "Advanced Control System M")
        categorynode634.Nodes.Add("item3431996632", "Advanced Control System L")
        categorynode634.Nodes.Add("item972195890", "Basic Control System XS")
        categorynode634.Nodes.Add("item3431996502", "Basic Control System S")
        categorynode634.Nodes.Add("item3431996504", "Basic Control System M")

        Dim categorynode635 As TreeNode = categorynode63.Nodes.Add("node635", "Core Systems")
        categorynode635.Nodes.Add("item1775106556", "Advanced Core System M")
        categorynode635.Nodes.Add("item1172598456", "Basic Core System XS")
        categorynode635.Nodes.Add("item1775106685", "Basic Core System S")
        categorynode635.Nodes.Add("item1775106424", "Exotic Core System S")
        categorynode635.Nodes.Add("item1775106492", "Rare Core System L")
        categorynode635.Nodes.Add("item1775106620", "Uncommon Core System S")
        categorynode635.Nodes.Add("item1775106618", "Uncommon Core System M")
        categorynode635.Nodes.Add("item1775106597", "Uncommon Core System L")

        Dim categorynode636 As TreeNode = categorynode63.Nodes.Add("node636", "Electric engines")
        categorynode636.Nodes.Add("item3728054834", "Basic Electric Engine S")
        categorynode636.Nodes.Add("item3728054836", "Basic Electric Engine M")
        categorynode636.Nodes.Add("item3172866509", "Uncommon Electric Engine XL")

        categorynode63.Nodes.Add("item2796831844", "Exotic Structural Parts")

        Dim categorynode637 As TreeNode = categorynode63.Nodes.Add("node637", "Firing Systems")
        categorynode637.Nodes.Add("item3740021214", "Advanced Firing System XS")
        categorynode637.Nodes.Add("item3242491986", "Advanced Firing System S")
        categorynode637.Nodes.Add("item3242491976", "Advanced Firing System M")
        categorynode637.Nodes.Add("item3242491977", "Advanced Firing System L")
        categorynode637.Nodes.Add("item3740092443", "Basic Firing System XS")
        categorynode637.Nodes.Add("item3740078396", "Exotic Firing System XS")
        categorynode637.Nodes.Add("item3242492880", "Exotic Firing System S")
        categorynode637.Nodes.Add("item3242492874", "Exotic Firing System M")
        categorynode637.Nodes.Add("item3242492875", "Exotic Firing System L")
        categorynode637.Nodes.Add("item3740074253", "Rare Firing System XS")
        categorynode637.Nodes.Add("item3242492817", "Rare Firing System S")
        categorynode637.Nodes.Add("item3242492811", "Rare Firing System M")
        categorynode637.Nodes.Add("item3242492810", "Rare Firing System L")

        Dim categorynode638 As TreeNode = categorynode63.Nodes.Add("node638", "Gas Cylinders")
        categorynode638.Nodes.Add("item792299450", "Basic Gas Cylinder XS")
        categorynode638.Nodes.Add("item2119086146", "Basic Gas Cylinder S")
        categorynode638.Nodes.Add("item2119086168", "Basic Gas Cylinder M")

        Dim categorynode639 As TreeNode = categorynode63.Nodes.Add("node639", "Ionic chambers")
        categorynode639.Nodes.Add("item962704747", "Advanced Ionic Chamber XS")
        categorynode639.Nodes.Add("item1390563262", "Advanced Ionic Chamber S")
        categorynode639.Nodes.Add("item1390563256", "Advanced Ionic Chamber M")
        categorynode639.Nodes.Add("item1390563239", "Advanced Ionic Chamber L")
        categorynode639.Nodes.Add("item962704738", "Advanced Ionic Chamber XL")
        categorynode639.Nodes.Add("item962712586", "Basic Ionic Chamber XS")
        categorynode639.Nodes.Add("item1390562873", "Basic Ionic Chamber S")
        categorynode639.Nodes.Add("item1390562879", "Basic Ionic Chamber M")
        categorynode639.Nodes.Add("item1390562878", "Basic Ionic Chamber L")
        categorynode639.Nodes.Add("item962712579", "Basic Ionic Chamber XL")
        categorynode639.Nodes.Add("item962700664", "Rare Ionic Chamber XS")
        categorynode639.Nodes.Add("item1390563197", "Rare Ionic Chamber S")
        categorynode639.Nodes.Add("item1390563195", "Rare Ionic Chamber M")
        categorynode639.Nodes.Add("item1390563172", "Rare Ionic Chamber L")
        categorynode639.Nodes.Add("item962700657", "Rare Ionic Chamber XL")
        categorynode639.Nodes.Add("item963003738", "Uncommon Ionic Chamber XS")
        categorynode639.Nodes.Add("item1390563327", "Uncommon Ionic Chamber S")
        categorynode639.Nodes.Add("item1390563321", "Uncommon Ionic Chamber M")
        categorynode639.Nodes.Add("item1390563302", "Uncommon Ionic Chamber L")
        categorynode639.Nodes.Add("item963003731", "Uncommon Ionic Chamber XL")

        Dim categorynode6310 As TreeNode = categorynode63.Nodes.Add("node6310", "Laser Chambers")
        categorynode6310.Nodes.Add("item1252768249", "Advanced Laser Chamber XS")
        categorynode6310.Nodes.Add("item2825503297", "Advanced Laser Chamber S")
        categorynode6310.Nodes.Add("item2825503323", "Advanced Laser Chamber M")
        categorynode6310.Nodes.Add("item2825503320", "Advanced Laser Chamber L")
        categorynode6310.Nodes.Add("item1252768242", "Advanced Laser Chamber XL")
        categorynode6310.Nodes.Add("item2825505990", "Basic Laser Chamber S")
        categorynode6310.Nodes.Add("item1252823771", "Exotic Laser Chamber XS")
        categorynode6310.Nodes.Add("item2825506243", "Exotic Laser Chamber S")
        categorynode6310.Nodes.Add("item2825506265", "Exotic Laser Chamber M")
        categorynode6310.Nodes.Add("item2825506266", "Exotic Laser Chamber L")
        categorynode6310.Nodes.Add("item1252819658", "Rare Laser Chamber XS")
        categorynode6310.Nodes.Add("item2825506178", "Rare Laser Chamber S")
        categorynode6310.Nodes.Add("item2825506200", "Rare Laser Chamber M")
        categorynode6310.Nodes.Add("item2825506203", "Rare Laser Chamber L")
        categorynode6310.Nodes.Add("item1252764136", "Uncommon Laser Chamber XS")

        Dim categorynode6311 As TreeNode = categorynode63.Nodes.Add("node6311", "Lights")
        categorynode6311.Nodes.Add("item1829611507", "Uncommon Light XS")
        categorynode6311.Nodes.Add("item3345566836", "Uncommon Light S")

        Dim categorynode6312 As TreeNode = categorynode63.Nodes.Add("node6312", "Magnetic Rails")
        categorynode6312.Nodes.Add("item4211034905", "Advanced Magnetic Rail XS")
        categorynode6312.Nodes.Add("item2722610741", "Advanced Magnetic Rail S")
        categorynode6312.Nodes.Add("item2722610747", "Advanced Magnetic Rail M")
        categorynode6312.Nodes.Add("item2722610746", "Advanced Magnetic Rail L")
        categorynode6312.Nodes.Add("item4210044590", "Exotic Magnetic Rail XS")
        categorynode6312.Nodes.Add("item2722609330", "Exotic Magnetic Rail S")
        categorynode6312.Nodes.Add("item2722609340", "Exotic Magnetic Rail M")
        categorynode6312.Nodes.Add("item2722609339", "Exotic Magnetic Rail L")
        categorynode6312.Nodes.Add("item4210065279", "Rare Magnetic Rail XS")
        categorynode6312.Nodes.Add("item2722609523", "Rare Magnetic Rail S")
        categorynode6312.Nodes.Add("item2722609533", "Rare Magnetic Rail M")
        categorynode6312.Nodes.Add("item2722609530", "Rare Magnetic Rail L")

        Dim categorynode6313 As TreeNode = categorynode63.Nodes.Add("node6313", "Mechanical Sensors")
        categorynode6313.Nodes.Add("item204469317", "Advanced Mechanical Sensor XS")
        categorynode6313.Nodes.Add("item204444775", "Basic Mechanical Sensor XS")
        categorynode6313.Nodes.Add("item204462057", "Exotic Mechanical Sensor XS")

        Dim categorynode6314 As TreeNode = categorynode63.Nodes.Add("node6314", "Missile Silos")
        categorynode6314.Nodes.Add("item3026385661", "Advanced Missile Silo XS")
        categorynode6314.Nodes.Add("item3857142123", "Advanced Missile Silo S")
        categorynode6314.Nodes.Add("item3857142113", "Advanced Missile Silo M")
        categorynode6314.Nodes.Add("item3857142112", "Advanced Missile Silo L")
        categorynode6314.Nodes.Add("item3026262169", "Exotic Missile Silo XS")
        categorynode6314.Nodes.Add("item3857142764", "Exotic Missile Silo S")
        categorynode6314.Nodes.Add("item3857142758", "Exotic Missile Silo M")
        categorynode6314.Nodes.Add("item3857142757", "Exotic Missile Silo L")
        categorynode6314.Nodes.Add("item3026356360", "Rare Missile Silo XS")
        categorynode6314.Nodes.Add("item3857142317", "Rare Missile Silo S")
        categorynode6314.Nodes.Add("item3857142311", "Rare Missile Silo M")
        categorynode6314.Nodes.Add("item3857142308", "Rare Missile Silo L")

        Dim categorynode6315 As TreeNode = categorynode63.Nodes.Add("node6315", "Mobile Panels")
        categorynode6315.Nodes.Add("item408022872", "Advanced Mobile Panel XS")
        categorynode6315.Nodes.Add("item494821869", "Advanced Mobile Panel S")
        categorynode6315.Nodes.Add("item494821863", "Advanced Mobile Panel M")
        categorynode6315.Nodes.Add("item494821860", "Advanced Mobile Panel L")
        categorynode6315.Nodes.Add("item408022865", "Advanced Mobile Panel XL")
        categorynode6315.Nodes.Add("item407690298", "Basic Mobile Panel XS")
        categorynode6315.Nodes.Add("item494825071", "Basic Mobile Panel S")
        categorynode6315.Nodes.Add("item494825061", "Basic Mobile Panel M")
        categorynode6315.Nodes.Add("item494825062", "Basic Mobile Panel L")
        categorynode6315.Nodes.Add("item407690291", "Basic Mobile Panel XL")
        categorynode6315.Nodes.Add("item407844051", "Rare Mobile Panel XS")
        categorynode6315.Nodes.Add("item494823725", "Rare Mobile Panel S")
        categorynode6315.Nodes.Add("item494823731", "Rare Mobile Panel M")
        categorynode6315.Nodes.Add("item494823730", "Rare Mobile Panel L")
        categorynode6315.Nodes.Add("item407844040", "Rare Mobile Panel XL")
        categorynode6315.Nodes.Add("item407969641", "Uncommon Mobile Panel XS")
        categorynode6315.Nodes.Add("item494821804", "Uncommon Mobile Panel S")
        categorynode6315.Nodes.Add("item494821798", "Uncommon Mobile Panel M")
        categorynode6315.Nodes.Add("item494821797", "Uncommon Mobile Panel L")
        categorynode6315.Nodes.Add("item407969632", "Uncommon Mobile Panel XL")

        Dim categorynode6316 As TreeNode = categorynode63.Nodes.Add("node6316", "Motherboards")
        categorynode6316.Nodes.Add("item242607950", "Advanced Motherboard M")

        Dim categorynode6317 As TreeNode = categorynode63.Nodes.Add("node6317", "Ore Scanners")
        categorynode6317.Nodes.Add("item3501536208", "Advanced Ore Scanner L")
        categorynode6317.Nodes.Add("item3501535556", "Basic Ore Scanner S")
        categorynode6317.Nodes.Add("item3501535583", "Basic Ore Scanner L")
        categorynode6317.Nodes.Add("item3501535314", "Exotic Ore Scanner L")
        categorynode6317.Nodes.Add("item3501536145", "Rare Ore Scanner L")
        categorynode6317.Nodes.Add("item3501535518", "Uncommon Ore Scanner L")
        categorynode6317.Nodes.Add("item788805607", "Uncommon Ore Scanner XL")

        Dim categorynode6318 As TreeNode = categorynode63.Nodes.Add("node6318", "Power Transformers")
        categorynode6318.Nodes.Add("item4186198417", "Advanced Power Transformer M")
        categorynode6318.Nodes.Add("item4186205972", "Basic Power Transformer M")
        categorynode6318.Nodes.Add("item4186198480", "Rare Power Transformer M")
        categorynode6318.Nodes.Add("item4186198483", "Rare Power Transformer L")
        categorynode6318.Nodes.Add("item3291043715", "Rare Power Transformer XL")
        categorynode6318.Nodes.Add("item4186206037", "Uncommon Power Transformer M")
        categorynode6318.Nodes.Add("item4186206035", "Uncommon Power Transformer S")

        categorynode63.Nodes.Add("item2796831847", "Rare Structural Parts")
        Dim categorynode6319 As TreeNode = categorynode63.Nodes.Add("node6319", "Robotic Arms")
        categorynode6319.Nodes.Add("item997368670", "Advanced Robotic Arm M")
        categorynode6319.Nodes.Add("item997368796", "Basic Robotic Arm M")
        categorynode6319.Nodes.Add("item997368799", "Basic Robotic Arm L")
        categorynode6319.Nodes.Add("item2999955044", "Basic Robotic Arm XL")
        categorynode6319.Nodes.Add("item997370746", "Rare Robotic Arm M")
        categorynode6319.Nodes.Add("item997368607", "Uncommon Robotic Arm M")

        Dim categorynode6320 As TreeNode = categorynode63.Nodes.Add("node6320", "Screens")
        categorynode6320.Nodes.Add("item1428608303", "Advanced Screen XS")
        categorynode6320.Nodes.Add("item1428608292", "Advanced Screen XL")
        categorynode6320.Nodes.Add("item184261422", "Basic Screen S")
        categorynode6320.Nodes.Add("item184261412", "Basic Screen M")
        categorynode6320.Nodes.Add("item1428596474", "Uncommon Screen XS")
        categorynode6320.Nodes.Add("item184261478", "Uncommon Screen L")
        categorynode6320.Nodes.Add("item1428596467", "Uncommon Screen XL")

        categorynode63.Nodes.Add("item2796831841", "Uncommon Structural Parts")

        Dim categorynode64 As TreeNode = categorynode6.Nodes.Add("node64", "Intermediary parts")
        Dim categorynode641 As TreeNode = categorynode64.Nodes.Add("node641", "Components")
        categorynode641.Nodes.Add("item794666751", "Advanced Component")
        categorynode641.Nodes.Add("item794666749", "Basic Component")
        categorynode641.Nodes.Add("item794666748", "Uncommon Component")

        Dim categorynode642 As TreeNode = categorynode64.Nodes.Add("node642", "Connectors")
        categorynode642.Nodes.Add("item2872711781", "Advanced Connector")
        categorynode642.Nodes.Add("item2872711779", "Basic Connector")
        categorynode642.Nodes.Add("item2872711778", "Uncommon Connector")

        Dim categorynode643 As TreeNode = categorynode64.Nodes.Add("node643", "Fixations")
        categorynode643.Nodes.Add("item466630567", "Advanced Fixation")
        categorynode643.Nodes.Add("item466630565", "Basic Fixation")
        categorynode643.Nodes.Add("item466630564", "Uncommon Fixation")

        Dim categorynode644 As TreeNode = categorynode64.Nodes.Add("node644", "LEDs")
        categorynode644.Nodes.Add("item1234754160", "Advanced LED")
        categorynode644.Nodes.Add("item1234754162", "Basic LED")
        categorynode644.Nodes.Add("item1234754161", "Uncommon LED")

        Dim categorynode645 As TreeNode = categorynode64.Nodes.Add("node645", "Pipes")
        categorynode645.Nodes.Add("item1799107244", "Advanced Pipe")
        categorynode645.Nodes.Add("item1799107246", "Basic Pipe")
        categorynode645.Nodes.Add("item1799107247", "Uncommon Pipe")

        Dim categorynode646 As TreeNode = categorynode64.Nodes.Add("node646", "Screws")
        categorynode646.Nodes.Add("item3936127017", "Advanced Screw")
        categorynode646.Nodes.Add("item3936127019", "Basic Screw")
        categorynode646.Nodes.Add("item3936127018", "Uncommon Screw")

        Dim categorynode65 As TreeNode = categorynode6.Nodes.Add("node65", "Structural Parts")
        Dim categorynode651 As TreeNode = categorynode65.Nodes.Add("node651", "Casings")
        categorynode651.Nodes.Add("item946544989", "Advanced Casing XS")
        categorynode651.Nodes.Add("item567007766", "Advanced Casing S")
        categorynode651.Nodes.Add("item567007760", "Advanced Casing M")
        categorynode651.Nodes.Add("item567007775", "Advanced Casing L")
        categorynode651.Nodes.Add("item946544964", "Advanced Casing XL")
        categorynode651.Nodes.Add("item946503935", "Basic Casing XS")
        categorynode651.Nodes.Add("item567008148", "Basic Casing S")
        categorynode651.Nodes.Add("item567007899", "Exotic Casing S")
        categorynode651.Nodes.Add("item946524256", "Rare Casing XS")
        categorynode651.Nodes.Add("item946516044", "Uncommon Casing XS")
        categorynode651.Nodes.Add("item567008215", "Uncommon Casing S")
        categorynode651.Nodes.Add("item567008209", "Uncommon Casing M")
        categorynode651.Nodes.Add("item946516085", "Uncommon Casing XL")

        Dim categorynode652 As TreeNode = categorynode65.Nodes.Add("node652", "Reinforced Frames")
        categorynode652.Nodes.Add("item1179601457", "Advanced Reinforced Frame XS")
        categorynode652.Nodes.Add("item994058059", "Advanced Reinforced Frame S")
        categorynode652.Nodes.Add("item994058069", "Advanced Reinforced Frame M")
        categorynode652.Nodes.Add("item994058066", "Advanced Reinforced Frame L")
        categorynode652.Nodes.Add("item1179601462", "Advanced Reinforced Frame XL")
        categorynode652.Nodes.Add("item1179610525", "Basic Reinforced Frame XS")
        categorynode652.Nodes.Add("item994058182", "Basic Reinforced Frame S")
        categorynode652.Nodes.Add("item994058204", "Basic Reinforced Frame M")
        categorynode652.Nodes.Add("item994058205", "Basic Reinforced Frame L")
        categorynode652.Nodes.Add("item1179610516", "Basic Reinforced Frame XL")
        categorynode652.Nodes.Add("item1179593235", "Exotic Reinforced Frame XS")
        categorynode652.Nodes.Add("item994057929", "Exotic Reinforced Frame S")
        categorynode652.Nodes.Add("item994057943", "Exotic Reinforced Frame M")
        categorynode652.Nodes.Add("item994057936", "Exotic Reinforced Frame L")
        categorynode652.Nodes.Add("item1179593236", "Exotic Reinforced Frame XL")
        categorynode652.Nodes.Add("item1179605664", "Rare Reinforced Frame XS")
        categorynode652.Nodes.Add("item994057994", "Rare Reinforced Frame S")
        categorynode652.Nodes.Add("item994058004", "Rare Reinforced Frame M")
        categorynode652.Nodes.Add("item994058003", "Rare Reinforced Frame L")
        categorynode652.Nodes.Add("item1179605671", "Rare Reinforced Frame XL")
        categorynode652.Nodes.Add("item1179614604", "Uncommon Reinforced Frame XS")
        categorynode652.Nodes.Add("item994058119", "Uncommon Reinforced Frame S")
        categorynode652.Nodes.Add("item994058141", "Uncommon Reinforced Frame M")
        categorynode652.Nodes.Add("item994058140", "Uncommon Reinforced Frame L")
        categorynode652.Nodes.Add("item1179614597", "Uncommon Reinforced Frame XL")

        Dim categorynode653 As TreeNode = categorynode65.Nodes.Add("node653", "Standard Frames")
        categorynode653.Nodes.Add("item873622227", "Advanced Standard Frame XS")
        categorynode653.Nodes.Add("item1981363796", "Advanced Standard Frame S")
        categorynode653.Nodes.Add("item1981362606", "Advanced Standard Frame M")
        categorynode653.Nodes.Add("item1981362607", "Advanced Standard Frame L")
        categorynode653.Nodes.Add("item873622058", "Advanced Standard Frame XL")
        categorynode653.Nodes.Add("item873663991", "Basic Standard Frame XS")
        categorynode653.Nodes.Add("item1981362643", "Basic Standard Frame S")
        categorynode653.Nodes.Add("item1981362473", "Basic Standard Frame M")
        categorynode653.Nodes.Add("item1981362474", "Basic Standard Frame L")
        categorynode653.Nodes.Add("item873614065", "Exotic Standard Frame XS")
        categorynode653.Nodes.Add("item1981363926", "Exotic Standard Frame S")
        categorynode653.Nodes.Add("item1981363756", "Exotic Standard Frame M")
        categorynode653.Nodes.Add("item1981363757", "Exotic Standard Frame L")
        categorynode653.Nodes.Add("item1981362581", "Rare Standard Frame S")
        categorynode653.Nodes.Add("item1981362671", "Rare Standard Frame M")
        categorynode653.Nodes.Add("item1981362670", "Rare Standard Frame L")
        categorynode653.Nodes.Add("item873676070", "Uncommon Standard Frame XS")
        categorynode653.Nodes.Add("item1981362450", "Uncommon Standard Frame S")
        categorynode653.Nodes.Add("item1981362536", "Uncommon Standard Frame M")
        categorynode653.Nodes.Add("item1981362539", "Uncommon Standard Frame L")
    End Sub

    '########## Date and Timestamp calculations ##########
    Private Sub TimeStampInt()
        currentdate = DateAndTime.Now
        CurrYear = currentdate.Year
        CurrMonth = currentdate.Month
        CurrDay = currentdate.Day
        CurrHour = currentdate.Hour
        CurrMin = currentdate.Minute
        CurrSec = currentdate.Second
    End Sub

    Private Sub TimeStampStr()
        currentdate = DateAndTime.Now
        CurrYear = currentdate.Year
        CurrMonth = currentdate.Month
        CurrDay = currentdate.Day
        CurrHour = currentdate.Hour
        CurrMin = currentdate.Minute
        CurrSec = currentdate.Second
        If Int(CurrMonth) < 10 Then
            CurrMonth = "0" & currentdate.Month
        End If
        If Int(CurrDay) < 10 Then
            CurrDay = "0" & currentdate.Day
        End If
        If Int(CurrHour) < 10 Then
            CurrHour = "0" & currentdate.Hour
        End If
        If Int(CurrMin) < 10 Then
            CurrMin = "0" & currentdate.Minute
        End If
        If Int(CurrSec) < 10 Then
            CurrSec = "0" & currentdate.Second
        End If
    End Sub

    Private Function GetTimeRemaining(ByVal dateinput As String, ByVal orderidinput As String)
        Dim timeremaining As TimeSpan
        Dim timestring As String
        If dateinput.StartsWith("@(") Then
            TimeStampInt()
            Dim Datetemp1 As String = dateinput.Remove(0, dateinput.IndexOf(") ") + 2) 'remove the unix time stamp
            Dim Datetemp3 As Date = Convert.ToDateTime(Datetemp1)
            If Datetemp3 > currentdate Then
                timeremaining = Datetemp3.Subtract(currentdate)
                If timeremaining.TotalSeconds > 120 Then
                    If timeremaining.TotalMinutes > 59 Then
                        If timeremaining.TotalHours > 48 Then
                            timestring = CStr(Math.Round(timeremaining.TotalDays)) & " days"
                        Else
                            timestring = CStr(Math.Round(timeremaining.TotalHours)) & " hours"
                        End If
                    Else
                        timestring = CStr(Math.Round(timeremaining.TotalMinutes)) & " minutes"
                    End If
                Else
                    timestring = CStr(Math.Round(timeremaining.TotalSeconds)) & " seconds"
                End If
            Else
                timestring = "Expired"
            End If
        Else
            timestring = "Invalid timestamp"
        End If
        Return timestring
    End Function

    '########## Self-Update check ##########
    Private Sub CheckForUpdates()
        Dim UpdateCheckCurrent As String = API_Request("ver", "").Replace("""", "").Trim()
        If UpdateCheckCurrent = API_Client_Version Then
            'Do nothing, we are up to date.
        Else
            'Current version and running version do not match.
            API_Available_Version = UpdateCheckCurrent
            'We need to now get the minimum supported client version from the server, and check if the running version is greater than that.
            Dim forceupdflag As Boolean = False
            Dim UpdateCheckMinimum As String = API_Request("minver", "").Replace("""", "").Trim()
            Dim versubsself() As String = API_Client_Version.Split(".")
            Dim versubs() As String = UpdateCheckMinimum.Split(".")
            If CInt(versubs(0)) > CInt(versubsself(0)) Then
                forceupdflag = True
            End If
            If CInt(versubs(1)) > CInt(versubsself(1)) Then
                forceupdflag = True
            End If
            If CInt(versubs(2)) > CInt(versubsself(2)) And CInt(versubs(1)) >= CInt(versubsself(1)) Then
                forceupdflag = True
            End If
            If forceupdflag = True Then
                RequireUpdate()
            Else
                'Running version is above minimum server requirements, but is still not the latest.
                LoginUpdateBanner.Visible = True
                UpdateButton1.Visible = True
                UpdateButton2.Visible = True
                LoginUpdateText.Text = "An update (Version " & API_Available_Version & ") for DUOpenMarket is available, but not required. Download?"
            End If
        End If
    End Sub

    Private Sub RequireUpdate()
        DiscordLoginButton.Enabled = False
        DiscordLoginButton2.Enabled = False
        DiscordLoginButton.Visible = False
        DiscordLoginButton2.Visible = False
        LoginUpdateBanner.BackColor = Color.FromArgb(255, 215, 65, 65)
        LoginUpdateBanner.Visible = True
        UpdateButton1.Visible = True
        UpdateButton2.Visible = True
        LoginUpdateText.Text = "An update (Version " & API_Available_Version & ") for DUOpenMarket is required. Download?"
    End Sub

    Public Function GetURLDataBin(ByVal URL As String, Optional ByRef UserName As String = "", Optional ByRef Password As String = "") As Byte()
        Dim Req As HttpWebRequest
        Dim SourceStream As System.IO.Stream
        Dim Response As HttpWebResponse
        Try
            Req = HttpWebRequest.Create(URL)
            Response = Req.GetResponse()
            SourceStream = Response.GetResponseStream()
            Dim Buffer(4096) As Byte, BlockSize As Integer
            Dim TempStream As New MemoryStream
            Do
                BlockSize = SourceStream.Read(Buffer, 0, 4096)
                If BlockSize > 0 Then TempStream.Write(Buffer, 0, BlockSize)
            Loop While BlockSize > 0
            Return TempStream.ToArray()
        Catch ex As Exception
            NewEventMsg(ex.Message)
        Finally
            SourceStream.Close()
            Response.Close()
        End Try
    End Function

    Private Sub UpdateButton1_Click(sender As Object, e As EventArgs) Handles UpdateButton1.Click
        'Simple self-update subroutine. First we'll get the binary contents from github, and save it to a temporary text file.
        'If the download isnt interrupted and succeeds, we will need to close the current instance of the program and launch the new one after renaming it to an exe.
        'The easiest way to do this is with a simple batch script, which is created via the streamwriter.
        'In batch, there's no simple command to wait a period of time without requiring the user to press a key...
        'To get around this we can use "Ping localhost -n X >NUL" where X is the number of seconds we want to wait, +1.
        'This is necessary to make sure the download and disk operations are complete before proceeding to the next command in the batch file.
        'May not be adequate on slower systems. Testing required.
        If API_Available_Version = "" Or API_Available_Version = Nothing Then
            NewEventMsg("Failed to get newest version number from server.")
        Else
            My.Computer.FileSystem.WriteAllBytes(My.Application.Info.DirectoryPath & "\DUOpenMarket Client.temp", GetURLDataBin("https://github.com/Jason-Bloomer/DUOpenMarket/releases/download/v" & API_Available_Version & "/DUOpenMarket.Client.exe"), False)
            Dim My_Process As New Process()
            Dim My_Process_Info As New ProcessStartInfo()
            Dim strPath As String = My.Application.Info.DirectoryPath & "\DUOM-update.bat"
            Dim swDestruct As StreamWriter = New StreamWriter(strPath)
            swDestruct.WriteLine("PING localhost -n 3 >NUL && taskkill /F /IM ""DUOpenMarket Client.exe"" && PING localhost -n 6 >NUL && del """ & My.Application.Info.DirectoryPath & "\DUOpenMarket Client.exe"" && ren """ & My.Application.Info.DirectoryPath & "\DUOpenMarket Client.temp"" """ & "DUOpenMarket Client.exe"" && del """ & strPath & """ && PING localhost -n 3 >NUL  && """ & My.Application.Info.DirectoryPath & "\DUOpenMarket Client.exe""")
            swDestruct.Close()
            My_Process_Info.FileName = strPath
            My_Process_Info.CreateNoWindow = True
            My_Process_Info.UseShellExecute = False
            My_Process.EnableRaisingEvents = False
            My_Process.StartInfo = My_Process_Info
            My_Process.Start()
        End If
    End Sub

    Private Sub UpdateButton2_Click(sender As Object, e As EventArgs) Handles UpdateButton2.Click
        Dim manualdownload As Process = Process.Start("https://github.com/Jason-Bloomer/DUOpenMarket/releases/")
    End Sub

    '########## Discord Login panel ##########
    Private Sub CenterLoginElements()
        If LoginPanel.Visible = True Then
            Dim temp1 As Point
            Dim temp2 As Point
            Dim temp3 As Point
            Dim temp4 As Point
            Dim temp5 As Point
            Dim temp6 As Point
            Dim temp7 As Point
            temp1.X = (LoginPanel.Size.Width / 2) - (LoginLogoPictureBox.Size.Width / 2)
            temp1.Y = ((LoginPanel.Size.Height / 2) - (LoginLogoPictureBox.Size.Height / 2)) * 0.5
            LoginLogoPictureBox.Location = temp1
            temp2.X = (LoginPanel.Size.Width / 2) - (LoginLabel1.Size.Width / 2)
            temp2.Y = ((LoginPanel.Size.Height / 2) - (LoginLabel1.Size.Height / 2)) * 1.2
            LoginLabel1.Location = temp2
            temp3.X = (LoginPanel.Size.Width / 2) - (LoginLabel2.Size.Width / 2)
            temp3.Y = (((LoginPanel.Size.Height / 2) - (LoginLabel2.Size.Height / 2)) * 1.2) + 30
            LoginLabel2.Location = temp3
            temp4.X = (LoginPanel.Size.Width / 2) - (LoginLabel3.Size.Width / 2)
            temp4.Y = (((LoginPanel.Size.Height / 2) - (LoginLabel3.Size.Height / 2)) * 1.2) + 60
            LoginLabel3.Location = temp4
            temp5.X = (LoginPanel.Size.Width / 2) - (DiscordLoginButton2.Size.Width / 2)
            temp5.Y = ((LoginPanel.Size.Height / 2) - (DiscordLoginButton2.Size.Height / 2)) * 1.6
            DiscordLoginButton2.Location = temp5
            temp6.X = ((LoginPanel.Size.Width / 2) - (UpdateButton1.Size.Width / 2)) * 0.4
            temp6.Y = ((LoginPanel.Size.Height / 2) - (UpdateButton1.Size.Height / 2)) * 1.95
            UpdateButton1.Location = temp6
            temp7.X = ((LoginPanel.Size.Width / 2) - (UpdateButton2.Size.Width / 2)) * 1.6
            temp7.Y = ((LoginPanel.Size.Height / 2) - (UpdateButton2.Size.Height / 2)) * 1.95
            UpdateButton2.Location = temp7
            LoginLabel2.Text = "Desktop Client - v" + API_Client_Version
            Label22.Text = "DUOpenMarket API Desktop Client - v" & API_Client_Version
        End If
    End Sub

    Private Sub ConnectionStyling()
        If API_Connected = False Then
            DiscordLoginButton.Text = "Login With Discord"
            DiscordLoginButton.ForeColor = Color.FromArgb(255, 215, 65, 65)
            ItemTree.Enabled = False
            ItemTreeSearch.Enabled = False
            MarketPanel.Visible = False
            LoginPanel.Visible = True
            TableLayoutPanel2.Visible = False
        Else
            DiscordLoginButton.Text = "Disconnect"
            DiscordLoginButton.ForeColor = Color.FromArgb(255, 65, 215, 65)
            ItemTree.Enabled = True
            ItemTreeSearch.Enabled = True
            MarketPanel.Visible = True
            LoginPanel.Visible = False
            TableLayoutPanel2.Visible = True
        End If
    End Sub

    Private Sub TestDiscordLogin() Handles DiscordLoginButton.Click, DiscordLoginButton2.Click
        If API_Connected = False Then
            Dim Discord_Login_Page As String = "https://duopenmarket.com/discordclientGetAuthCode.php"
            Discord_Login_Window_Handle = Process.Start(Discord_Login_Page)
            If ListenerStarted = False Then
                CreateListener()
                ListenerStarted = True
            End If
            ConnectionTimer.Start()
        Else
            NewEventMsg("Stopped Log Monitor.")
            API_Connected = False
            API_Discord_Auth_Code = Nothing
            API_Discord_Auth_State = Nothing
            OperationTimer.Stop()
            ConnectionStyling()
            NewEventMsg("Disconnected from API.")
            CenterLoginElements()
        End If
    End Sub

    Private Sub ConnectionTimer_Tick(sender As Object, e As EventArgs) Handles ConnectionTimer.Tick
        If API_Discord_Auth_Code IsNot Nothing Then
            If API_LogfileDirectory.Trim.Length = 0 Then
                NewEventMsg("Invalid log path supplied for file monitoring.")
                ConnectionTimer.Stop()
            Else
                If API_Discord_Auth_Code = "=access_denied" Then
                    NewEventMsg("The user cancelled authorization through discord.")
                    ConnectionTimer.Stop()
                Else
                    API_Connected = True
                    LoginPanel.Visible = False
                    NewEventMsg("Obtained Authorization code. Login successful.")
                    ConnectionStyling()
                    API_LogFile = GetNewestLogFile(API_LogfileDirectory)
                    ConnectionTimer.Stop()
                    ReadFileLines(API_LogFile)
                    OperationTimer.Start()
                    LogFileCheckTimer.Start()
                    NewEventMsg("Started Log Monitor.")
                    NewEventMsg("Current Log File: " & API_LogFile)
                End If
            End If
        End If
    End Sub

    Private Sub LoginTimer_Tick(sender As Object, e As EventArgs) Handles LoginTimer.Tick
        If t IsNot Nothing Then
            NewEventMsg("Login attempt timed out.")
            StopListener()
        End If
    End Sub

    '########## Title-Bar Buttons ##########
    Private Sub TitlebarAboutButton_Click(sender As Object, e As EventArgs) Handles TitlebarAboutButton.Click
        AboutForm.Label6.Text = "Desktop Client v" & API_Client_Version
        AboutForm.Show()
    End Sub

    Private Sub TitlebarMaxButton_Click(sender As Object, e As EventArgs) Handles TitlebarMaxButton.Click
        If WindowMaximizedState = True Then
            WindowMaximizedState = False
            Dim newbnds2 As Size = New Size()
            Dim newpoint2 As New Point
            newpoint2.X = WindowSavedPosX
            newpoint2.Y = WindowSavedPosY
            newbnds2.Height = WindowSavedBoundsY
            newbnds2.Width = WindowSavedBoundsX
            Me.Size = newbnds2
            Me.Location = newpoint2
        Else
            WindowSavedBoundsX = Me.Size.Width
            WindowSavedBoundsY = Me.Size.Height
            WindowSavedPosX = Me.Location.X
            WindowSavedPosY = Me.Location.Y
            WindowMaximizedState = True
            Dim currScreen As Screen = Screen.FromControl(Me)
            Dim newbnds1 As Size = New Size()
            Dim newpoint1 As New Point
            newpoint1.X = currScreen.WorkingArea.X
            newpoint1.Y = currScreen.WorkingArea.Y
            newbnds1.Height = currScreen.WorkingArea.Size.Height
            newbnds1.Width = currScreen.WorkingArea.Width
            Me.Size = newbnds1
            Me.Location = newpoint1
        End If
        CenterLoginElements()
    End Sub

    Private Sub TitlebarMinButton_Click(sender As Object, e As EventArgs) Handles TitlebarMinButton.Click
        Me.WindowState = FormWindowState.Minimized
    End Sub

    Private Sub TitlebarCloseButton_Click(sender As Object, e As EventArgs) Handles TitlebarCloseButton.Click
        If Setting_BackgroundWorker = True Then
            NotifyIcon1.Visible = True
            Me.Hide()
            NotifyIcon1.BalloonTipText = "DUOpenMarket - Working in Background"
            NotifyIcon1.ShowBalloonTip(500)
        Else
            SavePrefsToIni()
            Application.Exit()
            Me.Dispose()
        End If
    End Sub

    Private Sub TitlebarSettingsButton_Click(sender As Object, e As EventArgs) Handles TitlebarSettingsButton.Click
        SettingsForm.Show()
    End Sub

    '########## Title-Bar Click&Drag ##########
    Private Sub TitleBar_MouseUp(ByVal sender As Object, ByVal e As MouseEventArgs) Handles TitleBarPanel.MouseUp
        Go = False
        LeftSet = False
        TopSet = False
    End Sub

    Private Sub TitleBar_MouseDown(ByVal sender As Object, ByVal e As MouseEventArgs) Handles TitleBarPanel.MouseDown
        Go = True
        HoldLeft = (Control.MousePosition.X - Me.Location.X)
        HoldTop = (Control.MousePosition.Y - Me.Location.Y)
    End Sub

    Private Sub TitleBar_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs) Handles TitleBarPanel.MouseMove
        If Go = True Then
            If WindowMaximizedState = True Then
                WindowMaximizedState = False
                'reset size if maximized, to whatever size we were before maximizing
                Dim newbnds As Size = New Size()
                newbnds.Height = WindowSavedBoundsY
                newbnds.Width = WindowSavedBoundsX
                Me.Size = newbnds
            End If

            deltaX = Control.MousePosition.X - HoldLeft
            deltaY = Control.MousePosition.Y - HoldTop

            OffLeft = deltaX
            OffTop = deltaY

            Dim newpoint As New Point
            newpoint.X = OffLeft
            newpoint.Y = OffTop

            Me.Location = newpoint
            Me.Refresh()
            CenterLoginElements()
        End If
    End Sub

    '########## Resize Grabber Click&Drag ##########
    Private Sub ResizeGrabber_MouseUp(ByVal sender As Object, ByVal e As MouseEventArgs) Handles ResizeGrabber.MouseUp, ResizeGrabber2.MouseUp
        Go = False
        LeftSet = False
        TopSet = False
    End Sub

    Private Sub ResizeGrabber_MouseDown(ByVal sender As Object, ByVal e As MouseEventArgs) Handles ResizeGrabber.MouseDown, ResizeGrabber2.MouseDown
        Go = True
    End Sub

    Private Sub ResizeGrabber_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs) Handles ResizeGrabber.MouseMove, ResizeGrabber2.MouseMove
        If Go = True Then
            HoldWidth = (Control.MousePosition.X - Me.Left)
            HoldHeight = (Control.MousePosition.Y - Me.Top)
            If TopSet = False Then
                TopSet = True
            End If
            If LeftSet = False Then
                LeftSet = True
            End If
            If HoldWidth < 1280 Then
                HoldWidth = 1280
            End If
            If HoldHeight < 760 Then
                HoldHeight = 760
            End If
            Me.Width = HoldWidth
            Me.Height = HoldHeight
            Me.Refresh()
            CenterLoginElements()
        End If
    End Sub

    '########## Command Line Interface ##########
    Private Sub ConsoleInputBox_TextChanged(sender As Object, e As EventArgs) Handles ConsoleInputBox.TextChanged
        If ConsoleInputBox.Text IsNot "" Then
            Dim lastchar As String = ConsoleInputBox.Text.Remove(0, ConsoleInputBox.Text.Length - 1)
            If AscW(lastchar) = 10 Then
                ParseConsoleInput(ConsoleInputBox.Text.Remove(ConsoleInputBox.Text.Length - 2, 2))
                ConsoleInputBox.Text = ""
            End If
        End If
    End Sub

    Private Sub ConsoleSubmitButton_Click(sender As Object, e As EventArgs) Handles ConsoleSubmitButton.Click
        ParseConsoleInput(ConsoleInputBox.Text)
        ConsoleInputBox.Text = ""
    End Sub

    Private Sub NewEventMsg(ByVal input As String)
        If ConsoleTextBox.TextLength() > 524288000 Then
            ConsoleTextBox.Text = ConsoleTextBox.Text.Remove(0, 52428800)
        End If
        ConsoleTextBox.AppendText(Environment.NewLine & input)
        ConsoleTextBox.Select(ConsoleTextBox.TextLength, 0)
        ConsoleTextBox.ScrollToCaret()
    End Sub

    Private Sub ParseConsoleInput(ByVal inputstr As String)
        NewEventMsg("user@console_>: " & inputstr)
        Dim match As Boolean = False
        If inputstr.ToLower = "debug" Then
            match = True
            NewEventMsg(Environment.NewLine)
            NewEventMsg("---------- DUOpenMarket Variable Dump ----------")
            NewEventMsg("API_Connected = " & CStr(API_Connected))
            NewEventMsg("API_LogfileDirectory = " & CStr(API_LogfileDirectory))
            NewEventMsg("API_LogFile = " & CStr(API_LogFile))
            NewEventMsg("API_Discord_Auth_Code = " & CStr(API_Discord_Auth_Code))
            NewEventMsg("API_Discord_Auth_State = " & CStr(API_Discord_Auth_State))
            NewEventMsg("API_Discord_Allow_New_Auth = " & CStr(API_Discord_Allow_New_Auth))
            NewEventMsg("API_Last_Log_Processed = " & CStr(API_Last_Log_Processed))
            NewEventMsg("API_Client_Version = " & CStr(API_Client_Version))
            NewEventMsg("API_Available_Version = " & CStr(API_Available_Version))
            NewEventMsg("API_Log_Queue.Count = " & CStr(API_Log_Queue.Count))
            NewEventMsg("File Buffer Offset = " & CStr(_LastOffset))
            NewEventMsg("LastFileModified = " & CStr(LastFileModified))
            NewEventMsg("NumberofModifications = " & CStr(NumberofModifications))
            NewEventMsg("LogfileLastOffset = " & CStr(LogfileLastOffset))
            NewEventMsg("WindowMaximizedState = " & CStr(WindowMaximizedState))
            NewEventMsg("WindowNormalBoundsX = " & CStr(WindowNormalBoundsX))
            NewEventMsg("WindowNormalBoundsY = " & CStr(WindowNormalBoundsY))
            NewEventMsg("Theme State = " & CStr(Setting_ThemeState))
            NewEventMsg("Showing Raw Data = " & CStr(ShowRawData))
            NewEventMsg("API Read Requests Sent = " & CStr(NumberOfReads))
            NewEventMsg("API Update Requests Sent = " & CStr(NumberOfUpdates))
            NewEventMsg("API Create Requests Sent = " & CStr(NumberOfCreates))
            NewEventMsg("API Delete Requests Sent = " & CStr(NumberOfDeletes))
            NewEventMsg("Last Item ID From Logs = " & CStr(LastItemId))
            NewEventMsg("Unique ID Count = " & CStr(UniqueItemIds.Count))
            NewEventMsg("Listener Active = " & CStr(ListenerStarted))
            NewEventMsg("SearchUserTyping = " & CStr(SearchUserTyping))
            NewEventMsg("Show Filters = " & CStr(ShowFilters))
            NewEventMsg("FilterPriceMin = " & CStr(FilterPriceMin))
            NewEventMsg("FilterPriceMax = " & CStr(FilterPriceMax))
            NewEventMsg("FilterQuantityMin = " & CStr(FilterQuantityMin))
            NewEventMsg("FilterQuantityMax = " & CStr(FilterQuantityMax))
            NewEventMsg("FilterMarketList.Count = " & CStr(FilterMarketList.Count))
            NewEventMsg("FilterItemList.Count = " & CStr(FilterItemList.Count))
            NewEventMsg("Show Bookmarks = " & CStr(ShowBookmarks))
            NewEventMsg("Bookmarks.Count = " & CStr(Bookmarks.Count))
            NewEventMsg("currentdate = " & CStr(currentdate.ToString))
            NewEventMsg("Setting_SaveWindowLoc = " & CStr(Setting_SaveWindowLoc))
            NewEventMsg("Setting_SaveWindowSz = " & CStr(Setting_SaveWindowSz))
            NewEventMsg("Setting_SaveGridLayout = " & CStr(Setting_SaveGridLayout))
            NewEventMsg("------------- End Variable Dump ------------")
        End If
        If inputstr.ToLower = "help" Then
            match = True
            NewEventMsg(Environment.NewLine)
            NewEventMsg("---------- DUOpenMarket Console Help ----------")
            NewEventMsg("help    |  Print this dialogue. D'oh!")
            NewEventMsg("debug   |  Print the current values of all internal global variables and constants.")
            NewEventMsg("lastid  |  Print item ID's to the CLI as they are read from the logfile.")
            NewEventMsg("raw     |  Toggle between showing user-readable order data, or raw unmodified data.")
            NewEventMsg("process |  [arg1] - either 0, or 1. Turns batch order-processing on or off according to arg1")
            NewEventMsg("startup |  [arg1] - either 0, or 1. Creates a registry key file on the desktop which will enable or disable starting DUOM when windows boots.")
            NewEventMsg("------------- End Console Help ------------")
        End If
        If inputstr.ToLower = "raw" Then
            match = True
            If ShowRawData = False Then
                ShowRawData = True
                NewEventMsg("Showing raw API data. Type 'raw' again to go back to user-readable.")
            Else
                ShowRawData = False
                NewEventMsg("Showing user-readable data.")
            End If
        End If
        If inputstr.ToLower = "lastid" Then
            match = True
            Try
                If UniqueItemIds.Count > 0 Then
                    NewEventMsg("Last parsed item ID: " & UniqueItemIds(UniqueItemIds.Last))
                Else
                    NewEventMsg("No logs have been parsed, no ID's have been seen.")
                End If
            Catch ex As Exception
                NewEventMsg(ex.Message)
            End Try
        End If
        If inputstr.ToLower.StartsWith("process") = True Then
            match = True
            If inputstr.ToLower.EndsWith("1") = True Then
                Setting_Processinbatch = "True"
                NewEventMsg("Batch Processing Enabled.")
                OperationTimer.Interval = 3000
            Else
                Setting_Processinbatch = "False"
                NewEventMsg("Batch Processing Disabled.")
                OperationTimer.Interval = 50
            End If
        End If
        If inputstr.ToLower.StartsWith("startup") = True Then
            match = True
            If inputstr.ToLower.EndsWith("1") = True Then
                CreateEnableStartupKey()
            Else
                DisableStartupKey()
            End If
        End If
        If match = False Then
            NewEventMsg("Unknown command. Try again or type ""help"" for commands.")
        End If
    End Sub

    '########## Economy Statistics ##########
    Private Sub ResetEconomyStatLabels()
        EconStatLabel_Buy_High.Text = "0"
        EconStatLabel_Buy_Avg.Text = "0"
        EconStatLabel_Buy_Low.Text = "0"
        EconStatLabel_Buy_Total.Text = "0"
        EconStatLabel_Buy_Volume.Text = "0"
        EconStatLabel_Sell_High.Text = "0"
        EconStatLabel_Sell_Avg.Text = "0"
        EconStatLabel_Sell_Low.Text = "0"
        EconStatLabel_Sell_Total.Text = "0"
        EconStatLabel_Sell_Volume.Text = "0"
    End Sub

    Private Sub SetEconomyStatLabels()
        EconStatLabel_Buy_High.Text = EconomyStat_Buy_High.ToString()
        EconStatLabel_Buy_Avg.Text = EconomyStat_Buy_Avg.ToString()
        EconStatLabel_Buy_Low.Text = EconomyStat_Buy_Low.ToString()
        EconStatLabel_Buy_Total.Text = EconomyStat_Buy_Total.ToString()
        EconStatLabel_Buy_Volume.Text = EconomyStat_Buy_Vol.ToString()
        EconStatLabel_Sell_High.Text = EconomyStat_Sell_High.ToString()
        EconStatLabel_Sell_Avg.Text = EconomyStat_Sell_Avg.ToString()
        EconStatLabel_Sell_Low.Text = EconomyStat_Sell_Low.ToString()
        EconStatLabel_Sell_Total.Text = EconomyStat_Sell_Total.ToString()
        EconStatLabel_Sell_Volume.Text = EconomyStat_Sell_Vol.ToString()
    End Sub

    '########## Item Tree select ##########
    Private Sub TreeView1_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles ItemTree.AfterSelect, ItemTreeSearch.AfterSelect
        If CenterUXPanelMode = 1 Then
            OrderSearch(e)
        ElseIf CenterUXPanelMode = 2 Then
            GenerateHistogram()
        End If
    End Sub

    '########## Item Search filtering ##########
    Private Sub ItemSearchTextBox_TextChanged(sender As Object, e As EventArgs) Handles ItemSearchTextBox.TextChanged
        If ItemSearchTextBox.Text IsNot "" And ItemSearchTextBox.Text IsNot Nothing And ItemSearchTextBox.Text IsNot "Search..." Then
            SearchTimer.Stop()
            SearchTimer.Start()
        Else
            ItemTreeSearch.Visible = False
            ItemTree.Visible = True
        End If
    End Sub

    Private Sub SearchItems()
        'Check the item list using up to eight levels of recursion, adding all children nodes containing the search term to the new element.
        'Item list only needs six levels of recursion, the additional two are a safeguard in case the item structure changes. Shouldn't impact performance.
        If ShowFilters = True Then
            AdvItemTreeView.Nodes.Clear()
            For Each tn As TreeNode In ItemTree.Nodes
                If tn.Name.StartsWith("node") Then
                    'This is a category node, do not add it to list
                    For Each tn2 As TreeNode In tn.Nodes
                        If tn2.Name.StartsWith("node") Then
                            'This is a category node, do not add it to list
                            For Each tn3 As TreeNode In tn2.Nodes
                                If tn3.Name.StartsWith("node") Then
                                    'This is a category node, do not add it to list
                                    For Each tn4 As TreeNode In tn3.Nodes
                                        If tn4.Name.StartsWith("node") Then
                                            'This is a category node, do not add it to list
                                            For Each tn5 As TreeNode In tn4.Nodes
                                                If tn5.Name.StartsWith("node") Then
                                                    'This is a category node, do not add it to list
                                                    For Each tn6 As TreeNode In tn5.Nodes
                                                        If tn6.Name.StartsWith("node") Then
                                                            'This is a category node, do not add it to list
                                                            For Each tn7 As TreeNode In tn6.Nodes
                                                                If tn7.Name.StartsWith("node") Then
                                                                    'This is a category node, do not add it to list
                                                                    For Each tn8 As TreeNode In tn7.Nodes
                                                                        If tn8.Name.StartsWith("node") Then
                                                                            'This is a category node, do not add it to list
                                                                        Else
                                                                            Dim compstr As String = tn8.Text
                                                                            If compstr.ToLower.Contains(ItemSearchTextBox.Text.ToLower) Then
                                                                                AdvItemTreeView.Nodes.Add(tn8.Name, tn8.Text)
                                                                            End If
                                                                        End If
                                                                    Next tn8
                                                                Else
                                                                    Dim compstr As String = tn7.Text
                                                                    If compstr.ToLower.Contains(ItemSearchTextBox.Text.ToLower) Then
                                                                        AdvItemTreeView.Nodes.Add(tn7.Name, tn7.Text)
                                                                    End If
                                                                End If
                                                            Next tn7
                                                        Else
                                                            Dim compstr As String = tn6.Text
                                                            If compstr.ToLower.Contains(ItemSearchTextBox.Text.ToLower) Then
                                                                AdvItemTreeView.Nodes.Add(tn6.Name, tn6.Text)
                                                            End If
                                                        End If
                                                    Next tn6
                                                Else
                                                    Dim compstr As String = tn5.Text
                                                    If compstr.ToLower.Contains(ItemSearchTextBox.Text.ToLower) Then
                                                        AdvItemTreeView.Nodes.Add(tn5.Name, tn5.Text)
                                                    End If
                                                End If
                                            Next tn5
                                        Else
                                            Dim compstr As String = tn4.Text
                                            If compstr.ToLower.Contains(ItemSearchTextBox.Text.ToLower) Then
                                                AdvItemTreeView.Nodes.Add(tn4.Name, tn4.Text)
                                            End If
                                        End If
                                    Next tn4
                                Else
                                    Dim compstr As String = tn3.Text
                                    If compstr.ToLower.Contains(ItemSearchTextBox.Text.ToLower) Then
                                        AdvItemTreeView.Nodes.Add(tn3.Name, tn3.Text)
                                    End If
                                End If
                            Next tn3
                        Else
                            Dim compstr As String = tn2.Text
                            If compstr.ToLower.Contains(ItemSearchTextBox.Text.ToLower) Then
                                AdvItemTreeView.Nodes.Add(tn2.Name, tn2.Text)
                            End If
                        End If
                    Next tn2
                Else
                    Dim compstr As String = tn.Text
                    If compstr.ToLower.Contains(ItemSearchTextBox.Text.ToLower) Then
                        AdvItemTreeView.Nodes.Add(tn.Name, tn.Text)
                    End If
                End If
            Next tn


        Else
            ItemTreeSearch.Nodes.Clear()
            ItemTreeSearch.Visible = True
            ItemTree.Visible = False
            For Each tn As TreeNode In ItemTree.Nodes
                If tn.Name.StartsWith("node") Then
                    'This is a category node, do not add it to list
                    For Each tn2 As TreeNode In tn.Nodes
                        If tn2.Name.StartsWith("node") Then
                            'This is a category node, do not add it to list
                            For Each tn3 As TreeNode In tn2.Nodes
                                If tn3.Name.StartsWith("node") Then
                                    'This is a category node, do not add it to list
                                    For Each tn4 As TreeNode In tn3.Nodes
                                        If tn4.Name.StartsWith("node") Then
                                            'This is a category node, do not add it to list
                                            For Each tn5 As TreeNode In tn4.Nodes
                                                If tn5.Name.StartsWith("node") Then
                                                    'This is a category node, do not add it to list
                                                    For Each tn6 As TreeNode In tn5.Nodes
                                                        If tn6.Name.StartsWith("node") Then
                                                            'This is a category node, do not add it to list
                                                            For Each tn7 As TreeNode In tn6.Nodes
                                                                If tn7.Name.StartsWith("node") Then
                                                                    'This is a category node, do not add it to list
                                                                    For Each tn8 As TreeNode In tn7.Nodes
                                                                        If tn8.Name.StartsWith("node") Then
                                                                            'This is a category node, do not add it to list
                                                                        Else
                                                                            Dim compstr As String = tn8.Text
                                                                            If compstr.ToLower.Contains(ItemSearchTextBox.Text.ToLower) Then
                                                                                ItemTreeSearch.Nodes.Add(tn8.Name, tn8.Text)
                                                                            End If
                                                                        End If
                                                                    Next tn8
                                                                Else
                                                                    Dim compstr As String = tn7.Text
                                                                    If compstr.ToLower.Contains(ItemSearchTextBox.Text.ToLower) Then
                                                                        ItemTreeSearch.Nodes.Add(tn7.Name, tn7.Text)
                                                                    End If
                                                                End If
                                                            Next tn7
                                                        Else
                                                            Dim compstr As String = tn6.Text
                                                            If compstr.ToLower.Contains(ItemSearchTextBox.Text.ToLower) Then
                                                                ItemTreeSearch.Nodes.Add(tn6.Name, tn6.Text)
                                                            End If
                                                        End If
                                                    Next tn6
                                                Else
                                                    Dim compstr As String = tn5.Text
                                                    If compstr.ToLower.Contains(ItemSearchTextBox.Text.ToLower) Then
                                                        ItemTreeSearch.Nodes.Add(tn5.Name, tn5.Text)
                                                    End If
                                                End If
                                            Next tn5
                                        Else
                                            Dim compstr As String = tn4.Text
                                            If compstr.ToLower.Contains(ItemSearchTextBox.Text.ToLower) Then
                                                ItemTreeSearch.Nodes.Add(tn4.Name, tn4.Text)
                                            End If
                                        End If
                                    Next tn4
                                Else
                                    Dim compstr As String = tn3.Text
                                    If compstr.ToLower.Contains(ItemSearchTextBox.Text.ToLower) Then
                                        ItemTreeSearch.Nodes.Add(tn3.Name, tn3.Text)
                                    End If
                                End If
                            Next tn3
                        Else
                            Dim compstr As String = tn2.Text
                            If compstr.ToLower.Contains(ItemSearchTextBox.Text.ToLower) Then
                                ItemTreeSearch.Nodes.Add(tn2.Name, tn2.Text)
                            End If
                        End If
                    Next tn2
                Else
                    Dim compstr As String = tn.Text
                    If compstr.ToLower.Contains(ItemSearchTextBox.Text.ToLower) Then
                        ItemTreeSearch.Nodes.Add(tn.Name, tn.Text)
                    End If
                End If
            Next tn
        End If
    End Sub

    Private Sub SearchTimer_Tick(sender As Object, e As EventArgs) Handles SearchTimer.Tick
        SearchItems()
        SearchTimer.Stop()
    End Sub

    Private Sub ClearSearchBox() Handles ItemSearchTextBox.GotFocus
        ItemSearchTextBox.Text = ""
    End Sub

    Private Sub OrderSearch(e As TreeViewEventArgs)
        If API_Connected = True Then
            Dim queryitemid As String = e.Node.Name.Remove(0, 4)
            If e.Node.Name IsNot "" Then
                If e.Node.Name.StartsWith("item") Then
                    If e.Node.Name = "itemnil" Then
                        NewEventMsg("Unknown ID for item: " & e.Node.Text & "! - You can help us find it by placing a buy or sell order for one, and uploading the log via the client.")
                    Else
                        NewEventMsg("Sending Read request for: " & e.Node.Text & " - ID: " & queryitemid)
                        ResetEconomyStatLabels()
                        Dim TempResponse4 As String = API_Request("read", "itemid=" & queryitemid)
                        If TempResponse4 Is Nothing Then
                            NewEventMsg("Request failed!")
                        Else
                            If TempResponse4.StartsWith("The remote server returned an error") Then
                                NewEventMsg(TempResponse4)
                            Else
                                ClearDataTables()
                                UpdateSelectedItem(e.Node.Text)
                                If TempResponse4 = "false" Then
                                    NewEventMsg("Server returned no data for item.")
                                Else
                                    Dim first As Boolean = True
                                    While TempResponse4.Length > 128
                                        Dim orderid3 As String
                                        If first = True Then
                                            orderid3 = TempResponse4.Remove(0, 2)
                                            orderid3 = orderid3.Remove(orderid3.IndexOf(""":"))
                                            first = False
                                        Else
                                            orderid3 = TempResponse4.Remove(0, TempResponse4.IndexOf("},""") + 3)
                                            orderid3 = orderid3.Remove(orderid3.IndexOf(""""))
                                            TempResponse4 = TempResponse4.Remove(0, TempResponse4.IndexOf("},""") + 3)
                                        End If
                                        Dim marketid3 As String = TempResponse4.Remove(0, TempResponse4.IndexOf(""":""") + 3)
                                        marketid3 = marketid3.Remove(marketid3.IndexOf(""","""))
                                        TempResponse4 = TempResponse4.Remove(0, TempResponse4.IndexOf(""",""") + 3)
                                        Dim itemid3 As String = TempResponse4.Remove(0, TempResponse4.IndexOf(""":""") + 3)
                                        itemid3 = itemid3.Remove(itemid3.IndexOf(""","""))
                                        TempResponse4 = TempResponse4.Remove(0, TempResponse4.IndexOf(""",""") + 3)
                                        Dim quantity3 As String = TempResponse4.Remove(0, TempResponse4.IndexOf(""":""") + 3)
                                        quantity3 = quantity3.Remove(quantity3.IndexOf(""","""))
                                        TempResponse4 = TempResponse4.Remove(0, TempResponse4.IndexOf(""",""") + 3)
                                        Dim ordertype3 As String = TempResponse4.Remove(0, TempResponse4.IndexOf(""":""") + 3)
                                        ordertype3 = ordertype3.Remove(ordertype3.IndexOf(""","""))
                                        TempResponse4 = TempResponse4.Remove(0, TempResponse4.IndexOf(""",""") + 3)
                                        Dim expdate3 As String = TempResponse4.Remove(0, TempResponse4.IndexOf(""":""") + 3)
                                        expdate3 = expdate3.Remove(expdate3.IndexOf(""","""))
                                        TempResponse4 = TempResponse4.Remove(0, TempResponse4.IndexOf(""",""") + 3)
                                        Dim lastupdated3 As String = TempResponse4.Remove(0, TempResponse4.IndexOf(""":""") + 3)
                                        lastupdated3 = lastupdated3.Remove(lastupdated3.IndexOf(""","""))
                                        TempResponse4 = TempResponse4.Remove(0, TempResponse4.IndexOf(""",""") + 3)
                                        Dim price3 As String = TempResponse4.Remove(0, TempResponse4.IndexOf(""":""") + 3)
                                        price3 = price3.Remove(price3.IndexOf(""","""))
                                        'Price values that come in from logs or the API, need to be divided by 100. EG 327189 is actually 3271.89, and should be displayed that way
                                        Dim price3String As String
                                        If price3.Length > 2 Then
                                            Dim price3temp As String = price3.Remove(0, price3.Length - 2)
                                            price3String = price3.Remove(price3.Length - 2, 2) & "." & price3temp
                                        Else
                                            price3String = "." & price3
                                        End If
                                        If ordertype3 = 1 Then
                                            API_Buy_Orders.Rows.Add(marketid3, orderid3, itemid3, quantity3, price3, expdate3, lastupdated3)
                                            API_Buy_Orders_UI.Rows.Add(GetMarketName(marketid3), quantity3, price3String, GetTimeRemaining(expdate3, orderid3), e.Node.Text)
                                            EconomyStat_Buy_Total = EconomyStat_Buy_Total + (CInt(price3) * 0.01) * CInt(quantity3)
                                            EconomyStat_Buy_Vol = EconomyStat_Buy_Vol + CInt(quantity3)
                                            If (CInt(price3) * 0.01) > EconomyStat_Buy_High Then
                                                EconomyStat_Buy_High = (CInt(price3) * 0.01)
                                            End If
                                            If (CInt(price3) * 0.01) < EconomyStat_Buy_Low Then
                                                EconomyStat_Buy_Low = (CInt(price3) * 0.01)
                                            End If
                                            If EconomyStat_Buy_Low = 0 Then
                                                EconomyStat_Buy_Low = (CInt(price3) * 0.01)
                                            End If
                                            EconomyStat_Buy_Avg = Math.Round(EconomyStat_Buy_Total / EconomyStat_Buy_Vol, 2)
                                        End If
                                        If ordertype3 = 2 Then
                                            If quantity3.StartsWith("-") Then
                                                quantity3 = quantity3.Remove(0, 1)
                                            End If
                                            API_Sell_Orders.Rows.Add(marketid3, orderid3, itemid3, quantity3, price3, expdate3, lastupdated3)
                                            API_Sell_Orders_UI.Rows.Add(GetMarketName(marketid3), quantity3, price3String, GetTimeRemaining(expdate3, orderid3), e.Node.Text)
                                            EconomyStat_Sell_Total = EconomyStat_Sell_Total + (CInt(price3) * 0.01) * CInt(quantity3)
                                            EconomyStat_Sell_Vol = EconomyStat_Sell_Vol + CInt(quantity3)
                                            If (CInt(price3) * 0.01) > EconomyStat_Sell_High Then
                                                EconomyStat_Sell_High = (CInt(price3) * 0.01)
                                            End If
                                            If (CInt(price3) * 0.01) < EconomyStat_Buy_Low Then
                                                EconomyStat_Sell_Low = (CInt(price3) * 0.01)
                                            End If
                                            If EconomyStat_Sell_Low = 0 Then
                                                EconomyStat_Sell_Low = (CInt(price3) * 0.01)
                                            End If
                                            EconomyStat_Sell_Avg = Math.Round(EconomyStat_Sell_Total / EconomyStat_Sell_Vol, 2)
                                        End If
                                        Application.DoEvents()
                                    End While
                                    ResetDataTables()
                                    SetEconomyStatLabels()
                                End If
                            End If
                        End If
                    End If
                End If
            End If
        Else
            NewEventMsg("Cannot send Read Request - Not logged in.")
        End If
    End Sub

    '########## Advanced Item Search filtering ##########
    Private Sub AdvFilteringToggleButton_Click(sender As Object, e As EventArgs) Handles AdvFilteringToggleButton.Click
        If ShowFilters = False Then
            AdvFilteringToggleButton.Text = "Hide Advanced Filtering"
            ItemTree.Visible = False
            ItemTree.Enabled = False
            ItemTreeSearch.Visible = False
            ItemTreeSearch.Enabled = False
            FilterOrdersButton.Visible = True
            FilterResetButton.Visible = True
            FilterPanel.Visible = True
            FilterPanel.BringToFront()
            ShowFilters = True
            If ShowBookmarks = True Then
                LoadBookmarks()
            End If
        Else
            AdvFilteringToggleButton.Text = "Show Advanced Filtering"
            ItemTree.Visible = True
            ItemTree.Enabled = True
            ItemTreeSearch.Visible = True
            ItemTreeSearch.Enabled = True
            FilterOrdersButton.Visible = False
            FilterResetButton.Visible = False
            FilterPanel.Visible = False
            FilterPanel.SendToBack()
            ShowFilters = False
            If ShowBookmarks = True Then
                LoadBookmarks()
            End If
        End If
    End Sub

    Private Sub FilterOrdersButton_Click(sender As Object, e As EventArgs) Handles FilterOrdersButton.Click
        If CenterUXPanelMode = 1 Then
            AdvOrderSearch()
        ElseIf CenterUXPanelMode = 2 Then
            GenerateHistogram()
        End If
    End Sub

    Private Sub AdvOrderSearch()
        ResetAdvFilters()
        UpdateSelectedItem("")
        If FilterPriceMinBox.Text IsNot "" And FilterPriceMinBox.Text IsNot Nothing Then
            FilterPriceMin = CLng(FilterPriceMinBox.Text)
        End If
        If FilterPriceMaxBox.Text IsNot "" And FilterPriceMaxBox.Text IsNot Nothing Then
            FilterPriceMax = CLng(FilterPriceMaxBox.Text)
        End If
        If FilterQuantityMinBox.Text IsNot "" And FilterQuantityMinBox.Text IsNot Nothing Then
            FilterQuantityMin = CLng(FilterQuantityMinBox.Text)
        End If
        If FilterQuantityMaxBox.Text IsNot "" And FilterQuantityMaxBox.Text IsNot Nothing Then
            FilterQuantityMax = CLng(FilterQuantityMaxBox.Text)
        End If
        For Each selectedMarketNode As TreeNode In AdvMarketTreeView.Nodes
            If selectedMarketNode.Checked = True Then
                Dim NewItem2 As FilterMarketListStructure
                NewItem2.Name = selectedMarketNode.Name
                NewItem2.Text = selectedMarketNode.Text
                FilterMarketList.Add(NewItem2)
            End If
        Next selectedMarketNode
        For Each selectedItemNode As TreeNode In AdvItemTreeView.Nodes
            If selectedItemNode.Checked = True Then
                Dim NewItem3 As FilterItemListStructure
                NewItem3.Name = selectedItemNode.Name
                NewItem3.Text = selectedItemNode.Text
                FilterItemList.Add(NewItem3)
            End If
        Next selectedItemNode
        ClearDataTables()
        Dim tempfirst As Boolean = True
        For Each filterItem As FilterItemListStructure In FilterItemList
            If tempfirst = True Then
                UpdateSelectedItem(filterItem.Text)
                tempfirst = False
            Else
                UpdateSelectedItem(SelectedItemLabel.Text & ", " & filterItem.Text)
            End If
            For Each filterMarket As FilterMarketListStructure In FilterMarketList
                If API_Connected = True Then
                    Dim queryitemid As String = filterItem.Name.Remove(0, 4)
                    If filterItem.Name IsNot "" Then
                        If filterItem.Name.StartsWith("item") Then
                            If filterItem.Name = "itemnil" Then
                                NewEventMsg("Unknown ID for item: " & filterItem.Text & "! - You can help us find it by placing a buy or sell order for one, and uploading the log via the client.")
                            Else
                                NewEventMsg("Sending Read request for: " & filterItem.Text & " - ID: " & queryitemid)
                                ResetEconomyStatLabels()
                                Dim TempResponse4 As String = API_Request("read", "itemid=" & queryitemid & "&marketid=" & filterMarket.Name) ' & filterstring
                                If TempResponse4 Is Nothing Then
                                    NewEventMsg("Request failed!")
                                Else
                                    If TempResponse4.StartsWith("The remote server returned an error") Then
                                        NewEventMsg(TempResponse4)
                                    Else
                                        If TempResponse4 = "false" Then
                                            NewEventMsg("Server returned no data for item.")
                                        Else
                                            Dim first As Boolean = True
                                            While TempResponse4.Length > 128
                                                Dim orderid3 As String
                                                If first = True Then
                                                    orderid3 = TempResponse4.Remove(0, 2)
                                                    orderid3 = orderid3.Remove(orderid3.IndexOf(""":"))
                                                    first = False
                                                Else
                                                    orderid3 = TempResponse4.Remove(0, TempResponse4.IndexOf("},""") + 3)
                                                    orderid3 = orderid3.Remove(orderid3.IndexOf(""""))
                                                    TempResponse4 = TempResponse4.Remove(0, TempResponse4.IndexOf("},""") + 3)
                                                End If
                                                Dim marketid3 As String = TempResponse4.Remove(0, TempResponse4.IndexOf(""":""") + 3)
                                                marketid3 = marketid3.Remove(marketid3.IndexOf(""","""))
                                                TempResponse4 = TempResponse4.Remove(0, TempResponse4.IndexOf(""",""") + 3)
                                                Dim itemid3 As String = TempResponse4.Remove(0, TempResponse4.IndexOf(""":""") + 3)
                                                itemid3 = itemid3.Remove(itemid3.IndexOf(""","""))
                                                TempResponse4 = TempResponse4.Remove(0, TempResponse4.IndexOf(""",""") + 3)
                                                Dim quantity3 As String = TempResponse4.Remove(0, TempResponse4.IndexOf(""":""") + 3)
                                                quantity3 = quantity3.Remove(quantity3.IndexOf(""","""))
                                                TempResponse4 = TempResponse4.Remove(0, TempResponse4.IndexOf(""",""") + 3)
                                                Dim ordertype3 As String = TempResponse4.Remove(0, TempResponse4.IndexOf(""":""") + 3)
                                                ordertype3 = ordertype3.Remove(ordertype3.IndexOf(""","""))
                                                TempResponse4 = TempResponse4.Remove(0, TempResponse4.IndexOf(""",""") + 3)
                                                Dim expdate3 As String = TempResponse4.Remove(0, TempResponse4.IndexOf(""":""") + 3)
                                                expdate3 = expdate3.Remove(expdate3.IndexOf(""","""))
                                                TempResponse4 = TempResponse4.Remove(0, TempResponse4.IndexOf(""",""") + 3)
                                                Dim lastupdated3 As String = TempResponse4.Remove(0, TempResponse4.IndexOf(""":""") + 3)
                                                lastupdated3 = lastupdated3.Remove(lastupdated3.IndexOf(""","""))
                                                TempResponse4 = TempResponse4.Remove(0, TempResponse4.IndexOf(""",""") + 3)
                                                Dim price3 As String = TempResponse4.Remove(0, TempResponse4.IndexOf(""":""") + 3)
                                                price3 = price3.Remove(price3.IndexOf(""","""))
                                                'Price values that come in from logs or the API, need to be divided by 100. EG 327189 is actually 3271.89, and should be displayed that way
                                                Dim price3String As String
                                                If price3.Length > 2 Then
                                                    Dim price3temp As String = price3.Remove(0, price3.Length - 2)
                                                    price3String = price3.Remove(price3.Length - 2, 2) & "." & price3temp
                                                Else
                                                    price3String = "." & price3
                                                End If
                                                If ordertype3 = 1 Then
                                                    Dim pass As Boolean = True
                                                    If Not FilterPriceMin = Nothing Then
                                                        If Convert.ToInt64(price3) < FilterPriceMin Then
                                                            pass = False
                                                        End If
                                                    End If
                                                    If Not FilterPriceMax = Nothing Then
                                                        If Convert.ToInt64(price3) > FilterPriceMax Then
                                                            pass = False
                                                        End If
                                                    End If
                                                    If Not FilterQuantityMin = Nothing Then
                                                        If Convert.ToInt64(quantity3) < FilterQuantityMin Then
                                                            pass = False
                                                        End If
                                                    End If
                                                    If Not FilterQuantityMax = Nothing Then
                                                        If Convert.ToInt64(quantity3) > FilterQuantityMax Then
                                                            pass = False
                                                        End If
                                                    End If
                                                    If pass = True Then
                                                        API_Buy_Orders.Rows.Add(marketid3, orderid3, itemid3, quantity3, price3, expdate3, lastupdated3)
                                                        API_Buy_Orders_UI.Rows.Add(GetMarketName(marketid3), quantity3, price3String, GetTimeRemaining(expdate3, orderid3), filterItem.Text)
                                                        EconomyStat_Buy_Total = EconomyStat_Buy_Total + (CInt(price3) * 0.01) * CInt(quantity3)
                                                        EconomyStat_Buy_Vol = EconomyStat_Buy_Vol + CInt(quantity3)
                                                        If (CInt(price3) * 0.01) > EconomyStat_Buy_High Then
                                                            EconomyStat_Buy_High = (CInt(price3) * 0.01)
                                                        End If
                                                        If (CInt(price3) * 0.01) < EconomyStat_Buy_Low Then
                                                            EconomyStat_Buy_Low = (CInt(price3) * 0.01)
                                                        End If
                                                        If EconomyStat_Buy_Low = 0 Then
                                                            EconomyStat_Buy_Low = (CInt(price3) * 0.01)
                                                        End If
                                                        EconomyStat_Buy_Avg = Math.Round(EconomyStat_Buy_Total / EconomyStat_Buy_Vol, 2)
                                                    End If
                                                End If
                                                If ordertype3 = 2 Then
                                                    If quantity3.StartsWith("-") Then
                                                        quantity3 = quantity3.Remove(0, 1)
                                                    End If
                                                    Dim pass As Boolean = True
                                                    If Not FilterPriceMin = Nothing Then
                                                        If Convert.ToInt64(price3) < FilterPriceMin Then
                                                            pass = False
                                                        End If
                                                    End If
                                                    If Not FilterPriceMax = Nothing Then
                                                        If Convert.ToInt64(price3) > FilterPriceMax Then
                                                            pass = False
                                                        End If
                                                    End If
                                                    If Not FilterQuantityMin = Nothing Then
                                                        If Convert.ToInt64(quantity3) < FilterQuantityMin Then
                                                            pass = False
                                                        End If
                                                    End If
                                                    If Not FilterQuantityMax = Nothing Then
                                                        If Convert.ToInt64(quantity3) > FilterQuantityMax Then
                                                            pass = False
                                                        End If
                                                    End If
                                                    If pass = True Then
                                                        API_Sell_Orders.Rows.Add(marketid3, orderid3, itemid3, quantity3, price3, expdate3, lastupdated3)
                                                        API_Sell_Orders_UI.Rows.Add(GetMarketName(marketid3), quantity3, price3String, GetTimeRemaining(expdate3, orderid3), filterItem.Text)
                                                        EconomyStat_Sell_Total = EconomyStat_Sell_Total + (CInt(price3) * 0.01) * CInt(quantity3)
                                                        EconomyStat_Sell_Vol = EconomyStat_Sell_Vol + CInt(quantity3)
                                                        If (CInt(price3) * 0.01) > EconomyStat_Sell_High Then
                                                            EconomyStat_Sell_High = (CInt(price3) * 0.01)
                                                        End If
                                                        If (CInt(price3) * 0.01) < EconomyStat_Buy_Low Then
                                                            EconomyStat_Sell_Low = (CInt(price3) * 0.01)
                                                        End If
                                                        If EconomyStat_Sell_Low = 0 Then
                                                            EconomyStat_Sell_Low = (CInt(price3) * 0.01)
                                                        End If
                                                        EconomyStat_Sell_Avg = Math.Round(EconomyStat_Sell_Total / EconomyStat_Sell_Vol, 2)
                                                    End If
                                                End If
                                                Application.DoEvents()
                                                SetEconomyStatLabels()
                                            End While
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If
                Else
                    NewEventMsg("Cannot send Read Request - Not logged in.")
                End If
            Next filterMarket
        Next filterItem
        ResetDataTables()
    End Sub

    Private Sub FilterResetButton_Click(sender As Object, e As EventArgs) Handles FilterResetButton.Click
        ResetAdvFilterUI()
    End Sub

    Private Sub ResetAdvFilters()
        FilterMarketList.Clear()
        FilterItemList.Clear()
        FilterPriceMin = Nothing
        FilterPriceMax = Nothing
        FilterQuantityMin = Nothing
        FilterQuantityMax = Nothing
    End Sub

    Private Sub ResetAdvFilterUI()
        ResetAdvFilters()
        FilterPriceMinBox.Text = ""
        FilterPriceMaxBox.Text = ""
        FilterQuantityMinBox.Text = ""
        FilterQuantityMaxBox.Text = ""
        For Each selectedMarketNode2 As TreeNode In AdvMarketTreeView.Nodes
            selectedMarketNode2.Checked = False
        Next selectedMarketNode2
        For Each selectedItemNode2 As TreeNode In AdvItemTreeView.Nodes
            selectedItemNode2.Checked = False
        Next selectedItemNode2
    End Sub

    '########## Order-Histogram Panel Controls ##########
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        GraphingControlsPanel.Visible = True
        HistogramPanel.Visible = True
        RawOrderTable.Visible = False
        OrderTablePanel.Visible = False
        CenterUXPanelMode = 2
        'disable advanced searching for this might make this work eventually but for now, easiest to disable it.
        If ShowFilters = False Then
            AdvFilteringToggleButton.Text = "Hide Advanced Filtering"
            ItemTree.Visible = False
            ItemTree.Enabled = False
            ItemTreeSearch.Visible = False
            ItemTreeSearch.Enabled = False
            FilterOrdersButton.Visible = True
            FilterResetButton.Visible = True
            FilterPanel.Visible = True
            FilterPanel.BringToFront()
            ShowFilters = True
            If ShowBookmarks = True Then
                LoadBookmarks()
            End If
        End If
        GroupBox2.Visible = False
        GroupBox3.Visible = False
        AdvFilteringToggleButton.Enabled = False
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        GraphingControlsPanel.Visible = False
        HistogramPanel.Visible = False
        RawOrderTable.Visible = True
        OrderTablePanel.Visible = True
        CenterUXPanelMode = 1
        GroupBox2.Visible = True
        GroupBox3.Visible = True
        AdvFilteringToggleButton.Enabled = True
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        If ComboBox1.SelectedIndex = 0 Then
            HistEntries = 8
        End If
        If ComboBox1.SelectedIndex = 1 Then
            HistEntries = 17
        End If
        If ComboBox1.SelectedIndex = 2 Then
            HistEntries = 31
        End If
        If ComboBox1.SelectedIndex = 3 Then
            HistEntries = 91
        End If
        If ComboBox1.SelectedIndex = 4 Then
            HistEntries = 181
        End If
        If ComboBox1.SelectedIndex = 5 Then
            HistEntries = 366
        End If
    End Sub

    '########## Histogram Generation ##########
    Private Sub GenerateHistogram()
        ResetAdvFilters()
        UpdateSelectedItem("")
        For Each selectedMarketNode As TreeNode In AdvMarketTreeView.Nodes
            If selectedMarketNode.Checked = True Then
                Dim NewItem2 As FilterMarketListStructure
                NewItem2.Name = selectedMarketNode.Name
                NewItem2.Text = selectedMarketNode.Text
                FilterMarketList.Add(NewItem2)
            End If
        Next selectedMarketNode
        For Each selectedItemNode As TreeNode In AdvItemTreeView.Nodes
            If selectedItemNode.Checked = True Then
                Dim NewItem3 As FilterItemListStructure
                NewItem3.Name = selectedItemNode.Name
                NewItem3.Text = selectedItemNode.Text
                FilterItemList.Add(NewItem3)
            End If
        Next selectedItemNode
        If FilterMarketList.Count = 1 Then
            If FilterMarketList.Count = 1 Then
                Dim tempfirst As Boolean = True
                For Each filterItem As FilterItemListStructure In FilterItemList
                    If tempfirst = True Then
                        UpdateSelectedItem(filterItem.Text)
                        tempfirst = False
                    Else
                        UpdateSelectedItem(SelectedItemLabel.Text & ", " & filterItem.Text)
                    End If
                    For Each filterMarket As FilterMarketListStructure In FilterMarketList
                        If API_Connected = True Then
                            Dim queryitemid As String = filterItem.Name.Remove(0, 4)
                            If filterItem.Name IsNot "" Then
                                If filterItem.Name.StartsWith("item") Then
                                    If filterItem.Name = "itemnil" Then
                                        NewEventMsg("Unknown ID for item: " & filterItem.Text & "! - You can help us find it by placing a buy or sell order for one, and uploading the log via the client.")
                                    Else
                                        NewEventMsg("Sending History request for: " & filterItem.Text & " (ID: " & queryitemid & ")")
                                        Dim TempResponse4 As String = API_Request("history", "itemid=" & queryitemid & "&marketid=" & filterMarket.Name & "&entries=" & HistEntries) '& queryitemid)
                                        If TempResponse4 Is Nothing Then
                                            NewEventMsg("Request failed!")
                                        Else
                                            Try
                                                If TempResponse4.StartsWith("The remote server returned an error") Then
                                                    NewEventMsg(TempResponse4)
                                                Else
                                                    UpdateSelectedItem(filterItem.Text)
                                                    If TempResponse4 = "false" Then
                                                        NewEventMsg("Server returned no data for item.")
                                                    Else
                                                        Me.HistogramChart.Series("Sell Orders").Points.Clear()
                                                        Me.HistogramChart.Series("Buy Orders").Points.Clear()
                                                        Dim first As Boolean = True
                                                        While TempResponse4.Length > 48
                                                            Dim historderid As String
                                                            If first = True Then
                                                                historderid = TempResponse4.Remove(0, 2)
                                                                historderid = historderid.Remove(historderid.IndexOf(""":"))
                                                                first = False
                                                            Else
                                                                historderid = TempResponse4.Remove(0, TempResponse4.IndexOf("},""") + 3)
                                                                historderid = historderid.Remove(historderid.IndexOf(""""))
                                                                TempResponse4 = TempResponse4.Remove(0, TempResponse4.IndexOf("},""") + 3)
                                                            End If
                                                            Dim histdate As String = TempResponse4.Remove(0, TempResponse4.IndexOf(""":""") + 3)
                                                            histdate = histdate.Remove(histdate.IndexOf(""","""))
                                                            TempResponse4 = TempResponse4.Remove(0, TempResponse4.IndexOf(""",""") + 3)
                                                            Dim histmarketid As String = TempResponse4.Remove(0, TempResponse4.IndexOf(""":""") + 3)
                                                            histmarketid = histmarketid.Remove(histmarketid.IndexOf(""","""))
                                                            TempResponse4 = TempResponse4.Remove(0, TempResponse4.IndexOf(""",""") + 3)
                                                            Dim histbuyvolume As String = TempResponse4.Remove(0, TempResponse4.IndexOf(""":""") + 3)
                                                            histbuyvolume = histbuyvolume.Remove(histbuyvolume.IndexOf(""","""))
                                                            TempResponse4 = TempResponse4.Remove(0, TempResponse4.IndexOf(""",""") + 3)
                                                            Dim histsellvolume As String = TempResponse4.Remove(0, TempResponse4.IndexOf(""":""") + 3)
                                                            histsellvolume = histsellvolume.Remove(histsellvolume.IndexOf(""","""))
                                                            TempResponse4 = TempResponse4.Remove(0, TempResponse4.IndexOf(""",""") + 3)
                                                            Dim histbuytotal As String = TempResponse4.Remove(0, TempResponse4.IndexOf(""":""") + 3)
                                                            histbuytotal = histbuytotal.Remove(histbuytotal.IndexOf(""","""))
                                                            TempResponse4 = TempResponse4.Remove(0, TempResponse4.IndexOf(""",""") + 3)
                                                            Dim histselltotal As String = TempResponse4.Remove(0, TempResponse4.IndexOf(""":""") + 3)
                                                            histselltotal = histselltotal.Remove(histselltotal.IndexOf(""","""))
                                                            TempResponse4 = TempResponse4.Remove(0, TempResponse4.IndexOf(""",""") + 3)
                                                            Dim histbuymax As String = TempResponse4.Remove(0, TempResponse4.IndexOf(""":""") + 3)
                                                            histbuymax = histbuymax.Remove(histbuymax.IndexOf(""","""))
                                                            TempResponse4 = TempResponse4.Remove(0, TempResponse4.IndexOf(""",""") + 3)
                                                            Dim histbuyavg As String = TempResponse4.Remove(0, TempResponse4.IndexOf(""":""") + 3)
                                                            histbuyavg = histbuyavg.Remove(histbuyavg.IndexOf(""","""))
                                                            TempResponse4 = TempResponse4.Remove(0, TempResponse4.IndexOf(""",""") + 3)
                                                            Dim histbuymin As String = TempResponse4.Remove(0, TempResponse4.IndexOf(""":""") + 3)
                                                            histbuymin = histbuymin.Remove(histbuymin.IndexOf(""","""))
                                                            TempResponse4 = TempResponse4.Remove(0, TempResponse4.IndexOf(""",""") + 3)
                                                            Dim histsellmax As String = TempResponse4.Remove(0, TempResponse4.IndexOf(""":""") + 3)
                                                            histsellmax = histsellmax.Remove(histsellmax.IndexOf(""","""))
                                                            TempResponse4 = TempResponse4.Remove(0, TempResponse4.IndexOf(""",""") + 3)
                                                            Dim histsellavg As String = TempResponse4.Remove(0, TempResponse4.IndexOf(""":""") + 3)
                                                            histsellavg = histsellavg.Remove(histsellavg.IndexOf(""","""))
                                                            TempResponse4 = TempResponse4.Remove(0, TempResponse4.IndexOf(""",""") + 3)
                                                            Dim histsellmin As String = TempResponse4.Remove(0, TempResponse4.IndexOf(""":""") + 3)
                                                            histsellmin = histsellmin.Remove(histsellmin.IndexOf(""""))
                                                            'ignore average values for now, but we will draw them in later
                                                            If histbuymax.Length > 2 Then

                                                            End If
                                                            Me.HistogramChart.Series("Buy Orders").Points.AddXY(histdate, CInt(histbuyavg) * 0.01, CInt(histbuymin) * 0.01)
                                                            Me.HistogramChart.Series("Sell Orders").Points.AddXY(histdate, CInt(histsellavg) * 0.01, CInt(histsellmin) * 0.01)
                                                            Application.DoEvents()
                                                        End While
                                                        NewEventMsg("Loaded history for " & filterItem.Text & " (ID: " & queryitemid & ")")
                                                    End If
                                                End If
                                            Catch ex As Exception
                                                NewEventMsg(ex.Message)
                                                NewEventMsg("SERVER RESPONSE: " & TempResponse4)
                                            End Try
                                        End If
                                    End If
                                End If
                            End If
                        Else
                            NewEventMsg("Cannot send Read Request - Not logged in.")
                        End If
                    Next filterMarket
                Next filterItem
            Else
                NewEventMsg("Only a single item selection is currently supported in Graphing Mode.")
            End If
        Else
            NewEventMsg("Only a single market selection is currently supported in Graphing Mode.")
        End If
    End Sub

    '########## Search Bookmarks Panel ##########
    Private Sub BookmarkButton_Click(sender As Object, e As EventArgs) Handles BookmarkButton.Click
        If ShowBookmarks = False Then
            ShowBookmarks = True
            BookmarkPanel.Show()
            BookmarkPanel.BringToFront()
            LoadBookmarks()
        Else
            ShowBookmarks = False
            ItemTreeBookmarks.Nodes.Clear()
            BookmarkPanel.Hide()
            BookmarkPanel.SendToBack()
        End If
    End Sub

    Private Sub AddBookmarkButton_Click(sender As Object, e As EventArgs) Handles AddBookmarkButton.Click
        Try
            Dim emptyFound As Boolean = False
            Dim its As Integer = 0
            Dim iniCategory1 As String = ""
            If ShowFilters = True Then
                iniCategory1 = "BookmarksAdvanced"
            Else
                iniCategory1 = "Bookmarks"
            End If
            While emptyFound = False
                Dim bkmrkLoadString1 As String = ""
                bkmrkLoadString1 = GetIniValue(iniCategory1, "save" & CStr(its), My.Application.Info.DirectoryPath & "\DUOMbookmarks.ini")
                If bkmrkLoadString1 = Nothing Or bkmrkLoadString1.Trim = "" Then
                    emptyFound = True
                Else
                    its = its + 1
                End If
            End While
            If ShowFilters = True Then
                Dim bkmrkfilterstring As String = ""
                For bkmrkita As Integer = 1 To AdvMarketTreeView.Nodes.Count
                    If AdvMarketTreeView.Nodes(bkmrkita - 1).Checked = True Then
                        bkmrkfilterstring = bkmrkfilterstring & ",market=" & AdvMarketTreeView.Nodes(bkmrkita - 1).Name
                    End If
                Next bkmrkita
                For bkmrkitb As Integer = 1 To AdvItemTreeView.Nodes.Count
                    If AdvItemTreeView.Nodes(bkmrkitb - 1).Checked = True Then
                        bkmrkfilterstring = bkmrkfilterstring & ",item=" & AdvItemTreeView.Nodes(bkmrkitb - 1).Name
                    End If
                Next bkmrkitb
                SetIniValue(iniCategory1, "save" & CStr(its), My.Application.Info.DirectoryPath & "\DUOMbookmarks.ini", ItemSearchTextBox.Text & "," & FilterPriceMinBox.Text & "," & FilterPriceMaxBox.Text & "," & FilterQuantityMinBox.Text & "," & FilterQuantityMaxBox.Text & bkmrkfilterstring)
            Else
                SetIniValue(iniCategory1, "save" & CStr(its), My.Application.Info.DirectoryPath & "\DUOMbookmarks.ini", ItemSearchTextBox.Text)
            End If
            NewEventMsg("Bookmark Saved.")
            LoadBookmarks()
        Catch ex As Exception
            NewEventMsg("Bookmark Save Error:  " & ex.Message)
        End Try
    End Sub

    Private Sub LoadBookmarks()
        ItemTreeBookmarks.Nodes.Clear()
        Dim bkmrkLoad As Boolean = False
        Dim its As Integer = 0
        Dim iniCategory2 As String = ""
        If ShowFilters = True Then
            iniCategory2 = "BookmarksAdvanced"
        Else
            iniCategory2 = "Bookmarks"
        End If
        While bkmrkLoad = False
            Dim bkmrkLoadString As String
            bkmrkLoadString = GetIniValue(iniCategory2, "save" & CStr(its), My.Application.Info.DirectoryPath & "\DUOMbookmarks.ini")
            If bkmrkLoadString = Nothing Or bkmrkLoadString.Trim = "" Then
                bkmrkLoad = True
            Else
                If ShowFilters = True Then
                    ItemTreeBookmarks.Nodes.Add(bkmrkLoadString.Remove(bkmrkLoadString.IndexOf(","), bkmrkLoadString.Length - bkmrkLoadString.IndexOf(",")))
                Else
                    ItemTreeBookmarks.Nodes.Add(bkmrkLoadString)
                End If

            End If
            its = its + 1
        End While
    End Sub

    Private Sub ItemTreeBookmarks_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles ItemTreeBookmarks.AfterSelect
        Dim iniCategory3 As String = ""
        If ShowFilters = True Then
            Dim bkmrkRecallString As String = ""
            bkmrkRecallString = GetIniValue("BookmarksAdvanced", "save" & CStr(e.Node.Index), My.Application.Info.DirectoryPath & "\DUOMbookmarks.ini")
            ItemSearchTextBox.Text = bkmrkRecallString.Remove(bkmrkRecallString.IndexOf(","), bkmrkRecallString.Length - bkmrkRecallString.IndexOf(","))
            bkmrkRecallString = bkmrkRecallString.Remove(0, bkmrkRecallString.IndexOf(",") + 1)
            FilterPriceMinBox.Text = bkmrkRecallString.Remove(bkmrkRecallString.IndexOf(","), bkmrkRecallString.Length - bkmrkRecallString.IndexOf(","))
            bkmrkRecallString = bkmrkRecallString.Remove(1, bkmrkRecallString.IndexOf(",") + 1)
            FilterPriceMaxBox.Text = bkmrkRecallString.Remove(bkmrkRecallString.IndexOf(","), bkmrkRecallString.Length - bkmrkRecallString.IndexOf(","))
            bkmrkRecallString = bkmrkRecallString.Remove(1, bkmrkRecallString.IndexOf(",") + 1)
            FilterQuantityMinBox.Text = bkmrkRecallString.Remove(bkmrkRecallString.IndexOf(","), bkmrkRecallString.Length - bkmrkRecallString.IndexOf(","))
            bkmrkRecallString = bkmrkRecallString.Remove(1, bkmrkRecallString.IndexOf(",") + 1)
            FilterQuantityMaxBox.Text = bkmrkRecallString.Remove(bkmrkRecallString.IndexOf(","), bkmrkRecallString.Length - bkmrkRecallString.IndexOf(","))
            bkmrkRecallString = bkmrkRecallString.Remove(1, bkmrkRecallString.IndexOf(",") + 1)
        Else
            ItemSearchTextBox.Text = GetIniValue("Bookmarks", "save" & CStr(e.Node.Index), My.Application.Info.DirectoryPath & "\DUOMbookmarks.ini")
        End If
    End Sub

    '########## Selected Item Details ##########
    Private Sub UpdateSelectedItem(ByVal selected As String)
        SelectedItemLabel.Text = selected
        If SelectedItemLabel.Text.StartsWith(", ") Then
            SelectedItemLabel.Text = SelectedItemLabel.Text.Remove(0, 2)
        End If
        If SelectedItemLabel.Text.EndsWith(", ") Then
            SelectedItemLabel.Text = SelectedItemLabel.Text.Remove(SelectedItemLabel.Text.Length - 2, 2)
        End If
    End Sub

    Private Sub BuyOrderGridViewRaw_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles BuyOrderGridViewRaw.CellClick
        Try
            SelectedItemLabel.Text = BuyOrderGridViewRaw.Rows(e.RowIndex).Cells(4).Value
        Catch ex As Exception
            'When the user clicks a column header to sort a gridview, it will throw an exception because a DataGridViewCellEventArg with a negative RowIndex is passed. Using trycatch as a way to ignore that.
        End Try
    End Sub

    Private Sub SellOrderGridViewRaw_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles SellOrderGridViewRaw.CellClick
        Try
            SelectedItemLabel.Text = SellOrderGridViewRaw.Rows(e.RowIndex).Cells(4).Value
        Catch ex As Exception
            'When the user clicks a column header to sort a gridview, it will throw an exception because a DataGridViewCellEventArg with a negative RowIndex is passed. Using trycatch as a way to ignore that.
        End Try
    End Sub

    '############################## - File I/O and Parsing - ##############################
    '########## Log File Processing ##########
    Private Sub ReadFileLines(ByVal InputFile As String)
        If File.Exists(InputFile) Then
            Dim fileinfo As FileInfo = My.Computer.FileSystem.GetFileInfo(InputFile)
            If fileinfo.Length < 1 Then
                NewEventMsg("ERROR: Supplied data file contains no data @ " & InputFile)
                Exit Sub
            End If
            Dim fs2 As New FileStream(InputFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
            Dim sr2 As New StreamReader(fs2)
            If _LastOffset < sr2.BaseStream.Length Then
                sr2.BaseStream.Seek(_LastOffset, SeekOrigin.Begin)
                While sr2.Peek() > 0
                    Dim read As String = sr2.ReadLine()
                    If read.Length > 0 Then
                        ProcessLine(read)
                        Application.DoEvents()
                    End If
                End While
                _LastOffset = sr2.BaseStream.Position
            End If
            sr2.Close()
            fs2.Close()
        End If
    End Sub

    Private Sub ProcessLine(ByVal InputString As String)
        If InputString.StartsWith("<message>onUpdateMarketItemOrders: MarketOrders:") Then
            If InputString.EndsWith("</message>") Then
                If InputString = API_Last_Log_Processed Then
                    InputString = ""
                Else
                    API_Last_Log_Processed = InputString
                    InputString = InputString.Remove(0, InputString.IndexOf("MarketOrder:") + 12) 'Trim start
                    'check for case where there are zero orders for given item
                    If InputString.IndexOf("], ]] |") = -1 Then
                        NewEventMsg("No orders for item. Unable to retrieve ID from logs.")
                        InputString = ""
                    Else
                        InputString = InputString.Remove(InputString.IndexOf("], ]] |"), InputString.Length - InputString.IndexOf("], ]] |")) 'Trim end
                        While InputString.Length > 0
                            Dim NextOrder As String
                            If InputString.IndexOf("],") > 0 Then
                                NextOrder = InputString.Remove(InputString.IndexOf("],"), InputString.Length - InputString.IndexOf("],"))
                            Else
                                NextOrder = InputString
                            End If
                            Dim MarketId As String = NextOrder.Remove(NextOrder.IndexOf(","), NextOrder.Length - NextOrder.IndexOf(","))
                            MarketId = MarketId.Remove(0, 11)
                            NextOrder = NextOrder.Remove(0, NextOrder.IndexOf(" = ") + 3)
                            NextOrder = NextOrder.Remove(0, NextOrder.IndexOf(" = ") + 3)
                            Dim OrderId As String = NextOrder.Remove(NextOrder.IndexOf(","), NextOrder.Length - NextOrder.IndexOf(","))
                            NextOrder = NextOrder.Remove(0, NextOrder.IndexOf(" = ") + 3)
                            Dim ItemType As String = NextOrder.Remove(NextOrder.IndexOf(","), NextOrder.Length - NextOrder.IndexOf(","))
                            NextOrder = NextOrder.Remove(0, NextOrder.IndexOf(" = ") + 3)
                            Dim BuyQuantity As String = NextOrder.Remove(NextOrder.IndexOf(","), NextOrder.Length - NextOrder.IndexOf(","))
                            NextOrder = NextOrder.Remove(0, NextOrder.IndexOf(" = ") + 3)
                            Dim ExpDate As String = NextOrder.Remove(NextOrder.IndexOf(","), NextOrder.Length - NextOrder.IndexOf(","))
                            NextOrder = NextOrder.Remove(0, NextOrder.IndexOf(" = ") + 3)
                            Dim LastUpdated As String = NextOrder.Remove(NextOrder.IndexOf(","), NextOrder.Length - NextOrder.IndexOf(","))
                            NextOrder = NextOrder.Remove(0, NextOrder.IndexOf(" = ") + 3)
                            NextOrder = NextOrder.Remove(0, NextOrder.IndexOf(" = ") + 3)
                            Dim Price As String = NextOrder.Remove(NextOrder.IndexOf("]"), NextOrder.Length - NextOrder.IndexOf("]"))
                            If InputString.IndexOf("],") > 0 Then
                                InputString = InputString.Remove(0, InputString.IndexOf("]],") + 16)
                            Else
                                InputString = ""
                            End If
                            Dim NewItem1 As LogInfoStructure
                            NewItem1.marketid = MarketId
                            NewItem1.orderid = OrderId
                            NewItem1.itemtype = ItemType
                            NewItem1.quantity = BuyQuantity
                            NewItem1.expdate = ExpDate
                            NewItem1.lastupdate = LastUpdated
                            NewItem1.price = Price
                            API_Log_Queue.Add(NewItem1)
                            Application.DoEvents()
                        End While
                    End If
                End If
            End If
        End If
    End Sub

    Private Function GetNewestLogFile(ByVal path As String)
        TimeStampInt()
        Dim NewestLogPath As String = ""
        Dim NewestLogDate As Date = New Date(2000, 01, 01, 01, 01, 01)
        For Each NQLogfile As String In My.Computer.FileSystem.GetFiles(path)
            Dim filext As String = NQLogfile.Remove(0, NQLogfile.LastIndexOf("."))
            If filext = ".xml" Then
                Dim filename As String = NQLogfile.Remove(0, NQLogfile.LastIndexOf("\") + 1)
                Dim file_prefix As String = filename.Remove(filename.IndexOf("_") + 1, filename.Length - (filename.IndexOf("_") + 1))
                If file_prefix = "log_" Then
                    Dim filedate1 As String = filename.Remove(0, filename.IndexOf("_") + 1)
                    filedate1 = filedate1.Remove(filedate1.IndexOf("_"), filedate1.Length - (filedate1.IndexOf("_")))
                    Dim fileyear As String = filedate1.Remove(filedate1.IndexOf("-"), filedate1.Length - filedate1.IndexOf("-"))
                    Dim filemonth As String = filedate1.Remove(0, filedate1.IndexOf("-") + 1)
                    filemonth = filemonth.Remove(filemonth.IndexOf("-"), filemonth.Length - filemonth.IndexOf("-"))
                    Dim fileday As String = filedate1.Remove(0, filedate1.LastIndexOf("-") + 1)
                    filedate1 = filename.Remove(0, filename.LastIndexOf("_") + 1)
                    filedate1 = filedate1.Remove(filedate1.LastIndexOf("."), filedate1.Length - filedate1.LastIndexOf("."))
                    Dim filehour As String = filedate1.Remove(2, filedate1.Length - 2)
                    Dim filemin As String = filedate1.Remove(0, 3)
                    filemin = filemin.Remove(2, 4)
                    Dim filesec As String = filedate1.Remove(0, filedate1.Length - 3)
                    filesec = filesec.Remove(2, 1)
                    If CInt(fileyear) = CurrYear Then
                        If CInt(filemonth) = CurrMonth Then
                            If CInt(fileday) = CurrDay Then
                                Dim TempDate As Date = New Date(fileyear, filemonth, fileday, filehour, filemin, filesec)
                                If DateTime.Compare(TempDate, NewestLogDate) > 0 Then
                                    NewestLogDate = TempDate
                                    NewestLogPath = NQLogfile
                                End If
                            End If
                        End If
                    End If
                End If
            End If
        Next NQLogfile
        Return NewestLogPath
    End Function

    Private Sub LogFileCheckTimer_Tick(sender As Object, e As EventArgs) Handles LogFileCheckTimer.Tick
        Dim API_LogFile_Old As String = API_LogFile
        API_LogFile = GetNewestLogFile(API_LogfileDirectory)
        If API_LogFile IsNot Nothing And API_LogFile IsNot "" Then
            If API_LogFile = API_LogFile_Old Then
                'no change to log file, do nothing
            Else
                'There is a new log file, we need to clear the buffer for the old file, and announce this update to the user
                _LastOffset = 0 'reset log cursor position
                NewEventMsg("Current Log File: " & API_LogFile)
            End If
        End If
    End Sub

    '############################## - Queue Processing - ##############################
    Private Sub OperationTimer_Tick(sender As Object, e As EventArgs) Handles OperationTimer.Tick
        Dim hasLock = False
        Try
            Monitor.TryEnter(OperationTimerLocker, hasLock)
            If Not hasLock Then
                Return
            End If
            OperationTimer.Stop()
            If Setting_Processinbatch = "True" Then
                StatLabelQue.Text = API_Log_Queue.Count()
            Else
                StatLabelQue.Text = API_Sanitized_Request_Queue.Count()
            End If
            If API_LogFile = "" Or API_LogFile = Nothing Then
                'Do Nothing
            Else
                Dim fileinfo1 As FileInfo = My.Computer.FileSystem.GetFileInfo(API_LogFile)
                If fileinfo1.Length > CInt(_LastOffset) Then
                    ReadFileLines(API_LogFile)
                End If
                If API_Log_Queue.Count >= 1 Then
                    If Setting_Processinbatch = "True" Then
                        GetBatchList()
                    Else
                        GetLastItemId()
                        ProcessNextInQueue()
                    End If
                End If
                If Setting_Processinbatch = "True" Then
                    ProcessNextInSanitizedQueue()
                End If
            End If
            ConnectionStyling()
            LogBufferLabel.Text = CStr(_LastOffset)
            APIReadsLabel.Text = CStr(NumberOfReads)
            APIUpdatesLabel.Text = CStr(NumberOfUpdates)
            APICreatesLabel.Text = CStr(NumberOfCreates)
            APIDeletesLabel.Text = CStr(NumberOfDeletes)
            APIHistoriesLabel.Text = CStr(NumberOfHistories)
        Finally
            If hasLock Then
                Monitor.[Exit](OperationTimerLocker)
                OperationTimer.Start()
            End If
        End Try
    End Sub

    '########## Single-Order Process ##########
    Private Sub GetLastItemId()
        If CInt(StatLabelProc.Text) > 1 Then
            Dim seen As Boolean = False
            For item As Integer = 1 To UniqueItemIds.Count()
                If UniqueItemIds(item - 1) = API_Log_Queue(0).itemtype Then
                    seen = True
                End If
            Next item
            If seen = False Then
                UniqueItemIds.Add(API_Log_Queue(0).itemtype)
            End If
        Else
            UniqueItemIds.Add(API_Log_Queue(0).itemtype)
        End If
    End Sub

    Private Sub ProcessNextInQueue()
        Dim OrderType As Integer
        If Convert.ToInt64(API_Log_Queue(0).quantity) > 0 Then
            OrderType = 1
        Else
            OrderType = 2
        End If
        Dim TempResponse2 As String = API_Request("read", "orderid=" & API_Log_Queue(0).orderid)
        If TempResponse2 = "false" Then
            'if the server doesnt have this order, then we create it. what could go wrong?
            Dim TempResponse3 As String = API_Request("create", "orderid=" & API_Log_Queue(0).orderid & "&marketid=" & API_Log_Queue(0).marketid & "&itemid=" & API_Log_Queue(0).itemtype & "&quantity=" & API_Log_Queue(0).quantity & "&ordertype=" & OrderType & "&expiration=" & API_Log_Queue(0).expdate & "&lastupdated=" & API_Log_Queue(0).lastupdate & "&price=" & API_Log_Queue(0).price)
        Else
            If TempResponse2 Is Nothing Then
                NewEventMsg("TempResponse2 was null!")
            ElseIf TempResponse2.StartsWith("The remote server returned an error") Then
                NewEventMsg(TempResponse2)
            Else
                If TempResponse2.StartsWith("{") Then
                    Dim marketid2 As String = TempResponse2.Remove(0, TempResponse2.IndexOf(""":""") + 3)
                    marketid2 = marketid2.Remove(marketid2.IndexOf(""","""))
                    TempResponse2 = TempResponse2.Remove(0, TempResponse2.IndexOf(""",""") + 3)
                    Dim itemid2 As String = TempResponse2.Remove(0, TempResponse2.IndexOf(""":""") + 3)
                    itemid2 = itemid2.Remove(itemid2.IndexOf(""","""))
                    TempResponse2 = TempResponse2.Remove(0, TempResponse2.IndexOf(""",""") + 3)
                    Dim quantity2 As String = TempResponse2.Remove(0, TempResponse2.IndexOf(""":""") + 3)
                    quantity2 = quantity2.Remove(quantity2.IndexOf(""","""))
                    TempResponse2 = TempResponse2.Remove(0, TempResponse2.IndexOf(""",""") + 3)
                    TempResponse2 = TempResponse2.Remove(0, TempResponse2.IndexOf(""",""") + 3)
                    Dim expdate2 As String = TempResponse2.Remove(0, TempResponse2.IndexOf(""":""") + 3)
                    expdate2 = expdate2.Remove(expdate2.IndexOf(""","""))
                    TempResponse2 = TempResponse2.Remove(0, TempResponse2.IndexOf(""",""") + 3)
                    Dim lastupdated2 As String = TempResponse2.Remove(0, TempResponse2.IndexOf(""":""") + 3)
                    lastupdated2 = lastupdated2.Remove(lastupdated2.IndexOf(""","""))
                    TempResponse2 = TempResponse2.Remove(0, TempResponse2.IndexOf(""",""") + 3)
                    Dim price As String = TempResponse2.Remove(0, TempResponse2.IndexOf(""":""") + 3)
                    price = price.Remove(price.IndexOf(""","""))
                    If lastupdated2 = API_Log_Queue(0).lastupdate Then
                        'These orders have the same update date, we would need to compare it to the data in the queue to find out if they are different. But that would be very costly. Let's for now, assume the server already has the correct data and do nothing.
                    Else
                        Dim DateComparison1 As String = API_Log_Queue(0).lastupdate.Remove(0, API_Log_Queue(0).lastupdate.IndexOf(" "))
                        Dim DateComparison2 As String = lastupdated2.Remove(0, lastupdated2.IndexOf(" "))
                        Dim d1 = DateTime.Parse(DateComparison1)
                        Dim d2 = DateTime.Parse(DateComparison2)
                        If d1 > d2 Then
                            'this order already exists, so we need to update it with new values
                            Dim TempResponse3 As String = API_Request("update", "orderid=" & API_Log_Queue(0).orderid & "&marketid=" & API_Log_Queue(0).marketid & "&itemid=" & API_Log_Queue(0).itemtype & "&quantity=" & API_Log_Queue(0).quantity & "&ordertype=" & OrderType & "&expiration=" & API_Log_Queue(0).expdate & "&lastupdated=" & API_Log_Queue(0).lastupdate & "&price=" & API_Log_Queue(0).price)
                        End If
                    End If
                End If
            End If
        End If
        StatLabelProc.Text = CInt(StatLabelProc.Text) + 1
        API_Log_Queue.RemoveAt(0)
    End Sub

    '########## Batch-Order Process ##########

    Private Sub GetBatchList()
        CurrentBatchList.Clear()
        Dim currentID As String = API_Log_Queue(0).itemtype
        For qindex As Integer = 1 To API_Log_Queue.Count()
            If API_Log_Queue(qindex - 1).itemtype = currentID Then
                CurrentBatchList.Add(qindex - 1)
                Application.DoEvents()
            End If
        Next qindex
        GetBatchOrderInfo()
    End Sub

    Private Sub ClearBatchTables()
        API_Batch_Orders.Clear()
    End Sub

    Private Sub GetBatchOrderInfo()
        If API_Connected = True Then
            Dim queryitemid As String = API_Log_Queue(0).itemtype
            NewEventMsg("Sending batch-Read request for ID: " & API_Log_Queue(0).itemtype)
            If API_Log_Queue(0).itemtype IsNot "" Then
                Dim TempResponse6 As String = API_Request("read", "itemid=" & queryitemid)
                'Dim TempResponse6Original As String = API_Request("read", "itemid=" & queryitemid) -- debug
                If TempResponse6 Is Nothing Then
                    NewEventMsg("Request failed!")
                Else
                    If TempResponse6.StartsWith("The remote server returned an error") Then
                        NewEventMsg(TempResponse6)
                    Else
                        ClearBatchTables()
                        If TempResponse6 = "false" Then
                            NewEventMsg("Server returned no data for item.")
                        Else
                            Dim first As Boolean = True
                            While TempResponse6.Length > 128
                                Dim orderid3 As String
                                If first = True Then
                                    orderid3 = TempResponse6.Remove(0, 2)
                                    orderid3 = orderid3.Remove(orderid3.IndexOf(""":"))
                                    first = False
                                Else
                                    orderid3 = TempResponse6.Remove(0, TempResponse6.IndexOf("},""") + 3)
                                    orderid3 = orderid3.Remove(orderid3.IndexOf(""""))
                                    TempResponse6 = TempResponse6.Remove(0, TempResponse6.IndexOf("},""") + 3)
                                End If
                                Dim marketid3 As String = TempResponse6.Remove(0, TempResponse6.IndexOf(""":""") + 3)
                                marketid3 = marketid3.Remove(marketid3.IndexOf(""","""))
                                TempResponse6 = TempResponse6.Remove(0, TempResponse6.IndexOf(""",""") + 3)
                                Dim itemid3 As String = TempResponse6.Remove(0, TempResponse6.IndexOf(""":""") + 3)
                                itemid3 = itemid3.Remove(itemid3.IndexOf(""","""))
                                TempResponse6 = TempResponse6.Remove(0, TempResponse6.IndexOf(""",""") + 3)
                                Dim quantity3 As String = TempResponse6.Remove(0, TempResponse6.IndexOf(""":""") + 3)
                                quantity3 = quantity3.Remove(quantity3.IndexOf(""","""))
                                TempResponse6 = TempResponse6.Remove(0, TempResponse6.IndexOf(""",""") + 3)
                                Dim ordertype3 As String = TempResponse6.Remove(0, TempResponse6.IndexOf(""":""") + 3)
                                ordertype3 = ordertype3.Remove(ordertype3.IndexOf(""","""))
                                TempResponse6 = TempResponse6.Remove(0, TempResponse6.IndexOf(""",""") + 3)
                                Dim expdate3 As String = TempResponse6.Remove(0, TempResponse6.IndexOf(""":""") + 3)
                                expdate3 = expdate3.Remove(expdate3.IndexOf(""","""))
                                TempResponse6 = TempResponse6.Remove(0, TempResponse6.IndexOf(""",""") + 3)
                                Dim lastupdated3 As String = TempResponse6.Remove(0, TempResponse6.IndexOf(""":""") + 3)
                                lastupdated3 = lastupdated3.Remove(lastupdated3.IndexOf(""","""))
                                TempResponse6 = TempResponse6.Remove(0, TempResponse6.IndexOf(""",""") + 3)
                                Dim price3 As String = TempResponse6.Remove(0, TempResponse6.IndexOf(""":""") + 3)
                                price3 = price3.Remove(price3.IndexOf(""","""))
                                'Price values that come in from logs or the API, need to be divided by 100. EG 327189 is actually 3271.89, and should be displayed that way
                                Dim price3String As String
                                If price3.Length > 2 Then
                                    Dim price3temp As String = price3.Remove(0, price3.Length - 2)
                                    price3String = price3.Remove(price3.Length - 2, 2) & "." & price3temp
                                Else
                                    price3String = "." & price3
                                End If
                                'order type doesnt matter here
                                Dim NewItem2 As LogInfoStructure
                                NewItem2.marketid = marketid3
                                NewItem2.orderid = orderid3
                                NewItem2.itemtype = itemid3
                                NewItem2.quantity = quantity3
                                NewItem2.expdate = expdate3
                                NewItem2.lastupdate = lastupdated3
                                NewItem2.price = price3
                                API_Batch_Orders.Add(NewItem2)
                                Application.DoEvents()
                            End While
                        End If
                    End If
                End If
            End If
        Else
            NewEventMsg("Cannot send Read Request - Not logged in.")
        End If
        CompareBatch()
    End Sub

    Private Sub CompareBatch()
        For batchindex As Integer = 1 To CurrentBatchList.Count()
            Dim OrderType As Integer
            If Convert.ToInt64(API_Log_Queue(CurrentBatchList(batchindex - 1)).quantity) > 0 Then
                OrderType = 1
            Else
                OrderType = 2
            End If
            Dim matched As Boolean = False
            Dim matchorderid As String = API_Log_Queue(CurrentBatchList(batchindex - 1)).orderid
            Dim foundmatchindex As Integer = 0
            For batchorderindex As Integer = 1 To API_Batch_Orders.Count()
                If matched = False Then
                    If API_Batch_Orders(batchorderindex - 1).orderid = matchorderid Then
                        matched = True
                        foundmatchindex = batchorderindex - 1
                    End If
                End If
            Next batchorderindex
            If matched = True Then
                If API_Batch_Orders(foundmatchindex).lastupdate = API_Log_Queue(CurrentBatchList(batchindex - 1)).lastupdate Then
                    'These orders have the same update date, we would need to compare it to the data in the queue to find out if they are different. But that would be very costly. Let's for now, assume the server already has the correct data and do nothing.
                Else
                    Dim DateComparison1 As String = API_Log_Queue(CurrentBatchList(batchindex - 1)).lastupdate.Remove(0, API_Log_Queue(CurrentBatchList(batchindex - 1)).lastupdate.IndexOf(" "))
                    Dim DateComparison2 As String = API_Batch_Orders(foundmatchindex).lastupdate.Remove(0, API_Batch_Orders(foundmatchindex).lastupdate.IndexOf(" "))
                    Dim d1 = DateTime.Parse(DateComparison1)
                    Dim d2 = DateTime.Parse(DateComparison2)
                    If d1 > d2 Then
                        'this order already exists, so we need to update it with new values
                        Dim NewItem3 As APIRequestQueueStructure
                        NewItem3.type = "update"
                        NewItem3.data = "orderid=" & API_Log_Queue(CurrentBatchList(batchindex - 1)).orderid & "&marketid=" & API_Log_Queue(CurrentBatchList(batchindex - 1)).marketid & "&itemid=" & API_Log_Queue(CurrentBatchList(batchindex - 1)).itemtype & "&quantity=" & API_Log_Queue(CurrentBatchList(batchindex - 1)).quantity & "&ordertype=" & OrderType & "&expiration=" & API_Log_Queue(CurrentBatchList(batchindex - 1)).expdate & "&lastupdated=" & API_Log_Queue(CurrentBatchList(batchindex - 1)).lastupdate & "&price=" & API_Log_Queue(CurrentBatchList(batchindex - 1)).price
                        API_Sanitized_Request_Queue.Add(NewItem3)
                    End If
                End If
            Else
                Dim NewItem3 As APIRequestQueueStructure
                NewItem3.type = "create"
                NewItem3.data = "orderid=" & API_Log_Queue(CurrentBatchList(batchindex - 1)).orderid & "&marketid=" & API_Log_Queue(CurrentBatchList(batchindex - 1)).marketid & "&itemid=" & API_Log_Queue(CurrentBatchList(batchindex - 1)).itemtype & "&quantity=" & API_Log_Queue(CurrentBatchList(batchindex - 1)).quantity & "&ordertype=" & OrderType & "&expiration=" & API_Log_Queue(CurrentBatchList(batchindex - 1)).expdate & "&lastupdated=" & API_Log_Queue(CurrentBatchList(batchindex - 1)).lastupdate & "&price=" & API_Log_Queue(CurrentBatchList(batchindex - 1)).price
                API_Sanitized_Request_Queue.Add(NewItem3)
            End If
            Application.DoEvents()
        Next batchindex
        CompareOrderData()
    End Sub

    Private Sub CompareOrderData()
        'Compare the order data from the server, with current data from logs, and make delete requests for the data that no longer exists
        For qindex1 As Integer = 1 To API_Batch_Orders.Count()
            Dim currentID1 As String = API_Batch_Orders(qindex1 - 1).orderid
            Dim matched2 As Boolean = False
            For listindex1 As Integer = 1 To CurrentBatchList.Count()
                If matched2 = False Then
                    If currentID1 = API_Log_Queue(CurrentBatchList(listindex1 - 1)).orderid Then
                        matched2 = True
                        Application.DoEvents()
                    End If
                End If
            Next listindex1
            If matched2 = False Then
                CurrentBatchDeleteList.Add(currentID1)
            End If
        Next qindex1
        If CurrentBatchDeleteList.Count() > 0 Then
            For qindex3 As Integer = 1 To CurrentBatchDeleteList.Count()
                Dim NewItem4 As APIRequestQueueStructure
                NewItem4.type = "delete"
                NewItem4.data = "orderid=" & CurrentBatchDeleteList(qindex3 - 1)
                API_Sanitized_Request_Queue.Add(NewItem4)
            Next qindex3
        End If
        CurrentBatchDeleteList.Clear()
        FlushBatch()
    End Sub

    Private Sub FlushBatch()
        For flush1 = 1 To CurrentBatchList.Count()
            API_Log_Queue.RemoveAt(CurrentBatchList((CurrentBatchList.Count() - 1) - (flush1 - 1)))
            Application.DoEvents()
        Next flush1
        ClearBatchTables()
        CurrentBatchList.Clear()
    End Sub

    Private Sub ProcessNextInSanitizedQueue()
        Try
            If API_Sanitized_Request_Queue.Count() > 0 Then
                Dim TempResponse8 As String = API_Request(API_Sanitized_Request_Queue(0).type, API_Sanitized_Request_Queue(0).data)
                API_Sanitized_Request_Queue.RemoveAt(0)
                StatLabelProc.Text = CInt(StatLabelProc.Text) + 1
            End If
        Catch ex As Exception
            NewEventMsg(ex.Message)
        End Try
    End Sub

    '############################## - Asynchronous Callback Listener - ##############################
    Dim t As Thread
    Public Shared runServer As Boolean = True
    Delegate Sub InvokeControl(ByVal [text] As String, ByVal [switch] As Boolean)
    Private Shared listener As HttpListener

    Private Sub Start()
        AsynchronousListener(New String() {"http://localhost:43296/"})
    End Sub

    ' Asynchronous HTTP listener. Instantiates and starts the HttpListener class,
    ' adds the prefix URI's to listen for, and enters the asynchronous processing loop
    ' until the runServer flag is set false.
    ' Param name prefixes is the URI prefix array to which the server responds
    Public Sub AsynchronousListener(ByVal prefixes() As String)
        ' spin up listener
        listener = New HttpListener()
        ' add URI prefixes to listen for
        Dim s As String
        For Each s In prefixes
            listener.Prefixes.Add(s)
        Next s
        listener.Start()
        ' Create the delegate using the method to update the UI
        Dim _invokeControl As New InvokeControl(AddressOf InvokeUIThread)

        While runServer
            Dim result As IAsyncResult = listener.BeginGetContext(New AsyncCallback(AddressOf AsynchronousListenerCallback), listener)
            ' intermediate work can go on here while waiting for the asynchronous callback
            ' an asynchronous wait handle is used to prevent this thread from terminating
            ' while waiting for the asynchronous operation to complete.
            result.AsyncWaitHandle.WaitOne()
        End While
        ' If the runServer flag gets set to false, stop the server and close the listener.
        ' for some reason, even after we stop the listener and set it to nothing, the registration still exists... find out how to kill this later.
        listener.Stop()
        listener.Abort()
        listener.Close()
        listener = Nothing
    End Sub 'AsynchronousListener

    '/ In order to safely update the UI across threads, a delegate with this method is
    '/ called using Control.Invoke
    Private Shared Sub InvokeUIThread(ByVal [text] As String, ByVal [switch] As Boolean)
        If switch = True Then
            Form1.ConsoleTextBox.AppendText(Environment.NewLine & [text])
            Form1.ConsoleTextBox.Select(Form1.ConsoleTextBox.TextLength, 0)
            Form1.ConsoleTextBox.ScrollToCaret()
        End If
    End Sub 'InvokeUIThread

    ' Method called back when a client connects. BeginGetContext contains the AsynchCallback delegate
    ' for this method.
    ' param name result is the state object containing the HttpListener instance
    Public Sub AsynchronousListenerCallback(ByVal result As IAsyncResult)
        Try
            Dim listener As HttpListener = CType(result.AsyncState, HttpListener)
            ' Call EndGetContext to signal the completion of the asynchronous operation.
            Dim context As HttpListenerContext = listener.EndGetContext(result)
            Dim request As HttpListenerRequest = context.Request

            If API_Discord_Allow_New_Auth = True Then
                API_Discord_Auth_Code = request.RawUrl.Split("&")(0).Remove(0, 7)
                API_Discord_Auth_State = request.RawUrl.Split("&")(1).Remove(0, 6)
                API_Discord_Allow_New_Auth = False
            End If
            ' Get the response object to send our confirmation.
            Dim response As HttpListenerResponse = context.Response
            ' Construct a minimal response string.
            Dim responseString As String = "<HTML onload=""self.close()"" onfocus=""self.close()"" onclick=""self.close()""><BODY onload=""self.close()"" onfocus=""self.close()"" onclick=""self.close()""><script>setTimeout(function() {window.close();}, 50);</script><img style=""position:absolute;top:0;left:0;width:100%;height:100%"" src=""https://duopenmarket.com/assets/images/bg2.png""/><div style=""position:absolute;top:15%;left:27%;width:50%;height:35%;color:#FFFFFF;font-family:'Courier New';font-size:24px""><center><H1>DUOpenMarket - Authentication Successful</H1><br><br><br><br><H1>This window should close automatically.<br>You may close it if it does not.</H1></center></div></BODY></HTML>"
            Dim buffer As Byte() = System.Text.Encoding.UTF8.GetBytes(responseString)
            ' Get the response OutputStream and write the response to it.
            response.ContentLength64 = buffer.Length
            ' Identify the content type.
            response.ContentType = "text/html"
            Dim output As System.IO.Stream = response.OutputStream
            output.Write(buffer, 0, buffer.Length)
            ' Properly flush and close the output stream
            output.Flush()
            output.Close()
            StopListener()
        Catch ex As Exception
            Dim _invokeControl As New InvokeControl(AddressOf InvokeUIThread)
            ConsoleTextBox.Invoke(_invokeControl, ex.Message, True)
            StopListener()
        End Try
    End Sub

    Private Sub CreateListener()
        runServer = True
        t = New Thread(New ThreadStart(AddressOf Start))
        t.Start()
        LoginTimer.Start()
        API_Discord_Allow_New_Auth = True
    End Sub

    Private Sub StopListener()
        runServer = False
        t.Abort()
        LoginTimer.Stop()
    End Sub

    '############################## - Theme Skinning - ##############################
    Public Sub SetDarkTheme()
        BackgroundColor1 = Color.FromArgb(255, 30, 36, 42)
        BackgroundColor2 = Color.FromArgb(255, 50, 56, 62)
        BackgroundColor3 = Color.FromArgb(255, 26, 28, 32)

        ForegroundColor1 = Color.FromArgb(255, 224, 224, 224)
        ForegroundColor2 = Color.FromArgb(255, 165, 165, 165)
        ForegroundColor3 = Color.FromArgb(255, 125, 125, 125)

        GridColor1 = Color.FromArgb(255, 125, 125, 125)
        GridBGColor1 = Color.FromArgb(255, 26, 28, 32)
        GridBGColor2 = Color.FromArgb(255, 26, 38, 42)
        GridSelectColor1 = Color.FromArgb(255, 28, 74, 92)

        HistGridColor = Color.FromArgb(255, 255, 255, 255)
        HistBuyColor1 = Color.FromArgb(128, 0, 32, 85)
        HistBuyColor2 = Color.FromArgb(128, 0, 128, 188)
        HistBuyColor3 = Color.FromArgb(255, 0, 32, 128)
        HistSellColor1 = Color.FromArgb(128, 148, 148, 148)
        HistSellColor2 = Color.FromArgb(128, 215, 215, 215)
        HistSellColor3 = Color.FromArgb(255, 255, 255, 255)

        LoginPanel.BackgroundImage = My.Resources.loginbg
        MarketPanel.BackgroundImage = My.Resources.loginbg
        ResourcePanel.BackgroundImage = My.Resources.loginbg
        ResizeGrabber.BackgroundImage = My.Resources.grabber
        ResizeGrabber2.BackgroundImage = My.Resources.grabber

        SetApplicationTheme()
    End Sub

    Public Sub SetLightTheme()
        BackgroundColor1 = Color.FromArgb(255, 224, 224, 224)
        BackgroundColor2 = Color.FromArgb(64, 20, 46, 62)
        BackgroundColor3 = Color.FromArgb(255, 185, 185, 185)

        ForegroundColor1 = Color.FromArgb(255, 30, 36, 42)
        ForegroundColor2 = Color.FromArgb(255, 50, 56, 62)
        ForegroundColor3 = Color.FromArgb(255, 80, 86, 92)

        GridColor1 = Color.FromArgb(255, 30, 36, 42)
        GridBGColor1 = Color.FromArgb(255, 185, 185, 185)
        GridBGColor2 = Color.FromArgb(255, 205, 205, 205)
        GridSelectColor1 = Color.FromArgb(255, 235, 225, 185)

        HistGridColor = Color.FromArgb(255, 0, 0, 0)
        HistBuyColor1 = Color.FromArgb(128, 0, 8, 42)
        HistBuyColor2 = Color.FromArgb(128, 0, 32, 85)
        HistBuyColor3 = Color.FromArgb(128, 0, 32, 85)
        HistSellColor1 = Color.FromArgb(128, 32, 74, 128)
        HistSellColor2 = Color.FromArgb(128, 64, 148, 255)
        HistSellColor3 = Color.FromArgb(255, 64, 148, 255)

        LoginPanel.BackgroundImage = My.Resources.loginbgneg
        MarketPanel.BackgroundImage = My.Resources.loginbgneg
        ResourcePanel.BackgroundImage = My.Resources.loginbgneg
        ResizeGrabber.BackgroundImage = My.Resources.grabberneg
        ResizeGrabber2.BackgroundImage = My.Resources.grabberneg

        SetApplicationTheme()
    End Sub

    Public Sub SetCustomTheme()
        BackgroundColor1 = CustomBackgroundColor1
        BackgroundColor2 = BackgroundColor2
        BackgroundColor3 = BackgroundColor3

        ForegroundColor1 = ForegroundColor1
        ForegroundColor2 = ForegroundColor2
        ForegroundColor3 = ForegroundColor3

        GridColor1 = GridColor1
        GridBGColor1 = GridBGColor1
        GridBGColor2 = GridBGColor2
        GridSelectColor1 = GridSelectColor1

        HistGridColor = HistGridColor
        HistBuyColor1 = HistBuyColor1
        HistBuyColor2 = HistBuyColor2
        HistBuyColor3 = HistBuyColor3
        HistSellColor1 = HistSellColor1
        HistSellColor2 = HistSellColor2
        HistSellColor3 = HistSellColor3

        LoginPanel.BackgroundImage = Nothing
        MarketPanel.BackgroundImage = Nothing
        ResourcePanel.BackgroundImage = Nothing
        ResizeGrabber.BackgroundImage = My.Resources.grabberneg
        ResizeGrabber2.BackgroundImage = My.Resources.grabberneg

        SetApplicationTheme()
    End Sub

    Public Sub SetApplicationTheme()
        Me.BackColor = BackgroundColor1
        LoginPanel.BackColor = BackgroundColor1
        MarketPanel.BackColor = BackgroundColor1
        ResourcePanel.BackColor = BackgroundColor1
        MainPanel.BackColor = BackgroundColor1

        Label22.ForeColor = ForegroundColor2
        AboutForm.UpdateThemeState()
        TitlebarSettingsButton.ForeColor = ForegroundColor3
        TitlebarSettingsButton.FlatAppearance.MouseOverBackColor = BackgroundColor2
        TitlebarAboutButton.ForeColor = ForegroundColor3
        TitlebarAboutButton.FlatAppearance.MouseOverBackColor = BackgroundColor2
        TitlebarMinButton.ForeColor = ForegroundColor3
        TitlebarMinButton.FlatAppearance.MouseOverBackColor = BackgroundColor2
        TitlebarMaxButton.ForeColor = ForegroundColor3
        TitlebarMaxButton.FlatAppearance.MouseOverBackColor = BackgroundColor2
        TitlebarCloseButton.ForeColor = ForegroundColor3
        TitlebarCloseButton.FlatAppearance.MouseOverBackColor = BackgroundColor2

        LoginLabel1.ForeColor = ForegroundColor1
        LoginLabel2.ForeColor = ForegroundColor1
        LoginLabel3.ForeColor = ForegroundColor1
        DiscordLoginButton2.ForeColor = ForegroundColor1
        DiscordLoginButton2.FlatAppearance.MouseOverBackColor = BackgroundColor2

        UpdateButton1.ForeColor = ForegroundColor1
        UpdateButton1.FlatAppearance.MouseOverBackColor = BackgroundColor2
        UpdateButton2.ForeColor = ForegroundColor1
        UpdateButton2.FlatAppearance.MouseOverBackColor = BackgroundColor2

        ItemSearchTextBox.BackColor = BackgroundColor3
        ItemSearchTextBox.ForeColor = ForegroundColor1
        ItemTree.BackColor = BackgroundColor3
        ItemTree.ForeColor = ForegroundColor1
        ItemTreeSearch.BackColor = BackgroundColor3
        ItemTreeSearch.ForeColor = ForegroundColor1

        AdvFilteringToggleButton.BackColor = BackgroundColor3
        AdvFilteringToggleButton.ForeColor = ForegroundColor1
        GroupBox2.ForeColor = ForegroundColor1
        GroupBox3.ForeColor = ForegroundColor1
        Label15.ForeColor = ForegroundColor1
        Label16.ForeColor = ForegroundColor1
        FilterPriceMinBox.ForeColor = ForegroundColor1
        FilterPriceMinBox.BackColor = BackgroundColor3
        FilterPriceMaxBox.ForeColor = ForegroundColor1
        FilterPriceMaxBox.BackColor = BackgroundColor3
        FilterQuantityMinBox.ForeColor = ForegroundColor1
        FilterQuantityMinBox.BackColor = BackgroundColor3
        FilterQuantityMaxBox.ForeColor = ForegroundColor1
        FilterQuantityMaxBox.BackColor = BackgroundColor3
        Label17.ForeColor = ForegroundColor1
        Label18.ForeColor = ForegroundColor1
        AdvMarketTreeView.ForeColor = ForegroundColor1
        AdvMarketTreeView.BackColor = BackgroundColor3
        AdvItemTreeView.ForeColor = ForegroundColor1
        AdvItemTreeView.BackColor = BackgroundColor3
        FilterResetButton.ForeColor = ForegroundColor1
        FilterResetButton.BackColor = BackgroundColor3
        FilterOrdersButton.ForeColor = ForegroundColor1
        FilterOrdersButton.BackColor = BackgroundColor3

        BookmarkButton.BackColor = BackgroundColor3
        BookmarkButton.ForeColor = ForegroundColor1
        AddBookmarkButton.BackColor = BackgroundColor3
        AddBookmarkButton.ForeColor = ForegroundColor1
        BookmarkPanel.BackColor = BackgroundColor3
        BookmarkPanel.ForeColor = ForegroundColor1
        BookmarkLabel.ForeColor = ForegroundColor1
        ItemTreeBookmarks.BackColor = BackgroundColor3
        ItemTreeBookmarks.ForeColor = ForegroundColor1

        StatsPanel.BackColor = BackgroundColor3
        Label2.ForeColor = ForegroundColor1
        Label6.ForeColor = ForegroundColor1
        Label3.ForeColor = ForegroundColor1
        Label4.ForeColor = ForegroundColor1
        LogBufferLabel.ForeColor = ForegroundColor1
        StatLabelQue.ForeColor = ForegroundColor1
        StatLabelProc.ForeColor = ForegroundColor1
        GroupBox1.ForeColor = ForegroundColor1
        Label7.ForeColor = ForegroundColor1
        Label8.ForeColor = ForegroundColor1
        Label9.ForeColor = ForegroundColor1
        Label10.ForeColor = ForegroundColor1
        APIReadsLabel.ForeColor = ForegroundColor1
        APIUpdatesLabel.ForeColor = ForegroundColor1
        APICreatesLabel.ForeColor = ForegroundColor1
        APIDeletesLabel.ForeColor = ForegroundColor1


        Button7.ForeColor = ForegroundColor1
        Button7.FlatAppearance.MouseOverBackColor = BackgroundColor2
        Button7.BackColor = BackgroundColor3
        Button1.ForeColor = ForegroundColor1
        Button1.FlatAppearance.MouseOverBackColor = BackgroundColor2
        Button1.BackColor = BackgroundColor3
        ComboBox1.BackColor = BackgroundColor3
        ComboBox1.ForeColor = ForegroundColor1
        Label28.ForeColor = ForegroundColor1

        ConsoleTextBox.BackColor = BackgroundColor3
        ConsoleTextBox.ForeColor = ForegroundColor3
        ConsoleInputBox.BackColor = BackgroundColor3
        ConsoleInputBox.ForeColor = ForegroundColor2
        ConsoleSubmitButton.ForeColor = ForegroundColor2

        MarketOrdersButton.BackColor = BackgroundColor3
        MarketOrdersButton.ForeColor = ForegroundColor1
        ResourceManagerButton.BackColor = BackgroundColor3
        ResourceManagerButton.ForeColor = ForegroundColor1
        Button3.BackColor = BackgroundColor3
        Button3.ForeColor = ForegroundColor3

        SelectedItemLabel.ForeColor = ForegroundColor1
        Label1.ForeColor = ForegroundColor1
        Label5.ForeColor = ForegroundColor1


        BuyOrderGridViewRaw.BackgroundColor = BackgroundColor3
        BuyOrderGridViewRaw.GridColor = GridColor1
        SellOrderGridViewRaw.BackgroundColor = BackgroundColor3
        SellOrderGridViewRaw.GridColor = GridColor1

        BuyOrderGridViewRaw.RowsDefaultCellStyle.ForeColor = ForegroundColor1
        BuyOrderGridViewRaw.AlternatingRowsDefaultCellStyle.ForeColor = ForegroundColor1
        BuyOrderGridViewRaw.RowsDefaultCellStyle.BackColor = GridBGColor1
        BuyOrderGridViewRaw.AlternatingRowsDefaultCellStyle.BackColor = GridBGColor2
        BuyOrderGridViewRaw.RowsDefaultCellStyle.SelectionForeColor = ForegroundColor1
        BuyOrderGridViewRaw.RowsDefaultCellStyle.SelectionBackColor = GridSelectColor1

        SellOrderGridViewRaw.RowsDefaultCellStyle.ForeColor = ForegroundColor1
        SellOrderGridViewRaw.AlternatingRowsDefaultCellStyle.ForeColor = ForegroundColor1
        SellOrderGridViewRaw.RowsDefaultCellStyle.BackColor = GridBGColor1
        SellOrderGridViewRaw.AlternatingRowsDefaultCellStyle.BackColor = GridBGColor2
        SellOrderGridViewRaw.RowsDefaultCellStyle.SelectionForeColor = ForegroundColor1
        SellOrderGridViewRaw.RowsDefaultCellStyle.SelectionBackColor = GridSelectColor1

        HistogramChart.BackColor = BackgroundColor1
        HistogramChart.ChartAreas("ChartArea1").BackColor = BackgroundColor3
        HistogramChart.ChartAreas("ChartArea1").AxisX.TitleForeColor = HistGridColor
        HistogramChart.ChartAreas("ChartArea1").AxisX.LabelStyle.ForeColor = HistGridColor
        HistogramChart.ChartAreas("ChartArea1").AxisX.LineColor = HistGridColor
        HistogramChart.ChartAreas("ChartArea1").AxisX.MajorGrid.LineColor = HistGridColor
        HistogramChart.ChartAreas("ChartArea1").AxisX.MajorTickMark.LineColor = HistGridColor
        HistogramChart.ChartAreas("ChartArea1").AxisY.TitleForeColor = HistGridColor
        HistogramChart.ChartAreas("ChartArea1").AxisY.LabelStyle.ForeColor = HistGridColor
        HistogramChart.ChartAreas("ChartArea1").AxisY.LineColor = HistGridColor
        HistogramChart.ChartAreas("ChartArea1").AxisY.MajorGrid.LineColor = HistGridColor
        HistogramChart.ChartAreas("ChartArea1").AxisY.MajorTickMark.LineColor = HistGridColor

        HistogramChart.Series("Sell Orders").Color = HistSellColor1
        HistogramChart.Series("Sell Orders").BackSecondaryColor = HistSellColor2
        HistogramChart.Series("Sell Orders").BorderColor = HistSellColor3

        HistogramChart.Series("Buy Orders").Color = HistBuyColor1
        HistogramChart.Series("Buy Orders").BackSecondaryColor = HistBuyColor2
        HistogramChart.Series("Buy Orders").BorderColor = HistBuyColor3
    End Sub

    '############################## - System Tray Icon - ##############################
    Private Sub NotifyIcon1_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles NotifyIcon1.MouseDoubleClick
        Me.Show()
        NotifyIcon1.Visible = False
    End Sub

    Private Sub ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem1.Click
        SavePrefsToIni()
        Application.Exit()
        Me.Dispose()
    End Sub







    '############################## - DEVELOPMENT ONLY - ##############################

    Private Sub ResourceManagerButton_Click(sender As Object, e As EventArgs) Handles ResourceManagerButton.Click
        MarketPanel.Visible = False
        ResourcePanel.Visible = True
        ResourcePanel.BringToFront()
    End Sub

    Private Sub MarketOrdersButton_Click(sender As Object, e As EventArgs) Handles MarketOrdersButton.Click
        MarketPanel.Visible = True
        ResourcePanel.Visible = False
        MarketPanel.BringToFront()
    End Sub
End Class