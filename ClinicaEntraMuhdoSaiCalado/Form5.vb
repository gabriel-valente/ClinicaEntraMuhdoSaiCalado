﻿Imports System.Data.SqlClient
Imports System.Text.RegularExpressions
Imports System.Net.Mail

Public Class Form5
    Dim conexao As SqlConnection
    ReadOnly connectionString As String = "Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + Application.StartupPath + "\Clinica.mdf;Integrated Security=True;"
    Dim reader As SqlDataReader

    Dim ListaEspecialidade As List(Of Especialidades) = New List(Of Especialidades)()

    Dim ButaoGuardar As Boolean 'True = Adicionar, False = Editar
    Dim CodigoEspecialidade As Integer = 0

    Dim EspecialidadeValido As Boolean = False

    Private Sub Btn_Close_Click(sender As Object, e As EventArgs) Handles Btn_Close.Click
        If LastForm = 2 Then
            Form4.Show()
        ElseIf LastForm = 1 Then
            Form2.Show()
        End If

        Me.Close()
    End Sub

    Private Sub Form5_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        conexao = New SqlConnection(connectionString)

        Dados()
    End Sub

    Private Sub Lst_Especialidades_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Lst_Especialidades.SelectedIndexChanged
        Btn_EditarEspecialidade.Enabled = False
        Btn_EditarEspecialidade.BackgroundImage = My.Resources.ResourceManager.GetObject("editgray")
        Tb_Especialidade.Text = Nothing
        Tb_Especialidade.ReadOnly = True

        If Lst_Especialidades.SelectedIndices.Count >= 1 Then
            Btn_EditarEspecialidade.Enabled = True
            Btn_EditarEspecialidade.BackgroundImage = My.Resources.ResourceManager.GetObject("edit")
            Dim especialidade As Especialidades = ListaEspecialidade.Find(Function(es) es.Especialidade = Lst_Especialidades.FocusedItem.SubItems.Item(0).Text)

            CodigoEspecialidade = especialidade.KeyEspecialidade
            Tb_Especialidade.Text = especialidade.Especialidade
        ElseIf Lst_Especialidades.SelectedIndices.Count = 0 Then
            Btn_EditarEspecialidade.Enabled = False
            Btn_EditarEspecialidade.BackgroundImage = My.Resources.ResourceManager.GetObject("editgray")
            Tb_Especialidade.Text = Nothing
        End If
    End Sub

    Private Sub Btn_AdicionarUtente_Click(sender As Object, e As EventArgs) Handles Btn_AdicionarEspecialidade.Click
        Tb_Especialidade.Text = Nothing
        ButaoGuardar = True
        Btn_Cancel.Enabled = True
        Btn_Cancel.BackgroundImage = My.Resources.ResourceManager.GetObject("cancel")
        Btn_AdicionarEspecialidade.Enabled = False
        Btn_AdicionarEspecialidade.BackgroundImage = My.Resources.ResourceManager.GetObject("addgray")
        Btn_EditarEspecialidade.Enabled = False
        Btn_EditarEspecialidade.BackgroundImage = My.Resources.ResourceManager.GetObject("editgray")
        Lbl_SaveMethod.Text = "Inserir Especialidade"
        SwitchFields()
    End Sub

    Private Sub Btn_EditarUtente_Click(sender As Object, e As EventArgs) Handles Btn_EditarEspecialidade.Click
        Lst_Especialidades.Enabled = False
        CodigoEspecialidade = ListaEspecialidade.Find(Function(es) es.Especialidade = Lst_Especialidades.FocusedItem.SubItems.Item(0).Text).KeyEspecialidade

        ButaoGuardar = False
        Btn_Cancel.Enabled = True
        Btn_Cancel.BackgroundImage = My.Resources.ResourceManager.GetObject("cancel")
        Btn_AdicionarEspecialidade.Enabled = False
        Btn_AdicionarEspecialidade.BackgroundImage = My.Resources.ResourceManager.GetObject("addgray")
        Btn_EditarEspecialidade.Enabled = False
        Btn_EditarEspecialidade.BackgroundImage = My.Resources.ResourceManager.GetObject("editgray")
        Lbl_SaveMethod.Text = "Editar Especialidade"
        SwitchFields()
    End Sub

    Private Sub Btn_Cancel_Click(sender As Object, e As EventArgs) Handles Btn_Cancel.Click
        ButaoGuardar = Nothing
        Btn_EditarEspecialidade.Enabled = False
        Btn_EditarEspecialidade.BackgroundImage = My.Resources.ResourceManager.GetObject("editgray")
        Tb_Especialidade.Text = Nothing
        Tb_Especialidade.ReadOnly = True
        Lbl_ErroEspecialidade.Text = Nothing
        Btn_AdicionarEspecialidade.Enabled = True
        Btn_AdicionarEspecialidade.BackgroundImage = My.Resources.ResourceManager.GetObject("add")
        Btn_EditarEspecialidade.Enabled = False
        Btn_EditarEspecialidade.BackgroundImage = My.Resources.ResourceManager.GetObject("editgray")
        Lst_Especialidades.Enabled = True
        Lbl_SaveMethod.Text = "Sem Metodo Definido"
        Btn_Cancel.Enabled = False
        Btn_Cancel.BackgroundImage = My.Resources.ResourceManager.GetObject("cancelgray")
    End Sub

    Sub SwitchFields()
        If Tb_Especialidade.ReadOnly = True Then
            Tb_Especialidade.ReadOnly = False
        Else
            Tb_Especialidade.ReadOnly = True
        End If
    End Sub

    Sub Dados()
        Dim queryEspecialidades As SqlCommand = New SqlCommand("SELECT Key_Especialidade, Especialidade FROM Especialidade")
        conexao.Open()

        queryEspecialidades.Connection = conexao

        reader = queryEspecialidades.ExecuteReader()

        Lst_Especialidades.Items.Clear()
        ListaEspecialidade.Clear()

        While reader.Read()
            ListaEspecialidade.Add(New Especialidades With {
                            .KeyEspecialidade = reader("Key_Especialidade"),
                            .Especialidade = reader("Especialidade")
                            })

            Lst_Especialidades.Items.Add(New ListViewItem({reader("Especialidade")}))
        End While

        reader.Close()
        conexao.Close()
    End Sub

    Sub Validar()
        If EspecialidadeValido = True Then
            Btn_Guardar.Enabled = True
            Btn_Guardar.BackgroundImage = My.Resources.ResourceManager.GetObject("diskette")
        Else
            Btn_Guardar.Enabled = False
            Btn_Guardar.BackgroundImage = My.Resources.ResourceManager.GetObject("diskettegray")
        End If
    End Sub

    Private Sub Tb_Especialidade_TextChanged(sender As Object, e As EventArgs) Handles Tb_Especialidade.TextChanged
        EspecialidadeValido = False

        Tb_Especialidade.Text = Tb_Especialidade.Text.TrimStart()
        Dim especialidade() As Char = Tb_Especialidade.Text.ToCharArray()

        If Not String.IsNullOrEmpty(Tb_Especialidade.Text.TrimStart()) AndAlso Not Char.IsUpper(especialidade(0)) Then
            especialidade(0) = Char.ToUpper(especialidade(0))
            Tb_Especialidade.Text = New String(especialidade)
            Tb_Especialidade.SelectionStart = Tb_Especialidade.Text.Length
        End If

        For index = 0 To especialidade.Length - 1
            If index >= 1 AndAlso Char.IsWhiteSpace(especialidade(index)) AndAlso Char.IsWhiteSpace(especialidade(index - 1)) Then
                Tb_Especialidade.Text = Tb_Especialidade.Text.Remove(index, 1)
                Array.Clear(especialidade, 0, especialidade.Length)
                especialidade = Tb_Especialidade.Text.Trim().ToCharArray()
                Tb_Especialidade.SelectionStart = index + 1

                Exit For
            End If
        Next

        If especialidade.Length >= 4 Then
            EspecialidadeValido = True
        End If

        Validar()
    End Sub

    Private Sub Btn_Guardar_Click(sender As Object, e As EventArgs) Handles Btn_Guardar.Click
        If ButaoGuardar = True Then
            Dim queryInserir As SqlCommand = New SqlCommand("INSERT INTO Especialidade (Especialidade) VALUES (@Especialidade)")
            conexao.Open()
            queryInserir.Connection = conexao
            queryInserir.Parameters.AddWithValue("@Especialidade", Tb_Especialidade.Text.Trim())

            queryInserir.ExecuteNonQuery()
            queryInserir.Parameters.Clear()
        Else
            Dim queryEditar As SqlCommand = New SqlCommand("UPDATE Especialidade SET Especialidade = @Especialidade WHERE Key_Especialidade = @KeyEspecialidade")
            conexao.Open()
            queryEditar.Connection = conexao
            queryEditar.Parameters.AddWithValue("@Especialidade", Tb_Especialidade.Text.Trim())
            queryEditar.Parameters.AddWithValue("@KeyEspecialidade", CodigoEspecialidade)

            queryEditar.ExecuteNonQuery()
            queryEditar.Parameters.Clear()
        End If

        Btn_Cancel.Enabled = False
        Btn_Cancel.BackgroundImage = My.Resources.ResourceManager.GetObject("cancelgray")
        Btn_AdicionarEspecialidade.Enabled = True
        Btn_AdicionarEspecialidade.BackgroundImage = My.Resources.ResourceManager.GetObject("adduser")
        Btn_EditarEspecialidade.Enabled = False
        Btn_EditarEspecialidade.BackgroundImage = My.Resources.ResourceManager.GetObject("editusergray")
        Btn_Guardar.Enabled = False
        Btn_Guardar.BackgroundImage = My.Resources.ResourceManager.GetObject("diskettegray")
        Tb_Especialidade.Text = Nothing
        Lbl_SaveMethod.Text = "Sem Metodo Definido"
        SwitchFields()
        Lst_Especialidades.Enabled = True

        conexao.Close()
        Dados()
    End Sub
End Class