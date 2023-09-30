


Imports System
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Management
Imports System.Net
Imports System.Net.Http
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading.Tasks
Imports Newtonsoft.Json


Public NotInheritable Class ManejadorDatos
    'Private Shared url As String
    'Private Shared request As HttpWebRequest
    Private Shared resultnh As ResultDTO
    Private Shared contador As Integer = 0
    Private _url As String
    Private _httpClient As HttpClient
    Private _errors As Conterrors

    Public Sub New()
        _url = "http://34.194.91.102:8080/"
        _httpClient = New HttpClient()
        _httpClient.BaseAddress = New Uri(_url)
        _httpClient.Timeout = New TimeSpan(0, 0, 30)
        _httpClient.DefaultRequestHeaders.Clear()

        If resultnh Is Nothing Then resultnh = New ResultDTO() With {.Isok = False}
    End Sub

    Public Function PostBitacora(_comando As String) As Boolean
        Try
            '#If DEBUG Then
            '                resultnh = New ResultDTO() With {
            '                    .msg = "validacion falsa",
            '                    .Isok = True
            '                }
            '                Return True
            '#End If
            If contador < 600 AndAlso resultnh.Isok Then
                contador += 1
                Return True
            Else
                contador = 0
                Task.Run(Function() PostBitacoraAsync(_comando)).Wait()
            End If
        Catch ex As Exception
            Debug.WriteLine($"error inscripc ex:{ex.Message}")
            Return False
        End Try

        Return True
    End Function

    Friend Function ObteneResultado() As Boolean
        Return resultnh.Isok
    End Function

    Friend Shared Function ObteneContador() As Integer
        Return contador
    End Function

    Public Async Function PostBitacoraAsync(_comando As String) As Threading.Tasks.Task(Of Task)
        Try
            Dim _InfoSystema As New InfoSystema_validar()
            _InfoSystema.M1_EjecutarInfoSistem()
            _InfoSystema.M3_getMacAddress3()

            Dim _usuariosDTO As New BitacoraDTO() With {
                .idMAC = _InfoSystema.mac.Replace(".", "_"),
                .iPUsuario = "cargado",
                .usuario = _InfoSystema.usuario,
                .comando = _comando
            }

            Dim jsonResul As String = JsonConvert.SerializeObject(_usuariosDTO)
            Dim httpContent As HttpContent = New StringContent(jsonResul, Encoding.UTF8, "application/json")
            Dim response As HttpResponseMessage = _httpClient.PostAsync("api/bitacora", httpContent).Result

            If response.IsSuccessStatusCode Then
                Dim result As HttpResponseMessage = response.EnsureSuccessStatusCode()
                Dim json As String = response.Content.ReadAsStringAsync().Result
                resultnh = JsonConvert.DeserializeObject(Of ResultDTO)(json)
            Else
                Dim json As String = response.Content.ReadAsStringAsync().Result
                _errors = JsonConvert.DeserializeObject(Of Conterrors)(json)
                resultnh = New ResultDTO() With {
                    .Isok = False,
                    .msg = _errors.errors(0).msg
                }
            End If

        Catch ex As WebException
            Debug.WriteLine($" ex:{ex.Message}")
            resultnh = New ResultDTO() With {
                .msg = $" error catch ex:{ex.Message}",
                .Isok = False
            }
        End Try

    End Function

End Class


Public Class Conterrors
    Public Property errors() As errors()
End Class

Public Class errors
    Public Property value() As String
    Public Property msg() As String
    Public Property param() As String
    Public Property location() As String
End Class

Public Class ResultDTO
    Public Property msg() As String
    Public Property Isok() As Boolean

    Public Sub New()
        msg = "conexion fallida"
        Isok = False
    End Sub

End Class

Public Class ListaBitacoraDTO
    Public Property msg() As String
    Public Property bitacoras() As List(Of BitacoraDTO)
End Class

Public Class BitacoraDTO
    Public Property idMAC() As String
    Public Property iPUsuario() As String
    Public Property usuario() As String
    Public Property comando() As String
    Public Property Isok() As Boolean
    Public Property __v() As Integer
    Public Property _id() As String
End Class

Public Class ListaInscripcionDTO
    Public Property msg() As String
    Public Property Inscripcions() As List(Of InscripcionDTO)
End Class

Public Class InscripcionDTO
    Public Property idMAC() As String
    Public Property iPUsuario() As String
    Public Property usuario() As String
    Public Property EmpresaEjecutable() As String
    Public Property IsPermitido() As Boolean
    Public Property __v() As Integer

    Public Property _id() As String
End Class



Public Class InfoSystema_validar
    '0)propieades
    Public Property disco As String
    Public Property mac As String ' realmente es la dirección o número de la placa madre
    Public Property IpPublic As String
    Public Property usuario As String
    Public Property ruta As String
    Public Property caso As String

    '1)Constructor
    Public Sub New()
    End Sub

    Public Sub New(ruta As String, caso As String)
        Me.ruta = ruta
        Me.caso = caso
    End Sub

    Public Function Ejecutar() As Boolean
        Try
            If Not M1_EjecutarInfoSistem() Then Return False
            If Not M2_ObtenerIp() Then Return False
            If Not M3_getMacAddress3() Then Return False
        Catch ex As Exception
            Debug.WriteLine($" error en 'InfoSystema2'" & ex.Message)
            Return False
        End Try
        Return True
    End Function

    ' Métodos Sistema

    Public Function M1_EjecutarInfoSistem() As Boolean
        Try
            ' OBTENER INFO DISCO DURO
            Dim consultaSQLArquitectura As String = "SELECT * FROM Win32_Processor"
            Dim objArquitectura As New ManagementObjectSearcher(consultaSQLArquitectura)
            Dim serialDD As New ManagementObject("Win32_PhysicalMedia='\\.\PHYSICALDRIVE0'")

            For Each prop As PropertyData In serialDD.Properties
                Console.WriteLine("{0}: {1}", prop.Name, prop.Value)
            Next

            disco = serialDD.GetPropertyValue("SerialNumber").ToString()
            usuario = Environment.UserName
        Catch ex As Exception
            Debug.WriteLine($" error en 'M1_EjecutarInfoSistem'" & ex.Message)
            Return False
        End Try
        Return True
    End Function

    Public Function M2_ObtenerIp() As Boolean
        Try
            Dim ExternalIP As String
            ExternalIP = New WebClient().DownloadString("http://checkip.dyndns.org/")
            IpPublic = New Regex("\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}").Matches(ExternalIP)(0).ToString()
            Console.WriteLine("Ippublica:{0}", IpPublic)
        Catch ex As Exception
            ' no se devuelve false porque en caso de no haber conexión internet, sale de rutina principal sin calcular mac
            ' la mac es la importante
            Debug.WriteLine($" error en 'M2_obtenerIp'" & ex.Message)
            IpPublic = ""
        End Try
        Return True
    End Function

    Public Function M3_getMacAddress3() As Boolean
        Try
            Dim MOS As New ManagementObjectSearcher("Select * From Win32_BaseBoard")
            For Each getserial As ManagementObject In MOS.Get()
                mac = getserial("SerialNumber").ToString()  ' realmente placa madre
            Next
        Catch ex As Exception
            Return False
        End Try
        Return True
    End Function

End Class