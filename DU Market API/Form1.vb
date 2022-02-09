Option Explicit On
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Environment
Imports System.Net
Imports System.Net.Sockets
Imports System.Threading
Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Windows.Forms

Public Class Form1

    '########## GUI/MISC VARS ##########
    Dim showhlp = False
    Dim Go As Boolean
    Dim LeftSet As Boolean
    Dim TopSet As Boolean
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
    Dim ShowDevPanel As Boolean = False

    Dim API_Client_Version As String = "0.42.1"
    Dim API_Connected As Boolean = False
    Dim API_Username As String = ""
    Dim API_Password As String = ""
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
    Dim ShowRawData As Boolean = False

    Dim SearchUserTyping As Boolean = False
    Dim ShowFilters As Boolean = False
    Dim FilterPriceMin As Long
    Dim FilterPriceMax As Long
    Dim FilterQuantityMin As Long
    Dim FilterQuantityMax As Long
    Dim FilterMarketList As New List(Of FilterMarketListStructure)
    Dim FilterItemList As New List(Of FilterItemListStructure)

    Dim currentdate As Date
    Dim CurrYear As String
    Dim CurrMonth As String
    Dim CurrDay As String
    Dim CurrHour As String
    Dim CurrMin As String
    Dim CurrSec As String

    Dim TempResponse As String

    Public Setting_SaveWindowLoc As String
    Public Setting_SaveGridLayout As String

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

    '############################## DLL Imports ##############################
    <DllImport("kernel32")>
    Private Shared Function GetPrivateProfileString(ByVal section As String, ByVal key As String, ByVal def As String, ByVal retVal As StringBuilder, ByVal size As Integer, ByVal filePath As String) As Integer
    End Function

    <DllImport("kernel32")>
    Private Shared Function WritePrivateProfileString(ByVal lpSectionName As String, ByVal lpKeyName As String, ByVal lpString As String, ByVal lpFileName As String) As Long
    End Function


    '############################## Configuration File Functions ##############################
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

    Private Sub LoadPrefsFromIni()
        Dim savedWindowState As String
        Dim savedWindowLocX As String
        Dim savedWindowLocY As String
        Dim savedWindowSizeW As String
        Dim savedWindowSizeH As String
        Dim savedAbtWindowLocX As String
        Dim savedAbtWindowLocY As String
        Dim savedSetWindowLocX As String
        Dim savedSetWindowLocY As String
        Setting_SaveWindowLoc = GetIniValue("Application", "SaveWindowLoc", My.Application.Info.DirectoryPath & "\DUOMsettings.ini")
        Setting_SaveGridLayout = GetIniValue("Application", "SaveGridLayout", My.Application.Info.DirectoryPath & "\DUOMsettings.ini")
        If Setting_SaveWindowLoc = "True" Then
            savedWindowState = GetIniValue("Application", "WindowState", My.Application.Info.DirectoryPath & "\DUOMsettings.ini")
            savedWindowLocX = GetIniValue("Application", "WindowLocX", My.Application.Info.DirectoryPath & "\DUOMsettings.ini")
            savedWindowLocY = GetIniValue("Application", "WindowLocY", My.Application.Info.DirectoryPath & "\DUOMsettings.ini")
            savedWindowSizeW = GetIniValue("Application", "WindowSizeW", My.Application.Info.DirectoryPath & "\DUOMsettings.ini")
            savedWindowSizeH = GetIniValue("Application", "WindowSizeH", My.Application.Info.DirectoryPath & "\DUOMsettings.ini")
            savedAbtWindowLocX = GetIniValue("Application", "AbtWindowLocX", My.Application.Info.DirectoryPath & "\DUOMsettings.ini")
            savedAbtWindowLocY = GetIniValue("Application", "AbtWindowLocY", My.Application.Info.DirectoryPath & "\DUOMsettings.ini")
            savedSetWindowLocX = GetIniValue("Application", "SetWindowLocX", My.Application.Info.DirectoryPath & "\DUOMsettings.ini")
            savedSetWindowLocY = GetIniValue("Application", "SetWindowLocY", My.Application.Info.DirectoryPath & "\DUOMsettings.ini")
            If savedWindowState IsNot "" And savedWindowState IsNot Nothing Then
                WindowMaximizedState = CBool(savedWindowState)
            End If
            If savedWindowSizeW IsNot "" And savedWindowSizeW IsNot Nothing Then
                If savedWindowSizeH IsNot "" And savedWindowSizeH IsNot Nothing Then
                    Dim newbnds As Size = New Size()
                    newbnds.Width = CInt(savedWindowSizeW)
                    newbnds.Height = CInt(savedWindowSizeH)
                    MainPanel.Parent.Size = newbnds
                End If
            End If
            If savedWindowLocX IsNot "" And savedWindowLocX IsNot Nothing Then
                If savedWindowLocY IsNot "" And savedWindowLocY IsNot Nothing Then
                    Dim newpoint As New Point
                    newpoint.X = CInt(savedWindowLocX)
                    newpoint.Y = CInt(savedWindowLocY)
                    MainPanel.Parent.Location = newpoint
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
    End Sub

    Private Sub SavePrefsToIni()
        SetIniValue("Application", "SaveWindowLoc", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(Setting_SaveWindowLoc))
        SetIniValue("Application", "SaveGridLayout", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(Setting_SaveGridLayout))
        If Setting_SaveWindowLoc = "True" Then
            SetIniValue("Application", "WindowState", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(WindowMaximizedState))
            SetIniValue("Application", "WindowLocX", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(Me.Location.X))
            SetIniValue("Application", "WindowLocY", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(Me.Location.Y))
            SetIniValue("Application", "WindowSizeW", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(MainPanel.Parent.Size.Width))
            SetIniValue("Application", "WindowSizeH", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(MainPanel.Parent.Size.Height))
            SetIniValue("Application", "AbtWindowLocX", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(AboutForm.Location.X))
            SetIniValue("Application", "AbtWindowLocY", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(AboutForm.Location.Y))
            SetIniValue("Application", "SetWindowLocX", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(SettingsForm.Location.X))
            SetIniValue("Application", "SetWindowLocY", My.Application.Info.DirectoryPath & "\DUOMsettings.ini", CStr(SettingsForm.Location.Y))
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
    End Sub


    '############################## Form Load ##############################
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CheckForUpdates()
        TimeStampInt()
        LoadPrefsFromIni()
        CenterLoginElements()
        API_LogfileDirectory = GetFolderPath(SpecialFolder.LocalApplicationData) & "\NQ\DualUniverse\log"
        FileSystemWatcher1.IncludeSubdirectories = False
        FileSystemWatcher1.EnableRaisingEvents = False
        InitDataTables()
        InitItemList()
        SetupGridViewStyling()
        InitMarketTable()
        InitAdvMarketTree()
        NewEventMsg("Initialized.")
    End Sub

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

    Private Sub SetupGridViewStyling()
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

    Private Sub InitMarketTable()
        API_Market_Table.Columns.Add("id", GetType(Integer))
        API_Market_Table.Columns.Add("name", GetType(String))
        API_Market_Table.Rows.Add(1662, "Market Talemai 6")
        API_Market_Table.Rows.Add(1617, "Market Alioth District 01")
        API_Market_Table.Rows.Add(1752, "Market Ion Moon I 2")
        API_Market_Table.Rows.Add(1623, "Market Alioth District 06")
        API_Market_Table.Rows.Add(1755, "Market Sanctuary 01")
        API_Market_Table.Rows.Add(1753, "Market Ion Moon II 1")
        API_Market_Table.Rows.Add(1893, "Market Sanctuary 06")
        API_Market_Table.Rows.Add(35747, "Small Market")
        API_Market_Table.Rows.Add(1618, "Market Alioth District 10")
        API_Market_Table.Rows.Add(1754, "Market Ion Moon II 2")
        API_Market_Table.Rows.Add(1619, "Market Alioth District 02")
        API_Market_Table.Rows.Add(1620, "Market Alioth District 03")
        API_Market_Table.Rows.Add(1621, "Market Alioth District 04")
        API_Market_Table.Rows.Add(1756, "Market Sanctuary 10")
        API_Market_Table.Rows.Add(1889, "Market Sanctuary 02")
        API_Market_Table.Rows.Add(1890, "Market Sanctuary 03")
        API_Market_Table.Rows.Add(1891, "Market Sanctuary 04")
        API_Market_Table.Rows.Add(1892, "Market Sanctuary 05")
        API_Market_Table.Rows.Add(1894, "Market Sanctuary 07")
        API_Market_Table.Rows.Add(1895, "Market Sanctuary 08")
        API_Market_Table.Rows.Add(1622, "Market Alioth District 05")
        API_Market_Table.Rows.Add(1624, "Market Alioth District 07")
        API_Market_Table.Rows.Add(1896, "Market Sanctuary 09")
        API_Market_Table.Rows.Add(1633, "Market Alioth District 08")
        API_Market_Table.Rows.Add(1634, "Market Alioth District 09")
        API_Market_Table.Rows.Add(1635, "Market Madis 1")
        API_Market_Table.Rows.Add(1636, "Market Madis 2")
        API_Market_Table.Rows.Add(1745, "Market Ion 1")
        API_Market_Table.Rows.Add(1746, "Market Ion 2")
        API_Market_Table.Rows.Add(1747, "Market Ion 3")
        API_Market_Table.Rows.Add(1748, "Market Ion 4")
        API_Market_Table.Rows.Add(1749, "Market Ion 5")
        API_Market_Table.Rows.Add(1750, "Market Ion 6")
        API_Market_Table.Rows.Add(1751, "Market Ion Moon I 1")
        API_Market_Table.Rows.Add(1639, "Market Madis 5")
        API_Market_Table.Rows.Add(1637, "Market Madis 3")
        API_Market_Table.Rows.Add(1640, "Market Madis 6")
        API_Market_Table.Rows.Add(1641, "Market Alioth 20")
        API_Market_Table.Rows.Add(1642, "Market Alioth 11")
        API_Market_Table.Rows.Add(1643, "Market Alioth 12")
        API_Market_Table.Rows.Add(1644, "Market Alioth 13")
        API_Market_Table.Rows.Add(1645, "Market Alioth 14 ")
        API_Market_Table.Rows.Add(17559, "Market Alioth 15")
        API_Market_Table.Rows.Add(1647, "Market Alioth 16")
        API_Market_Table.Rows.Add(1648, "Market Alioth 17")
        API_Market_Table.Rows.Add(1649, "Market Alioth 18")
        API_Market_Table.Rows.Add(1650, "Market Alioth 19")
        API_Market_Table.Rows.Add(1651, "Market Thades 1")
        API_Market_Table.Rows.Add(1652, "Market Thades 2")
        API_Market_Table.Rows.Add(1653, "Market Thades 3")
        API_Market_Table.Rows.Add(1654, "Market Thades 4")
        API_Market_Table.Rows.Add(1655, "Market Thades 5")
        API_Market_Table.Rows.Add(1656, "Market Thades 6")
        API_Market_Table.Rows.Add(1657, "Market Talemai 1")
        API_Market_Table.Rows.Add(1658, "Market Talemai 2")
        API_Market_Table.Rows.Add(1659, "Market Talemai 3")
        API_Market_Table.Rows.Add(1660, "Market Talemai 4")
        API_Market_Table.Rows.Add(1661, "Market Talemai 5")
        API_Market_Table.Rows.Add(1663, "Market Feli 1")
        API_Market_Table.Rows.Add(1664, "Market Feli 2")
        API_Market_Table.Rows.Add(1665, "Market Feli 3")
        API_Market_Table.Rows.Add(1666, "Market Feli 4")
        API_Market_Table.Rows.Add(1667, "Market Feli 5")
        API_Market_Table.Rows.Add(1668, "Market Feli 6")
        API_Market_Table.Rows.Add(1669, "Market Sicari 1")
        API_Market_Table.Rows.Add(1670, "Market Sicari 2")
        API_Market_Table.Rows.Add(1671, "Market Sicari 3")
        API_Market_Table.Rows.Add(1672, "Market Sicari 4")
        API_Market_Table.Rows.Add(1673, "Market Sicari 5")
        API_Market_Table.Rows.Add(1674, "Market Sicari 6")
        API_Market_Table.Rows.Add(1675, "Market Sinnen 1")
        API_Market_Table.Rows.Add(1676, "Market Sinnen 2")
        API_Market_Table.Rows.Add(1677, "Market Sinnen 3")
        API_Market_Table.Rows.Add(1678, "Market Sinnen 4")
        API_Market_Table.Rows.Add(1679, "Market Sinnen 5")
        API_Market_Table.Rows.Add(1680, "Market Sinnen 6")
        API_Market_Table.Rows.Add(1681, "Market Teoma 1")
        API_Market_Table.Rows.Add(1682, "Market Teoma 2")
        API_Market_Table.Rows.Add(1683, "Market Teoma 3")
        API_Market_Table.Rows.Add(1684, "Market Teoma 4")
        API_Market_Table.Rows.Add(1685, "Market Teoma 5")
        API_Market_Table.Rows.Add(1686, "Market Teoma 6")
        API_Market_Table.Rows.Add(1687, "Market Jago 1")
        API_Market_Table.Rows.Add(1688, "Market Jago 2")
        API_Market_Table.Rows.Add(1689, "Market Jago 3")
        API_Market_Table.Rows.Add(1690, "Market Jago 4")
        API_Market_Table.Rows.Add(1691, "Market Jago 5")
        API_Market_Table.Rows.Add(1692, "Market Jago 6")
        API_Market_Table.Rows.Add(1693, "Market Madis Moon I 1")
        API_Market_Table.Rows.Add(1694, "Market Madis Moon I 2")
        API_Market_Table.Rows.Add(1695, "Market Madis Moon II 1")
        API_Market_Table.Rows.Add(1696, "Market Madis Moon II 2")
        API_Market_Table.Rows.Add(1697, "Market Madis Moon III 1")
        API_Market_Table.Rows.Add(1638, "Market Madis 4")
        API_Market_Table.Rows.Add(1698, "Market Madis Moon III 2")
        API_Market_Table.Rows.Add(1705, "Market Sanctuary 12")
        API_Market_Table.Rows.Add(1706, "Market Sanctuary 13")
        API_Market_Table.Rows.Add(1699, "Market Alioth Moon I 1")
        API_Market_Table.Rows.Add(1707, "Market Sanctuary 14")
        API_Market_Table.Rows.Add(1700, "Market Alioth Moon I 2")
        API_Market_Table.Rows.Add(1712, "Market Sanctuary 19")
        API_Market_Table.Rows.Add(1713, "Market Thades Moon I 1")
        API_Market_Table.Rows.Add(1714, "Market Thades Moon I 2")
        API_Market_Table.Rows.Add(1715, "Market Thades Moon II 1")
        API_Market_Table.Rows.Add(1716, "Market Thades Moon II 2")
        API_Market_Table.Rows.Add(1717, "Market Talemai Moon II 1")
        API_Market_Table.Rows.Add(1719, "Market Talemai Moon III 1")
        API_Market_Table.Rows.Add(1720, "Market Talemai Moon III 2")
        API_Market_Table.Rows.Add(1721, "Market Talemai Moon I 1")
        API_Market_Table.Rows.Add(1722, "Market Talemai Moon I 2")
        API_Market_Table.Rows.Add(1723, "Market Feli Moon I 1")
        API_Market_Table.Rows.Add(1724, "Market Feli Moon I 2")
        API_Market_Table.Rows.Add(1725, "Market Sinnen Moon I 1")
        API_Market_Table.Rows.Add(1726, "Market Sinnen Moon I 2")
        API_Market_Table.Rows.Add(1727, "Market Lacobus 1")
        API_Market_Table.Rows.Add(1728, "Market Lacobus 2")
        API_Market_Table.Rows.Add(1729, "Market Lacobus 3")
        API_Market_Table.Rows.Add(1730, "Market Lacobus 4")
        API_Market_Table.Rows.Add(1731, "Market Lacobus 5")
        API_Market_Table.Rows.Add(1732, "Market Lacobus 6")
        API_Market_Table.Rows.Add(1733, "Market Lacobus Moon III 1")
        API_Market_Table.Rows.Add(1734, "Market Lacobus Moon III 2")
        API_Market_Table.Rows.Add(1735, "Market Lacobus Moon I 1")
        API_Market_Table.Rows.Add(1736, "Market Lacobus Moon I 2")
        API_Market_Table.Rows.Add(1737, "Market Lacobus Moon II 1")
        API_Market_Table.Rows.Add(1738, "Market Lacobus Moon II 2")
        API_Market_Table.Rows.Add(1739, "Market Symeon 1")
        API_Market_Table.Rows.Add(1740, "Market Symeon 2")
        API_Market_Table.Rows.Add(1741, "Market Symeon 3")
        API_Market_Table.Rows.Add(1742, "Market Symeon 4")
        API_Market_Table.Rows.Add(1743, "Market Symeon 5")
        API_Market_Table.Rows.Add(1744, "Market Symeon 6")
        API_Market_Table.Rows.Add(1701, "Market Alioth Moon IV 1")
        API_Market_Table.Rows.Add(1702, "Market Alioth Moon IV 2")
        API_Market_Table.Rows.Add(1703, "Market Sanctuary 20")
        API_Market_Table.Rows.Add(1704, "Market Sanctuary 11")
        API_Market_Table.Rows.Add(1708, "Market Sanctuary 15")
        API_Market_Table.Rows.Add(1709, "Market Sanctuary 16")
        API_Market_Table.Rows.Add(1710, "Market Sanctuary 17")
        API_Market_Table.Rows.Add(1711, "Market Sanctuary 18")
        API_Market_Table.Rows.Add(1718, "Market Talemai Moon II 2")
    End Sub

    Private Sub InitAdvMarketTree()
        AdvMarketTreeView.Nodes.Add("1623", "Market Alioth District 06")
        AdvMarketTreeView.Nodes.Add("1617", "Market Alioth District 01")
        AdvMarketTreeView.Nodes.Add("1619", "Market Alioth District 02")
        AdvMarketTreeView.Nodes.Add("1620", "Market Alioth District 03")
        AdvMarketTreeView.Nodes.Add("1621", "Market Alioth District 04")
        AdvMarketTreeView.Nodes.Add("1622", "Market Alioth District 05")
        AdvMarketTreeView.Nodes.Add("1624", "Market Alioth District 07")
        AdvMarketTreeView.Nodes.Add("1633", "Market Alioth District 08")
        AdvMarketTreeView.Nodes.Add("1634", "Market Alioth District 09")
        AdvMarketTreeView.Nodes.Add("1618", "Market Alioth District 10")
        AdvMarketTreeView.Nodes.Add("1642", "Market Alioth 11")
        AdvMarketTreeView.Nodes.Add("1643", "Market Alioth 12")
        AdvMarketTreeView.Nodes.Add("1644", "Market Alioth 13")
        AdvMarketTreeView.Nodes.Add("1645", "Market Alioth 14")
        AdvMarketTreeView.Nodes.Add("17559", "Market Alioth 15")
        AdvMarketTreeView.Nodes.Add("1647", "Market Alioth 16")
        AdvMarketTreeView.Nodes.Add("1648", "Market Alioth 17")
        AdvMarketTreeView.Nodes.Add("1649", "Market Alioth 18")
        AdvMarketTreeView.Nodes.Add("1650", "Market Alioth 19")
        AdvMarketTreeView.Nodes.Add("1641", "Market Alioth 20")
        AdvMarketTreeView.Nodes.Add("1755", "Market Sanctuary 01")
        AdvMarketTreeView.Nodes.Add("1889", "Market Sanctuary 02")
        AdvMarketTreeView.Nodes.Add("1890", "Market Sanctuary 03")
        AdvMarketTreeView.Nodes.Add("1891", "Market Sanctuary 04")
        AdvMarketTreeView.Nodes.Add("1892", "Market Sanctuary 05")
        AdvMarketTreeView.Nodes.Add("1893", "Market Sanctuary 06")
        AdvMarketTreeView.Nodes.Add("1894", "Market Sanctuary 07")
        AdvMarketTreeView.Nodes.Add("1895", "Market Sanctuary 08")
        AdvMarketTreeView.Nodes.Add("1896", "Market Sanctuary 09")
        AdvMarketTreeView.Nodes.Add("1756", "Market Sanctuary 10")
        AdvMarketTreeView.Nodes.Add("1704", "Market Sanctuary 11")
        AdvMarketTreeView.Nodes.Add("1705", "Market Sanctuary 12")
        AdvMarketTreeView.Nodes.Add("1706", "Market Sanctuary 13")
        AdvMarketTreeView.Nodes.Add("1707", "Market Sanctuary 14")
        AdvMarketTreeView.Nodes.Add("1708", "Market Sanctuary 15")
        AdvMarketTreeView.Nodes.Add("1709", "Market Sanctuary 16")
        AdvMarketTreeView.Nodes.Add("1710", "Market Sanctuary 17")
        AdvMarketTreeView.Nodes.Add("1711", "Market Sanctuary 18")
        AdvMarketTreeView.Nodes.Add("1712", "Market Sanctuary 19")
        AdvMarketTreeView.Nodes.Add("1703", "Market Sanctuary 20")
        AdvMarketTreeView.Nodes.Add("1699", "Market Alioth Moon I 1")
        AdvMarketTreeView.Nodes.Add("1700", "Market Alioth Moon I 2")
        AdvMarketTreeView.Nodes.Add("1701", "Market Alioth Moon IV 1")
        AdvMarketTreeView.Nodes.Add("1702", "Market Alioth Moon IV 2")
        AdvMarketTreeView.Nodes.Add("1635", "Market Madis 1")
        AdvMarketTreeView.Nodes.Add("1636", "Market Madis 2")
        AdvMarketTreeView.Nodes.Add("1637", "Market Madis 3")
        AdvMarketTreeView.Nodes.Add("1638", "Market Madis 4")
        AdvMarketTreeView.Nodes.Add("1639", "Market Madis 5")
        AdvMarketTreeView.Nodes.Add("1640", "Market Madis 6")
        AdvMarketTreeView.Nodes.Add("1693", "Market Madis Moon I 1")
        AdvMarketTreeView.Nodes.Add("1694", "Market Madis Moon I 2")
        AdvMarketTreeView.Nodes.Add("1695", "Market Madis Moon II 1")
        AdvMarketTreeView.Nodes.Add("1696", "Market Madis Moon II 2")
        AdvMarketTreeView.Nodes.Add("1697", "Market Madis Moon III 1")
        AdvMarketTreeView.Nodes.Add("1698", "Market Madis Moon III 2")
        AdvMarketTreeView.Nodes.Add("1651", "Market Thades 1")
        AdvMarketTreeView.Nodes.Add("1652", "Market Thades 2")
        AdvMarketTreeView.Nodes.Add("1653", "Market Thades 3")
        AdvMarketTreeView.Nodes.Add("1654", "Market Thades 4")
        AdvMarketTreeView.Nodes.Add("1655", "Market Thades 5")
        AdvMarketTreeView.Nodes.Add("1656", "Market Thades 6")
        AdvMarketTreeView.Nodes.Add("1713", "Market Thades Moon I 1")
        AdvMarketTreeView.Nodes.Add("1714", "Market Thades Moon I 2")
        AdvMarketTreeView.Nodes.Add("1715", "Market Thades Moon II 1")
        AdvMarketTreeView.Nodes.Add("1716", "Market Thades Moon II 2")
        'AdvMarketTreeView.Nodes.Add("35747", "Small Market") -- Location unknown. It's in the market list, coords of 0,0,0. Maybe a dev-only object? Or just something that doesnt actually exist.
        AdvMarketTreeView.Nodes.Add("1745", "Market Ion 1")
        AdvMarketTreeView.Nodes.Add("1746", "Market Ion 2")
        AdvMarketTreeView.Nodes.Add("1747", "Market Ion 3")
        AdvMarketTreeView.Nodes.Add("1748", "Market Ion 4")
        AdvMarketTreeView.Nodes.Add("1749", "Market Ion 5")
        AdvMarketTreeView.Nodes.Add("1750", "Market Ion 6")
        AdvMarketTreeView.Nodes.Add("1751", "Market Ion Moon I 1")
        AdvMarketTreeView.Nodes.Add("1752", "Market Ion Moon I 2")
        AdvMarketTreeView.Nodes.Add("1753", "Market Ion Moon II 1")
        AdvMarketTreeView.Nodes.Add("1754", "Market Ion Moon II 2")
        AdvMarketTreeView.Nodes.Add("1657", "Market Talemai 1")
        AdvMarketTreeView.Nodes.Add("1658", "Market Talemai 2")
        AdvMarketTreeView.Nodes.Add("1659", "Market Talemai 3")
        AdvMarketTreeView.Nodes.Add("1660", "Market Talemai 4")
        AdvMarketTreeView.Nodes.Add("1661", "Market Talemai 5")
        AdvMarketTreeView.Nodes.Add("1662", "Market Talemai 6")
        AdvMarketTreeView.Nodes.Add("1721", "Market Talemai Moon I 1")
        AdvMarketTreeView.Nodes.Add("1722", "Market Talemai Moon I 2")
        AdvMarketTreeView.Nodes.Add("1717", "Market Talemai Moon II 1")
        AdvMarketTreeView.Nodes.Add("1718", "Market Talemai Moon II 2")
        AdvMarketTreeView.Nodes.Add("1719", "Market Talemai Moon III 1")
        AdvMarketTreeView.Nodes.Add("1720", "Market Talemai Moon III 2")
        AdvMarketTreeView.Nodes.Add("1663", "Market Feli 1")
        AdvMarketTreeView.Nodes.Add("1664", "Market Feli 2")
        AdvMarketTreeView.Nodes.Add("1665", "Market Feli 3")
        AdvMarketTreeView.Nodes.Add("1666", "Market Feli 4")
        AdvMarketTreeView.Nodes.Add("1667", "Market Feli 5")
        AdvMarketTreeView.Nodes.Add("1668", "Market Feli 6")
        AdvMarketTreeView.Nodes.Add("1723", "Market Feli Moon I 1")
        AdvMarketTreeView.Nodes.Add("1724", "Market Feli Moon I 2")
        AdvMarketTreeView.Nodes.Add("1669", "Market Sicari 1")
        AdvMarketTreeView.Nodes.Add("1670", "Market Sicari 2")
        AdvMarketTreeView.Nodes.Add("1671", "Market Sicari 3")
        AdvMarketTreeView.Nodes.Add("1672", "Market Sicari 4")
        AdvMarketTreeView.Nodes.Add("1673", "Market Sicari 5")
        AdvMarketTreeView.Nodes.Add("1674", "Market Sicari 6")
        AdvMarketTreeView.Nodes.Add("1675", "Market Sinnen 1")
        AdvMarketTreeView.Nodes.Add("1676", "Market Sinnen 2")
        AdvMarketTreeView.Nodes.Add("1677", "Market Sinnen 3")
        AdvMarketTreeView.Nodes.Add("1678", "Market Sinnen 4")
        AdvMarketTreeView.Nodes.Add("1679", "Market Sinnen 5")
        AdvMarketTreeView.Nodes.Add("1680", "Market Sinnen 6")
        AdvMarketTreeView.Nodes.Add("1681", "Market Teoma 1")
        AdvMarketTreeView.Nodes.Add("1682", "Market Teoma 2")
        AdvMarketTreeView.Nodes.Add("1683", "Market Teoma 3")
        AdvMarketTreeView.Nodes.Add("1684", "Market Teoma 4")
        AdvMarketTreeView.Nodes.Add("1685", "Market Teoma 5")
        AdvMarketTreeView.Nodes.Add("1686", "Market Teoma 6")
        AdvMarketTreeView.Nodes.Add("1687", "Market Jago 1")
        AdvMarketTreeView.Nodes.Add("1688", "Market Jago 2")
        AdvMarketTreeView.Nodes.Add("1689", "Market Jago 3")
        AdvMarketTreeView.Nodes.Add("1690", "Market Jago 4")
        AdvMarketTreeView.Nodes.Add("1691", "Market Jago 5")
        AdvMarketTreeView.Nodes.Add("1692", "Market Jago 6")
        AdvMarketTreeView.Nodes.Add("1725", "Market Sinnen Moon I 1")
        AdvMarketTreeView.Nodes.Add("1726", "Market Sinnen Moon I 2")
        AdvMarketTreeView.Nodes.Add("1727", "Market Lacobus 1")
        AdvMarketTreeView.Nodes.Add("1728", "Market Lacobus 2")
        AdvMarketTreeView.Nodes.Add("1729", "Market Lacobus 3")
        AdvMarketTreeView.Nodes.Add("1730", "Market Lacobus 4")
        AdvMarketTreeView.Nodes.Add("1731", "Market Lacobus 5")
        AdvMarketTreeView.Nodes.Add("1732", "Market Lacobus 6")
        AdvMarketTreeView.Nodes.Add("1735", "Market Lacobus Moon I 1")
        AdvMarketTreeView.Nodes.Add("1736", "Market Lacobus Moon I 2")
        AdvMarketTreeView.Nodes.Add("1737", "Market Lacobus Moon II 1")
        AdvMarketTreeView.Nodes.Add("1738", "Market Lacobus Moon II 2")
        AdvMarketTreeView.Nodes.Add("1733", "Market Lacobus Moon III 1")
        AdvMarketTreeView.Nodes.Add("1734", "Market Lacobus Moon III 2")
        AdvMarketTreeView.Nodes.Add("1739", "Market Symeon 1")
        AdvMarketTreeView.Nodes.Add("1740", "Market Symeon 2")
        AdvMarketTreeView.Nodes.Add("1741", "Market Symeon 3")
        AdvMarketTreeView.Nodes.Add("1742", "Market Symeon 4")
        AdvMarketTreeView.Nodes.Add("1743", "Market Symeon 5")
        AdvMarketTreeView.Nodes.Add("1744", "Market Symeon 6")
    End Sub

    Private Sub InitItemList()
        Dim categorynode1 As TreeNode = ItemTree.Nodes.Add("node1", "Aphelia Mission Packages")
        Dim categorynode11 As TreeNode = categorynode1.Nodes.Add("node11", "Very Large Mission Packages")
        Dim categorynode12 As TreeNode = categorynode1.Nodes.Add("node12", "Large Mission Packages")
        Dim categorynode13 As TreeNode = categorynode1.Nodes.Add("node13", "Medium Mission Packages")
        Dim categorynode14 As TreeNode = categorynode1.Nodes.Add("node14", "Small Mission Packages")
        Dim categorynode15 As TreeNode = categorynode1.Nodes.Add("node15", "Very Small Mission Packages")


        Dim categorynode2 As TreeNode = ItemTree.Nodes.Add("node2", "Consumables")
        Dim categorynode21 As TreeNode = categorynode2.Nodes.Add("node1", "Ammunition")
        Dim categorynode211 As TreeNode = categorynode21.Nodes.Add("node1", "Cannon Ammo")
        Dim categorynode2111 As TreeNode = categorynode211.Nodes.Add("node1", "Cannon Ammo L")
        Dim categorynode21111 As TreeNode = categorynode2111.Nodes.Add("node1", "Kinetic Ammo")
        categorynode21111.Nodes.Add("item234876889", "Cannon Kinetic Ammo L")
        categorynode21111.Nodes.Add("item2564171448", "Cannon Precision Kinetic Ammo L")
        categorynode21111.Nodes.Add("item3111934432", "Cannon Heavy Kinetic Ammo L")
        categorynode21111.Nodes.Add("item1368644517", "Cannon Defense Kinetic Ammo L")
        categorynode21111.Nodes.Add("item2754186867", "Cannon Agile Kinetic Ammo L")
        Dim categorynode21112 As TreeNode = categorynode2111.Nodes.Add("node1", "Thermic Ammo")
        categorynode21112.Nodes.Add("item2150864517", "Cannon Thermic Ammo L")
        categorynode21112.Nodes.Add("item2793941079", "Cannon Precision Thermic Ammo L")
        categorynode21112.Nodes.Add("item3705351908", "Cannon Heavy Thermic Ammo L")
        categorynode21112.Nodes.Add("item99109453", "Cannon Defense Thermic Ammo L")
        categorynode21112.Nodes.Add("item3564640746", "Cannon Agile Thermic Ammo L")
        Dim categorynode2112 As TreeNode = categorynode211.Nodes.Add("node1", "Cannon Ammo M")
        Dim categorynode21121 As TreeNode = categorynode2112.Nodes.Add("node1", "Kinetic Ammo")
        categorynode21121.Nodes.Add("item1087392944", "Cannon Kinetic Ammo M")
        categorynode21121.Nodes.Add("item1837088359", "Cannon Precision Kinetic Ammo M")
        categorynode21121.Nodes.Add("item2318901128", "Cannon Heavy Kinetic Ammo M")
        categorynode21121.Nodes.Add("item3802426170", "Cannon Defense Kinetic Ammo M")
        categorynode21121.Nodes.Add("item3901365200", "Cannon Agile Kinetic Ammo M")
        Dim categorynode21122 As TreeNode = categorynode2112.Nodes.Add("node1", "Thermic Ammo")
        categorynode21122.Nodes.Add("item2886559338", "Cannon Thermic Ammo M")
        categorynode21122.Nodes.Add("item1445049256", "Cannon Precision Thermic Ammo M")
        categorynode21122.Nodes.Add("item1627746607", "Cannon Heavy Thermic Ammo M")
        categorynode21122.Nodes.Add("item3352702648", "Cannon Defense Thermic Ammo M")
        categorynode21122.Nodes.Add("item1958427908", "Cannon Agile Thermic Ammo M")
        Dim categorynode2113 As TreeNode = categorynode211.Nodes.Add("node1", "Cannon Ammo S")
        Dim categorynode21131 As TreeNode = categorynode2113.Nodes.Add("node1", "Kinetic Ammo")
        categorynode21131.Nodes.Add("item864736227", "Cannon Kinetic Ammo S")
        categorynode21131.Nodes.Add("item1256032552", "Cannon Precision Kinetic Ammo S")
        categorynode21131.Nodes.Add("item2013297395", "Cannon Heavy Kinetic Ammo S")
        categorynode21131.Nodes.Add("item52497197", "Cannon Defense Kinetic Ammo S")
        categorynode21131.Nodes.Add("item2048035010", "Cannon Agile Kinetic Ammo S")
        Dim categorynode21132 As TreeNode = categorynode2113.Nodes.Add("node1", "Thermic Ammo")
        categorynode21132.Nodes.Add("item3253142563", "Cannon Thermic Ammo S")
        categorynode21132.Nodes.Add("item1081563239", "Cannon Precision Thermic Ammo S")
        categorynode21132.Nodes.Add("item1912399735", "Cannon Heavy Thermic Ammo S")
        categorynode21132.Nodes.Add("item846420746", "Cannon Defense Thermic Ammo S")
        categorynode21132.Nodes.Add("item2014631386", "Cannon Agile Thermic Ammo S")
        Dim categorynode2114 As TreeNode = categorynode211.Nodes.Add("node1", "Cannon Ammo XS")
        Dim categorynode21141 As TreeNode = categorynode2114.Nodes.Add("node1", "Kinetic Ammo")
        categorynode21141.Nodes.Add("item3818049598", "Cannon Kinetic Ammo XS")
        categorynode21141.Nodes.Add("item3238320397", "Cannon Precision Kinetic Ammo XS")
        categorynode21141.Nodes.Add("item1980351716", "Cannon Heavy Kinetic Ammo XS")
        categorynode21141.Nodes.Add("item2680492642", "Cannon Defense Kinetic Ammo XS")
        categorynode21141.Nodes.Add("item2746947552", "Cannon Agile Kinetic Ammo XS")
        Dim categorynode21142 As TreeNode = categorynode2114.Nodes.Add("node1", "Thermic Ammo")
        categorynode21142.Nodes.Add("item3607061517", "Cannon Thermic Ammo XS")
        categorynode21142.Nodes.Add("item2917884317", "Cannon Precision Thermic Ammo XS")
        categorynode21142.Nodes.Add("item726551231", "Cannon Heavy Thermic Ammo XS")
        categorynode21142.Nodes.Add("item147467923", "Cannon Defense Thermic Ammo XS")
        categorynode21142.Nodes.Add("item370579567", "Cannon Agile Thermic Ammo XS")





        Dim categorynode212 As TreeNode = categorynode21.Nodes.Add("node1", "Laser Ammo")

        Dim categorynode213 As TreeNode = categorynode21.Nodes.Add("node1", "Missile Pod Ammo")

        Dim categorynode214 As TreeNode = categorynode21.Nodes.Add("node1", "Railgun Ammo")


        Dim categorynode22 As TreeNode = categorynode2.Nodes.Add("node1", "Fireworks")

        Dim categorynode23 As TreeNode = categorynode2.Nodes.Add("node1", "Scrap")

        Dim categorynode24 As TreeNode = categorynode2.Nodes.Add("node1", "Warp Cell")



        Dim categorynode3 As TreeNode = ItemTree.Nodes.Add("node3", "Data Items")
        Dim categorynode31 As TreeNode = categorynode3.Nodes.Add("node1", "Schematics")


        Dim categorynode4 As TreeNode = ItemTree.Nodes.Add("node4", "Elements")
        Dim categorynode41 As TreeNode = categorynode4.Nodes.Add("node4", "Combat & Defense")

        Dim categorynode42 As TreeNode = categorynode4.Nodes.Add("node4", "Furniture & Appliances")

        Dim categorynode43 As TreeNode = categorynode4.Nodes.Add("node4", "Industry & Infrastructure")

        Dim categorynode44 As TreeNode = categorynode4.Nodes.Add("node4", "Planetary")

        Dim categorynode45 As TreeNode = categorynode4.Nodes.Add("node4", "Systems")

        Dim categorynode46 As TreeNode = categorynode4.Nodes.Add("node4", "Transportation")



        Dim categorynode5 As TreeNode = ItemTree.Nodes.Add("node5", "Materials")
        Dim categorynode51 As TreeNode = categorynode5.Nodes.Add("node5", "Fuel")

        Dim categorynode52 As TreeNode = categorynode5.Nodes.Add("node5", "Honeycomb")

        Dim categorynode53 As TreeNode = categorynode5.Nodes.Add("node5", "Minable")

        Dim categorynode54 As TreeNode = categorynode5.Nodes.Add("node5", "Refined")



        Dim categorynode6 As TreeNode = ItemTree.Nodes.Add("node6", "Parts")
        Dim categorynode61 As TreeNode = categorynode6.Nodes.Add("node6", "Complex")

        Dim categorynode62 As TreeNode = categorynode6.Nodes.Add("node6", "Exceptional")

        Dim categorynode63 As TreeNode = categorynode6.Nodes.Add("node6", "Functional")

        Dim categorynode64 As TreeNode = categorynode6.Nodes.Add("node6", "Intermediary")

        Dim categorynode65 As TreeNode = categorynode6.Nodes.Add("node6", "Structural")





        ItemTree.Nodes.Add("item2266058296", "Canopy Metal flat S")
        ItemTree.Nodes.Add("item3729727572", "Canopy Metal flat M")
        ItemTree.Nodes.Add("item2635025376", "Canopy Metal flat L")
        ItemTree.Nodes.Add("item767916091", "Canopy Metal corner S")
        ItemTree.Nodes.Add("item283728321", "Canopy Metal corner M")
        ItemTree.Nodes.Add("item2118024887", "Canopy Metal corner L")
        ItemTree.Nodes.Add("item3294726704", "Canopy Metal tilted S")
        ItemTree.Nodes.Add("item4015784029", "Canopy Metal tilted M")
        ItemTree.Nodes.Add("item798367766", "Canopy Metal tilted L")
        ItemTree.Nodes.Add("item1339058404", "Canopy Metal triangle S")
        ItemTree.Nodes.Add("item265675573", "Canopy Metal triangle M")
        ItemTree.Nodes.Add("item3943842048", "Canopy Metal triangle L")

        ItemTree.Nodes.Add("item3204140764", "Exotic Mining Unit L")
        ItemTree.Nodes.Add("item3204140767", "Rare Mining Unit L")
        ItemTree.Nodes.Add("item3204140766", "Advanced Mining Unit L")
        ItemTree.Nodes.Add("item3204140761", "Uncommon Mining Unit L")
        ItemTree.Nodes.Add("item1949562989", "Basic Mining Unit S")
        ItemTree.Nodes.Add("item3204140760", "Basic Mining Unit L")

        ItemTree.Nodes.Add("item2421673145", "Space Fuel Tank XS")

        ItemTree.Nodes.Add("item2882830295", "Shield Generator XS")
        ItemTree.Nodes.Add("item3696387320", "Shield Generator S")
        ItemTree.Nodes.Add("item254923774", "Shield Generator M")
        ItemTree.Nodes.Add("item2034818941", "Shield Generator L")

        ItemTree.Nodes.Add("item2413564665", "Deep Space Asteroid Tracker")

        ItemTree.Nodes.Add("item2814304696", "Black Steel Panel")
        ItemTree.Nodes.Add("item2814304696", "Dark Gray Steel Panel")
        ItemTree.Nodes.Add("item2814304696", "Gray Steel Panel")
        ItemTree.Nodes.Add("item2814304696", "Green Steel Panel")
        ItemTree.Nodes.Add("item2814304696", "Ice Steel Panel")
        ItemTree.Nodes.Add("item2814304696", "Light Gray Steel Panel")
        ItemTree.Nodes.Add("item2814304696", "Military Steel Panel")
        ItemTree.Nodes.Add("item2814304696", "Orange Steel Panel")
        ItemTree.Nodes.Add("item2814304696", "Purple Steel Panel")
        ItemTree.Nodes.Add("item2814304696", "Red Steel Panel")
        ItemTree.Nodes.Add("item2814304696", "Sky Steel Panel")
        ItemTree.Nodes.Add("item2814304696", "Yellow Steel Panel")
        ItemTree.Nodes.Add("item2814304696", "White Steel Panel")
        ItemTree.Nodes.Add("item2814304696", "Painted Yellow Steel")
        ItemTree.Nodes.Add("item2814304696", "Painted White Steel")
        ItemTree.Nodes.Add("item2814304696", "Painted Red Steel")

        ItemTree.Nodes.Add("item111253038", "Advanced Precision Railgun M")
        ItemTree.Nodes.Add("item111253039", "Rare Precision Railgun M")
        ItemTree.Nodes.Add("item111253024", "Exotic Precision Railgun M")

        ItemTree.Nodes.Add("item1767704175", "Advanced Agile Railgun S")
        ItemTree.Nodes.Add("item1767704174", "Rare Agile Railgun S")
        ItemTree.Nodes.Add("item1767704161", "Exotic Agile Railgun S")

        ItemTree.Nodes.Add("item223437801", "Advanced Defense Railgun S")
        ItemTree.Nodes.Add("item223437800", "Rare Defense Railgun S")
        ItemTree.Nodes.Add("item223437807", "Exotic Defense Railgun S")

        ItemTree.Nodes.Add("item2991505111", "Advanced Heavy Railgun S")
        ItemTree.Nodes.Add("item2991505104", "Rare Heavy Railgun S")
        ItemTree.Nodes.Add("item2991505105", "Exotic Heavy Railgun S")

        ItemTree.Nodes.Add("item831043069", "Advanced Precision Railgun S")
        ItemTree.Nodes.Add("item831043070", "Rare Precision Railgun S")
        ItemTree.Nodes.Add("item831043071", "Exotic Precision Railgun S")

        ItemTree.Nodes.Add("item1641776328", "Exotic Heavy Railgun M")
        ItemTree.Nodes.Add("item1641776331", "Rare Heavy Railgun M")
        ItemTree.Nodes.Add("item1641776330", "Advanced Heavy Railgun M")

        ItemTree.Nodes.Add("item3396072237", "Exotic Defense Railgun M")

        ItemTree.Nodes.Add("item2239993844", "Advanced Precision Missile XS")
        ItemTree.Nodes.Add("item2239993845", "Rare Precision Missile XS")
        ItemTree.Nodes.Add("item2239993846", "Exotic Precision Missile XS")

        ItemTree.Nodes.Add("item3650288374", "Advanced Agile Missile L")
        ItemTree.Nodes.Add("item3650288369", "Rare Agile Missile L")
        ItemTree.Nodes.Add("item3650288368", "Exotic Agile Missile L")

        ItemTree.Nodes.Add("item3453451048", "Advanced Defense Missile L")
        ItemTree.Nodes.Add("item3453451051", "Rare Defense Missile L")
        ItemTree.Nodes.Add("item3453451050", "Exotic Defense Missile L")

        ItemTree.Nodes.Add("item3611570509", "Exotic Heavy Missile XS")
        ItemTree.Nodes.Add("item708864069", "Advanced Heavy Missile L")
        ItemTree.Nodes.Add("item708864067", "Exotic Heavy Missile L")

        ItemTree.Nodes.Add("item1205879485", "Advanced Precision Missile L")
        ItemTree.Nodes.Add("item1205879482", "Rare Precision Missile L")
        ItemTree.Nodes.Add("item1205879483", "Exotic Precision Missile L")

        ItemTree.Nodes.Add("item598736203", "Advanced Agile Missile M")
        ItemTree.Nodes.Add("item598736196", "Rare Agile Missile M")
        ItemTree.Nodes.Add("item598736197", "Exotic Agile Missile M")

        ItemTree.Nodes.Add("item1068910670", "Advanced Defense Missile M")
        ItemTree.Nodes.Add("item1068910671", "Rare Defense Missile M")
        ItemTree.Nodes.Add("item1068910656", "Exotic Defense Missile M")

        ItemTree.Nodes.Add("item708864066", "Rare Heavy Missile L")

        ItemTree.Nodes.Add("item3611570511", "Advanced Heavy Missile XS")
        ItemTree.Nodes.Add("item3611570508", "Rare Heavy Missile XS")

        ItemTree.Nodes.Add("item134390789", "Exotic Defense Missile XS")

        ItemTree.Nodes.Add("item3840109424", "Advanced Precision Laser M")
        ItemTree.Nodes.Add("item3840109425", "Rare Precision Laser M")
        ItemTree.Nodes.Add("item3840109426", "Exotic Precision Laser M")

        ItemTree.Nodes.Add("item4124398193", "Advanced Agile Laser S")
        ItemTree.Nodes.Add("item4124398192", "Rare Agile Laser S")
        ItemTree.Nodes.Add("item4124398199", "Exotic Agile Laser S")

        ItemTree.Nodes.Add("item1737118473", "Advanced Defense Laser S")
        ItemTree.Nodes.Add("item1737118474", "Rare Defense Laser S")
        ItemTree.Nodes.Add("item1737118475", "Exotic Defense Laser S")

        ItemTree.Nodes.Add("item338218847", "Advanced Heavy Laser S")
        ItemTree.Nodes.Add("item338218841", "Exotic Heavy Laser S")
        ItemTree.Nodes.Add("item3730148334", "Advanced Precision Laser S")
        ItemTree.Nodes.Add("item3730148335", "Rare Precision Laser S")
        ItemTree.Nodes.Add("item3730148320", "Exotic Precision Laser S")
        ItemTree.Nodes.Add("item1780076560", "Advanced Agile Missile XS")
        ItemTree.Nodes.Add("item1780076561", "Rare Agile Missile XS")
        ItemTree.Nodes.Add("item1780076562", "Exotic Agile Missile XS")
        ItemTree.Nodes.Add("item134390791", "Advanced Defense Missile XS")
        ItemTree.Nodes.Add("item134390788", "Rare Defense Missile XS")
        ItemTree.Nodes.Add("item338218840", "Rare Heavy Laser S")
        ItemTree.Nodes.Add("item1102564708", "Advanced Heavy Missile M")
        ItemTree.Nodes.Add("item1102564706", "Exotic Heavy Missile M")
        ItemTree.Nodes.Add("item690643397", "Advanced Precision Railgun XS")
        ItemTree.Nodes.Add("item690643396", "Rare Precision Railgun XS")
        ItemTree.Nodes.Add("item690643419", "Exotic Precision Railgun XS")
        ItemTree.Nodes.Add("item4062760160", "Advanced Agile Railgun L")
        ItemTree.Nodes.Add("item4062760163", "Rare Agile Railgun L")
        ItemTree.Nodes.Add("item4062760162", "Exotic Agile Railgun L")
        ItemTree.Nodes.Add("item3670363955", "Advanced Defense Railgun L")
        ItemTree.Nodes.Add("item3670363952", "Rare Defense Railgun L")
        ItemTree.Nodes.Add("item3670363953", "Exotic Defense Railgun L")
        ItemTree.Nodes.Add("item1816732409", "Exotic Heavy Railgun XS")
        ItemTree.Nodes.Add("item30018129", "Advanced Heavy Railgun L")
        ItemTree.Nodes.Add("item30018135", "Exotic Heavy Railgun L")
        ItemTree.Nodes.Add("item2916726762", "Advanced Precision Railgun L")
        ItemTree.Nodes.Add("item2916726763", "Rare Precision Railgun L")
        ItemTree.Nodes.Add("item2916726760", "Exotic Precision Railgun L")
        ItemTree.Nodes.Add("item3057550275", "Advanced Agile Railgun M")
        ItemTree.Nodes.Add("item3057550300", "Rare Agile Railgun M")
        ItemTree.Nodes.Add("item3057550301", "Exotic Agile Railgun M")
        ItemTree.Nodes.Add("item3396072211", "Advanced Defense Railgun M")
        ItemTree.Nodes.Add("item3396072236", "Rare Defense Railgun M")
        ItemTree.Nodes.Add("item30018128", "Rare Heavy Railgun L")
        ItemTree.Nodes.Add("item1102564707", "Rare Heavy Missile M")
        ItemTree.Nodes.Add("item1816732408", "Rare Heavy Railgun XS")
        ItemTree.Nodes.Add("item2108818541", "Exotic Defense Railgun XS")
        ItemTree.Nodes.Add("item1217643701", "Advanced Precision Missile M")
        ItemTree.Nodes.Add("item1217643700", "Rare Precision Missile M")
        ItemTree.Nodes.Add("item1217644363", "Exotic Precision Missile M")
        ItemTree.Nodes.Add("item1843877007", "Advanced Agile Missile S")
        ItemTree.Nodes.Add("item1843877006", "Rare Agile Missile S")
        ItemTree.Nodes.Add("item1843877005", "Exotic Agile Missile S")
        ItemTree.Nodes.Add("item136359050", "Advanced Defense Missile S")
        ItemTree.Nodes.Add("item136359051", "Rare Defense Missile S")
        ItemTree.Nodes.Add("item136359048", "Exotic Defense Missile S")
        ItemTree.Nodes.Add("item1816732415", "Advanced Heavy Railgun XS")
        ItemTree.Nodes.Add("item1100091709", "Advanced Heavy Missile S")
        ItemTree.Nodes.Add("item1100091711", "Exotic Heavy Missile S")
        ItemTree.Nodes.Add("item2668363439", "Advanced Precision Missile S")
        ItemTree.Nodes.Add("item2668363432", "Rare Precision Missile S")
        ItemTree.Nodes.Add("item2668363433", "Exotic Precision Missile S")
        ItemTree.Nodes.Add("item549955075", "Advanced Agile Railgun XS")
        ItemTree.Nodes.Add("item549955100", "Rare Agile Railgun XS")
        ItemTree.Nodes.Add("item549955101", "Exotic Agile Railgun XS")
        ItemTree.Nodes.Add("item2108818543", "Advanced Defense Railgun XS")
        ItemTree.Nodes.Add("item2108818540", "Rare Defense Railgun XS")
        ItemTree.Nodes.Add("item1100091708", "Rare Heavy Missile S")
        ItemTree.Nodes.Add("item373451737", "Container XL")
        ItemTree.Nodes.Add("item572613525", "Expanded Container XL")
        ItemTree.Nodes.Add("item333062081", "Dispenser")
        ItemTree.Nodes.Add("item3588766026", "Exotic Heavy Laser M")
        ItemTree.Nodes.Add("item2236273961", "Canopy Windshield triangle L")
        ItemTree.Nodes.Add("item3521312761", "Canopy Windshield triangle M")
        ItemTree.Nodes.Add("item2792485016", "Canopy Windshield triangle S")
        ItemTree.Nodes.Add("item3588765877", "Rare Heavy Laser M")
        ItemTree.Nodes.Add("item3588765876", "Advanced Heavy Laser M")
        ItemTree.Nodes.Add("item3805044395", "Exotic Defense Laser M")
        ItemTree.Nodes.Add("item3455226645", "Advanced Precision Cannon XS")
        ItemTree.Nodes.Add("item3455226644", "Rare Precision Cannon XS")
        ItemTree.Nodes.Add("item3455226647", "Exotic Precision Cannon XS")
        ItemTree.Nodes.Add("item3152865672", "Advanced Agile Cannon L")
        ItemTree.Nodes.Add("item3152865673", "Rare Agile Cannon L")
        ItemTree.Nodes.Add("item3152865678", "Exotic Agile Cannon L")
        ItemTree.Nodes.Add("item418164306", "Advanced Defense Cannon L")
        ItemTree.Nodes.Add("item418164307", "Rare Defense Cannon L")
        ItemTree.Nodes.Add("item418164308", "Exotic Defense Cannon L")
        ItemTree.Nodes.Add("item3384934783", "Exotic Heavy Cannon XS")
        ItemTree.Nodes.Add("item3960316615", "Advanced Heavy Cannon L")
        ItemTree.Nodes.Add("item3960316609", "Exotic Heavy Cannon L")
        ItemTree.Nodes.Add("item845167470", "Advanced Precision Cannon L")
        ItemTree.Nodes.Add("item845167469", "Rare Precision Cannon L")
        ItemTree.Nodes.Add("item845167468", "Exotic Precision Cannon L")
        ItemTree.Nodes.Add("item2672575276", "Advanced Agile Cannon M")
        ItemTree.Nodes.Add("item2672575279", "Rare Agile Cannon M")
        ItemTree.Nodes.Add("item2672575278", "Exotic Agile Cannon M")
        ItemTree.Nodes.Add("item2383624964", "Advanced Defense Cannon M")
        ItemTree.Nodes.Add("item2383624965", "Rare Defense Cannon M")
        ItemTree.Nodes.Add("item3960316608", "Rare Heavy Cannon L")
        ItemTree.Nodes.Add("item2383624966", "Exotic Defense Cannon M")
        ItemTree.Nodes.Add("item3384934780", "Rare Heavy Cannon XS")
        ItemTree.Nodes.Add("item3467785553", "Exotic Defense Cannon XS")
        ItemTree.Nodes.Add("item2608116214", "Exotic Quick-Wired Space Radar M")
        ItemTree.Nodes.Add("item809783408", "Advanced Phased-Array Space Radar S")
        ItemTree.Nodes.Add("item809783311", "Rare Phased-Array Space Radar S")
        ItemTree.Nodes.Add("item809783310", "Exotic Phased-Array Space Radar S")
        ItemTree.Nodes.Add("item2375197139", "Advanced Protected Space Radar S")
        ItemTree.Nodes.Add("item2375197136", "Rare Protected Space Radar S")
        ItemTree.Nodes.Add("item2375197137", "Exotic Protected Space Radar S")
        ItemTree.Nodes.Add("item838245688", "Advanced Quick-Wired Space Radar S")
        ItemTree.Nodes.Add("item838245691", "Rare Quick-Wired Space Radar S")
        ItemTree.Nodes.Add("item3384934781", "Advanced Heavy Cannon XS")
        ItemTree.Nodes.Add("item838245690", "Exotic Quick-Wired Space Radar S")
        ItemTree.Nodes.Add("item63667997", "Transponder")
        ItemTree.Nodes.Add("item684853120", "Advanced Agile Cannon XS")
        ItemTree.Nodes.Add("item684853151", "Rare Agile Cannon XS")
        ItemTree.Nodes.Add("item684853150", "Exotic Agile Cannon XS")
        ItemTree.Nodes.Add("item3467785559", "Advanced Defense Cannon XS")
        ItemTree.Nodes.Add("item3467785552", "Rare Defense Cannon XS")
        ItemTree.Nodes.Add("item2608116213", "Rare Quick-Wired Space Radar M")
        ItemTree.Nodes.Add("item2188788020", "Advanced Heavy Cannon M")
        ItemTree.Nodes.Add("item2188788022", "Exotic Heavy Cannon M")
        ItemTree.Nodes.Add("item1604660449", "Advanced Precision Laser XS")
        ItemTree.Nodes.Add("item1604660448", "Rare Precision Laser XS")
        ItemTree.Nodes.Add("item1604660455", "Exotic Precision Laser XS")
        ItemTree.Nodes.Add("item679378436", "Advanced Agile Laser L")
        ItemTree.Nodes.Add("item679378437", "Rare Agile Laser L")
        ItemTree.Nodes.Add("item679378438", "Exotic Agile Laser L")
        ItemTree.Nodes.Add("item3991674478", "Advanced Defense Laser L")
        ItemTree.Nodes.Add("item3991674479", "Rare Defense Laser L")
        ItemTree.Nodes.Add("item3991674464", "Exotic Defense Laser L")
        ItemTree.Nodes.Add("item3698237863", "Exotic Heavy Laser XS")
        ItemTree.Nodes.Add("item4270062446", "Advanced Heavy Laser L")
        ItemTree.Nodes.Add("item4270062440", "Exotic Heavy Laser L")
        ItemTree.Nodes.Add("item2356629408", "Advanced Precision Laser L")
        ItemTree.Nodes.Add("item2356629409", "Rare Precision Laser L")
        ItemTree.Nodes.Add("item2356629410", "Exotic Precision Laser L")
        ItemTree.Nodes.Add("item360504284", "Advanced Agile Laser M")
        ItemTree.Nodes.Add("item360504287", "Rare Agile Laser M")
        ItemTree.Nodes.Add("item360504286", "Exotic Agile Laser M")
        ItemTree.Nodes.Add("item3805044393", "Advanced Defense Laser M")
        ItemTree.Nodes.Add("item3805044394", "Rare Defense Laser M")
        ItemTree.Nodes.Add("item4270062441", "Rare Heavy Laser L")
        ItemTree.Nodes.Add("item2188788021", "Rare Heavy Cannon M")
        ItemTree.Nodes.Add("item3698237862", "Rare Heavy Laser XS")
        ItemTree.Nodes.Add("item796456747", "Exotic Defense Laser XS")
        ItemTree.Nodes.Add("item2457342404", "Advanced Precision Cannon M")
        ItemTree.Nodes.Add("item2457342403", "Rare Precision Cannon M")
        ItemTree.Nodes.Add("item2457342402", "Exotic Precision Cannon M")
        ItemTree.Nodes.Add("item429894438", "Advanced Agile Cannon S")
        ItemTree.Nodes.Add("item429894437", "Rare Agile Cannon S")
        ItemTree.Nodes.Add("item429894436", "Exotic Agile Cannon S")
        ItemTree.Nodes.Add("item1073121333", "Advanced Defense Cannon S")
        ItemTree.Nodes.Add("item1073121334", "Rare Defense Cannon S")
        ItemTree.Nodes.Add("item1073121335", "Exotic Defense Cannon S")
        ItemTree.Nodes.Add("item3698237865", "Advanced Heavy Laser XS")
        ItemTree.Nodes.Add("item2058706007", "Advanced Heavy Cannon S")
        ItemTree.Nodes.Add("item2058706005", "Exotic Heavy Cannon S")
        ItemTree.Nodes.Add("item3567179843", "Advanced Precision Cannon S")
        ItemTree.Nodes.Add("item3567179842", "Rare Precision Cannon S")
        ItemTree.Nodes.Add("item3567179845", "Exotic Precision Cannon S")
        ItemTree.Nodes.Add("item3972697532", "Advanced Agile Laser XS")
        ItemTree.Nodes.Add("item3972697533", "Rare Agile Laser XS")
        ItemTree.Nodes.Add("item3972697534", "Exotic Agile Laser XS")
        ItemTree.Nodes.Add("item796456749", "Advanced Defense Laser XS")
        ItemTree.Nodes.Add("item796456746", "Rare Defense Laser XS")
        ItemTree.Nodes.Add("item2058706004", "Rare Heavy Cannon S")
        ItemTree.Nodes.Add("item2608116212", "Advanced Quick-Wired Space Radar M")
        ItemTree.Nodes.Add("item3060580945", "Rare Protected Space Radar M")
        ItemTree.Nodes.Add("item3060580944", "Exotic Protected Space Radar M")
        ItemTree.Nodes.Add("item2075264944", "Advanced Phased-Array Space Radar L")
        ItemTree.Nodes.Add("item2075264591", "Rare Phased-Array Space Radar L")
        ItemTree.Nodes.Add("item2075264590", "Exotic Phased-Array Space Radar L")
        ItemTree.Nodes.Add("item3250064333", "Rare Protected Space Radar L")
        ItemTree.Nodes.Add("item3250064334", "Exotic Protected Space Radar L")
        ItemTree.Nodes.Add("item3612800224", "Advanced Quick-Wired Space Radar L")
        ItemTree.Nodes.Add("item3612800255", "Rare Quick-Wired Space Radar L")
        ItemTree.Nodes.Add("item3612800254", "Exotic Quick-Wired Space Radar L")
        ItemTree.Nodes.Add("item1707018154", "Advanced Phased-Array Space Radar M")
        ItemTree.Nodes.Add("item1707018149", "Rare Phased-Array Space Radar M")
        ItemTree.Nodes.Add("item1707018148", "Exotic Phased-Array Space Radar M")
        ItemTree.Nodes.Add("item3060580950", "Advanced Protected Space Radar M")
        ItemTree.Nodes.Add("item3250064332", "Advanced Protected Space Radar L")
        ItemTree.Nodes.Add("item877202037", "Niobium Scrap")
        ItemTree.Nodes.Add("item2165650011", "Titanium Scrap")
        ItemTree.Nodes.Add("item3307634000", "Vanadium Scrap")
        ItemTree.Nodes.Add("item934426297", "Advanced Military Space Engine XL")
        ItemTree.Nodes.Add("item2814304696", "Painted Sky Steel")
        ItemTree.Nodes.Add("item734351314", "Painted Yellow Maraging Steel")
        ItemTree.Nodes.Add("item734351314", "Black Maraging Steel Panel")
        ItemTree.Nodes.Add("item734351314", "Dark Gray Maraging Steel Panel")
        ItemTree.Nodes.Add("item734351314", "Gray Maraging Steel Panel")
        ItemTree.Nodes.Add("item734351314", "Green Maraging Steel Panel")
        ItemTree.Nodes.Add("item734351314", "Ice Maraging Steel Panel")
        ItemTree.Nodes.Add("item734351314", "Light Gray Maraging Steel Panel")
        ItemTree.Nodes.Add("item734351314", "Military Maraging Steel Panel")
        ItemTree.Nodes.Add("item734351314", "Orange Maraging Steel Panel")
        ItemTree.Nodes.Add("item734351314", "Purple Maraging Steel Panel")
        ItemTree.Nodes.Add("item734351314", "Red Maraging Steel Panel")
        ItemTree.Nodes.Add("item734351314", "White Maraging Steel Panel")
        ItemTree.Nodes.Add("item734351314", "Yellow Maraging Steel Panel")
        ItemTree.Nodes.Add("item734351314", "Sky Maraging Steel Panel")
        ItemTree.Nodes.Add("item734351314", "Painted White Maraging Steel")
        ItemTree.Nodes.Add("item734351314", "Painted Sky Maraging Steel")
        ItemTree.Nodes.Add("item734351314", "Painted Red Maraging Steel")
        ItemTree.Nodes.Add("item734351314", "Painted Purple Maraging Steel")
        ItemTree.Nodes.Add("item483425306", "Painted Yellow Grade 5 Titanium Alloy")
        ItemTree.Nodes.Add("item483425306", "Black Grade 5 Titanium Alloy Panel")
        ItemTree.Nodes.Add("item483425306", "Dark Gray Grade 5 Titanium Alloy Panel")
        ItemTree.Nodes.Add("item483425306", "Gray Grade 5 Titanium Alloy Panel")
        ItemTree.Nodes.Add("item483425306", "Green Grade 5 Titanium Alloy Panel")
        ItemTree.Nodes.Add("item1972837708", "Aged Inconel")
        ItemTree.Nodes.Add("item1972837708", "Glossy Inconel")
        ItemTree.Nodes.Add("item1972837708", "Matte Inconel")
        ItemTree.Nodes.Add("item1972837708", "Painted Black Inconel")
        ItemTree.Nodes.Add("item1972837708", "Painted Dark Gray Inconel")
        ItemTree.Nodes.Add("item483425306", "Painted White Grade 5 Titanium Alloy")
        ItemTree.Nodes.Add("item1972837708", "Painted Gray Inconel")
        ItemTree.Nodes.Add("item1972837708", "Painted Ice Inconel")
        ItemTree.Nodes.Add("item1972837708", "Painted Light Gray Inconel")
        ItemTree.Nodes.Add("item1972837708", "Painted Military Inconel")
        ItemTree.Nodes.Add("item1972837708", "Painted Orange Inconel")
        ItemTree.Nodes.Add("item1972837708", "Painted Purple Inconel")
        ItemTree.Nodes.Add("item1972837708", "Painted Red Inconel")
        ItemTree.Nodes.Add("item1972837708", "Painted Sky Inconel")
        ItemTree.Nodes.Add("item1972837708", "Painted White Inconel")
        ItemTree.Nodes.Add("item1972837708", "Painted Yellow Inconel")
        ItemTree.Nodes.Add("item1972837708", "Black Inconel Panel")
        ItemTree.Nodes.Add("item1972837708", "Painted Green Inconel")
        ItemTree.Nodes.Add("item1972837708", "Dark Gray Inconel Panel ")
        ItemTree.Nodes.Add("item483425306", "Painted Sky Grade 5 Titanium Alloy")
        ItemTree.Nodes.Add("item483425306", "Painted Purple Grade 5 Titanium Alloy")
        ItemTree.Nodes.Add("item483425306", "Ice Grade 5 Titanium Alloy Panel")
        ItemTree.Nodes.Add("item483425306", "Light Gray Grade 5 Titanium Alloy Panel")
        ItemTree.Nodes.Add("item483425306", "Military Grade 5 Titanium Alloy Panel")
        ItemTree.Nodes.Add("item483425306", "Orange Grade 5 Titanium Alloy Panel")
        ItemTree.Nodes.Add("item483425306", "Purple Grade 5 Titanium Alloy Panel")
        ItemTree.Nodes.Add("item483425306", "Red Grade 5 Titanium Alloy Panel")
        ItemTree.Nodes.Add("item483425306", "Sky Grade 5 Titanium Alloy Panel")
        ItemTree.Nodes.Add("item483425306", "White Grade 5 Titanium Alloy Panel")
        ItemTree.Nodes.Add("item483425306", "Painted Red Grade 5 Titanium Alloy")
        ItemTree.Nodes.Add("item483425306", "Yellow Grade 5 Titanium Alloy Panel")
        ItemTree.Nodes.Add("item483425306", "Glossy Grade 5 Titanium Alloy")
        ItemTree.Nodes.Add("item483425306", "Matte Grade 5 Titanium Alloy")
        ItemTree.Nodes.Add("item483425306", "Painted Black Grade 5 Titanium Alloy")
        ItemTree.Nodes.Add("item483425306", "Painted Dark Gray Grade 5 Titanium Alloy")
        ItemTree.Nodes.Add("item483425306", "Painted Gray Grade 5 Titanium Alloy")
        ItemTree.Nodes.Add("item483425306", "Painted Green Grade 5 Titanium Alloy")
        ItemTree.Nodes.Add("item483425306", "Painted Ice Grade 5 Titanium Alloy")
        ItemTree.Nodes.Add("item483425306", "Painted Light Gray Grade 5 Titanium Alloy")
        ItemTree.Nodes.Add("item483425306", "Painted Military Grade 5 Titanium Alloy")
        ItemTree.Nodes.Add("item483425306", "Painted Orange Grade 5 Titanium Alloy")
        ItemTree.Nodes.Add("item483425306", "Aged Grade 5 Titanium Alloy")
        ItemTree.Nodes.Add("item1972837708", "Gray Inconel Panel")
        ItemTree.Nodes.Add("item1972837708", "Ice Inconel Panel")
        ItemTree.Nodes.Add("item3573936284", "Gray Mangalloy Panel")
        ItemTree.Nodes.Add("item3573936284", "Green Mangalloy Panel")
        ItemTree.Nodes.Add("item3573936284", "Ice Mangalloy Panel")
        ItemTree.Nodes.Add("item3573936284", "Light Gray Mangalloy Panel")
        ItemTree.Nodes.Add("item3573936284", "Military Mangalloy Panel")
        ItemTree.Nodes.Add("item3573936284", "Orange Mangalloy Panel")
        ItemTree.Nodes.Add("item3573936284", "Purple Mangalloy Panel")
        ItemTree.Nodes.Add("item3573936284", "Red Mangalloy Panel")
        ItemTree.Nodes.Add("item3573936284", "Sky Mangalloy Panel")
        ItemTree.Nodes.Add("item3573936284", "White Mangalloy Panel")
        ItemTree.Nodes.Add("item3573936284", "Dark Gray Mangalloy Panel")
        ItemTree.Nodes.Add("item3573936284", "Yellow Mangalloy Panel")
        ItemTree.Nodes.Add("item734351314", "Glossy Maraging Steel")
        ItemTree.Nodes.Add("item734351314", "Matte Maraging Steel")
        ItemTree.Nodes.Add("item734351314", "Painted Black Maraging Steel")
        ItemTree.Nodes.Add("item734351314", "Painted Dark Gray Maraging Steel")
        ItemTree.Nodes.Add("item734351314", "Painted Gray Maraging Steel")
        ItemTree.Nodes.Add("item734351314", "Painted Green Maraging Steel")
        ItemTree.Nodes.Add("item734351314", "Painted Ice Maraging Steel")
        ItemTree.Nodes.Add("item734351314", "Painted Light Gray Maraging Steel")
        ItemTree.Nodes.Add("item734351314", "Painted Military Maraging Steel")
        ItemTree.Nodes.Add("item734351314", "Painted Orange Maraging Steel")
        ItemTree.Nodes.Add("item734351314", "Aged Maraging Steel")
        ItemTree.Nodes.Add("item1972837708", "Green Inconel Panel")
        ItemTree.Nodes.Add("item3573936284", "Black Mangalloy Panel")
        ItemTree.Nodes.Add("item3573936284", "Painted White Mangalloy")
        ItemTree.Nodes.Add("item1972837708", "Light Gray Inconel Panel")
        ItemTree.Nodes.Add("item1972837708", "Military Inconel Panel")
        ItemTree.Nodes.Add("item1972837708", "Orange Inconel Panel")
        ItemTree.Nodes.Add("item1972837708", "Purple Inconel Panel")
        ItemTree.Nodes.Add("item1972837708", "Red Inconel Panel")
        ItemTree.Nodes.Add("item1972837708", "Sky Inconel Panel")
        ItemTree.Nodes.Add("item1972837708", "White Inconel Panel")
        ItemTree.Nodes.Add("item1972837708", "Yellow Inconel Panel")
        ItemTree.Nodes.Add("item3573936284", "Aged Mangalloy")
        ItemTree.Nodes.Add("item3573936284", "Glossy Mangalloy")
        ItemTree.Nodes.Add("item3573936284", "Painted Yellow Mangalloy")
        ItemTree.Nodes.Add("item3573936284", "Matte Mangalloy")
        ItemTree.Nodes.Add("item3573936284", "Painted Dark Gray Mangalloy")
        ItemTree.Nodes.Add("item3573936284", "Painted Gray Mangalloy")
        ItemTree.Nodes.Add("item3573936284", "Painted Green Mangalloy")
        ItemTree.Nodes.Add("item3573936284", "Painted Ice Mangalloy")
        ItemTree.Nodes.Add("item3573936284", "Painted Light Gray Mangalloy")
        ItemTree.Nodes.Add("item3573936284", "Painted Military Mangalloy")
        ItemTree.Nodes.Add("item3573936284", "Painted Orange Mangalloy")
        ItemTree.Nodes.Add("item3573936284", "Painted Purple Mangalloy")
        ItemTree.Nodes.Add("item3573936284", "Painted Red Mangalloy")
        ItemTree.Nodes.Add("item3573936284", "Painted Sky Mangalloy")
        ItemTree.Nodes.Add("item3573936284", "Painted Black Mangalloy")
        ItemTree.Nodes.Add("item1160705623", "Red Sc-Al Panel")
        ItemTree.Nodes.Add("item1160705623", "Sky Sc-Al Panel")
        ItemTree.Nodes.Add("item1160705623", "White Sc-Al Panel")
        ItemTree.Nodes.Add("item1160705623", "Yellow Sc-Al Panel")
        ItemTree.Nodes.Add("item3134890135", "Aged Silumin")
        ItemTree.Nodes.Add("item3134890135", "Glossy Silumin")
        ItemTree.Nodes.Add("item3134890135", "Matte Silumin")
        ItemTree.Nodes.Add("item3134890135", "Painted Black Silumin")
        ItemTree.Nodes.Add("item3134890135", "Painted Dark Gray Silumin")
        ItemTree.Nodes.Add("item3134890135", "Painted Gray Silumin")
        ItemTree.Nodes.Add("item1160705623", "Purple Sc-Al Panel")
        ItemTree.Nodes.Add("item3134890135", "Painted Green Silumin")
        ItemTree.Nodes.Add("item3134890135", "Painted Light Gray Silumin")
        ItemTree.Nodes.Add("item3134890135", "Painted Military Silumin")
        ItemTree.Nodes.Add("item3134890135", "Painted Orange Silumin")
        ItemTree.Nodes.Add("item3134890135", "Painted Purple Silumin")
        ItemTree.Nodes.Add("item3134890135", "Painted Red Silumin")
        ItemTree.Nodes.Add("item3134890135", "Painted Sky Silumin")
        ItemTree.Nodes.Add("item3134890135", "Painted White Silumin")
        ItemTree.Nodes.Add("item3134890135", "Painted Yellow Silumin")
        ItemTree.Nodes.Add("item3134890135", "Black Silumin Panel")
        ItemTree.Nodes.Add("item3134890135", "Dark Gray Silumin Panel")
        ItemTree.Nodes.Add("item3134890135", "Painted Ice Silumin")
        ItemTree.Nodes.Add("item3134890135", "Gray Silumin Panel")
        ItemTree.Nodes.Add("item1160705623", "Orange Sc-Al Panel")
        ItemTree.Nodes.Add("item1160705623", "Light Gray Sc-Al Panel")
        ItemTree.Nodes.Add("item1160705623", "Aged Sc-Al")
        ItemTree.Nodes.Add("item1160705623", "Glossy Sc-Al")
        ItemTree.Nodes.Add("item1160705623", "Matte Sc-Al")
        ItemTree.Nodes.Add("item1160705623", "Painted Black Sc-Al")
        ItemTree.Nodes.Add("item1160705623", "Painted Dark Gray Sc-Al")
        ItemTree.Nodes.Add("item1160705623", "Painted Gray Sc-Al")
        ItemTree.Nodes.Add("item1160705623", "Painted Green Sc-Al")
        ItemTree.Nodes.Add("item1160705623", "Painted Ice Sc-Al")
        ItemTree.Nodes.Add("item1160705623", "Painted Light Gray Sc-Al")
        ItemTree.Nodes.Add("item1160705623", "Military Sc-Al Panel")
        ItemTree.Nodes.Add("item1160705623", "Painted Military Sc-Al")
        ItemTree.Nodes.Add("item1160705623", "Painted Purple Sc-Al")
        ItemTree.Nodes.Add("item1160705623", "Painted Red Sc-Al")
        ItemTree.Nodes.Add("item1160705623", "Painted Sky Sc-Al")
        ItemTree.Nodes.Add("item1160705623", "Painted White Sc-Al")
        ItemTree.Nodes.Add("item1160705623", "Painted Yellow Sc-Al")
        ItemTree.Nodes.Add("item1160705623", "Black Sc-Al Panel")
        ItemTree.Nodes.Add("item1160705623", "Dark Gray Sc-Al Panel")
        ItemTree.Nodes.Add("item1160705623", "Gray Sc-Al Panel")
        ItemTree.Nodes.Add("item1160705623", "Green Sc-Al Panel")
        ItemTree.Nodes.Add("item1160705623", "Ice Sc-Al Panel")
        ItemTree.Nodes.Add("item1160705623", "Painted Orange Sc-Al")
        ItemTree.Nodes.Add("item3134890135", "Green Silumin Panel")
        ItemTree.Nodes.Add("item3134890135", "Light Gray Silumin Panel")
        ItemTree.Nodes.Add("item3200326100", "Green Stainless Steel Panel")
        ItemTree.Nodes.Add("item3200326100", "Ice Stainless Steel Panel")
        ItemTree.Nodes.Add("item3200326100", "Light Gray Stainless Steel Panel")
        ItemTree.Nodes.Add("item3200326100", "Military Stainless Steel Panel")
        ItemTree.Nodes.Add("item3200326100", "Orange Stainless Steel Panel")
        ItemTree.Nodes.Add("item3200326100", "Purple Stainless Steel Panel")
        ItemTree.Nodes.Add("item3200326100", "Red Stainless Steel Panel")
        ItemTree.Nodes.Add("item3200326100", "Sky Stainless Steel Panel")
        ItemTree.Nodes.Add("item3200326100", "White Stainless Steel Panel")
        ItemTree.Nodes.Add("item3200326100", "Yellow Stainless Steel Panel")
        ItemTree.Nodes.Add("item3200326100", "Gray Stainless Steel Panel")
        ItemTree.Nodes.Add("item2814304696", "Painted Ice Steel")
        ItemTree.Nodes.Add("item2814304696", "Glossy Steel")
        ItemTree.Nodes.Add("item2814304696", "Matte Steel")
        ItemTree.Nodes.Add("item2814304696", "Painted Black Steel")
        ItemTree.Nodes.Add("item2814304696", "Painted Dark Gray Steel")
        ItemTree.Nodes.Add("item2814304696", "Painted Gray Steel")
        ItemTree.Nodes.Add("item2814304696", "Painted Green Steel")
        ItemTree.Nodes.Add("item2814304696", "Painted Light Gray Steel")
        ItemTree.Nodes.Add("item2814304696", "Painted Military Steel")
        ItemTree.Nodes.Add("item2814304696", "Painted Orange Steel")
        ItemTree.Nodes.Add("item2814304696", "Painted Purple Steel")
        ItemTree.Nodes.Add("item2814304696", "Aged Steel")
        ItemTree.Nodes.Add("item3134890135", "Ice Silumin Panel")
        ItemTree.Nodes.Add("item3200326100", "Dark Gray Stainless Steel Panel")
        ItemTree.Nodes.Add("item3200326100", "Painted Yellow Stainless Steel")
        ItemTree.Nodes.Add("item3134890135", "Military Silumin Panel")
        ItemTree.Nodes.Add("item3134890135", "Orange Silumin Panel")
        ItemTree.Nodes.Add("item3134890135", "Purple Silumin Panel")
        ItemTree.Nodes.Add("item3134890135", "Red Silumin Panel")
        ItemTree.Nodes.Add("item3134890135", "Sky Silumin Panel")
        ItemTree.Nodes.Add("item3134890135", "White Silumin Panel")
        ItemTree.Nodes.Add("item3134890135", "Yellow Silumin Panel")
        ItemTree.Nodes.Add("item3200326100", "Aged Stainless Steel")
        ItemTree.Nodes.Add("item3200326100", "Glossy Stainless Steel")
        ItemTree.Nodes.Add("item3200326100", "Matte Stainless Steel")
        ItemTree.Nodes.Add("item3200326100", "Black Stainless Steel Panel")
        ItemTree.Nodes.Add("item3200326100", "Painted Black Stainless Steel")
        ItemTree.Nodes.Add("item3200326100", "Painted Gray Stainless Steel")
        ItemTree.Nodes.Add("item3200326100", "Painted Green Stainless Steel")
        ItemTree.Nodes.Add("item3200326100", "Painted Ice Stainless Steel")
        ItemTree.Nodes.Add("item3200326100", "Painted Light Gray Stainless Steel")
        ItemTree.Nodes.Add("item3200326100", "Painted Military Stainless Steel")
        ItemTree.Nodes.Add("item3200326100", "Painted Orange Stainless Steel")
        ItemTree.Nodes.Add("item3200326100", "Painted Purple Stainless Steel")
        ItemTree.Nodes.Add("item3200326100", "Painted Red Stainless Steel")
        ItemTree.Nodes.Add("item3200326100", "Painted Sky Stainless Steel")
        ItemTree.Nodes.Add("item3200326100", "Painted White Stainless Steel")
        ItemTree.Nodes.Add("item3200326100", "Painted Dark Gray Stainless Steel")
        ItemTree.Nodes.Add("item1993502154", "Yellow Duralumin Panel")
        ItemTree.Nodes.Add("item1993502154", "White Duralumin Panel")
        ItemTree.Nodes.Add("item1993502154", "Sky Duralumin Panel")
        ItemTree.Nodes.Add("item1993502154", "Red Duralumin Panel")
        ItemTree.Nodes.Add("item1301142496", "Advanced Maneuver Atmospheric Engine S")
        ItemTree.Nodes.Add("item1301142497", "Rare Maneuver Atmospheric Engine S")
        ItemTree.Nodes.Add("item385121456", "Advanced Military Atmospheric Engine S")
        ItemTree.Nodes.Add("item385121459", "Rare Military Atmospheric Engine S")
        ItemTree.Nodes.Add("item3689697794", "Advanced Safe Atmospheric Engine S")
        ItemTree.Nodes.Add("item3689697821", "Rare Safe Atmospheric Engine S")
        ItemTree.Nodes.Add("item1152783535", "Rare Freight Atmospheric Engine S")
        ItemTree.Nodes.Add("item2711764150", "Rare Freight Atmospheric Engine XS")
        ItemTree.Nodes.Add("item4201522399", "Advanced Maneuver Atmospheric Engine XS")
        ItemTree.Nodes.Add("item4201522392", "Rare Maneuver Atmospheric Engine XS")
        ItemTree.Nodes.Add("item2472120802", "Advanced Military Atmospheric Engine XS")
        ItemTree.Nodes.Add("item2472120803", "Rare Military Atmospheric Engine XS")
        ItemTree.Nodes.Add("item3612851279", "Advanced Safe Atmospheric Engine XS")
        ItemTree.Nodes.Add("item3612851272", "Rare Safe Atmospheric Engine XS")
        ItemTree.Nodes.Add("item2711764151", "Advanced Freight Atmospheric Engine XS")
        ItemTree.Nodes.Add("item1152783534", "Advanced Freight Atmospheric Engine S")
        ItemTree.Nodes.Add("item1397818124", "Advanced Maneuver Atmospheric Engine L")
        ItemTree.Nodes.Add("item1397818123", "Rare Maneuver Atmospheric Engine L")
        ItemTree.Nodes.Add("item2559369183", "Advanced Military Atmospheric Engine L")
        ItemTree.Nodes.Add("item2559369176", "Rare Military Atmospheric Engine L")
        ItemTree.Nodes.Add("item3211645339", "Advanced Safe Atmospheric Engine L")
        ItemTree.Nodes.Add("item3211645332", "Rare Safe Atmospheric Engine L")
        ItemTree.Nodes.Add("item488092470", "Advanced Freight Atmospheric Engine M")
        ItemTree.Nodes.Add("item3377917825", "Advanced Maneuver Atmospheric Engine M")
        ItemTree.Nodes.Add("item3377917824", "Rare Maneuver Atmospheric Engine M")
        ItemTree.Nodes.Add("item790956353", "Advanced Military Atmospheric Engine M")
        ItemTree.Nodes.Add("item790956382", "Rare Military Atmospheric Engine M")
        ItemTree.Nodes.Add("item2370891601", "Advanced Safe Atmospheric Engine M")
        ItemTree.Nodes.Add("item2370891600", "Rare Safe Atmospheric Engine M")
        ItemTree.Nodes.Add("item488092471", "Rare Freight Atmospheric Engine M")
        ItemTree.Nodes.Add("item270403386", "Advanced Freight Space Engine S")
        ItemTree.Nodes.Add("item270403387", "Rare Freight Space Engine S")
        ItemTree.Nodes.Add("item1624640879", "Advanced Maneuver Space Engine S")
        ItemTree.Nodes.Add("item1624640872", "Rare Maneuver Space Engine S")
        ItemTree.Nodes.Add("item2510194716", "Advanced Military Space Engine S")
        ItemTree.Nodes.Add("item2510194717", "Rare Military Space Engine S")
        ItemTree.Nodes.Add("item2682344779", "Advanced Safe Space Engine S")
        ItemTree.Nodes.Add("item2497069958", "Advanced Freight Space Engine XL")
        ItemTree.Nodes.Add("item2497069959", "Rare Freight Space Engine XL")
        ItemTree.Nodes.Add("item1773467599", "Advanced Maneuver Space Engine XL")
        ItemTree.Nodes.Add("item1773467598", "Rare Maneuver Space Engine XL")
        ItemTree.Nodes.Add("item934426296", "Rare Military Space Engine XL")
        ItemTree.Nodes.Add("item2682344778", "Rare Safe Space Engine S")
        ItemTree.Nodes.Add("item1326315525", "Rare Safe Space Engine M")
        ItemTree.Nodes.Add("item2809629801", "Advanced Freight Space Engine L")
        ItemTree.Nodes.Add("item2809629798", "Rare Freight Space Engine L")
        ItemTree.Nodes.Add("item4025377657", "Advanced Maneuver Space Engine L")
        ItemTree.Nodes.Add("item4025377658", "Rare Maneuver Space Engine L")
        ItemTree.Nodes.Add("item2379018394", "Advanced Military Space Engine L")
        ItemTree.Nodes.Add("item2379018393", "Rare Military Space Engine L")
        ItemTree.Nodes.Add("item1326315524", "Advanced Safe Space Engine M")
        ItemTree.Nodes.Add("itemnil", "Advanced Safe Space Engine L")
        ItemTree.Nodes.Add("item516669710", "Advanced Freight Space Engine M")
        ItemTree.Nodes.Add("item516669711", "Rare Freight Space Engine M")
        ItemTree.Nodes.Add("item1757019469", "Advanced Maneuver Space Engine M")
        ItemTree.Nodes.Add("item1757019468", "Rare Maneuver Space Engine M")
        ItemTree.Nodes.Add("item37629188", "Advanced Military Space Engine M")
        ItemTree.Nodes.Add("item37629189", "Rare Military Space Engine M")
        ItemTree.Nodes.Add("itemnil", "Rare Safe Space Engine L")
        ItemTree.Nodes.Add("item1638517112", "Rare Freight Atmospheric Engine L")
        ItemTree.Nodes.Add("item1638517115", "Advanced Freight Atmospheric Engine L")
        ItemTree.Nodes.Add("item3478227881", "Advanced Safe Space Engine XL")
        ItemTree.Nodes.Add("item1993502154", "Painted Black Duralumin")
        ItemTree.Nodes.Add("item1993502154", "Painted Dark Gray Duralumin")
        ItemTree.Nodes.Add("item1993502154", "Painted Gray Duralumin")
        ItemTree.Nodes.Add("item1993502154", "Painted Green Duralumin")
        ItemTree.Nodes.Add("item1993502154", "Painted Ice Duralumin")
        ItemTree.Nodes.Add("item1993502154", "Painted Light Gray Duralumin")
        ItemTree.Nodes.Add("item1993502154", "Painted Military Duralumin")
        ItemTree.Nodes.Add("item1993502154", "Painted Orange Duralumin")
        ItemTree.Nodes.Add("item1993502154", "Painted Purple Duralumin")
        ItemTree.Nodes.Add("item1993502154", "Painted Red Duralumin")
        ItemTree.Nodes.Add("item1993502154", "Matte Duralumin")
        ItemTree.Nodes.Add("item1993502154", "Painted Sky Duralumin")
        ItemTree.Nodes.Add("item1993502154", "Painted Yellow Duralumin")
        ItemTree.Nodes.Add("item1993502154", "Black Duralumin Panel")
        ItemTree.Nodes.Add("item1993502154", "Dark Gray Duralumin Panel")
        ItemTree.Nodes.Add("item1993502154", "Gray Duralumin Panel")
        ItemTree.Nodes.Add("item1993502154", "Green Duralumin Panel")
        ItemTree.Nodes.Add("item1993502154", "Ice Duralumin Panel")
        ItemTree.Nodes.Add("item1993502154", "Light Gray Duralumin Panel")
        ItemTree.Nodes.Add("item1993502154", "Military Duralumin Panel")
        ItemTree.Nodes.Add("item1993502154", "Orange Duralumin Panel")
        ItemTree.Nodes.Add("item1993502154", "Purple Duralumin Panel")
        ItemTree.Nodes.Add("item1993502154", "Painted White Duralumin")
        ItemTree.Nodes.Add("item1993502154", "Glossy Duralumin")
        ItemTree.Nodes.Add("item1993502154", "Aged Duralumin")
        ItemTree.Nodes.Add("item3478227882", "Rare Safe Space Engine XL")
        ItemTree.Nodes.Add("item3719125853", "Advanced Freight Space Engine XS")
        ItemTree.Nodes.Add("item3719125852", "Rare Freight Space Engine XS")
        ItemTree.Nodes.Add("item2368501172", "Advanced Maneuver Space Engine XS")
        ItemTree.Nodes.Add("item2368501171", "Rare Maneuver Space Engine XS")
        ItemTree.Nodes.Add("item1754053134", "Advanced Military Space Engine XS")
        ItemTree.Nodes.Add("item1754053133", "Rare Military Space Engine XS")
        ItemTree.Nodes.Add("item175947629", "Advanced Safe Space Engine XS")
        ItemTree.Nodes.Add("item175947630", "Rare Safe Space Engine XS")
        ItemTree.Nodes.Add("item2906228118", "Al-Li Painted Military")
        ItemTree.Nodes.Add("item2906228118", "Al-Li Painted Orange")
        ItemTree.Nodes.Add("item2906228118", "Al-Li Painted Purple")
        ItemTree.Nodes.Add("item2906228118", "Al-Li Painted Red")
        ItemTree.Nodes.Add("item2906228118", "Al-Li Painted Sky")
        ItemTree.Nodes.Add("item2906228118", "Al-Li Painted White")
        ItemTree.Nodes.Add("item2906228118", "Al-Li Painted Yellow")
        ItemTree.Nodes.Add("item2906228118", "Black Al-Li Panel")
        ItemTree.Nodes.Add("item2906228118", "Dark Gray Al-Li Panel")
        ItemTree.Nodes.Add("item2906228118", "Gray Al-Li Panel")
        ItemTree.Nodes.Add("item2906228118", "Al-Li Painted Light Gray")
        ItemTree.Nodes.Add("item2906228118", "Green Al-Li Panel")
        ItemTree.Nodes.Add("item2906228118", "Light Gray Al-Li Panel")
        ItemTree.Nodes.Add("item2906228118", "Military Al-Li Panel")
        ItemTree.Nodes.Add("item2906228118", "Orange Al-Li Panel")
        ItemTree.Nodes.Add("item2906228118", "Purple Al-Li Panel")
        ItemTree.Nodes.Add("item2906228118", "Red Al-Li Panel")
        ItemTree.Nodes.Add("item2906228118", "Sky Al-Li Panel")
        ItemTree.Nodes.Add("item2906228118", "White Al-Li Panel")
        ItemTree.Nodes.Add("item2906228118", "Yellow Al-Li Panel")
        ItemTree.Nodes.Add("item2906228118", "Ice Al-Li Panel")
        ItemTree.Nodes.Add("item2906228118", "Al-Li Painted Ice")
        ItemTree.Nodes.Add("item2906228118", "Al-Li Painted Gray")
        ItemTree.Nodes.Add("item2906228118", "Al-Li Painted Green")
        ItemTree.Nodes.Add("item2906228118", "Aged Al-Li")
        ItemTree.Nodes.Add("item2906228118", "Glossy Al-Li")
        ItemTree.Nodes.Add("item2906228118", "Matte Al-Li")
        ItemTree.Nodes.Add("item2906228118", "Al-Li Painted Black")
        ItemTree.Nodes.Add("item2906228118", "Al-Li Painted Dark Gray")
        ItemTree.Nodes.Add("item3264314258", "Uncommon Recycler M")
        ItemTree.Nodes.Add("item584577125", "Uncommon Refiner M")
        ItemTree.Nodes.Add("item1132446360", "Uncommon Smelter M")
        ItemTree.Nodes.Add("item2793358079", "Advanced 3D Printer M")
        ItemTree.Nodes.Add("item1762226675", "Advanced Assembly Line L")
        ItemTree.Nodes.Add("item1762227888", "Advanced Assembly Line M")
        ItemTree.Nodes.Add("item1762226232", "Advanced Assembly Line S")
        ItemTree.Nodes.Add("item2480866767", "Advanced Assembly Line XL")
        ItemTree.Nodes.Add("item2480928551", "Advanced Assembly Line XS")
        ItemTree.Nodes.Add("item648743080", "Advanced Chemical Industry M")
        ItemTree.Nodes.Add("item2808015394", "Uncommon Metalwork Industry M")
        ItemTree.Nodes.Add("item2861848557", "Advanced Electronics Industry M")
        ItemTree.Nodes.Add("item3026799988", "Advanced Honeycomb Refinery M")
        ItemTree.Nodes.Add("item2808015397", "Advanced Metalwork Industry M")
        ItemTree.Nodes.Add("item3264314259", "Advanced Recycler M")
        ItemTree.Nodes.Add("item584577124", "Advanced Refiner M")
        ItemTree.Nodes.Add("item1132446361", "Advanced Smelter M")
        ItemTree.Nodes.Add("item2793358076", "Rare 3D Printer M")
        ItemTree.Nodes.Add("item1762226674", "Rare Assembly Line L")
        ItemTree.Nodes.Add("item1762227889", "Rare Assembly Line M")
        ItemTree.Nodes.Add("item1762226233", "Rare Assembly Line S")
        ItemTree.Nodes.Add("item2480866766", "Rare Assembly Line XL")
        ItemTree.Nodes.Add("item2200747731", "Advanced Glass Furnace M")
        ItemTree.Nodes.Add("item2480928544", "Rare Assembly Line XS")
        ItemTree.Nodes.Add("item3026799987", "Uncommon Honeycomb Refinery M")
        ItemTree.Nodes.Add("item2861848558", "Uncommon Electronics Industry M")
        ItemTree.Nodes.Add("item2200747728", "Uncommon Glass Furnace M")
        ItemTree.Nodes.Add("item2793358078", "Uncommon 3D Printer M")
        ItemTree.Nodes.Add("item1762226636", "Uncommon Assembly Line L")
        ItemTree.Nodes.Add("item1762227855", "Uncommon Assembly Line M")
        ItemTree.Nodes.Add("item1762226235", "Uncommon Assembly Line S")
        ItemTree.Nodes.Add("item2480866760", "Uncommon Assembly Line XL")
        ItemTree.Nodes.Add("item2480928550", "Uncommon Assembly Line XS")
        ItemTree.Nodes.Add("item648743083", "Uncommon Chemical Industry M")
        ItemTree.Nodes.Add("item648743081", "Rare Chemical Industry M")
        ItemTree.Nodes.Add("item2200747730", "Rare Glass Furnace M")
        ItemTree.Nodes.Add("item2861848556", "Rare Electronics Industry M")
        ItemTree.Nodes.Add("item3026799989", "Rare Honeycomb Refinery M")
        ItemTree.Nodes.Add("item2808015396", "Rare Metalwork Industry M")
        ItemTree.Nodes.Add("item3264314284", "Rare Recycler M")
        ItemTree.Nodes.Add("item584577123", "Rare Refiner M")
        ItemTree.Nodes.Add("item1132446358", "Rare Smelter M")
        ItemTree.Nodes.Add("item3437395596", "XOR Operator")
        ItemTree.Nodes.Add("item1839029088", "NOR Operator")
        ItemTree.Nodes.Add("item3600874516", "NAND Operator")
        ItemTree.Nodes.Add("itemnil", "Thoramine")
        ItemTree.Nodes.Add("item1268122879", "Luminescent White Glass")
        ItemTree.Nodes.Add("item774130122", "Repair Unit")
        ItemTree.Nodes.Add("item4186206037", "Uncommon Power Transformer M")
        ItemTree.Nodes.Add("item4186206035", "Uncommon Power Transformer S")
        ItemTree.Nodes.Add("item3760652609", "Painted Ice Silver")
        ItemTree.Nodes.Add("item3760652609", "Painted Light Gray Silver")
        ItemTree.Nodes.Add("item3760652609", "Painted Military Silver")
        ItemTree.Nodes.Add("item3760652609", "Painted Orange Silver")
        ItemTree.Nodes.Add("item3760652609", "Painted Purple Silver")
        ItemTree.Nodes.Add("item3760652609", "Painted Red Silver")
        ItemTree.Nodes.Add("item3760652609", "Painted Sky Silver")
        ItemTree.Nodes.Add("item3760652609", "Painted White Silver")
        ItemTree.Nodes.Add("item3760652609", "Painted Yellow Silver")
        ItemTree.Nodes.Add("item3760652609", "Black Silver Panel")
        ItemTree.Nodes.Add("item3760652609", "Dark Gray Silver Panel")
        ItemTree.Nodes.Add("item3760652609", "Gray Silver Panel")
        ItemTree.Nodes.Add("item3760652609", "Green Silver Panel")
        ItemTree.Nodes.Add("item3760652609", "Ice Silver Panel")
        ItemTree.Nodes.Add("item3760652609", "Painted Green Silver")
        ItemTree.Nodes.Add("item3760652609", "Light Gray Silver Panel")
        ItemTree.Nodes.Add("item3760652609", "Orange Silver Panel")
        ItemTree.Nodes.Add("item3760652609", "Purple Silver Panel")
        ItemTree.Nodes.Add("item3760652609", "Red Silver Panel")
        ItemTree.Nodes.Add("item3760652609", "Sky Silver Panel")
        ItemTree.Nodes.Add("item3760652609", "White Silver Panel")
        ItemTree.Nodes.Add("item3760652609", "Yellow Silver Panel")
        ItemTree.Nodes.Add("item2013004922", "Aged Sodium")
        ItemTree.Nodes.Add("item2013004922", "Glossy Sodium")
        ItemTree.Nodes.Add("item2013004922", "Matte Sodium")
        ItemTree.Nodes.Add("item2013004922", "Painted Black Sodium")
        ItemTree.Nodes.Add("item2013004922", "Painted Dark Gray Sodium")
        ItemTree.Nodes.Add("item2013004922", "Painted Gray Sodium")
        ItemTree.Nodes.Add("item2013004922", "Painted Green Sodium")
        ItemTree.Nodes.Add("item2013004922", "Painted Ice Sodium")
        ItemTree.Nodes.Add("item3760652609", "Military Silver Panel")
        ItemTree.Nodes.Add("item3760652609", "Painted Gray Silver")
        ItemTree.Nodes.Add("item3760652609", "Painted Dark Gray Silver")
        ItemTree.Nodes.Add("item3760652609", "Painted Black Silver")
        ItemTree.Nodes.Add("item4079996329", "Purple Silicon Panel")
        ItemTree.Nodes.Add("item4079996329", "Red Silicon Panel")
        ItemTree.Nodes.Add("item4079996329", "Sky Silicon Panel")
        ItemTree.Nodes.Add("item4079996329", "White Silicon Panel")
        ItemTree.Nodes.Add("item4079996329", "Yellow Silicon Panel")
        ItemTree.Nodes.Add("item4079996329", "Aged Silicon")
        ItemTree.Nodes.Add("item4079996329", "Glossy Silicon")
        ItemTree.Nodes.Add("item4079996329", "Matte Silicon")
        ItemTree.Nodes.Add("item4079996329", "Painted Black Silicon")
        ItemTree.Nodes.Add("item4079996329", "Painted Dark Gray Silicon")
        ItemTree.Nodes.Add("item4079996329", "Painted Gray Silicon")
        ItemTree.Nodes.Add("item4079996329", "Painted Green Silicon")
        ItemTree.Nodes.Add("item4079996329", "Painted Ice Silicon")
        ItemTree.Nodes.Add("item4079996329", "Painted Light Gray Silicon")
        ItemTree.Nodes.Add("item4079996329", "Painted Military Silicon")
        ItemTree.Nodes.Add("item4079996329", "Painted Orange Silicon")
        ItemTree.Nodes.Add("item4079996329", "Painted Purple Silicon")
        ItemTree.Nodes.Add("item3760652609", "Matte Silver")
        ItemTree.Nodes.Add("item3760652609", "Glossy Silver")
        ItemTree.Nodes.Add("item3760652609", "Aged Silver")
        ItemTree.Nodes.Add("item4079996329", "Military Silicon Panel")
        ItemTree.Nodes.Add("item4079996329", "Light Gray Silicon Panel")
        ItemTree.Nodes.Add("item4079996329", "Ice Silicon Panel")
        ItemTree.Nodes.Add("item2013004922", "Painted Light Gray Sodium")
        ItemTree.Nodes.Add("item4079996329", "Green Silicon Panel")
        ItemTree.Nodes.Add("item4079996329", "Dark Gray Silicon Panel")
        ItemTree.Nodes.Add("item4079996329", "Black Silicon Panel")
        ItemTree.Nodes.Add("item4079996329", "Painted Yellow Silicon")
        ItemTree.Nodes.Add("item4079996329", "Painted White Silicon")
        ItemTree.Nodes.Add("item4079996329", "Painted Sky Silicon")
        ItemTree.Nodes.Add("item4079996329", "Painted Red Silicon")
        ItemTree.Nodes.Add("item4079996329", "Gray Silicon Panel")
        ItemTree.Nodes.Add("item4079996329", "Orange Silicon Panel")
        ItemTree.Nodes.Add("item2013004922", "Painted Military Sodium")
        ItemTree.Nodes.Add("item2013004922", "Painted Purple Sodium")
        ItemTree.Nodes.Add("item1519873395", "Gray Sulfur Panel")
        ItemTree.Nodes.Add("item1519873395", "Green Sulfur Panel")
        ItemTree.Nodes.Add("item1519873395", "Ice Sulfur Panel")
        ItemTree.Nodes.Add("item1519873395", "Light Gray Sulfur Panel")
        ItemTree.Nodes.Add("item1519873395", "Military Sulfur Panel")
        ItemTree.Nodes.Add("item1519873395", "Orange Sulfur Panel")
        ItemTree.Nodes.Add("item1519873395", "Purple Sulfur Panel")
        ItemTree.Nodes.Add("item1519873395", "Red Sulfur Panel")
        ItemTree.Nodes.Add("item1519873395", "Sky Sulfur Panel")
        ItemTree.Nodes.Add("item1519873395", "White Sulfur Panel")
        ItemTree.Nodes.Add("item1519873395", "Yellow Sulfur Panel")
        ItemTree.Nodes.Add("item402511494", "Aged Titanium")
        ItemTree.Nodes.Add("item402511494", "Glossy Titanium")
        ItemTree.Nodes.Add("item402511494", "Matte Titanium")
        ItemTree.Nodes.Add("item1519873395", "Dark Gray Sulfur Panel")
        ItemTree.Nodes.Add("item402511494", "Painted Black Titanium")
        ItemTree.Nodes.Add("item402511494", "Painted Gray Titanium")
        ItemTree.Nodes.Add("item402511494", "Painted Green Titanium")
        ItemTree.Nodes.Add("item402511494", "Painted Ice Titanium")
        ItemTree.Nodes.Add("item402511494", "Painted Light Gray Titanium")
        ItemTree.Nodes.Add("item402511494", "Painted Military Titanium")
        ItemTree.Nodes.Add("item402511494", "Painted Orange Titanium")
        ItemTree.Nodes.Add("item402511494", "Painted Purple Titanium")
        ItemTree.Nodes.Add("item402511494", "Painted Red Titanium")
        ItemTree.Nodes.Add("item402511494", "Painted Sky Titanium")
        ItemTree.Nodes.Add("item402511494", "Painted White Titanium")
        ItemTree.Nodes.Add("item402511494", "Painted Yellow Titanium")
        ItemTree.Nodes.Add("item402511494", "Black Titanium Panel")
        ItemTree.Nodes.Add("item402511494", "Dark Gray Titanium Panel")
        ItemTree.Nodes.Add("item402511494", "Gray Titanium Panel")
        ItemTree.Nodes.Add("item402511494", "Painted Dark Gray Titanium")
        ItemTree.Nodes.Add("item1519873395", "Black Sulfur Panel")
        ItemTree.Nodes.Add("item1519873395", "Painted Yellow Sulfur")
        ItemTree.Nodes.Add("item1519873395", "Painted White Sulfur")
        ItemTree.Nodes.Add("item2013004922", "Painted Red Sodium")
        ItemTree.Nodes.Add("item2013004922", "Painted Sky Sodium")
        ItemTree.Nodes.Add("item2013004922", "Painted White Sodium")
        ItemTree.Nodes.Add("item2013004922", "Painted Yellow Sodium")
        ItemTree.Nodes.Add("item2013004922", "Black Sodium Panel")
        ItemTree.Nodes.Add("item2013004922", "Dark Gray Sodium Panel")
        ItemTree.Nodes.Add("item2013004922", "Gray Sodium Panel")
        ItemTree.Nodes.Add("item2013004922", "Green Sodium Panel")
        ItemTree.Nodes.Add("item2013004922", "Ice Sodium Panel")
        ItemTree.Nodes.Add("item2013004922", "Light Gray Sodium Panel")
        ItemTree.Nodes.Add("item2013004922", "Military Sodium Panel")
        ItemTree.Nodes.Add("item2013004922", "Orange Sodium Panel")
        ItemTree.Nodes.Add("item2013004922", "Purple Sodium Panel")
        ItemTree.Nodes.Add("item2013004922", "Red Sodium Panel")
        ItemTree.Nodes.Add("item2013004922", "Sky Sodium Panel")
        ItemTree.Nodes.Add("item2013004922", "White Sodium Panel")
        ItemTree.Nodes.Add("item2013004922", "Yellow Sodium Panel")
        ItemTree.Nodes.Add("item1519873395", "Painted Sky Sulfur")
        ItemTree.Nodes.Add("item1519873395", "Painted Red Sulfur")
        ItemTree.Nodes.Add("item1519873395", "Painted Purple Sulfur")
        ItemTree.Nodes.Add("item1519873395", "Painted Orange Sulfur")
        ItemTree.Nodes.Add("item1519873395", "Painted Military Sulfur")
        ItemTree.Nodes.Add("item1519873395", "Painted Light Gray Sulfur")
        ItemTree.Nodes.Add("item2013004922", "Painted Orange Sodium")
        ItemTree.Nodes.Add("item1519873395", "Painted Ice Sulfur")
        ItemTree.Nodes.Add("item1519873395", "Painted Gray Sulfur")
        ItemTree.Nodes.Add("item1519873395", "Painted Dark Gray Sulfur")
        ItemTree.Nodes.Add("item1519873395", "Painted Black Sulfur")
        ItemTree.Nodes.Add("item1519873395", "Matte Sulfur")
        ItemTree.Nodes.Add("item1519873395", "Glossy Sulfur")
        ItemTree.Nodes.Add("item1519873395", "Aged Sulfur")
        ItemTree.Nodes.Add("item1519873395", "Painted Green Sulfur")
        ItemTree.Nodes.Add("item402511494", "Green Titanium Panel")
        ItemTree.Nodes.Add("item2980173742", "Yellow Scandium Panel")
        ItemTree.Nodes.Add("item2980173742", "Sky Scandium Panel")
        ItemTree.Nodes.Add("item3522164802", "Painted White Manganese")
        ItemTree.Nodes.Add("item3522164802", "Painted Yellow Manganese")
        ItemTree.Nodes.Add("item3522164802", "Black Manganese Panel")
        ItemTree.Nodes.Add("item3522164802", "Dark Gray Manganese Panel")
        ItemTree.Nodes.Add("item3522164802", "Gray Manganese Panel")
        ItemTree.Nodes.Add("item3522164802", "Green Manganese Panel")
        ItemTree.Nodes.Add("item3522164802", "Ice Manganese Panel")
        ItemTree.Nodes.Add("item3522164802", "Light Gray Manganese Panel")
        ItemTree.Nodes.Add("item3522164802", "Military Manganese Panel")
        ItemTree.Nodes.Add("item3522164802", "Orange Manganese Panel")
        ItemTree.Nodes.Add("item3522164802", "Purple Manganese Panel")
        ItemTree.Nodes.Add("item3522164802", "Red Manganese Panel")
        ItemTree.Nodes.Add("item3522164802", "Sky Manganese Panel")
        ItemTree.Nodes.Add("item3522164802", "White Manganese Panel")
        ItemTree.Nodes.Add("item3522164802", "Painted Sky Manganese")
        ItemTree.Nodes.Add("item3522164802", "Yellow Manganese Panel")
        ItemTree.Nodes.Add("item1194276464", "Glossy Nickel")
        ItemTree.Nodes.Add("item1194276464", "Matte Nickel")
        ItemTree.Nodes.Add("item1194276464", "Painted Black Nickel")
        ItemTree.Nodes.Add("item1194276464", "Painted Dark Gray Nickel")
        ItemTree.Nodes.Add("item1194276464", "Painted Gray Nickel")
        ItemTree.Nodes.Add("item1194276464", "Painted Green Nickel")
        ItemTree.Nodes.Add("item1194276464", "Painted Ice Nickel")
        ItemTree.Nodes.Add("item1194276464", "Painted Light Gray Nickel")
        ItemTree.Nodes.Add("item1194276464", "Painted Military Nickel")
        ItemTree.Nodes.Add("item1194276464", "Painted Orange Nickel")
        ItemTree.Nodes.Add("item1194276464", "Painted Purple Nickel")
        ItemTree.Nodes.Add("item1194276464", "Painted Red Nickel")
        ItemTree.Nodes.Add("item1194276464", "Painted Sky Nickel")
        ItemTree.Nodes.Add("item1194276464", "Painted White Nickel")
        ItemTree.Nodes.Add("item1194276464", "Aged Nickel")
        ItemTree.Nodes.Add("item3522164802", "Painted Red Manganese")
        ItemTree.Nodes.Add("item3522164802", "Painted Purple Manganese")
        ItemTree.Nodes.Add("item3522164802", "Painted Orange Manganese")
        ItemTree.Nodes.Add("item1987555115", "Painted Light Gray Lithium")
        ItemTree.Nodes.Add("item1987555115", "Painted Military Lithium")
        ItemTree.Nodes.Add("item1987555115", "Painted Orange Lithium")
        ItemTree.Nodes.Add("item1987555115", "Painted Purple Lithium")
        ItemTree.Nodes.Add("item1987555115", "Painted Red Lithium")
        ItemTree.Nodes.Add("item1987555115", "Painted Sky Lithium")
        ItemTree.Nodes.Add("item1987555115", "Painted White Lithium")
        ItemTree.Nodes.Add("item1987555115", "Painted Yellow Lithium")
        ItemTree.Nodes.Add("item1987555115", "Black Lithium Panel")
        ItemTree.Nodes.Add("item1987555115", "Dark Gray Lithium Panel")
        ItemTree.Nodes.Add("item1987555115", "Gray Lithium Panel")
        ItemTree.Nodes.Add("item1987555115", "Green Lithium Panel")
        ItemTree.Nodes.Add("item1987555115", "Ice Lithium Panel")
        ItemTree.Nodes.Add("item1987555115", "Light Gray Lithium Panel")
        ItemTree.Nodes.Add("item1987555115", "Military Lithium Panel")
        ItemTree.Nodes.Add("item1987555115", "Orange Lithium Panel")
        ItemTree.Nodes.Add("item1987555115", "Purple Lithium Panel")
        ItemTree.Nodes.Add("item3522164802", "Painted Military Manganese")
        ItemTree.Nodes.Add("item3522164802", "Painted Light Gray Manganese")
        ItemTree.Nodes.Add("item3522164802", "Painted Ice Manganese")
        ItemTree.Nodes.Add("item3522164802", "Painted Green Manganese")
        ItemTree.Nodes.Add("item3522164802", "Painted Gray Manganese")
        ItemTree.Nodes.Add("item3522164802", "Painted Dark Gray Manganese")
        ItemTree.Nodes.Add("item1194276464", "Painted Yellow Nickel")
        ItemTree.Nodes.Add("item3522164802", "Painted Black Manganese")
        ItemTree.Nodes.Add("item3522164802", "Glossy Manganese")
        ItemTree.Nodes.Add("item3522164802", "Aged Manganese")
        ItemTree.Nodes.Add("item1987555115", "Yellow Lithium Panel")
        ItemTree.Nodes.Add("item1987555115", "White Lithium Panel")
        ItemTree.Nodes.Add("item1987555115", "Sky Lithium Panel")
        ItemTree.Nodes.Add("item1987555115", "Red Lithium Panel")
        ItemTree.Nodes.Add("item3522164802", "Matte Manganese")
        ItemTree.Nodes.Add("item2980173742", "White Scandium Panel")
        ItemTree.Nodes.Add("item1194276464", "Black Nickel Panel")
        ItemTree.Nodes.Add("item1194276464", "Gray Nickel Panel")
        ItemTree.Nodes.Add("item30546913", "Red Niobium Panel")
        ItemTree.Nodes.Add("item30546913", "Sky Niobium Panel")
        ItemTree.Nodes.Add("item30546913", "White Niobium Panel")
        ItemTree.Nodes.Add("item30546913", "Yellow Niobium Panel")
        ItemTree.Nodes.Add("item2980173742", "Aged Scandium")
        ItemTree.Nodes.Add("item2980173742", "Glossy Scandium")
        ItemTree.Nodes.Add("item2980173742", "Matte Scandium")
        ItemTree.Nodes.Add("item2980173742", "Painted Black Scandium")
        ItemTree.Nodes.Add("item2980173742", "Painted Dark Gray Scandium")
        ItemTree.Nodes.Add("item2980173742", "Painted Gray Scandium")
        ItemTree.Nodes.Add("item2980173742", "Painted Green Scandium")
        ItemTree.Nodes.Add("item2980173742", "Painted Ice Scandium")
        ItemTree.Nodes.Add("item2980173742", "Painted Light Gray Scandium")
        ItemTree.Nodes.Add("item2980173742", "Painted Military Scandium")
        ItemTree.Nodes.Add("item30546913", "Purple Niobium Panel")
        ItemTree.Nodes.Add("item2980173742", "Painted Orange Scandium")
        ItemTree.Nodes.Add("item2980173742", "Painted Red Scandium")
        ItemTree.Nodes.Add("item2980173742", "Painted Sky Scandium")
        ItemTree.Nodes.Add("item2980173742", "Painted White Scandium")
        ItemTree.Nodes.Add("item2980173742", "Painted Yellow Scandium")
        ItemTree.Nodes.Add("item2980173742", "Black Scandium Panel")
        ItemTree.Nodes.Add("item2980173742", "Dark Gray Scandium Panel")
        ItemTree.Nodes.Add("item2980173742", "Gray Scandium Panel")
        ItemTree.Nodes.Add("item2980173742", "Green Scandium Panel")
        ItemTree.Nodes.Add("item2980173742", "Ice Scandium Panel")
        ItemTree.Nodes.Add("item2980173742", "Light Gray Scandium Panel")
        ItemTree.Nodes.Add("item2980173742", "Military Scandium Panel")
        ItemTree.Nodes.Add("item2980173742", "Orange Scandium Panel")
        ItemTree.Nodes.Add("item2980173742", "Purple Scandium Panel")
        ItemTree.Nodes.Add("item2980173742", "Red Scandium Panel")
        ItemTree.Nodes.Add("item2980173742", "Painted Purple Scandium")
        ItemTree.Nodes.Add("item30546913", "Orange Niobium Panel")
        ItemTree.Nodes.Add("item30546913", "Military Niobium Panel")
        ItemTree.Nodes.Add("item30546913", "Light Gray Niobium Panel")
        ItemTree.Nodes.Add("item1194276464", "Green Nickel Panel")
        ItemTree.Nodes.Add("item1194276464", "Ice Nickel Panel")
        ItemTree.Nodes.Add("item1194276464", "Light Gray Nickel Panel")
        ItemTree.Nodes.Add("item1194276464", "Military Nickel Panel")
        ItemTree.Nodes.Add("item1194276464", "Orange Nickel Panel")
        ItemTree.Nodes.Add("item1194276464", "Purple Nickel Panel")
        ItemTree.Nodes.Add("item1194276464", "Red Nickel Panel")
        ItemTree.Nodes.Add("item1194276464", "Sky Nickel Panel")
        ItemTree.Nodes.Add("item1194276464", "White Nickel Panel")
        ItemTree.Nodes.Add("item1194276464", "Yellow Nickel Panel")
        ItemTree.Nodes.Add("item30546913", "Aged Niobium")
        ItemTree.Nodes.Add("item30546913", "Glossy Niobium")
        ItemTree.Nodes.Add("item30546913", "Matte Niobium")
        ItemTree.Nodes.Add("item30546913", "Painted Black Niobium")
        ItemTree.Nodes.Add("item30546913", "Painted Dark Gray Niobium")
        ItemTree.Nodes.Add("item30546913", "Painted Gray Niobium")
        ItemTree.Nodes.Add("item30546913", "Painted Green Niobium")
        ItemTree.Nodes.Add("item30546913", "Ice Niobium Panel")
        ItemTree.Nodes.Add("item30546913", "Green Niobium Panel")
        ItemTree.Nodes.Add("item30546913", "Gray Niobium Panel")
        ItemTree.Nodes.Add("item30546913", "Dark Gray Niobium Panel")
        ItemTree.Nodes.Add("item30546913", "Black Niobium Panel")
        ItemTree.Nodes.Add("item30546913", "Painted Yellow Niobium")
        ItemTree.Nodes.Add("item1194276464", "Dark Gray Nickel Panel")
        ItemTree.Nodes.Add("item30546913", "Painted White Niobium")
        ItemTree.Nodes.Add("item30546913", "Painted Red Niobium")
        ItemTree.Nodes.Add("item30546913", "Painted Purple Niobium")
        ItemTree.Nodes.Add("item30546913", "Painted Orange Niobium")
        ItemTree.Nodes.Add("item30546913", "Painted Military Niobium")
        ItemTree.Nodes.Add("item30546913", "Painted Light Gray Niobium")
        ItemTree.Nodes.Add("item30546913", "Painted Ice Niobium")
        ItemTree.Nodes.Add("item30546913", "Painted Sky Niobium")
        ItemTree.Nodes.Add("item1987555115", "Painted Ice Lithium")
        ItemTree.Nodes.Add("item402511494", "Ice Titanium Panel")
        ItemTree.Nodes.Add("item402511494", "Military Titanium Panel")
        ItemTree.Nodes.Add("itemnil", "Basic Structural Parts")
        ItemTree.Nodes.Add("itemnil", "Uncommon Structural Parts")
        ItemTree.Nodes.Add("itemnil", "Exotic Structural Parts")
        ItemTree.Nodes.Add("itemnil", "Rare Structural Parts")
        ItemTree.Nodes.Add("itemnil", "Advanced Structural Parts")
        ItemTree.Nodes.Add("itemnil", "Basic Structural Parts")
        ItemTree.Nodes.Add("item402511494", "Light Gray Titanium Panel")
        ItemTree.Nodes.Add("item1605580774", "Painted White Vanadium")
        ItemTree.Nodes.Add("item1605580774", "Painted Yellow Vanadium")
        ItemTree.Nodes.Add("item1605580774", "Black Vanadium Panel")
        ItemTree.Nodes.Add("item1605580774", "Dark Gray Vanadium Panel")
        ItemTree.Nodes.Add("item1605580774", "Gray Vanadium Panel")
        ItemTree.Nodes.Add("item1605580774", "Green Vanadium Panel")
        ItemTree.Nodes.Add("item1605580774", "Ice Vanadium Panel")
        ItemTree.Nodes.Add("item1605580774", "Light Gray Vanadium Panel")
        ItemTree.Nodes.Add("item1605580774", "Military Vanadium Panel")
        ItemTree.Nodes.Add("item1605580774", "Orange Vanadium Panel")
        ItemTree.Nodes.Add("item1605580774", "Purple Vanadium Panel")
        ItemTree.Nodes.Add("item1605580774", "Red Vanadium Panel")
        ItemTree.Nodes.Add("item1605580774", "Sky Vanadium Panel")
        ItemTree.Nodes.Add("item1605580774", "White Vanadium Panel")
        ItemTree.Nodes.Add("item1605580774", "Painted Sky Vanadium")
        ItemTree.Nodes.Add("item1605580774", "Yellow Vanadium Panel")
        ItemTree.Nodes.Add("item1605580774", "Painted Red Vanadium")
        ItemTree.Nodes.Add("item1605580774", "Painted Purple Vanadium")
        ItemTree.Nodes.Add("item1605580774", "Painted Orange Vanadium")
        ItemTree.Nodes.Add("item402511494", "Orange Titanium Panel")
        ItemTree.Nodes.Add("item402511494", "Purple Titanium Panel")
        ItemTree.Nodes.Add("item402511494", "Red Titanium Panel")
        ItemTree.Nodes.Add("item402511494", "Sky Titanium Panel")
        ItemTree.Nodes.Add("item402511494", "White Titanium Panel")
        ItemTree.Nodes.Add("item402511494", "Yellow Titanium Panel")
        ItemTree.Nodes.Add("item1605580774", "Painted Military Vanadium")
        ItemTree.Nodes.Add("item1605580774", "Painted Light Gray Vanadium")
        ItemTree.Nodes.Add("item1605580774", "Painted Ice Vanadium")
        ItemTree.Nodes.Add("item1605580774", "Painted Green Vanadium")
        ItemTree.Nodes.Add("item1605580774", "Painted Gray Vanadium")
        ItemTree.Nodes.Add("item1605580774", "Painted Dark Gray Vanadium")
        ItemTree.Nodes.Add("item1605580774", "Painted Black Vanadium")
        ItemTree.Nodes.Add("item1605580774", "Glossy Vanadium")
        ItemTree.Nodes.Add("item1605580774", "Aged Vanadium")
        ItemTree.Nodes.Add("item1605580774", "Matte Vanadium")
        ItemTree.Nodes.Add("item1339253011", "Warp Cell")
        ItemTree.Nodes.Add("item1987555115", "Painted Green Lithium")
        ItemTree.Nodes.Add("item1987555115", "Painted Gray Lithium")
        ItemTree.Nodes.Add("item1987555115", "Painted Dark Gray Lithium")
        ItemTree.Nodes.Add("item123493466", "Aged Aluminium")
        ItemTree.Nodes.Add("item123493466", "Glossy Aluminium")
        ItemTree.Nodes.Add("item123493466", "Matte Aluminium")
        ItemTree.Nodes.Add("item123493466", "Painted Black Aluminium")
        ItemTree.Nodes.Add("item123493466", "Painted Dark Gray Aluminium")
        ItemTree.Nodes.Add("item123493466", "Painted Gray Aluminium")
        ItemTree.Nodes.Add("item2643443936", "Warp Drive L")
        ItemTree.Nodes.Add("item764397251", "Fuel Intake XS")
        ItemTree.Nodes.Add("item297147615", "Hatch S")
        ItemTree.Nodes.Add("item273900142", "Uncommon Freight Space Engine L")
        ItemTree.Nodes.Add("item613453124", "Uncommon Maneuver Space Engine L")
        ItemTree.Nodes.Add("item2637003463", "Uncommon Military Space Engine L")
        ItemTree.Nodes.Add("item892904533", "Uncommon Safe Space Engine L")
        ItemTree.Nodes.Add("item99470466", "Uncommon Freight Space Engine M")
        ItemTree.Nodes.Add("item3024541675", "Uncommon Maneuver Space Engine M")
        ItemTree.Nodes.Add("item3897078752", "Uncommon Military Space Engine M")
        ItemTree.Nodes.Add("item2489350112", "Uncommon Safe Space Engine M")
        ItemTree.Nodes.Add("item887167900", "Uncommon Safe Atmospheric Engine XS")
        ItemTree.Nodes.Add("item3764949976", "Uncommon Freight Space Engine S")
        ItemTree.Nodes.Add("item529520576", "Uncommon Military Space Engine S")
        ItemTree.Nodes.Add("item2090364569", "Uncommon Safe Space Engine S")
        ItemTree.Nodes.Add("item130796680", "Uncommon Freight Space Engine XL")
        ItemTree.Nodes.Add("item1237158531", "Uncommon Maneuver Space Engine XL")
        ItemTree.Nodes.Add("item701947611", "Uncommon Military Space Engine XL")
        ItemTree.Nodes.Add("item3846850308", "Uncommon Safe Space Engine XL")
        ItemTree.Nodes.Add("item16482091", "Uncommon Freight Space Engine XS")
        ItemTree.Nodes.Add("item1213509759", "Uncommon Maneuver Space Engine XS")
        ItemTree.Nodes.Add("item1971700279", "Uncommon Military Space Engine XS")
        ItemTree.Nodes.Add("item3083225012", "Uncommon Safe Space Engine XS")
        ItemTree.Nodes.Add("item2468029849", "Warp Beacon XL")
        ItemTree.Nodes.Add("item1171610140", "Uncommon Maneuver Space Engine S")
        ItemTree.Nodes.Add("item676012472", "Uncommon Military Atmospheric Engine XS")
        ItemTree.Nodes.Add("item1933133404", "Uncommon Maneuver Atmospheric Engine XS")
        ItemTree.Nodes.Add("item3174850377", "Uncommon Freight Atmospheric Engine XS")
        ItemTree.Nodes.Add("item3667785070", "Surrogate Pod Station")
        ItemTree.Nodes.Add("item2093838343", "Surrogate VR Station")
        ItemTree.Nodes.Add("item1053170502", "Uncommon Freight Atmospheric Engine L")
        ItemTree.Nodes.Add("item1679964557", "Uncommon Safe Atmospheric Engine S")
        ItemTree.Nodes.Add("item2203746213", "Uncommon Military Atmospheric Engine S")
        ItemTree.Nodes.Add("item317861818", "Uncommon Maneuver Atmospheric Engine S")
        ItemTree.Nodes.Add("item1503780712", "Uncommon Freight Atmospheric Engine S")
        ItemTree.Nodes.Add("item260237137", "Uncommon Safe Atmospheric Engine M")
        ItemTree.Nodes.Add("item3847351355", "Uncommon Maneuver Atmospheric Engine M")
        ItemTree.Nodes.Add("item230429858", "Uncommon Freight Atmospheric Engine M")
        ItemTree.Nodes.Add("item2510112556", "Uncommon Safe Atmospheric Engine L")
        ItemTree.Nodes.Add("item2714399324", "Uncommon Military Atmospheric Engine L")
        ItemTree.Nodes.Add("item3475626911", "Uncommon Maneuver Atmospheric Engine L")
        ItemTree.Nodes.Add("item3295665550", "Uncommon Military Atmospheric Engine M")
        ItemTree.Nodes.Add("item123493466", "Painted Green Aluminium")
        ItemTree.Nodes.Add("item123493466", "Painted Ice Aluminium")
        ItemTree.Nodes.Add("item123493466", "Painted Light Gray Aluminium")
        ItemTree.Nodes.Add("item1440099000", "Military Fluorine Panel")
        ItemTree.Nodes.Add("item1440099000", "Orange Fluorine Panel")
        ItemTree.Nodes.Add("item1440099000", "Purple Fluorine Panel")
        ItemTree.Nodes.Add("item1440099000", "Red Fluorine Panel")
        ItemTree.Nodes.Add("item1440099000", "Sky Fluorine Panel")
        ItemTree.Nodes.Add("item1440099000", "White Fluorine Panel")
        ItemTree.Nodes.Add("item1440099000", "Yellow Fluorine Panel")
        ItemTree.Nodes.Add("item1440099000", "Light Gray Fluorine Panel")
        ItemTree.Nodes.Add("item2892111312", "Aged Gold")
        ItemTree.Nodes.Add("item2892111312", "Glossy Gold")
        ItemTree.Nodes.Add("item2892111312", "Matte Gold")
        ItemTree.Nodes.Add("item2892111312", "Painted Black Gold")
        ItemTree.Nodes.Add("item2892111312", "Painted Dark Gray Gold")
        ItemTree.Nodes.Add("item2892111312", "Painted Gray Gold")
        ItemTree.Nodes.Add("item2892111312", "Painted Green Gold")
        ItemTree.Nodes.Add("item2892111312", "Painted Ice Gold")
        ItemTree.Nodes.Add("item1440099000", "Ice Fluorine Panel")
        ItemTree.Nodes.Add("item1440099000", "Green Fluorine Panel")
        ItemTree.Nodes.Add("item1440099000", "Gray Fluorine Panel")
        ItemTree.Nodes.Add("item1374916603", "Black Copper Panel")
        ItemTree.Nodes.Add("item1374916603", "Dark Gray Copper Panel")
        ItemTree.Nodes.Add("item1374916603", "Gray Copper Panel")
        ItemTree.Nodes.Add("item1374916603", "Green Copper Panel")
        ItemTree.Nodes.Add("item1374916603", "Ice Copper Panel")
        ItemTree.Nodes.Add("item1374916603", "Light Gray Copper Panel")
        ItemTree.Nodes.Add("item1374916603", "Military Copper Panel")
        ItemTree.Nodes.Add("item1374916603", "Orange Copper Panel")
        ItemTree.Nodes.Add("item1374916603", "Purple Copper Panel")
        ItemTree.Nodes.Add("item1374916603", "Red Copper Panel")
        ItemTree.Nodes.Add("item1374916603", "Sky Copper Panel")
        ItemTree.Nodes.Add("item1374916603", "White Copper Panel")
        ItemTree.Nodes.Add("item1374916603", "Yellow Copper Panel")
        ItemTree.Nodes.Add("item1440099000", "Aged Fluorine")
        ItemTree.Nodes.Add("item1440099000", "Glossy Fluorine")
        ItemTree.Nodes.Add("item1440099000", "Matte Fluorine")
        ItemTree.Nodes.Add("item1440099000", "Painted Black Fluorine")
        ItemTree.Nodes.Add("item1440099000", "Dark Gray Fluorine Panel")
        ItemTree.Nodes.Add("item1440099000", "Black Fluorine Panel")
        ItemTree.Nodes.Add("item1440099000", "Painted Yellow Fluorine")
        ItemTree.Nodes.Add("item1440099000", "Painted White Fluorine")
        ItemTree.Nodes.Add("item1440099000", "Painted Sky Fluorine")
        ItemTree.Nodes.Add("item1440099000", "Painted Red Fluorine")
        ItemTree.Nodes.Add("item2892111312", "Painted Light Gray Gold")
        ItemTree.Nodes.Add("item1440099000", "Painted Purple Fluorine")
        ItemTree.Nodes.Add("item1440099000", "Painted Military Fluorine")
        ItemTree.Nodes.Add("item1440099000", "Painted Light Gray Fluorine")
        ItemTree.Nodes.Add("item1440099000", "Painted Ice Fluorine")
        ItemTree.Nodes.Add("item1440099000", "Painted Green Fluorine")
        ItemTree.Nodes.Add("item1440099000", "Painted Gray Fluorine")
        ItemTree.Nodes.Add("item1440099000", "Painted Dark Gray Fluorine")
        ItemTree.Nodes.Add("item1440099000", "Painted Orange Fluorine")
        ItemTree.Nodes.Add("item1374916603", "Painted Yellow Copper")
        ItemTree.Nodes.Add("item2892111312", "Painted Military Gold")
        ItemTree.Nodes.Add("item2892111312", "Painted Purple Gold")
        ItemTree.Nodes.Add("item2085561075", "Dark Gray Iron Panel")
        ItemTree.Nodes.Add("item2085561075", "Gray Iron Panel")
        ItemTree.Nodes.Add("item2085561075", "Green Iron Panel")
        ItemTree.Nodes.Add("item2085561075", "Ice Iron Panel")
        ItemTree.Nodes.Add("item2085561075", "Light Gray Iron Panel")
        ItemTree.Nodes.Add("item2085561075", "Military Iron Panel")
        ItemTree.Nodes.Add("item2085561075", "Orange Iron Panel")
        ItemTree.Nodes.Add("item2085561075", "Purple Iron Panel")
        ItemTree.Nodes.Add("item2085561075", "Red Iron Panel")
        ItemTree.Nodes.Add("item2085561075", "Sky Iron Panel")
        ItemTree.Nodes.Add("item2085561075", "White Iron Panel")
        ItemTree.Nodes.Add("item2085561075", "Yellow Iron Panel")
        ItemTree.Nodes.Add("item2085561075", "Black Iron Panel")
        ItemTree.Nodes.Add("item1987555115", "Aged Lithium")
        ItemTree.Nodes.Add("item1987555115", "Glossy Lithium")
        ItemTree.Nodes.Add("item1987555115", "Matte Lithium")
        ItemTree.Nodes.Add("item1987555115", "Painted Black Lithium")
        ItemTree.Nodes.Add("item2085561075", "Painted Yellow Iron")
        ItemTree.Nodes.Add("item2085561075", "Painted White Iron")
        ItemTree.Nodes.Add("item2085561075", "Painted Sky Iron")
        ItemTree.Nodes.Add("item2892111312", "Painted Red Gold")
        ItemTree.Nodes.Add("item2892111312", "Painted Sky Gold")
        ItemTree.Nodes.Add("item2892111312", "Painted White Gold")
        ItemTree.Nodes.Add("item2892111312", "Painted Yellow Gold")
        ItemTree.Nodes.Add("item2892111312", "Black Gold Panel")
        ItemTree.Nodes.Add("item2892111312", "Dark Gray Gold Panel")
        ItemTree.Nodes.Add("item2892111312", "Gray Gold Panel")
        ItemTree.Nodes.Add("item2892111312", "Green Gold Panel")
        ItemTree.Nodes.Add("item2892111312", "Ice Gold Panel")
        ItemTree.Nodes.Add("item2892111312", "Light Gray Gold Panel")
        ItemTree.Nodes.Add("item2892111312", "Military Gold Panel")
        ItemTree.Nodes.Add("item2892111312", "Orange Gold Panel")
        ItemTree.Nodes.Add("item2892111312", "Purple Gold Panel")
        ItemTree.Nodes.Add("item2892111312", "Red Gold Panel")
        ItemTree.Nodes.Add("item2892111312", "Sky Gold Panel")
        ItemTree.Nodes.Add("item2892111312", "White Gold Panel")
        ItemTree.Nodes.Add("item2892111312", "Yellow Gold Panel")
        ItemTree.Nodes.Add("item2085561075", "Painted Red Iron")
        ItemTree.Nodes.Add("item2085561075", "Painted Purple Iron")
        ItemTree.Nodes.Add("item2085561075", "Painted Orange Iron")
        ItemTree.Nodes.Add("item2085561075", "Painted Military Iron")
        ItemTree.Nodes.Add("item2085561075", "Painted Light Gray Iron")
        ItemTree.Nodes.Add("item2085561075", "Painted Ice Iron")
        ItemTree.Nodes.Add("item2892111312", "Painted Orange Gold")
        ItemTree.Nodes.Add("item2085561075", "Painted Green Iron")
        ItemTree.Nodes.Add("item2085561075", "Painted Dark Gray Iron")
        ItemTree.Nodes.Add("item2085561075", "Painted Black Iron")
        ItemTree.Nodes.Add("item2085561075", "Matte Iron")
        ItemTree.Nodes.Add("item2085561075", "Glossy Iron")
        ItemTree.Nodes.Add("item2085561075", "Aged Iron")
        ItemTree.Nodes.Add("item2085561075", "Painted Gray Iron")
        ItemTree.Nodes.Add("item1374916603", "Painted White Copper")
        ItemTree.Nodes.Add("item1374916603", "Painted Sky Copper")
        ItemTree.Nodes.Add("item1374916603", "Painted Red Copper")
        ItemTree.Nodes.Add("item3628423708", "Light Gray Calcium Panel")
        ItemTree.Nodes.Add("item3628423708", "Military Calcium Panel")
        ItemTree.Nodes.Add("item3628423708", "Orange Calcium Panel")
        ItemTree.Nodes.Add("item3628423708", "Purple Calcium Panel")
        ItemTree.Nodes.Add("item3628423708", "Red Calcium Panel")
        ItemTree.Nodes.Add("item3628423708", "Sky Calcium Panel")
        ItemTree.Nodes.Add("item3628423708", "White Calcium Panel")
        ItemTree.Nodes.Add("item3628423708", "Yellow Calcium Panel")
        ItemTree.Nodes.Add("item1063775897", "Aged Carbon")
        ItemTree.Nodes.Add("item1063775897", "Matte Carbon")
        ItemTree.Nodes.Add("item1063775897", "Painted Black Carbon")
        ItemTree.Nodes.Add("item1063775897", "Painted Dark Gray Carbon")
        ItemTree.Nodes.Add("item1063775897", "Painted Gray Carbon")
        ItemTree.Nodes.Add("item1063775897", "Painted Green Carbon")
        ItemTree.Nodes.Add("item1063775897", "Painted Ice Carbon")
        ItemTree.Nodes.Add("item1063775897", "Painted Light Gray Carbon")
        ItemTree.Nodes.Add("item1063775897", "Painted Military Carbon")
        ItemTree.Nodes.Add("item1063775897", "Painted Orange Carbon")
        ItemTree.Nodes.Add("item1063775897", "Painted Purple Carbon")
        ItemTree.Nodes.Add("item1063775897", "Painted Red Carbon")
        ItemTree.Nodes.Add("item1063775897", "Painted Sky Carbon")
        ItemTree.Nodes.Add("item1063775897", "Painted White Carbon")
        ItemTree.Nodes.Add("item1063775897", "Painted Yellow Carbon")
        ItemTree.Nodes.Add("item1063775897", "Glossy Carbon")
        ItemTree.Nodes.Add("item123493466", "Painted Military Aluminium")
        ItemTree.Nodes.Add("item123493466", "Painted Orange Aluminium")
        ItemTree.Nodes.Add("item123493466", "Painted Purple Aluminium")
        ItemTree.Nodes.Add("item123493466", "Painted Red Aluminium")
        ItemTree.Nodes.Add("item123493466", "Painted Sky Aluminium")
        ItemTree.Nodes.Add("item123493466", "Painted White Aluminium")
        ItemTree.Nodes.Add("item123493466", "Painted Yellow Aluminium")
        ItemTree.Nodes.Add("item123493466", "Black Aluminium Panel")
        ItemTree.Nodes.Add("item123493466", "Dark Gray Aluminium Panel")
        ItemTree.Nodes.Add("item123493466", "Gray Aluminium Panel")
        ItemTree.Nodes.Add("item123493466", "Green Aluminium Panel")
        ItemTree.Nodes.Add("item123493466", "Ice Aluminium Panel")
        ItemTree.Nodes.Add("item123493466", "Light Gray Aluminium Panel")
        ItemTree.Nodes.Add("item123493466", "Military Aluminium Panel")
        ItemTree.Nodes.Add("item123493466", "Orange Aluminium Panel")
        ItemTree.Nodes.Add("item123493466", "Purple Aluminium Panel")
        ItemTree.Nodes.Add("item123493466", "Red Aluminium Panel")
        ItemTree.Nodes.Add("item3628423708", "Painted Light Gray Calcium")
        ItemTree.Nodes.Add("item3628423708", "Painted Green Calcium")
        ItemTree.Nodes.Add("item3628423708", "Painted Gray Calcium")
        ItemTree.Nodes.Add("item1063775897", "Black Carbon Panel")
        ItemTree.Nodes.Add("item3628423708", "Painted Dark Gray Calcium")
        ItemTree.Nodes.Add("item3628423708", "Matte Calcium")
        ItemTree.Nodes.Add("item3628423708", "Glossy Calcium")
        ItemTree.Nodes.Add("item3628423708", "Aged Calcium")
        ItemTree.Nodes.Add("item123493466", "Yellow Aluminium Panel")
        ItemTree.Nodes.Add("item123493466", "White Aluminium Panel")
        ItemTree.Nodes.Add("item123493466", "Sky Aluminium Panel")
        ItemTree.Nodes.Add("item3628423708", "Painted Black Calcium")
        ItemTree.Nodes.Add("item1063775897", "Dark Gray Carbon Panel")
        ItemTree.Nodes.Add("item1063775897", "Gray Carbon Panel")
        ItemTree.Nodes.Add("item1063775897", "Green Carbon Panel")
        ItemTree.Nodes.Add("item1374916603", "Painted Purple Copper")
        ItemTree.Nodes.Add("item1374916603", "Painted Orange Copper")
        ItemTree.Nodes.Add("item1374916603", "Painted Military Copper")
        ItemTree.Nodes.Add("item1374916603", "Painted Light Gray Copper")
        ItemTree.Nodes.Add("item1374916603", "Painted Ice Copper")
        ItemTree.Nodes.Add("item1374916603", "Painted Green Copper")
        ItemTree.Nodes.Add("item1374916603", "Painted Gray Copper")
        ItemTree.Nodes.Add("item1374916603", "Painted Black Copper")
        ItemTree.Nodes.Add("item1374916603", "Matte Copper")
        ItemTree.Nodes.Add("item1374916603", "Glossy Copper")
        ItemTree.Nodes.Add("item1374916603", "Aged Copper")
        ItemTree.Nodes.Add("item1374916603", "Painted Dark Gray Copper")
        ItemTree.Nodes.Add("itemnil", "Uncommon Structural Parts")
        ItemTree.Nodes.Add("item1063775897", "Ice Carbon Panel")
        ItemTree.Nodes.Add("item1063775897", "Light Gray Carbon Panel")
        ItemTree.Nodes.Add("item1063775897", "Military Carbon Panel")
        ItemTree.Nodes.Add("item1063775897", "Orange Carbon Panel")
        ItemTree.Nodes.Add("item1063775897", "Purple Carbon Panel")
        ItemTree.Nodes.Add("item1063775897", "Red Carbon Panel")
        ItemTree.Nodes.Add("item1063775897", "Sky Carbon Panel")
        ItemTree.Nodes.Add("item1063775897", "White Carbon Panel")
        ItemTree.Nodes.Add("item1063775897", "Yellow Carbon Panel")
        ItemTree.Nodes.Add("itemnil", "Advanced Structural Parts")
        ItemTree.Nodes.Add("item3628423708", "Ice Calcium Panel")
        ItemTree.Nodes.Add("item3628423708", "Green Calcium Panel")
        ItemTree.Nodes.Add("item3628423708", "Gray Calcium Panel")
        ItemTree.Nodes.Add("item3628423708", "Dark Gray Calcium Panel")
        ItemTree.Nodes.Add("item3628423708", "Painted Ice Calcium")
        ItemTree.Nodes.Add("item3628423708", "Black Calcium Panel")
        ItemTree.Nodes.Add("item3628423708", "Painted White Calcium")
        ItemTree.Nodes.Add("item3628423708", "Painted Sky Calcium")
        ItemTree.Nodes.Add("item3628423708", "Painted Red Calcium")
        ItemTree.Nodes.Add("item3628423708", "Painted Purple Calcium")
        ItemTree.Nodes.Add("item3628423708", "Painted Orange Calcium")
        ItemTree.Nodes.Add("item3628423708", "Painted Military Calcium")
        ItemTree.Nodes.Add("item3628423708", "Painted Yellow Calcium")
        ItemTree.Nodes.Add("item3292873120", "Painted Orange Cobalt")
        ItemTree.Nodes.Add("item3292873120", "Painted Military Cobalt")
        ItemTree.Nodes.Add("item3292873120", "Painted Light Gray Cobalt")
        ItemTree.Nodes.Add("item3292873120", "Painted Ice Cobalt")
        ItemTree.Nodes.Add("item3292873120", "Painted Green Cobalt")
        ItemTree.Nodes.Add("item3292873120", "Painted Gray Cobalt")
        ItemTree.Nodes.Add("item3292873120", "Painted Purple Cobalt")
        ItemTree.Nodes.Add("item3292873120", "Painted Dark Gray Cobalt")
        ItemTree.Nodes.Add("item3292873120", "Matte Cobalt")
        ItemTree.Nodes.Add("item3292873120", "Glossy Cobalt")
        ItemTree.Nodes.Add("item3292873120", "Aged Cobalt")
        ItemTree.Nodes.Add("item1406093224", "Yellow Chromium Panel")
        ItemTree.Nodes.Add("item1406093224", "White Chromium Panel")
        ItemTree.Nodes.Add("item1406093224", "Sky Chromium Panel")
        ItemTree.Nodes.Add("item3292873120", "Painted Black Cobalt")
        ItemTree.Nodes.Add("item1406093224", "Red Chromium Panel")
        ItemTree.Nodes.Add("item3292873120", "Painted Red Cobalt")
        ItemTree.Nodes.Add("item3292873120", "Painted White Cobalt")
        ItemTree.Nodes.Add("item3292873120", "Yellow Cobalt Panel")
        ItemTree.Nodes.Add("item3292873120", "White Cobalt Panel")
        ItemTree.Nodes.Add("item3292873120", "Sky Cobalt Panel")
        ItemTree.Nodes.Add("item3292873120", "Red Cobalt Panel")
        ItemTree.Nodes.Add("item3292873120", "Purple Cobalt Panel")
        ItemTree.Nodes.Add("item3292873120", "Orange Cobalt Panel")
        ItemTree.Nodes.Add("item3292873120", "Painted Sky Cobalt")
        ItemTree.Nodes.Add("item3292873120", "Military Cobalt Panel")
        ItemTree.Nodes.Add("item3292873120", "Ice Cobalt Panel")
        ItemTree.Nodes.Add("item3292873120", "Green Cobalt Panel")
        ItemTree.Nodes.Add("item3292873120", "Gray Cobalt Panel")
        ItemTree.Nodes.Add("item3292873120", "Dark Gray Cobalt Panel")
        ItemTree.Nodes.Add("item3292873120", "Black Cobalt Panel")
        ItemTree.Nodes.Add("item3292873120", "Painted Yellow Cobalt")
        ItemTree.Nodes.Add("item3292873120", "Light Gray Cobalt Panel")
        ItemTree.Nodes.Add("item1406093224", "Purple Chromium Panel")
        ItemTree.Nodes.Add("item1406093224", "Military Chromium Panel")
        ItemTree.Nodes.Add("item1406093224", "Painted Black Chromium")
        ItemTree.Nodes.Add("item1406093224", "Matte Chromium")
        ItemTree.Nodes.Add("item1406093224", "Glossy Chromium")
        ItemTree.Nodes.Add("item1406093224", "Aged Chromium")
        ItemTree.Nodes.Add("item1406093224", "Painted Dark Gray Chromium")
        ItemTree.Nodes.Add("item1406093224", "Orange Chromium Panel")
        ItemTree.Nodes.Add("item1406093224", "Painted Gray Chromium")
        ItemTree.Nodes.Add("item1406093224", "Painted Ice Chromium")
        ItemTree.Nodes.Add("item1406093224", "Light Gray Chromium Panel")
        ItemTree.Nodes.Add("item1406093224", "Ice Chromium Panel")
        ItemTree.Nodes.Add("item1406093224", "Green Chromium Panel")
        ItemTree.Nodes.Add("item1406093224", "Gray Chromium Panel")
        ItemTree.Nodes.Add("item1406093224", "Dark Gray Chromium Panel")
        ItemTree.Nodes.Add("item1406093224", "Black Chromium Panel")
        ItemTree.Nodes.Add("item1406093224", "Painted Green Chromium")
        ItemTree.Nodes.Add("item1406093224", "Painted Yellow Chromium")
        ItemTree.Nodes.Add("item1406093224", "Painted Sky Chromium")
        ItemTree.Nodes.Add("item1406093224", "Painted Red Chromium")
        ItemTree.Nodes.Add("item1406093224", "Painted Purple Chromium")
        ItemTree.Nodes.Add("item1406093224", "Painted Orange Chromium")
        ItemTree.Nodes.Add("item1406093224", "Painted Military Chromium")
        ItemTree.Nodes.Add("item1406093224", "Painted Light Gray Chromium")
        ItemTree.Nodes.Add("item1406093224", "Painted White Chromium")
        ItemTree.Nodes.Add("itemnil", "Rare Structural Parts")
        ItemTree.Nodes.Add("itemnil", "Exotic Structural Parts")
        ItemTree.Nodes.Add("item873614065", "Exotic Standard Frame XS")
        ItemTree.Nodes.Add("item1981363926", "Exotic Standard Frame S")
        ItemTree.Nodes.Add("item1981363756", "Exotic Standard Frame M")
        ItemTree.Nodes.Add("item1981363757", "Exotic Standard Frame L")
        ItemTree.Nodes.Add("item1981362581", "Rare Standard Frame S")
        ItemTree.Nodes.Add("item1981362671", "Rare Standard Frame M")
        ItemTree.Nodes.Add("item1981362670", "Rare Standard Frame L")
        ItemTree.Nodes.Add("item873622227", "Advanced Standard Frame XS")
        ItemTree.Nodes.Add("item873622058", "Advanced Standard Frame XL")
        ItemTree.Nodes.Add("item1981363796", "Advanced Standard Frame S")
        ItemTree.Nodes.Add("item1981362606", "Advanced Standard Frame M")
        ItemTree.Nodes.Add("item1981362607", "Advanced Standard Frame L")
        ItemTree.Nodes.Add("item873676070", "Uncommon Standard Frame XS")
        ItemTree.Nodes.Add("item1981362450", "Uncommon Standard Frame S")
        ItemTree.Nodes.Add("item1981362536", "Uncommon Standard Frame M")
        ItemTree.Nodes.Add("item1981362539", "Uncommon Standard Frame L")
        ItemTree.Nodes.Add("item873663991", "Basic Standard Frame XS")
        ItemTree.Nodes.Add("item1981362643", "Basic Standard Frame S")
        ItemTree.Nodes.Add("item1981362473", "Basic Standard Frame M")
        ItemTree.Nodes.Add("item1981362474", "Basic Standard Frame L")
        ItemTree.Nodes.Add("item1179593235", "Exotic Reinforced Frame XS")
        ItemTree.Nodes.Add("item1179593236", "Exotic Reinforced Frame XL")
        ItemTree.Nodes.Add("item994057929", "Exotic Reinforced Frame S")
        ItemTree.Nodes.Add("item994057943", "Exotic Reinforced Frame M")
        ItemTree.Nodes.Add("item994057936", "Exotic Reinforced Frame L")
        ItemTree.Nodes.Add("item1179605664", "Rare Reinforced Frame XS")
        ItemTree.Nodes.Add("item1179605671", "Rare Reinforced Frame XL")
        ItemTree.Nodes.Add("item994057994", "Rare Reinforced Frame S")
        ItemTree.Nodes.Add("item994058004", "Rare Reinforced Frame M")
        ItemTree.Nodes.Add("item994058003", "Rare Reinforced Frame L")
        ItemTree.Nodes.Add("item1179601457", "Advanced Reinforced Frame XS")
        ItemTree.Nodes.Add("item1179601462", "Advanced Reinforced Frame XL")
        ItemTree.Nodes.Add("item994058059", "Advanced Reinforced Frame S")
        ItemTree.Nodes.Add("item994058069", "Advanced Reinforced Frame M")
        ItemTree.Nodes.Add("item994058066", "Advanced Reinforced Frame L")
        ItemTree.Nodes.Add("item1179614604", "Uncommon Reinforced Frame XS")
        ItemTree.Nodes.Add("item1179614597", "Uncommon Reinforced Frame XL")
        ItemTree.Nodes.Add("item994058119", "Uncommon Reinforced Frame S")
        ItemTree.Nodes.Add("item994058141", "Uncommon Reinforced Frame M")
        ItemTree.Nodes.Add("item994058140", "Uncommon Reinforced Frame L")
        ItemTree.Nodes.Add("item1179610525", "Basic Reinforced Frame XS")
        ItemTree.Nodes.Add("item1179610516", "Basic Reinforced Frame XL")
        ItemTree.Nodes.Add("item994058182", "Basic Reinforced Frame S")
        ItemTree.Nodes.Add("item994058204", "Basic Reinforced Frame M")
        ItemTree.Nodes.Add("item994058205", "Basic Reinforced Frame L")
        ItemTree.Nodes.Add("item567007766", "Advanced Casing S")
        ItemTree.Nodes.Add("item567007899", "Exotic Casing S")
        ItemTree.Nodes.Add("item946524256", "Rare Casing XS")
        ItemTree.Nodes.Add("item946544989", "Advanced Casing XS")
        ItemTree.Nodes.Add("item946544964", "Advanced Casing XL")
        ItemTree.Nodes.Add("item567007760", "Advanced Casing M")
        ItemTree.Nodes.Add("item567007775", "Advanced Casing L")
        ItemTree.Nodes.Add("item946516044", "Uncommon Casing XS")
        ItemTree.Nodes.Add("item946516085", "Uncommon Casing XL")
        ItemTree.Nodes.Add("item567008215", "Uncommon Casing S")
        ItemTree.Nodes.Add("item567008209", "Uncommon Casing M")
        ItemTree.Nodes.Add("item946503935", "Basic Casing XS")
        ItemTree.Nodes.Add("item567008148", "Basic Casing S")
        ItemTree.Nodes.Add("item3936127017", "Advanced Screw")
        ItemTree.Nodes.Add("item3936127018", "Uncommon Screw")
        ItemTree.Nodes.Add("item3936127019", "Basic Screw")
        ItemTree.Nodes.Add("item1799107244", "Advanced Pipe")
        ItemTree.Nodes.Add("item1799107247", "Uncommon Pipe")
        ItemTree.Nodes.Add("item1799107246", "Basic Pipe")
        ItemTree.Nodes.Add("item1234754160", "Advanced LED")
        ItemTree.Nodes.Add("item1234754161", "Uncommon LED")
        ItemTree.Nodes.Add("item1234754162", "Basic LED")
        ItemTree.Nodes.Add("item466630567", "Advanced Fixation")
        ItemTree.Nodes.Add("item466630564", "Uncommon Fixation")
        ItemTree.Nodes.Add("item466630565", "Basic Fixation")
        ItemTree.Nodes.Add("item2872711781", "Advanced Connector")
        ItemTree.Nodes.Add("item2872711778", "Uncommon Connector")
        ItemTree.Nodes.Add("item2872711779", "Basic Connector")
        ItemTree.Nodes.Add("item794666751", "Advanced Component")
        ItemTree.Nodes.Add("item794666748", "Uncommon Component")
        ItemTree.Nodes.Add("item794666749", "Basic Component")
        ItemTree.Nodes.Add("item3026262169", "Exotic Missile Silo XS")
        ItemTree.Nodes.Add("item3857142764", "Exotic Missile Silo S")
        ItemTree.Nodes.Add("item3857142758", "Exotic Missile Silo M")
        ItemTree.Nodes.Add("item3857142757", "Exotic Missile Silo L")
        ItemTree.Nodes.Add("item3026356360", "Rare Missile Silo XS")
        ItemTree.Nodes.Add("item3857142317", "Rare Missile Silo S")
        ItemTree.Nodes.Add("item3857142311", "Rare Missile Silo M")
        ItemTree.Nodes.Add("item3857142308", "Rare Missile Silo L")
        ItemTree.Nodes.Add("item3026385661", "Advanced Missile Silo XS")
        ItemTree.Nodes.Add("item3857142123", "Advanced Missile Silo S")
        ItemTree.Nodes.Add("item3857142113", "Advanced Missile Silo M")
        ItemTree.Nodes.Add("item3857142112", "Advanced Missile Silo L")
        ItemTree.Nodes.Add("item1428608303", "Advanced Screen XS")
        ItemTree.Nodes.Add("item1428608292", "Advanced Screen XL")
        ItemTree.Nodes.Add("item1428596474", "Uncommon Screen XS")
        ItemTree.Nodes.Add("item1428596467", "Uncommon Screen XL")
        ItemTree.Nodes.Add("item184261478", "Uncommon Screen L")
        ItemTree.Nodes.Add("item184261422", "Basic Screen S")
        ItemTree.Nodes.Add("item184261412", "Basic Screen M")
        ItemTree.Nodes.Add("item997370746", "Rare Robotic Arm M")
        ItemTree.Nodes.Add("item997368670", "Advanced Robotic Arm M")
        ItemTree.Nodes.Add("item997368607", "Uncommon Robotic Arm M")
        ItemTree.Nodes.Add("item2999955044", "Basic Robotic Arm XL")
        ItemTree.Nodes.Add("item997368796", "Basic Robotic Arm M")
        ItemTree.Nodes.Add("item997368799", "Basic Robotic Arm L")
        ItemTree.Nodes.Add("item3291043715", "Rare Power Transformer XL")
        ItemTree.Nodes.Add("item4186198480", "Rare Power Transformer M")
        ItemTree.Nodes.Add("item4186198483", "Rare Power Transformer L")
        ItemTree.Nodes.Add("item4186198417", "Advanced Power Transformer M")
        ItemTree.Nodes.Add("item4186205972", "Basic Power Transformer M")
        ItemTree.Nodes.Add("item3501536208", "Advanced Ore Scanner L")
        ItemTree.Nodes.Add("item3501535314", "Exotic Ore Scanner L")
        ItemTree.Nodes.Add("item3501536145", "Rare Ore Scanner L")
        ItemTree.Nodes.Add("item788805607", "Uncommon Ore Scanner XL")
        ItemTree.Nodes.Add("item3501535518", "Uncommon Ore Scanner L")
        ItemTree.Nodes.Add("item3501535556", "Basic Ore Scanner S")
        ItemTree.Nodes.Add("item3501535583", "Basic Ore Scanner L")
        ItemTree.Nodes.Add("item242607950", "Advanced Motherboard M")
        ItemTree.Nodes.Add("item407844051", "Rare Mobile Panel XS")
        ItemTree.Nodes.Add("item407844040", "Rare Mobile Panel XL")
        ItemTree.Nodes.Add("item494823725", "Rare Mobile Panel S")
        ItemTree.Nodes.Add("item494823731", "Rare Mobile Panel M")
        ItemTree.Nodes.Add("item494823730", "Rare Mobile Panel L")
        ItemTree.Nodes.Add("item408022872", "Advanced Mobile Panel XS")
        ItemTree.Nodes.Add("item408022865", "Advanced Mobile Panel XL")
        ItemTree.Nodes.Add("item494821869", "Advanced Mobile Panel S")
        ItemTree.Nodes.Add("item494821863", "Advanced Mobile Panel M")
        ItemTree.Nodes.Add("item494821860", "Advanced Mobile Panel L")
        ItemTree.Nodes.Add("item407969641", "Uncommon Mobile Panel XS")
        ItemTree.Nodes.Add("item407969632", "Uncommon Mobile Panel XL")
        ItemTree.Nodes.Add("item494821804", "Uncommon Mobile Panel S")
        ItemTree.Nodes.Add("item494821798", "Uncommon Mobile Panel M")
        ItemTree.Nodes.Add("item494821797", "Uncommon Mobile Panel L")
        ItemTree.Nodes.Add("item407690298", "Basic Mobile Panel XS")
        ItemTree.Nodes.Add("item407690291", "Basic Mobile Panel XL")
        ItemTree.Nodes.Add("item494825071", "Basic Mobile Panel S")
        ItemTree.Nodes.Add("item494825061", "Basic Mobile Panel M")
        ItemTree.Nodes.Add("item494825062", "Basic Mobile Panel L")
        ItemTree.Nodes.Add("item204462057", "Exotic Mechanical Sensor XS")
        ItemTree.Nodes.Add("item204469317", "Advanced Mechanical Sensor XS")
        ItemTree.Nodes.Add("item204444775", "Basic Mechanical Sensor XS")
        ItemTree.Nodes.Add("item4210044590", "Exotic Magnetic Rail XS")
        ItemTree.Nodes.Add("item2722609330", "Exotic Magnetic Rail S")
        ItemTree.Nodes.Add("item2722609340", "Exotic Magnetic Rail M")
        ItemTree.Nodes.Add("item2722609339", "Exotic Magnetic Rail L")
        ItemTree.Nodes.Add("item4210065279", "Rare Magnetic Rail XS")
        ItemTree.Nodes.Add("item2722609523", "Rare Magnetic Rail S")
        ItemTree.Nodes.Add("item2722609533", "Rare Magnetic Rail M")
        ItemTree.Nodes.Add("item2722609530", "Rare Magnetic Rail L")
        ItemTree.Nodes.Add("item4211034905", "Advanced Magnetic Rail XS")
        ItemTree.Nodes.Add("item2722610741", "Advanced Magnetic Rail S")
        ItemTree.Nodes.Add("item2722610747", "Advanced Magnetic Rail M")
        ItemTree.Nodes.Add("item2722610746", "Advanced Magnetic Rail L")
        ItemTree.Nodes.Add("item1829611507", "Uncommon Light XS")
        ItemTree.Nodes.Add("item3345566836", "Uncommon Light S")
        ItemTree.Nodes.Add("item1252764136", "Uncommon Laser Chamber XS")
        ItemTree.Nodes.Add("item1252823771", "Exotic Laser Chamber XS")
        ItemTree.Nodes.Add("item2825506243", "Exotic Laser Chamber S")
        ItemTree.Nodes.Add("item2825506265", "Exotic Laser Chamber M")
        ItemTree.Nodes.Add("item2825506266", "Exotic Laser Chamber L")
        ItemTree.Nodes.Add("item1252819658", "Rare Laser Chamber XS")
        ItemTree.Nodes.Add("item2825506178", "Rare Laser Chamber S")
        ItemTree.Nodes.Add("item2825506200", "Rare Laser Chamber M")
        ItemTree.Nodes.Add("item2825506203", "Rare Laser Chamber L")
        ItemTree.Nodes.Add("item1252768249", "Advanced Laser Chamber XS")
        ItemTree.Nodes.Add("item1252768242", "Advanced Laser Chamber XL")
        ItemTree.Nodes.Add("item2825503297", "Advanced Laser Chamber S")
        ItemTree.Nodes.Add("item2825503323", "Advanced Laser Chamber M")
        ItemTree.Nodes.Add("item2825503320", "Advanced Laser Chamber L")
        ItemTree.Nodes.Add("item2825505990", "Basic Laser Chamber S")
        ItemTree.Nodes.Add("item962700664", "Rare Ionic Chamber XS")
        ItemTree.Nodes.Add("item962700657", "Rare Ionic Chamber XL")
        ItemTree.Nodes.Add("item1390563197", "Rare Ionic Chamber S")
        ItemTree.Nodes.Add("item1390563195", "Rare Ionic Chamber M")
        ItemTree.Nodes.Add("item1390563172", "Rare Ionic Chamber L")
        ItemTree.Nodes.Add("item962704747", "Advanced Ionic Chamber XS")
        ItemTree.Nodes.Add("item962704738", "Advanced Ionic Chamber XL")
        ItemTree.Nodes.Add("item1390563262", "Advanced Ionic Chamber S")
        ItemTree.Nodes.Add("item1390563256", "Advanced Ionic Chamber M")
        ItemTree.Nodes.Add("item1390563239", "Advanced Ionic Chamber L")
        ItemTree.Nodes.Add("item963003738", "Uncommon Ionic Chamber XS")
        ItemTree.Nodes.Add("item963003731", "Uncommon Ionic Chamber XL")
        ItemTree.Nodes.Add("item1390563327", "Uncommon Ionic Chamber S")
        ItemTree.Nodes.Add("item1390563321", "Uncommon Ionic Chamber M")
        ItemTree.Nodes.Add("item1390563302", "Uncommon Ionic Chamber L")
        ItemTree.Nodes.Add("item962712586", "Basic Ionic Chamber XS")
        ItemTree.Nodes.Add("item962712579", "Basic Ionic Chamber XL")
        ItemTree.Nodes.Add("item1390562873", "Basic Ionic Chamber S")
        ItemTree.Nodes.Add("item1390562879", "Basic Ionic Chamber M")
        ItemTree.Nodes.Add("item1390562878", "Basic Ionic Chamber L")
        ItemTree.Nodes.Add("item792299450", "Basic Gas Cylinder XS")
        ItemTree.Nodes.Add("item2119086146", "Basic Gas Cylinder S")
        ItemTree.Nodes.Add("item2119086168", "Basic Gas Cylinder M")
        ItemTree.Nodes.Add("item3740078396", "Exotic Firing System XS")
        ItemTree.Nodes.Add("item3242492880", "Exotic Firing System S")
        ItemTree.Nodes.Add("item3242492874", "Exotic Firing System M")
        ItemTree.Nodes.Add("item3242492875", "Exotic Firing System L")
        ItemTree.Nodes.Add("item3740074253", "Rare Firing System XS")
        ItemTree.Nodes.Add("item3242492817", "Rare Firing System S")
        ItemTree.Nodes.Add("item3242492811", "Rare Firing System M")
        ItemTree.Nodes.Add("item3242492810", "Rare Firing System L")
        ItemTree.Nodes.Add("item3740021214", "Advanced Firing System XS")
        ItemTree.Nodes.Add("item3242491986", "Advanced Firing System S")
        ItemTree.Nodes.Add("item3242491976", "Advanced Firing System M")
        ItemTree.Nodes.Add("item3242491977", "Advanced Firing System L")
        ItemTree.Nodes.Add("item3740092443", "Basic Firing System XS")
        ItemTree.Nodes.Add("item3172866509", "Uncommon Electric Engine XL")
        ItemTree.Nodes.Add("item3728054834", "Basic Electric Engine S")
        ItemTree.Nodes.Add("item3728054836", "Basic Electric Engine M")
        ItemTree.Nodes.Add("item1775106556", "Advanced Core System M")
        ItemTree.Nodes.Add("item1775106424", "Exotic Core System S")
        ItemTree.Nodes.Add("item1775106492", "Rare Core System L")
        ItemTree.Nodes.Add("item1775106620", "Uncommon Core System S")
        ItemTree.Nodes.Add("item1775106618", "Uncommon Core System M")
        ItemTree.Nodes.Add("item1775106597", "Uncommon Core System L")
        ItemTree.Nodes.Add("item1172598456", "Basic Core System XS")
        ItemTree.Nodes.Add("item1775106685", "Basic Core System S")
        ItemTree.Nodes.Add("item3431996625", "Advanced Control System S")
        ItemTree.Nodes.Add("item3431996639", "Advanced Control System M")
        ItemTree.Nodes.Add("item3431996632", "Advanced Control System L")
        ItemTree.Nodes.Add("item972195890", "Basic Control System XS")
        ItemTree.Nodes.Add("item3431996502", "Basic Control System S")
        ItemTree.Nodes.Add("item3431996504", "Basic Control System M")
        ItemTree.Nodes.Add("item4016318475", "Rare Combustion Chamber XS")
        ItemTree.Nodes.Add("item2662310018", "Rare Combustion Chamber S")
        ItemTree.Nodes.Add("item2662310020", "Rare Combustion Chamber M")
        ItemTree.Nodes.Add("item2662310021", "Rare Combustion Chamber L")
        ItemTree.Nodes.Add("item4016322616", "Advanced Combustion Chamber XS")
        ItemTree.Nodes.Add("item2662310081", "Advanced Combustion Chamber S")
        ItemTree.Nodes.Add("item2662310087", "Advanced Combustion Chamber M")
        ItemTree.Nodes.Add("item2662310086", "Advanced Combustion Chamber L")
        ItemTree.Nodes.Add("item4016359657", "Uncommon Combustion Chamber XS")
        ItemTree.Nodes.Add("item2662309888", "Uncommon Combustion Chamber S")
        ItemTree.Nodes.Add("item2662309894", "Uncommon Combustion Chamber M")
        ItemTree.Nodes.Add("item2662309895", "Uncommon Combustion Chamber L")
        ItemTree.Nodes.Add("item4017996241", "Basic Combustion Chamber XS")
        ItemTree.Nodes.Add("item2662317132", "Basic Combustion Chamber S")
        ItemTree.Nodes.Add("item2662317126", "Basic Combustion Chamber M")
        ItemTree.Nodes.Add("item2662317125", "Basic Combustion Chamber L")
        ItemTree.Nodes.Add("item625115242", "Rare Chemical Container M")
        ItemTree.Nodes.Add("item3714764686", "Advanced Chemical Container XL")
        ItemTree.Nodes.Add("item625115345", "Advanced Chemical Container S")
        ItemTree.Nodes.Add("item625115179", "Advanced Chemical Container M")
        ItemTree.Nodes.Add("item625115176", "Advanced Chemical Container L")
        ItemTree.Nodes.Add("item625289663", "Uncommon Chemical Container M")
        ItemTree.Nodes.Add("item3717621915", "Basic Chemical Container XS")
        ItemTree.Nodes.Add("item3717621906", "Basic Chemical Container XL")
        ItemTree.Nodes.Add("item625289720", "Basic Chemical Container S")
        ItemTree.Nodes.Add("item625289726", "Basic Chemical Container M")
        ItemTree.Nodes.Add("item625289727", "Basic Chemical Container L")
        ItemTree.Nodes.Add("item2302031898", "Advanced Antenna XL")
        ItemTree.Nodes.Add("item2302040376", "Exotic Antenna XL")
        ItemTree.Nodes.Add("item1080827676", "Exotic Antenna S")
        ItemTree.Nodes.Add("item1080827674", "Exotic Antenna M")
        ItemTree.Nodes.Add("item1080827653", "Exotic Antenna L")
        ItemTree.Nodes.Add("item1080827741", "Rare Antenna S")
        ItemTree.Nodes.Add("item1080827739", "Rare Antenna M")
        ItemTree.Nodes.Add("item1080827716", "Rare Antenna L")
        ItemTree.Nodes.Add("item1080827550", "Advanced Antenna S")
        ItemTree.Nodes.Add("item1080827544", "Advanced Antenna M")
        ItemTree.Nodes.Add("item1080827527", "Advanced Antenna L")
        ItemTree.Nodes.Add("item2302027954", "Uncommon Antenna XS")
        ItemTree.Nodes.Add("item1080827615", "Uncommon Antenna S")
        ItemTree.Nodes.Add("item1080827609", "Uncommon Antenna M")
        ItemTree.Nodes.Add("item1080827590", "Uncommon Antenna L")
        ItemTree.Nodes.Add("item2301991330", "Basic Antenna XS")
        ItemTree.Nodes.Add("item2301991355", "Basic Antenna XL")
        ItemTree.Nodes.Add("item1080826905", "Basic Antenna S")
        ItemTree.Nodes.Add("item984088007", "Advanced Quantum Barrier")
        ItemTree.Nodes.Add("item2601646634", "Exotic Quantum Alignment Unit")
        ItemTree.Nodes.Add("item2601646635", "Rare Quantum Alignment Unit")
        ItemTree.Nodes.Add("item2601646636", "Advanced Quantum Alignment Unit")
        ItemTree.Nodes.Add("item375744325", "Advanced Antimatter Core")
        ItemTree.Nodes.Add("item2999509666", "Advanced Anti-Gravity Core")
        ItemTree.Nodes.Add("item2999509692", "Exotic Anti-Gravity Core")
        ItemTree.Nodes.Add("item2999509693", "Rare Anti-Gravity Core")
        ItemTree.Nodes.Add("item2599686739", "Advanced Solid Warhead")
        ItemTree.Nodes.Add("item2599686738", "Uncommon Solid Warhead")
        ItemTree.Nodes.Add("item3640212314", "Exotic Singularity Container")
        ItemTree.Nodes.Add("item3640212315", "Rare Singularity Container")
        ItemTree.Nodes.Add("item3640212312", "Advanced Singularity Container")
        ItemTree.Nodes.Add("item3640212313", "Uncommon Singularity Container")
        ItemTree.Nodes.Add("item3640212318", "Basic Singularity Container")
        ItemTree.Nodes.Add("item850241762", "Exotic Quantum Core")
        ItemTree.Nodes.Add("item850241763", "Rare Quantum Core")
        ItemTree.Nodes.Add("item850241764", "Advanced Quantum Core")
        ItemTree.Nodes.Add("item850241765", "Uncommon Quantum Core")
        ItemTree.Nodes.Add("item850241766", "Basic Quantum Core")
        ItemTree.Nodes.Add("item3808417020", "Advanced Processor")
        ItemTree.Nodes.Add("item3808417021", "Uncommon Processor")
        ItemTree.Nodes.Add("item3808417022", "Basic Processor")
        ItemTree.Nodes.Add("item527681751", "Exotic Power System")
        ItemTree.Nodes.Add("item527681750", "Rare Power System")
        ItemTree.Nodes.Add("item527681753", "Advanced Power System")
        ItemTree.Nodes.Add("item527681752", "Uncommon Power System")
        ItemTree.Nodes.Add("item527681755", "Basic Power System")
        ItemTree.Nodes.Add("item3739200055", "Exotic Optics")
        ItemTree.Nodes.Add("item3739200048", "Rare Optics")
        ItemTree.Nodes.Add("item3739200049", "Advanced Optics")
        ItemTree.Nodes.Add("item3739200050", "Uncommon Optics")
        ItemTree.Nodes.Add("item3739200051", "Basic Optics")
        ItemTree.Nodes.Add("item1246524879", "Uncommon Magnet")
        ItemTree.Nodes.Add("item1246524878", "Basic Magnet")
        ItemTree.Nodes.Add("item1246524876", "Advanced Magnet")
        ItemTree.Nodes.Add("item1246524866", "Exotic Magnet")
        ItemTree.Nodes.Add("item1246524877", "Rare Magnet")
        ItemTree.Nodes.Add("item1971447079", "Rare Injector")
        ItemTree.Nodes.Add("item1971447078", "Advanced Injector")
        ItemTree.Nodes.Add("item1971447073", "Uncommon Injector")
        ItemTree.Nodes.Add("item1971447072", "Basic Injector")
        ItemTree.Nodes.Add("item2541811484", "Advanced Explosive Module")
        ItemTree.Nodes.Add("item2541811485", "Uncommon Explosive Module")
        ItemTree.Nodes.Add("item2541811486", "Basic Explosive Module")
        ItemTree.Nodes.Add("item1331181091", "Exotic hydraulics")
        ItemTree.Nodes.Add("item1331181088", "Rare hydraulics")
        ItemTree.Nodes.Add("item1331181089", "Advanced hydraulics")
        ItemTree.Nodes.Add("item1331181118", "Uncommon hydraulics")
        ItemTree.Nodes.Add("item1331181119", "Basic hydraulics")
        ItemTree.Nodes.Add("item1297540450", "Exotic Electronics")
        ItemTree.Nodes.Add("item1297540451", "Rare Electronics")
        ItemTree.Nodes.Add("item1297540452", "Advanced Electronics")
        ItemTree.Nodes.Add("item1297540453", "Uncommon Electronics")
        ItemTree.Nodes.Add("item1297540454", "Basic Electronics")
        ItemTree.Nodes.Add("item2660328732", "Exotic Burner")
        ItemTree.Nodes.Add("item2660328735", "Rare Burner")
        ItemTree.Nodes.Add("item2660328734", "Advanced Burner")
        ItemTree.Nodes.Add("item2660328729", "Uncommon Burner")
        ItemTree.Nodes.Add("item2660328728", "Basic Burner")
        ItemTree.Nodes.Add("item3661595540", "Advanced Antimatter Capsule")
        ItemTree.Nodes.Add("item3661595539", "Uncommon Antimatter Capsule")
        ItemTree.Nodes.Add("item3661595538", "Basic Antimatter Capsule")
        ItemTree.Nodes.Add("itemnil", "Sanctuary Territory Unit")
        ItemTree.Nodes.Add("item1358842892", "Territory Unit")
        ItemTree.Nodes.Add("item2738359893", "Static Core Unit S")
        ItemTree.Nodes.Add("item2738359963", "Static Core Unit XS")
        ItemTree.Nodes.Add("item910155097", "Static Core Unit L")
        ItemTree.Nodes.Add("item909184430", "Static Core Unit M")
        ItemTree.Nodes.Add("item3624940909", "Space Core Unit S")
        ItemTree.Nodes.Add("item3624942103", "Space Core Unit XS")
        ItemTree.Nodes.Add("item5904544", "Space Core Unit L")
        ItemTree.Nodes.Add("item5904195", "Space Core Unit M")
        ItemTree.Nodes.Add("item183890525", "Dynamic Core Unit S")
        ItemTree.Nodes.Add("item183890713", "Dynamic Core Unit XS")
        ItemTree.Nodes.Add("item1417952990", "Dynamic Core Unit L")
        ItemTree.Nodes.Add("item1418170469", "Dynamic Core Unit M")
        ItemTree.Nodes.Add("item3929462194", "Virtual scaffolding projector")
        ItemTree.Nodes.Add("item4118496992", "Space Radar S")
        ItemTree.Nodes.Add("item3831485995", "Space Radar M")
        ItemTree.Nodes.Add("item2802863920", "Space Radar L")
        ItemTree.Nodes.Add("item4213791403", "Atmospheric Radar S")
        ItemTree.Nodes.Add("item612626034", "Atmospheric Radar M")
        ItemTree.Nodes.Add("item3094514782", "Atmospheric Radar L")
        ItemTree.Nodes.Add("item2012928469", "Pressure tile")
        ItemTree.Nodes.Add("item4181147843", "Manual Switch")
        ItemTree.Nodes.Add("item1550904282", "Manual Button XS")
        ItemTree.Nodes.Add("item2896791363", "Manual Button S")
        ItemTree.Nodes.Add("item783555860", "Laser Receiver")
        ItemTree.Nodes.Add("item2153998731", "Infrared Laser Receiver")
        ItemTree.Nodes.Add("item3996923355", "Info Button S")
        ItemTree.Nodes.Add("item485149481", "Detection Zone M")
        ItemTree.Nodes.Add("item485149228", "Detection Zone S")
        ItemTree.Nodes.Add("item485151209", "Detection Zone XS")
        ItemTree.Nodes.Add("item4241228057", "Detection Zone L")
        ItemTree.Nodes.Add("item1261703398", "Navigator Chair")
        ItemTree.Nodes.Add("item2169816178", "Encampment Chair")
        ItemTree.Nodes.Add("item554266799", "Office Chair")
        ItemTree.Nodes.Add("item853107412", "railgun S")
        ItemTree.Nodes.Add("item2733257194", "railgun M")
        ItemTree.Nodes.Add("item430145504", "railgun L")
        ItemTree.Nodes.Add("item31327772", "railgun XS")
        ItemTree.Nodes.Add("item1109891544", "missile S")
        ItemTree.Nodes.Add("item1557865377", "missile M")
        ItemTree.Nodes.Add("item3873532190", "missile L")
        ItemTree.Nodes.Add("item1260582276", "missile XS")
        ItemTree.Nodes.Add("item32593579", "laser S")
        ItemTree.Nodes.Add("item1117413121", "laser M")
        ItemTree.Nodes.Add("item3516228574", "laser L")
        ItemTree.Nodes.Add("item11309408", "laser XS")
        ItemTree.Nodes.Add("item1901919706", "cannon S")
        ItemTree.Nodes.Add("item1699425404", "cannon M")
        ItemTree.Nodes.Add("item3289044684", "cannon L")
        ItemTree.Nodes.Add("item3741742452", "cannon XS")
        ItemTree.Nodes.Add("item1109114394", "Resurrection Node")
        ItemTree.Nodes.Add("item3923388834", "Vertical Light XS")
        ItemTree.Nodes.Add("item3231255047", "Vertical Light S")
        ItemTree.Nodes.Add("item1603266808", "Vertical Light M")
        ItemTree.Nodes.Add("item2027152926", "Vertical Light L")
        ItemTree.Nodes.Add("item177821174", "Square Light XS")
        ItemTree.Nodes.Add("item3981684520", "Square Light S")
        ItemTree.Nodes.Add("item632353355", "Square Light M")
        ItemTree.Nodes.Add("item823697268", "Square Light L")
        ItemTree.Nodes.Add("item25682791", "Long Light XS")
        ItemTree.Nodes.Add("item3180371725", "Long Light S")
        ItemTree.Nodes.Add("item677591159", "Long Light M")
        ItemTree.Nodes.Add("item3524314552", "Long Light L")
        ItemTree.Nodes.Add("item787207321", "Headlight")
        ItemTree.Nodes.Add("item3988662884", "Transparent Screen L")
        ItemTree.Nodes.Add("item3988662951", "Transparent Screen M")
        ItemTree.Nodes.Add("item3988663014", "Transparent Screen S")
        ItemTree.Nodes.Add("item3988665660", "Transparent Screen XS")
        ItemTree.Nodes.Add("item879675317", "Screen XL")
        ItemTree.Nodes.Add("item184261558", "Screen M")
        ItemTree.Nodes.Add("item184261490", "Screen S")
        ItemTree.Nodes.Add("item184261427", "Screen XS")
        ItemTree.Nodes.Add("item3919696834", "Vertical Sign XS")
        ItemTree.Nodes.Add("item2610895147", "Vertical Sign M")
        ItemTree.Nodes.Add("item1533790308", "Vertical Sign L")
        ItemTree.Nodes.Add("item166656023", "Sign XS")
        ItemTree.Nodes.Add("item362159734", "Sign S")
        ItemTree.Nodes.Add("item3068429457", "Sign M")
        ItemTree.Nodes.Add("item166549741", "Sign L")
        ItemTree.Nodes.Add("item4078067869", "Landing Gear XS")
        ItemTree.Nodes.Add("item1884031929", "Landing Gear S")
        ItemTree.Nodes.Add("item1899560165", "Landing Gear M")
        ItemTree.Nodes.Add("item2667697870", "Landing Gear L")
        ItemTree.Nodes.Add("item3685982092", "Force Field L")
        ItemTree.Nodes.Add("item3686006062", "Force Field M")
        ItemTree.Nodes.Add("item3685998465", "Force Field S")
        ItemTree.Nodes.Add("item3686074288", "Force Field XS")
        ItemTree.Nodes.Add("item201196316", "Sliding Door S")
        ItemTree.Nodes.Add("item1139773633", "Reinforced Sliding Door")
        ItemTree.Nodes.Add("item1097676949", "Gate XS")
        ItemTree.Nodes.Add("item1256519882", "Gate XL")
        ItemTree.Nodes.Add("item581667413", "Expanded gate S")
        ItemTree.Nodes.Add("item2858887382", "Gate M")
        ItemTree.Nodes.Add("item1289884535", "Expanded gate L")
        ItemTree.Nodes.Add("item3709017308", "Interior door")
        ItemTree.Nodes.Add("item4249659729", "Airlock")
        ItemTree.Nodes.Add("item741980535", "Sliding Door M")
        ItemTree.Nodes.Add("item3663249627", "Elevator XS")
        ItemTree.Nodes.Add("item3858829819", "Territory Scanner")
        ItemTree.Nodes.Add("item1722901246", "Telemeter")
        ItemTree.Nodes.Add("item2585415184", "Gyroscope")
        ItemTree.Nodes.Add("item4139262245", "Transfer Unit")
        ItemTree.Nodes.Add("item2556123438", "basic Smelter M")
        ItemTree.Nodes.Add("item3701755071", "basic Refiner M")
        ItemTree.Nodes.Add("item3914155468", "basic Recycler M")
        ItemTree.Nodes.Add("item2022563937", "basic Metalwork Industry M")
        ItemTree.Nodes.Add("item3857150880", "basic Honeycomb Refinery M")
        ItemTree.Nodes.Add("item1215026169", "basic Glass Furnace M")
        ItemTree.Nodes.Add("item2702446443", "basic Electronics industry M")
        ItemTree.Nodes.Add("item2681009434", "basic Chemical industry M")
        ItemTree.Nodes.Add("item1762226876", "basic Assembly Line XS")
        ItemTree.Nodes.Add("item1762226819", "basic Assembly Line XL")
        ItemTree.Nodes.Add("item983225818", "basic Assembly Line S")
        ItemTree.Nodes.Add("item983225808", "basic Assembly Line M")
        ItemTree.Nodes.Add("item983225811", "basic Assembly Line L")
        ItemTree.Nodes.Add("item409410678", "basic 3D Printer M")
        ItemTree.Nodes.Add("item3775402879", "Vertical Booster XS")
        ItemTree.Nodes.Add("item3556600005", "Vertical Booster S")
        ItemTree.Nodes.Add("item913372512", "Vertical Booster M")
        ItemTree.Nodes.Add("item2216363013", "Vertical Booster L")
        ItemTree.Nodes.Add("item2243775376", "Basic Space Engine XS")
        ItemTree.Nodes.Add("item2200254788", "Basic Space Engine XL")
        ItemTree.Nodes.Add("item1326357437", "Basic Space Engine S")
        ItemTree.Nodes.Add("item85796763", "Basic Space Engine M")
        ItemTree.Nodes.Add("item2495558023", "Basic Space Engine L")
        ItemTree.Nodes.Add("item3243532126", "Retro-rocket Brake M")
        ItemTree.Nodes.Add("item1452351552", "Retro-rocket Brake L")
        ItemTree.Nodes.Add("item3039211660", "Retro-rocket Brake S")
        ItemTree.Nodes.Add("item2112772336", "Rocket Engine S")
        ItemTree.Nodes.Add("item3623903713", "Rocket Engine M")
        ItemTree.Nodes.Add("item359938916", "Rocket Engine L")
        ItemTree.Nodes.Add("item2333052331", "Hover engine S")
        ItemTree.Nodes.Add("item2991279664", "Hover engine M")
        ItemTree.Nodes.Add("item1105322870", "Flat hover engine L")
        ItemTree.Nodes.Add("item2494203891", "Hover engine L")
        ItemTree.Nodes.Add("item710193240", "Basic Atmospheric Engine XS")
        ItemTree.Nodes.Add("item2043566501", "Basic Atmospheric Engine S")
        ItemTree.Nodes.Add("item4072611011", "Basic Atmospheric Engine M")
        ItemTree.Nodes.Add("item2375915630", "Basic Atmospheric Engine L")
        ItemTree.Nodes.Add("item1727614690", "Wing XS")
        ItemTree.Nodes.Add("item2532454166", "Wing S")
        ItemTree.Nodes.Add("item4179758576", "Wing variant M")
        ItemTree.Nodes.Add("item404188468", "Wing M")
        ItemTree.Nodes.Add("item1455311973", "Stabilizer XS")
        ItemTree.Nodes.Add("item1234961120", "Stabilizer S")
        ItemTree.Nodes.Add("item3474622996", "Stabilizer M")
        ItemTree.Nodes.Add("item1090402453", "Stabilizer L")
        ItemTree.Nodes.Add("item2292270972", "Aileron XS")
        ItemTree.Nodes.Add("item2334843027", "Compact Aileron XS")
        ItemTree.Nodes.Add("item1923840124", "Compact Aileron S")
        ItemTree.Nodes.Add("item4017253256", "Compact Aileron M")
        ItemTree.Nodes.Add("item2737703104", "Aileron S")
        ItemTree.Nodes.Add("item1856288931", "Aileron M")
        ItemTree.Nodes.Add("item65048663", "Atmospheric Airbrake S")
        ItemTree.Nodes.Add("item2198271703", "Atmospheric Airbrake M")
        ItemTree.Nodes.Add("item104971834", "Atmospheric Airbrake L")
        ItemTree.Nodes.Add("item2648523849", "Adjustor XS")
        ItemTree.Nodes.Add("item47474508", "Adjustor S")
        ItemTree.Nodes.Add("item3790013467", "Adjustor M")
        ItemTree.Nodes.Add("item2818864930", "Adjustor L")
        ItemTree.Nodes.Add("item1694177571", "Relay")
        ItemTree.Nodes.Add("item2082095499", "Receiver S")
        ItemTree.Nodes.Add("item736740615", "Receiver M")
        ItemTree.Nodes.Add("item3732634076", "Receiver XS")
        ItemTree.Nodes.Add("item1707712023", "OR operator")
        ItemTree.Nodes.Add("item2629309308", "NOT operator")
        ItemTree.Nodes.Add("item1784722190", "Laser Emitter")
        ItemTree.Nodes.Add("item609676854", "Infrared Laser Emitter")
        ItemTree.Nodes.Add("item3287187256", "Emitter S")
        ItemTree.Nodes.Add("item2809213930", "Emitter M")
        ItemTree.Nodes.Add("item1279651501", "Emitter XS")
        ItemTree.Nodes.Add("item1474604499", "Delay Line")
        ItemTree.Nodes.Add("item812400865", "Databank")
        ItemTree.Nodes.Add("item888063487", "10 counter")
        ItemTree.Nodes.Add("item888062910", "7 counter")
        ItemTree.Nodes.Add("item888062908", "5 counter")
        ItemTree.Nodes.Add("item888062905", "2 counter")
        ItemTree.Nodes.Add("item888062906", "3 counter")
        ItemTree.Nodes.Add("item2569152632", "AND operator")
        ItemTree.Nodes.Add("item542805258", "Keyboard unit")
        ItemTree.Nodes.Add("item124823209", "Spaceship Hologram S")
        ItemTree.Nodes.Add("item85154060", "Spaceship Hologram M")
        ItemTree.Nodes.Add("item2137895179", "Spaceship Hologram L")
        ItemTree.Nodes.Add("item4090740447", "Planet Hologram")
        ItemTree.Nodes.Add("item1541106442", "Planet Hologram L")
        ItemTree.Nodes.Add("item630574502", "Suspended Fruit Plant")
        ItemTree.Nodes.Add("item630574503", "Suspended Plant B")
        ItemTree.Nodes.Add("item630574504", "Suspended Plant A")
        ItemTree.Nodes.Add("item630574505", "Bagged Plant B")
        ItemTree.Nodes.Add("item630574506", "Bagged Plant A")
        ItemTree.Nodes.Add("item195870299", "Plant Case E")
        ItemTree.Nodes.Add("item195870296", "Plant Case D")
        ItemTree.Nodes.Add("item195870297", "Plant Case C")
        ItemTree.Nodes.Add("item195870294", "Plant Case B")
        ItemTree.Nodes.Add("item195870295", "Plant Case A")
        ItemTree.Nodes.Add("item1797415729", "Plant")
        ItemTree.Nodes.Add("item2648123924", "Bonsai")
        ItemTree.Nodes.Add("item3106061141", "Ficus Plant B")
        ItemTree.Nodes.Add("item3106061140", "Ficus Plant A")
        ItemTree.Nodes.Add("item3106061143", "Foliage Plant Case B")
        ItemTree.Nodes.Add("item3106061142", "Foliage Plant Case A")
        ItemTree.Nodes.Add("item3106061129", "Salad Plant Case")
        ItemTree.Nodes.Add("item3106061128", "Eggplant Plant Case")
        ItemTree.Nodes.Add("item3106061131", "Squash Plant Case")
        ItemTree.Nodes.Add("item3106061130", "Plant Case M")
        ItemTree.Nodes.Add("item3106061133", "Plant Case S")
        ItemTree.Nodes.Add("item543225023", "Pipe D M")
        ItemTree.Nodes.Add("item2709793409", "Pipe B M")
        ItemTree.Nodes.Add("item2824951359", "Pipe A M")
        ItemTree.Nodes.Add("item2917319456", "Pipe Connector M")
        ItemTree.Nodes.Add("item2123842216", "Pipe corner M")
        ItemTree.Nodes.Add("item2937058341", "Pipe C M")
        ItemTree.Nodes.Add("item4145570204", "Steel panel")
        ItemTree.Nodes.Add("item1220701936", "Steel column")
        ItemTree.Nodes.Add("item3337817675", "Hull decorative Element C")
        ItemTree.Nodes.Add("item3337817674", "Hull decorative Element B")
        ItemTree.Nodes.Add("item3337817677", "Hull decorative Element A")
        ItemTree.Nodes.Add("item3893102542", "Wooden table L")
        ItemTree.Nodes.Add("item1395483977", "Table")
        ItemTree.Nodes.Add("item2018455538", "Wooden Sofa")
        ItemTree.Nodes.Add("item1235633417", "Sofa")
        ItemTree.Nodes.Add("item4083139459", "Shelf empty")
        ItemTree.Nodes.Add("item4083139484", "Shelf half full")
        ItemTree.Nodes.Add("item4083139485", "Shelf full")
        ItemTree.Nodes.Add("item3193900802", "Eye Doll's Workshop - Artist Unknown")
        ItemTree.Nodes.Add("item3193900801", "Parrotos Sanctuary - Artist Unknown")
        ItemTree.Nodes.Add("item3193900800", "HMS Ajax33 - Artist Unknown")
        ItemTree.Nodes.Add("item2216112746", "Nightstand")
        ItemTree.Nodes.Add("item1082668972", "Wooden low table")
        ItemTree.Nodes.Add("item3824401006", "Wooden dresser")
        ItemTree.Nodes.Add("item283549593", "Dresser")
        ItemTree.Nodes.Add("item3736537839", "Wooden armchair")
        ItemTree.Nodes.Add("item2453312794", "Wooden Chair")
        ItemTree.Nodes.Add("item3813093434", "Round carpet")
        ItemTree.Nodes.Add("item3813093435", "Square carpet")
        ItemTree.Nodes.Add("item542122758", "Bench")
        ItemTree.Nodes.Add("item4216497731", "Bed")
        ItemTree.Nodes.Add("item3845900543", "Wooden table M")
        ItemTree.Nodes.Add("item1268259677", "Wooden wardrobe")
        ItemTree.Nodes.Add("item2428627426", "Wardrobe")
        ItemTree.Nodes.Add("item1407324391", "Trash can")
        ItemTree.Nodes.Add("item1700326385", "Corner Cable Model B")
        ItemTree.Nodes.Add("item1700326384", "Corner Cable Model A")
        ItemTree.Nodes.Add("item1542146746", "Cable Model C M")
        ItemTree.Nodes.Add("item1542146745", "Cable Model B M")
        ItemTree.Nodes.Add("item1542146744", "Cable Model A M")
        ItemTree.Nodes.Add("item1542390549", "Cable Model C S")
        ItemTree.Nodes.Add("item1542390550", "Cable Model B S")
        ItemTree.Nodes.Add("item1700326390", "Corner Cable Model C")
        ItemTree.Nodes.Add("item1542390551", "Cable Model A S")
        ItemTree.Nodes.Add("item3929116491", "Toilet unit B")
        ItemTree.Nodes.Add("item4186859262", "Toilet unit A")
        ItemTree.Nodes.Add("item3517217013", "Urinal unit")
        ItemTree.Nodes.Add("item2846288811", "Shower Unit")
        ItemTree.Nodes.Add("item400937499", "Sink unit")
        ItemTree.Nodes.Add("item1377211067", "Barrier corner")
        ItemTree.Nodes.Add("item3261824887", "Barrier M")
        ItemTree.Nodes.Add("item3261824822", "Barrier S")
        ItemTree.Nodes.Add("item1951235468", "Antenna S")
        ItemTree.Nodes.Add("item206489025", "Antenna M")
        ItemTree.Nodes.Add("item413322747", "Antenna L")
        ItemTree.Nodes.Add("item2429336341", "Wingtip S")
        ItemTree.Nodes.Add("item3695530525", "Wingtip M")
        ItemTree.Nodes.Add("item3292462663", "Wingtip L")
        ItemTree.Nodes.Add("item1894947006", "Vertical wing")
        ItemTree.Nodes.Add("item3268459843", "Window XS")
        ItemTree.Nodes.Add("item1952409967", "Bay window XL")
        ItemTree.Nodes.Add("item242448402", "Window S")
        ItemTree.Nodes.Add("item515378511", "Armored window XS")
        ItemTree.Nodes.Add("item3014939922", "Armored window S")
        ItemTree.Nodes.Add("item2158665549", "Armored window M")
        ItemTree.Nodes.Add("item1804139232", "Armored window L")
        ItemTree.Nodes.Add("item3924941627", "Window M")
        ItemTree.Nodes.Add("item894516284", "Window L")
        ItemTree.Nodes.Add("item561162197", "Glass Panel S")
        ItemTree.Nodes.Add("item2266946860", "Glass Panel M")
        ItemTree.Nodes.Add("item1165506034", "Glass Panel L")
        ItemTree.Nodes.Add("item2433054263", "Canopy Windshield flat S")
        ItemTree.Nodes.Add("item1900076171", "Canopy Windshield flat M")
        ItemTree.Nodes.Add("item1001848134", "Canopy Windshield flat L")
        ItemTree.Nodes.Add("item695039310", "Canopy Windshield corner S")
        ItemTree.Nodes.Add("item1484667376", "Canopy Windshield corner M")
        ItemTree.Nodes.Add("item4226053198", "Canopy Windshield corner L")
        ItemTree.Nodes.Add("item1326565833", "Canopy Windshield tilted S")
        ItemTree.Nodes.Add("item4167375414", "Canopy Windshield tilted M")
        ItemTree.Nodes.Add("item2086563919", "Canopy Windshield tilted L")
        ItemTree.Nodes.Add("item286542481", "Emergency controller")
        ItemTree.Nodes.Add("item564736657", "Gunner Module M")
        ItemTree.Nodes.Add("item3327293642", "Gunner Module L")
        ItemTree.Nodes.Add("item1373443625", "Gunner Module S")
        ItemTree.Nodes.Add("item1744160618", "Hovercraft seat controller")
        ItemTree.Nodes.Add("item3640291983", "Cockpit controller")
        ItemTree.Nodes.Add("item3655856020", "Command seat controller")
        ItemTree.Nodes.Add("item1866437084", "Remote Controller")
        ItemTree.Nodes.Add("item3415128439", "Programming board")

        ItemTree.Nodes.Add("item2125213321", "Container L")
        ItemTree.Nodes.Add("item1689381593", "Container XS")
        ItemTree.Nodes.Add("item1594689569", "Container S")
        ItemTree.Nodes.Add("item521274609", "Container M")
        ItemTree.Nodes.Add("item373359444", "Container Hub")

        ItemTree.Nodes.Add("item773467906", "Space Fuel Tank M")
        ItemTree.Nodes.Add("item1790622152", "Space Fuel Tank S")
        ItemTree.Nodes.Add("item2212207656", "Space Fuel Tank L")

        ItemTree.Nodes.Add("item1663412227", "Rocket Fuel Tank XS")
        ItemTree.Nodes.Add("item2477859329", "Rocket Fuel Tank M")
        ItemTree.Nodes.Add("item4180073139", "Rocket Fuel Tank L")
        ItemTree.Nodes.Add("item3126840739", "Rocket Fuel Tank S")

        ItemTree.Nodes.Add("item3273319200", "Atmospheric Fuel Tank XS")
        ItemTree.Nodes.Add("item3464628964", "Atmospheric Fuel Tank M")
        ItemTree.Nodes.Add("item3039582547", "Atmospheric Fuel Tank L")
        ItemTree.Nodes.Add("item2183619036", "Atmospheric Fuel Tank S")

        ItemTree.Nodes.Add("item16651125", "Deprecated Dispenser")

        ItemTree.Nodes.Add("item50309297", "Ammo Container XL L")
        ItemTree.Nodes.Add("item300986010", "Ammo Container S XS")
        ItemTree.Nodes.Add("item2300179701", "Ammo Container M S")
        ItemTree.Nodes.Add("item923167511", "Ammo Container L M")

        ItemTree.Nodes.Add("item966816758", "Anti-Gravity Pulsor")
        ItemTree.Nodes.Add("item3997343699", "Anti-Gravity Generator S")
        ItemTree.Nodes.Add("item233079829", "Anti-Gravity Generator M")
        ItemTree.Nodes.Add("item294414265", "Anti-Gravity Generator L")

        ItemTree.Nodes.Add("item1182663952", "Manganese Scrap")
        ItemTree.Nodes.Add("item1423148560", "Sulfur Scrap")
        ItemTree.Nodes.Add("item1831205658", "Sodium Scrap")
        ItemTree.Nodes.Add("item3814734889", "Silver Scrap")
        ItemTree.Nodes.Add("item4063983201", "Silicon Scrap")
        ItemTree.Nodes.Add("item270611770", "Scandium Scrap")
        ItemTree.Nodes.Add("item409671366", "Nickel Scrap")
        ItemTree.Nodes.Add("item2115439708", "Lithium Scrap")
        ItemTree.Nodes.Add("item2558961706", "Iron Scrap")
        ItemTree.Nodes.Add("item1032380176", "Gold Scrap")
        ItemTree.Nodes.Add("item3150580281", "Fluorine Scrap")
        ItemTree.Nodes.Add("item3630798120", "Copper Scrap")
        ItemTree.Nodes.Add("item1370993297", "Cobalt Scrap")
        ItemTree.Nodes.Add("item409040753", "Chromium Scrap")
        ItemTree.Nodes.Add("item3857279161", "Carbon Scrap")
        ItemTree.Nodes.Add("item1251531294", "Calcium Scrap")
        ItemTree.Nodes.Add("item2417840347", "Aluminium Scrap")

        ItemTree.Nodes.Add("item3211418846", "Pure Scandium")
        ItemTree.Nodes.Add("item947806142", "Pure Oxygen")
        ItemTree.Nodes.Add("item1126600143", "Niobium pure")
        ItemTree.Nodes.Add("item3012303017", "Nickel pure")
        ItemTree.Nodes.Add("item2007627267", "Vanadium pure")
        ItemTree.Nodes.Add("item752542080", "Pure Titanium")
        ItemTree.Nodes.Add("item3822811562", "Pure Sulfur")
        ItemTree.Nodes.Add("item3603734543", "Pure Sodium")
        ItemTree.Nodes.Add("item1807690770", "Silver Pure")
        ItemTree.Nodes.Add("item2589986891", "Pure Silicon")
        ItemTree.Nodes.Add("item2421303625", "Pure manganese")
        ItemTree.Nodes.Add("item3810111622", "Pure lithium")
        ItemTree.Nodes.Add("item198782496", "Pure Iron")
        ItemTree.Nodes.Add("item1010524904", "Pure hydrogen")
        ItemTree.Nodes.Add("item3837955371", "Pure Gold")
        ItemTree.Nodes.Add("item3323724376", "Pure Fluorine")
        ItemTree.Nodes.Add("item1466453887", "Pure Copper")
        ItemTree.Nodes.Add("item2031444137", "Pure Cobalt")
        ItemTree.Nodes.Add("item2147954574", "Pure Chromium")
        ItemTree.Nodes.Add("item159858782", "Pure carbon")
        ItemTree.Nodes.Add("item2112763718", "Pure Calcium")
        ItemTree.Nodes.Add("item2240749601", "Pure aluminium")

        ItemTree.Nodes.Add("item770773323", "Wood product")
        ItemTree.Nodes.Add("item255776324", "Vanamer product")
        ItemTree.Nodes.Add("item1734893264", "Ti-Nb Supraconductor product")
        ItemTree.Nodes.Add("item511774178", "Steel product")
        ItemTree.Nodes.Add("item2984358477", "Stainless Steel product")
        ItemTree.Nodes.Add("item2565702107", "Silumin Product")
        ItemTree.Nodes.Add("item2929462635", "Sc-Al Alloy Product")
        ItemTree.Nodes.Add("item2550840787", "Red Gold Product")
        ItemTree.Nodes.Add("item2097691217", "Polysulfide plastic product")
        ItemTree.Nodes.Add("item2014531313", "Polycarbonate plastic product")
        ItemTree.Nodes.Add("item4103265826", "Polycalcite plastic product")
        ItemTree.Nodes.Add("item331532952", "Marble product")
        ItemTree.Nodes.Add("item3518490274", "Maraging Steel product")
        ItemTree.Nodes.Add("item4150961531", "Manganese Reinforced glass product")
        ItemTree.Nodes.Add("item3987872305", "Mangalloy product")
        ItemTree.Nodes.Add("item167908167", "Inconel product")
        ItemTree.Nodes.Add("item3292291904", "Grade 5 Titanium Alloy product")
        ItemTree.Nodes.Add("item606249095", "Gold-Coated glass product")
        ItemTree.Nodes.Add("item3308209457", "Glass product")
        ItemTree.Nodes.Add("item918590356", "Fluoropolymer product")
        ItemTree.Nodes.Add("item231758472", "Duralumin product")
        ItemTree.Nodes.Add("item1673011820", "Cu-Ag Alloy product")
        ItemTree.Nodes.Add("item645870905", "Concrete product")
        ItemTree.Nodes.Add("item1622880428", "Carbon Fiber product")
        ItemTree.Nodes.Add("item1034957327", "Calcium Reinforced Copper product")
        ItemTree.Nodes.Add("item2679709617", "Brick Honeycomb")
        ItemTree.Nodes.Add("item2646210914", "Biological matter product")
        ItemTree.Nodes.Add("item2021406770", "Al-Li Alloy product")
        ItemTree.Nodes.Add("item18262914", "Al-Fe Alloy product")
        ItemTree.Nodes.Add("item2301749833", "Ag-Li Reinforced glass product")
        ItemTree.Nodes.Add("item1942154251", "Advanced glass product")

        ItemTree.Nodes.Add("item2162350405", "Vanadinite")
        ItemTree.Nodes.Add("item629636034", "Illmenite")
        ItemTree.Nodes.Add("item4041459743", "Pyrite")
        ItemTree.Nodes.Add("item343766315", "Natron")
        ItemTree.Nodes.Add("item1050500112", "Acanthite")
        ItemTree.Nodes.Add("item3724036288", "Quartz")
        ItemTree.Nodes.Add("item271971371", "Kolbeckite")
        ItemTree.Nodes.Add("item1065079614", "Garnierite")
        ItemTree.Nodes.Add("item3934774987", "Rhodonite")
        ItemTree.Nodes.Add("item3837858336", "Petalite")
        ItemTree.Nodes.Add("item4234772167", "Hematite")
        ItemTree.Nodes.Add("item1866812055", "Gold nuggets")
        ItemTree.Nodes.Add("item1467310917", "Cryolite")
        ItemTree.Nodes.Add("item2289641763", "Malachite")
        ItemTree.Nodes.Add("item2029139010", "Chromite")
        ItemTree.Nodes.Add("item299255727", "Coal")
        ItemTree.Nodes.Add("item3086347393", "Limestone")
        ItemTree.Nodes.Add("item262147665", "Bauxite")
        ItemTree.Nodes.Add("item789110817", "Columbite")
        ItemTree.Nodes.Add("item3546085401", "Cobaltite")

        ItemTree.Nodes.Add("item2497146600", "White pattern wood")
        ItemTree.Nodes.Add("item2497146600", "Stained gray pattern wood")
        ItemTree.Nodes.Add("item2497146600", "Aged gray pattern wood")
        ItemTree.Nodes.Add("item2497146600", "Gray pattern wood")
        ItemTree.Nodes.Add("item2497146600", "Stained brown pattern wood 4")
        ItemTree.Nodes.Add("item2497146600", "Aged brown pattern wood 4")
        ItemTree.Nodes.Add("item2497146600", "Brown pattern wood 4")
        ItemTree.Nodes.Add("item2497146600", "Stained brown pattern wood 3")
        ItemTree.Nodes.Add("item2497146600", "Aged brown pattern wood 3")
        ItemTree.Nodes.Add("item2497146600", "Brown pattern wood 3")
        ItemTree.Nodes.Add("item2497146600", "Stained brown pattern wood 2")
        ItemTree.Nodes.Add("item2497146600", "Aged brown pattern wood 2")
        ItemTree.Nodes.Add("item2497146600", "Brown pattern wood 2")
        ItemTree.Nodes.Add("item2497146600", "Stained brown pattern wood 1")
        ItemTree.Nodes.Add("item2497146600", "Aged brown pattern wood 1")
        ItemTree.Nodes.Add("item2497146600", "Brown pattern wood 1")
        ItemTree.Nodes.Add("item2497146600", "Black pattern wood")
        ItemTree.Nodes.Add("item2497146600", "Matte light gray wood")
        ItemTree.Nodes.Add("item2497146600", "Matte dark gray wood")
        ItemTree.Nodes.Add("item2497146600", "Matte gray wood")
        ItemTree.Nodes.Add("item2497146600", "Matte light brown wood 4")
        ItemTree.Nodes.Add("item2497146600", "Matte dark brown wood 4")
        ItemTree.Nodes.Add("item2497146600", "Matte brown wood 4")
        ItemTree.Nodes.Add("item2497146600", "Matte light brown wood 3")
        ItemTree.Nodes.Add("item2497146600", "Matte dark brown wood 3")
        ItemTree.Nodes.Add("item2497146600", "Matte brown wood 3")
        ItemTree.Nodes.Add("item2497146600", "Matte light brown wood 2")
        ItemTree.Nodes.Add("item2497146600", "Matte dark brown wood 2")
        ItemTree.Nodes.Add("item2497146600", "Matte brown wood 2")
        ItemTree.Nodes.Add("item2497146600", "Matte light brown wood 1")
        ItemTree.Nodes.Add("item2497146600", "Matte dark brown wood 1")
        ItemTree.Nodes.Add("item2497146600", "Matte brown wood 1")
        ItemTree.Nodes.Add("item2497146600", "Matte black wood")
        ItemTree.Nodes.Add("item2497146600", "Polished white wood")
        ItemTree.Nodes.Add("item2497146600", "Polished light gray wood")
        ItemTree.Nodes.Add("item2497146600", "Polished dark gray wood")
        ItemTree.Nodes.Add("item2497146600", "Polished gray wood")
        ItemTree.Nodes.Add("item2497146600", "Polished light brown wood 4")
        ItemTree.Nodes.Add("item2497146600", "Polished dark brown wood 4")
        ItemTree.Nodes.Add("item2497146600", "Polished brown wood 4")
        ItemTree.Nodes.Add("item2497146600", "Polished light brown wood 3")
        ItemTree.Nodes.Add("item2497146600", "Polished dark brown wood 3")
        ItemTree.Nodes.Add("item2497146600", "Polished brown wood 3")
        ItemTree.Nodes.Add("item2497146600", "Polished light brown wood 2")
        ItemTree.Nodes.Add("item2497146600", "Polished dark brown wood 2")
        ItemTree.Nodes.Add("item2497146600", "Polished brown wood 2")
        ItemTree.Nodes.Add("item2497146600", "Polished light brown wood 1")
        ItemTree.Nodes.Add("item2497146600", "Polished dark brown wood 1")
        ItemTree.Nodes.Add("item2497146600", "Polished brown wood 1")
        ItemTree.Nodes.Add("item2497146600", "Polished black wood")
        ItemTree.Nodes.Add("item2497146600", "Matte white wood")

        ItemTree.Nodes.Add("item2814304696", "Stained yellow pattern steel")
        ItemTree.Nodes.Add("item2814304696", "Aged yellow pattern steel")
        ItemTree.Nodes.Add("item2814304696", "Blue pattern steel")
        ItemTree.Nodes.Add("item2814304696", "Black pattern steel")
        ItemTree.Nodes.Add("item2814304696", "Stained beige pattern steel")
        ItemTree.Nodes.Add("item2814304696", "Aged beige pattern steel")
        ItemTree.Nodes.Add("item2814304696", "Beige pattern steel")
        ItemTree.Nodes.Add("item2814304696", "Galvanized light yellow steel")
        ItemTree.Nodes.Add("item2814304696", "Galvanized dark yellow steel")
        ItemTree.Nodes.Add("item2814304696", "Galvanized yellow steel")
        ItemTree.Nodes.Add("item2814304696", "Galvanized white steel")
        ItemTree.Nodes.Add("item2814304696", "Galvanized light red steel")
        ItemTree.Nodes.Add("item2814304696", "Galvanized red steel (cold)")
        ItemTree.Nodes.Add("item2814304696", "Galvanized red steel")
        ItemTree.Nodes.Add("item2814304696", "Galvanized light orange steel")
        ItemTree.Nodes.Add("item2814304696", "Galvanized dark orange steel")
        ItemTree.Nodes.Add("item2814304696", "Galvanized orange steel")
        ItemTree.Nodes.Add("item2814304696", "Galvanized light green steel")
        ItemTree.Nodes.Add("item2814304696", "Galvanized dark green steel")
        ItemTree.Nodes.Add("item2814304696", "Galvanized green steel")
        ItemTree.Nodes.Add("item2814304696", "Galvanized light gray steel")
        ItemTree.Nodes.Add("item2814304696", "Galvanized dark gray steel")
        ItemTree.Nodes.Add("item2814304696", "Galvanized gray steel")
        ItemTree.Nodes.Add("item2814304696", "Galvanized light blue steel")
        ItemTree.Nodes.Add("item2814304696", "Galvanized dark blue steel")
        ItemTree.Nodes.Add("item2814304696", "Galvanized blue steel")
        ItemTree.Nodes.Add("item2814304696", "Galvanized black steel")
        ItemTree.Nodes.Add("item2814304696", "Galvanized light beige steel")
        ItemTree.Nodes.Add("item2814304696", "Galvanized dark beige steel")
        ItemTree.Nodes.Add("item2814304696", "Galvanized beige steel")
        ItemTree.Nodes.Add("item2814304696", "Polished light yellow steel")
        ItemTree.Nodes.Add("item2814304696", "Polished dark yellow steel")
        ItemTree.Nodes.Add("item2814304696", "Polished dark red steel")
        ItemTree.Nodes.Add("item2814304696", "Polished red steel")
        ItemTree.Nodes.Add("item2814304696", "Polished light orange steel")
        ItemTree.Nodes.Add("item2814304696", "Polished dark orange steel")
        ItemTree.Nodes.Add("item2814304696", "Polished orange steel")
        ItemTree.Nodes.Add("item2814304696", "Polished light green steel")
        ItemTree.Nodes.Add("item2814304696", "Polished dark green steel")
        ItemTree.Nodes.Add("item2814304696", "Polished green steel")
        ItemTree.Nodes.Add("item2814304696", "Polished light gray steel")
        ItemTree.Nodes.Add("item2814304696", "Polished dark gray steel")
        ItemTree.Nodes.Add("item2814304696", "Polished gray steel")
        ItemTree.Nodes.Add("item2814304696", "Polished light blue steel")
        ItemTree.Nodes.Add("item2814304696", "Polished dark blue steel")
        ItemTree.Nodes.Add("item2814304696", "Polished black steel")
        ItemTree.Nodes.Add("item2814304696", "Polished light beige steel")
        ItemTree.Nodes.Add("item2814304696", "Polished dark beige steel")
        ItemTree.Nodes.Add("item2814304696", "Polished beige steel")
        ItemTree.Nodes.Add("item2814304696", "Polished yellow steel")
        ItemTree.Nodes.Add("item2814304696", "Polished white steel")
        ItemTree.Nodes.Add("item2814304696", "Galvanized dark red steel")
        ItemTree.Nodes.Add("item2814304696", "Polished blue steel")
        ItemTree.Nodes.Add("item2814304696", "Yellow pattern steel")
        ItemTree.Nodes.Add("item2814304696", "White pattern steel")
        ItemTree.Nodes.Add("item2814304696", "Stained red pattern steel")
        ItemTree.Nodes.Add("item2814304696", "Red pattern steel")
        ItemTree.Nodes.Add("item2814304696", "Stained orange pattern steel")
        ItemTree.Nodes.Add("item2814304696", "Aged orange pattern steel")
        ItemTree.Nodes.Add("item2814304696", "Orange pattern steel")
        ItemTree.Nodes.Add("item2814304696", "Stained green pattern steel")
        ItemTree.Nodes.Add("item2814304696", "Aged green pattern steel")
        ItemTree.Nodes.Add("item2814304696", "Green pattern steel")
        ItemTree.Nodes.Add("item2814304696", "Stained gray pattern steel")
        ItemTree.Nodes.Add("item2814304696", "Aged gray pattern steel")
        ItemTree.Nodes.Add("item2814304696", "Gray pattern steel")
        ItemTree.Nodes.Add("item2814304696", "Stained blue pattern steel")
        ItemTree.Nodes.Add("item2814304696", "Aged blue pattern steel")
        ItemTree.Nodes.Add("item2814304696", "Aged red pattern steel")
        ItemTree.Nodes.Add("item2814304696", "Polished light red steel steel")

        ItemTree.Nodes.Add("item1269767483", "Stained yellow pattern plastic")
        ItemTree.Nodes.Add("item1269767483", "Aged yellow pattern plastic")
        ItemTree.Nodes.Add("item1269767483", "Stained yellow pattern plastic (cold)")
        ItemTree.Nodes.Add("item1269767483", "Aged yellow pattern plastic (cold)")
        ItemTree.Nodes.Add("item1269767483", "Yellow pattern plastic (cold)")
        ItemTree.Nodes.Add("item1269767483", "Yellow pattern plastic")
        ItemTree.Nodes.Add("item1269767483", "White pattern plastic")
        ItemTree.Nodes.Add("item1269767483", "Stained red pattern plastic")
        ItemTree.Nodes.Add("item1269767483", "Aged red pattern plastic")
        ItemTree.Nodes.Add("item1269767483", "Stained red pattern plastic (cold)")
        ItemTree.Nodes.Add("item1269767483", "Aged red pattern plastic (cold)")
        ItemTree.Nodes.Add("item1269767483", "Red pattern plastic (cold)")
        ItemTree.Nodes.Add("item1269767483", "Red pattern plastic")
        ItemTree.Nodes.Add("item1269767483", "Stained orange pattern plastic")
        ItemTree.Nodes.Add("item1269767483", "Aged orange pattern plastic")
        ItemTree.Nodes.Add("item1269767483", "Stained orange pattern plastic (cold)")
        ItemTree.Nodes.Add("item1269767483", "Aged orange pattern plastic (cold)")
        ItemTree.Nodes.Add("item1269767483", "Orange pattern plastic (cold)")
        ItemTree.Nodes.Add("item1269767483", "Orange pattern plastic")
        ItemTree.Nodes.Add("item1269767483", "Stained green pattern plastic")
        ItemTree.Nodes.Add("item1269767483", "Aged green pattern plastic")
        ItemTree.Nodes.Add("item1269767483", "Stained green pattern plastic (cold)")
        ItemTree.Nodes.Add("item1269767483", "Aged green pattern plastic (cold)")
        ItemTree.Nodes.Add("item1269767483", "Green pattern plastic (cold)")
        ItemTree.Nodes.Add("item1269767483", "Green pattern plastic")
        ItemTree.Nodes.Add("item1269767483", "Stained gray pattern plastic")
        ItemTree.Nodes.Add("item1269767483", "Aged gray pattern plastic")
        ItemTree.Nodes.Add("item1269767483", "Gray pattern plastic")
        ItemTree.Nodes.Add("item1269767483", "Stained blue pattern plastic")
        ItemTree.Nodes.Add("item1269767483", "Aged blue pattern plastic")
        ItemTree.Nodes.Add("item1269767483", "Stained blue pattern plastic (cold)")
        ItemTree.Nodes.Add("item1269767483", "Aged blue pattern plastic (cold)")
        ItemTree.Nodes.Add("item1269767483", "Blue pattern plastic (cold)")
        ItemTree.Nodes.Add("item1269767483", "Blue pattern plastic")
        ItemTree.Nodes.Add("item1269767483", "Black pattern plastic")
        ItemTree.Nodes.Add("item1269767483", "Stained beige pattern plastic")
        ItemTree.Nodes.Add("item1269767483", "Aged beige pattern plastic")
        ItemTree.Nodes.Add("item1269767483", "Stained beige pattern plastic (cold)")
        ItemTree.Nodes.Add("item1269767483", "Aged beige pattern plastic (cold)")
        ItemTree.Nodes.Add("item1269767483", "Beige pattern plastic(cold)")
        ItemTree.Nodes.Add("item1269767483", "Beige pattern plastic")
        ItemTree.Nodes.Add("item1269767483", "Matte light yellow plastic")
        ItemTree.Nodes.Add("item1269767483", "Matte dark yellow plastic")
        ItemTree.Nodes.Add("item1269767483", "Matte light yellow plastic (cold)")
        ItemTree.Nodes.Add("item1269767483", "Matte dark yellow plastic (cold)")
        ItemTree.Nodes.Add("item1269767483", "Matte yellow plastic (cold)")
        ItemTree.Nodes.Add("item1269767483", "Matte yellow plastic")
        ItemTree.Nodes.Add("item1269767483", "Matte white plastic")
        ItemTree.Nodes.Add("item1269767483", "Matte light red plastic")
        ItemTree.Nodes.Add("item1269767483", "Matte dark red plastic")
        ItemTree.Nodes.Add("item1269767483", "Matte light red plastic (cold)")
        ItemTree.Nodes.Add("item1269767483", "Matte dark red plastic (cold)")
        ItemTree.Nodes.Add("item1269767483", "Matte red plastic (cold)")
        ItemTree.Nodes.Add("item1269767483", "Matte red plastic")
        ItemTree.Nodes.Add("item1269767483", "Matte light orange plastic")
        ItemTree.Nodes.Add("item1269767483", "Matte dark orange plastic")
        ItemTree.Nodes.Add("item1269767483", "Matte light orange plastic (cold)")
        ItemTree.Nodes.Add("item1269767483", "Matte dark orange plastic (cold)")
        ItemTree.Nodes.Add("item1269767483", "Matte orange plastic (cold)")
        ItemTree.Nodes.Add("item1269767483", "Matte orange plastic")
        ItemTree.Nodes.Add("item1269767483", "Matte light green plastic")
        ItemTree.Nodes.Add("item1269767483", "Matte dark green plastic")
        ItemTree.Nodes.Add("item1269767483", "Matte light green plastic (cold)")
        ItemTree.Nodes.Add("item1269767483", "Matte dark green plastic (cold)")
        ItemTree.Nodes.Add("item1269767483", "Matte green plastic (cold)")
        ItemTree.Nodes.Add("item1269767483", "Matte green plastic")
        ItemTree.Nodes.Add("item1269767483", "Matte light gray plastic")
        ItemTree.Nodes.Add("item1269767483", "Matte dark gray plastic")
        ItemTree.Nodes.Add("item1269767483", "Matte gray plastic")
        ItemTree.Nodes.Add("item1269767483", "Matte light blue plastic")
        ItemTree.Nodes.Add("item1269767483", "Matte dark blue plastic")
        ItemTree.Nodes.Add("item1269767483", "Matte light blue plastic (cold)")
        ItemTree.Nodes.Add("item1269767483", "Matte dark blue plastic (cold)")
        ItemTree.Nodes.Add("item1269767483", "Matte blue (cold) plastic")
        ItemTree.Nodes.Add("item1269767483", "Matte blue plastic")
        ItemTree.Nodes.Add("item1269767483", "Matte black plastic")
        ItemTree.Nodes.Add("item1269767483", "Matte light beige plastic")
        ItemTree.Nodes.Add("item1269767483", "Matte dark beige plastic")
        ItemTree.Nodes.Add("item1269767483", "Matte light beige plastic(cold)")
        ItemTree.Nodes.Add("item1269767483", "Matte dark beige plastic (cold)")
        ItemTree.Nodes.Add("item1269767483", "Matte beige plastic (cold)")
        ItemTree.Nodes.Add("item1269767483", "Matte beige plastic")
        ItemTree.Nodes.Add("item1269767483", "Glossy light yellow plastic")
        ItemTree.Nodes.Add("item1269767483", "Glossy dark yellow plastic")
        ItemTree.Nodes.Add("item1269767483", "Glossy light yellow plastic (cold)")
        ItemTree.Nodes.Add("item1269767483", "Glossy dark yellow plastic (cold)")
        ItemTree.Nodes.Add("item1269767483", "Glossy yellow plastic")
        ItemTree.Nodes.Add("item1269767483", "Glossy yellow plastic")
        ItemTree.Nodes.Add("item1269767483", "Glossy light red plastic")
        ItemTree.Nodes.Add("item1269767483", "Glossy dark red plastic")
        ItemTree.Nodes.Add("item1269767483", "Glossy light red plastic (cold)")
        ItemTree.Nodes.Add("item1269767483", "Glossy dark red plastic (cold)")
        ItemTree.Nodes.Add("item1269767483", "Glossy red plastic (cold)")
        ItemTree.Nodes.Add("item1269767483", "Glossy red plastic")
        ItemTree.Nodes.Add("item1269767483", "Glossy light orange plastic")
        ItemTree.Nodes.Add("item1269767483", "Glossy dark orange plastic")
        ItemTree.Nodes.Add("item1269767483", "Glossy light orange plastic (cold)")
        ItemTree.Nodes.Add("item1269767483", "Glossy dark orange plastic (cold)")
        ItemTree.Nodes.Add("item1269767483", "Glossy orange plastic (cold)")
        ItemTree.Nodes.Add("item1269767483", "Glossy orange plastic")
        ItemTree.Nodes.Add("item1269767483", "Glossy light green plastic")
        ItemTree.Nodes.Add("item1269767483", "Glossy dark green plastic")
        ItemTree.Nodes.Add("item1269767483", "Glossy light green plastic (cold)")
        ItemTree.Nodes.Add("item1269767483", "Glossy dark green plastic (cold)")
        ItemTree.Nodes.Add("item1269767483", "Glossy green plastic (cold)")
        ItemTree.Nodes.Add("item1269767483", "Glossy green plastic")
        ItemTree.Nodes.Add("item1269767483", "Glossy light gray plastic")
        ItemTree.Nodes.Add("item1269767483", "Glossy dark gray plastic")
        ItemTree.Nodes.Add("item1269767483", "Glossy gray plastic")
        ItemTree.Nodes.Add("item1269767483", "Glossy light blue plastic")
        ItemTree.Nodes.Add("item1269767483", "Glossy dark blue plastic")
        ItemTree.Nodes.Add("item1269767483", "Glossy light blue plastic (cold)")
        ItemTree.Nodes.Add("item1269767483", "Glossy dark blue plastic (cold)")
        ItemTree.Nodes.Add("item1269767483", "Glossy blue plastic (cold)")
        ItemTree.Nodes.Add("item1269767483", "Glossy blue plastic")
        ItemTree.Nodes.Add("item1269767483", "Glossy black plastic")
        ItemTree.Nodes.Add("item1269767483", "Glossy light beige plastic")
        ItemTree.Nodes.Add("item1269767483", "Glossy dark beige plastic")
        ItemTree.Nodes.Add("item1269767483", "Glossy light beige plastic (cold)")
        ItemTree.Nodes.Add("item1269767483", "Glossy white plastic")
        ItemTree.Nodes.Add("item1269767483", "Glossy dark beige plastic (cold)")
        ItemTree.Nodes.Add("item1269767483", "Glossy beige plastic (cold)")
        ItemTree.Nodes.Add("item1269767483", "Glossy beige plastic")

        ItemTree.Nodes.Add("item2003621933", "Stained yellow pattern marble (cold)")
        ItemTree.Nodes.Add("item2003621933", "Aged yellow pattern marble (cold)")
        ItemTree.Nodes.Add("item2003621933", "Yellow pattern marble (cold)")
        ItemTree.Nodes.Add("item2003621933", "White pattern marble")
        ItemTree.Nodes.Add("item2003621933", "Stained red pattern marble (cold)")
        ItemTree.Nodes.Add("item2003621933", "Aged red pattern marble (cold)")
        ItemTree.Nodes.Add("item2003621933", "Red pattern marble (cold)")
        ItemTree.Nodes.Add("item2003621933", "Stained orange pattern marble (cold)")
        ItemTree.Nodes.Add("item2003621933", "Aged orange pattern marble (cold)")
        ItemTree.Nodes.Add("item2003621933", "Orange pattern marble (cold)")
        ItemTree.Nodes.Add("item2003621933", "Stained green pattern marble (cold)")
        ItemTree.Nodes.Add("item2003621933", "Aged green pattern marble (cold)")
        ItemTree.Nodes.Add("item2003621933", "Green pattern marble (cold)")
        ItemTree.Nodes.Add("item2003621933", "Stained gray pattern marble")
        ItemTree.Nodes.Add("item2003621933", "Aged gray pattern marble")
        ItemTree.Nodes.Add("item2003621933", "Gray pattern marble")
        ItemTree.Nodes.Add("item2003621933", "Stained blue pattern marble(cold)")
        ItemTree.Nodes.Add("item2003621933", "Aged blue pattern marble (cold)")
        ItemTree.Nodes.Add("item2003621933", "Blue pattern marble (cold)")
        ItemTree.Nodes.Add("item2003621933", "Black pattern marble")
        ItemTree.Nodes.Add("item2003621933", "Stained beige pattern marble (cold)")
        ItemTree.Nodes.Add("item2003621933", "Aged beige pattern marble (cold)")
        ItemTree.Nodes.Add("item2003621933", "Beige pattern marble (cold)")
        ItemTree.Nodes.Add("item2003621933", "Matte light yellow marble (cold)")
        ItemTree.Nodes.Add("item2003621933", "Matte dark yellow marble (cold)")
        ItemTree.Nodes.Add("item2003621933", "Matte yellow marble (cold)")
        ItemTree.Nodes.Add("item2003621933", "Matte white marble")
        ItemTree.Nodes.Add("item2003621933", "Matte light red marble (cold)")
        ItemTree.Nodes.Add("item2003621933", "Matte dark red marble (cold)")
        ItemTree.Nodes.Add("item2003621933", "Matte red marble (cold)")
        ItemTree.Nodes.Add("item2003621933", "Matte light orange marble (cold)")
        ItemTree.Nodes.Add("item2003621933", "Matte dark orange marble (cold)")
        ItemTree.Nodes.Add("item2003621933", "Matte orange marble (cold)")
        ItemTree.Nodes.Add("item2003621933", "Matte light green marble (cold)")
        ItemTree.Nodes.Add("item2003621933", "Matte dark green marble (cold)")
        ItemTree.Nodes.Add("item2003621933", "Matte green marble (cold)")
        ItemTree.Nodes.Add("item2003621933", "Matte light gray marble")
        ItemTree.Nodes.Add("item2003621933", "Matte dark gray marble")
        ItemTree.Nodes.Add("item2003621933", "Matte gray marble")
        ItemTree.Nodes.Add("item2003621933", "Matte light blue marble (cold)")
        ItemTree.Nodes.Add("item2003621933", "Matte dark blue marble (cold)")
        ItemTree.Nodes.Add("item2003621933", "Matte blue marble (cold)")
        ItemTree.Nodes.Add("item2003621933", "Matte black marble")
        ItemTree.Nodes.Add("item2003621933", "Matte light beige marble (cold)")
        ItemTree.Nodes.Add("item2003621933", "Matte dark beige marble (cold)")
        ItemTree.Nodes.Add("item2003621933", "Matte beige marble (cold)")
        ItemTree.Nodes.Add("item2003621933", "Polished light yellow marble (cold)")
        ItemTree.Nodes.Add("item2003621933", "Polished dark yellow marble (cold)")
        ItemTree.Nodes.Add("item2003621933", "Polished yellow marble (cold)")
        ItemTree.Nodes.Add("item2003621933", "Polished white marble")
        ItemTree.Nodes.Add("item2003621933", "Polished light red marble (cold)")
        ItemTree.Nodes.Add("item2003621933", "Polished dark red marble (cold)")
        ItemTree.Nodes.Add("item2003621933", "Polished red marble (cold)")
        ItemTree.Nodes.Add("item2003621933", "Polished orange marble (cold)")
        ItemTree.Nodes.Add("item2003621933", "Polished light green marble (cold)")
        ItemTree.Nodes.Add("item2003621933", "Polished dark green marble (cold)")
        ItemTree.Nodes.Add("item2003621933", "Polished green marble (cold)")
        ItemTree.Nodes.Add("item2003621933", "Polished light gray marble")
        ItemTree.Nodes.Add("item2003621933", "Polished dark gray marble")
        ItemTree.Nodes.Add("item2003621933", "Polished gray marble")
        ItemTree.Nodes.Add("item2003621933", "Polished light blue marble (cold)")
        ItemTree.Nodes.Add("item2003621933", "Polished dark blue marble (cold)")
        ItemTree.Nodes.Add("item2003621933", "Polished blue marble (cold)")
        ItemTree.Nodes.Add("item2003621933", "Polished black marble")
        ItemTree.Nodes.Add("item2003621933", "Polished light beige marble (cold)")
        ItemTree.Nodes.Add("item2003621933", "Polished dark beige marble (cold)")
        ItemTree.Nodes.Add("item2003621933", "Polished beige marble (cold)")
        ItemTree.Nodes.Add("item2003621933", "Polished light orange marble (cold)")
        ItemTree.Nodes.Add("item2003621933", "Polished dark orange marble (cold)")

        ItemTree.Nodes.Add("item38264863", "White pattern concrete")
        ItemTree.Nodes.Add("item38264863", "Stained gray pattern concrete")
        ItemTree.Nodes.Add("item38264863", "Aged gray pattern concrete")
        ItemTree.Nodes.Add("item38264863", "Gray pattern concrete")
        ItemTree.Nodes.Add("item38264863", "Black pattern concrete")
        ItemTree.Nodes.Add("item38264863", "Matte white concrete")
        ItemTree.Nodes.Add("item38264863", "Matte light gray concrete")
        ItemTree.Nodes.Add("item38264863", "Matte dark gray concrete")
        ItemTree.Nodes.Add("item38264863", "Matte gray concrete")
        ItemTree.Nodes.Add("item38264863", "Matte black concrete")
        ItemTree.Nodes.Add("item38264863", "Waxed white concrete")
        ItemTree.Nodes.Add("item38264863", "Waxed light gray concrete")
        ItemTree.Nodes.Add("item38264863", "Waxed dark gray concrete")
        ItemTree.Nodes.Add("item38264863", "Waxed gray concrete")
        ItemTree.Nodes.Add("item38264863", "Waxed black concrete")

        ItemTree.Nodes.Add("item2647328640", "White pattern carbon fiber")
        ItemTree.Nodes.Add("item2647328640", "Stained gray pattern carbon fiber")
        ItemTree.Nodes.Add("item2647328640", "Aged gray pattern carbon fiber")
        ItemTree.Nodes.Add("item2647328640", "Gray pattern carbon fiber")
        ItemTree.Nodes.Add("item2647328640", "Black pattern carbon fiber")
        ItemTree.Nodes.Add("item2647328640", "Matte white carbon fiber")
        ItemTree.Nodes.Add("item2647328640", "Matte light gray carbon fiber")
        ItemTree.Nodes.Add("item2647328640", "Matte dark gray carbon fiber")
        ItemTree.Nodes.Add("item2647328640", "Matte gray carbon fiber")
        ItemTree.Nodes.Add("item2647328640", "Matte black carbon fiber")
        ItemTree.Nodes.Add("item2647328640", "Glossy white carbon fiber")
        ItemTree.Nodes.Add("item2647328640", "Glossy light gray carbon fiber")
        ItemTree.Nodes.Add("item2647328640", "Glossy dark gray carbon fiber")
        ItemTree.Nodes.Add("item2647328640", "Glossy gray carbon fiber")
        ItemTree.Nodes.Add("item2647328640", "Glossy black carbon fiber")

        ItemTree.Nodes.Add("item2698580432", "White pattern brick")
        ItemTree.Nodes.Add("item2698580432", "Stained gray pattern brick")
        ItemTree.Nodes.Add("item2698580432", "Aged gray pattern brick")
        ItemTree.Nodes.Add("item2698580432", "Gray pattern brick")
        ItemTree.Nodes.Add("item2698580432", "Stained pattern brick 4")
        ItemTree.Nodes.Add("item2698580432", "Aged pattern brick 4")
        ItemTree.Nodes.Add("item2698580432", "Brick pattern 4")
        ItemTree.Nodes.Add("item2698580432", "Stained pattern brick 3")
        ItemTree.Nodes.Add("item2698580432", "Aged pattern brick 3")
        ItemTree.Nodes.Add("item2698580432", "Brick pattern 3")
        ItemTree.Nodes.Add("item2698580432", "Stained pattern brick 2")
        ItemTree.Nodes.Add("item2698580432", "Aged pattern brick 2")
        ItemTree.Nodes.Add("item2698580432", "Brick pattern 2")
        ItemTree.Nodes.Add("item2698580432", "Stained pattern brick 1")
        ItemTree.Nodes.Add("item2698580432", "Aged pattern brick 1")
        ItemTree.Nodes.Add("item2698580432", "Brick pattern 1")
        ItemTree.Nodes.Add("item2698580432", "Black pattern brick")
        ItemTree.Nodes.Add("item2698580432", "Matte white brick")
        ItemTree.Nodes.Add("item2698580432", "Matte gray brick")
        ItemTree.Nodes.Add("item2698580432", "Matte light brick 4")
        ItemTree.Nodes.Add("item2698580432", "Matte dark brick 4")
        ItemTree.Nodes.Add("item2698580432", "Matte black brick 4")
        ItemTree.Nodes.Add("item2698580432", "Matte light brick 3")
        ItemTree.Nodes.Add("item2698580432", "Matte dark brick 3")
        ItemTree.Nodes.Add("item2698580432", "Matte black brick 3")
        ItemTree.Nodes.Add("item2698580432", "Matte light brick 2")
        ItemTree.Nodes.Add("item2698580432", "Matte dark brick 2")
        ItemTree.Nodes.Add("item2698580432", "Matte black brick 2")
        ItemTree.Nodes.Add("item2698580432", "Matte light brick 1")
        ItemTree.Nodes.Add("item2698580432", "Matte dark brick 1")
        ItemTree.Nodes.Add("item2698580432", "Matte black brick 1")
        ItemTree.Nodes.Add("item2698580432", "Matte black brick")
        ItemTree.Nodes.Add("item2698580432", "Waxed white brick")
        ItemTree.Nodes.Add("item2698580432", "Waxed light gray brick")
        ItemTree.Nodes.Add("item2698580432", "Waxed dark gray brick")
        ItemTree.Nodes.Add("item2698580432", "Waxed gray brick")
        ItemTree.Nodes.Add("item2698580432", "Waxed stained brick 4")
        ItemTree.Nodes.Add("item2698580432", "Waxed aged brick 4")
        ItemTree.Nodes.Add("item2698580432", "Waxed brick 4")
        ItemTree.Nodes.Add("item2698580432", "Waxed stained brick 3")
        ItemTree.Nodes.Add("item2698580432", "Waxed aged brick 3")
        ItemTree.Nodes.Add("item2698580432", "Waxed brick 3")
        ItemTree.Nodes.Add("item2698580432", "Waxed stained brick 2")
        ItemTree.Nodes.Add("item2698580432", "Waxed aged brick 2")
        ItemTree.Nodes.Add("item2698580432", "Waxed brick 2")
        ItemTree.Nodes.Add("item2698580432", "Waxed stained brick 1")
        ItemTree.Nodes.Add("item2698580432", "Waxed aged brick 1")
        ItemTree.Nodes.Add("item2698580432", "Waxed brick 1")
        ItemTree.Nodes.Add("item2698580432", "Waxed black brick")
        ItemTree.Nodes.Add("item2698580432", "Matte dark gray brick")
        ItemTree.Nodes.Add("item2698580432", "Matte light gray brick")

        ItemTree.Nodes.Add("item402511494", "White pattern Titanium")
        ItemTree.Nodes.Add("item402511494", "Stained gray pattern Titanium")
        ItemTree.Nodes.Add("item402511494", "Aged gray pattern Titanium")
        ItemTree.Nodes.Add("item402511494", "Gray pattern Titanium")
        ItemTree.Nodes.Add("item402511494", "Black pattern Titanium")
        ItemTree.Nodes.Add("item402511494", "Galvanized white Titanium")
        ItemTree.Nodes.Add("item402511494", "Galvanized light gray Titanium")
        ItemTree.Nodes.Add("item402511494", "Galvanized dark gray Titanium")
        ItemTree.Nodes.Add("item402511494", "Galvanized gray Titanium")
        ItemTree.Nodes.Add("item402511494", "Galvanized black Titanium")
        ItemTree.Nodes.Add("item402511494", "Polished white Titanium")
        ItemTree.Nodes.Add("item402511494", "Polished light gray Titanium")
        ItemTree.Nodes.Add("item402511494", "Polished dark gray Titanium")
        ItemTree.Nodes.Add("item402511494", "Polished gray Titanium")
        ItemTree.Nodes.Add("item402511494", "Polished black Titanium")

        ItemTree.Nodes.Add("item2085561075", "White pattern iron")
        ItemTree.Nodes.Add("item2085561075", "Stained gray pattern iron")
        ItemTree.Nodes.Add("item2085561075", "Aged gray pattern iron")
        ItemTree.Nodes.Add("item2085561075", "Gray pattern iron")
        ItemTree.Nodes.Add("item2085561075", "Black pattern iron")
        ItemTree.Nodes.Add("item2085561075", "Galvanized white iron")
        ItemTree.Nodes.Add("item2085561075", "Galvanized light gray iron")
        ItemTree.Nodes.Add("item2085561075", "Galvanized dark gray iron")
        ItemTree.Nodes.Add("item2085561075", "Galvanized gray iron")
        ItemTree.Nodes.Add("item2085561075", "Galvanized black iron")
        ItemTree.Nodes.Add("item2085561075", "Polished white iron")
        ItemTree.Nodes.Add("item2085561075", "Polished light gray iron")
        ItemTree.Nodes.Add("item2085561075", "Polished dark gray iron")
        ItemTree.Nodes.Add("item2085561075", "Polished gray iron")
        ItemTree.Nodes.Add("item2085561075", "Polished black iron")

        ItemTree.Nodes.Add("item2892111312", "White pattern Gold")
        ItemTree.Nodes.Add("item2892111312", "Stained gray pattern Gold")
        ItemTree.Nodes.Add("item2892111312", "Aged gray pattern Gold")
        ItemTree.Nodes.Add("item2892111312", "Gray pattern Gold")
        ItemTree.Nodes.Add("item2892111312", "Black pattern Gold")
        ItemTree.Nodes.Add("item2892111312", "Galvanized white Gold")
        ItemTree.Nodes.Add("item2892111312", "Galvanized light gray Gold")
        ItemTree.Nodes.Add("item2892111312", "Galvanized dark gray Gold")
        ItemTree.Nodes.Add("item2892111312", "Galvanized gray Gold")
        ItemTree.Nodes.Add("item2892111312", "Galvanized black Gold")
        ItemTree.Nodes.Add("item2892111312", "Polished white Gold")
        ItemTree.Nodes.Add("item2892111312", "Polished light gray Gold")
        ItemTree.Nodes.Add("item2892111312", "Polished dark gray Gold")
        ItemTree.Nodes.Add("item2892111312", "Polished gray Gold")
        ItemTree.Nodes.Add("item2892111312", "Polished black Gold")

        ItemTree.Nodes.Add("item1374916603", "Polished gray Copper")
        ItemTree.Nodes.Add("item1374916603", "Polished black Copper")
        ItemTree.Nodes.Add("item1374916603", "White pattern Copper")
        ItemTree.Nodes.Add("item1374916603", "Stained gray pattern Copper")
        ItemTree.Nodes.Add("item1374916603", "Aged gray pattern Copper")
        ItemTree.Nodes.Add("item1374916603", "Gray pattern Copper")
        ItemTree.Nodes.Add("item1374916603", "Black pattern Copper")
        ItemTree.Nodes.Add("item1374916603", "Galvanized white Copper")
        ItemTree.Nodes.Add("item1374916603", "Galvanized light gray Copper")
        ItemTree.Nodes.Add("item1374916603", "Aged gray Copper")
        ItemTree.Nodes.Add("item1374916603", "Galvanized gray Copper")
        ItemTree.Nodes.Add("item1374916603", "Galvanized black Copper")
        ItemTree.Nodes.Add("item1374916603", "Polished white Copper")
        ItemTree.Nodes.Add("item1374916603", "Polished light gray Copper")
        ItemTree.Nodes.Add("item1374916603", "Polished dark gray Copper")

        ItemTree.Nodes.Add("item1406093224", "White pattern Chromium")
        ItemTree.Nodes.Add("item1406093224", "Stained gray pattern Chromium")
        ItemTree.Nodes.Add("item1406093224", "Aged gray pattern Chromium")
        ItemTree.Nodes.Add("item1406093224", "Gray pattern Chromium")
        ItemTree.Nodes.Add("item1406093224", "Black pattern Chromium")
        ItemTree.Nodes.Add("item1406093224", "Galvanized white Chromium")
        ItemTree.Nodes.Add("item1406093224", "Galvanized light gray Chromium")
        ItemTree.Nodes.Add("item1406093224", "Galvanized dark gray Chromium")
        ItemTree.Nodes.Add("item1406093224", "Galvanized gray Chromium")
        ItemTree.Nodes.Add("item1406093224", "Galvanized black Chromium")
        ItemTree.Nodes.Add("item1406093224", "Polished white Chromium")
        ItemTree.Nodes.Add("item1406093224", "Polished light gray Chromium")
        ItemTree.Nodes.Add("item1406093224", "Polished dark gray Chromium")
        ItemTree.Nodes.Add("item1406093224", "Polished gray Chromium")
        ItemTree.Nodes.Add("item1406093224", "Polished black Chromium")

        ItemTree.Nodes.Add("item123493466", "White aluminium pattern")
        ItemTree.Nodes.Add("item123493466", "Stained gray pattern aluminium")
        ItemTree.Nodes.Add("item123493466", "Aged gray pattern aluminium")
        ItemTree.Nodes.Add("item123493466", "Y aluminium pattern")
        ItemTree.Nodes.Add("item123493466", "Black pattern aluminium")
        ItemTree.Nodes.Add("item123493466", "Galvanized white aluminium")
        ItemTree.Nodes.Add("item123493466", "Galvanized light gray aluminium")
        ItemTree.Nodes.Add("item123493466", "Galvanized dark gray aluminium")
        ItemTree.Nodes.Add("item123493466", "Galvanized gray aluminium")
        ItemTree.Nodes.Add("item123493466", "Galvanized black aluminium")
        ItemTree.Nodes.Add("item123493466", "Polished white aluminium")
        ItemTree.Nodes.Add("item123493466", "Polished light gray aluminium")
        ItemTree.Nodes.Add("item123493466", "Polished dark gray aluminium")
        ItemTree.Nodes.Add("item123493466", "Polished gray aluminium")
        ItemTree.Nodes.Add("item123493466", "Polished black aluminium")

        ItemTree.Nodes.Add("item3729464850", "Catalyst 5")
        ItemTree.Nodes.Add("item3729464849", "Catalyst 4")
        ItemTree.Nodes.Add("item3729464848", "Catalyst 3")

        ItemTree.Nodes.Add("item840202987", "Kergon-X4 fuel")
        ItemTree.Nodes.Add("item840202986", "Kergon-X3 fuel")
        ItemTree.Nodes.Add("item840202981", "Kergon-X2 fuel")
        ItemTree.Nodes.Add("item840202980", "Kergon-X1 fuel")

        ItemTree.Nodes.Add("item106455050", "Xeron Fuel")

        ItemTree.Nodes.Add("item2579672037", "Nitron Fuel")


        ItemTree.Nodes.Add("item2277755297", "Railgun Electromagnetic Ammo S")
        ItemTree.Nodes.Add("item3511898141", "Railgun Precision Electromagnetic Ammo S")
        ItemTree.Nodes.Add("item3384068103", "Railgun Heavy Electromagnetic Ammo S")
        ItemTree.Nodes.Add("item2890607046", "Railgun Defense Electromagnetic Ammo S")
        ItemTree.Nodes.Add("item1818470694", "Railgun Agile Electromagnetic Ammo S")
        ItemTree.Nodes.Add("item2665059784", "Railgun Antimatter Ammo S")
        ItemTree.Nodes.Add("item2423442023", "Railgun Precision Antimatter Ammo S")
        ItemTree.Nodes.Add("item2944291964", "Railgun Heavy Antimatter Ammo S")
        ItemTree.Nodes.Add("item2454971316", "Railgun Defense Antimatter Ammo S")
        ItemTree.Nodes.Add("item2765153031", "Railgun Agile Antimatter Ammo S")
        ItemTree.Nodes.Add("item1314738719", "Railgun Electromagnetic Ammo M")
        ItemTree.Nodes.Add("item3778585474", "Railgun Precision Electromagnetic Ammo M")
        ItemTree.Nodes.Add("item711588165", "Railgun Heavy Electromagnetic Ammo M")
        ItemTree.Nodes.Add("item2547387530", "Railgun Defense Electromagnetic Ammo M")
        ItemTree.Nodes.Add("item2401068335", "Railgun Agile Electromagnetic Ammo M")
        ItemTree.Nodes.Add("item3025930763", "Railgun Antimatter Ammo M")
        ItemTree.Nodes.Add("item1378313789", "Railgun Precision Antimatter Ammo M")
        ItemTree.Nodes.Add("item1129867076", "Railgun Heavy Antimatter Ammo M")
        ItemTree.Nodes.Add("item2519489329", "Railgun Defense Antimatter Ammo M")
        ItemTree.Nodes.Add("item2753235550", "Railgun Agile Antimatter Ammo M")
        ItemTree.Nodes.Add("item19332250", "Railgun Electromagnetic Ammo L")
        ItemTree.Nodes.Add("item3711223735", "Railgun Precision Electromagnetic Ammo L")
        ItemTree.Nodes.Add("item985599166", "Railgun Heavy Electromagnetic Ammo L")
        ItemTree.Nodes.Add("item2997406270", "Railgun Defense Electromagnetic Ammo L")
        ItemTree.Nodes.Add("item493646316", "Railgun Agile Electromagnetic Ammo L")
        ItemTree.Nodes.Add("item4091052814", "Railgun Antimatter Ammo L")
        ItemTree.Nodes.Add("item2009039852", "Railgun Precision Antimatter Ammo L")
        ItemTree.Nodes.Add("item1555786609", "Railgun Heavy Antimatter Ammo L")
        ItemTree.Nodes.Add("item1377917611", "Railgun Defense Antimatter Ammo L")
        ItemTree.Nodes.Add("item994404082", "Railgun Agile Antimatter Ammo L")
        ItemTree.Nodes.Add("item2513950249", "Railgun Electromagnetic Ammo XS")
        ItemTree.Nodes.Add("item2661753045", "Railgun Precision Electromagnetic Ammo XS")
        ItemTree.Nodes.Add("item671997275", "Railgun Heavy Electromagnetic Ammo XS")
        ItemTree.Nodes.Add("item1190298485", "Railgun Defense Electromagnetic Ammo XS")
        ItemTree.Nodes.Add("item4121476880", "Railgun Agile Electromagnetic Ammo XS")
        ItemTree.Nodes.Add("item3669030673", "Railgun Antimatter Ammo XS")
        ItemTree.Nodes.Add("item2897347844", "Railgun Precision Antimatter Ammo XS")
        ItemTree.Nodes.Add("item2975180925", "Railgun Heavy Antimatter Ammo XS")
        ItemTree.Nodes.Add("item1685710165", "Railgun Defense Antimatter Ammo XS")
        ItemTree.Nodes.Add("item2562077926", "Railgun Agile Antimatter Ammo XS")
        ItemTree.Nodes.Add("item108337911", "Missile Kinetic Ammo S")
        ItemTree.Nodes.Add("item2116379443", "Missile Precision Kinetic Ammo S")
        ItemTree.Nodes.Add("item578039658", "Missile Heavy Kinetic Ammo S")
        ItemTree.Nodes.Add("item1256805327", "Missile Defense Kinetic Ammo S")
        ItemTree.Nodes.Add("item2679053199", "Missile Agile Kinetic Ammo S")
        ItemTree.Nodes.Add("item2425505244", "Missile Antimatter Ammo S")
        ItemTree.Nodes.Add("item2982583326", "Missile Precision Antimatter Ammo S")
        ItemTree.Nodes.Add("item1333805710", "Missile Heavy Antimatter Ammo S")
        ItemTree.Nodes.Add("item116711443", "Missile Defense Antimatter Ammo S")
        ItemTree.Nodes.Add("item1284945646", "Missile Agile Antimatter Ammo S")
        ItemTree.Nodes.Add("item3718373809", "Missile Kinetic Ammo M")
        ItemTree.Nodes.Add("item871384738", "Missile Precision Kinetic Ammo M")
        ItemTree.Nodes.Add("item1209270788", "Missile Heavy Kinetic Ammo M")
        ItemTree.Nodes.Add("item397326901", "Missile Defense Kinetic Ammo M")
        ItemTree.Nodes.Add("item1491281175", "Missile Agile Kinetic Ammo M")
        ItemTree.Nodes.Add("item403006216", "Missile Antimatter Ammo M")
        ItemTree.Nodes.Add("item144252385", "Missile Precision Antimatter Ammo M")
        ItemTree.Nodes.Add("item291497016", "Missile Heavy Antimatter Ammo M")
        ItemTree.Nodes.Add("item3987182986", "Missile Defense Antimatter Ammo M")
        ItemTree.Nodes.Add("item326385703", "Missile Agile Antimatter Ammo M")
        ItemTree.Nodes.Add("item934893004", "Missile Kinetic Ammo L")
        ItemTree.Nodes.Add("item897887498", "Missile Precision Kinetic Ammo L")
        ItemTree.Nodes.Add("item3073125595", "Missile Heavy Kinetic Ammo L")
        ItemTree.Nodes.Add("item1186613579", "Missile Defense Kinetic Ammo L")
        ItemTree.Nodes.Add("item2529340738", "Missile Agile Kinetic Ammo L")
        ItemTree.Nodes.Add("item995805029", "Missile Antimatter Ammo L")
        ItemTree.Nodes.Add("item3164761417", "Missile Precision Antimatter Ammo L")
        ItemTree.Nodes.Add("item3376140874", "Missile Heavy Antimatter Ammo L")
        ItemTree.Nodes.Add("item579968086", "Missile Defense Antimatter Ammo L")
        ItemTree.Nodes.Add("item3594012056", "Missile Agile Antimatter Ammo L")
        ItemTree.Nodes.Add("item2392386214", "Missile Kinetic Ammo XS")
        ItemTree.Nodes.Add("item1503181393", "Missile Precision Kinetic Ammo XS")
        ItemTree.Nodes.Add("item2591026571", "Missile Heavy Kinetic Ammo XS")
        ItemTree.Nodes.Add("item3939368391", "Missile Defense Kinetic Ammo XS")
        ItemTree.Nodes.Add("item2148925933", "Missile Agile Kinetic Ammo XS")
        ItemTree.Nodes.Add("item2845912456", "Missile Antimatter Ammo XS")
        ItemTree.Nodes.Add("item2239958675", "Missile Precision Antimatter Ammo XS")
        ItemTree.Nodes.Add("item1154972320", "Missile Heavy Antimatter Ammo XS")
        ItemTree.Nodes.Add("item2059964042", "Missile Defense Antimatter Ammo XS")
        ItemTree.Nodes.Add("item2340151566", "Missile Agile Antimatter Ammo XS")
        ItemTree.Nodes.Add("item1363871248", "Laser Thermic Ammo S")
        ItemTree.Nodes.Add("item3820970963", "Laser Precision Thermic Ammo S")
        ItemTree.Nodes.Add("item1750052574", "Laser Heavy Thermic Ammo S")
        ItemTree.Nodes.Add("item1933474332", "Laser Defense Thermic Ammo S")
        ItemTree.Nodes.Add("item3423590348", "Laser Agile Thermic Ammo S")
        ItemTree.Nodes.Add("item1921694649", "Laser Electromagnetic Ammo S")
        ItemTree.Nodes.Add("item4088065384", "Laser Precision Electromagnetic Ammo S")
        ItemTree.Nodes.Add("item1929049234", "Laser Heavy Electromagnetic Ammo S")
        ItemTree.Nodes.Add("item2667876309", "Laser Defense Electromagnetic Ammo S")
        ItemTree.Nodes.Add("item3098134459", "Laser Agile Electromagnetic Ammo S")
        ItemTree.Nodes.Add("item2843836124", "Laser Thermic Ammo M")
        ItemTree.Nodes.Add("item3708417017", "Laser Precision Thermic Ammo M")
        ItemTree.Nodes.Add("item984810201", "Laser Heavy Thermic Ammo M")
        ItemTree.Nodes.Add("item1230483435", "Laser Defense Thermic Ammo M")
        ItemTree.Nodes.Add("item212874547", "Laser Agile Thermic Ammo M")
        ItemTree.Nodes.Add("item1693315392", "Laser Electromagnetic Ammo M")
        ItemTree.Nodes.Add("item1610308198", "Laser Precision Electromagnetic Ammo M")
        ItemTree.Nodes.Add("item220854647", "Laser Heavy Electromagnetic Ammo M")
        ItemTree.Nodes.Add("item483699778", "Laser Defense Electromagnetic Ammo M")
        ItemTree.Nodes.Add("item2948970732", "Laser Agile Electromagnetic Ammo M")
        ItemTree.Nodes.Add("item1068250257", "Laser Thermic Ammo L")
        ItemTree.Nodes.Add("item36119774", "Laser Precision Thermic Ammo L")
        ItemTree.Nodes.Add("item518572846", "Laser Heavy Thermic Ammo L")
        ItemTree.Nodes.Add("item2619099776", "Laser Defense Thermic Ammo L")
        ItemTree.Nodes.Add("item154196902", "Laser Agile Thermic Ammo L")
        ItemTree.Nodes.Add("item2465107224", "Laser Electromagnetic Ammo L")
        ItemTree.Nodes.Add("item1664787227", "Laser Precision Electromagnetic Ammo L")
        ItemTree.Nodes.Add("item2281477958", "Laser Heavy Electromagnetic Ammo L")
        ItemTree.Nodes.Add("item2006239134", "Laser Defense Electromagnetic Ammo L")
        ItemTree.Nodes.Add("item2170035253", "Laser Agile Electromagnetic Ammo L")
        ItemTree.Nodes.Add("item4135531540", "Laser Thermic Ammo XS")
        ItemTree.Nodes.Add("item1765328811", "Laser Precision Thermic Ammo XS")
        ItemTree.Nodes.Add("item2678465305", "Laser Heavy Thermic Ammo XS")
        ItemTree.Nodes.Add("item839159661", "Laser Defense Thermic Ammo XS")
        ItemTree.Nodes.Add("item570530668", "Laser Agile Thermic Ammo XS")
        ItemTree.Nodes.Add("item3637130597", "Laser Electromagnetic Ammo XS")
        ItemTree.Nodes.Add("item3539993652", "Laser Precision Electromagnetic Ammo XS")
        ItemTree.Nodes.Add("item902792933", "Laser Heavy Electromagnetic Ammo XS")
        ItemTree.Nodes.Add("item1067471403", "Laser Defense Electromagnetic Ammo XS")
        ItemTree.Nodes.Add("item552630719", "Laser Agile Electromagnetic Ammo XS")
        ItemTree.Nodes.Add("item3818049598", "Cannon Kinetic Ammo XS")
        ItemTree.Nodes.Add("item3238320397", "Cannon Precision Kinetic Ammo XS")
        ItemTree.Nodes.Add("item1980351716", "Cannon Heavy Kinetic Ammo XS")
        ItemTree.Nodes.Add("item2680492642", "Cannon Defense Kinetic Ammo XS")
        ItemTree.Nodes.Add("item2746947552", "Cannon Agile Kinetic Ammo XS")
        ItemTree.Nodes.Add("item3607061517", "Cannon Thermic Ammo XS")
        ItemTree.Nodes.Add("item2917884317", "Cannon Precision Thermic Ammo XS")
        ItemTree.Nodes.Add("item726551231", "Cannon Heavy Thermic Ammo XS")
        ItemTree.Nodes.Add("item147467923", "Cannon Defense Thermic Ammo XS")
        ItemTree.Nodes.Add("item370579567", "Cannon Agile Thermic Ammo XS")
        ItemTree.Nodes.Add("item2542033786", "Metal Throne")
        ItemTree.Nodes.Add("item392866463", "Golden Throne")
        ItemTree.Nodes.Add("item536277576", "Obsidian Throne")
    End Sub

    Private Sub CheckForUpdates()
        Dim UpdateCheckCurrent As String = API_Request("http://duopenmarket.xyz/openmarketapi.php/version?").Trim("""")
        If UpdateCheckCurrent = API_Client_Version Then
            'Do nothing, we are up to date.
        Else
            'Current version and running version do not match.
            'We need to now get the minimum supported client version from the server, and check if the running version is greater than that.
            Dim forceupdflag As Boolean = False
            Dim UpdateCheckMinimum As String = API_Request("http://duopenmarket.xyz/openmarketapi.php/minversion?").Trim("""")
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
        LoginUpdateText.Text = "An update for DUOpenMarket is required. Download?"
    End Sub

    Private Sub UpdateButton1_Click(sender As Object, e As EventArgs) Handles UpdateButton1.Click
        'Simple self-update subroutine. First we'll get the binary contents from github, and save it to a temporary text file.
        'If the download isnt interrupted and succeeds, we will need to close the current instance of the program and launch the new one after renaming it to an exe.
        'The easiest way to do this is with a simple batch script, which is created via the streamwriter.
        'In batch, there's no simple command to wait a period of time without requiring the user to press a key...
        'To get around this we can use "Ping localhost -n X >NUL" where X is the number of seconds we want to wait, +1.
        'This is necessary to make sure the download and disk operations are complete before proceeding to the next command in the batch file.
        'May not be adequate on slower systems. Testing required.
        My.Computer.FileSystem.WriteAllBytes(My.Application.Info.DirectoryPath & "\DUOpenMarket Client.temp", GetURLDataBin("https://github.com/Jason-Bloomer/DUOpenMarket/releases/download/v0.41.1/DUOpenMarket.Client.exe"), False)
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
    End Sub

    Private Sub UpdateButton2_Click(sender As Object, e As EventArgs) Handles UpdateButton2.Click
        Dim manualdownload As Process = Process.Start("https://github.com/Jason-Bloomer/DUOpenMarket/releases/")
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

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button11.Click
        AboutForm.Label6.Text = "Desktop Client v" & API_Client_Version
        AboutForm.Show()
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        If WindowMaximizedState = True Then
            Me.WindowState = FormWindowState.Normal
            WindowMaximizedState = False
            Dim newbnds As Size = New Size()
            newbnds.Height = WindowNormalBoundsY
            newbnds.Width = WindowNormalBoundsX
            MainPanel.Size = newbnds
        Else
            Me.WindowState = FormWindowState.Maximized
            WindowMaximizedState = True
            Dim currScreen As Screen = Screen.FromControl(Me)
            Dim newbnds As Size = New Size()
            newbnds.Height = currScreen.WorkingArea.Height
            newbnds.Width = currScreen.WorkingArea.Width
            MainPanel.Size = newbnds
        End If
        CenterLoginElements()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Me.WindowState = FormWindowState.Minimized
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        SavePrefsToIni()
        Application.Exit()
        Me.Dispose()
    End Sub

    Private Sub TitleBar_MouseUp(ByVal sender As Object, ByVal e As MouseEventArgs) Handles TitleBarPanel.MouseUp
        Go = False
        LeftSet = False
        TopSet = False
    End Sub

    Private Sub TitleBar_MouseDown(ByVal sender As Object, ByVal e As MouseEventArgs) Handles TitleBarPanel.MouseDown
        Go = True
    End Sub

    Private Sub TitleBar_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs) Handles TitleBarPanel.MouseMove
        If Go = True Then

            If Me.WindowState = FormWindowState.Maximized Then
                Me.WindowState = FormWindowState.Normal
                WindowMaximizedState = False
            End If
            HoldLeft = (Control.MousePosition.X)
            HoldTop = (Control.MousePosition.Y)
            If TopSet = False Then
                OffTop = HoldTop - sender.Parent.Parent.Location.Y
                TopSet = True
            End If
            If LeftSet = False Then
                OffLeft = HoldLeft - sender.Parent.Parent.Location.X
                LeftSet = True
            End If
            'reset size if maximized, to whatever size we were before maximizing
            Dim newbnds As Size = New Size()
            If Me.WindowState = FormWindowState.Normal Then
                newbnds.Height = WindowNormalBoundsY
                newbnds.Width = WindowNormalBoundsX
                MainPanel.Size = newbnds
            Else
                newbnds.Height = Screen.PrimaryScreen.Bounds.Height
                newbnds.Width = Screen.PrimaryScreen.Bounds.Width
                MainPanel.Size = newbnds
            End If
            'define where the window has been dragged to
            Dim newpoint As New Point
            newpoint.X = HoldLeft - OffLeft
            newpoint.Y = HoldTop - OffTop
            Dim AllScreens() As Screen = Screen.AllScreens
            Dim XwithinBounds As Boolean = False
            Dim YwithinBounds As Boolean = False
            Dim WwithinBounds As Boolean = False
            Dim HwithinBounds As Boolean = False
            For Each displayScreen As Screen In AllScreens
                'check if that point would put any part of the window off screen, and if so, correct
                If newpoint.X > displayScreen.WorkingArea.X Then
                    XwithinBounds = True
                End If
                If (newpoint.X + newbnds.Width) < (displayScreen.WorkingArea.X + displayScreen.WorkingArea.Width) Then
                    WwithinBounds = True
                End If

                If newpoint.Y > displayScreen.WorkingArea.Y Then
                    YwithinBounds = True
                End If
                If (newpoint.Y + newbnds.Height) < (displayScreen.WorkingArea.Y + displayScreen.WorkingArea.Height) Then
                    HwithinBounds = True
                End If
            Next displayScreen
            If XwithinBounds = True And WwithinBounds = True Then
                newpoint.X = HoldLeft - OffLeft
            Else
                newpoint.X = sender.Parent.Parent.Location.X
            End If

            If YwithinBounds = True And HwithinBounds = True Then
                newpoint.Y = HoldTop - OffTop
            Else
                newpoint.Y = sender.Parent.Parent.Location.Y
            End If
            sender.Parent.Parent.Location = newpoint
            sender.Parent.Parent.Refresh()
            CenterLoginElements()
        End If
    End Sub

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

    Private Sub CenterLoginElements()
        If LoginPanel.Visible = True Then
            Dim temp1 As Point
            Dim temp2 As Point
            Dim temp3 As Point
            Dim temp4 As Point
            Dim temp5 As Point
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
            LoginLabel2.Text = "Desktop Client - v" + API_Client_Version
        End If
    End Sub

    Private Sub ConnectionStyling()
        If API_Connected = False Then
            DiscordLoginButton.Text = "Login With Discord"
            ConnectionLabel.Text = "Not Connected"
            ConnectionPanel.BackColor = Color.FromArgb(255, 215, 65, 65)
            ItemTree.Enabled = False
            ItemTreeSearch.Enabled = False
            MarketPanel.Visible = False
            LoginPanel.Visible = True
        Else
            DiscordLoginButton.Text = "Disconnect"
            ConnectionLabel.Text = "Connected"
            ConnectionPanel.BackColor = Color.FromArgb(255, 65, 215, 65)
            ItemTree.Enabled = True
            ItemTreeSearch.Enabled = True
            MarketPanel.Visible = True
            LoginPanel.Visible = False
        End If
    End Sub

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

    Private Sub ParseConsoleInput(ByVal inputstr As String)
        NewEventMsg("user@console_>: " & inputstr)
        Dim match As Boolean = False
        If inputstr = "debug" Then
            match = True
            NewEventMsg(Environment.NewLine)
            NewEventMsg("---------- DUOpenMarket Variable Dump ----------")
            NewEventMsg("API_Connected = " & CStr(API_Connected))
            NewEventMsg("API_LogfileDirectory = " & CStr(API_LogfileDirectory))
            NewEventMsg("API_LogFile = " & CStr(API_LogFile))
            NewEventMsg("API_Discord_Auth_Code = " & CStr(API_Discord_Auth_Code))
            NewEventMsg("API_Discord_Auth_State = " & CStr(API_Discord_Auth_State))
            NewEventMsg("API_Last_Log_Processed = " & CStr(API_Last_Log_Processed))
            NewEventMsg("File Buffer Offset = " & CStr(_LastOffset))
            NewEventMsg("LastFileModified = " & CStr(LastFileModified))
            NewEventMsg("NumberofModifications = " & CStr(NumberofModifications))
            NewEventMsg("LogfileLastOffset = " & CStr(LogfileLastOffset))
            NewEventMsg("WindowMaximizedState = " & CStr(WindowMaximizedState))
            NewEventMsg("WindowNormalBoundsX = " & CStr(WindowNormalBoundsX))
            NewEventMsg("WindowNormalBoundsY = " & CStr(WindowNormalBoundsY))
            NewEventMsg("Showing Raw Data = " & CStr(ShowRawData))
            NewEventMsg("API Read Requests Sent = " & CStr(NumberOfReads))
            NewEventMsg("API Update Requests Sent = " & CStr(NumberOfUpdates))
            NewEventMsg("API Create Requests Sent = " & CStr(NumberOfCreates))
            NewEventMsg("API Delete Requests Sent = 0")
            NewEventMsg("Last Item ID From Logs = " & CStr(LastItemId))
            NewEventMsg("------------- End Variable Dump ------------")
        End If
        If inputstr = "help" Then
            match = True
            NewEventMsg(Environment.NewLine)
            NewEventMsg("---------- DUOpenMarket Console Help ----------")
            NewEventMsg("help   |  Prints this dialogue. D'oh!")
            NewEventMsg("debug  |  Prints the current values of all internal global variables and constants.")
            NewEventMsg("dev    |  Toggles the developer control panel's visibility.")
            NewEventMsg("raw    |  Toggles between showing user-readable order data, or raw unmodified data.")
            NewEventMsg("------------- End Console Help ------------")
        End If
        If inputstr = "dev" Then
            If ShowDevPanel = False Then
                DeveloperPanel.Visible = True
                DeveloperPanel.BringToFront()
                NewEventMsg("Showing Developer Panel.")
                ShowDevPanel = True
            Else
                DeveloperPanel.Visible = False
                DeveloperPanel.SendToBack()
                NewEventMsg("Hiding Developer Panel.")
                ShowDevPanel = False
            End If
            match = True
        End If
        If inputstr = "raw" Then
            match = True
            If ShowRawData = False Then
                ShowRawData = True
                NewEventMsg("Showing raw API data. Type 'raw' again to go back to user-readable.")
            Else
                ShowRawData = False
                NewEventMsg("Showing user-readable data.")
            End If
        End If
        If inputstr = "lastid" Then
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
        If match = False Then
            NewEventMsg("Unknown command. Try again or type ""help"" for commands.")
        End If
    End Sub

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
                    'do nothing
                Else
                    API_Last_Log_Processed = InputString
                    InputString = InputString.Remove(0, InputString.IndexOf("MarketOrder:") + 12) 'Trim start
                    'This checks for the case where there is zero orders for a given item.
                    Try
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
                    Catch err2 As Exception
                        NewEventMsg(err2.Message)
                    End Try
                End If
            End If
        End If
    End Sub

    Private Function API_Request(ByVal addr As String)
        Try
            Dim requestUrl As String = addr
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

    Private Sub NewEventMsg(ByVal input As String)
        ConsoleTextBox.AppendText(Environment.NewLine & input)
        ConsoleTextBox.Select(ConsoleTextBox.TextLength, 0)
        ConsoleTextBox.ScrollToCaret()
    End Sub

    Private Sub FileSystemWatcher1_Changed(ByVal sender As Object,
        ByVal e As System.IO.FileSystemEventArgs) Handles _
        FileSystemWatcher1.Changed
        'NewEventMsg("MODIFIED " & e.FullPath)
    End Sub

    Private Sub FileSystemWatcher1_Created(ByVal sender As Object,
        ByVal e As System.IO.FileSystemEventArgs) Handles _
        FileSystemWatcher1.Created
        'NewEventMsg("CREATED " & e.FullPath)
        Dim lfprefix As String = e.FullPath.Remove(0, e.FullPath.LastIndexOf("\"))
        If lfprefix.StartsWith("log_") = True Then
            If lfprefix.EndsWith(".xml") = True Then
                API_LogFile = e.FullPath
            End If
        End If
    End Sub

    Private Sub FileSystemWatcher1_Deleted(ByVal sender As Object,
        ByVal e As System.IO.FileSystemEventArgs) Handles _
        FileSystemWatcher1.Deleted
        'NewEventMsg("DELETED " & e.FullPath)
    End Sub

    Private Sub FileSystemWatcher1_Renamed(ByVal sender As Object, ByVal e As System.IO.RenamedEventArgs) Handles _
        FileSystemWatcher1.Renamed
        'NewEventMsg("RENAMED " & vbTab & e.OldFullPath & " TO " & e.FullPath)
    End Sub

    Private Sub FileSystemWatcher1_Error(ByVal sender As Object,
        ByVal e As System.IO.ErrorEventArgs) Handles _
        FileSystemWatcher1.Error
        If FileSystemWatcher1.EnableRaisingEvents Then
            FileSystemWatcher1.EnableRaisingEvents = False
            FileSystemWatcher1.InternalBufferSize = 2 *
                    FileSystemWatcher1.InternalBufferSize
            FileSystemWatcher1.EnableRaisingEvents = True
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

    Private Sub ProcessNextInQueue()
        Dim OrderType As Integer
        If Convert.ToInt64(API_Log_Queue(0).quantity) > 0 Then
            OrderType = 1
        Else
            OrderType = 2
        End If
        NumberOfReads = NumberOfReads + 1
        Dim TempResponse2 As String = API_Request("http://duopenmarket.xyz/openmarketapi.php/" & API_Discord_Auth_Code & "/read?orderid=" & API_Log_Queue(0).orderid)
        If TempResponse2 = "false" Then
            'if the server doesnt have this order, then we create it. what could go wrong?
            NumberOfCreates = NumberOfCreates + 1
            Dim TempResponse3 As String = API_Request("http://duopenmarket.xyz/openmarketapi.php/" & API_Discord_Auth_Code & "/create?orderid=" & API_Log_Queue(0).orderid & "&marketid=" & API_Log_Queue(0).marketid & "&itemid=" & API_Log_Queue(0).itemtype & "&quantity=" & API_Log_Queue(0).quantity & "&ordertype=" & OrderType & "&expiration=" & API_Log_Queue(0).expdate & "&lastupdated=" & API_Log_Queue(0).lastupdate & "&price=" & API_Log_Queue(0).price & "&creator=" & API_Username)
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
                            NumberOfUpdates = NumberOfUpdates + 1
                            Dim TempResponse3 As String = API_Request("http://duopenmarket.xyz/openmarketapi.php/" & API_Discord_Auth_Code & "/update?orderid=" & API_Log_Queue(0).orderid & "&marketid=" & API_Log_Queue(0).marketid & "&itemid=" & API_Log_Queue(0).itemtype & "&quantity=" & API_Log_Queue(0).quantity & "&ordertype=" & OrderType & "&expiration=" & API_Log_Queue(0).expdate & "&lastupdated=" & API_Log_Queue(0).lastupdate & "&price=" & API_Log_Queue(0).price)
                        End If
                    End If
                End If
            End If
        End If
        StatLabelProc.Text = CInt(StatLabelProc.Text) + 1
        API_Log_Queue.RemoveAt(0)
    End Sub

    Private Sub OperationTimer_Tick(sender As Object, e As EventArgs) Handles OperationTimer.Tick
        StatLabelQue.Text = API_Log_Queue.Count()
        API_LogFile = GetNewestLogFile(API_LogfileDirectory)
        If API_LogFile IsNot Nothing And API_LogFile IsNot "" Then
            Dim fileinfo1 As FileInfo = My.Computer.FileSystem.GetFileInfo(API_LogFile)
            If fileinfo1.Length > CInt(_LastOffset) Then
                ReadFileLines(API_LogFile)
            End If
            If API_Log_Queue.Count >= 1 Then
                GetLastItemId()
                ProcessNextInQueue()
            End If
        End If
        ConnectionStyling()
        LogBufferLabel.Text = CStr(_LastOffset)
        APIReadsLabel.Text = CStr(NumberOfReads)
        APIUpdatesLabel.Text = CStr(NumberOfUpdates)
        APICreatesLabel.Text = CStr(NumberOfCreates)
        'APIDeletesLabel.Text = CStr(NumberOfDeletes)
    End Sub

    Private Sub GetLastItemId()
        If CInt(StatLabelProc.Text) > 1 Then
            Dim seen As Boolean = False
            For item As Integer = 1 To UniqueItemIds.Count()
                If UniqueItemIds(item - 1) = API_Log_Queue(0).itemtype Then
                    seen = True
                End If
            Next item
            If seen = False Then
                UniqueItemIds.Add(CStr(API_Log_Queue(0).itemtype))
            End If
        Else
            UniqueItemIds.Add(API_Log_Queue(0).itemtype.ToString)
        End If
    End Sub

    Private Sub UpdateSelectedItem(ByVal selected As String)
        SelectedItemLabel.Text = selected
    End Sub

    Private Sub TreeView1_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles ItemTree.AfterSelect, ItemTreeSearch.AfterSelect
        If API_Connected = True Then
            Dim queryitemid As String = e.Node.Name.Remove(0, 4)
            NewEventMsg("Sending Read request for: " & e.Node.Text & " - ID: " & queryitemid)
            If e.Node.Name IsNot "" Then
                If e.Node.Name.StartsWith("item") Then
                    If e.Node.Name = "itemnil" Then
                        NewEventMsg("Unknown ID for item: " & e.Node.Text & "! - You can help us find it by placing a buy or sell order for one, and uploading the log via the client.")
                    Else
                        NumberOfReads = NumberOfReads + 1
                        Dim TempResponse4 As String = API_Request("http://duopenmarket.xyz/openmarketapi.php/" & API_Discord_Auth_Code & "/read?itemid=" & queryitemid)
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
                                    While TempResponse4.Length > 48
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
                                        End If
                                        If ordertype3 = 2 Then
                                            If quantity3.StartsWith("-") Then
                                                quantity3 = quantity3.Remove(0, 1)
                                            End If
                                            API_Sell_Orders.Rows.Add(marketid3, orderid3, itemid3, quantity3, price3, expdate3, lastupdated3)
                                            API_Sell_Orders_UI.Rows.Add(GetMarketName(marketid3), quantity3, price3String, GetTimeRemaining(expdate3, orderid3), e.Node.Text)
                                        End If
                                    End While
                                    ResetDataTables()
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

    Private Function GetMarketName(ByVal input As Integer)
        Dim result() As DataRow = API_Market_Table.Select("id = " & input)
        For Each row As DataRow In result
            Return row.Item(1)
        Next
    End Function

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
        If ShowFilters = True Then
            AdvItemTreeView.Nodes.Clear()
            For Each tn As TreeNode In ItemTree.Nodes
                If tn.Name.StartsWith("node") Then
                    'This is a category node, do not add it to list
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

    Private Sub TestDiscordLogin() Handles DiscordLoginButton.Click, DiscordLoginButton2.Click
        If API_Connected = False Then
            Dim Discord_Login_Page As String = "http://duopenmarket.xyz/discordclientGetAuthCode.php"
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

    Dim t As Thread

    Public Shared runServer As Boolean = True

    ' Delegate to allow cross-thread update of UI safely
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

        ListBox1.Invoke(_invokeControl, "Entering request processing loop", False)
        While runServer
            Dim result As IAsyncResult = listener.BeginGetContext(New AsyncCallback(AddressOf AsynchronousListenerCallback), listener)
            ' intermediate work can go on here while waiting for the asynchronous callback
            ' an asynchronous wait handle is used to prevent this thread from terminating
            ' while waiting for the asynchronous operation to complete.
            ListBox1.Invoke(_invokeControl, "Waiting for asyncronous request processing.", False)
            result.AsyncWaitHandle.WaitOne()
            ListBox1.Invoke(_invokeControl, "Asynchronous request processed.", False)
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
        Else
            Form1.ListBox1.Items.Add([text])
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

            'Dim _invokeControl As New InvokeControl(AddressOf InvokeUIThread)
            'ConsoleTextBox.Invoke(_invokeControl, API_Discord_Auth_Code, True)
            'ConsoleTextBox.Invoke(_invokeControl, API_Discord_Auth_State, True)

            ' Get the response object to send our confirmation.
            Dim response As HttpListenerResponse = context.Response
            ' Construct a minimal response string.
            Dim responseString As String = "<HTML onload=""self.close()"" onfocus=""self.close()"" onclick=""self.close()""><BODY onload=""self.close()"" onfocus=""self.close()"" onclick=""self.close()""><script>setTimeout(function() {window.close();}, 50);</script><img style=""position:absolute;top:0;left:0;width:100%;height:100%"" src=""http://duopenmarket.xyz/assets/images/bg2.png""/><div style=""position:absolute;top:15%;left:27%;width:50%;height:35%;color:#FFFFFF;font-family:'Courier New';font-size:24px""><center><H1>DUOpenMarket - Authentication Successful</H1><br><br><br><br><H1>This window should close automatically.<br>You may close it if it does not.</H1></center></div></BODY></HTML>"
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

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        CreateListener()
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        StopListener()
    End Sub

    Private Sub LoginTimer_Tick(sender As Object, e As EventArgs) Handles LoginTimer.Tick
        If t IsNot Nothing Then
            NewEventMsg("Login attempt timed out.")
            StopListener()
        End If
    End Sub

    Private Sub ConnectionTimer_Tick(sender As Object, e As EventArgs) Handles ConnectionTimer.Tick
        If API_Discord_Auth_Code IsNot Nothing Then
            If API_LogfileDirectory.Trim.Length = 0 Then
                NewEventMsg("Invalid log path supplied for file monitoring.")
                ConnectionTimer.Stop()
                FileSystemWatcher1.EnableRaisingEvents = False
            Else
                If API_Discord_Auth_Code = "=access_denied" Then
                    NewEventMsg("The user cancelled authorization through discord.")
                    ConnectionTimer.Stop()
                    FileSystemWatcher1.EnableRaisingEvents = False
                Else
                    API_Connected = True
                    LoginPanel.Visible = False
                    NewEventMsg("Obtained Authorization code. Login successful.")
                    ConnectionStyling()
                    FileSystemWatcher1.Filter = API_LogfileDirectory.Trim
                    FileSystemWatcher1.Path = API_LogfileDirectory.Trim
                    FileSystemWatcher1.NotifyFilter =
                IO.NotifyFilters.CreationTime Or IO.NotifyFilters.LastWrite Or
                IO.NotifyFilters.LastAccess Or IO.NotifyFilters.FileName
                    FileSystemWatcher1.EnableRaisingEvents = True
                    NewEventMsg("Started Log Monitor.")
                    API_LogFile = GetNewestLogFile(API_LogfileDirectory)
                    NewEventMsg("Current Log File: " & API_LogFile)
                    ConnectionTimer.Stop()
                    ReadFileLines(API_LogFile)
                    OperationTimer.Start()
                End If
            End If
        End If
    End Sub

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
        End If
    End Sub

    Private Sub FilterOrdersButton_Click(sender As Object, e As EventArgs) Handles FilterOrdersButton.Click
        ResetAdvFilters()
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
        For Each filterItem As FilterItemListStructure In FilterItemList
            For Each filterMarket As FilterMarketListStructure In FilterMarketList
                If API_Connected = True Then
                    Dim queryitemid As String = filterItem.Name.Remove(0, 4)
                    NewEventMsg("Sending Read request for: " & filterItem.Text & " - ID: " & queryitemid)
                    If filterItem.Name IsNot "" Then
                        If filterItem.Name.StartsWith("item") Then
                            If filterItem.Name = "itemnil" Then
                                NewEventMsg("Unknown ID for item: " & filterItem.Text & "! - You can help us find it by placing a buy or sell order for one, and uploading the log via the client.")
                            Else
                                NumberOfReads = NumberOfReads + 1
                                Dim TempResponse4 As String = API_Request("http://duopenmarket.xyz/openmarketapi.php/" & API_Discord_Auth_Code & "/read?itemid=" & queryitemid & "&marketid=" & filterMarket.Name) ' & filterstring
                                If TempResponse4 Is Nothing Then
                                    NewEventMsg("Request failed!")
                                Else
                                    If TempResponse4.StartsWith("The remote server returned an error") Then
                                        NewEventMsg(TempResponse4)
                                    Else
                                        UpdateSelectedItem(filterItem.Text)
                                        If TempResponse4 = "false" Then
                                            NewEventMsg("Server returned no data for item.")
                                        Else
                                            Dim first As Boolean = True
                                            While TempResponse4.Length > 48
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
                                                    End If
                                                End If
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

    Private Sub FilterResetButton_Click(sender As Object, e As EventArgs) Handles FilterResetButton.Click
        ResetAdvFilterUI()
    End Sub

    Private Sub SettingsButton_Click(sender As Object, e As EventArgs) Handles SettingsButton.Click
        SettingsForm.Show()
    End Sub

    Private Sub BuyOrderGridViewRaw_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles BuyOrderGridViewRaw.CellClick
        SelectedItemLabel.Text = BuyOrderGridViewRaw.Rows(e.RowIndex).Cells(e.ColumnIndex).Value
    End Sub
End Class