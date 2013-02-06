Option Explicit On
Option Strict On

'Imports System.Drawing
'Imports System.Drawing.Imaging
Imports System.Drawing.Drawing2D
'Imports System.Drawing.Bitmap
'Imports System.Windows.Media.Imaging
Imports System.Configuration
Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions

Public Class CRMPhotoImportHelper
	<System.Runtime.InteropServices.DllImport("gdi32.dll")> _
	Public Shared Function DeleteObject(ByVal hObject As IntPtr) As Boolean
	End Function

	Private _ProcessStatus As ProcessStatusType = ProcessStatusType.Stopped

	Private Property ProcessStatus() As ProcessStatusType
		Get
			Return _ProcessStatus
		End Get
		Set(ByVal value As ProcessStatusType)
			_ProcessStatus = value
		End Set
	End Property

	Public Enum ProcessStatusType
		Stopped
		Running
		Paused
	End Enum

	Private _TargetWidth As Integer = 0
	Private _TargetHeight As Integer = 0
	Private _PaddingTop As Decimal = 0
	Private _PaddingSide As Decimal = 0


	Private Sub CRMPhotoImportHelper_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
		e.Cancel = False
		If Me._ProcessStatus <> ProcessStatusType.Stopped Then
			If MsgBox("Headshots are currently being processed.  Are you sure you want to Exit?", MsgBoxStyle.YesNo, Me.Text) <> MsgBoxResult.Yes Then
				e.Cancel = True
			Else
				Me._ProcessStatus = ProcessStatusType.Stopped
			End If
		End If
	End Sub


	Private Sub CRMPhotoImportHelper_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

		Me.lblChildID.Text = ""
		Me.lblStatus.Text = ""

		Me.txtMaxPhotos.Text = "5000"
		Me.txtBlackbaudPhotoLocation.Text = "\\bbhcifs01\clientdata\Production\21195P\ChildPhotos" '"\\bbhcifs01\clientdata\staging\21195D\ChildPhotos" 

		SetButtons()


	End Sub


	Private Sub OpenFileToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
		Dim dlg As FolderBrowserDialog

		dlg = New FolderBrowserDialog()
		dlg.SelectedPath = txtSourceFolderName.Text
		If dlg.ShowDialog() = Windows.Forms.DialogResult.OK Then
			txtSourceFolderName.Text = dlg.SelectedPath
		End If
	End Sub

	Private Sub cmdProcessFolder_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdProcessFolder.Click


		Dim parmsValid As Boolean = True

		Dim pictureTitle As String = String.Empty
		Dim maxFilesToProcess As Integer = 0
		Dim fileLocation As String = Me.txtBlackbaudPhotoLocation.Text

		' Check a (USA style) telephone number
		Dim validFileNameFullbodyRegEx As New Regex("^[cC][0-9][0-9][0-9][0-9][0-9][0-9]$")
		Dim validFileNameHeadshotRegEx As New Regex("^[cC][0-9][0-9][0-9][0-9][0-9][0-9]-H$")

		Dim ProcessedSubFolder As String = "_Import"
		Dim ExceptionSubFolder As String = "_Exceptions"

		Dim NumberOfHeadshots As Integer = 0
		Dim NumberOfFullBody As Integer = 0

		Dim NumberOfFilesProcessed As Integer = 0
		Dim NumberOfInvalidFiles As Integer = 0

		Dim newFileName As String = String.Empty
		Dim ProcessedFiles As String = String.Empty



		If cbPictureTitle.Text = String.Empty Then
			MsgBox("Missing Picture Title", MsgBoxStyle.Critical, "Please enter a Picture Title")
			parmsValid = False
		Else
			pictureTitle = cbPictureTitle.Text	' 2012 Child Profile"	'"""BLACKBAUDHOST\Import21195P,21195P@imp123PWd"""	 '"2012 Health Update"
		End If

		If IsNumeric(Me.txtMaxPhotos.Text) Then
			maxFilesToProcess = CInt(Me.txtMaxPhotos.Text)
		Else
			MsgBox("Please enter a number for the Maximum number of photos to process in this batch.", MsgBoxStyle.Critical, Me.Text & " - Processing Error")
			parmsValid = False
		End If

		If Not Directory.Exists(Me.txtSourceFolderName.Text) Then
			MsgBox("The Source Folder does not exist.  Please select a valid folder.", MsgBoxStyle.Critical, Me.Text & " - Processing Error")
			parmsValid = False
		End If


		If parmsValid Then
			Dim PathName As String = txtSourceFolderName.Text

			If Not PathName.EndsWith("\") Then
				PathName &= "\"
			End If

			Dim importFilePathName As String = txtImportFileFolderName.Text

			If Not importFilePathName.EndsWith("\") Then
				importFilePathName &= "\"
			End If

			Dim ExceptionPath As String = PathName & ExceptionSubFolder
			Dim ProcessedPath As String = PathName & ProcessedSubFolder

			If Not Directory.Exists(ProcessedPath) Then
				Directory.CreateDirectory(ProcessedPath)
			End If

			If Not Directory.Exists(ExceptionPath) Then
				Directory.CreateDirectory(ExceptionPath)
			End If


			Dim importFile As StreamWriter = New StreamWriter(importFilePathName & "ChildPhotoImport-" & DateTime.Now.ToString("yyyyMMddHHmmss") & ".csv")
			importFile.WriteLine("ChildLookupID,AttachmentType,PictureTitle,FileName,FileLocation")

			Dim exceptionFile As StreamWriter = New StreamWriter(importFilePathName & "ChildPhotoImport-Exception-" & DateTime.Now.ToString("yyyyMMddHHmmss") & ".csv")
			exceptionFile.WriteLine("FileName,Exception")

			cmdProcessFolder.Enabled = False

			Me.ProcessStatus = ProcessStatusType.Running

			SetButtons()

			lblStatus.Text = "Adding to import file..."

			txtSourceFolderName.Focus()
			txtSourceFolderName.SelectionLength = 0
			rtbMessage.Text = ""

			For Each ImageFileWithPath As String In IO.Directory.GetFiles(PathName)
				Application.DoEvents()

				Do While Me.ProcessStatus <> ProcessStatusType.Running
					Application.DoEvents()
					If Me.ProcessStatus = ProcessStatusType.Stopped Then
						SetButtons()
						lblStatus.Text = "Import file creation Cancelled."
						Exit Sub
					End If
				Loop


				If Path.GetExtension(ImageFileWithPath).ToLower <> ".jpg" Then
					' skip thumbs.db file
					If Path.GetFileName(ImageFileWithPath).ToLower <> "thumbs.db" Then
						Rename(ImageFileWithPath, ExceptionPath & "\" & Path.GetFileName(ImageFileWithPath))
						NumberOfInvalidFiles += 1
						exceptionFile.WriteLine(Path.GetFileName(ImageFileWithPath) & "," & "Not a jpg file")
						rtbMessage.AppendText(Path.GetFileName(ImageFileWithPath) & vbTab & "Not a jpg file" & vbNewLine)
					End If
				Else
					Application.DoEvents()

					NumberOfFilesProcessed += 1

					lblNumProcessedValue.Text = NumberOfFilesProcessed.ToString


					If validFileNameFullbodyRegEx.IsMatch(Path.GetFileNameWithoutExtension(ImageFileWithPath)) Then
						NumberOfFullBody += 1
						newFileName = Path.GetFileNameWithoutExtension(ImageFileWithPath).ToUpper & Path.GetExtension(ImageFileWithPath)
						Try
							Rename(ImageFileWithPath, ProcessedPath & "\" & newFileName)
							importFile.WriteLine(Path.GetFileNameWithoutExtension(ImageFileWithPath).ToUpper & ",Child Photo - Full Body - Current," & pictureTitle & "," & newFileName & "," & fileLocation) 'ProcessedPath)

						Catch ex As Exception
							NumberOfInvalidFiles += 1

							exceptionFile.WriteLine(Path.GetFileName(ImageFileWithPath) & "," & "File move failed. " & ex.Message)
							rtbMessage.AppendText(Path.GetFileName(ImageFileWithPath) & vbTab & "File move failed. " & ex.Message & vbNewLine)
						End Try


					ElseIf validFileNameHeadshotRegEx.IsMatch(Path.GetFileNameWithoutExtension(ImageFileWithPath)) Then
						NumberOfFullBody += 1
						newFileName = Path.GetFileNameWithoutExtension(ImageFileWithPath).ToUpper & Path.GetExtension(ImageFileWithPath)

						Try
							Rename(ImageFileWithPath, ProcessedPath & "\" & newFileName)
							importFile.WriteLine(Path.GetFileNameWithoutExtension(ImageFileWithPath).ToUpper.Substring(0, 7) & ",Child Photo - Headshot - Current," & pictureTitle & "," & newFileName & "," & fileLocation) 'ProcessedPath)
						Catch ex As Exception
							NumberOfInvalidFiles += 1

							exceptionFile.WriteLine(Path.GetFileName(ImageFileWithPath) & "," & "File move failed. " & ex.Message)
							rtbMessage.AppendText(Path.GetFileName(ImageFileWithPath) & vbTab & "File move failed. " & ex.Message & vbNewLine)
						End Try
					Else
						Rename(ImageFileWithPath, ExceptionPath & "\" & Path.GetFileName(ImageFileWithPath))
						NumberOfInvalidFiles += 1

						exceptionFile.WriteLine(Path.GetFileName(ImageFileWithPath) & "," & "Invalid file name")
						rtbMessage.AppendText(Path.GetFileName(ImageFileWithPath) & vbTab & "Invalid file name" & vbNewLine)

					End If

					lblNumInvalid.Text = NumberOfInvalidFiles.ToString
					rtbMessage.ScrollToCaret()

					Me.Refresh()

					If NumberOfFilesProcessed = maxFilesToProcess Then
						Exit For
					End If
					Application.DoEvents()

				End If
			Next

			importFile.Close()
			exceptionFile.Close()

			Me.ProcessStatus = ProcessStatusType.Stopped

			SetButtons()

			lblStatus.Text = "Import file creation completed."

		End If
	End Sub


	Private Sub SetButtons()
		If Me.ProcessStatus = ProcessStatusType.Running Then
			cmdProcessFolder.Enabled = False
			cmdCancel.Visible = True
			cmdPause.Visible = True
		Else
			cmdProcessFolder.Enabled = True
			cmdCancel.Visible = False
			cmdPause.Visible = False
		End If
	End Sub


	'Private Function ResizeImage(ByVal imgToResize As Image, ByVal size As Size) As Image
	'	Dim sourceWidth As Integer = imgToResize.Width
	'	Dim sourceHeight As Integer = imgToResize.Height

	'	Dim nPercent As Decimal = 0
	'	Dim nPercentW As Decimal = 0
	'	Dim nPercentH As Decimal = 0

	'	nPercentW = (CType(size.Width, Decimal) / sourceWidth)
	'	nPercentH = (CType(size.Height, Decimal) / sourceHeight)

	'	If (nPercentH < nPercentW) Then
	'		nPercent = nPercentH
	'	Else
	'		nPercent = nPercentW
	'	End If

	'	Dim destWidth As Integer = CType((sourceWidth * nPercent), Integer)
	'	Dim destHeight As Integer = CType((sourceHeight * nPercent), Integer)

	'	Dim b As Bitmap = New Bitmap(destWidth, destHeight)
	'	Dim g As Graphics = Graphics.FromImage(b)
	'	g.InterpolationMode = InterpolationMode.HighQualityBicubic

	'	g.DrawImage(imgToResize, 0, 0, destWidth, destHeight)
	'	g.Dispose()

	'	Return b
	'End Function

	Private Sub cmdBrowseFolder_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdBrowseFolder.Click
		Dim dlg As FolderBrowserDialog

		dlg = New FolderBrowserDialog()
		dlg.SelectedPath = txtSourceFolderName.Text
		If dlg.ShowDialog() = Windows.Forms.DialogResult.OK Then
			txtSourceFolderName.Text = dlg.SelectedPath
		End If

		txtSourceFolderName.Focus()
	End Sub

	Private Sub cmdCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCancel.Click
		If Me.ProcessStatus = ProcessStatusType.Stopped Then
			Application.Exit()
		Else
			If MsgBox("Are you sure you want to stop creating import file?", MsgBoxStyle.YesNo, Me.Text) = MsgBoxResult.Yes Then
				Me.ProcessStatus = ProcessStatusType.Stopped
			End If
		End If
	End Sub

	Private Sub cmdPause_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdPause.Click
		If Me.ProcessStatus = ProcessStatusType.Running Then
			Me.ProcessStatus = ProcessStatusType.Paused
			cmdPause.Text = "Resume"
			lblStatus.Text = "Paused..."
		Else
			Me.ProcessStatus = ProcessStatusType.Running
			cmdPause.Text = "Pause"
			lblStatus.Text = "Adding to import file...."
		End If
	End Sub

	Public Sub New()

		' This call is required by the Windows Form Designer.
		InitializeComponent()

		' Add any initialization after the InitializeComponent() call.


	End Sub

	Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
		Dim dlg As FolderBrowserDialog

		dlg = New FolderBrowserDialog()
		dlg.SelectedPath = txtImportFileFolderName.Text
		If dlg.ShowDialog() = Windows.Forms.DialogResult.OK Then
			txtImportFileFolderName.Text = dlg.SelectedPath
		End If

		txtSourceFolderName.Focus()
	End Sub

End Class
